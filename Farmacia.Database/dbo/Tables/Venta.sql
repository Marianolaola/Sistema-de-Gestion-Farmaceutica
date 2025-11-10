CREATE TABLE [dbo].[Venta] (
    [id_venta]    INT      IDENTITY (1, 1) NOT NULL,
    [fecha_venta] DATETIME NOT NULL,
    [id_cliente]  INT      NOT NULL,
    [id_usuario]  INT      NOT NULL,
    [total] DECIMAL(10, 2) NULL, 
    CONSTRAINT [pk_id_venta] PRIMARY KEY CLUSTERED ([id_venta] ASC),
    CONSTRAINT [chk_fecha_venta] CHECK ([fecha_venta]<=getdate()),
    CONSTRAINT [fk_venta_cliente] FOREIGN KEY ([id_cliente]) REFERENCES [dbo].[Cliente] ([id_cliente]),
    CONSTRAINT [fk_venta_usuario] FOREIGN KEY ([id_usuario]) REFERENCES [dbo].[Usuario] ([id_usuario])
);

