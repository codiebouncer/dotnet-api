using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PropMan.Models;

public partial class DataContext : DbContext
{
    public DataContext()
    {
    }

    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentPlan> PaymentPlans { get; set; }

    public virtual DbSet<Picture> Pictures { get; set; }
    public DbSet<TenantCredit> TenantCredits { get; set; }

    public virtual DbSet<Property> Properties { get; set; }

    public virtual DbSet<PropertyTenant> PropertyTenants { get; set; }

    public virtual DbSet<PropertyUnit> PropertyUnits { get; set; }

    public virtual DbSet<Repair> Repairs { get; set; }

    public virtual DbSet<Tenant> Tenants { get; set; }

    public virtual DbSet<User> Users { get; set; }
    public DbSet<ErrorLog> ErrorLogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost,1433;Database=PropertyManagementDB;User Id=sa;Password=StrongP@ssword123;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.LogId);

            entity.ToTable("AuditLog");

            entity.Property(e => e.LogId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Description).IsUnicode(false);

            entity.HasOne(d => d.Company).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AuditLog_Company");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AuditLog_User");
        });
         modelBuilder.Entity<ErrorLog>(entity =>
        {
            entity.ToTable("ErrorLogs");

            entity.HasKey(e => e.ErrorLogId);

            entity.Property(e => e.Message).IsRequired();
            entity.Property(e => e.StackTrace);
            entity.Property(e => e.Path).HasMaxLength(500);
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("Company");

            entity.Property(e => e.CompanyId).ValueGeneratedNever();
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK_ActiveInvoice");

            entity.ToTable("Invoice");

            entity.Property(e => e.InvoiceId).ValueGeneratedNever();
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.AmountPaid).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.InvoiceDate).HasColumnType("datetime");
            entity.Property(e => e.LastPaymentDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Company).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ActiveInvoice_Company");

            entity.HasOne(d => d.PropertyTenant).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.PropertyTenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ActiveInvoice_PropertyTenant");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notification");

            entity.Property(e => e.NotificationId).ValueGeneratedNever();
            entity.Property(e => e.MessageContent).IsUnicode(false);
            entity.Property(e => e.MessageType).HasMaxLength(50);
            entity.Property(e => e.SentAt).HasColumnType("datetime");

            entity.HasOne(d => d.Company).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notification_Company");

            entity.HasOne(d => d.PropertyTenant).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.PropertyTenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notification_PropertyTenant");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.Property(e => e.PaymentId).ValueGeneratedNever();
            entity.Property(e => e.AmountPaid).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);

            entity.HasOne(d => d.Company).WithMany(p => p.Payments)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payments_Company");

            entity.HasOne(d => d.Invoice).WithMany(p => p.Payments)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payments_ActiveInvoice");

            entity.HasOne(d => d.PropertyTenant).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PropertyTenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payments_PropertyTenant");
        });

        modelBuilder.Entity<PaymentPlan>(entity =>
        {
            entity.HasKey(e => e.PlanId);

            entity.ToTable("PaymentPlan");

            entity.Property(e => e.PlanId).ValueGeneratedNever();
            entity.Property(e => e.Frequency)
                .HasMaxLength(50)
                .IsUnicode(false);


        });

        modelBuilder.Entity<Picture>(entity =>
        {
            entity.ToTable("Picture");

            entity.Property(e => e.PictureId).ValueGeneratedNever();
            entity.Property(e => e.DateUploaded).HasColumnType("datetime");
            entity.Property(e => e.FileName).IsUnicode(false);

            entity.HasOne(d => d.Property).WithMany(p => p.Pictures)
                .HasForeignKey(d => d.PropertyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Picture_Property");
        });

        modelBuilder.Entity<Property>(entity =>
        {
            entity.ToTable("Property");

            entity.Property(e => e.PropertyId).ValueGeneratedNever();
            entity.Property(e => e.CostPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Cost Price");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.OccupancyStatus).HasMaxLength(50);
            entity.Property(e => e.PropertyType).HasMaxLength(50);
            entity.Property(e => e.SellingPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Selling Price");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Company).WithMany(p => p.Properties)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Property_Company");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Properties)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Property_User");
        });

        modelBuilder.Entity<PropertyTenant>(entity =>
        {
            entity.ToTable("PropertyTenant");

            entity.Property(e => e.PropertyId).ValueGeneratedNever();
            entity.Property(e => e.AmountDue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.LastPaymentDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsFixedLength();

            entity.HasOne(d => d.Company).WithMany(p => p.PropertyTenants)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PropertyTenant_Company");

            entity.HasOne(d => d.Plan).WithMany(p => p.PropertyTenants)
                .HasForeignKey(d => d.PlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PropertyTenant_PaymentPlan");

            entity.HasOne(d => d.Property).WithMany(p => p.PropertyTenants)
                .HasForeignKey(d => d.PropertyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PropertyTenant_Property");

            entity.HasOne(d => d.Tenant).WithMany(p => p.PropertyTenants)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PropertyTenant_Tenant");

            entity.HasOne(d => d.Unit).WithMany(p => p.PropertyTenants)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PropertyTenant_PropertyUnit");
            entity.Property(e => e.InitialDeposit).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.installmentAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.AmountDue).HasColumnType("decimal(18, 2)");
    
        });

        modelBuilder.Entity<PropertyUnit>(entity =>
        {
            entity.HasKey(e => e.UnitId);

            entity.ToTable("PropertyUnit");

            entity.Property(e => e.UnitId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.RentPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UnitName).HasMaxLength(50);

            entity.HasOne(d => d.Company).WithMany(p => p.PropertyUnits)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PropertyUnit_Company");

            entity.HasOne(d => d.Property).WithMany(p => p.PropertyUnits)
                .HasForeignKey(d => d.PropertyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PropertyUnit_Property");
        });

        modelBuilder.Entity<Repair>(entity =>
        {
            entity.Property(e => e.RepairId).ValueGeneratedNever();
            entity.Property(e => e.CompletedDate).HasColumnType("datetime");
            entity.Property(e => e.Cost).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Company).WithMany(p => p.Repairs)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Repairs_Company");

            entity.HasOne(d => d.Property)
        .WithMany(p => p.Repairs)
        .HasForeignKey(d => d.PropertyId)
        .OnDelete(DeleteBehavior.ClientSetNull)
        .HasConstraintName("FK_Repairs_Property");    

        });

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("Tenant");

            entity.Property(e => e.TenantId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.NationalId)
                                                .HasMaxLength(50)
                                                .IsRequired(false);



            entity.HasOne(d => d.Company).WithMany(p => p.Tenants)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tenant_Company");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash).IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Role).HasMaxLength(50);

            entity.HasOne(d => d.Company).WithMany(p => p.Users)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Company");
        });
        modelBuilder.Entity<TenantCredit>(entity =>
        {
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
        });



        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
