using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notification.Migrations
{
    /// <inheritdoc />
    public partial class AddIdPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IdPayment",
                table: "TrNotification",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdPayment",
                table: "TrNotification");
        }
    }
}
