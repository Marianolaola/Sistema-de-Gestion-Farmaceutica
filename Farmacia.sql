CREATE DATABASE SistemaFarmaceutico;
GO
USE SistemaFarmaceutico;

----CLIENTE-------------------------------------------------------------------------------------------
CREATE TABLE Cliente
(
  id_cliente INT IDENTITY (1,1),
  nombre NVARCHAR(150) NOT NULL,
  apellido NVARCHAR(150) NOT NULL,
  dni INT NOT NULL,
  fecha_nacimiento DATE NOT NULL, 
  telefono NVARCHAR(20) NOT NULL,
  direccion NVARCHAR(100) NOT NULL,
  email NVARCHAR(100),
  CONSTRAINT pk_id_cliente PRIMARY KEY (id_cliente),
  CONSTRAINT uq_dni UNIQUE (dni),
  CONSTRAINT uq_telefono UNIQUE (telefono),
  CONSTRAINT uq_email UNIQUE (email),
);

ALTER TABLE Cliente
	ADD CONSTRAINT chk_fecha_nacimiento CHECK (fecha_nacimiento < GETDATE());
ALTER TABLE Cliente
	ADD activo BIT NOT NULL DEFAULT 1;


INSERT INTO Cliente (nombre,apellido,dni,fecha_nacimiento, telefono, direccion, email) VALUES ('Miguel','Fernandez',11222333,'2001-07-25', '3794001264', 'Sarmiento 1064', 'miguelfernandez@gmail');
INSERT INTO Cliente (nombre,apellido,dni,fecha_nacimiento, telefono, direccion, email) VALUES ('Mariano','Gonzalez',11333222,'2000-06-19', '3795124679', 'Av. Siempre viva 123', 'marianogonzales@hotmail');
INSERT INTO Cliente (nombre,apellido,dni,fecha_nacimiento, telefono, direccion, email) VALUES ('Juan','Gomez',44210687,'2002-10-21', '3795021687', 'Junín 728', 'juangomez@yahoo');
INSERT INTO Cliente (nombre,apellido,dni,fecha_nacimiento, telefono, direccion, email) VALUES ('Bruno','Meza',45123574,'2003-03-16', '3794215478', 'Yrigoyen 990', 'brunomeza@gmail');
INSERT INTO Cliente (nombre,apellido,dni,fecha_nacimiento, telefono, direccion, email) VALUES ('Ignacio','Vargas',45035987,'2003-03-16', '3794123456', 'Av Centenario', 'nachovargas@gmail');
INSERT INTO Cliente (nombre,apellido,dni,fecha_nacimiento, telefono, direccion, email) VALUES ('Franco','Menentiel',46123456,'2004-03-16', '3794134695', 'Av. 3 de abril 464', 'franco_menentiel316@gmail');
INSERT INTO Cliente (nombre,apellido,dni,fecha_nacimiento, telefono, direccion, email) VALUES ('Joaquin','Espinoza',40326871,'1999-05-21', '3794234864', 'La Rioja 442', 'joaquinEsp@gmail');



SELECT * FROM Cliente;
---DELETE FROM Cliente;


----OBRA SOCIAL-------------------------------------------------------------------------------------------
CREATE TABLE Obra_Social
(
 id_obra_social INT IDENTITY (1,1),
 nombre NVARCHAR(50) NOT NULL,
 CONSTRAINT pk_id_obra_social PRIMARY KEY (id_obra_social),
);

INSERT INTO Obra_Social (nombre) VALUES ('PAMI');
INSERT INTO Obra_Social (nombre) VALUES ('INSSSEP');
INSERT INTO Obra_Social (nombre) VALUES ('OSDE');
SELECT * FROM Obra_Social;


----CLIENTE OBRA SOCIAL-------------------------------------------------------------------------------------------
CREATE TABLE Cliente_Obra_Social
(
	id_cliente INT NOT NULL,
	id_obra_social INT NOT NULL,
	nro_afiliado INT NOT NULL,
	CONSTRAINT fk_cliente_obra_social_cliente FOREIGN KEY (id_cliente) REFERENCES Cliente(id_cliente),
	CONSTRAINT fk_cliente_obra_social_obra_social FOREIGN KEY (id_obra_social) REFERENCES Obra_Social(id_obra_social),
	CONSTRAINT pk_cliente_obra_social PRIMARY KEY (id_cliente, id_obra_social),
	CONSTRAINT uq_cliente_obra_afiliado UNIQUE (id_obra_social,nro_afiliado)
);

INSERT INTO Cliente_Obra_Social (id_cliente,id_obra_social,nro_afiliado) VALUES (1, 1, 89536888);
INSERT INTO Cliente_Obra_Social (id_cliente, id_obra_social,nro_afiliado) VALUES (1, 2, 11246854);
INSERT INTO Cliente_Obra_Social (id_cliente, id_obra_social,nro_afiliado) VALUES (1, 3, 22568472);

INSERT INTO Cliente_Obra_Social (id_cliente, id_obra_social,nro_afiliado) VALUES (2, 1, 11111111);
INSERT INTO Cliente_Obra_Social (id_cliente, id_obra_social,nro_afiliado) VALUES (2, 2, 11111112);
INSERT INTO Cliente_Obra_Social (id_cliente, id_obra_social,nro_afiliado) VALUES (3, 1, 11111113);
INSERT INTO Cliente_Obra_Social (id_cliente, id_obra_social,nro_afiliado) VALUES (3, 3, 11111114);

INSERT INTO Cliente_Obra_Social (id_cliente, id_obra_social,nro_afiliado) VALUES (4, 1, 11111115);
INSERT INTO Cliente_Obra_Social (id_cliente, id_obra_social,nro_afiliado) VALUES (5, 2, 11111116);
INSERT INTO Cliente_Obra_Social (id_cliente, id_obra_social,nro_afiliado) VALUES (6, 3, 11111116);

SELECT * FROM Cliente_Obra_Social;


-----USUARIO--------------------------------------------------------------------------------------------------
CREATE TABLE Usuario
(
  id_usuario INT IDENTITY (1,1),
  nombre VARCHAR(150) NOT NULL,
  apellido VARCHAR(150) NOT NULL,
  email NVARCHAR(50) NOT NULL,
  contraseña VARCHAR(250) NOT NULL,
  rol VARCHAR(50) NOT NULL,
  CONSTRAINT pk_id_usuario PRIMARY KEY (id_usuario),
  CONSTRAINT UQ_email_usuario UNIQUE (email)
);


INSERT INTO Usuario (nombre,apellido,email,contraseña,rol) VALUES ('Nazareno', 'Mendez','nazareno@gmail.com', 123, 'Administrador');
INSERT INTO Usuario (nombre,apellido,email,contraseña,rol) VALUES ('Juan', 'Perez','juan@gmail.com', 123, 'Gerente');
INSERT INTO Usuario (nombre,apellido,email,contraseña,rol) VALUES ('Lalo', 'Garza','lalo@gmail.com', 123, 'Farmaceutico');
SELECT * FROM Usuario;

-----MEDICAMENTO--------------------------------------------------------------------------------------------------
CREATE TABLE Medicamento
(
  id_medicamento INT IDENTITY (1,1),
  nombre_comercial VARCHAR(150) NOT NULL,
  precio_unitario FLOAT NOT NULL,
  presentacion VARCHAR(250) NOT NULL,
  laboratorio VARCHAR(100) NOT NULL,
  stock INT NOT NULL,
  stock_minimo INT NOT NULL,
  CONSTRAINT pk_id_medicamento PRIMARY KEY (id_medicamento),
  CONSTRAINT UQ_nombre_comercial_presentacion_laboratorio UNIQUE (nombre_comercial,presentacion,laboratorio)
);

INSERT INTO Medicamento (nombre_comercial,precio_unitario,presentacion,laboratorio,stock,stock_minimo) VALUES 
						('Paracetamol', 1500.00,'Caja x8 comprimidos 500m','Barbadan', 150, 50);
INSERT INTO Medicamento (nombre_comercial,precio_unitario,presentacion,laboratorio,stock,stock_minimo) VALUES 
						('Clonazepam', 1300.00,'Caja x8 comprimidos 0.5m','Barbadan', 150, 50);
SELECT * FROM Medicamento;


-----VENTA--------------------------------------------------------------------------------------------------
CREATE TABLE Venta
(
  id_venta INT IDENTITY (1,1),
  fecha_venta DATETIME NOT NULL,---- CHECKKK
  id_cliente INT NOT NULL,
  id_usuario INT NOT NULL,
  CONSTRAINT pk_id_venta PRIMARY KEY (id_venta),
  CONSTRAINT fk_venta_cliente FOREIGN KEY (id_cliente) REFERENCES Cliente(id_cliente),
  CONSTRAINT fk_venta_usuario FOREIGN KEY (id_usuario) REFERENCES Usuario(id_usuario)
);


ALTER TABLE Venta
	ADD CONSTRAINT chk_fecha_venta CHECK (fecha_venta <= GETDATE());

INSERT INTO Venta (fecha_venta,id_cliente,id_usuario) VALUES ('2025-07-23',1,3);
INSERT INTO Venta (fecha_venta,id_cliente,id_usuario) VALUES ('2025-09-09',2,3);
INSERT INTO Venta (fecha_venta,id_cliente,id_usuario) VALUES ('2025-09-10',2,3);
INSERT INTO Venta (fecha_venta,id_cliente,id_usuario) VALUES ('2026-09-09',2,3); ----- check activado, da error
SELECT * FROM Venta;


------DETALLE VENTA--------------------------------------------------------------------------------------------------
CREATE TABLE Detalle_Venta
(
  id_venta INT NOT NULL,
  id_medicamento INT NOT NULL,
  cantidad INT NOT NULL,
  subtotal FLOAT NOT NULL,
  CONSTRAINT pk_venta_medicamento PRIMARY KEY (id_venta,id_medicamento),
  CONSTRAINT fk_detalle_venta_venta FOREIGN KEY (id_venta) REFERENCES Venta(id_venta),
  CONSTRAINT fk_detalle_venta_medicamento FOREIGN KEY (id_medicamento) REFERENCES Medicamento(id_medicamento),
);

INSERT INTO Detalle_Venta (id_venta,id_medicamento,cantidad,subtotal) VALUES (1,1,3,4500);
INSERT INTO Detalle_Venta (id_venta,id_medicamento,cantidad,subtotal) VALUES (2,2,1,1300);
INSERT INTO Detalle_Venta (id_venta,id_medicamento,cantidad,subtotal) VALUES (3,1,1,1500);
SELECT * FROM Detalle_Venta;