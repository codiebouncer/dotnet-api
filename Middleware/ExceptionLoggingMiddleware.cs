using PropMan.Models;

public class ExceptionLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
{
    try
    {
        var errRepo = context.RequestServices.GetRequiredService<IErrorLogRepository>();

        await errRepo.LogAsync(new ErrorLog
        {
            Message = ex.Message,
            StackTrace = ex.StackTrace ?? "",
            Path = context.Request.Path,
            Timestamp = DateTime.UtcNow
        });
    }
    catch (Exception logEx)
    {
        Console.WriteLine("Failed to log error → " + logEx.Message);
    }

    context.Response.StatusCode = 500;
    await context.Response.WriteAsJsonAsync(new
    {
        status = 500,
        message = "An unexpected error occurred."
    });
}

    }
}
