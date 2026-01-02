using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvitalERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHubspotPollingV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HubspotCompanyId",
                table: "Clientes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "HubspotSyncStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastClosedDateProcessedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HubspotSyncStates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Proyectos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Folio = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HubspotDealId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    NombreProyecto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Moneda = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    JsonPayloadOriginal = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proyectos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Proyectos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_HubspotCompanyId",
                table: "Clientes",
                column: "HubspotCompanyId",
                unique: true,
                filter: "[HubspotCompanyId] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_Proyectos_ClienteId",
                table: "Proyectos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Proyectos_Folio",
                table: "Proyectos",
                column: "Folio",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proyectos_HubspotDealId",
                table: "Proyectos",
                column: "HubspotDealId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HubspotSyncStates");

            migrationBuilder.DropTable(
                name: "Proyectos");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_HubspotCompanyId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "HubspotCompanyId",
                table: "Clientes");
        }
    }
}
