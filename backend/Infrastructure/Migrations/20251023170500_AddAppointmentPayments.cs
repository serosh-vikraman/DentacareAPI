using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddAppointmentPayments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE TABLE IF NOT EXISTS `AppointmentPayments` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `AppointmentId` char(36) COLLATE ascii_general_ci NOT NULL,
    `Mode` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    `ReferenceNumber` varchar(256) CHARACTER SET utf8mb4 NULL,
    `TotalAmount` decimal(18,2) NOT NULL,
    `TenantId` char(36) COLLATE ascii_general_ci NOT NULL,
    `BranchId` char(36) COLLATE ascii_general_ci NULL,
    `CreatedUtc` datetime(6) NOT NULL,
    `UpdatedUtc` datetime(6) NULL,
    `IsDeleted` bit(1) NOT NULL,
    CONSTRAINT `PK_AppointmentPayments` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE IF NOT EXISTS `AppointmentPaymentItems` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `AppointmentPaymentId` char(36) COLLATE ascii_general_ci NOT NULL,
    `ServiceId` char(36) COLLATE ascii_general_ci NULL,
    `ServiceName` varchar(256) CHARACTER SET utf8mb4 NOT NULL,
    `Amount` decimal(18,2) NOT NULL,
    CONSTRAINT `PK_AppointmentPaymentItems` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE INDEX IF NOT EXISTS `IX_AppPay_Tenant_App` ON `AppointmentPayments` (`TenantId`, `AppointmentId`);

ALTER TABLE `AppointmentPaymentItems` ADD CONSTRAINT `FK_AppPayItems_AppPay` FOREIGN KEY (`AppointmentPaymentId`) REFERENCES `AppointmentPayments` (`Id`) ON DELETE CASCADE;
", suppressTransaction: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS `AppointmentPaymentItems`;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS `AppointmentPayments`;");
        }
    }
}




