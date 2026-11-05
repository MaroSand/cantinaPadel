-- Ejecutar una sola vez (o nuevamente de forma segura) sobre cantina_padel.
-- Varios usa precio ingresado al momento de la venta, por eso no tiene margen fijo.
INSERT INTO categorias (nombre, activa, porcentaje_ganancia)
SELECT 'Varios', 1, 0.00
WHERE NOT EXISTS (
    SELECT 1
    FROM categorias
    WHERE LOWER(TRIM(nombre)) = 'varios'
      AND activa = 1
);
