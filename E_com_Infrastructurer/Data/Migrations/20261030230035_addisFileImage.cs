using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_com_Infrastructurer.Data.Migrations
{
    /// <inheritdoc />
    public partial class addisFileImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReceiptImage",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiptImage",
                table: "Orders");
        }
    }
}
