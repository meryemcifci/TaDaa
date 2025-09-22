using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaDaa.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddReminderTimeToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "ReminderTime",
                table: "AspNetUsers",
                type: "time",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReminderTime",
                table: "AspNetUsers");
        }
    }
}
