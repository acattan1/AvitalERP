using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvitalERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProyectoImpuestosV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proyectos_Clientes_ClienteId",
                table: "Proyectos");

            migrationBuilder.RenameColumn(
                name: "Monto",
                table: "Proyectos",
                newName: "Total");

            migrationBuilder.AlterColumn<int>(
                name: "ClienteId",
                table: "Proyectos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "AplicaIVA",
                table: "Proyectos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AplicaRetencionISR",
                table: "Proyectos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "HubspotCompanyId",
                table: "Proyectos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HubspotCompanyName",
                table: "Proyectos",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "IvaImporte",
                table: "Proyectos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RetencionISRImporte",
                table: "Proyectos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "Proyectos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TasaIVA",
                table: "Proyectos",
                type: "decimal(9,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TasaRetencionISR",
                table: "Proyectos",
                type: "decimal(9,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "HubspotCompanyLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HubspotCompanyId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HubspotCompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HubspotCompanyLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HubspotCompanyLinks_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HubspotCompanyLinks_ClienteId",
                table: "HubspotCompanyLinks",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_HubspotCompanyLinks_HubspotCompanyId",
                table: "HubspotCompanyLinks",
                column: "HubspotCompanyId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Proyectos_Clientes_ClienteId",
                table: "Proyectos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proyectos_Clientes_ClienteId",
                table: "Proyectos");

            migrationBuilder.DropTable(
                name: "HubspotCompanyLinks");

            migrationBuilder.DropColumn(
                name: "AplicaIVA",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "AplicaRetencionISR",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "HubspotCompanyId",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "HubspotCompanyName",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "IvaImporte",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "RetencionISRImporte",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "Subtotal",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "TasaIVA",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "TasaRetencionISR",
                table: "Proyectos");

            migrationBuilder.RenameColumn(
                name: "Total",
                table: "Proyectos",
                newName: "Monto");

            migrationBuilder.AlterColumn<int>(
                name: "ClienteId",
                table: "Proyectos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Proyectos_Clientes_ClienteId",
                table: "Proyectos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
