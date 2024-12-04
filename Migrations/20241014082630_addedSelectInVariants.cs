using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EStore.Migrations
{
    /// <inheritdoc />
    public partial class addedSelectInVariants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DisplayType",
                table: "Variants",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayType",
                table: "Variants");
        }
    }
}
