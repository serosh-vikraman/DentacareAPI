using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingDentalVitals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContinuousSpO2",
                table: "PatientProfiles",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "HeartRateECG",
                table: "PatientProfiles",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "IntraExtraNotes",
                table: "PatientProfiles",
                type: "varchar(2048)",
                maxLength: 2048,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "MucosalNotes",
                table: "PatientProfiles",
                type: "varchar(2048)",
                maxLength: 2048,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "OcclusionNotes",
                table: "PatientProfiles",
                type: "varchar(2048)",
                maxLength: 2048,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "SalivaPHFlow",
                table: "PatientProfiles",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TMJNotes",
                table: "PatientProfiles",
                type: "varchar(2048)",
                maxLength: 2048,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContinuousSpO2",
                table: "PatientProfiles");

            migrationBuilder.DropColumn(
                name: "HeartRateECG",
                table: "PatientProfiles");

            migrationBuilder.DropColumn(
                name: "IntraExtraNotes",
                table: "PatientProfiles");

            migrationBuilder.DropColumn(
                name: "MucosalNotes",
                table: "PatientProfiles");

            migrationBuilder.DropColumn(
                name: "OcclusionNotes",
                table: "PatientProfiles");

            migrationBuilder.DropColumn(
                name: "SalivaPHFlow",
                table: "PatientProfiles");

            migrationBuilder.DropColumn(
                name: "TMJNotes",
                table: "PatientProfiles");
        }
    }
}
