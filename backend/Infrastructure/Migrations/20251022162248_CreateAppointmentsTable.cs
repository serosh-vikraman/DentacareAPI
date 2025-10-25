using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateAppointmentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE TABLE IF NOT EXISTS `Appointments` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `PatientName` varchar(256) CHARACTER SET utf8mb4 NOT NULL,
    `PatientMRNumber` varchar(32) CHARACTER SET utf8mb4 NULL,
    `PatientProfileId` char(36) COLLATE ascii_general_ci NULL,
    `Department` varchar(128) CHARACTER SET utf8mb4 NOT NULL,
    `DoctorName` varchar(256) CHARACTER SET utf8mb4 NOT NULL,
    `DoctorProfileId` char(36) COLLATE ascii_general_ci NULL,
    `ConsultMode` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    `PaymentMode` varchar(32) CHARACTER SET utf8mb4 NULL,
    `Date` date NOT NULL,
    `StartTime` time(6) NOT NULL,
    `EndTime` time(6) NULL,
    `Reason` longtext CHARACTER SET utf8mb4 NULL,
    `Notes` longtext CHARACTER SET utf8mb4 NULL,
    `InvestigationRvg` varchar(256) CHARACTER SET utf8mb4 NULL,
    `InvestigationOpg` bit(1) NULL,
    `InvestigationCeph` bit(1) NULL,
    `InvestigationOcclusal` bit(1) NULL,
    `InvestigationCbct` bit(1) NULL,
    `InvestigationBlood` varchar(256) CHARACTER SET utf8mb4 NULL,
    `InvestigationOthers` longtext CHARACTER SET utf8mb4 NULL,
    `DifferentialDiagnosis` longtext CHARACTER SET utf8mb4 NULL,
    `Diagnosis` longtext CHARACTER SET utf8mb4 NULL,
    `TreatmentPlan` longtext CHARACTER SET utf8mb4 NULL,
    `Status` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    `TenantId` char(36) COLLATE ascii_general_ci NOT NULL,
    `BranchId` char(36) COLLATE ascii_general_ci NULL,
    `CreatedUtc` datetime(6) NOT NULL,
    `UpdatedUtc` datetime(6) NULL,
    `IsDeleted` bit(1) NOT NULL,
    CONSTRAINT `PK_Appointments` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;
", suppressTransaction: false);

            // Create index if it doesn't exist
            migrationBuilder.Sql(@"
SET @idx := (SELECT COUNT(1) FROM INFORMATION_SCHEMA.STATISTICS
  WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Appointments' AND INDEX_NAME = 'IX_Appointments_TenantId_Date');
SET @sql := IF(@idx = 0,
  'CREATE INDEX `IX_Appointments_TenantId_Date` ON `Appointments` (`TenantId`, `Date`);',
  'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS `Appointments`;");
        }
    }
}
