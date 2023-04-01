using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyStore.Infrastructure.Migrations
{
    public partial class ccc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_AddressIdId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_AddressIdId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "AddressIdId",
                table: "Orders");

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Products",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "AddressId",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                table: "Orders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SizeType",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SizeType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Size",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SizeTypeId = table.Column<Guid>(nullable: true),
                    SizeName = table.Column<string>(nullable: true),
                    ProductId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Size", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Size_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Size_SizeType_SizeTypeId",
                        column: x => x.SizeTypeId,
                        principalTable: "SizeType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AddressId",
                table: "Orders",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Size_ProductId",
                table: "Size",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Size_SizeTypeId",
                table: "Size",
                column: "SizeTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_AddressId",
                table: "Orders",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_AddressId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Size");

            migrationBuilder.DropTable(
                name: "SizeType");

            migrationBuilder.DropIndex(
                name: "IX_Orders_AddressId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Completed",
                table: "Orders");

            migrationBuilder.AddColumn<Guid>(
                name: "AddressIdId",
                table: "Orders",
                type: "char(36)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AddressIdId",
                table: "Orders",
                column: "AddressIdId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_AddressIdId",
                table: "Orders",
                column: "AddressIdId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
