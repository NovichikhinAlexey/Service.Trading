using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.Trading.Postgres.Migrations
{
    public partial class TradeRequestAndExternalTrade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cb_trading");

            migrationBuilder.CreateTable(
                name: "external_trade",
                schema: "cb_trading",
                columns: table => new
                {
                    TradeId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    RequestId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Source = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Market = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Volume = table.Column<decimal>(type: "numeric", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsBuySide = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ExchangeTradeId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_external_trade", x => x.TradeId);
                });

            migrationBuilder.CreateTable(
                name: "trade_request",
                schema: "cb_trading",
                columns: table => new
                {
                    RequestId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    SellAssetSymbol = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    BuyAssetSymbol = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    SellAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    BuyAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    FeeAsset = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    FeeAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ErrorCode = table.Column<int>(type: "integer", nullable: false),
                    ErrorMessage = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    ExpectedSellAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    ExpectedBuyAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    ClientId = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trade_request", x => x.RequestId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_external_trade_RequestId",
                schema: "cb_trading",
                table: "external_trade",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_external_trade_Status",
                schema: "cb_trading",
                table: "external_trade",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_external_trade_Timestamp",
                schema: "cb_trading",
                table: "external_trade",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_trade_request_ClientId",
                schema: "cb_trading",
                table: "trade_request",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_trade_request_Timestamp",
                schema: "cb_trading",
                table: "trade_request",
                column: "Timestamp");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "external_trade",
                schema: "cb_trading");

            migrationBuilder.DropTable(
                name: "trade_request",
                schema: "cb_trading");
        }
    }
}
