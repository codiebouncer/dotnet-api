using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using PropMan.Models;
using Propman.Services.UserContext;
using PropMan.Services.UserContext;
using PropMan.Extensions;
using Hangfire;
using Propman.Services;
using Propman.Logic;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// ✅ 1. Add Controllers
builder.Services.AddControllers();

// ✅ 2. Add Swagger + JWT Bearer Support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PropMan API",
        Version = "v1",
        Description = "API documentation for PropMan backend"
    });

    // 🔐 Add JWT authentication in Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// ✅ 3. Configure Database

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ 4. Register Repositories and Services
builder.Services.AddProjectServices();
builder.Services.AddHttpContextAccessor(); // Required for UserContext
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddScoped<IPropAssLogic, PropAssLogic>();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});


// ✅ 5. Configure JWT Authentication
var tokenKey = builder.Configuration.GetValue<string>("AppSettings:Token");
builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddHangfireServer();
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})

.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Disable in dev
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddCors(options => options.AddPolicy(name: "Allow frontend",
    policy =>
    {
        policy.AllowAnyOrigin() 
            .AllowAnyMethod() 
            .AllowAnyHeader();
    }));

 //Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ✅ 6. Build Application
var app = builder.Build();
app.UseMiddleware<ExceptionLoggingMiddleware>();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
    ),
    RequestPath = ""
});


app.UseHangfireDashboard("/jobs");

// ✅ 7. Middleware Order is CRUCIAL

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "PropMan API v1");
        options.RoutePrefix = string.Empty; // open Swagger at root (e.g. localhost:5000)
    });
}
using (var scope = app.Services.CreateScope())
{
    var jobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    jobManager.AddOrUpdate<IInvoiceJobService>(
        "check-overdue-invoices",
        job => job.CheckAndNotifyOverdueInvoices(),
        Cron.Daily
    );
    jobManager.AddOrUpdate<IInvoiceJobService>(
        "send-payment-reminders",
        job => job.SendPaymentReminders(),
        Cron.Daily
    );
    jobManager.AddOrUpdate<IInvoiceJobService>(
        "send-payment-reminders",
        job => job.GenerateInvoice(),
        Cron.Daily
    );

}


app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
// Authentication MUST come before Authorization


app.UseAuthentication();
app.UseAuthorization();

// ✅ 8. Swagger setup (both Dev & optional Prod)


// ✅ 9. Map Controllers
app.MapControllers();

// ✅ 10. Run App
app.Run();
