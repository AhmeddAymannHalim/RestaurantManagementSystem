using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RestaurantManageSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImplementedSettingAndForgetPasswordAndResetPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "OrderItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "MenuItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "IsActive", "Key", "UpdatedAt", "Value" },
                values: new object[,]
                {
                    { 1, "Email", new DateTime(2025, 12, 30, 6, 21, 40, 484, DateTimeKind.Utc).AddTicks(6798), "SMTP Server Address", true, "Email.SmtpServer", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "smtp.gmail.com" },
                    { 2, "Email", new DateTime(2025, 12, 30, 6, 21, 40, 484, DateTimeKind.Utc).AddTicks(6801), "SMTP Port Number", true, "Email.SmtpPort", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "587" },
                    { 3, "Email", new DateTime(2025, 12, 30, 6, 21, 40, 484, DateTimeKind.Utc).AddTicks(6803), "Sender Email Address", true, "Email.FromEmail", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "noreply@restaurant.com" },
                    { 4, "Email", new DateTime(2025, 12, 30, 6, 21, 40, 484, DateTimeKind.Utc).AddTicks(6804), "Email Account Password (App Password for Gmail)", true, "Email.Password", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "" },
                    { 5, "Email", new DateTime(2025, 12, 30, 6, 21, 40, 484, DateTimeKind.Utc).AddTicks(6805), "Enable SSL for SMTP", true, "Email.EnableSsl", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "true" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Settings_Key",
                table: "Settings",
                column: "Key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "MenuItems");
        }
    }
}
