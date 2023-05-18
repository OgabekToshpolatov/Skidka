using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArzonOL.Migrations
{
    /// <inheritdoc />
    public partial class AddedmodelBuilderCascadeApproach : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductCategoryApproaches_ProductCategoryApproachId",
                table: "Products");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductCategoryApproaches_ProductCategoryApproachId",
                table: "Products",
                column: "ProductCategoryApproachId",
                principalTable: "ProductCategoryApproaches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductCategoryApproaches_ProductCategoryApproachId",
                table: "Products");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductCategoryApproaches_ProductCategoryApproachId",
                table: "Products",
                column: "ProductCategoryApproachId",
                principalTable: "ProductCategoryApproaches",
                principalColumn: "Id");
        }
    }
}
