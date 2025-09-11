using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeekShopping.OrderAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateColumnNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order_detail_order_header_OrderHeaderId",
                table: "order_detail");

            migrationBuilder.DropIndex(
                name: "IX_order_detail_OrderHeaderId",
                table: "order_detail");

            migrationBuilder.RenameColumn(
                name: "productName",
                table: "order_detail",
                newName: "product_name");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "order_detail",
                newName: "product_id");

            migrationBuilder.AddColumn<long>(
                name: "orderHeader_id",
                table: "order_detail",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_order_detail_orderHeader_id",
                table: "order_detail",
                column: "orderHeader_id");

            migrationBuilder.AddForeignKey(
                name: "FK_order_detail_order_header_orderHeader_id",
                table: "order_detail",
                column: "orderHeader_id",
                principalTable: "order_header",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order_detail_order_header_orderHeader_id",
                table: "order_detail");

            migrationBuilder.DropIndex(
                name: "IX_order_detail_orderHeader_id",
                table: "order_detail");

            migrationBuilder.DropColumn(
                name: "orderHeader_id",
                table: "order_detail");

            migrationBuilder.RenameColumn(
                name: "product_name",
                table: "order_detail",
                newName: "productName");

            migrationBuilder.RenameColumn(
                name: "product_id",
                table: "order_detail",
                newName: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_order_detail_OrderHeaderId",
                table: "order_detail",
                column: "OrderHeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_order_detail_order_header_OrderHeaderId",
                table: "order_detail",
                column: "OrderHeaderId",
                principalTable: "order_header",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
