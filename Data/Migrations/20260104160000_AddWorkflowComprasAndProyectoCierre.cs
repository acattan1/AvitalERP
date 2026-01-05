using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvitalERP.Data.Migrations
{
    public partial class AddWorkflowComprasAndProyectoCierre : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // === Proyectos: cierre + OneDrive ===
            migrationBuilder.AddColumn<string>(
                name: "CerradoPor",
                table: "Proyectos",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCierre",
                table: "Proyectos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OneDriveFolderUrl",
                table: "Proyectos",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            // === Técnicos ===
            migrationBuilder.CreateTable(
                name: "Tecnicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tecnicos", x => x.Id);
                });

            // === Tipos de proyecto ===
            migrationBuilder.CreateTable(
                name: "ProyectoTipos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoTipos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProyectoTipos_Codigo",
                table: "ProyectoTipos",
                column: "Codigo",
                unique: true);

            // === Plantillas de pasos ===
            migrationBuilder.CreateTable(
                name: "ProyectoTipoPasoPlantillas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProyectoTipoId = table.Column<int>(type: "int", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RequiereEvidencia = table.Column<bool>(type: "bit", nullable: false),
                    DiasObjetivo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoTipoPasoPlantillas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProyectoTipoPasoPlantillas_ProyectoTipos_ProyectoTipoId",
                        column: x => x.ProyectoTipoId,
                        principalTable: "ProyectoTipos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProyectoTipoPasoPlantillas_ProyectoTipoId",
                table: "ProyectoTipoPasoPlantillas",
                column: "ProyectoTipoId");

            // === Tipos asignados por proyecto (N:N) ===
            migrationBuilder.CreateTable(
                name: "ProyectoTiposAsignados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProyectoId = table.Column<int>(type: "int", nullable: false),
                    ProyectoTipoId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoTiposAsignados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProyectoTiposAsignados_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProyectoTiposAsignados_ProyectoTipos_ProyectoTipoId",
                        column: x => x.ProyectoTipoId,
                        principalTable: "ProyectoTipos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProyectoTiposAsignados_ProyectoId",
                table: "ProyectoTiposAsignados",
                column: "ProyectoId");
            migrationBuilder.CreateIndex(
                name: "IX_ProyectoTiposAsignados_ProyectoTipoId",
                table: "ProyectoTiposAsignados",
                column: "ProyectoTipoId");

            // === Pasos por proyecto ===
            migrationBuilder.CreateTable(
                name: "ProyectoPasos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProyectoId = table.Column<int>(type: "int", nullable: false),
                    ProyectoTipoPasoPlantillaId = table.Column<int>(type: "int", nullable: true),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FechaObjetivo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaHecho = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notas = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TecnicoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoPasos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProyectoPasos_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProyectoPasos_ProyectoTipoPasoPlantillas_ProyectoTipoPasoPlantillaId",
                        column: x => x.ProyectoTipoPasoPlantillaId,
                        principalTable: "ProyectoTipoPasoPlantillas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProyectoPasos_Tecnicos_TecnicoId",
                        column: x => x.TecnicoId,
                        principalTable: "Tecnicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProyectoPasos_ProyectoId",
                table: "ProyectoPasos",
                column: "ProyectoId");
            migrationBuilder.CreateIndex(
                name: "IX_ProyectoPasos_ProyectoTipoPasoPlantillaId",
                table: "ProyectoPasos",
                column: "ProyectoTipoPasoPlantillaId");
            migrationBuilder.CreateIndex(
                name: "IX_ProyectoPasos_TecnicoId",
                table: "ProyectoPasos",
                column: "TecnicoId");

            // === Evidencias ===
            migrationBuilder.CreateTable(
                name: "ProyectoPasoEvidencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProyectoPasoId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    StoredPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoPasoEvidencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProyectoPasoEvidencias_ProyectoPasos_ProyectoPasoId",
                        column: x => x.ProyectoPasoId,
                        principalTable: "ProyectoPasos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProyectoPasoEvidencias_ProyectoPasoId",
                table: "ProyectoPasoEvidencias",
                column: "ProyectoPasoId");

            // === CFDI documentos ===
            migrationBuilder.CreateTable(
                name: "CfdiDocumentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProyectoId = table.Column<int>(type: "int", nullable: true),
                    FileNameXml = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    StoredPathXml = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileNamePdf = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                    StoredPathPdf = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OneDriveUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UUID = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    EmisorRfc = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    EmisorNombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReceptorRfc = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Moneda = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CfdiDocumentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CfdiDocumentos_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CfdiDocumentos_ProyectoId",
                table: "CfdiDocumentos",
                column: "ProyectoId");

            // === CFDI conceptos ===
            migrationBuilder.CreateTable(
                name: "CfdiConceptos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CfdiDocumentoId = table.Column<int>(type: "int", nullable: false),
                    ClaveProdServ = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NoIdentificacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Importe = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Iva = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DestinoTipo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ProyectoId = table.Column<int>(type: "int", nullable: true),
                    Categoria = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CfdiConceptos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CfdiConceptos_CfdiDocumentos_CfdiDocumentoId",
                        column: x => x.CfdiDocumentoId,
                        principalTable: "CfdiDocumentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CfdiConceptos_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CfdiConceptos_CfdiDocumentoId",
                table: "CfdiConceptos",
                column: "CfdiDocumentoId");
            migrationBuilder.CreateIndex(
                name: "IX_CfdiConceptos_ProyectoId",
                table: "CfdiConceptos",
                column: "ProyectoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "CfdiConceptos");
            migrationBuilder.DropTable(name: "CfdiDocumentos");
            migrationBuilder.DropTable(name: "ProyectoPasoEvidencias");
            migrationBuilder.DropTable(name: "ProyectoPasos");
            migrationBuilder.DropTable(name: "ProyectoTiposAsignados");
            migrationBuilder.DropTable(name: "ProyectoTipoPasoPlantillas");
            migrationBuilder.DropTable(name: "Tecnicos");
            migrationBuilder.DropTable(name: "ProyectoTipos");

            migrationBuilder.DropColumn(name: "CerradoPor", table: "Proyectos");
            migrationBuilder.DropColumn(name: "FechaCierre", table: "Proyectos");
            migrationBuilder.DropColumn(name: "OneDriveFolderUrl", table: "Proyectos");
        }
    }
}
