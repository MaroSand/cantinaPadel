using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.UI
{
    public class FrmAlquilerDia : Form
    {
        private readonly LogicaTurnoDia _logica;

        private ComboBox cmbCancha = null!;
        private DateTimePicker dtpFecha = null!;
        private TextBox txtBuscarCliente = null!;
        private CheckBox chkIncluirCancelados = null!;
        private Panel _panelRaiz = null!;
        private Panel _panelResultadosClientes = null!;
        private ListBox _listaResultadosClientes = null!;
        private Label _lblSinResultados = null!;
        private List<ClienteBusquedaTurno> _clientesEncontrados = new();
        private bool _asignandoTextoCliente;
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

        // Id del cliente del último alquiler registrado en este formulario (0 si no se registró ninguno)
        // El punto de venta lo usa para cargar ese cliente en la venta que se está armando
        public int IdClienteUltimoAlquiler { get; private set; }

        public FrmAlquilerDia()
        {
            _logica = new LogicaTurnoDia();
            InicializarComponentes();
        }

        private void InicializarComponentes()
        {
            Text = "Turnos";
            BackColor = Color.White;
            Dock = DockStyle.Fill;
            Padding = new Padding(16);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 7,
                Margin = Padding.Empty
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 67));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 43));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 67));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 57));

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
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 2, 0, 2),
                Font = new Font("Segoe UI", 10F),
                AccessibleName = "Buscar cliente",
                AccessibleDescription = "Escribí al menos dos letras y elegí un resultado de la lista desplegable.",
                PlaceholderText = "Buscar por apellido, nombre, DNI o email"
            };
            txtBuscarCliente.KeyDown += txtBuscarCliente_KeyDown;
            txtBuscarCliente.Leave += txtBuscarCliente_Leave;

            // Único mecanismo de búsqueda de clientes: búsqueda en vivo (Enter la lanza al instante).
            // Espera 300ms sin que se tipee nada antes de consultar la bd
            _debounceBusquedaCliente = new System.Windows.Forms.Timer { Interval = 300 };
            _debounceBusquedaCliente.Tick += (_, _) =>
            {
                _debounceBusquedaCliente.Stop();
                BuscarClientes();
            };
            Disposed += (_, _) => _debounceBusquedaCliente.Dispose();
            txtBuscarCliente.TextChanged += (_, _) =>
            {
                if (_asignandoTextoCliente)
                    return;

                bool habiaClienteSeleccionado = _idClienteSeleccionado > 0;
                _idClienteSeleccionado = 0;
                lblClienteSeleccionado.Text = "Cliente seleccionado: ninguno.";
                OcultarResultadosClientes();
                ActualizarEstadoBotonReservar();
                if (habiaClienteSeleccionado)
                    ActualizarVistaReservas();

                _debounceBusquedaCliente.Stop();
                _debounceBusquedaCliente.Start();
            };

            chkIncluirCancelados = new CheckBox
            {
                Text = "Incluir cancelados del cliente",
                AutoSize = true,
                Margin = new Padding(0, 26, 16, 8),
                Enabled = false,
                AccessibleDescription = "Disponible al seleccionar un cliente."
            };
            chkIncluirCancelados.CheckedChanged += (_, _) => ActualizarVistaReservas();

            btnActualizar = CrearBoton("Actualizar");
            btnActualizar.Width = 118;
            btnActualizar.Height = 36;
            btnActualizar.Margin = new Padding(0, 18, 8, 8);
            btnActualizar.BackColor = Color.White;
            btnActualizar.UseVisualStyleBackColor = false;
            btnActualizar.Click += (_, _) => RefrescarDatos();

            btnReservar = CrearBoton("Registrar alquiler");
            btnReservar.Width = 168;
            btnReservar.Height = 36;
            btnReservar.Margin = new Padding(8, 0, 0, 0);
            btnReservar.BackColor = Color.Gold;
            btnReservar.FlatStyle = FlatStyle.Flat;
            btnReservar.FlatAppearance.BorderSize = 0;
            btnReservar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnReservar.ForeColor = Color.FromArgb(35, 35, 35);
            btnReservar.UseVisualStyleBackColor = false;
            btnReservar.Enabled = false;
            btnReservar.Click += btnReservar_Click;

            var modalidad = CrearGrupoModalidad();

            filtros.Controls.Add(CrearCampo("Cancha", cmbCancha));
            filtros.Controls.Add(CrearCampo("Fecha inicial", dtpFecha));
            filtros.Controls.Add(modalidad);
            filtros.Controls.Add(btnActualizar);

            lblClienteSeleccionado = CrearLeyenda("Cliente seleccionado: ninguno.");
            lblResumenReserva = CrearLeyenda("Reserva: por día, 1 turno.");
            lblClienteSeleccionado.BackColor = Color.FromArgb(248, 249, 250);
            lblResumenReserva.BackColor = Color.FromArgb(255, 249, 230);

            var estadoReserva = CrearResumenReserva();
            var busquedaCliente = CrearPanelBusquedaCliente();

            dgvHorarios = CrearGrilla();
            ConfigurarColumnasHorarios();
            dgvHorarios.SelectionChanged += dgvHorarios_SelectionChanged;
            dgvHorarios.Enter += (_, _) => OcultarResultadosClientes();

            lblSeleccionHorario = CrearLeyenda("Elegí una banda libre para continuar.");
            lblSeleccionReserva = CrearLeyenda("Elegí una reserva activa para cancelar.");
            var encabezadoHorarios = CrearEncabezadoSeccion("Horarios disponibles", lblSeleccionHorario, btnReservar);

            btnCancelar = CrearBoton("Cancelar turno");
            btnCancelar.Width = 142;
            btnCancelar.Height = 34;
            btnCancelar.Margin = new Padding(8, 0, 0, 0);
            btnCancelar.ForeColor = Color.DarkRed;
            btnCancelar.Enabled = false;
            btnCancelar.Click += btnCancelar_Click;

            chkIncluirCancelados.Text = "Mostrar cancelados";
            chkIncluirCancelados.Margin = new Padding(8, 8, 4, 0);
            var accionesReservas = new FlowLayoutPanel
            {
                AutoSize = true,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            accionesReservas.Controls.Add(chkIncluirCancelados);
            accionesReservas.Controls.Add(btnCancelar);
            var encabezadoReservas = CrearEncabezadoSeccion("Turnos reservados", lblSeleccionReserva, accionesReservas);

            dgvReservas = CrearGrilla();
            ConfigurarColumnasReservas();
            dgvReservas.SelectionChanged += dgvReservas_SelectionChanged;
            dgvReservas.Enter += (_, _) => OcultarResultadosClientes();

            layout.Controls.Add(filtros, 0, 0);
            layout.Controls.Add(busquedaCliente, 0, 1);
            layout.Controls.Add(estadoReserva, 0, 2);
            layout.Controls.Add(encabezadoHorarios, 0, 3);
            layout.Controls.Add(dgvHorarios, 0, 4);
            layout.Controls.Add(encabezadoReservas, 0, 5);
            layout.Controls.Add(dgvReservas, 0, 6);

            _panelRaiz = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            _panelRaiz.Controls.Add(layout);
            InicializarListaResultadosClientes();
            _panelRaiz.Controls.Add(_panelResultadosClientes);
            _panelResultadosClientes.BringToFront();
            _panelRaiz.Resize += (_, _) => PosicionarResultadosClientes();
            Controls.Add(_panelRaiz);

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
            if (cmbCancha?.SelectedValue is int idCancha)
                CargarHorarios(idCancha);

            ActualizarResumenReserva();
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
                string texto = txtBuscarCliente.Text.Trim();
                if (texto.Length < 2)
                {
                    _clientesEncontrados.Clear();
                    _listaResultadosClientes.DataSource = null;
                    OcultarResultadosClientes();
                    return;
                }

                _clientesEncontrados = _logica.BuscarClientes(texto);
                _listaResultadosClientes.DataSource = null;
                _listaResultadosClientes.DataSource = _clientesEncontrados;
                _listaResultadosClientes.DisplayMember = nameof(ClienteBusquedaTurno.NombreCompleto);
                _listaResultadosClientes.SelectedIndex = -1;
                _lblSinResultados.Visible = _clientesEncontrados.Count == 0;
                _listaResultadosClientes.Visible = _clientesEncontrados.Count > 0;
                _lblSinResultados.Text = "No encontramos clientes. Probá con otro nombre, DNI o email.";
                MostrarResultadosClientes();
            }
            catch (Exception ex)
            {
                OcultarResultadosClientes();
                MessageBox.Show($"Error al buscar clientes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InicializarListaResultadosClientes()
        {
            _listaResultadosClientes = new ListBox
            {
                Dock = DockStyle.Fill,
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 44,
                IntegralHeight = false,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9F),
                BackColor = Color.White,
                FormattingEnabled = true,
                Visible = false,
                TabStop = true,
                AccessibleName = "Resultados de clientes",
                AccessibleDescription = "Usá las flechas y Enter para elegir un cliente."
            };
            _listaResultadosClientes.DrawItem += DibujarResultadoCliente;
            _listaResultadosClientes.Format += (_, e) =>
            {
                if (e.ListItem is ClienteBusquedaTurno cliente)
                    e.Value = string.Join(" · ", new[] { cliente.NombreCompleto, cliente.Dni, cliente.Email }
                        .Where(texto => !string.IsNullOrWhiteSpace(texto)));
            };
            _listaResultadosClientes.MouseClick += (_, e) =>
            {
                if (e.Button == MouseButtons.Left)
                    SeleccionarResultadoCliente();
            };
            _listaResultadosClientes.KeyDown += ListaResultadosClientes_KeyDown;

            _lblSinResultados = new Label
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12, 8, 12, 8),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.DimGray,
                BackColor = Color.White,
                Visible = false
            };

            _panelResultadosClientes = new Panel
            {
                Visible = false,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Padding = Padding.Empty
            };
            _panelResultadosClientes.Controls.Add(_listaResultadosClientes);
            _panelResultadosClientes.Controls.Add(_lblSinResultados);
        }

        private void DibujarResultadoCliente(object? sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index < 0 || e.Index >= _listaResultadosClientes.Items.Count ||
                _listaResultadosClientes.Items[e.Index] is not ClienteBusquedaTurno cliente)
                return;

            bool seleccionado = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Color colorTexto = e.ForeColor;
            Color colorDetalle = seleccionado ? e.ForeColor : Color.DimGray;
            var areaNombre = new Rectangle(e.Bounds.X + 10, e.Bounds.Y + 3, e.Bounds.Width - 20, 19);
            var areaDetalle = new Rectangle(e.Bounds.X + 10, e.Bounds.Y + 22, e.Bounds.Width - 20, 18);
            using var fuenteNombre = new Font(_listaResultadosClientes.Font, FontStyle.Bold);
            using var fuenteDetalle = new Font(_listaResultadosClientes.Font.FontFamily, 8.5F, FontStyle.Regular);
            string detalle = string.Join("  ·  ", new[]
            {
                string.IsNullOrWhiteSpace(cliente.Dni) ? null : $"DNI {cliente.Dni}",
                string.IsNullOrWhiteSpace(cliente.Email) ? null : cliente.Email
            }.Where(texto => texto != null));

            TextRenderer.DrawText(e.Graphics, cliente.NombreCompleto, fuenteNombre, areaNombre, colorTexto,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
            TextRenderer.DrawText(e.Graphics, detalle, fuenteDetalle, areaDetalle, colorDetalle,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
            e.DrawFocusRectangle();
        }

        private void MostrarResultadosClientes()
        {
            PosicionarResultadosClientes();
            _panelResultadosClientes.Visible = true;
            _panelResultadosClientes.BringToFront();
        }

        private void PosicionarResultadosClientes()
        {
            if (_panelRaiz == null || _panelResultadosClientes == null || !txtBuscarCliente.IsHandleCreated)
                return;

            Point origen = _panelRaiz.PointToClient(txtBuscarCliente.PointToScreen(Point.Empty));
            int ancho = Math.Max(180, Math.Min(txtBuscarCliente.Width, _panelRaiz.ClientSize.Width - origen.X - 8));
            int altoDeseado = _clientesEncontrados.Count == 0
                ? 48
                : Math.Min(5, _clientesEncontrados.Count) * _listaResultadosClientes.ItemHeight + 2;
            int altoDisponible = Math.Max(44, _panelRaiz.ClientSize.Height - origen.Y - txtBuscarCliente.Height - 8);
            int arriba = origen.Y + txtBuscarCliente.Height + 2;
            if (_clientesEncontrados.Count > 0)
                altoDeseado = Math.Min(altoDeseado, Math.Max(_listaResultadosClientes.ItemHeight, altoDisponible / _listaResultadosClientes.ItemHeight * _listaResultadosClientes.ItemHeight));

            _panelResultadosClientes.Location = new Point(Math.Max(0, origen.X), arriba);
            _panelResultadosClientes.Size = new Size(ancho, Math.Min(altoDeseado, altoDisponible));
        }

        private void OcultarResultadosClientes()
        {
            if (_panelResultadosClientes != null)
                _panelResultadosClientes.Visible = false;
        }

        private void SeleccionarResultadoCliente()
        {
            int indice = _listaResultadosClientes.SelectedIndex;
            if (indice < 0 || indice >= _clientesEncontrados.Count)
                return;

            var cliente = _clientesEncontrados[indice];
            _debounceBusquedaCliente.Stop();
            _idClienteSeleccionado = cliente.IdCliente;
            _asignandoTextoCliente = true;
            txtBuscarCliente.Text = cliente.NombreCompleto;
            txtBuscarCliente.SelectionStart = txtBuscarCliente.TextLength;
            _asignandoTextoCliente = false;
            lblClienteSeleccionado.Text = string.IsNullOrWhiteSpace(cliente.Dni)
                ? $"Cliente seleccionado: {cliente.NombreCompleto}"
                : $"Cliente seleccionado: {cliente.NombreCompleto} · DNI {cliente.Dni}";
            OcultarResultadosClientes();
            ActualizarEstadoBotonReservar();
            ActualizarVistaReservas();
            txtBuscarCliente.Focus();
        }

        private void ListaResultadosClientes_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SeleccionarResultadoCliente();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                OcultarResultadosClientes();
                txtBuscarCliente.Focus();
            }
        }

        private void txtBuscarCliente_Leave(object? sender, EventArgs e)
        {
            if (!IsHandleCreated || IsDisposed)
                return;

            BeginInvoke(new Action(() =>
            {
                if (!IsDisposed && !_panelResultadosClientes.ContainsFocus)
                    OcultarResultadosClientes();
            }));
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
                        h.Estado
                    })
                    .ToList();

                dgvHorarios.DataSource = horarios;
                LimpiarSeleccion(dgvHorarios);
                _horaInicioSeleccionada = null;
                _horaFinSeleccionada = null;
                lblSeleccionHorario.Text = horarios.Count == 0
                    ? "No hay horarios disponibles para esta fecha."
                    : "Elegí una banda libre para continuar.";
                ActualizarEstadoBotonReservar();
                ActualizarResumenReserva();
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
                        FechaValor = i.Fecha,
                        Fecha = i.Fecha.ToString("dd/MM/yyyy"),
                        Cancha = i.HorarioCancha.Cancha?.Nombre ?? "-",
                        Horario = $"{i.HorarioCancha.HoraInicio:hh\\:mm} - {i.HorarioCancha.HoraFin:hh\\:mm}",
                        Cliente = $"{i.TurnoReservado.Cliente.Persona.Apellido}, {i.TurnoReservado.Cliente.Persona.Nombre}",
                        Modalidad = i.TurnoReservado.Modalidad,
                        Estado = i.Estado,
                        Pago = i.EstadoPago
                    })
                    .ToList();

                PaginadorGrilla.Mostrar(dgvReservas, reservas);
                LimpiarSeleccion(dgvReservas);
                _idInstanciaSeleccionada = 0;
                btnCancelar.Enabled = false;
                lblSeleccionReserva.Text = reservas.Count == 0
                    ? "No hay turnos activos para esta cancha y fecha."
                    : "Elegí un turno activo para habilitar su cancelación.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar reservas: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Muestra en la misma grilla todos los turnos del cliente seleccionado, indistinto de la fecha o la cancha.
        // Si chkIncluirCancelados está tildado, trae también los turnos cancelados.
        private void CargarReservasPorCliente(int idCliente)
        {
            try
            {
                _idInstanciaSeleccionada = 0;
                bool incluirCancelados = chkIncluirCancelados.Checked;
                var reservas = _logica.ObtenerReservasPorCliente(idCliente, incluirCancelados)
                    .Select(i => new
                    {
                        i.IdInstancia,
                        FechaValor = i.Fecha,
                        Fecha = i.Fecha.ToString("dd/MM/yyyy"),
                        Cancha = i.HorarioCancha.Cancha?.Nombre ?? "-",
                        Horario = $"{i.HorarioCancha.HoraInicio:hh\\:mm} - {i.HorarioCancha.HoraFin:hh\\:mm}",
                        Cliente = $"{i.TurnoReservado.Cliente.Persona.Apellido}, {i.TurnoReservado.Cliente.Persona.Nombre}",
                        Modalidad = i.TurnoReservado.Modalidad,
                        Estado = i.Estado,
                        Pago = i.EstadoPago
                    })
                    .ToList();

                PaginadorGrilla.Mostrar(dgvReservas, reservas);
                LimpiarSeleccion(dgvReservas);
                _idInstanciaSeleccionada = 0;
                btnCancelar.Enabled = false;
                lblSeleccionReserva.Text = reservas.Count == 0
                    ? "Este cliente no tiene turnos reservados."
                    : "Elegí un turno activo para habilitar su cancelación.";
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

                IdClienteUltimoAlquiler = _idClienteSeleccionado;

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

            var fila = dgvReservas.CurrentRow;
            DateTime fecha = fila != null ? Convert.ToDateTime(fila.Cells["FechaValor"].Value) : DateTime.MinValue;
            string modalidad = fila?.Cells["Modalidad"].Value?.ToString() ?? string.Empty;

            // No se puede cancelar retroactivamente un turno ya ocurrido.
            if (fecha.Date < DateTime.Today)
            {
                MessageBox.Show("No se puede cancelar un turno de una fecha que ya pasó.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // En un turno Fijo (Mensual/Anual), cancelar un día en el medio
            // no cancela solo ese día: se pierde el resto de la reserva desde
            // esa fecha en adelante. El mensaje lo deja explícito.
            string mensaje = modalidad == "Fijo"
                ? $"Esta reserva es Fija. Si cancelás, se pierde la reserva desde el {fecha:dd/MM/yyyy} en adelante (los turnos anteriores a esa fecha no se tocan).\n\n¿Seguro que querés cancelar?"
                : "¿Seguro que querés cancelar este turno?";

            var confirmacion = MessageBox.Show(
                mensaje,
                "Confirmar cancelacion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes)
                return;

            try
            {
                _logica.CancelarTurno(_idInstanciaSeleccionada, Sesion.IdUsuario);
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
            {
                lblSeleccionHorario.Text = "Elegí una banda libre para continuar.";
                ActualizarEstadoBotonReservar();
                ActualizarResumenReserva();
                return;
            }

            string estado = dgvHorarios.CurrentRow.Cells["Estado"].Value?.ToString() ?? string.Empty;
            if (!string.Equals(estado, "Libre", StringComparison.OrdinalIgnoreCase))
            {
                lblSeleccionHorario.Text = string.Equals(estado, "Iniciado", StringComparison.OrdinalIgnoreCase)
                    ? "La banda seleccionada ya empezó o ya pasó."
                    : "La banda seleccionada ya está ocupada.";
                ActualizarEstadoBotonReservar();
                ActualizarResumenReserva();
                return;
            }

            if (dgvHorarios.CurrentRow.Cells["HoraInicio"].Value is not TimeSpan horaInicio ||
                dgvHorarios.CurrentRow.Cells["HoraFin"].Value is not TimeSpan horaFin)
            {
                ActualizarEstadoBotonReservar();
                return;
            }

            _horaInicioSeleccionada = horaInicio;
            _horaFinSeleccionada = horaFin;
            lblSeleccionHorario.Text = $"Horario seleccionado: {dgvHorarios.CurrentRow.Cells["Horario"].Value}";
            ActualizarEstadoBotonReservar();
            ActualizarResumenReserva();
        }

        private void txtBuscarCliente_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
            {
                _debounceBusquedaCliente.Stop();
                if (!_panelResultadosClientes.Visible)
                    BuscarClientes();

                if (_listaResultadosClientes.Items.Count > 0)
                {
                    int indice = _listaResultadosClientes.SelectedIndex;
                    if (e.KeyCode == Keys.Down)
                        _listaResultadosClientes.SelectedIndex = Math.Min(_listaResultadosClientes.Items.Count - 1, indice + 1);
                    else
                        _listaResultadosClientes.SelectedIndex = indice < 0
                            ? _listaResultadosClientes.Items.Count - 1
                            : Math.Max(0, indice - 1);
                }

                e.SuppressKeyPress = true;
                e.Handled = true;
                return;
            }

            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                _debounceBusquedaCliente.Stop();
                if (!_panelResultadosClientes.Visible)
                    BuscarClientes();

                if (_listaResultadosClientes.Items.Count > 0)
                {
                    if (_listaResultadosClientes.SelectedIndex < 0)
                        _listaResultadosClientes.SelectedIndex = 0;
                    SeleccionarResultadoCliente();
                }

                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape && _panelResultadosClientes.Visible)
            {
                OcultarResultadosClientes();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        private void dgvReservas_SelectionChanged(object? sender, EventArgs e)
        {
            _idInstanciaSeleccionada = 0;

            if (dgvReservas.CurrentRow == null)
            {
                lblSeleccionReserva.Text = "Elegí un turno activo para habilitar su cancelación.";
                btnCancelar.Enabled = false;
                return;
            }

            _idInstanciaSeleccionada = Convert.ToInt32(dgvReservas.CurrentRow.Cells["IdInstancia"].Value);
            string estado = dgvReservas.CurrentRow.Cells["Estado"].Value?.ToString() ?? string.Empty;
            DateTime fecha = Convert.ToDateTime(dgvReservas.CurrentRow.Cells["FechaValor"].Value);
            bool estaActiva = string.Equals(estado, InstanciaTurno.EstadoActiva, StringComparison.OrdinalIgnoreCase);
            bool esFutura = fecha.Date >= DateTime.Today;
            btnCancelar.Enabled = estaActiva && esFutura;

            lblSeleccionReserva.Text = !estaActiva
                ? "El turno seleccionado ya está cancelado."
                : !esFutura
                    ? "No se pueden cancelar turnos de fechas pasadas."
                    : $"Turno seleccionado: {fecha:dd/MM/yyyy} · {dgvReservas.CurrentRow.Cells["Horario"].Value}";
        }

        private void ActualizarEstadoBotonReservar()
        {
            if (chkIncluirCancelados != null)
                chkIncluirCancelados.Enabled = _idClienteSeleccionado > 0;

            if (btnReservar == null)
                return;

            btnReservar.Enabled = _idClienteSeleccionado > 0 &&
                _horaInicioSeleccionada.HasValue && _horaFinSeleccionada.HasValue &&
                cmbCancha?.SelectedValue is int idCancha && idCancha > 0;
        }

        private static void LimpiarSeleccion(DataGridView grilla)
        {
            grilla.ClearSelection();
            grilla.CurrentCell = null;
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
                ? $"Reserva: por día, {dtpFecha.Value:dd/MM/yyyy}, {horario}."
                : $"Reserva: {modalidad.ToLower()}, del {dtpFecha.Value:dd/MM/yyyy} al {fechaFin:dd/MM/yyyy}, {cantidadTurnos} turnos, {horario}.";
        }

        private GroupBox CrearGrupoModalidad()
        {
            var grupo = new GroupBox
            {
                Text = "Modalidad",
                Width = 270,
                Height = 64,
                Margin = new Padding(0, 0, 16, 0),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            rdoDia = new RadioButton
            {
                Text = "Por día",
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
                AutoSize = false,
                Height = 36,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Margin = new Padding(0, 18, 10, 8),
                Padding = new Padding(10, 0, 10, 0)
            };
        }

        private Control CrearPanelBusquedaCliente()
        {
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Height = 42,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0, 4, 0, 4),
                Padding = Padding.Empty
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var etiqueta = new Label
            {
                Text = "Cliente para reservar",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Margin = new Padding(0, 0, 12, 0)
            };
            panel.Controls.Add(etiqueta, 0, 0);
            panel.Controls.Add(txtBuscarCliente, 1, 0);
            return panel;
        }

        private Control CrearResumenReserva()
        {
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Height = 38,
                ColumnCount = 2,
                RowCount = 1,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                Margin = new Padding(0, 2, 0, 4),
                Padding = Padding.Empty
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            lblClienteSeleccionado.Dock = DockStyle.Fill;
            lblResumenReserva.Dock = DockStyle.Fill;
            panel.Controls.Add(lblClienteSeleccionado, 0, 0);
            panel.Controls.Add(lblResumenReserva, 1, 0);
            return panel;
        }

        private static Control CrearEncabezadoSeccion(string titulo, Label estado, Control acciones)
        {
            var encabezado = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Height = 58,
                ColumnCount = 2,
                RowCount = 2,
                Margin = new Padding(0, 7, 0, 2),
                Padding = Padding.Empty
            };
            encabezado.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            encabezado.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            encabezado.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            encabezado.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            var etiquetaTitulo = new Label
            {
                Text = titulo,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            };
            estado.Dock = DockStyle.Fill;
            estado.Font = new Font("Segoe UI", 9F);
            estado.ForeColor = Color.DimGray;
            estado.Padding = Padding.Empty;

            var barraAcciones = new FlowLayoutPanel
            {
                AutoSize = true,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                Anchor = AnchorStyles.Right,
                Dock = DockStyle.Right,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            acciones.Margin = acciones is Button ? new Padding(8, 0, 0, 0) : Padding.Empty;
            barraAcciones.Controls.Add(acciones);

            encabezado.Controls.Add(etiquetaTitulo, 0, 0);
            encabezado.Controls.Add(estado, 0, 1);
            encabezado.Controls.Add(barraAcciones, 1, 0);
            encabezado.SetRowSpan(barraAcciones, 2);
            return encabezado;
        }

        private static Control CrearCampo(string etiqueta, Control control)
        {
            var panel = new Panel
            {
                Width = control.Width + 12,
                Height = 64,
                Margin = new Padding(0, 0, 12, 0),
                Padding = Padding.Empty
            };

            var label = new Label
            {
                Text = etiqueta,
                AutoSize = true,
                Location = new Point(0, 0),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            control.Location = new Point(0, 26);
            if (control is ComboBox or DateTimePicker)
                control.Font = new Font("Segoe UI", 9F);
            panel.Controls.Add(label);
            panel.Controls.Add(control);

            return panel;
        }

        private static Label CrearLeyenda(string texto)
        {
            return new Label
            {
                Text = texto,
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true,
                Padding = new Padding(8, 0, 8, 0),
                Margin = Padding.Empty,
                Font = new Font("Segoe UI", 9F)
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
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                ColumnHeadersDefaultCellStyle =
                {
                    BackColor = Color.FromArgb(245, 246, 248),
                    ForeColor = Color.FromArgb(45, 55, 65),
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                },
                DefaultCellStyle =
                {
                    SelectionBackColor = Color.FromArgb(255, 241, 190),
                    SelectionForeColor = Color.FromArgb(35, 35, 35),
                    Padding = new Padding(6, 0, 6, 0)
                },
                RowHeadersVisible = false,
                Margin = Padding.Empty,
                Font = new Font("Segoe UI", 9F),
                RowTemplate = { Height = 32 }
            };
        }

        private void ConfigurarColumnasHorarios()
        {
            dgvHorarios.Columns.Clear();
            dgvHorarios.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Horario", Name = "Horario", DataPropertyName = "Horario", FillWeight = 35 });
            dgvHorarios.Columns.Add(new DataGridViewTextBoxColumn { Name = "HoraInicio", DataPropertyName = "HoraInicio", Visible = false });
            dgvHorarios.Columns.Add(new DataGridViewTextBoxColumn { Name = "HoraFin", DataPropertyName = "HoraFin", Visible = false });
            dgvHorarios.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Estado", Name = "Estado", DataPropertyName = "Estado", FillWeight = 25 });
        }

        private void ConfigurarColumnasReservas()
        {
            dgvReservas.Columns.Clear();
            dgvReservas.Columns.Add(new DataGridViewTextBoxColumn { Name = "IdInstancia", DataPropertyName = "IdInstancia", Visible = false });
            dgvReservas.Columns.Add(new DataGridViewTextBoxColumn { Name = "FechaValor", DataPropertyName = "FechaValor", Visible = false });
            dgvReservas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Modalidad", DataPropertyName = "Modalidad", Visible = false });
            dgvReservas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Fecha", Name = "Fecha", DataPropertyName = "Fecha", FillWeight = 18 });
            dgvReservas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Cancha", Name = "Cancha", DataPropertyName = "Cancha", FillWeight = 22 });
            dgvReservas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Horario", Name = "Horario", DataPropertyName = "Horario", FillWeight = 25 });
            dgvReservas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Cliente", Name = "Cliente", DataPropertyName = "Cliente", FillWeight = 45 });
            dgvReservas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Estado", Name = "Estado", DataPropertyName = "Estado", FillWeight = 18 });
            dgvReservas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Pago", Name = "Pago", DataPropertyName = "Pago", FillWeight = 30 });
        }
    }
}
