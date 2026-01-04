using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvitalERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixIdentityContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HubspotCompanyLinks_Clientes_ClienteId",
                table: "HubspotCompanyLinks");

            migrationBuilder.DropIndex(
                name: "IX_HubspotCompanyLinks_HubspotCompanyId",
                table: "HubspotCompanyLinks");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_HubspotCompanyId",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_Rfc",
                table: "Clientes");

            migrationBuilder.AlterColumn<decimal>(
                name: "TasaRetencionISR",
                table: "Proyectos",
                type: "decimal(9,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TasaIVA",
                table: "Proyectos",
                type: "decimal(9,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,4)");

            migrationBuilder.AlterColumn<string>(
                name: "HubspotCompanyName",
                table: "Proyectos",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "HubspotCompanyId",
                table: "Proyectos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "HubspotCompanyId",
                table: "Clientes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaRegistro",
                table: "Clientes",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Clientes",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Email",
                table: "Clientes",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_HubspotCompanyId",
                table: "Clientes",
                column: "HubspotCompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_HubspotCompanyLinks_Clientes_ClienteId",
                table: "HubspotCompanyLinks",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HubspotCompanyLinks_Clientes_ClienteId",
                table: "HubspotCompanyLinks");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_Email",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_HubspotCompanyId",
                table: "Clientes");

            migrationBuilder.AlterColumn<decimal>(
                name: "TasaRetencionISR",
                table: "Proyectos",
                type: "decimal(9,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TasaIVA",
                table: "Proyectos",
                type: "decimal(9,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,6)");

            migrationBuilder.AlterColumn<string>(
                name: "HubspotCompanyName",
                table: "Proyectos",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HubspotCompanyId",
                table: "Proyectos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HubspotCompanyId",
                table: "Clientes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaRegistro",
                table: "Clientes",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.CreateIndex(
                name: "IX_HubspotCompanyLinks_HubspotCompanyId",
                table: "HubspotCompanyLinks",
                column: "HubspotCompanyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_HubspotCompanyId",
                table: "Clientes",
                column: "HubspotCompanyId",
                unique: true,
                filter: "[HubspotCompanyId] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Rfc",
                table: "Clientes",
                column: "Rfc",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HubspotCompanyLinks_Clientes_ClienteId",
                table: "HubspotCompanyLinks",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
