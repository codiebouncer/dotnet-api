using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropMan.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantCreditTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.CompanyId);
                });

            migrationBuilder.CreateTable(
                name: "PaymentPlan",
                columns: table => new
                {
                    PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Frequency = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentPlan", x => x.PlanId);
                    table.ForeignKey(
                        name: "FK_PaymentPlan_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                });

            migrationBuilder.CreateTable(
                name: "Tenant",
                columns: table => new
                {
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    NationalId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant", x => x.TenantId);
                    table.ForeignKey(
                        name: "FK_Tenant_Company",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_User_Company",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                });

            migrationBuilder.CreateTable(
                name: "AuditLog",
                columns: table => new
                {
                    LogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_AuditLog_Company",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_AuditLog_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Property",
                columns: table => new
                {
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalUnits = table.Column<int>(type: "int", nullable: false),
                    OccuppiedUnits = table.Column<int>(type: "int", nullable: false),
                    VacantUnits = table.Column<int>(type: "int", nullable: false),
                    CostPrice = table.Column<decimal>(name: "Cost Price", type: "decimal(18,2)", nullable: false),
                    SellingPrice = table.Column<decimal>(name: "Selling Price", type: "decimal(18,2)", nullable: true),
                    OccupancyStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Property", x => x.PropertyId);
                    table.ForeignKey(
                        name: "FK_Property_Company",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_Property_User",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Picture",
                columns: table => new
                {
                    PictureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    DateUploaded = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Picture", x => x.PictureId);
                    table.ForeignKey(
                        name: "FK_Picture_Property",
                        column: x => x.PropertyId,
                        principalTable: "Property",
                        principalColumn: "PropertyId");
                });

            migrationBuilder.CreateTable(
                name: "PropertyUnit",
                columns: table => new
                {
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RentPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyUnit", x => x.UnitId);
                    table.ForeignKey(
                        name: "FK_PropertyUnit_Company",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_PropertyUnit_Property",
                        column: x => x.PropertyId,
                        principalTable: "Property",
                        principalColumn: "PropertyId");
                });

            migrationBuilder.CreateTable(
                name: "PropertyTenant",
                columns: table => new
                {
                    PropertyTenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    installmentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    totalInstallments = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    AmountDue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InitialDeposit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastPaymentDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsPrimaryTenant = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyTenant", x => x.PropertyTenantId);
                    table.ForeignKey(
                        name: "FK_PropertyTenant_Company",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_PropertyTenant_PaymentPlan",
                        column: x => x.PlanId,
                        principalTable: "PaymentPlan",
                        principalColumn: "PlanId");
                    table.ForeignKey(
                        name: "FK_PropertyTenant_Property",
                        column: x => x.PropertyId,
                        principalTable: "Property",
                        principalColumn: "PropertyId");
                    table.ForeignKey(
                        name: "FK_PropertyTenant_PropertyUnit",
                        column: x => x.UnitId,
                        principalTable: "PropertyUnit",
                        principalColumn: "UnitId");
                    table.ForeignKey(
                        name: "FK_PropertyTenant_Tenant",
                        column: x => x.TenantId,
                        principalTable: "Tenant",
                        principalColumn: "TenantId");
                });

            migrationBuilder.CreateTable(
                name: "Invoice",
                columns: table => new
                {
                    InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyTenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastPaymentDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveInvoice", x => x.InvoiceId);
                    table.ForeignKey(
                        name: "FK_ActiveInvoice_Company",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_ActiveInvoice_PropertyTenant",
                        column: x => x.PropertyTenantId,
                        principalTable: "PropertyTenant",
                        principalColumn: "PropertyTenantId");
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    NotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyTenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MessageContent = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    SentTo = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_Notification_Company",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_Notification_PropertyTenant",
                        column: x => x.PropertyTenantId,
                        principalTable: "PropertyTenant",
                        principalColumn: "PropertyTenantId");
                });

            migrationBuilder.CreateTable(
                name: "Repairs",
                columns: table => new
                {
                    RepairId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyTenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repairs", x => x.RepairId);
                    table.ForeignKey(
                        name: "FK_Repairs_Company",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_Repairs_PropertyTenant",
                        column: x => x.PropertyTenantId,
                        principalTable: "PropertyTenant",
                        principalColumn: "PropertyTenantId");
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyTenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payments_ActiveInvoice",
                        column: x => x.InvoiceId,
                        principalTable: "Invoice",
                        principalColumn: "InvoiceId");
                    table.ForeignKey(
                        name: "FK_Payments_Company",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_Payments_PropertyTenant",
                        column: x => x.PropertyTenantId,
                        principalTable: "PropertyTenant",
                        principalColumn: "PropertyTenantId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_CompanyId",
                table: "AuditLog",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_UserId",
                table: "AuditLog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_CompanyId",
                table: "Invoice",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_PropertyTenantId",
                table: "Invoice",
                column: "PropertyTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_CompanyId",
                table: "Notification",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_PropertyTenantId",
                table: "Notification",
                column: "PropertyTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentPlan_CompanyId",
                table: "PaymentPlan",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CompanyId",
                table: "Payments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_InvoiceId",
                table: "Payments",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PropertyTenantId",
                table: "Payments",
                column: "PropertyTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Picture_PropertyId",
                table: "Picture",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_Property_CompanyId",
                table: "Property",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Property_CreatedBy",
                table: "Property",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyTenant_CompanyId",
                table: "PropertyTenant",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyTenant_PlanId",
                table: "PropertyTenant",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyTenant_PropertyId",
                table: "PropertyTenant",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyTenant_TenantId",
                table: "PropertyTenant",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyTenant_UnitId",
                table: "PropertyTenant",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyUnit_CompanyId",
                table: "PropertyUnit",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyUnit_PropertyId",
                table: "PropertyUnit",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_Repairs_CompanyId",
                table: "Repairs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Repairs_PropertyTenantId",
                table: "Repairs",
                column: "PropertyTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenant_CompanyId",
                table: "Tenant",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_User_CompanyId",
                table: "User",
                column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLog");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Picture");

            migrationBuilder.DropTable(
                name: "Repairs");

            migrationBuilder.DropTable(
                name: "Invoice");

            migrationBuilder.DropTable(
                name: "PropertyTenant");

            migrationBuilder.DropTable(
                name: "PaymentPlan");

            migrationBuilder.DropTable(
                name: "PropertyUnit");

            migrationBuilder.DropTable(
                name: "Tenant");

            migrationBuilder.DropTable(
                name: "Property");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Company");
        }
    }
}
