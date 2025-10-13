CREATE TABLE [dbo].[Medicamento] (
    [id_medicamento]   INT           IDENTITY (1, 1) NOT NULL,
    [nombre_comercial] VARCHAR (150) NOT NULL,
    [precio_unitario]  FLOAT (53)    NOT NULL,
    [presentacion]     VARCHAR (250) NOT NULL,
    [laboratorio]      VARCHAR (100) NOT NULL,
    [stock]            INT           NOT NULL,
    [stock_minimo]     INT           NOT NULL,
    [activado]         BIT           CONSTRAINT [df_medicamento_activado] DEFAULT ((1)) NULL,
    CONSTRAINT [pk_id_medicamento] PRIMARY KEY CLUSTERED ([id_medicamento] ASC),
    CONSTRAINT [UQ_nombre_comercial_presentacion_laboratorio] UNIQUE NONCLUSTERED ([nombre_comercial] ASC, [presentacion] ASC, [laboratorio] ASC)
);

