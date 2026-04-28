using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notely.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfileImagePath",
                table: "AspNetUsers",
                newName: "ProfileImagepath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfileImagepath",
                table: "AspNetUsers",
                newName: "ProfileImagePath");
        }
    }
}
