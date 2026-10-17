using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_com_Infrastructurer.Data.Migrations
{
    /// <inheritdoc />
    public partial class addcolumsIsProdsed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Description", "Name", "NewPrice", "OldPrice" },
                values: new object[] { 9, 7, "ios sss", "tvs", 60000m, 0m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);
        }
    }
}
