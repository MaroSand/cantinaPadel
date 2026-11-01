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
        SELECT id_empleado INTO v_admin FROM empleados WHERE nombre_usuario = 'admin' AND activo = 1 LIMIT 1;
        SELECT id_empleado INTO v_empleado FROM empleados WHERE nombre_usuario = 'empleado1' AND activo = 1 LIMIT 1;
        SELECT id_cliente INTO v_cliente FROM clientes WHERE email = 'cf@cantinapadel.com' LIMIT 1;
        SELECT id_caja INTO v_caja_fija FROM cajas WHERE id_empleado = v_empleado LIMIT 1;
        SELECT id_proveedor INTO v_proveedor_bebidas FROM proveedores WHERE nombre_empresa = 'Distribuidora Bebidas S.A.' LIMIT 1;
        SELECT id_proveedor INTO v_proveedor_padel FROM proveedores WHERE nombre_empresa = 'Padel Supplies SRL' LIMIT 1;
        SELECT p.id_producto, p.precio_venta INTO v_producto_hora, v_venta_unitaria
        FROM productos p JOIN categorias c ON c.id_categoria = p.id_categoria
        WHERE c.nombre = 'Hora Padel' AND p.nombre = 'Hora Padel' AND p.activo = 1
        ORDER BY p.id_producto LIMIT 1;
        SET v_hora_bruta = ROUND(v_venta_unitaria * 1.21, 2);

        SELECT id_producto, precio_costo, precio_venta, id_proveedor
        INTO v_producto_coca, v_costo, v_venta_unitaria, v_proveedor_anterior
        FROM productos WHERE nombre = 'Coca Cola 500ml' AND activo = 1 ORDER BY id_producto LIMIT 1;
        SET v_coca_bruta = ROUND(v_venta_unitaria * 1.21, 2);

        SELECT id_producto, precio_costo, precio_venta, id_proveedor
        INTO v_producto_gatorade, v_costo, v_venta_unitaria, v_proveedor_anterior
        FROM productos WHERE nombre = 'Gatorade Manzana 500ml' AND activo = 1 ORDER BY id_producto LIMIT 1;
        SET v_gatorade_bruto = ROUND(v_venta_unitaria * 1.21, 2);

        SELECT id_producto, precio_costo, precio_venta, id_proveedor
        INTO v_producto_overgrip, v_costo, v_venta_unitaria, v_proveedor_anterior
        FROM productos WHERE nombre = 'Overgrip Bullpadel x3' AND activo = 1 ORDER BY id_producto LIMIT 1;
        SET v_overgrip_bruto = ROUND(v_venta_unitaria * 1.21, 2);

        IF v_admin IS NULL OR v_empleado IS NULL OR v_cliente IS NULL OR v_caja_fija IS NULL
           OR v_proveedor_bebidas IS NULL OR v_proveedor_padel IS NULL OR v_producto_hora IS NULL
           OR v_producto_coca IS NULL OR v_producto_gatorade IS NULL OR v_producto_overgrip IS NULL THEN
            SIGNAL SQLSTATE '45000'
                SET MESSAGE_TEXT = 'Faltan los datos base del dump demo; no se cargó ningún dato de reportes.';
        END IF;

        -- Compras: los costos se toman del valor actual. Se registra el estado previo
        -- requerido por el flujo real de anulación y se incrementa el stock comprado.
        SELECT precio_costo, precio_venta, id_proveedor
        INTO v_costo, v_venta_unitaria, v_proveedor_anterior FROM productos WHERE id_producto = v_producto_coca;
        INSERT INTO compras (id_proveedor, id_empleado, fecha_compra, numero_comprobante, total, anulada)
        VALUES (v_proveedor_bebidas, v_admin, DATE_SUB(DATE_ADD(CURDATE(), INTERVAL 9 HOUR), INTERVAL 2 DAY), 'DEMO-REPORTES-COMPRA-001', v_costo * 12, 0);
        SET v_compra = LAST_INSERT_ID();
        INSERT INTO detalles_compra
            (id_compra, id_producto, cantidad, precio_unitario, subtotal, costo_anterior, precio_venta_anterior, id_proveedor_anterior)
        VALUES (v_compra, v_producto_coca, 12, v_costo, v_costo * 12, v_costo, v_venta_unitaria, v_proveedor_anterior);
        UPDATE productos SET stock_actual = stock_actual + 12 WHERE id_producto = v_producto_coca;

        SELECT precio_costo, precio_venta, id_proveedor
        INTO v_costo, v_venta_unitaria, v_proveedor_anterior FROM productos WHERE id_producto = v_producto_gatorade;
        INSERT INTO compras (id_proveedor, id_empleado, fecha_compra, numero_comprobante, total, anulada)
        VALUES (v_proveedor_bebidas, v_admin, DATE_SUB(DATE_ADD(CURDATE(), INTERVAL 10 HOUR), INTERVAL 1 DAY), 'DEMO-REPORTES-COMPRA-002', v_costo * 8, 0);
        SET v_compra = LAST_INSERT_ID();
        INSERT INTO detalles_compra
            (id_compra, id_producto, cantidad, precio_unitario, subtotal, costo_anterior, precio_venta_anterior, id_proveedor_anterior)
        VALUES (v_compra, v_producto_gatorade, 8, v_costo, v_costo * 8, v_costo, v_venta_unitaria, v_proveedor_anterior);
        UPDATE productos SET stock_actual = stock_actual + 8 WHERE id_producto = v_producto_gatorade;

        SELECT precio_costo, precio_venta, id_proveedor
        INTO v_costo, v_venta_unitaria, v_proveedor_anterior FROM productos WHERE id_producto = v_producto_overgrip;
        INSERT INTO compras (id_proveedor, id_empleado, fecha_compra, numero_comprobante, total, anulada)
        VALUES (v_proveedor_padel, v_admin, DATE_ADD(CURDATE(), INTERVAL 11 HOUR), 'DEMO-REPORTES-COMPRA-003', v_costo * 4, 0);
        SET v_compra = LAST_INSERT_ID();
        INSERT INTO detalles_compra
            (id_compra, id_producto, cantidad, precio_unitario, subtotal, costo_anterior, precio_venta_anterior, id_proveedor_anterior)
        VALUES (v_compra, v_producto_overgrip, 4, v_costo, v_costo * 4, v_costo, v_venta_unitaria, v_proveedor_anterior);
        UPDATE productos SET stock_actual = stock_actual + 4 WHERE id_producto = v_producto_overgrip;

        -- Turno de caja histórico cerrado: el arqueo y los movimientos coinciden
        -- con las ventas que se insertan abajo, con una diferencia explicada de $300.
        INSERT INTO turnos_caja
            (id_caja, id_empleado, fecha_apertura, monto_apertura, fecha_cierre,
             cierre_efectivo, cierre_tarjeta, cierre_transferencia, efectivo_final,
             efectivo_esperado, diferencia_efectivo, motivo_diferencia, estado)
        VALUES
            (v_caja_fija, v_empleado, DATE_ADD(CURDATE(), INTERVAL 8 HOUR), 20000.00,
             DATE_ADD(CURDATE(), INTERVAL 22 HOUR), 7308.40, 3146.00, v_overgrip_bruto,
             31008.40, 31308.40, -300.00, 'Diferencia de arqueo incluida en datos demo.', 'Cerrada');
        SET v_turno_caja = LAST_INSERT_ID();

        INSERT INTO ingresos_caja (id_caja, id_admin, monto, fecha_ingreso)
        VALUES (v_turno_caja, v_empleado, 5000.00, DATE_ADD(CURDATE(), INTERVAL 9 HOUR));
        INSERT INTO retiros_caja (id_caja, id_empleado, monto, motivo, fecha_retiro)
        VALUES (v_turno_caja, v_admin, 1000.00, 'Retiro de prueba para conciliar el cierre.', DATE_ADD(CURDATE(), INTERVAL 18 HOUR));

        -- Una reserva real de prueba con una instancia en un horario disponible hoy.
        SELECT h.id_horario INTO v_horario
        FROM horarios_cancha h JOIN canchas c ON c.id_cancha = h.id_cancha
        WHERE c.nombre = 'Cancha 1' AND c.activa = 1 AND h.activo = 1
          AND h.dia_semana = CASE DAYOFWEEK(CURDATE())
                WHEN 1 THEN 'Domingo' WHEN 2 THEN 'Lunes' WHEN 3 THEN 'Martes'
                WHEN 4 THEN 'Miercoles' WHEN 5 THEN 'Jueves' WHEN 6 THEN 'Viernes' WHEN 7 THEN 'Sabado' END
          AND TIME_TO_SEC(TIMEDIFF(h.hora_fin, h.hora_inicio)) = 3600
          AND NOT EXISTS (
                SELECT 1 FROM instancias_turno i
                WHERE i.id_horario = h.id_horario AND i.fecha = CURDATE() AND i.estado = 'Activa')
        ORDER BY h.hora_inicio LIMIT 1;
        IF v_horario IS NULL THEN
            SIGNAL SQLSTATE '45000'
                SET MESSAGE_TEXT = 'No hay un horario libre hoy en Cancha 1 para la reserva demo.';
        END IF;
        INSERT INTO turnos_reservados (id_cliente, id_empleado, modalidad, fecha_inicio, fecha_fin, estado)
        VALUES (v_cliente, v_empleado, 'Por dia', CURDATE(), NULL, 'Activo');
        SET v_turno_reservado = LAST_INSERT_ID();
        SET v_turno_efectivo = v_turno_reservado;
        INSERT INTO instancias_turno (id_turno, id_horario, fecha, estado, estado_pago, forma_pago)
        VALUES (v_turno_reservado, v_horario, CURDATE(), 'Activa', 'Pagado', 'Efectivo');

        -- Venta mixta de pádel + almacén en efectivo; el reporte de ventas debe dividirla por tipo.
        SET v_monto_efectivo = v_hora_bruta + v_coca_bruta;
        INSERT INTO ventas
            (id_venta_padre, id_caja, id_cliente, id_empleado, fecha_venta, subtotal, iva, total,
             tipo_comprobante, forma_pago, estado, pagado)
        VALUES (NULL, v_turno_caja, v_cliente, v_empleado, DATE_ADD(CURDATE(), INTERVAL 12 HOUR),
                ROUND(v_monto_efectivo / 1.21, 2), v_monto_efectivo - ROUND(v_monto_efectivo / 1.21, 2),
                v_monto_efectivo, 'B', 'Efectivo', 'Activa', 1);
        SET v_venta = LAST_INSERT_ID();
        INSERT INTO detalles_venta (id_venta, id_movimiento, id_producto, id_turno, precio_unitario, subtotal, pagado)
        VALUES
            (v_venta, NULL, NULL, v_turno_efectivo, v_hora_bruta, v_hora_bruta, 1),
            (v_venta, NULL, v_producto_coca, NULL, v_coca_bruta, v_coca_bruta, 1);
        UPDATE productos SET stock_actual = stock_actual - 1 WHERE id_producto = v_producto_coca;

        -- Venta de dos bebidas por tarjeta (dos detalles, como hace la aplicación).
        SET v_monto_tarjeta = v_gatorade_bruto * 2;
        INSERT INTO ventas
            (id_venta_padre, id_caja, id_cliente, id_empleado, fecha_venta, subtotal, iva, total,
             tipo_comprobante, forma_pago, estado, pagado)
        VALUES (NULL, v_turno_caja, v_cliente, v_empleado, DATE_ADD(CURDATE(), INTERVAL 14 HOUR),
                ROUND(v_monto_tarjeta / 1.21, 2), v_monto_tarjeta - ROUND(v_monto_tarjeta / 1.21, 2),
                v_monto_tarjeta, 'B', 'Tarjeta', 'Activa', 1);
        SET v_venta = LAST_INSERT_ID();
        INSERT INTO detalles_venta (id_venta, id_movimiento, id_producto, id_turno, precio_unitario, subtotal, pagado)
        VALUES (v_venta, NULL, v_producto_gatorade, NULL, v_gatorade_bruto, v_gatorade_bruto, 1),
               (v_venta, NULL, v_producto_gatorade, NULL, v_gatorade_bruto, v_gatorade_bruto, 1);
        UPDATE productos SET stock_actual = stock_actual - 2 WHERE id_producto = v_producto_gatorade;

        -- Venta de almacén por transferencia; permite probar los tres medios de pago
        -- sin inventar un segundo turno de cancha que quizá no esté disponible.
        INSERT INTO ventas
            (id_venta_padre, id_caja, id_cliente, id_empleado, fecha_venta, subtotal, iva, total,
             tipo_comprobante, forma_pago, estado, pagado)
        VALUES (NULL, v_turno_caja, v_cliente, v_empleado, DATE_ADD(CURDATE(), INTERVAL 16 HOUR),
                ROUND(v_overgrip_bruto / 1.21, 2), v_overgrip_bruto - ROUND(v_overgrip_bruto / 1.21, 2),
                v_overgrip_bruto, 'B', 'Transferencia', 'Activa', 1);
        SET v_venta = LAST_INSERT_ID();
        INSERT INTO detalles_venta (id_venta, id_movimiento, id_producto, id_turno, precio_unitario, subtotal, pagado)
        VALUES (v_venta, NULL, v_producto_overgrip, NULL, v_overgrip_bruto, v_overgrip_bruto, 1);
        UPDATE productos SET stock_actual = stock_actual - 1 WHERE id_producto = v_producto_overgrip;

        COMMIT;
        SELECT 'Datos demo de reportes cargados correctamente.' AS resultado;
    ELSE
        SELECT 'El lote demo de reportes ya estaba cargado; no se insertaron duplicados.' AS resultado;
    END IF;
END$$
DELIMITER ;

CALL cargar_datos_demo_reportes();
DROP PROCEDURE IF EXISTS cargar_datos_demo_reportes;
