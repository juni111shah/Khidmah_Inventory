using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khidmah_Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSalesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountPaid",
                table: "SalesOrders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ChangeAmount",
                table: "SalesOrders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsPos",
                table: "SalesOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "SalesOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PosSessionId",
                table: "SalesOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PosSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OpeningBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ClosingBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ExpectedBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PosSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_PosSessionId",
                table: "SalesOrders",
                column: "PosSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_PosSessions_UserId",
                table: "PosSessions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrders_PosSessions_PosSessionId",
                table: "SalesOrders",
                column: "PosSessionId",
                principalTable: "PosSessions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrders_PosSessions_PosSessionId",
                table: "SalesOrders");

            migrationBuilder.DropTable(
                name: "PosSessions");

            migrationBuilder.DropIndex(
                name: "IX_SalesOrders_PosSessionId",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "AmountPaid",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "ChangeAmount",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "IsPos",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "PosSessionId",
                table: "SalesOrders");
        }
    }
}
