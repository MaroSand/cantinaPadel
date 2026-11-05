-- Ejecutar sobre cantina_padel después de crear la base con el script principal.
ALTER TABLE instancias_turno
    ADD COLUMN fecha_hora_cancelacion DATETIME NULL,
    ADD COLUMN id_empleado_cancelo INT NULL,
    ADD INDEX idx_instancias_fecha_cancelacion (fecha_hora_cancelacion),
    ADD CONSTRAINT fk_instancias_empleado_cancelo
        FOREIGN KEY (id_empleado_cancelo)
        REFERENCES Empleados (id_empleado)
        ON DELETE SET NULL
        ON UPDATE CASCADE;
