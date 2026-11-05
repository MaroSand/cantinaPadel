DROP DATABASE IF EXISTS cantina_padel;

CREATE DATABASE cantina_padel
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE cantina_padel;

-- ============================================================
-- PERSONAS, EMPLEADOS, CLIENTES Y PROVEEDORES
-- ============================================================

CREATE TABLE personas (
    id_persona INT NOT NULL AUTO_INCREMENT,
    nombre VARCHAR(100) NOT NULL,
    apellido VARCHAR(100) NOT NULL,
    dni VARCHAR(20) NULL UNIQUE,
    cuit VARCHAR(21) NULL,
    condicion_iva VARCHAR(50) NULL,
    telefono VARCHAR(30) NULL,
    direccion VARCHAR(200) NULL,
    activo TINYINT(1) NOT NULL DEFAULT 1,
    fecha_alta DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    es_cliente TINYINT(1) NOT NULL DEFAULT 0,
    es_empleado TINYINT(1) NOT NULL DEFAULT 0,
    es_proveedor TINYINT(1) NOT NULL DEFAULT 0,
    PRIMARY KEY (id_persona)
) ENGINE=InnoDB;

CREATE TABLE clientes (
    id_cliente INT NOT NULL AUTO_INCREMENT,
    id_persona INT NOT NULL,
    email VARCHAR(150) NOT NULL,
    saldo_cuenta_corriente DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    PRIMARY KEY (id_cliente),
    UNIQUE KEY uq_cliente_persona (id_persona),
    CONSTRAINT chk_cliente_saldo_no_negativo
        CHECK (saldo_cuenta_corriente >= 0),
    CONSTRAINT fk_cliente_persona
        FOREIGN KEY (id_persona) REFERENCES personas(id_persona)
        ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE TABLE empleados (
    id_empleado INT NOT NULL AUTO_INCREMENT,
    id_persona INT NOT NULL,
    nombre_usuario VARCHAR(25) NOT NULL,
    contrasena VARCHAR(100) NOT NULL,
    rol VARCHAR(20) NOT NULL DEFAULT 'Empleado',
    activo TINYINT(1) NOT NULL DEFAULT 1,
    PRIMARY KEY (id_empleado),
    UNIQUE KEY uq_empleado_persona (id_persona),
    UNIQUE KEY uq_empleado_usuario (nombre_usuario),
    CONSTRAINT chk_empleado_rol
        CHECK (rol IN ('Admin','Empleado')),
    CONSTRAINT fk_empleado_persona
        FOREIGN KEY (id_persona) REFERENCES personas(id_persona)
        ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE TABLE proveedores (
    id_proveedor INT NOT NULL AUTO_INCREMENT,
    id_persona INT NOT NULL,
    nombre_empresa VARCHAR(100) NOT NULL,
    PRIMARY KEY (id_proveedor),
    UNIQUE KEY uq_proveedor_persona (id_persona),
    CONSTRAINT fk_proveedor_persona
        FOREIGN KEY (id_persona) REFERENCES personas(id_persona)
        ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;

-- ============================================================
-- CATÁLOGO
-- ============================================================

CREATE TABLE categorias (
    id_categoria INT NOT NULL AUTO_INCREMENT,
    nombre VARCHAR(100) NOT NULL,
    activa TINYINT(1) NOT NULL DEFAULT 1,
    porcentaje_ganancia DECIMAL(5,2) NOT NULL DEFAULT 0.00,
    PRIMARY KEY (id_categoria)
) ENGINE=InnoDB;

CREATE TABLE marcas (
    id_marca INT NOT NULL AUTO_INCREMENT,
    nombre VARCHAR(100) NOT NULL,
    activa TINYINT(1) NOT NULL DEFAULT 1,
    PRIMARY KEY (id_marca)
) ENGINE=InnoDB;

CREATE TABLE productos (
    id_producto INT NOT NULL AUTO_INCREMENT,
    id_categoria INT NOT NULL,
    id_marca INT NULL,
    id_proveedor INT NULL,
    nombre VARCHAR(150) NOT NULL,
    codigo_barras VARCHAR(50) NULL UNIQUE,
    precio_costo DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    precio_venta DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    stock_actual INT NOT NULL DEFAULT 0,
    stock_minimo INT NOT NULL DEFAULT 0,
    activo TINYINT(1) NOT NULL DEFAULT 1,
    PRIMARY KEY (id_producto),
    CONSTRAINT fk_producto_categoria
        FOREIGN KEY (id_categoria) REFERENCES categorias(id_categoria)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_producto_marca
        FOREIGN KEY (id_marca) REFERENCES marcas(id_marca)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_producto_proveedor
        FOREIGN KEY (id_proveedor) REFERENCES proveedores(id_proveedor)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB;

-- ============================================================
-- CANCHAS Y HORARIOS
-- ============================================================

CREATE TABLE canchas (
    id_cancha INT NOT NULL AUTO_INCREMENT,
    nombre VARCHAR(100) NOT NULL,
    id_producto INT NOT NULL,
    activa TINYINT(1) NOT NULL DEFAULT 1,
    PRIMARY KEY (id_cancha),
    CONSTRAINT fk_cancha_producto
        FOREIGN KEY (id_producto) REFERENCES productos(id_producto)
) ENGINE=InnoDB;

CREATE TABLE horarios_cancha (
    id_horario INT NOT NULL AUTO_INCREMENT,
    id_cancha INT NOT NULL,
    dia_semana VARCHAR(15) NOT NULL,
    hora_inicio TIME NOT NULL,
    hora_fin TIME NOT NULL,
    activo TINYINT(1) NOT NULL DEFAULT 1,
    PRIMARY KEY (id_horario),
    CONSTRAINT fk_horario_cancha
        FOREIGN KEY (id_cancha) REFERENCES canchas(id_cancha)
        ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;

-- ============================================================
-- CAJA FIJA POR EMPLEADO Y TURNOS DE LA CAJA FÍSICA
-- ============================================================

-- Una fila fija por empleado. No representa un turno ni efectivo separado.
CREATE TABLE cajas (
    id_caja INT NOT NULL AUTO_INCREMENT,
    id_empleado INT NOT NULL,
    PRIMARY KEY (id_caja),
    UNIQUE KEY uq_caja_empleado (id_empleado),
    CONSTRAINT fk_caja_empleado
        FOREIGN KEY (id_empleado) REFERENCES empleados(id_empleado)
) ENGINE=InnoDB;

-- Cada apertura y cierre de la caja física queda como un turno histórico.
CREATE TABLE turnos_caja (
    id_turno_caja INT NOT NULL AUTO_INCREMENT,
    id_caja INT NOT NULL,
    id_empleado INT NOT NULL,
    fecha_apertura DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    monto_apertura DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    fecha_cierre DATETIME NULL,
    cierre_efectivo DECIMAL(12,2) NULL,
    cierre_tarjeta DECIMAL(12,2) NULL,
    cierre_transferencia DECIMAL(12,2) NULL,
    efectivo_final DECIMAL(12,2) NULL,
    efectivo_esperado DECIMAL(12,2) NULL,
    diferencia_efectivo DECIMAL(12,2) NULL,
    motivo_diferencia VARCHAR(250) NULL,
    estado VARCHAR(10) NOT NULL DEFAULT 'Abierta',
    turno_abierto TINYINT
        GENERATED ALWAYS AS (IF(estado = 'Abierta', 1, NULL)) STORED,
    PRIMARY KEY (id_turno_caja),
    UNIQUE KEY uq_un_solo_turno_abierto (turno_abierto),
    INDEX idx_turno_empleado_fecha (id_empleado, fecha_apertura),
    CONSTRAINT chk_turno_caja_monto_apertura
        CHECK (monto_apertura >= 0),
    CONSTRAINT chk_turno_caja_estado
        CHECK (estado IN ('Abierta','Cerrada')),
    CONSTRAINT fk_turno_caja_fija
        FOREIGN KEY (id_caja) REFERENCES cajas(id_caja),
    CONSTRAINT fk_turno_caja_empleado
        FOREIGN KEY (id_empleado) REFERENCES empleados(id_empleado)
) ENGINE=InnoDB;

-- Retiros e ingresos apuntan al turno abierto, no a la caja fija del empleado.
CREATE TABLE retiros_caja (
    id_retiro INT NOT NULL AUTO_INCREMENT,
    id_caja INT NOT NULL,
    id_empleado INT NOT NULL,
    monto DECIMAL(12,2) NOT NULL,
    motivo VARCHAR(300) NULL,
    fecha_retiro DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id_retiro),
    CONSTRAINT fk_retiro_turno_caja
        FOREIGN KEY (id_caja) REFERENCES turnos_caja(id_turno_caja),
    CONSTRAINT fk_retiro_empleado
        FOREIGN KEY (id_empleado) REFERENCES empleados(id_empleado)
) ENGINE=InnoDB;

-- Ingreso de efectivo (cambio): sin contraseña, UNA sola vez por turno de caja.
-- id_admin conserva el nombre histórico, pero guarda al empleado que realizó el ingreso.
CREATE TABLE ingresos_caja (
    id_ingreso INT NOT NULL AUTO_INCREMENT,
    id_caja INT NOT NULL,
    id_admin INT NOT NULL,
    monto DECIMAL(12,2) NOT NULL,
    fecha_ingreso DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id_ingreso),
    UNIQUE KEY uq_un_ingreso_por_turno (id_caja),
    CONSTRAINT fk_ingreso_turno_caja
        FOREIGN KEY (id_caja) REFERENCES turnos_caja(id_turno_caja),
    CONSTRAINT fk_ingreso_admin
        FOREIGN KEY (id_admin) REFERENCES empleados(id_empleado)
) ENGINE=InnoDB;

-- ============================================================
-- RESERVAS DE PÁDEL
-- ============================================================

CREATE TABLE turnos_reservados (
    id_turno INT NOT NULL AUTO_INCREMENT,
    id_cliente INT NOT NULL,
    id_empleado INT NOT NULL,
    modalidad VARCHAR(10) NOT NULL DEFAULT 'Por dia',
    fecha_inicio DATE NOT NULL,
    fecha_fin DATE NULL,
    estado VARCHAR(15) NOT NULL DEFAULT 'Activo',
    PRIMARY KEY (id_turno),
    CONSTRAINT chk_turno_modalidad
        CHECK (modalidad IN ('Por dia','Fijo')),
    CONSTRAINT chk_turno_estado
        CHECK (estado IN ('Activo','Cancelado')),
    CONSTRAINT fk_reserva_cliente
        FOREIGN KEY (id_cliente) REFERENCES clientes(id_cliente),
    CONSTRAINT fk_reserva_empleado
        FOREIGN KEY (id_empleado) REFERENCES empleados(id_empleado)
) ENGINE=InnoDB;

CREATE TABLE instancias_turno (
    id_instancia INT NOT NULL AUTO_INCREMENT,
    id_turno INT NOT NULL,
    id_horario INT NOT NULL,
    fecha DATE NOT NULL,
    estado VARCHAR(15) NOT NULL DEFAULT 'Activa',
    estado_pago VARCHAR(20) NOT NULL DEFAULT 'Pendiente',
    forma_pago VARCHAR(20) NULL,
    fecha_hora_cancelacion DATETIME NULL,
    id_empleado_cancelo INT NULL,
    PRIMARY KEY (id_instancia),
    INDEX idx_instancias_fecha_cancelacion (fecha_hora_cancelacion),
    CONSTRAINT chk_instancia_estado
        CHECK (estado IN ('Activa','Cancelada')),
    CONSTRAINT chk_instancia_estado_pago
        CHECK (estado_pago IN ('Pendiente','Pagado','Cuenta Corriente')),
    CONSTRAINT chk_instancia_forma_pago
        CHECK (
            forma_pago IS NULL OR
            forma_pago IN ('Efectivo','Tarjeta','Transferencia','MercadoPago')
        ),
    CONSTRAINT fk_instancia_turno
        FOREIGN KEY (id_turno) REFERENCES turnos_reservados(id_turno)
        ON DELETE CASCADE,
    CONSTRAINT fk_instancia_horario
        FOREIGN KEY (id_horario) REFERENCES horarios_cancha(id_horario),
    CONSTRAINT fk_instancia_empleado_cancelo
        FOREIGN KEY (id_empleado_cancelo) REFERENCES empleados(id_empleado)
        ON DELETE SET NULL
        ON UPDATE CASCADE
) ENGINE=InnoDB;

-- ============================================================
-- VENTAS
-- ============================================================

-- La columna id_caja conserva el nombre histórico, pero contiene
-- el ID de turnos_caja.id_turno_caja.
CREATE TABLE ventas (
    id_venta INT NOT NULL AUTO_INCREMENT,
    id_venta_padre INT NULL,
    id_caja INT NOT NULL,
    id_cliente INT NOT NULL,
    id_empleado INT NOT NULL,
    fecha_venta DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    subtotal DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    iva DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    total DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    tipo_comprobante CHAR(1) NOT NULL DEFAULT 'B',
    forma_pago VARCHAR(20) NOT NULL,
    estado VARCHAR(10) NOT NULL DEFAULT 'Activa',
    pagado TINYINT(1) NOT NULL DEFAULT 1,
    PRIMARY KEY (id_venta),
    INDEX idx_ventas_padre (id_venta_padre),
    INDEX idx_ventas_pagado (pagado),
    CONSTRAINT chk_venta_comprobante
        CHECK (tipo_comprobante IN ('A','B','C','T','R')),
    CONSTRAINT chk_venta_forma_pago
        CHECK (
            forma_pago IN (
                'Efectivo','Tarjeta','Transferencia',
                'MercadoPago','Cuenta Corriente'
            )
        ),
    CONSTRAINT chk_venta_estado
        CHECK (estado IN ('Activa','Anulada')),
    CONSTRAINT fk_venta_padre
        FOREIGN KEY (id_venta_padre) REFERENCES ventas(id_venta)
        ON DELETE SET NULL ON UPDATE CASCADE,
    CONSTRAINT fk_venta_turno_caja
        FOREIGN KEY (id_caja) REFERENCES turnos_caja(id_turno_caja),
    CONSTRAINT fk_venta_cliente
        FOREIGN KEY (id_cliente) REFERENCES clientes(id_cliente),
    CONSTRAINT fk_venta_empleado
        FOREIGN KEY (id_empleado) REFERENCES empleados(id_empleado)
) ENGINE=InnoDB;

CREATE TABLE detalles_venta (
    id_detalle INT NOT NULL AUTO_INCREMENT,
    id_venta INT NOT NULL,
    id_movimiento INT NULL,
    id_producto INT NULL,
    id_turno INT NULL,
    precio_unitario DECIMAL(12,2) NOT NULL,
    subtotal DECIMAL(12,2) NOT NULL,
    pagado TINYINT(1) NOT NULL DEFAULT 0,
    PRIMARY KEY (id_detalle),
    INDEX idx_detalle_producto_pagado (id_producto, pagado),
    INDEX idx_detalle_movimiento (id_movimiento),
    CONSTRAINT chk_detalle_tipo
        CHECK (
            (id_producto IS NOT NULL AND id_turno IS NULL) OR
            (id_producto IS NULL AND id_turno IS NOT NULL)
        ),
    CONSTRAINT fk_detalle_venta
        FOREIGN KEY (id_venta) REFERENCES ventas(id_venta)
        ON DELETE CASCADE,
    CONSTRAINT fk_detalle_producto
        FOREIGN KEY (id_producto) REFERENCES productos(id_producto),
    CONSTRAINT fk_detalle_turno
        FOREIGN KEY (id_turno) REFERENCES turnos_reservados(id_turno)
) ENGINE=InnoDB;

-- ============================================================
-- COMPRAS A PROVEEDORES
-- ============================================================

CREATE TABLE compras (
    id_compra INT NOT NULL AUTO_INCREMENT,
    id_proveedor INT NOT NULL,
    id_empleado INT NOT NULL,
    fecha_compra DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    numero_comprobante VARCHAR(30) NULL,
    total DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    anulada TINYINT(1) NOT NULL DEFAULT 0,
    PRIMARY KEY (id_compra),
    INDEX idx_compras_fecha (fecha_compra),
    INDEX idx_compras_proveedor_fecha (id_proveedor, fecha_compra),
    CONSTRAINT fk_compra_proveedor
        FOREIGN KEY (id_proveedor) REFERENCES proveedores(id_proveedor),
    CONSTRAINT fk_compra_empleado
        FOREIGN KEY (id_empleado) REFERENCES empleados(id_empleado)
) ENGINE=InnoDB;

CREATE TABLE detalles_compra (
    id_detalle_compra INT NOT NULL AUTO_INCREMENT,
    id_compra INT NOT NULL,
    id_producto INT NOT NULL,
    cantidad INT NOT NULL DEFAULT 1,
    precio_unitario DECIMAL(12,2) NOT NULL,
    subtotal DECIMAL(12,2) NOT NULL,

    -- Estado del producto antes de registrar esta compra.
    -- Se usa para restaurar costo, precio de venta y proveedor al anular la compra.
    costo_anterior DECIMAL(12,2) NULL,
    precio_venta_anterior DECIMAL(12,2) NULL,
    id_proveedor_anterior INT NULL,

    PRIMARY KEY (id_detalle_compra),
    UNIQUE KEY uq_detalle_compra_producto (id_compra, id_producto),
    CONSTRAINT chk_detalle_compra_cantidad
        CHECK (cantidad > 0),
    CONSTRAINT chk_detalle_compra_precio
        CHECK (precio_unitario > 0),
    CONSTRAINT fk_detalle_compra_compra
        FOREIGN KEY (id_compra) REFERENCES compras(id_compra)
        ON DELETE CASCADE,
    CONSTRAINT fk_detalle_compra_producto
        FOREIGN KEY (id_producto) REFERENCES productos(id_producto),
    CONSTRAINT fk_detalle_compra_proveedor_anterior
        FOREIGN KEY (id_proveedor_anterior) REFERENCES proveedores(id_proveedor)
        ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB;

-- ============================================================
-- CUENTA CORRIENTE
-- ============================================================

-- id_caja conserva el nombre de columna antiguo y apunta al turno.
CREATE TABLE movimientos_cuenta_corriente (
    id_movimiento INT NOT NULL AUTO_INCREMENT,
    id_cliente INT NOT NULL,
    id_caja INT NOT NULL,
    id_venta INT NULL,
    id_instancia_turno INT NULL,
    id_empleado INT NOT NULL,
    fecha DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    tipo VARCHAR(10) NOT NULL,
    tipodepago VARCHAR(20) NULL,
    monto DECIMAL(12,2) NOT NULL,
    saldo_posterior DECIMAL(12,2) NOT NULL,
    PRIMARY KEY (id_movimiento),
    INDEX idx_mov_cc_cliente_fecha (id_cliente, fecha),
    CONSTRAINT chk_mov_tipo
        CHECK (tipo IN ('Cargo','Pago')),
    CONSTRAINT chk_mov_tipodepago
        CHECK (
            tipodepago IS NULL OR
            tipodepago IN ('Efectivo','Tarjeta','Transferencia','MercadoPago')
        ),
    CONSTRAINT fk_mov_cliente
        FOREIGN KEY (id_cliente) REFERENCES clientes(id_cliente),
    CONSTRAINT fk_mov_turno_caja
        FOREIGN KEY (id_caja) REFERENCES turnos_caja(id_turno_caja),
    CONSTRAINT fk_mov_venta
        FOREIGN KEY (id_venta) REFERENCES ventas(id_venta),
    CONSTRAINT fk_mov_instancia
        FOREIGN KEY (id_instancia_turno) REFERENCES instancias_turno(id_instancia),
    CONSTRAINT fk_mov_empleado
        FOREIGN KEY (id_empleado) REFERENCES empleados(id_empleado)
) ENGINE=InnoDB;

ALTER TABLE detalles_venta
    ADD CONSTRAINT fk_detalle_movimiento
    FOREIGN KEY (id_movimiento)
    REFERENCES movimientos_cuenta_corriente(id_movimiento)
    ON DELETE SET NULL ON UPDATE CASCADE;

-- ============================================================
-- DATOS DE EJEMPLO
-- ============================================================

INSERT INTO personas
    (nombre, apellido, dni, es_empleado, activo)
VALUES
    ('Admin', 'Sistema', '12345678', 1, 1),
    ('Empleado', 'Uno', '87654321', 1, 1),
    ('Empleado', 'Dos', '79461358', 1, 1);

INSERT INTO empleados (id_persona, nombre_usuario, contrasena, rol)
VALUES
    ((SELECT id_persona FROM personas WHERE dni = '12345678'), 'admin', '12345678', 'Admin'),
    ((SELECT id_persona FROM personas WHERE dni = '87654321'), 'empleado1', '87654321', 'Empleado'),
    ((SELECT id_persona FROM personas WHERE dni = '79461358'), 'empleado2', '79461358', 'Empleado');

-- Crea la caja fija para cada empleado cargado.
INSERT INTO cajas (id_empleado)
SELECT id_empleado FROM empleados;

INSERT INTO personas (nombre, apellido, dni, es_cliente, activo)
VALUES ('Consumidor', 'Final', '00000000', 1, 1);

INSERT INTO clientes (id_persona, email, saldo_cuenta_corriente)
VALUES (
    (SELECT id_persona FROM personas WHERE dni = '00000000'),
    'cf@cantinapadel.com',
    0.00
);

INSERT INTO categorias (nombre, activa, porcentaje_ganancia)
VALUES
    ('Bebidas', 1, 30.00),
    ('Snacks', 1, 30.00),
    ('Lacteos', 1, 30.00),
    ('Sin Categoria', 1, 30.00),
    ('Hora Padel', 1, 30.00),
    ('Varios', 1, 0.00),
    ('Indumentaria y Equipamiento', 1, 35.00),
    ('Grip y Accesorios', 1, 40.00);

INSERT INTO marcas (nombre, activa)
VALUES
    ('Coca-Cola', 1),
    ('Gatorade', 1),
    ('Villavicencio', 1),
    ('Lay''s', 1),
    ('BULLPADEL', 1),
    ('Head', 1),
    ('Saladix', 1),
    ('Sin Marca', 1);

INSERT INTO personas
    (nombre, apellido, dni, cuit, condicion_iva, telefono, direccion, activo, es_proveedor)
VALUES
    ('Juan', 'Pérez', '25111222', '20-25111222-9', 'Responsable Inscripto', '1144445555', 'Av. Corrientes 1234', 1, 1),
    ('Carlos', 'Gómez', '28333444', '30-28333444-8', 'Responsable Inscripto', '1133332222', 'Calle Falsa 123', 1, 1);

INSERT INTO proveedores (id_persona, nombre_empresa)
VALUES
    ((SELECT id_persona FROM personas WHERE dni = '25111222'), 'Distribuidora Bebidas S.A.'),
    ((SELECT id_persona FROM personas WHERE dni = '28333444'), 'Padel Supplies SRL');

INSERT INTO personas
    (nombre, apellido, dni, cuit, condicion_iva, telefono, direccion, activo, es_cliente)
VALUES
    ('Lucas', 'Fernández', '38123456', '20-38123456-3', 'Consumidor Final', '1155667788', 'San Martín 450', 1, 1),
    ('Mariana', 'López', '40987654', '27-40987654-5', 'Consumidor Final', '1122334455', 'Belgrano 890', 1, 1),
    ('Roberto', 'Rossi', '32111999', '20-32111999-1', 'Monotributo', '1199887766', 'Mitre 12', 1, 1);

INSERT INTO clientes (id_persona, email, saldo_cuenta_corriente)
VALUES
    ((SELECT id_persona FROM personas WHERE dni = '38123456'), 'lucas.f@gmail.com', 0.00),
    ((SELECT id_persona FROM personas WHERE dni = '40987654'), 'mariana.lopez@hotmail.com', 0.00),
    ((SELECT id_persona FROM personas WHERE dni = '32111999'), 'rrossi@empresa.com', 0.00);

INSERT INTO productos
    (id_categoria, nombre, precio_venta, activo)
SELECT id_categoria, 'Hora Padel', 5000.00, 1
FROM categorias
WHERE nombre = 'Hora Padel';

INSERT INTO productos
    (id_categoria, nombre, precio_venta, activo)
SELECT id_categoria, 'Media hora Padel', 2500.00, 1
FROM categorias
WHERE nombre = 'Hora Padel';

INSERT INTO canchas (nombre, id_producto, activa)
SELECT 'Cancha 1', id_producto, 1
FROM productos
WHERE nombre = 'Hora Padel';

INSERT INTO productos
    (id_categoria, id_marca, id_proveedor, nombre, codigo_barras, precio_costo, precio_venta, stock_actual, stock_minimo, activo)
VALUES
    ((SELECT id_categoria FROM categorias WHERE nombre = 'Bebidas'), (SELECT id_marca FROM marcas WHERE nombre = 'Coca-Cola'), 1, 'Coca Cola 500ml', '7790001001', 800.00, 1040.00, 48, 12, 1),
    ((SELECT id_categoria FROM categorias WHERE nombre = 'Bebidas'), (SELECT id_marca FROM marcas WHERE nombre = 'Gatorade'), 1, 'Gatorade Manzana 500ml', '7790001002', 1000.00, 1300.00, 24, 6, 1),
    ((SELECT id_categoria FROM categorias WHERE nombre = 'Bebidas'), (SELECT id_marca FROM marcas WHERE nombre = 'Villavicencio'), 1, 'Agua Mineral Sin Gas 500ml', '7790001003', 400.00, 520.00, 60, 20, 1),
    ((SELECT id_categoria FROM categorias WHERE nombre = 'Snacks'), (SELECT id_marca FROM marcas WHERE nombre = 'Lay''s'), 1, 'Papas Klasicas Lay''s 95g', '7790002001', 1200.00, 1560.00, 15, 5, 1),
    ((SELECT id_categoria FROM categorias WHERE nombre = 'Snacks'), (SELECT id_marca FROM marcas WHERE nombre = 'Saladix'), 1, 'Snack Saladix Picadas 80g', '7790002002', 700.00, 910.00, 30, 10, 1),
    ((SELECT id_categoria FROM categorias WHERE nombre = 'Grip y Accesorios'), (SELECT id_marca FROM marcas WHERE nombre = 'BULLPADEL'), 2, 'Overgrip Bullpadel x3', '7790003001', 2500.00, 3500.00, 20, 5, 1),
    ((SELECT id_categoria FROM categorias WHERE nombre = 'Grip y Accesorios'), (SELECT id_marca FROM marcas WHERE nombre = 'Head'), 2, 'Tubo de Pelotas Head Pádel Pro', '7790003002', 7000.00, 9800.00, 12, 4, 1);

-- Catalogo ampliado para cantina de padel en Presidencia Roque Saenz Pena, Chaco.
-- Datos ficticios de prueba: 100 familias x 10 presentaciones = 1.000 productos.
-- Precios ilustrativos en pesos argentinos; actualizar con costos reales.
INSERT INTO categorias (nombre,activa,porcentaje_ganancia) VALUES
('Galletitas',1,30.00),
('Golosinas',1,35.00),
('Almacen',1,25.00),
('Lacteos y Refrigerados',1,30.00),
('Sandwiches y Comidas',1,35.00),
('Helados y Congelados',1,35.00),
('Descartables',1,40.00),
('Higiene y Limpieza',1,35.00);
INSERT INTO marcas (nombre,activa) VALUES
('Pepsi',1),
('Manaos',1),
('Pritty',1),
('Secco',1),
('Cunnington',1),
('Eco de los Andes',1),
('Aquarius',1),
('Doritos',1),
('Cheetos',1),
('Mani King',1),
('Pehuamar',1),
('Krachitos',1),
('Marolio',1),
('Bagley',1),
('Oreo',1),
('Terrabusi',1),
('Don Satur',1),
('Arcor',1),
('Mogul',1),
('Georgalos',1),
('Lheritier',1),
('Dos Corazones',1),
('Playadito',1),
('Amanda',1),
('Nobleza Gaucha',1),
('Taragui',1),
('Lucchetti',1),
('Knorr',1),
('Natura',1),
('La Serenisima',1),
('Sancor',1),
('Tregar',1),
('Milkaut',1),
('Swift',1),
('Paty',1),
('Paladini',1),
('Granja Tres Arroyos',1),
('Hellmanns',1),
('Grido',1),
('Darnel',1),
('Elite',1),
('Wilson',1),
('Nox',1),
('Adidas',1),
('Asics',1),
('Dunlop',1);
CREATE TEMPORARY TABLE tmp_catalogo_seed (categoria VARCHAR(100),marca VARCHAR(100),nombre_base VARCHAR(150),grupo_variante VARCHAR(20),costo_base DECIMAL(12,2)) ENGINE=Memory;
INSERT INTO tmp_catalogo_seed VALUES
('Bebidas','Coca-Cola','Gaseosa cola','bebida',950),
('Bebidas','Pepsi','Gaseosa sin azucar','bebida',1025),
('Bebidas','Manaos','Gaseosa lima-limon','bebida',1100),
('Bebidas','Pritty','Gaseosa naranja','bebida',1175),
('Bebidas','Secco','Agua mineral sin gas','bebida',1250),
('Bebidas','Cunnington','Agua mineral con gas','bebida',1325),
('Bebidas','Villavicencio','Agua saborizada limon','bebida',1400),
('Bebidas','Eco de los Andes','Agua saborizada manzana','bebida',1475),
('Bebidas','Aquarius','Bebida deportiva naranja','bebida',1550),
('Bebidas','Gatorade','Jugo multifruta','bebida',1625),
('Snacks','Lay''s','Papas clasicas','snack',800),
('Snacks','Doritos','Papas cheddar','snack',875),
('Snacks','Cheetos','Nachos queso','snack',950),
('Snacks','Saladix','Chizitos','snack',1025),
('Snacks','Mani King','Snack pizza','snack',1100),
('Snacks','Pehuamar','Mani salado','snack',1175),
('Snacks','Krachitos','Mani tostado','snack',1250),
('Snacks','Marolio','Palitos salados','snack',1325),
('Snacks','Lay''s','Papas tubo','snack',1400),
('Snacks','Saladix','Pochoclo','snack',1475),
('Galletitas','Bagley','Galletitas surtidas','galletita',700),
('Galletitas','Oreo','Galletitas rellenas chocolate','galletita',775),
('Galletitas','Terrabusi','Galletitas de agua','galletita',850),
('Galletitas','Don Satur','Bizcochos salados','galletita',925),
('Galletitas','Marolio','Pepas membrillo','galletita',1000),
('Galletitas','Bagley','Alfajor triple','galletita',1075),
('Galletitas','Terrabusi','Vainillas','galletita',1150),
('Galletitas','Oreo','Galletitas mini','galletita',1225),
('Galletitas','Bagley','Galletitas dulces','galletita',1300),
('Galletitas','Don Satur','Bizcochos de grasa','galletita',1375),
('Golosinas','Arcor','Chocolate con leche','golosina',500),
('Golosinas','Mogul','Caramelos masticables','golosina',575),
('Golosinas','Georgalos','Mani con chocolate','golosina',650),
('Golosinas','Lheritier','Chupetin frutal','golosina',725),
('Golosinas','Dos Corazones','Alfajor simple','golosina',800),
('Golosinas','Terrabusi','Alfajor triple','golosina',875),
('Golosinas','Arcor','Turron de mani','golosina',950),
('Golosinas','Mogul','Gomitas frutales','golosina',1025),
('Golosinas','Georgalos','Mantecol','golosina',1100),
('Golosinas','Arcor','Barra de cereal','golosina',1175),
('Almacen','Marolio','Yerba mate con palo','almacen',1500),
('Almacen','Playadito','Yerba mate tradicional','almacen',1575),
('Almacen','Amanda','Yerba mate suave','almacen',1650),
('Almacen','Nobleza Gaucha','Yerba mate seleccion especial','almacen',1725),
('Almacen','Taragui','Yerba mate clasica','almacen',1800),
('Almacen','Marolio','Azucar comun','almacen',1875),
('Almacen','Marolio','Arroz largo fino','almacen',1950),
('Almacen','Lucchetti','Fideos guiseros','almacen',2025),
('Almacen','Knorr','Caldo de verduras','almacen',2100),
('Almacen','Natura','Aceite de girasol','almacen',2175),
('Lacteos y Refrigerados','La Serenisima','Leche entera larga vida','lacteo',1200),
('Lacteos y Refrigerados','Sancor','Leche descremada','lacteo',1275),
('Lacteos y Refrigerados','Tregar','Yogur bebible frutilla','lacteo',1350),
('Lacteos y Refrigerados','Milkaut','Yogur firme vainilla','lacteo',1425),
('Lacteos y Refrigerados','La Serenisima','Postre chocolate','lacteo',1500),
('Lacteos y Refrigerados','Sancor','Queso crema clasico','lacteo',1575),
('Lacteos y Refrigerados','Tregar','Flan con dulce de leche','lacteo',1650),
('Lacteos y Refrigerados','Milkaut','Crema de leche','lacteo',1725),
('Lacteos y Refrigerados','La Serenisima','Leche chocolatada','lacteo',1800),
('Lacteos y Refrigerados','Sancor','Queso barra feteado','lacteo',1875),
('Sandwiches y Comidas','Swift','Hamburguesa clasica','congelado',1500),
('Sandwiches y Comidas','Paty','Hamburguesa vacuna','congelado',1575),
('Sandwiches y Comidas','Paladini','Salchicha viena','congelado',1650),
('Sandwiches y Comidas','Granja Tres Arroyos','Nuggets de pollo','congelado',1725),
('Sandwiches y Comidas','Hellmanns','Mayonesa clasica','congelado',1800),
('Sandwiches y Comidas','Paladini','Medallon de pollo','congelado',1875),
('Sandwiches y Comidas','Marolio','Pan hamburguesa','congelado',1950),
('Sandwiches y Comidas','Marolio','Pan de pancho','congelado',2025),
('Sandwiches y Comidas','Swift','Jamon cocido feteado','congelado',2100),
('Sandwiches y Comidas','Paladini','Medallon vegetal','congelado',2175),
('Helados y Congelados','Grido','Helado palito de agua','congelado',1000),
('Helados y Congelados','Grido','Helado palito bombon','congelado',1075),
('Helados y Congelados','Grido','Cono dulce de leche','congelado',1150),
('Helados y Congelados','Grido','Pote helado chocolate','congelado',1225),
('Helados y Congelados','Granja Tres Arroyos','Papas prefritas','congelado',1300),
('Helados y Congelados','Granja Tres Arroyos','Medallon pollo congelado','congelado',1375),
('Helados y Congelados','Grido','Helado palito frutal','congelado',1450),
('Helados y Congelados','Grido','Pote helado vainilla','congelado',1525),
('Helados y Congelados','Granja Tres Arroyos','Bastones muzzarella','congelado',1600),
('Helados y Congelados','Grido','Helado palito crema','congelado',1675),
('Descartables','Darnel','Vaso plastico','descartable',500),
('Descartables','Darnel','Plato playo','descartable',575),
('Descartables','Darnel','Cubiertos','descartable',650),
('Descartables','Elite','Servilletas','descartable',725),
('Descartables','Darnel','Bandeja','descartable',800),
('Descartables','Darnel','Vaso termico','descartable',875),
('Descartables','Elite','Papel aluminio','descartable',950),
('Descartables','Darnel','Contenedor','descartable',1025),
('Descartables','Darnel','Sorbetes','descartable',1100),
('Descartables','Elite','Toalla de papel','descartable',1175),
('Grip y Accesorios','BULLPADEL','Overgrip perforado x3','padel',2500),
('Grip y Accesorios','Head','Overgrip liso x3','padel',2575),
('Grip y Accesorios','Wilson','Overgrip tacky x3','padel',2650),
('Grip y Accesorios','BULLPADEL','Protector de marco','padel',2725),
('Grip y Accesorios','Head','Tubo pelotas padel','padel',2800),
('Grip y Accesorios','Wilson','Pelota premium x3','padel',2875),
('Grip y Accesorios','Nox','Muñequera deportiva','padel',2950),
('Grip y Accesorios','Adidas','Medias deportivas','padel',3025),
('Grip y Accesorios','Asics','Toalla deportiva','padel',3100),
('Grip y Accesorios','Dunlop','Bolso paletero','padel',3175);
CREATE TEMPORARY TABLE tmp_variantes (grupo_variante VARCHAR(20),sufijo VARCHAR(40),factor DECIMAL(6,3),orden INT) ENGINE=Memory;
INSERT INTO tmp_variantes VALUES
('bebida','200 ml',0.45,1),
('bebida','354 ml',0.65,2),
('bebida','500 ml',0.85,3),
('bebida','600 ml',1,4),
('bebida','1 L',1.45,5),
('bebida','1.5 L',1.9,6),
('bebida','2 L',2.2,7),
('bebida','2.25 L',2.4,8),
('bebida','3 L',2.9,9),
('bebida','pack x6',5.2,10),
('snack','20 g',0.45,1),
('snack','35 g',0.6,2),
('snack','45 g',0.75,3),
('snack','60 g',0.9,4),
('snack','80 g',1,5),
('snack','100 g',1.2,6),
('snack','120 g',1.4,7),
('snack','150 g',1.65,8),
('snack','200 g',2,9),
('snack','pack x6',5.5,10),
('galletita','90 g',0.55,1),
('galletita','100 g',0.65,2),
('galletita','120 g',0.75,3),
('galletita','130 g',0.85,4),
('galletita','150 g',1,5),
('galletita','170 g',1.15,6),
('galletita','200 g',1.35,7),
('galletita','250 g',1.65,8),
('galletita','pack x3',2.8,9),
('galletita','pack x6',5.2,10),
('golosina','15 g',0.4,1),
('golosina','25 g',0.55,2),
('golosina','30 g',0.65,3),
('golosina','40 g',0.8,4),
('golosina','50 g',1,5),
('golosina','70 g',1.25,6),
('golosina','80 g',1.4,7),
('golosina','100 g',1.7,8),
('golosina','pack x5',3.8,9),
('golosina','pack x10',7.2,10),
('almacen','250 g',0.55,1),
('almacen','400 g',0.75,2),
('almacen','500 g',1,3),
('almacen','1 kg',1.85,4),
('almacen','1.5 kg',2.65,5),
('almacen','2 kg',3.4,6),
('almacen','pack x3',2.8,7),
('almacen','pack x6',5.3,8),
('almacen','pack x10',8.5,9),
('almacen','pack x12',10,10),
('lacteo','120 ml',0.45,1),
('lacteo','200 ml',0.6,2),
('lacteo','500 ml',0.85,3),
('lacteo','1 L',1,4),
('lacteo','1.5 L',1.45,5),
('lacteo','2 L',1.9,6),
('lacteo','pack x2',1.9,7),
('lacteo','pack x4',3.6,8),
('lacteo','pack x6',5.2,9),
('lacteo','pack x12',10,10),
('congelado','200 g',0.5,1),
('congelado','300 g',0.7,2),
('congelado','400 g',0.85,3),
('congelado','500 g',1,4),
('congelado','700 g',1.35,5),
('congelado','1 kg',1.8,6),
('congelado','1.2 kg',2.1,7),
('congelado','1.5 kg',2.5,8),
('congelado','pack x2',1.9,9),
('congelado','pack x3',2.8,10),
('descartable','pack x10',0.7,1),
('descartable','pack x20',1,2),
('descartable','pack x25',1.2,3),
('descartable','pack x50',1.8,4),
('descartable','pack x100',3,5),
('descartable','pack x150',4,6),
('descartable','pack x200',5,7),
('descartable','pack x250',6,8),
('descartable','pack x500',10,9),
('descartable','pack familiar',3.5,10),
('padel','negro',0.8,1),
('padel','azul',0.85,2),
('padel','rojo',0.9,3),
('padel','blanco',0.95,4),
('padel','verde',1,5),
('padel','amarillo',1,6),
('padel','naranja',1.05,7),
('padel','rosa',1.1,8),
('padel','talle S',1,9),
('padel','talle M',1.1,10);
INSERT INTO productos (id_categoria,id_marca,id_proveedor,nombre,codigo_barras,precio_costo,precio_venta,stock_actual,stock_minimo,activo)
SELECT c.id_categoria,m.id_marca,pr.id_proveedor,CONCAT(s.nombre_base,' ',v.sufijo),
 CONCAT('779',LPAD(9000000000+ROW_NUMBER() OVER(ORDER BY s.categoria,s.nombre_base,v.orden),10,'0')),
 ROUND(s.costo_base*v.factor,2),ROUND(ROUND(s.costo_base*v.factor*(1+c.porcentaje_ganancia/100)/10)*10,2),
 MOD(ROW_NUMBER() OVER(ORDER BY s.categoria,s.nombre_base,v.orden)*7,46)+5,
 MOD(ROW_NUMBER() OVER(ORDER BY s.categoria,s.nombre_base,v.orden),8)+3,1
FROM tmp_catalogo_seed s JOIN tmp_variantes v ON v.grupo_variante=s.grupo_variante
JOIN categorias c ON c.nombre=s.categoria JOIN marcas m ON m.nombre=s.marca
JOIN proveedores pr ON pr.nombre_empresa=IF(s.grupo_variante='padel','Padel Supplies SRL','Distribuidora Bebidas S.A.');

-- Productos de la categoría Varios: se cobran con importe manual en Punto de Venta.
INSERT INTO productos
    (id_categoria,id_marca,id_proveedor,nombre,codigo_barras,precio_costo,precio_venta,stock_actual,stock_minimo,activo)
SELECT c.id_categoria,m.id_marca,NULL,v.nombre,CONCAT('7799900000',LPAD(v.n,3,'0')),v.costo,0.00,500,10,1
FROM categorias c
JOIN marcas m ON m.nombre='Sin Marca'
JOIN (
    SELECT 1 n,'Caramelos surtidos' nombre,25.00 costo UNION ALL
    SELECT 2,'Chicles sueltos',30.00 UNION ALL SELECT 3,'Gomitas por peso',40.00 UNION ALL
    SELECT 4,'Maní por puñado',120.00 UNION ALL SELECT 5,'Confites surtidos',35.00 UNION ALL
    SELECT 6,'Alfajores artesanales',350.00 UNION ALL SELECT 7,'Turrones sueltos',90.00 UNION ALL
    SELECT 8,'Barritas de cereal',140.00 UNION ALL SELECT 9,'Chupetines',45.00 UNION ALL
    SELECT 10,'Pastillas de menta',35.00 UNION ALL SELECT 11,'Cereal bañado',65.00 UNION ALL
    SELECT 12,'Galletitas sueltas',50.00 UNION ALL SELECT 13,'Mix de frutos secos',220.00 UNION ALL
    SELECT 14,'Bocaditos de chocolate',85.00 UNION ALL SELECT 15,'Caramelos masticables',25.00 UNION ALL
    SELECT 16,'Pochoclos',80.00 UNION ALL SELECT 17,'Papas fritas por porción',130.00 UNION ALL
    SELECT 18,'Palitos salados por porción',110.00 UNION ALL SELECT 19,'Chocolatines',120.00 UNION ALL
    SELECT 20,'Bombones surtidos',150.00 UNION ALL SELECT 21,'Gomitas ácidas',45.00 UNION ALL
    SELECT 22,'Chicles globo',25.00 UNION ALL SELECT 23,'Nueces por puñado',260.00 UNION ALL
    SELECT 24,'Pasas de uva por puñado',90.00 UNION ALL SELECT 25,'Mix salado',180.00 UNION ALL
    SELECT 26,'Snack de maíz por porción',100.00 UNION ALL SELECT 27,'Tostadas saborizadas',85.00 UNION ALL
    SELECT 28,'Caramelos rellenos',30.00 UNION ALL SELECT 29,'Garrapiñadas',100.00 UNION ALL
    SELECT 30,'Mini golosinas surtidas',75.00
) v ON TRUE
WHERE c.nombre='Varios';
DROP TEMPORARY TABLE tmp_variantes;
DROP TEMPORARY TABLE tmp_catalogo_seed;

INSERT INTO horarios_cancha (id_cancha, dia_semana, hora_inicio, hora_fin, activo)
VALUES
    (1, 'Lunes', '16:00:00', '17:00:00', 1),
    (1, 'Lunes', '17:00:00', '19:00:00', 1),
    (1, 'Lunes', '19:00:00', '20:00:00', 1),
    (1, 'Lunes', '20:00:00', '22:00:00', 1),
    (1, 'Martes', '16:00:00', '17:00:00', 1),
    (1, 'Martes', '17:00:00', '19:00:00', 1),
    (1, 'Martes', '19:00:00', '20:00:00', 1),
    (1, 'Martes', '20:00:00' ,'22:00:00', 1);






-- ============================================================
-- CLIENTES, PROVEEDORES Y TURNOS DE DEMOSTRACION
-- Datos anonimos/ficticios para Presidencia Roque Saenz Pena, Chaco.
-- ============================================================

-- Evita el error MySQL 1137 ("Can't reopen table") usando tablas temporales
-- separadas para cada posición del número.
DROP TEMPORARY TABLE IF EXISTS tmp_numeros;
DROP TEMPORARY TABLE IF EXISTS tmp_digitos_a;
DROP TEMPORARY TABLE IF EXISTS tmp_digitos_b;
DROP TEMPORARY TABLE IF EXISTS tmp_digitos_c;
DROP TEMPORARY TABLE IF EXISTS tmp_digitos;
CREATE TEMPORARY TABLE tmp_digitos_a (n TINYINT NOT NULL PRIMARY KEY);
CREATE TEMPORARY TABLE tmp_digitos_b (n TINYINT NOT NULL PRIMARY KEY);
CREATE TEMPORARY TABLE tmp_digitos_c (n TINYINT NOT NULL PRIMARY KEY);
INSERT INTO tmp_digitos_a (n) VALUES (0),(1),(2),(3),(4),(5),(6),(7),(8),(9);
INSERT INTO tmp_digitos_b (n) VALUES (0),(1),(2),(3),(4),(5),(6),(7),(8),(9);
INSERT INTO tmp_digitos_c (n) VALUES (0),(1),(2),(3),(4),(5),(6),(7),(8),(9);

CREATE TEMPORARY TABLE tmp_numeros (n INT NOT NULL PRIMARY KEY);
INSERT INTO tmp_numeros (n)
SELECT a.n * 100 + b.n * 10 + c.n + 1
FROM tmp_digitos_a a CROSS JOIN tmp_digitos_b b CROSS JOIN tmp_digitos_c c
WHERE a.n * 100 + b.n * 10 + c.n + 1 <= 120;

-- 120 clientes de prueba, con emails .test y DNI de rango reservado para estos datos.
INSERT INTO personas
    (nombre, apellido, dni, telefono, direccion, activo, es_cliente)
SELECT
    'Cliente',
    CONCAT('Demo', LPAD(n, 3, '0')),
    CAST(45000000 + n AS CHAR),
    CONCAT('3644-500', LPAD(n, 4, '0')),
    CONCAT('Calle ', 10 + MOD(n, 50), ' N ', 500 + n, ', Saenz Pena'),
    1,
    1
FROM tmp_numeros;

INSERT INTO clientes (id_persona, email, saldo_cuenta_corriente)
SELECT id_persona, CONCAT('cliente', LPAD(CAST(dni AS UNSIGNED) - 45000000, 3, '0'), '@cantinapadel.test'), 0.00
FROM personas
WHERE CAST(dni AS UNSIGNED) BETWEEN 45000001 AND 45000120;

-- 15 proveedores de ejemplo con contactos y empresas ficticios.
INSERT INTO personas
    (nombre, apellido, dni, condicion_iva, telefono, direccion, activo, es_proveedor)
SELECT
    'Contacto',
    CONCAT('Proveedor', LPAD(n, 3, '0')),
    CAST(46000000 + n AS CHAR),
    'Responsable Inscripto',
    CONCAT('3644-510', LPAD(n, 4, '0')),
    CONCAT('Zona comercial, Saenz Pena, Chaco'),
    1,
    1
FROM tmp_numeros
WHERE n <= 15;

INSERT INTO proveedores (id_persona, nombre_empresa)
SELECT p.id_persona,
    CASE CAST(p.dni AS UNSIGNED) - 46000000
        WHEN 1 THEN 'Distribuidora Chaqueña Norte'
        WHEN 2 THEN 'Mayorista Centro Chaqueño'
        WHEN 3 THEN 'Bebidas del Nordeste'
        WHEN 4 THEN 'Alimentos del Chaco'
        WHEN 5 THEN 'Distribuidora Sáenz Peña'
        WHEN 6 THEN 'Lácteos del Litoral'
        WHEN 7 THEN 'Congelados del Nordeste'
        WHEN 8 THEN 'Golosinas y Snacks Chaqueños'
        WHEN 9 THEN 'Descartables Centro'
        WHEN 10 THEN 'Higiene Comercial Norte'
        WHEN 11 THEN 'Padel Chaco Equipamientos'
        WHEN 12 THEN 'Deportes del Nordeste'
        WHEN 13 THEN 'Almacén Mayorista Chaqueño'
        WHEN 14 THEN 'Distribuidora Ruta 16'
        WHEN 15 THEN 'Bebidas y Alimentos Sáenz Peña'
    END
FROM personas p
WHERE CAST(p.dni AS UNSIGNED) BETWEEN 46000001 AND 46000015;

-- Completa horarios de miércoles a domingo; lunes y martes ya están cargados.
INSERT INTO horarios_cancha (id_cancha, dia_semana, hora_inicio, hora_fin, activo)
SELECT c.id_cancha, d.dia_semana, h.hora_inicio, h.hora_fin, 1
FROM canchas c
CROSS JOIN (
    SELECT 'Miercoles' AS dia_semana UNION ALL
    SELECT 'Jueves' UNION ALL SELECT 'Viernes' UNION ALL
    SELECT 'Sabado' UNION ALL SELECT 'Domingo'
) d
CROSS JOIN (
    SELECT '16:00:00' AS hora_inicio, '17:00:00' AS hora_fin UNION ALL
    SELECT '17:00:00', '19:00:00' UNION ALL
    SELECT '19:00:00', '20:00:00' UNION ALL
    SELECT '20:00:00', '22:00:00'
) h
WHERE c.nombre = 'Cancha 1';

-- 120 reservas, una por fecha, repartidas entre los clientes de prueba.
-- El rango incluye fechas recientes y futuras para que el sistema tenga historial y agenda.
INSERT INTO turnos_reservados
    (id_cliente, id_empleado, modalidad, fecha_inicio, fecha_fin, estado)
SELECT
    cl.id_cliente,
    1 + MOD(n.n, 3),
    'Por dia',
    DATE_ADD(CURDATE(), INTERVAL (n.n - 60) DAY),
    NULL,
    IF(MOD(n.n, 17) = 0, 'Cancelado', 'Activo')
FROM tmp_numeros n
JOIN clientes cl
    ON cl.email = CONCAT('cliente', LPAD(n.n, 3, '0'), '@cantinapadel.test');

INSERT INTO instancias_turno
    (id_turno, id_horario, fecha, estado, estado_pago, forma_pago)
SELECT
    tr.id_turno,
    h.id_horario,
    tr.fecha_inicio,
    IF(MOD(n.n, 17) = 0, 'Cancelada', 'Activa'),
    IF(MOD(n.n, 17) = 0, 'Pendiente', IF(MOD(n.n, 4) = 0, 'Pagado', 'Pendiente')),
    CASE
        WHEN MOD(n.n, 17) = 0 OR MOD(n.n, 4) <> 0 THEN NULL
        WHEN MOD(n.n, 3) = 0 THEN 'Efectivo'
        WHEN MOD(n.n, 3) = 1 THEN 'Tarjeta'
        ELSE 'MercadoPago'
    END
FROM tmp_numeros n
JOIN clientes cl
    ON cl.email = CONCAT('cliente', LPAD(n.n, 3, '0'), '@cantinapadel.test')
JOIN turnos_reservados tr
    ON tr.id_cliente = cl.id_cliente
   AND tr.fecha_inicio = DATE_ADD(CURDATE(), INTERVAL (n.n - 60) DAY)
JOIN horarios_cancha h
    ON h.id_cancha = 1
   AND h.dia_semana = CASE DAYOFWEEK(tr.fecha_inicio)
        WHEN 1 THEN 'Domingo'
        WHEN 2 THEN 'Lunes'
        WHEN 3 THEN 'Martes'
        WHEN 4 THEN 'Miercoles'
        WHEN 5 THEN 'Jueves'
        WHEN 6 THEN 'Viernes'
        WHEN 7 THEN 'Sabado'
    END
   AND h.hora_inicio = CASE MOD(n.n - 1, 4)
        WHEN 0 THEN '16:00:00'
        WHEN 1 THEN '17:00:00'
        WHEN 2 THEN '19:00:00'
        ELSE '20:00:00'
    END;

-- 30 turnos de caja históricos, todos cerrados para no dejar una caja abierta al iniciar.
INSERT INTO turnos_caja
    (id_caja, id_empleado, fecha_apertura, monto_apertura, fecha_cierre,
     cierre_efectivo, cierre_tarjeta, cierre_transferencia, efectivo_final,
     efectivo_esperado, diferencia_efectivo, motivo_diferencia, estado)
SELECT
    c.id_caja,
    c.id_empleado,
    DATE_SUB(NOW(), INTERVAL n.n DAY),
    15000.00 + MOD(n.n * 137, 12000),
    DATE_ADD(DATE_SUB(NOW(), INTERVAL n.n DAY), INTERVAL 8 HOUR),
    45000.00 + MOD(n.n * 811, 30000),
    18000.00 + MOD(n.n * 503, 20000),
    12000.00 + MOD(n.n * 307, 15000),
    30000.00 + MOD(n.n * 811, 30000),
    30000.00 + MOD(n.n * 811, 30000),
    0.00,
    NULL,
    'Cerrada'
FROM tmp_numeros n
JOIN cajas c ON c.id_empleado = 1 + MOD(n.n - 1, 3)
WHERE n.n <= 30;

DROP TEMPORARY TABLE tmp_numeros;
DROP TEMPORARY TABLE tmp_digitos_a;
DROP TEMPORARY TABLE tmp_digitos_b;
DROP TEMPORARY TABLE tmp_digitos_c;

-- Datos ficticios para revisar los reportes de Ventas, Compras y Caja.
-- Ejecutar DESPUÉS del dump que crea y carga cantina_padel.
-- No borra datos ni modifica la estructura. La carga es transaccional e idempotente:
-- si ya existe la factura DEMO-REPORTES-COMPRA-001, no vuelve a cargar el lote.

USE cantina_padel;

DROP PROCEDURE IF EXISTS cargar_datos_demo_reportes;
DELIMITER $$
CREATE PROCEDURE cargar_datos_demo_reportes()
BEGIN
    DECLARE v_lote_existente INT DEFAULT 0;
    DECLARE v_admin INT;
    DECLARE v_empleado INT;
    DECLARE v_cliente INT;
    DECLARE v_caja_fija INT;
    DECLARE v_turno_caja INT;
    DECLARE v_proveedor_bebidas INT;
    DECLARE v_proveedor_padel INT;
    DECLARE v_producto_hora INT;
    DECLARE v_producto_coca INT;
    DECLARE v_producto_gatorade INT;
    DECLARE v_producto_overgrip INT;
    DECLARE v_compra INT;
    DECLARE v_turno_reservado INT;
    DECLARE v_turno_efectivo INT;
    DECLARE v_horario INT;
    DECLARE v_venta INT;
    DECLARE v_costo DECIMAL(12,2);
    DECLARE v_venta_unitaria DECIMAL(12,2);
    DECLARE v_proveedor_anterior INT;
    DECLARE v_hora_bruta DECIMAL(12,2);
    DECLARE v_coca_bruta DECIMAL(12,2);
    DECLARE v_gatorade_bruto DECIMAL(12,2);
    DECLARE v_overgrip_bruto DECIMAL(12,2);
    DECLARE v_monto_efectivo DECIMAL(12,2);
    DECLARE v_monto_tarjeta DECIMAL(12,2);

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    SELECT COUNT(*) INTO v_lote_existente
    FROM compras
    WHERE numero_comprobante = 'DEMO-REPORTES-COMPRA-001';

    IF v_lote_existente = 0 THEN
        START TRANSACTION;

        -- Referencias tomadas por claves de negocio del dump, nunca por IDs supuestos.
        SELECT id_empleado INTO v_admin
        FROM empleados
        WHERE nombre_usuario = 'admin' AND activo = 1
        LIMIT 1;

        SELECT id_empleado INTO v_empleado
        FROM empleados
        WHERE nombre_usuario = 'empleado1' AND activo = 1
        LIMIT 1;

        SELECT id_cliente INTO v_cliente
        FROM clientes
        WHERE email = 'cf@cantinapadel.com'
        LIMIT 1;

        SELECT id_caja INTO v_caja_fija
        FROM cajas
        WHERE id_empleado = v_empleado
        LIMIT 1;

        SELECT id_proveedor INTO v_proveedor_bebidas
        FROM proveedores
        WHERE nombre_empresa = 'Distribuidora Bebidas S.A.'
        LIMIT 1;

        SELECT id_proveedor INTO v_proveedor_padel
        FROM proveedores
        WHERE nombre_empresa = 'Padel Supplies SRL'
        LIMIT 1;

        SELECT p.id_producto, p.precio_venta
        INTO v_producto_hora, v_venta_unitaria
        FROM productos p
        JOIN categorias c ON c.id_categoria = p.id_categoria
        WHERE c.nombre = 'Hora Padel'
          AND p.nombre = 'Hora Padel'
          AND p.activo = 1
        ORDER BY p.id_producto
        LIMIT 1;

        SET v_hora_bruta = ROUND(v_venta_unitaria * 1.21, 2);

        SELECT id_producto, precio_costo, precio_venta, id_proveedor
        INTO v_producto_coca, v_costo, v_venta_unitaria, v_proveedor_anterior
        FROM productos
        WHERE nombre = 'Coca Cola 500ml' AND activo = 1
        ORDER BY id_producto
        LIMIT 1;

        SET v_coca_bruta = ROUND(v_venta_unitaria * 1.21, 2);

        SELECT id_producto, precio_costo, precio_venta, id_proveedor
        INTO v_producto_gatorade, v_costo, v_venta_unitaria, v_proveedor_anterior
        FROM productos
        WHERE nombre = 'Gatorade Manzana 500ml' AND activo = 1
        ORDER BY id_producto
        LIMIT 1;

        SET v_gatorade_bruto = ROUND(v_venta_unitaria * 1.21, 2);

        SELECT id_producto, precio_costo, precio_venta, id_proveedor
        INTO v_producto_overgrip, v_costo, v_venta_unitaria, v_proveedor_anterior
        FROM productos
        WHERE nombre = 'Overgrip Bullpadel x3' AND activo = 1
        ORDER BY id_producto
        LIMIT 1;

        SET v_overgrip_bruto = ROUND(v_venta_unitaria * 1.21, 2);

        IF v_admin IS NULL OR v_empleado IS NULL OR v_cliente IS NULL OR v_caja_fija IS NULL
           OR v_proveedor_bebidas IS NULL OR v_proveedor_padel IS NULL OR v_producto_hora IS NULL
           OR v_producto_coca IS NULL OR v_producto_gatorade IS NULL OR v_producto_overgrip IS NULL THEN
            SIGNAL SQLSTATE '45000'
                SET MESSAGE_TEXT = 'Faltan los datos base del dump demo; no se cargó ningún dato de reportes.';
        END IF;

        -- Compras: se toma el costo actual y se guarda el estado previo
        -- requerido por el flujo real de anulación. También se actualiza el stock.
        SELECT precio_costo, precio_venta, id_proveedor
        INTO v_costo, v_venta_unitaria, v_proveedor_anterior
        FROM productos
        WHERE id_producto = v_producto_coca;

        INSERT INTO compras
            (id_proveedor, id_empleado, fecha_compra, numero_comprobante, total, anulada)
        VALUES
            (v_proveedor_bebidas, v_admin,
             DATE_SUB(DATE_ADD(CURDATE(), INTERVAL 9 HOUR), INTERVAL 2 DAY),
             'DEMO-REPORTES-COMPRA-001', v_costo * 12, 0);

        SET v_compra = LAST_INSERT_ID();

        INSERT INTO detalles_compra
            (id_compra, id_producto, cantidad, precio_unitario, subtotal,
             costo_anterior, precio_venta_anterior, id_proveedor_anterior)
        VALUES
            (v_compra, v_producto_coca, 12, v_costo, v_costo * 12,
             v_costo, v_venta_unitaria, v_proveedor_anterior);

        UPDATE productos
        SET stock_actual = stock_actual + 12
        WHERE id_producto = v_producto_coca;

        SELECT precio_costo, precio_venta, id_proveedor
        INTO v_costo, v_venta_unitaria, v_proveedor_anterior
        FROM productos
        WHERE id_producto = v_producto_gatorade;

        INSERT INTO compras
            (id_proveedor, id_empleado, fecha_compra, numero_comprobante, total, anulada)
        VALUES
            (v_proveedor_bebidas, v_admin,
             DATE_SUB(DATE_ADD(CURDATE(), INTERVAL 10 HOUR), INTERVAL 1 DAY),
             'DEMO-REPORTES-COMPRA-002', v_costo * 8, 0);

        SET v_compra = LAST_INSERT_ID();

        INSERT INTO detalles_compra
            (id_compra, id_producto, cantidad, precio_unitario, subtotal,
             costo_anterior, precio_venta_anterior, id_proveedor_anterior)
        VALUES
            (v_compra, v_producto_gatorade, 8, v_costo, v_costo * 8,
             v_costo, v_venta_unitaria, v_proveedor_anterior);

        UPDATE productos
        SET stock_actual = stock_actual + 8
        WHERE id_producto = v_producto_gatorade;

        SELECT precio_costo, precio_venta, id_proveedor
        INTO v_costo, v_venta_unitaria, v_proveedor_anterior
        FROM productos
        WHERE id_producto = v_producto_overgrip;

        INSERT INTO compras
            (id_proveedor, id_empleado, fecha_compra, numero_comprobante, total, anulada)
        VALUES
            (v_proveedor_padel, v_admin,
             DATE_ADD(CURDATE(), INTERVAL 11 HOUR),
             'DEMO-REPORTES-COMPRA-003', v_costo * 4, 0);

        SET v_compra = LAST_INSERT_ID();

        INSERT INTO detalles_compra
            (id_compra, id_producto, cantidad, precio_unitario, subtotal,
             costo_anterior, precio_venta_anterior, id_proveedor_anterior)
        VALUES
            (v_compra, v_producto_overgrip, 4, v_costo, v_costo * 4,
             v_costo, v_venta_unitaria, v_proveedor_anterior);

        UPDATE productos
        SET stock_actual = stock_actual + 4
        WHERE id_producto = v_producto_overgrip;

        -- Turno de caja cerrado. El arqueo coincide con las ventas y movimientos
        -- cargados abajo, con una diferencia explicada de $300.
        INSERT INTO turnos_caja
            (id_caja, id_empleado, fecha_apertura, monto_apertura, fecha_cierre,
             cierre_efectivo, cierre_tarjeta, cierre_transferencia, efectivo_final,
             efectivo_esperado, diferencia_efectivo, motivo_diferencia, estado)
        VALUES
            (v_caja_fija, v_empleado,
             DATE_ADD(CURDATE(), INTERVAL 8 HOUR), 20000.00,
             DATE_ADD(CURDATE(), INTERVAL 22 HOUR),
             7308.40, 3146.00, v_overgrip_bruto,
             31008.40, 31308.40, -300.00,
             'Diferencia de arqueo incluida en datos demo.',
             'Cerrada');

        SET v_turno_caja = LAST_INSERT_ID();

        INSERT INTO ingresos_caja (id_caja, id_admin, monto, fecha_ingreso)
        VALUES
            (v_turno_caja, v_empleado, 5000.00,
             DATE_ADD(CURDATE(), INTERVAL 9 HOUR));

        INSERT INTO retiros_caja (id_caja, id_empleado, monto, motivo, fecha_retiro)
        VALUES
            (v_turno_caja, v_admin, 1000.00,
             'Retiro de prueba para conciliar el cierre.',
             DATE_ADD(CURDATE(), INTERVAL 18 HOUR));

        -- Una reserva real de prueba con una instancia en un horario disponible hoy.
        SELECT h.id_horario INTO v_horario
        FROM horarios_cancha h
        JOIN canchas c ON c.id_cancha = h.id_cancha
        WHERE c.nombre = 'Cancha 1'
          AND c.activa = 1
          AND h.activo = 1
          AND h.dia_semana = CASE DAYOFWEEK(CURDATE())
                WHEN 1 THEN 'Domingo'
                WHEN 2 THEN 'Lunes'
                WHEN 3 THEN 'Martes'
                WHEN 4 THEN 'Miercoles'
                WHEN 5 THEN 'Jueves'
                WHEN 6 THEN 'Viernes'
                WHEN 7 THEN 'Sabado'
              END
          AND TIME_TO_SEC(TIMEDIFF(h.hora_fin, h.hora_inicio)) = 3600
          AND NOT EXISTS (
                SELECT 1
                FROM instancias_turno i
                WHERE i.id_horario = h.id_horario
                  AND i.fecha = CURDATE()
                  AND i.estado = 'Activa')
        ORDER BY h.hora_inicio
        LIMIT 1;

        IF v_horario IS NULL THEN
            SIGNAL SQLSTATE '45000'
                SET MESSAGE_TEXT = 'No hay un horario libre hoy en Cancha 1 para la reserva demo.';
        END IF;

        INSERT INTO turnos_reservados
            (id_cliente, id_empleado, modalidad, fecha_inicio, fecha_fin, estado)
        VALUES
            (v_cliente, v_empleado, 'Por dia', CURDATE(), NULL, 'Activo');

        SET v_turno_reservado = LAST_INSERT_ID();
        SET v_turno_efectivo = v_turno_reservado;

        INSERT INTO instancias_turno
            (id_turno, id_horario, fecha, estado, estado_pago, forma_pago)
        VALUES
            (v_turno_reservado, v_horario, CURDATE(), 'Activa', 'Pagado', 'Efectivo');

        -- Venta mixta de pádel y almacén en efectivo.
        -- El reporte de ventas debe dividirla por tipo.
        SET v_monto_efectivo = v_hora_bruta + v_coca_bruta;

        INSERT INTO ventas
            (id_venta_padre, id_caja, id_cliente, id_empleado, fecha_venta,
             subtotal, iva, total, tipo_comprobante, forma_pago, estado, pagado)
        VALUES
            (NULL, v_turno_caja, v_cliente, v_empleado,
             DATE_ADD(CURDATE(), INTERVAL 12 HOUR),
             ROUND(v_monto_efectivo / 1.21, 2),
             v_monto_efectivo - ROUND(v_monto_efectivo / 1.21, 2),
             v_monto_efectivo, 'B', 'Efectivo', 'Activa', 1);

        SET v_venta = LAST_INSERT_ID();

        INSERT INTO detalles_venta
            (id_venta, id_movimiento, id_producto, id_turno, precio_unitario, subtotal, pagado)
        VALUES
            (v_venta, NULL, NULL, v_turno_efectivo, v_hora_bruta, v_hora_bruta, 1),
            (v_venta, NULL, v_producto_coca, NULL, v_coca_bruta, v_coca_bruta, 1);

        UPDATE productos
        SET stock_actual = stock_actual - 1
        WHERE id_producto = v_producto_coca;

        -- Venta de dos bebidas por tarjeta (dos detalles, como hace la aplicación).
        SET v_monto_tarjeta = v_gatorade_bruto * 2;

        INSERT INTO ventas
            (id_venta_padre, id_caja, id_cliente, id_empleado, fecha_venta,
             subtotal, iva, total, tipo_comprobante, forma_pago, estado, pagado)
        VALUES
            (NULL, v_turno_caja, v_cliente, v_empleado,
             DATE_ADD(CURDATE(), INTERVAL 14 HOUR),
             ROUND(v_monto_tarjeta / 1.21, 2),
             v_monto_tarjeta - ROUND(v_monto_tarjeta / 1.21, 2),
             v_monto_tarjeta, 'B', 'Tarjeta', 'Activa', 1);

        SET v_venta = LAST_INSERT_ID();

        INSERT INTO detalles_venta
            (id_venta, id_movimiento, id_producto, id_turno, precio_unitario, subtotal, pagado)
        VALUES
            (v_venta, NULL, v_producto_gatorade, NULL, v_gatorade_bruto, v_gatorade_bruto, 1),
            (v_venta, NULL, v_producto_gatorade, NULL, v_gatorade_bruto, v_gatorade_bruto, 1);

        UPDATE productos
        SET stock_actual = stock_actual - 2
        WHERE id_producto = v_producto_gatorade;

        -- Venta de almacén por transferencia; permite probar los tres medios de pago
        -- sin inventar un segundo turno de cancha que quizá no esté disponible.
        INSERT INTO ventas
            (id_venta_padre, id_caja, id_cliente, id_empleado, fecha_venta,
             subtotal, iva, total, tipo_comprobante, forma_pago, estado, pagado)
        VALUES
            (NULL, v_turno_caja, v_cliente, v_empleado,
             DATE_ADD(CURDATE(), INTERVAL 16 HOUR),
             ROUND(v_overgrip_bruto / 1.21, 2),
             v_overgrip_bruto - ROUND(v_overgrip_bruto / 1.21, 2),
             v_overgrip_bruto, 'B', 'Transferencia', 'Activa', 1);

        SET v_venta = LAST_INSERT_ID();

        INSERT INTO detalles_venta
            (id_venta, id_movimiento, id_producto, id_turno, precio_unitario, subtotal, pagado)
        VALUES
            (v_venta, NULL, v_producto_overgrip, NULL, v_overgrip_bruto, v_overgrip_bruto, 1);

        UPDATE productos
        SET stock_actual = stock_actual - 1
        WHERE id_producto = v_producto_overgrip;

        COMMIT;
    ELSE
        -- El lote demo ya estaba cargado; no se insertan duplicados.
        DO 0;
    END IF;
END$$
DELIMITER ;

CALL cargar_datos_demo_reportes();
DROP PROCEDURE IF EXISTS cargar_datos_demo_reportes;

-- ============================================================
-- CARGA MASIVA PARA PRUEBAS (100.000 ventas en ~13,7 años)
-- Amplía el período y volumen de los reportes de ventas, compras y caja.
-- Todo es ficticio. El dump borra y recrea la BD al comienzo, por eso
-- esta carga se ejecuta una sola vez en cada instalación limpia.
-- ============================================================

DROP TEMPORARY TABLE IF EXISTS tmp_max_a;
DROP TEMPORARY TABLE IF EXISTS tmp_max_b;
DROP TEMPORARY TABLE IF EXISTS tmp_max_c;
DROP TEMPORARY TABLE IF EXISTS tmp_max_d;
DROP TEMPORARY TABLE IF EXISTS tmp_max_e;
DROP TEMPORARY TABLE IF EXISTS tmp_max_n;
DROP TEMPORARY TABLE IF EXISTS tmp_max_clientes;
DROP TEMPORARY TABLE IF EXISTS tmp_max_empleados;
DROP TEMPORARY TABLE IF EXISTS tmp_max_cajas;
DROP TEMPORARY TABLE IF EXISTS tmp_max_almacen;
DROP TEMPORARY TABLE IF EXISTS tmp_max_hora;
DROP TEMPORARY TABLE IF EXISTS tmp_max_proveedores;
DROP TEMPORARY TABLE IF EXISTS tmp_max_ventas;

CREATE TEMPORARY TABLE tmp_max_a (n TINYINT NOT NULL PRIMARY KEY);
CREATE TEMPORARY TABLE tmp_max_b (n TINYINT NOT NULL PRIMARY KEY);
CREATE TEMPORARY TABLE tmp_max_c (n TINYINT NOT NULL PRIMARY KEY);
CREATE TEMPORARY TABLE tmp_max_d (n TINYINT NOT NULL PRIMARY KEY);
CREATE TEMPORARY TABLE tmp_max_e (n TINYINT NOT NULL PRIMARY KEY);
INSERT INTO tmp_max_a VALUES (0),(1),(2),(3),(4),(5),(6),(7),(8),(9);
INSERT INTO tmp_max_b SELECT n FROM tmp_max_a;
INSERT INTO tmp_max_c SELECT n FROM tmp_max_a;
INSERT INTO tmp_max_d SELECT n FROM tmp_max_a;
INSERT INTO tmp_max_e SELECT n FROM tmp_max_a;

CREATE TEMPORARY TABLE tmp_max_n (n INT NOT NULL PRIMARY KEY);
INSERT INTO tmp_max_n (n)
SELECT a.n*10000+b.n*1000+c.n*100+d.n*10+e.n+1
FROM tmp_max_a a CROSS JOIN tmp_max_b b CROSS JOIN tmp_max_c c
CROSS JOIN tmp_max_d d CROSS JOIN tmp_max_e e;

CREATE TEMPORARY TABLE tmp_max_clientes (rn INT PRIMARY KEY, id_cliente INT NOT NULL);
INSERT INTO tmp_max_clientes
SELECT ROW_NUMBER() OVER (ORDER BY id_cliente), id_cliente FROM clientes;
CREATE TEMPORARY TABLE tmp_max_empleados (rn INT PRIMARY KEY, id_empleado INT NOT NULL);
INSERT INTO tmp_max_empleados
SELECT ROW_NUMBER() OVER (ORDER BY id_empleado), id_empleado FROM empleados WHERE activo=1;
CREATE TEMPORARY TABLE tmp_max_cajas (rn INT PRIMARY KEY, id_caja INT NOT NULL, id_empleado INT NOT NULL);
INSERT INTO tmp_max_cajas
SELECT ROW_NUMBER() OVER (ORDER BY c.id_caja), c.id_caja, c.id_empleado
FROM cajas c JOIN empleados e ON e.id_empleado=c.id_empleado WHERE e.activo=1;
CREATE TEMPORARY TABLE tmp_max_almacen (rn INT PRIMARY KEY, id_producto INT NOT NULL, precio_costo DECIMAL(12,2), precio_venta DECIMAL(12,2));
INSERT INTO tmp_max_almacen
SELECT ROW_NUMBER() OVER (ORDER BY p.id_producto), p.id_producto, p.precio_costo, p.precio_venta
FROM productos p JOIN categorias c ON c.id_categoria=p.id_categoria
WHERE LOWER(c.nombre)='almacen' AND p.activo=1;
CREATE TEMPORARY TABLE tmp_max_hora (id_producto INT PRIMARY KEY, precio_venta DECIMAL(12,2));
INSERT INTO tmp_max_hora
SELECT p.id_producto,p.precio_venta FROM productos p JOIN categorias c ON c.id_categoria=p.id_categoria
WHERE c.nombre='Hora Padel' AND p.nombre='Hora Padel' AND p.activo=1
ORDER BY p.id_producto LIMIT 1;
CREATE TEMPORARY TABLE tmp_max_proveedores (rn INT PRIMARY KEY, id_proveedor INT NOT NULL);
INSERT INTO tmp_max_proveedores
SELECT ROW_NUMBER() OVER (ORDER BY id_proveedor), id_proveedor FROM proveedores;

-- Cada venta queda asociada a una caja cerrada de la misma fecha.
-- 5.000 días de caja y 100.000 ventas distribuidas (20 por día).
INSERT INTO turnos_caja
    (id_caja,id_empleado,fecha_apertura,monto_apertura,fecha_cierre,cierre_efectivo,cierre_tarjeta,
     cierre_transferencia,efectivo_final,efectivo_esperado,diferencia_efectivo,motivo_diferencia,estado)
SELECT ca.id_caja,ca.id_empleado,
       DATE_ADD(DATE_ADD(DATE_SUB(CURDATE(),INTERVAL 4999 DAY),INTERVAL n.n-1 DAY),INTERVAL 8 HOUR),
       20000, DATE_ADD(DATE_ADD(DATE_SUB(CURDATE(),INTERVAL 4999 DAY),INTERVAL n.n-1 DAY),INTERVAL 16 HOUR),
       45000+MOD(n.n*173,80000),12000+MOD(n.n*97,30000),8000+MOD(n.n*53,20000),
       45000+MOD(n.n*173,80000),45000+MOD(n.n*173,80000),0,NULL,'Cerrada'
FROM tmp_max_n n JOIN tmp_max_cajas ca ON ca.rn=MOD(n.n-1,(SELECT COUNT(*) FROM tmp_max_cajas))+1
WHERE n.n<=5000;

CREATE TEMPORARY TABLE tmp_max_ventas (
    n INT PRIMARY KEY,id_cliente INT NOT NULL,id_empleado INT NOT NULL,id_turno_caja INT NOT NULL,
    fecha DATETIME NOT NULL,id_producto INT NOT NULL,precio DECIMAL(12,2) NOT NULL,
    total DECIMAL(12,2) NOT NULL,forma_pago VARCHAR(20) NOT NULL,estado VARCHAR(10) NOT NULL
);
INSERT INTO tmp_max_ventas
SELECT n.n,cl.id_cliente,em.id_empleado,tc.id_turno_caja,
       DATE_ADD(DATE_ADD(DATE_ADD(DATE_ADD(DATE_SUB(CURDATE(),INTERVAL 4999 DAY),INTERVAL FLOOR((n.n-1)/20) DAY),
                         INTERVAL 9+MOD(n.n,10) HOUR),INTERVAL MOD(n.n,60) MINUTE),INTERVAL MOD(n.n,59)+1 SECOND),
       prod.id_producto,prod.precio_venta,ROUND(prod.precio_venta*1.21,2),
       CASE MOD(n.n,4) WHEN 0 THEN 'Efectivo' WHEN 1 THEN 'Tarjeta' WHEN 2 THEN 'Transferencia' ELSE 'MercadoPago' END,
       IF(MOD(n.n,97)=0,'Anulada','Activa')
FROM tmp_max_n n
JOIN tmp_max_clientes cl ON cl.rn=MOD(n.n-1,(SELECT COUNT(*) FROM tmp_max_clientes))+1
JOIN tmp_max_empleados em ON em.rn=MOD(n.n-1,(SELECT COUNT(*) FROM tmp_max_empleados))+1
JOIN tmp_max_cajas ca ON ca.rn=MOD(n.n-1,(SELECT COUNT(*) FROM tmp_max_cajas))+1
JOIN turnos_caja tc ON tc.id_caja=ca.id_caja
    AND tc.fecha_apertura=DATE_ADD(DATE_ADD(DATE_SUB(CURDATE(),INTERVAL 4999 DAY),INTERVAL FLOOR((n.n-1)/20) DAY),INTERVAL 8 HOUR)
LEFT JOIN tmp_max_hora hora ON MOD(n.n,5)=0
LEFT JOIN tmp_max_almacen al ON MOD(n.n,5)<>0 AND al.rn=MOD(n.n-1,(SELECT COUNT(*) FROM tmp_max_almacen))+1
JOIN productos prod ON prod.id_producto=COALESCE(hora.id_producto,al.id_producto)
WHERE n.n<=100000;

INSERT INTO ventas
    (id_venta_padre,id_caja,id_cliente,id_empleado,fecha_venta,subtotal,iva,total,tipo_comprobante,forma_pago,estado,pagado)
SELECT NULL,v.id_turno_caja,v.id_cliente,v.id_empleado,v.fecha,
       ROUND(v.total/1.21,2),v.total-ROUND(v.total/1.21,2),v.total,'B',v.forma_pago,v.estado,1
FROM tmp_max_ventas v;

-- El momento de venta es único por día y secuencia, permite asociar el detalle sin asumir IDs autoincrementales.
INSERT INTO detalles_venta (id_venta,id_movimiento,id_producto,id_turno,precio_unitario,subtotal,pagado)
SELECT ve.id_venta,NULL,v.id_producto,NULL,v.total,v.total,1
FROM tmp_max_ventas v JOIN ventas ve
  ON ve.fecha_venta=v.fecha AND ve.id_cliente=v.id_cliente AND ve.id_empleado=v.id_empleado
 AND ve.id_caja=v.id_turno_caja AND ve.id_venta_padre IS NULL;

-- 10.000 compras en los mismos 5.000 días, con comprobantes únicos y productos variados.
INSERT INTO compras (id_proveedor,id_empleado,fecha_compra,numero_comprobante,total,anulada)
SELECT pr.id_proveedor,em.id_empleado,
       DATE_ADD(DATE_ADD(DATE_SUB(CURDATE(),INTERVAL 4999 DAY),INTERVAL FLOOR((n.n-1)/2) DAY),INTERVAL 10+MOD(n.n,3) HOUR),
       CONCAT('MAX-C-',LPAD(n.n,8,'0')),ROUND(prod.precio_costo*(5+MOD(n.n,46)),2),IF(MOD(n.n,89)=0,1,0)
FROM tmp_max_n n
JOIN tmp_max_proveedores pr ON pr.rn=MOD(n.n-1,(SELECT COUNT(*) FROM tmp_max_proveedores))+1
JOIN tmp_max_empleados em ON em.rn=MOD(n.n-1,(SELECT COUNT(*) FROM tmp_max_empleados))+1
JOIN tmp_max_almacen prod ON prod.rn=MOD(n.n-1,(SELECT COUNT(*) FROM tmp_max_almacen))+1
WHERE n.n<=10000;

INSERT INTO detalles_compra
    (id_compra,id_producto,cantidad,precio_unitario,subtotal,costo_anterior,precio_venta_anterior,id_proveedor_anterior)
SELECT co.id_compra,prod.id_producto,5+MOD(n.n,46),prod.precio_costo,
       ROUND(prod.precio_costo*(5+MOD(n.n,46)),2),prod.precio_costo,prod.precio_venta,NULL
FROM tmp_max_n n
JOIN tmp_max_almacen prod ON prod.rn=MOD(n.n-1,(SELECT COUNT(*) FROM tmp_max_almacen))+1
JOIN compras co ON co.numero_comprobante=CONCAT('MAX-C-',LPAD(n.n,8,'0'))
WHERE n.n<=10000;

-- Ajusta el stock al saldo neto de las compras y ventas ficticias cargadas.
UPDATE productos p
LEFT JOIN (
    SELECT dc.id_producto,SUM(dc.cantidad) AS unidades
    FROM detalles_compra dc JOIN compras co ON co.id_compra=dc.id_compra
    WHERE co.numero_comprobante LIKE 'MAX-C-%' AND co.anulada=0
    GROUP BY dc.id_producto
) compras_max ON compras_max.id_producto=p.id_producto
LEFT JOIN (
    SELECT dv.id_producto,COUNT(*) AS unidades
    FROM detalles_venta dv JOIN ventas ve ON ve.id_venta=dv.id_venta
    JOIN productos pr ON pr.id_producto=dv.id_producto
    JOIN categorias ca ON ca.id_categoria=pr.id_categoria
    WHERE ve.fecha_venta>=DATE_SUB(CURDATE(),INTERVAL 4999 DAY)
      AND ve.id_venta_padre IS NULL AND ve.estado='Activa' AND dv.pagado=1
      AND dv.id_producto IS NOT NULL AND LOWER(ca.nombre)='almacen' AND SECOND(ve.fecha_venta)>0
    GROUP BY dv.id_producto
) ventas_max ON ventas_max.id_producto=p.id_producto
SET p.stock_actual=p.stock_actual+COALESCE(compras_max.unidades,0)-COALESCE(ventas_max.unidades,0)
WHERE (compras_max.unidades IS NOT NULL OR ventas_max.unidades IS NOT NULL)
  AND EXISTS (SELECT 1 FROM categorias ca WHERE ca.id_categoria=p.id_categoria AND LOWER(ca.nombre)='almacen');

-- 10.000 reservas e instancias para ampliar agenda e historial de cancelaciones.
CREATE TEMPORARY TABLE tmp_max_reservas (
    n INT PRIMARY KEY,id_cliente INT NOT NULL,id_empleado INT NOT NULL,id_horario INT NOT NULL,
    fecha DATE NOT NULL,estado VARCHAR(15) NOT NULL
);
INSERT INTO tmp_max_reservas
SELECT n.n,cl.id_cliente,em.id_empleado,h.id_horario,
       DATE_ADD(DATE_SUB(CURDATE(),INTERVAL 4999 DAY),INTERVAL FLOOR((n.n-1)/2) DAY),
       IF(MOD(n.n,13)=0,'Cancelado','Activo')
FROM tmp_max_n n
JOIN tmp_max_clientes cl ON cl.rn=MOD(n.n-1,(SELECT COUNT(*) FROM tmp_max_clientes))+1
JOIN tmp_max_empleados em ON em.rn=MOD(n.n-1,(SELECT COUNT(*) FROM tmp_max_empleados))+1
JOIN horarios_cancha h ON h.id_cancha=1 AND h.activo=1
 AND h.dia_semana=CASE DAYOFWEEK(DATE_ADD(DATE_SUB(CURDATE(),INTERVAL 4999 DAY),INTERVAL FLOOR((n.n-1)/2) DAY))
      WHEN 1 THEN 'Domingo' WHEN 2 THEN 'Lunes' WHEN 3 THEN 'Martes' WHEN 4 THEN 'Miercoles'
      WHEN 5 THEN 'Jueves' WHEN 6 THEN 'Viernes' ELSE 'Sabado' END
 AND h.hora_inicio=CASE MOD(n.n,4) WHEN 0 THEN '16:00:00' WHEN 1 THEN '17:00:00'
                                   WHEN 2 THEN '19:00:00' ELSE '20:00:00' END
WHERE n.n<=10000;

INSERT INTO turnos_reservados (id_cliente,id_empleado,modalidad,fecha_inicio,fecha_fin,estado)
SELECT id_cliente,id_empleado,'Por dia',fecha,NULL,estado FROM tmp_max_reservas;
INSERT INTO instancias_turno
    (id_turno,id_horario,fecha,estado,estado_pago,forma_pago,fecha_hora_cancelacion,id_empleado_cancelo)
SELECT tr.id_turno,r.id_horario,r.fecha,
       IF(r.estado='Cancelado','Cancelada','Activa'),
       IF(r.estado='Cancelado' OR MOD(r.n,3)<>0,'Pendiente','Pagado'),
       IF(r.estado='Cancelado' OR MOD(r.n,3)<>0,NULL,
          CASE MOD(r.n,4) WHEN 0 THEN 'Efectivo' WHEN 1 THEN 'Tarjeta' WHEN 2 THEN 'Transferencia' ELSE 'MercadoPago' END),
       IF(r.estado='Cancelado',DATE_ADD(DATE_ADD(r.fecha,INTERVAL 11 HOUR),INTERVAL MOD(r.n,59) MINUTE),NULL),
       IF(r.estado='Cancelado',r.id_empleado,NULL)
FROM tmp_max_reservas r JOIN turnos_reservados tr
  ON tr.id_cliente=r.id_cliente AND tr.id_empleado=r.id_empleado AND tr.fecha_inicio=r.fecha
 AND tr.modalidad='Por dia' AND tr.estado=r.estado;
DROP TEMPORARY TABLE tmp_max_reservas;

DROP TEMPORARY TABLE tmp_max_ventas;
DROP TEMPORARY TABLE tmp_max_proveedores;
DROP TEMPORARY TABLE tmp_max_hora;
DROP TEMPORARY TABLE tmp_max_almacen;
DROP TEMPORARY TABLE tmp_max_cajas;
DROP TEMPORARY TABLE tmp_max_empleados;
DROP TEMPORARY TABLE tmp_max_clientes;
DROP TEMPORARY TABLE tmp_max_n;
DROP TEMPORARY TABLE tmp_max_a;
DROP TEMPORARY TABLE tmp_max_b;
DROP TEMPORARY TABLE tmp_max_c;
DROP TEMPORARY TABLE tmp_max_d;
DROP TEMPORARY TABLE tmp_max_e;




