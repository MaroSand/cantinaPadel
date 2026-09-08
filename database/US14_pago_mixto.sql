-- US-14: permite registrar una venta pagada parcialmente en efectivo y
-- parcialmente por transferencia. Ejecutar una sola vez en la base existente.
ALTER TABLE ventas
    DROP CHECK chk_venta_forma_pago,
    ADD CONSTRAINT chk_venta_forma_pago CHECK (
        forma_pago IN ('Efectivo', 'Transferencia', 'Mixto', 'Tarjeta', 'MercadoPago', 'Cuenta Corriente')
    );
