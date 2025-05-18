IF SCHEMA_ID(N'CodingExercise') IS NULL EXEC(N'CREATE SCHEMA [CodingExercise];');
GO


CREATE TABLE [CodingExercise].[Document] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [FilePath] nvarchar(max) NOT NULL,
    [UploadedAt] datetime2 NOT NULL,
    [UploadedBy] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Document] PRIMARY KEY ([Id])
    );
GO


CREATE TABLE [CodingExercise].[IngestionTask] (
    [Id] int NOT NULL IDENTITY,
    [DocumentId] int NOT NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [SpringResponse] nvarchar(max) NULL,
    CONSTRAINT [PK_IngestionTask] PRIMARY KEY ([Id])
    );
GO
