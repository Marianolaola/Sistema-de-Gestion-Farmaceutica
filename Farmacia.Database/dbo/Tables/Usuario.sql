CREATE TABLE [dbo].[Usuario] (
    [id_usuario] INT           IDENTITY (1, 1) NOT NULL,
    [nombre]     VARCHAR (150) NOT NULL,
    [apellido]   VARCHAR (150) NOT NULL,
    [email]      NVARCHAR (50) NOT NULL,
    [contraseña] NVARCHAR(255) NOT NULL,
    [rol]        VARCHAR (50)  NOT NULL,
    CONSTRAINT [pk_id_usuario] PRIMARY KEY CLUSTERED ([id_usuario] ASC),
    CONSTRAINT [UQ_email_usuario] UNIQUE NONCLUSTERED ([email] ASC)
);

