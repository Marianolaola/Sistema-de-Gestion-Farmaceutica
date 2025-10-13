CREATE TABLE [dbo].[Obra_Social] (
    [id_obra_social] INT           IDENTITY (1, 1) NOT NULL,
    [nombre]         NVARCHAR (50) NOT NULL,
    CONSTRAINT [pk_id_obra_social] PRIMARY KEY CLUSTERED ([id_obra_social] ASC)
);

