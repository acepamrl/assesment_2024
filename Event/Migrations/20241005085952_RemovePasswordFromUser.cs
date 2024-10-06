using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Event.Migrations
{
    /// <inheritdoc />
    public partial class RemovePasswordFromUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "msUser");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "msUser",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
