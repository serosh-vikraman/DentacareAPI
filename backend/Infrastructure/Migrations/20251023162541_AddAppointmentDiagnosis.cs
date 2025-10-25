using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentDiagnosis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
SET @exists := (SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'Diagnosis');
SET @sql := IF(@exists = 0, 'ALTER TABLE `Appointments` ADD `Diagnosis` longtext NULL;', 'SELECT 1');
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;

SET @exists := (SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'DifferentialDiagnosis');
SET @sql := IF(@exists = 0, 'ALTER TABLE `Appointments` ADD `DifferentialDiagnosis` longtext NULL;', 'SELECT 1');
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;

SET @exists := (SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'InvestigationBlood');
SET @sql := IF(@exists = 0, 'ALTER TABLE `Appointments` ADD `InvestigationBlood` varchar(256) NULL;', 'SELECT 1');
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;

SET @exists := (SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'InvestigationCbct');
SET @sql := IF(@exists = 0, 'ALTER TABLE `Appointments` ADD `InvestigationCbct` tinyint(1) NULL;', 'SELECT 1');
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;

SET @exists := (SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'InvestigationCeph');
SET @sql := IF(@exists = 0, 'ALTER TABLE `Appointments` ADD `InvestigationCeph` tinyint(1) NULL;', 'SELECT 1');
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;

SET @exists := (SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'InvestigationOcclusal');
SET @sql := IF(@exists = 0, 'ALTER TABLE `Appointments` ADD `InvestigationOcclusal` tinyint(1) NULL;', 'SELECT 1');
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;

SET @exists := (SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'InvestigationOpg');
SET @sql := IF(@exists = 0, 'ALTER TABLE `Appointments` ADD `InvestigationOpg` tinyint(1) NULL;', 'SELECT 1');
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;

SET @exists := (SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'InvestigationOthers');
SET @sql := IF(@exists = 0, 'ALTER TABLE `Appointments` ADD `InvestigationOthers` longtext NULL;', 'SELECT 1');
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;

SET @exists := (SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'InvestigationRvg');
SET @sql := IF(@exists = 0, 'ALTER TABLE `Appointments` ADD `InvestigationRvg` varchar(256) NULL;', 'SELECT 1');
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;

SET @exists := (SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'TreatmentPlan');
SET @sql := IF(@exists = 0, 'ALTER TABLE `Appointments` ADD `TreatmentPlan` longtext NULL;', 'SELECT 1');
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
", suppressTransaction: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
SET @exists := (SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'Diagnosis');
SET @sql := IF(@exists = 1, 'ALTER TABLE `Appointments` DROP COLUMN `Diagnosis`;', 'SELECT 1');
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
SET @exists := (SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'DifferentialDiagnosis');
SET @sql := IF(@exists = 1, 'ALTER TABLE `Appointments` DROP COLUMN `DifferentialDiagnosis`;', 'SELECT 1');
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
SET @exists := (SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'InvestigationBlood');
SET @sql := IF(@exists = 1, 'ALTER TABLE `Appointments` DROP COLUMN `InvestigationBlood`;', 'SELECT 1');
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
SET @exists := (SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'InvestigationCbct');
SET @sql := IF(@exists = 1, 'ALTER TABLE `Appointments` DROP COLUMN `InvestigationCbct`;', 'SELECT 1');
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
SET @exists := (SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'InvestigationCeph');
SET @sql := IF(@exists = 1, 'ALTER TABLE `Appointments` DROP COLUMN `InvestigationCeph`;', 'SELECT 1');
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
SET @exists := (SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'InvestigationOcclusal');
SET @sql := IF(@exists = 1, 'ALTER TABLE `Appointments` DROP COLUMN `InvestigationOcclusal`;', 'SELECT 1');
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
SET @exists := (SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'InvestigationOpg');
SET @sql := IF(@exists = 1, 'ALTER TABLE `Appointments` DROP COLUMN `InvestigationOpg`;', 'SELECT 1');
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
SET @exists := (SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'InvestigationOthers');
SET @sql := IF(@exists = 1, 'ALTER TABLE `Appointments` DROP COLUMN `InvestigationOthers`;', 'SELECT 1');
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
SET @exists := (SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'InvestigationRvg');
SET @sql := IF(@exists = 1, 'ALTER TABLE `Appointments` DROP COLUMN `InvestigationRvg`;', 'SELECT 1');
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
SET @exists := (SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'TreatmentPlan');
SET @sql := IF(@exists = 1, 'ALTER TABLE `Appointments` DROP COLUMN `TreatmentPlan`;', 'SELECT 1');
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
", suppressTransaction: false);
        }
    }
}
