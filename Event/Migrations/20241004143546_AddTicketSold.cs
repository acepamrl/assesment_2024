using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Event.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketSold : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TicketSold",
                table: "msEvent",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketSold",
                table: "msEvent");
        }
    }
}
