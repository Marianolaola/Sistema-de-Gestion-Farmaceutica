CREATE TABLE [dbo].[Cliente_Obra_Social] (
    [id_cliente]     INT           NOT NULL,
    [id_obra_social] INT           NOT NULL,
    [nro_afiliado]   NVARCHAR (50) NOT NULL,
    CONSTRAINT [pk_cliente_obra_social] PRIMARY KEY CLUSTERED ([id_cliente] ASC, [id_obra_social] ASC),
    CONSTRAINT [fk_cliente_obra_social_cliente] FOREIGN KEY ([id_cliente]) REFERENCES [dbo].[Cliente] ([id_cliente]),
    CONSTRAINT [fk_cliente_obra_social_obra_social] FOREIGN KEY ([id_obra_social]) REFERENCES [dbo].[Obra_Social] ([id_obra_social]),
    CONSTRAINT [uq_cliente_obra_afiliado] UNIQUE NONCLUSTERED ([id_obra_social] ASC, [nro_afiliado] ASC)
);

