CREATE TABLE [dbo].[Cliente] (
    [id_cliente]       INT            IDENTITY (1, 1) NOT NULL,
    [nombre]           NVARCHAR (150) NOT NULL,
    [apellido]         NVARCHAR (150) NOT NULL,
    [dni]              INT            NOT NULL,
    [fecha_nacimiento] DATE           NOT NULL,
    [telefono]         NVARCHAR (20)  NOT NULL,
    [direccion]        NVARCHAR (100) NOT NULL,
    [email]            NVARCHAR (100) NULL,
    [activo]           BIT            DEFAULT ((1)) NOT NULL,
    CONSTRAINT [pk_id_cliente] PRIMARY KEY CLUSTERED ([id_cliente] ASC),
    CONSTRAINT [chk_fecha_nacimiento] CHECK ([fecha_nacimiento]<getdate()),
    CONSTRAINT [uq_dni] UNIQUE NONCLUSTERED ([dni] ASC),
    CONSTRAINT [uq_email] UNIQUE NONCLUSTERED ([email] ASC),
    CONSTRAINT [uq_telefono] UNIQUE NONCLUSTERED ([telefono] ASC)
);

