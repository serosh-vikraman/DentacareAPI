using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddServicesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE TABLE IF NOT EXISTS `Services` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `Name` varchar(256) CHARACTER SET utf8mb4 NOT NULL,
    `Amount` decimal(18,2) NOT NULL,
    `Category` varchar(128) CHARACTER SET utf8mb4 NULL,
    `TenantId` char(36) COLLATE ascii_general_ci NOT NULL,
    `BranchId` char(36) COLLATE ascii_general_ci NULL,
    `CreatedUtc` datetime(6) NOT NULL,
    `UpdatedUtc` datetime(6) NULL,
    `IsDeleted` bit(1) NOT NULL,
    CONSTRAINT `PK_Services` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;
", suppressTransaction: false);

            migrationBuilder.Sql(@"
SET @idx := (SELECT COUNT(1) FROM INFORMATION_SCHEMA.STATISTICS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Services' AND INDEX_NAME = 'IX_Services_TenantId_Name');
SET @sql := IF(@idx = 0, 'CREATE INDEX `IX_Services_TenantId_Name` ON `Services` (`TenantId`, `Name`);', 'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS `Services`;");
        }
    }
}




