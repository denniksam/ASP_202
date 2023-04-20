using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_202.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsPublicFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDatesPublic",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailPublic",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRealNamePublic",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDatesPublic",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsEmailPublic",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsRealNamePublic",
                table: "Users");
        }
    }
}
