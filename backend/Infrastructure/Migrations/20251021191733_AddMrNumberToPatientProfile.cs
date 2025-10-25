using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMrNumberToPatientProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MRNumber",
                table: "PatientProfiles",
                type: "varchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PatientProfiles_TenantId_MRNumber",
                table: "PatientProfiles",
                columns: new[] { "TenantId", "MRNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PatientProfiles_TenantId_MRNumber",
                table: "PatientProfiles");

            migrationBuilder.DropColumn(
                name: "MRNumber",
                table: "PatientProfiles");
        }
    }
}
