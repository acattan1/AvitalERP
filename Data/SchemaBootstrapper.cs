using Microsoft.EntityFrameworkCore;

namespace AvitalERP.Data
{
    /// <summary>
    /// Asegura que el esquema mínimo exista en la BD existente.
    /// Esto evita depender de comandos EF Tools (Update-Database) en máquinas donde no estén instalados.
    /// </summary>
    public static class SchemaBootstrapper
    {
        public static async Task EnsureAsync(AppDbContext db)
        {
            // Nota: SQL Server
            var sql = @"
-- =========================
-- Proyectos: columnas nuevas
-- =========================
IF COL_LENGTH('dbo.Proyectos', 'CerradoPor') IS NULL
    ALTER TABLE dbo.Proyectos ADD CerradoPor nvarchar(200) NULL;

IF COL_LENGTH('dbo.Proyectos', 'FechaCierre') IS NULL
    ALTER TABLE dbo.Proyectos ADD FechaCierre datetime2 NULL;

IF COL_LENGTH('dbo.Proyectos', 'OneDriveFolderUrl') IS NULL
    ALTER TABLE dbo.Proyectos ADD OneDriveFolderUrl nvarchar(1000) NULL;

-- =========================
-- Técnicos
-- =========================
IF OBJECT_ID('dbo.Tecnicos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Tecnicos (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Tecnicos PRIMARY KEY,
        Nombre nvarchar(120) NOT NULL,
        Activo bit NOT NULL CONSTRAINT DF_Tecnicos_Activo DEFAULT(1),
        CreatedAt datetime2 NOT NULL CONSTRAINT DF_Tecnicos_CreatedAt DEFAULT (SYSUTCDATETIME())
    );
END

-- =========================
-- Tipos de Proyecto + Plantillas
-- =========================
IF OBJECT_ID('dbo.ProyectoTipos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ProyectoTipos (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ProyectoTipos PRIMARY KEY,
        Codigo nvarchar(40) NOT NULL,
        Nombre nvarchar(120) NOT NULL,
        Activo bit NOT NULL CONSTRAINT DF_ProyectoTipos_Activo DEFAULT(1),
        CreatedAt datetime2 NOT NULL CONSTRAINT DF_ProyectoTipos_CreatedAt DEFAULT (SYSUTCDATETIME())
    );
    CREATE UNIQUE INDEX IX_ProyectoTipos_Codigo ON dbo.ProyectoTipos(Codigo);
END

IF OBJECT_ID('dbo.ProyectoTipoPasoPlantillas', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ProyectoTipoPasoPlantillas (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ProyectoTipoPasoPlantillas PRIMARY KEY,
        ProyectoTipoId int NOT NULL,
        Orden int NOT NULL,
        Nombre nvarchar(200) NOT NULL,
        Descripcion nvarchar(800) NULL,
        RequiereEvidencia bit NOT NULL CONSTRAINT DF_ProyectoTipoPasoPlantillas_RequiereEvidencia DEFAULT(0),
        Activo bit NOT NULL CONSTRAINT DF_ProyectoTipoPasoPlantillas_Activo DEFAULT(1),
        CreatedAt datetime2 NOT NULL CONSTRAINT DF_ProyectoTipoPasoPlantillas_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT FK_ProyectoTipoPasoPlantillas_ProyectoTipos FOREIGN KEY (ProyectoTipoId) REFERENCES dbo.ProyectoTipos(Id)
    );
    CREATE INDEX IX_ProyectoTipoPasoPlantillas_ProyectoTipoId ON dbo.ProyectoTipoPasoPlantillas(ProyectoTipoId);
END

IF OBJECT_ID('dbo.ProyectoTiposAsignados', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ProyectoTiposAsignados (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ProyectoTiposAsignados PRIMARY KEY,
        ProyectoId int NOT NULL,
        ProyectoTipoId int NOT NULL,
        CreatedAt datetime2 NOT NULL CONSTRAINT DF_ProyectoTiposAsignados_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT FK_ProyectoTiposAsignados_Proyectos FOREIGN KEY (ProyectoId) REFERENCES dbo.Proyectos(Id) ON DELETE CASCADE,
        CONSTRAINT FK_ProyectoTiposAsignados_ProyectoTipos FOREIGN KEY (ProyectoTipoId) REFERENCES dbo.ProyectoTipos(Id)
    );
    CREATE INDEX IX_ProyectoTiposAsignados_ProyectoId ON dbo.ProyectoTiposAsignados(ProyectoId);
    CREATE INDEX IX_ProyectoTiposAsignados_ProyectoTipoId ON dbo.ProyectoTiposAsignados(ProyectoTipoId);
END

-- =========================
-- Pasos de Proyecto + Evidencias
-- =========================
IF OBJECT_ID('dbo.ProyectoPasos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ProyectoPasos (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ProyectoPasos PRIMARY KEY,
        ProyectoId int NOT NULL,
        ProyectoTipoId int NULL,
        Orden int NOT NULL,
        Nombre nvarchar(200) NOT NULL,
        Descripcion nvarchar(800) NULL,
        Estado nvarchar(30) NOT NULL CONSTRAINT DF_ProyectoPasos_Estado DEFAULT('Pendiente'),
        TecnicoId int NULL,
        FechaObjetivo datetime2 NULL,
        FechaHecho datetime2 NULL,
        Notas nvarchar(2000) NULL,
        CreatedAt datetime2 NOT NULL CONSTRAINT DF_ProyectoPasos_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt datetime2 NOT NULL CONSTRAINT DF_ProyectoPasos_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT FK_ProyectoPasos_Proyectos FOREIGN KEY (ProyectoId) REFERENCES dbo.Proyectos(Id) ON DELETE CASCADE,
        CONSTRAINT FK_ProyectoPasos_ProyectoTipos FOREIGN KEY (ProyectoTipoId) REFERENCES dbo.ProyectoTipos(Id),
        CONSTRAINT FK_ProyectoPasos_Tecnicos FOREIGN KEY (TecnicoId) REFERENCES dbo.Tecnicos(Id)
    );
    CREATE INDEX IX_ProyectoPasos_ProyectoId ON dbo.ProyectoPasos(ProyectoId);
END

IF OBJECT_ID('dbo.ProyectoPasoEvidencias', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ProyectoPasoEvidencias (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ProyectoPasoEvidencias PRIMARY KEY,
        ProyectoPasoId int NOT NULL,
        FilePath nvarchar(400) NOT NULL,
        OriginalFileName nvarchar(255) NULL,
        ContentType nvarchar(120) NULL,
        SizeBytes bigint NOT NULL CONSTRAINT DF_ProyectoPasoEvidencias_Size DEFAULT(0),
        UploadedAt datetime2 NOT NULL CONSTRAINT DF_ProyectoPasoEvidencias_UploadedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT FK_ProyectoPasoEvidencias_ProyectoPasos FOREIGN KEY (ProyectoPasoId) REFERENCES dbo.ProyectoPasos(Id) ON DELETE CASCADE
    );
    CREATE INDEX IX_ProyectoPasoEvidencias_ProyectoPasoId ON dbo.ProyectoPasoEvidencias(ProyectoPasoId);
END

-- =========================
-- Compras CFDI
-- =========================
IF OBJECT_ID('dbo.CfdiDocumentos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CfdiDocumentos (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_CfdiDocumentos PRIMARY KEY,
        ProyectoId int NULL,
        FileNameXml nvarchar(260) NOT NULL CONSTRAINT DF_CfdiDocumentos_FileNameXml DEFAULT(''),
        StoredPathXml nvarchar(500) NOT NULL CONSTRAINT DF_CfdiDocumentos_StoredPathXml DEFAULT(''),
        FileNamePdf nvarchar(260) NULL,
        StoredPathPdf nvarchar(500) NULL,
        OneDriveUrl nvarchar(1000) NULL,
        UUID nvarchar(64) NULL,
        EmisorRfc nvarchar(20) NULL,
        EmisorNombre nvarchar(200) NULL,
        ReceptorRfc nvarchar(20) NULL,
        Fecha datetime2 NULL,
        Subtotal decimal(18,2) NOT NULL CONSTRAINT DF_CfdiDocumentos_Subtotal DEFAULT(0),
        Total decimal(18,2) NOT NULL CONSTRAINT DF_CfdiDocumentos_Total DEFAULT(0),
        Moneda nvarchar(10) NULL,
        UploadedAt datetime2 NOT NULL CONSTRAINT DF_CfdiDocumentos_UploadedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT FK_CfdiDocumentos_Proyectos FOREIGN KEY (ProyectoId) REFERENCES dbo.Proyectos(Id)
    );
    CREATE INDEX IX_CfdiDocumentos_ProyectoId ON dbo.CfdiDocumentos(ProyectoId);
    CREATE INDEX IX_CfdiDocumentos_UUID ON dbo.CfdiDocumentos(UUID);
END

IF OBJECT_ID('dbo.CfdiConceptos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CfdiConceptos (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_CfdiConceptos PRIMARY KEY,
        CfdiDocumentoId int NOT NULL,
        ClaveProdServ nvarchar(20) NULL,
        NoIdentificacion nvarchar(50) NULL,
        Descripcion nvarchar(500) NOT NULL,
        Cantidad decimal(18,4) NOT NULL CONSTRAINT DF_CfdiConceptos_Cantidad DEFAULT(0),
        ValorUnitario decimal(18,4) NOT NULL CONSTRAINT DF_CfdiConceptos_ValorUnitario DEFAULT(0),
        Importe decimal(18,2) NOT NULL CONSTRAINT DF_CfdiConceptos_Importe DEFAULT(0),
        Iva decimal(18,2) NULL,
        DestinoTipo nvarchar(30) NOT NULL CONSTRAINT DF_CfdiConceptos_DestinoTipo DEFAULT('Proyecto'),
        ProyectoId int NULL,
        Categoria nvarchar(120) NULL,
        CONSTRAINT FK_CfdiConceptos_CfdiDocumentos FOREIGN KEY (CfdiDocumentoId) REFERENCES dbo.CfdiDocumentos(Id) ON DELETE CASCADE,
        CONSTRAINT FK_CfdiConceptos_Proyectos FOREIGN KEY (ProyectoId) REFERENCES dbo.Proyectos(Id)
    );
    CREATE INDEX IX_CfdiConceptos_CfdiDocumentoId ON dbo.CfdiConceptos(CfdiDocumentoId);
END
";

            await db.Database.ExecuteSqlRawAsync(sql);
        }
    }
}
