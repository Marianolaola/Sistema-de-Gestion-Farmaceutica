CREATE TABLE [dbo].[Detalle_Venta] (
    [id_venta]       INT        NOT NULL,
    [id_medicamento] INT        NOT NULL,
    [cantidad]       INT        NOT NULL,
    [subtotal]       FLOAT (53) NOT NULL,
    CONSTRAINT [pk_venta_medicamento] PRIMARY KEY CLUSTERED ([id_venta] ASC, [id_medicamento] ASC),
    CONSTRAINT [fk_detalle_venta_medicamento] FOREIGN KEY ([id_medicamento]) REFERENCES [dbo].[Medicamento] ([id_medicamento]),
    CONSTRAINT [fk_detalle_venta_venta] FOREIGN KEY ([id_venta]) REFERENCES [dbo].[Venta] ([id_venta])
);

