using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockAnalyzer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Acoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ticker = table.Column<string>(type: "TEXT", maxLength: 6, nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Acoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cotacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Abertura = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    Maxima = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    Minima = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    Fechamento = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    Volume = table.Column<long>(type: "INTEGER", nullable: false),
                    AcaoId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cotacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cotacoes_Acoes_AcaoId",
                        column: x => x.AcaoId,
                        principalTable: "Acoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Acoes_Ticker",
                table: "Acoes",
                column: "Ticker",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cotacoes_AcaoId_Data",
                table: "Cotacoes",
                columns: new[] { "AcaoId", "Data" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cotacoes");

            migrationBuilder.DropTable(
                name: "Acoes");
        }
    }
}
