using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropMan.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTenantId1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "UnitId",
                table: "PropertyTenant",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId1",
                table: "PropertyTenant",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ReminderSent",
                table: "Invoice",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "AuditLog",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateTable(
                name: "TenantCredits",
                columns: table => new
                {
                    TenantCreditId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyTenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantCredits", x => x.TenantCreditId);
                    table.ForeignKey(
                        name: "FK_TenantCredits_PropertyTenant_PropertyTenantId",
                        column: x => x.PropertyTenantId,
                        principalTable: "PropertyTenant",
                        principalColumn: "PropertyTenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyTenant_TenantId1",
                table: "PropertyTenant",
                column: "TenantId1",
                unique: true,
                filter: "[TenantId1] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TenantCredits_PropertyTenantId",
                table: "TenantCredits",
                column: "PropertyTenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyTenant_Tenant_TenantId1",
                table: "PropertyTenant",
                column: "TenantId1",
                principalTable: "Tenant",
                principalColumn: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyTenant_Tenant_TenantId1",
                table: "PropertyTenant");

            migrationBuilder.DropTable(
                name: "TenantCredits");

            migrationBuilder.DropIndex(
                name: "IX_PropertyTenant_TenantId1",
                table: "PropertyTenant");

            migrationBuilder.DropColumn(
                name: "TenantId1",
                table: "PropertyTenant");

            migrationBuilder.DropColumn(
                name: "ReminderSent",
                table: "Invoice");

            migrationBuilder.AlterColumn<Guid>(
                name: "UnitId",
                table: "PropertyTenant",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "AuditLog",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
