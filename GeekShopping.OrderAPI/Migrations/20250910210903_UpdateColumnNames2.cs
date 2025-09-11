using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeekShopping.OrderAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateColumnNames2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order_detail_order_header_OrderHeaderId",
                table: "order_detail");

            migrationBuilder.DropIndex(
                name: "IX_order_detail_OrderHeaderId",
                table: "order_detail");

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
    }
}
