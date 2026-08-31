using cantinaPadel.BLL;

namespace cantinaPadel.UI
{
    public class FrmAlquilerDia : Form
    {
        private readonly LogicaTurnoDia _logica;

        private ComboBox cmbCancha = null!;
        private DateTimePicker dtpFecha = null!;
        private TextBox txtBuscarCliente = null!;
        private Button btnBuscarCliente = null!;
        private DataGridView dgvClientes = null!;
        private DataGridView dgvHorarios = null!;
        private DataGridView dgvReservas = null!;
        private Button btnReservar = null!;
        private Button btnCancelar = null!;
        private Button btnActualizar = null!;
        private Label lblSeleccionHorario = null!;
        private Label lblSeleccionReserva = null!;
        private Label lblClienteSeleccionado = null!;
        private Label lblResumenReserva = null!;
        private RadioButton rdoDia = null!;
        private RadioButton rdoMensual = null!;
        private RadioButton rdoAnual = null!;

        private TimeSpan? _horaInicioSeleccionada;
        private TimeSpan? _horaFinSeleccionada;
        private int _idInstanciaSeleccionada;
        private int _idClienteSeleccionado;
        private System.Windows.Forms.Timer _debounceBusquedaCliente = null!;

        public FrmAlquilerDia()
        {
            _logica = new LogicaTurnoDia();
            InicializarComponentes();
        }

        private void InicializarComponentes()
        {
            Text = "Alquiler por Dia";
            BackColor = Color.White;
            Dock = DockStyle.Fill;
            Padding = new Padding(16);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 8
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 38));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 37));

            var filtros = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                WrapContents = true,
                Padding = new Padding(0, 0, 0, 12)
            };

            cmbCancha = CrearCombo(220);
            cmbCancha.SelectedIndexChanged += (_, _) => RefrescarDatos();

            dtpFecha = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Width = 160,
                MinDate = DateTime.Today
            };
            dtpFecha.ValueChanged += (_, _) =>
            {
                ActualizarResumenReserva();
                RefrescarDatos();
            };

            txtBuscarCliente = new TextBox
            {
                Width = 260,
                Margin = new Padding(0, 0, 8, 8)
            };
            txtBuscarCliente.KeyDown += txtBuscarCliente_KeyDown;

            // Búsqueda en vivo, espera 300ms sin que se tipee nada antes de consultar la bd
            _debounceBusquedaCliente = new System.Windows.Forms.Timer { Interval = 300 };
            _debounceBusquedaCliente.Tick += (_, _) =>
            {
                _debounceBusquedaCliente.Stop();
                BuscarClientes();
            };
            txtBuscarCliente.TextChanged += (_, _) =>
            {
                _debounceBusquedaCliente.Stop();
                _debounceBusquedaCliente.Start();
            };

            btnBuscarCliente = CrearBoton("Buscar cliente");
            btnBuscarCliente.Click += (_, _) => BuscarClientes();

            btnActualizar = CrearBoton("Actualizar");
            btnActualizar.Click += (_, _) => RefrescarDatos();

            btnReservar = CrearBoton("Registrar alquiler");
            btnReservar.Click += btnReservar_Click;

            var modalidad = CrearGrupoModalidad();

            filtros.Controls.Add(CrearCampo("Cancha", cmbCancha));
            filtros.Controls.Add(CrearCampo("Primera fecha", dtpFecha));
            filtros.Controls.Add(modalidad);
            filtros.Controls.Add(CrearCampo("Buscar cliente", txtBuscarCliente));
            filtros.Controls.Add(btnBuscarCliente);
            filtros.Controls.Add(btnActualizar);
            filtros.Controls.Add(btnReservar);

            lblClienteSeleccionado = CrearLeyenda("Cliente seleccionado: ninguno.");
            lblResumenReserva = CrearLeyenda("Reserva: por dia, 1 turno.");

            dgvClientes = CrearGrilla();
            ConfigurarColumnasClientes();
            dgvClientes.SelectionChanged += dgvClientes_SelectionChanged;

            dgvHorarios = CrearGrilla();
            ConfigurarColumnasHorarios();
            dgvHorarios.SelectionChanged += dgvHorarios_SelectionChanged;

            lblSeleccionHorario = CrearLeyenda("Horarios disponibles: seleccione una banda libre.");
            lblSeleccionReserva = CrearLeyenda("Seleccione una reserva activa para cancelar.");

            var reservasHeader = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                WrapContents = true,
                Padding = new Padding(0, 12, 0, 8)
            };

            btnCancelar = CrearBoton("Cancelar turno");
            btnCancelar.ForeColor = Color.DarkRed;
            btnCancelar.Click += btnCancelar_Click;

            reservasHeader.Controls.Add(lblSeleccionReserva);
            reservasHeader.Controls.Add(btnCancelar);

            dgvReservas = CrearGrilla();
            ConfigurarColumnasReservas();
            dgvReservas.SelectionChanged += dgvReservas_SelectionChanged;

            layout.Controls.Add(filtros, 0, 0);
            layout.Controls.Add(lblClienteSeleccionado, 0, 1);
            layout.Controls.Add(lblResumenReserva, 0, 2);
            layout.Controls.Add(dgvClientes, 0, 3);
            layout.Controls.Add(lblSeleccionHorario, 0, 4);
            layout.Controls.Add(dgvHorarios, 0, 5);
            layout.Controls.Add(reservasHeader, 0, 6);
            layout.Controls.Add(dgvReservas, 0, 7);

            Controls.Add(layout);

            Load += FrmAlquilerDia_Load;
        }

        private void FrmAlquilerDia_Load(object? sender, EventArgs e)
        {
            CargarCombos();
            BuscarClientes();
            RefrescarDatos();
        }

        private void CargarCombos()
        {
            var canchas = _logica.ObtenerCanchasActivas();
            cmbCancha.DataSource = canchas;
            cmbCancha.DisplayMember = "Nombre";
            cmbCancha.ValueMember = "IdCancha";

            ActualizarResumenReserva();
        }

        private void RefrescarDatos()
        {
            ActualizarResumenReserva();

            if (cmbCancha?.SelectedValue is int idCancha)
                CargarHorarios(idCancha);

            ActualizarVistaReservas();
        }

        // Si hay un cliente seleccionado desde la búsqueda, la grilla de reservas
        // muestra TODOS sus turnos activos (cualquier fecha/cancha). Si no, vuelve
        // a mostrar las reservas del día y cancha actualmente elegidos.
        private void ActualizarVistaReservas()
        {
            if (_idClienteSeleccionado > 0)
            {
                CargarReservasPorCliente(_idClienteSeleccionado);
            }
            else if (cmbCancha?.SelectedValue is int idCancha)
            {
                CargarReservas(idCancha);
            }
        }

        private void BuscarClientes()
        {
            try
            {
                _idClienteSeleccionado = 0;
                lblClienteSeleccionado.Text = "Cliente seleccionado: ninguno.";

                var clientes = _logica.BuscarClientes(txtBuscarCliente.Text)
                    .Select(c => new
                    {
                        c.IdCliente,
                        Cliente = c.NombreCompleto,
                        c.Dni,
                        c.Email,
                        c.Telefono
                    })
                    .ToList();

                dgvClientes.DataSource = null;
                dgvClientes.DataSource = clientes;

                // Al lanzar una nueva búsqueda se pierde la selección anterior, así que la grilla de reservas vuelve a mostrar el día/cancha actual
                ActualizarVistaReservas();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar clientes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarHorarios(int idCancha)
        {
            try
            {
                _horaInicioSeleccionada = null;
                _horaFinSeleccionada = null;
                var horarios = _logica.ObtenerHorarios(idCancha, dtpFecha.Value.Date)
                    .Select(h => new
                    {
                        h.Horario,
                        h.HoraInicio,
                        h.HoraFin,
                        h.Estado,
                        Precio = h.PrecioHora
                    })
                    .ToList();

                dgvHorarios.DataSource = null;
                dgvHorarios.DataSource = horarios;
                lblSeleccionHorario.Text = horarios.Count == 0
                    ? "Horarios disponibles: no hay franjas configuradas para mostrar."
                    : "Horarios disponibles: seleccione una banda libre.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar horarios: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarReservas(int idCancha)
        {
            try
            {
                _idInstanciaSeleccionada = 0;
                var reservas = _logica.ObtenerReservas(dtpFecha.Value.Date, idCancha)
                    .Select(i => new
                    {
                        i.IdInstancia,
                        Fecha = i.Fecha.ToString("dd/MM/yyyy"),
                        Cancha = i.HorarioCancha.Cancha?.Nombre ?? "-",
                        Horario = $"{i.HorarioCancha.HoraInicio:hh\\:mm} - {i.HorarioCancha.HoraFin:hh\\:mm}",
                        Cliente = $"{i.TurnoReservado.Cliente.Persona.Apellido}, {i.TurnoReservado.Cliente.Persona.Nombre}",
                        Pago = i.EstadoPago,
                        Precio = i.HorarioCancha.Cancha?.PrecioHora ?? 0m
                    })
                    .ToList();

                dgvReservas.DataSource = null;
                dgvReservas.DataSource = reservas;
                lblSeleccionReserva.Text = reservas.Count == 0
                    ? "Reservas del dia: no hay turnos activos para esta cancha y fecha."
                    : "Reservas del dia: seleccione una reserva activa para cancelar.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar reservas: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Muestra en la misma grilla todos los turnos activos del cliente seleccionado, indistinto de la fecha o la cancha
        private void CargarReservasPorCliente(int idCliente)
        {
            try
            {
                _idInstanciaSeleccionada = 0;
                var reservas = _logica.ObtenerReservasPorCliente(idCliente)
                    .Select(i => new
                    {
                        i.IdInstancia,
                        Fecha = i.Fecha.ToString("dd/MM/yyyy"),
                        Cancha = i.HorarioCancha.Cancha?.Nombre ?? "-",
                        Horario = $"{i.HorarioCancha.HoraInicio:hh\\:mm} - {i.HorarioCancha.HoraFin:hh\\:mm}",
                        Cliente = $"{i.TurnoReservado.Cliente.Persona.Apellido}, {i.TurnoReservado.Cliente.Persona.Nombre}",
                        Pago = i.EstadoPago,
                        Precio = i.HorarioCancha.Cancha?.PrecioHora ?? 0m
                    })
                    .ToList();

                dgvReservas.DataSource = null;
                dgvReservas.DataSource = reservas;
                lblSeleccionReserva.Text = reservas.Count == 0
                    ? "Reservas del cliente: no tiene turnos activos reservados."
                    : "Reservas del cliente: seleccione una reserva activa para cancelar.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar las reservas del cliente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReservar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_idClienteSeleccionado <= 0)
                {
                    MessageBox.Show("Busque y seleccione un cliente real para asociar el cobro.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_horaInicioSeleccionada == null || _horaFinSeleccionada == null)
                {
                    MessageBox.Show("Seleccione una banda horaria libre.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cmbCancha.SelectedValue is not int idCancha)
                {
                    MessageBox.Show("Seleccione una cancha.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _logica.RegistrarAlquiler(
                    _idClienteSeleccionado,
                    Sesion.IdUsuario,
                    idCancha,
                    dtpFecha.Value.Date,
                    _horaInicioSeleccionada.Value,
                    _horaFinSeleccionada.Value,
                    ObtenerModalidadSeleccionada());

                MessageBox.Show("Alquiler registrado correctamente. Queda pendiente para el cobro.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefrescarDatos();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RefrescarDatos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object? sender, EventArgs e)
        {
            if (_idInstanciaSeleccionada <= 0)
            {
                MessageBox.Show("Seleccione una reserva para cancelar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmacion = MessageBox.Show(
                "Desea cancelar el turno seleccionado y volver a publicarlo como disponible?",
                "Confirmar cancelacion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes)
                return;

            try
            {
                _logica.CancelarTurno(_idInstanciaSeleccionada);
                MessageBox.Show("Turno cancelado correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefrescarDatos();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvHorarios_SelectionChanged(object? sender, EventArgs e)
        {
            _horaInicioSeleccionada = null;
            _horaFinSeleccionada = null;

            if (dgvHorarios.CurrentRow == null)
                return;

            string estado = dgvHorarios.CurrentRow.Cells["Estado"].Value?.ToString() ?? string.Empty;
            if (!string.Equals(estado, "Libre", StringComparison.OrdinalIgnoreCase))
            {
                lblSeleccionHorario.Text = "La banda seleccionada ya esta ocupada.";
                return;
            }

            if (dgvHorarios.CurrentRow.Cells["HoraInicio"].Value is not TimeSpan horaInicio ||
                dgvHorarios.CurrentRow.Cells["HoraFin"].Value is not TimeSpan horaFin)
            {
                return;
            }

            _horaInicioSeleccionada = horaInicio;
            _horaFinSeleccionada = horaFin;
            lblSeleccionHorario.Text = $"Horario seleccionado: {dgvHorarios.CurrentRow.Cells["Horario"].Value}";
            ActualizarResumenReserva();
        }

        private void dgvClientes_SelectionChanged(object? sender, EventArgs e)
        {
            _idClienteSeleccionado = 0;

            if (dgvClientes.CurrentRow == null)
            {
                ActualizarVistaReservas();
                return;
            }

            object? idValue = dgvClientes.CurrentRow.Cells["IdCliente"].Value;
            if (idValue == null)
            {
                ActualizarVistaReservas();
                return;
            }

            _idClienteSeleccionado = Convert.ToInt32(idValue);
            lblClienteSeleccionado.Text = $"Cliente seleccionado: {dgvClientes.CurrentRow.Cells["Cliente"].Value}";

            // Al elegir un cliente de la búsqueda, la grilla de reservas pasa a mostrar todos sus turnos activos
            ActualizarVistaReservas();
        }

        private void txtBuscarCliente_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            e.SuppressKeyPress = true;
            // frena el timer si estaba corriendo, por si el usuario apreta Enter antes de que pasen los 300ms
            _debounceBusquedaCliente.Stop();
            BuscarClientes();
        }

        private void dgvReservas_SelectionChanged(object? sender, EventArgs e)
        {
            _idInstanciaSeleccionada = 0;

            if (dgvReservas.CurrentRow == null)
                return;

            _idInstanciaSeleccionada = Convert.ToInt32(dgvReservas.CurrentRow.Cells["IdInstancia"].Value);
            lblSeleccionReserva.Text = $"Reserva seleccionada: {dgvReservas.CurrentRow.Cells["Horario"].Value}";
        }

        private string ObtenerModalidadSeleccionada()
        {
            if (rdoMensual.Checked) return LogicaTurnoDia.ModalidadMensual;
            if (rdoAnual.Checked) return LogicaTurnoDia.ModalidadAnual;
            return LogicaTurnoDia.ModalidadDia;
        }

        private void ActualizarResumenReserva()
        {
            string modalidad = ObtenerModalidadSeleccionada();
            DateTime fechaFin = _logica.CalcularFechaFin(modalidad, dtpFecha.Value.Date);
            int cantidadTurnos = _logica.CalcularCantidadTurnos(modalidad, dtpFecha.Value.Date);

            string horario = _horaInicioSeleccionada.HasValue && _horaFinSeleccionada.HasValue
                ? $"{_horaInicioSeleccionada:hh\\:mm} - {_horaFinSeleccionada:hh\\:mm}"
                : "sin horario seleccionado";

            lblResumenReserva.Text = modalidad == LogicaTurnoDia.ModalidadDia
                ? $"Reserva: por dia, {dtpFecha.Value:dd/MM/yyyy}, {horario}."
                : $"Reserva: {modalidad.ToLower()}, del {dtpFecha.Value:dd/MM/yyyy} al {fechaFin:dd/MM/yyyy}, {cantidadTurnos} turnos, {horario}.";
        }

        private GroupBox CrearGrupoModalidad()
        {
            var grupo = new GroupBox
            {
                Text = "Modalidad",
                Width = 270,
                Height = 64,
                Margin = new Padding(0, 0, 16, 0)
            };

            rdoDia = new RadioButton
            {
                Text = "Por dia",
                Checked = true,
                AutoSize = true,
                Location = new Point(10, 28)
            };
            rdoMensual = new RadioButton
            {
                Text = "Mensual",
                AutoSize = true,
                Location = new Point(92, 28)
            };
            rdoAnual = new RadioButton
            {
                Text = "Anual",
                AutoSize = true,
                Location = new Point(185, 28)
            };

            rdoDia.CheckedChanged += Modalidad_CheckedChanged;
            rdoMensual.CheckedChanged += Modalidad_CheckedChanged;
            rdoAnual.CheckedChanged += Modalidad_CheckedChanged;

            grupo.Controls.Add(rdoDia);
            grupo.Controls.Add(rdoMensual);
            grupo.Controls.Add(rdoAnual);

            return grupo;
        }

        private void Modalidad_CheckedChanged(object? sender, EventArgs e)
        {
            if (sender is RadioButton { Checked: true })
                ActualizarResumenReserva();
        }

        private static ComboBox CrearCombo(int width)
        {
            return new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = width,
                Margin = new Padding(0, 0, 16, 8)
            };
        }

        private static Button CrearBoton(string texto)
        {
            return new Button
            {
                Text = texto,
                AutoSize = true,
                Height = 34,
                Margin = new Padding(0, 18, 10, 8),
                Padding = new Padding(12, 0, 12, 0)
            };
        }

        private static Control CrearCampo(string etiqueta, Control control)
        {
            var panel = new Panel
            {
                Width = control.Width + 12,
                Height = 64,
                Margin = new Padding(0, 0, 12, 0)
            };

            var label = new Label
            {
                Text = etiqueta,
                AutoSize = true,
                Location = new Point(0, 0)
            };

            control.Location = new Point(0, 26);
            panel.Controls.Add(label);
            panel.Controls.Add(control);

            return panel;
        }

        private static Label CrearLeyenda(string texto)
        {
            return new Label
            {
                Text = texto,
                AutoSize = true,
                Margin = new Padding(0, 8, 18, 8)
            };
        }

        private static DataGridView CrearGrilla()
        {
            return new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                RowHeadersVisible = false
            };
        }

        private void ConfigurarColumnasHorarios()
        {
            dgvHorarios.Columns.Clear();
            dgvHorarios.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Horario", Name = "Horario", DataPropertyName = "Horario", FillWeight = 35 });
            dgvHorarios.Columns.Add(new DataGridViewTextBoxColumn { Name = "HoraInicio", DataPropertyName = "HoraInicio", Visible = false });
            dgvHorarios.Columns.Add(new DataGridViewTextBoxColumn { Name = "HoraFin", DataPropertyName = "HoraFin", Visible = false });
            dgvHorarios.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Estado", Name = "Estado", DataPropertyName = "Estado", FillWeight = 25 });

            var precio = new DataGridViewTextBoxColumn
            {
                HeaderText = "Precio",
                Name = "Precio",
                DataPropertyName = "Precio",
                FillWeight = 25,
                DefaultCellStyle = { Format = "C2" }
            };
            dgvHorarios.Columns.Add(precio);
        }

        private void ConfigurarColumnasClientes()
        {
            dgvClientes.Columns.Clear();
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { Name = "IdCliente", DataPropertyName = "IdCliente", Visible = false });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Cliente", Name = "Cliente", DataPropertyName = "Cliente", FillWeight = 40 });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "DNI", Name = "Dni", DataPropertyName = "Dni", FillWeight = 18 });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Email", Name = "Email", DataPropertyName = "Email", FillWeight = 30 });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Telefono", Name = "Telefono", DataPropertyName = "Telefono", FillWeight = 22 });
        }

        private void ConfigurarColumnasReservas()
        {
            dgvReservas.Columns.Clear();
            dgvReservas.Columns.Add(new DataGridViewTextBoxColumn { Name = "IdInstancia", DataPropertyName = "IdInstancia", Visible = false });
            dgvReservas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Fecha", Name = "Fecha", DataPropertyName = "Fecha", FillWeight = 18 });
            dgvReservas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Cancha", Name = "Cancha", DataPropertyName = "Cancha", FillWeight = 22 });
            dgvReservas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Horario", Name = "Horario", DataPropertyName = "Horario", FillWeight = 25 });
            dgvReservas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Cliente", Name = "Cliente", DataPropertyName = "Cliente", FillWeight = 45 });
            dgvReservas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Pago", Name = "Pago", DataPropertyName = "Pago", FillWeight = 30 });

            var precio = new DataGridViewTextBoxColumn
            {
                HeaderText = "Precio",
                Name = "Precio",
                DataPropertyName = "Precio",
                FillWeight = 25,
                DefaultCellStyle = { Format = "C2" }
            };
            dgvReservas.Columns.Add(precio);
        }
    }
}