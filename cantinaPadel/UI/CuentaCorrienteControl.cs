using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.UI;

public class CuentaCorrienteControl : UserControl
{
    private readonly LogicaCuentaCorriente _logica;

    private const int MinimoCaracteresBusqueda = 2;

    private readonly TextBox _txtBuscarCliente = new() { Width = 260 };
    // Búsqueda en vivo: espera 300ms sin que se tipee nada antes de consultar la bd (igual que el resto de los buscadores del sistema)
    private readonly System.Windows.Forms.Timer _debounceBusquedaCliente = new() { Interval = 300 };
    // Se activa cuando el texto del buscador se completa por código (CargarCliente), para que no dispare una búsqueda que pise la grilla
    private bool _suprimirBusquedaAutomatica;
    private readonly GrillaClientes _dgvClientes = new();
    private readonly DataGridView _dgvDeuda = new();
    private readonly Label _lblClienteSeleccionado = new() { AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold), Text = "Ningún cliente seleccionado" };
    private readonly Label _lblCredito = new() { AutoSize = true };
    // Desglose Deuda total - Crédito previo = A cobrar ahora. Se muestra solo cuando hay crédito previo, para que quede claro por qué el monto
    // a cobrar es menor a la deuda bruta (y así el cajero no cobre de más)
    private readonly Label _lblDesglose = new() { AutoSize = true, ForeColor = Color.DimGray, Font = new Font("Segoe UI", 9, FontStyle.Italic) };
    private readonly Label _lblDeudaTotal = new() { AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold), Text = "Total a cobrar (estimativo): $0,00" };
    private readonly Label _lblAvisoEstimativo = new()
    {
        AutoSize = true,
        ForeColor = Color.DimGray,
        Font = new Font("Segoe UI", 9, FontStyle.Italic),
        Text = "El monto es estimativo: puede variar si hay aumentos de precios.",
        Visible = false
    };

    private readonly Label _lblEstadoVacio = new()
    {
        AutoSize = false,
        Dock = DockStyle.Fill,
        TextAlign = ContentAlignment.MiddleCenter,
        ForeColor = Color.DimGray,
        Font = new Font("Segoe UI", 10, FontStyle.Italic),
        Text = "Buscá un cliente por DNI, apellido o nombre para ver su cuenta corriente."
    };
    // Monto a cobrar: campo de texto simple (sin flechitas) que agrega solo los puntos de miles. Lo máximo que se puede cobrar es la deuda neta
    private readonly TextBox _txtMonto = new() { Width = 140, TextAlign = HorizontalAlignment.Right, MaxLength = 14, Text = "0" };
    private decimal _montoMaximo;
    private bool _formateandoMonto;

    // Mensaje que ocupa el lugar de la grilla de clientes cuando no hay nada para mostrar (sin buscar todavía, o sin coincidencias)
    private const string MensajeInicialBusqueda = "Escriba el nombre, apellido o DNI del cliente para buscarlo.";
    private readonly Label _lblMensajeBusqueda = new()
    {
        Dock = DockStyle.Fill,
        TextAlign = ContentAlignment.MiddleCenter,
        BackColor = Color.White,
        Font = new Font("Segoe UI", 10.5f, FontStyle.Italic),
        ForeColor = Color.DimGray
    };
    private readonly ComboBox _cmbTipoPago = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 150 };
    // Pago en efectivo: con cuánto entrega el cliente (puede ser más de lo que debe) y el vuelto resultante. Solo visibles con forma de pago Efectivo
    private readonly Label _lblPagaCon = new() { Text = "Paga con", AutoSize = true, Margin = new Padding(12, 7, 6, 0) };
    private readonly TextBox _txtPagaCon = new() { Width = 110, TextAlign = HorizontalAlignment.Right };
    private readonly Label _lblVuelto = new() { AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), Margin = new Padding(12, 6, 6, 0) };
    private readonly Button _btnRegistrarPago = new() { Text = "Registrar pago", AutoSize = true, BackColor = Color.PaleGreen };

    private List<Cliente> _clientesEncontrados = new();
    private Cliente? _clienteSeleccionado;
    // Último resumen cargado: se necesita a mano para armar el preview de confirmación (cuántas unidades se saldan, cuánto queda de crédito)
    // antes de mandar el pago, sin tener que volver a consultar la bd
    private ResumenCuentaCorriente? _resumenActual;

    public CuentaCorrienteControl() : this(new LogicaCuentaCorriente()) { }

    internal CuentaCorrienteControl(LogicaCuentaCorriente logica)
    {
        _logica = logica;
        InicializarComponentes();
    }

    private void InicializarComponentes()
    {
        Dock = DockStyle.Fill;
        BackColor = Color.FromArgb(255, 255, 192);

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(18), ColumnCount = 1, RowCount = 5 };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 35));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 65));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // Búsqueda de cliente
        var busqueda = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
        // Único mecanismo de búsqueda: automática mientras se escribe (Enter la lanza al instante, sin esperar el retardo)
        _debounceBusquedaCliente.Tick += (_, _) =>
        {
            _debounceBusquedaCliente.Stop();
            BuscarClientes();
        };
        _txtBuscarCliente.TextChanged += (_, _) =>
        {
            if (_suprimirBusquedaAutomatica) return;
            _debounceBusquedaCliente.Stop();
            _debounceBusquedaCliente.Start();
        };
        // Desde el buscador se puede navegar los resultados con ↑ ↓ y, con Enter, traer la cuenta corriente del cliente marcado
        _txtBuscarCliente.KeyDown += (_, e) =>
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    e.Handled = e.SuppressKeyPress = true;
                    MoverSeleccionCliente(1);
                    break;
                case Keys.Up:
                    e.Handled = e.SuppressKeyPress = true;
                    MoverSeleccionCliente(-1);
                    break;
                case Keys.Enter:
                    e.Handled = e.SuppressKeyPress = true;
                    // Si Enter llega antes de que pase el retardo de la búsqueda automática, se busca ya mismo para no elegir sobre resultados viejos
                    if (_debounceBusquedaCliente.Enabled)
                    {
                        _debounceBusquedaCliente.Stop();
                        BuscarClientes();
                    }
                    SeleccionarClienteDeGrilla();
                    break;
            }
        };
        Disposed += (_, _) => _debounceBusquedaCliente.Dispose();
        busqueda.Controls.Add(new Label { Text = "Cliente (DNI, apellido o nombre)", AutoSize = true, Margin = new Padding(0, 7, 6, 0) });
        busqueda.Controls.Add(_txtBuscarCliente);
        layout.Controls.Add(busqueda, 0, 0);

        ConfigurarGrillaClientes();
        // Grilla y mensaje comparten lugar: se alterna la visibilidad según haya o no resultados que mostrar
        var contenedorClientes = new Panel { Dock = DockStyle.Fill };
        contenedorClientes.Controls.Add(_dgvClientes);
        contenedorClientes.Controls.Add(_lblMensajeBusqueda);
        layout.Controls.Add(contenedorClientes, 0, 1);

        var encabezadoCuenta = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true, Padding = new Padding(0, 10, 0, 0) };
        encabezadoCuenta.Controls.Add(_lblClienteSeleccionado);
        _lblCredito.Margin = new Padding(24, 4, 0, 0);
        encabezadoCuenta.Controls.Add(_lblCredito);

        var filaDesglose = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true, Padding = new Padding(0, 2, 0, 4) };
        filaDesglose.Controls.Add(_lblDesglose);

        // encabezadoCuenta (nombre del cliente + crédito) y filaDesglose (el detalle Deuda - Crédito = A cobrar)
        var panelEncabezado = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.TopDown, WrapContents = false };
        panelEncabezado.Controls.Add(encabezadoCuenta);
        panelEncabezado.Controls.Add(filaDesglose);
        layout.Controls.Add(panelEncabezado, 0, 2);

        ConfigurarGrillaDeuda();
        // Estado vacío y grilla comparten lugar: se alterna la visibilidad según haya o no cliente elegido
        var contenedorDeuda = new Panel { Dock = DockStyle.Fill };
        contenedorDeuda.Controls.Add(_dgvDeuda);
        contenedorDeuda.Controls.Add(_lblEstadoVacio);
        layout.Controls.Add(contenedorDeuda, 0, 3);

        var panelPago = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true, Padding = new Padding(0, 10, 0, 0) };
        panelPago.Controls.Add(_lblDeudaTotal);
        var espaciador = new Label { Width = 24 };
        panelPago.Controls.Add(espaciador);
        panelPago.Controls.Add(new Label { Text = "Monto a cobrar", AutoSize = true, Margin = new Padding(0, 7, 6, 0) });
        panelPago.Controls.Add(_txtMonto);
        _cmbTipoPago.DisplayMember = "Texto";
        _cmbTipoPago.ValueMember = "Valor";
        // Mismas formas de pago que el Punto de Venta (MetodoPago de LogicaVenta), sin Cuenta Corriente
        _cmbTipoPago.DataSource = LogicaCuentaCorriente.MetodosPagoDeCobro
            .Select(m => new { Texto = NombreMetodoPago(m), Valor = m })
            .ToList();
        panelPago.Controls.Add(new Label { Text = "Forma de pago", AutoSize = true, Margin = new Padding(12, 7, 6, 0) });
        panelPago.Controls.Add(_cmbTipoPago);
        panelPago.Controls.Add(_lblPagaCon);
        panelPago.Controls.Add(_txtPagaCon);
        panelPago.Controls.Add(_lblVuelto);
        _cmbTipoPago.SelectedIndexChanged += (_, _) => ActualizarVuelto();
        ConfigurarCampoMonto();
        _txtPagaCon.TextChanged += (_, _) => ActualizarVuelto();
        _btnRegistrarPago.Click += btnRegistrarPago_Click;
        panelPago.Controls.Add(_btnRegistrarPago);
        panelPago.Controls.Add(_lblAvisoEstimativo);
        panelPago.SetFlowBreak(_btnRegistrarPago, true);
        layout.Controls.Add(panelPago, 0, 4);

        Controls.Add(layout);

        MostrarEstadoVacio();
        MostrarMensajeBusqueda(MensajeInicialBusqueda, esError: false);
        ActualizarVuelto();
    }

    private bool EsPagoEnEfectivo() => _cmbTipoPago.SelectedValue is MetodoPago m && m == MetodoPago.Efectivo;

    // Muestra "Paga con" y el vuelto solo cuando se cobra en efectivo, y recalcula el vuelto contra el monto a cobrar
    private void ActualizarVuelto()
    {
        bool efectivo = EsPagoEnEfectivo();
        _lblPagaCon.Visible = efectivo;
        _txtPagaCon.Visible = efectivo;
        _lblVuelto.Visible = efectivo;
        if (!efectivo) return;

        decimal montoACobrar = ObtenerMonto();
        if (!CalculadorVuelto.TryParsearMonto(_txtPagaCon.Text, out var pagaCon) || pagaCon <= 0m)
        {
            _lblVuelto.ForeColor = Color.DimGray;
            _lblVuelto.Text = "Vuelto: -";
        }
        else if (pagaCon < montoACobrar)
        {
            _lblVuelto.ForeColor = Color.Firebrick;
            _lblVuelto.Text = $"Falta: {montoACobrar - pagaCon:C2}";
        }
        else
        {
            _lblVuelto.ForeColor = Color.ForestGreen;
            _lblVuelto.Text = $"Vuelto: {pagaCon - montoACobrar:C2}";
        }
    }

    // Mismo texto que usa el Punto de Venta para cada forma de pago
    private static string NombreMetodoPago(MetodoPago metodo) => metodo switch
    {
        MetodoPago.Efectivo => "Efectivo",
        MetodoPago.Transferencia => "Transferencia",
        MetodoPago.Tarjeta => "Tarjeta",
        MetodoPago.BilleteraVirtual => "Billetera Virtual",
        MetodoPago.CuentaCorriente => "Cuenta Corriente",
        _ => metodo.ToString()
    };

    private void MostrarEstadoVacio()
    {
        _lblEstadoVacio.Visible = true;
        _dgvDeuda.Visible = false;
        _txtMonto.Enabled = false;
        _cmbTipoPago.Enabled = false;
        _txtPagaCon.Enabled = false;
        _btnRegistrarPago.Enabled = false;
    }

    private void ConfigurarGrillaClientes()
    {
        _dgvClientes.Dock = DockStyle.Fill;
        _dgvClientes.AutoGenerateColumns = false;
        _dgvClientes.ReadOnly = true;
        _dgvClientes.AllowUserToAddRows = false;
        _dgvClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { Name = "IdCliente", DataPropertyName = "IdCliente", Visible = false });
        _dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Dni", HeaderText = "DNI", Width = 100 });
        _dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Apellido", HeaderText = "Apellido", Width = 150 });
        _dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre", HeaderText = "Nombre", Width = 150 });
        _dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Email", HeaderText = "Email", Width = 220 });
        _dgvClientes.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) SeleccionarClienteDeGrilla(); };
        // Enter sobre la grilla (por ejemplo después de navegar con las flechas) trae la cuenta del cliente marcado
        _dgvClientes.EnterPresionado += (_, _) => SeleccionarClienteDeGrilla();
        AplicarEstiloBlanco(_dgvClientes);
        EstirarColumnas(_dgvClientes);
    }

    // Las columnas se estiran para ocupar todo el ancho de la grilla, manteniendo la proporción de los anchos con que se definieron
    // (la columna más ancha, como Email o Producto, recibe proporcionalmente más espacio). Debe llamarse con las columnas ya agregadas
    private static void EstirarColumnas(DataGridView dgv)
    {
        foreach (DataGridViewColumn columna in dgv.Columns)
        {
            columna.FillWeight = columna.Width;
            columna.MinimumWidth = 60;
        }

        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
    }

    // Grilla completamente blanca: fondo (incluido el espacio vacío debajo de las filas y a la derecha de las columnas), celdas, filas
    // alternas y encabezados. La fila seleccionada conserva un celeste muy suave para que se vea cuál está elegida
    private static void AplicarEstiloBlanco(DataGridView dgv)
    {
        dgv.BackgroundColor = Color.White;
        dgv.BorderStyle = BorderStyle.None;
        dgv.GridColor = Color.Gainsboro;
        dgv.RowHeadersVisible = false;

        dgv.DefaultCellStyle.BackColor = Color.White;
        dgv.DefaultCellStyle.ForeColor = Color.Black;
        dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(222, 236, 252);
        dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
        dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

        // Sin esto, Windows ignora los colores del encabezado y los dibuja con el tema del sistema (gris)
        dgv.EnableHeadersVisualStyles = false;
        dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
        dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.White;
        dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.Black;
    }

    private void ConfigurarGrillaDeuda()
    {
        _dgvDeuda.Dock = DockStyle.Fill;
        _dgvDeuda.AutoGenerateColumns = false;
        _dgvDeuda.ReadOnly = true;
        _dgvDeuda.AllowUserToAddRows = false;
        _dgvDeuda.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _dgvDeuda.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FechaVenta", HeaderText = "Fecha", Width = 130, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" } });
        _dgvDeuda.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "IdVenta", HeaderText = "Nº Venta", Width = 80 });
        _dgvDeuda.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NombreProducto", HeaderText = "Producto", Width = 260 });
        _dgvDeuda.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Monto", HeaderText = "Monto adeudado", Width = 130, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
        AplicarEstiloBlanco(_dgvDeuda);
        EstirarColumnas(_dgvDeuda);
    }

    private void BuscarClientes()
    {
        try
        {
            string texto = _txtBuscarCliente.Text.Trim();

            // No se consulta la bd con el campo vacío o con un solo caracter: serían demasiados resultados que no sirven para elegir un cliente.
            // La consulta, además, siempre está limitada a 25 filas (ClienteRepository.Buscar)
            if (texto.Length < MinimoCaracteresBusqueda)
            {
                _clientesEncontrados = new List<Cliente>();
                _dgvClientes.DataSource = null;
                MostrarMensajeBusqueda(MensajeInicialBusqueda, esError: false);
                return;
            }

            _clientesEncontrados = _logica.BuscarClientes(texto);

            // Sin coincidencias: se avisa en el lugar de la grilla (sin ventana emergente)
            if (_clientesEncontrados.Count == 0)
            {
                _dgvClientes.DataSource = null;
                MostrarMensajeBusqueda(
                    $"No se encontró ningún cliente que coincida con “{texto}”.",
                    esError: true);
                return;
            }

            _dgvClientes.DataSource = _clientesEncontrados
                .Select(c => new { c.IdCliente, c.Persona.Dni, c.Persona.Apellido, c.Persona.Nombre, c.Email })
                .ToList();
            MostrarGrillaClientes();

            // Queda marcado el primer resultado, así alcanza con apretar Enter (o moverse con ↑ ↓ y luego Enter)
            MarcarFilaCliente(0);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "No se pudo buscar clientes", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void MostrarMensajeBusqueda(string mensaje, bool esError)
    {
        _lblMensajeBusqueda.Text = mensaje;
        _lblMensajeBusqueda.ForeColor = esError ? Color.Firebrick : Color.DimGray;
        _lblMensajeBusqueda.Visible = true;
        _dgvClientes.Visible = false;
    }

    private void MostrarGrillaClientes()
    {
        _lblMensajeBusqueda.Visible = false;
        _dgvClientes.Visible = true;
    }

    // Marca (fila actual + seleccionada) la fila indicada de la grilla de clientes. La celda actual debe ser de una columna visible
    private void MarcarFilaCliente(int indice)
    {
        if (_dgvClientes.Rows.Count == 0) return;

        indice = Math.Clamp(indice, 0, _dgvClientes.Rows.Count - 1);
        var columna = _dgvClientes.Columns.GetFirstColumn(DataGridViewElementStates.Visible);
        if (columna == null) return;

        _dgvClientes.ClearSelection();
        _dgvClientes.CurrentCell = _dgvClientes.Rows[indice].Cells[columna.Index];
        _dgvClientes.Rows[indice].Selected = true;
    }

    private void MoverSeleccionCliente(int desplazamiento)
    {
        if (!_dgvClientes.Visible || _dgvClientes.Rows.Count == 0) return;

        int actual = _dgvClientes.CurrentRow?.Index ?? -1;
        MarcarFilaCliente(actual + desplazamiento);
    }

    private void SeleccionarClienteDeGrilla()
    {
        if (_dgvClientes.CurrentRow?.Cells["IdCliente"].Value is not int idCliente) return;
        var cliente = _clientesEncontrados.FirstOrDefault(c => c.IdCliente == idCliente);
        if (cliente == null) return;

        _clienteSeleccionado = cliente;
        CargarCuenta();

        // Con la cuenta ya cargada, el cursor pasa al monto para poder cobrar enseguida (si el cliente no tiene deuda queda deshabilitado)
        if (_txtMonto.Enabled)
            _txtMonto.Focus();
    }

    // Si el cajero ya eligió un cliente para la venta, "Ver Cuenta Corriente" llama a este método y evita que el cliente se tenga que buscar dos veces
    public void CargarCliente(Cliente cliente)
    {
        if (cliente == null) return;

        _clienteSeleccionado = cliente;
        _clientesEncontrados = new List<Cliente> { cliente };
        // Se completa el texto sin disparar la búsqueda automática: reemplazaría la grilla con el único cliente por el resultado de buscar ese texto
        _debounceBusquedaCliente.Stop();
        _suprimirBusquedaAutomatica = true;
        try { _txtBuscarCliente.Text = $"{cliente.Persona.Apellido}, {cliente.Persona.Nombre}"; }
        finally { _suprimirBusquedaAutomatica = false; }
        _dgvClientes.DataSource = _clientesEncontrados
            .Select(c => new { c.IdCliente, c.Persona.Dni, c.Persona.Apellido, c.Persona.Nombre, c.Email })
            .ToList();
        MostrarGrillaClientes();

        CargarCuenta();
    }

    private void CargarCuenta()
    {
        if (_clienteSeleccionado == null) return;

        ResumenCuentaCorriente resumen;
        try
        {
            resumen = _logica.ObtenerResumen(_clienteSeleccionado.IdCliente);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "No se pudo cargar la cuenta corriente", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        _resumenActual = resumen;

        _lblEstadoVacio.Visible = false;
        _dgvDeuda.Visible = true;

        _lblClienteSeleccionado.Text = $"{_clienteSeleccionado.Persona.Apellido}, {_clienteSeleccionado.Persona.Nombre} (DNI {_clienteSeleccionado.Persona.Dni})";
        _lblCredito.Text = DescribirCredito(resumen);
        _dgvDeuda.DataSource = resumen.Pendientes.ToList();

        // Visibilidad del estado del sistema: si hay deuda se resalta en rojo, si está saldada se muestra en verde
        bool hayDeuda = resumen.DeudaNeta > 0m;
        _lblDeudaTotal.ForeColor = hayDeuda ? Color.Firebrick : Color.ForestGreen;
        _lblDeudaTotal.Text = hayDeuda ? $"Total a cobrar (estimativo): {resumen.DeudaNeta:C2}" : "Sin deuda pendiente";
        _lblAvisoEstimativo.Visible = hayDeuda;

        // Si hay crédito de un pago parcial anterior, se muestra la cuenta completa (Deuda total - Crédito previo = A cobrar ahora)
        _lblDesglose.Text = hayDeuda && resumen.PagosParcialesAcreditados > 0m
            ? $"Deuda total: {resumen.DeudaBruta:C2}   -   Crédito previo: {resumen.PagosParcialesAcreditados:C2}   =   A cobrar ahora (estimativo): {resumen.DeudaNeta:C2}"
            : string.Empty;

        // No se puede cobrar más de lo que se debe (evita generar un saldo a favor que no corresponde)
        _montoMaximo = hayDeuda ? resumen.DeudaNeta : 0m;
        _txtMonto.Text = "0";
        _txtMonto.Enabled = hayDeuda;
        _txtPagaCon.Clear();
        _txtPagaCon.Enabled = hayDeuda;
        _cmbTipoPago.Enabled = hayDeuda;
        _btnRegistrarPago.Enabled = hayDeuda;
    }

    // ---- Campo "Monto a cobrar" ----

    private void ConfigurarCampoMonto()
    {
        // Solo números: los puntos de los miles se agregan solos y no hace falta escribir decimales
        _txtMonto.KeyPress += (_, e) =>
        {
            if (!(e.KeyChar >= '0' && e.KeyChar <= '9') && !char.IsControl(e.KeyChar))
                e.Handled = true;
        };

        // Al entrar al campo desaparece el "0" para escribir el importe directamente; si ya hay un monto, queda seleccionado para reemplazarlo
        _txtMonto.Enter += (_, _) =>
        {
            if (ObtenerMonto() == 0m)
                _txtMonto.Clear();
            else
                BeginInvoke(new Action(() => _txtMonto.SelectAll()));
        };

        // Si se sale del campo sin escribir nada, vuelve a mostrar 0
        _txtMonto.Leave += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(_txtMonto.Text))
                _txtMonto.Text = "0";
        };

        _txtMonto.TextChanged += (_, _) =>
        {
            if (_formateandoMonto) return;
            FormatearMonto();
            ActualizarVuelto();
        };
    }

    // Monto numérico del campo (0 si está vacío)
    private decimal ObtenerMonto()
    {
        string texto = _txtMonto.Text.Replace(".", string.Empty).Replace(',', '.');
        return decimal.TryParse(texto, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var monto)
            ? monto
            : 0m;
    }

    // Va armando el importe a medida que se escribe: 1500 -> 1.500, 25000 -> 25.000. Nunca deja superar la deuda: si se pasa, se ajusta al máximo
    // (que puede tener centavos si la deuda los tiene; en ese caso se muestra con coma, y se puede borrar con normalidad)
    private void FormatearMonto()
    {
        string texto = _txtMonto.Text;
        int cursor = Math.Min(_txtMonto.SelectionStart, texto.Length);
        int digitosAntesDelCursor = texto.Take(cursor).Count(char.IsDigit);

        int posComa = texto.IndexOf(',');
        string parteEntera = posComa >= 0 ? texto[..posComa] : texto;
        string parteDecimal = posComa >= 0
            ? new string(texto[(posComa + 1)..].Where(char.IsDigit).Take(2).ToArray())
            : string.Empty;

        string enteros = new string(parteEntera.Where(char.IsDigit).ToArray());
        string sinCeros = enteros.TrimStart('0');
        if (sinCeros.Length == 0 && enteros.Length > 0)
            sinCeros = "0";

        decimal monto = decimal.Parse(
            (sinCeros.Length == 0 ? "0" : sinCeros) + (parteDecimal.Length > 0 ? "." + parteDecimal : string.Empty),
            System.Globalization.CultureInfo.InvariantCulture);

        string nuevoTexto;
        bool cursorAlFinal = false;
        if (monto > _montoMaximo)
        {
            nuevoTexto = FormatearImporte(_montoMaximo);
            cursorAlFinal = true;
        }
        else
        {
            nuevoTexto = AgruparMiles(sinCeros) + (posComa >= 0 ? "," + parteDecimal : string.Empty);
        }

        if (nuevoTexto == texto) return;

        _formateandoMonto = true;
        try
        {
            _txtMonto.Text = nuevoTexto;

            // El cursor queda después del mismo dígito en el que estaba, aunque hayan aparecido o desaparecido puntos
            int nuevaPosicion = nuevoTexto.Length;
            if (!cursorAlFinal)
            {
                nuevaPosicion = 0;
                int contados = 0;
                while (nuevaPosicion < nuevoTexto.Length && contados < digitosAntesDelCursor)
                {
                    if (char.IsDigit(nuevoTexto[nuevaPosicion])) contados++;
                    nuevaPosicion++;
                }
            }
            _txtMonto.SelectionStart = nuevaPosicion;
        }
        finally
        {
            _formateandoMonto = false;
        }
    }

    // 1234567 -> "1.234.567"
    private static string AgruparMiles(string digitos)
    {
        var sb = new System.Text.StringBuilder();
        for (int i = digitos.Length - 1, n = 0; i >= 0; i--, n++)
        {
            if (n > 0 && n % 3 == 0) sb.Insert(0, '.');
            sb.Insert(0, digitos[i]);
        }
        return sb.ToString();
    }

    // 1234 -> "1.234"; 1234.5 -> "1.234,50"
    private static string FormatearImporte(decimal importe)
    {
        decimal enteros = Math.Truncate(importe);
        string texto = AgruparMiles(((long)enteros).ToString());
        int centavos = (int)Math.Round((importe - enteros) * 100m);
        return centavos > 0 ? $"{texto},{centavos:00}" : texto;
    }

    // Grilla que avisa cuando se aprieta Enter (por defecto, el DataGridView usa Enter para bajar a la fila siguiente)
    private sealed class GrillaClientes : DataGridView
    {
        public event EventHandler? EnterPresionado;

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                EnterPresionado?.Invoke(this, EventArgs.Empty);
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }
    }

    // "Saldo a favor" solo aparece si el cliente pagó de más. Si debe algo, lo que ya entregó para el próximo producto se muestra como crédito
    // acumulado (que se va a sumar automáticamente al próximo pago)
    private static string DescribirCredito(ResumenCuentaCorriente resumen)
    {
        if (resumen.SaldoAFavor > 0m)
            return $"Saldo a favor: {resumen.SaldoAFavor:C2}";
        if (resumen.PagosParcialesAcreditados > 0m)
            return $"Crédito a favor (se descuenta automáticamente del próximo pago): {resumen.PagosParcialesAcreditados:C2}";
        return string.Empty;
    }

    private void btnRegistrarPago_Click(object? sender, EventArgs e)
    {
        if (_clienteSeleccionado == null)
        {
            MessageBox.Show(this, "Buscá y seleccioná un cliente primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        decimal monto = ObtenerMonto();
        if (monto <= 0)
        {
            MessageBox.Show(this, "Ingresá un monto mayor a cero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Crédito previo que tenía el cliente antes de este pago
        // Se guarda acá porque CargarCuenta() lo va a pisar después de registrar el pago, y se necesita tanto para el preview como para el ticket
        decimal creditoPrevio = _resumenActual?.PagosParcialesAcreditados ?? 0m;
        var pago = new PagoVenta { Metodo = _cmbTipoPago.SelectedValue is MetodoPago metodo ? metodo : MetodoPago.Efectivo };
        string tipoPago = pago.FormaPago;

        // Efectivo: "Paga con" es opcional. El cliente puede entregar más de lo que debe: se cobra solo el monto y se informa el vuelto
        decimal pagaCon = 0m;
        decimal vuelto = 0m;
        if (pago.Metodo == MetodoPago.Efectivo)
        {
            if (!CalculadorVuelto.TryParsearMonto(_txtPagaCon.Text, out pagaCon))
            {
                MessageBox.Show(this, "El monto con el que paga el cliente no es válido.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtPagaCon.Focus();
                _txtPagaCon.SelectAll();
                return;
            }

            var errorPago = CalculadorVuelto.Validar(monto, pagaCon);
            if (errorPago != null)
            {
                MessageBox.Show(this, errorPago, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtPagaCon.Focus();
                _txtPagaCon.SelectAll();
                return;
            }

            vuelto = CalculadorVuelto.Calcular(monto, pagaCon);
        }

        if (!ConfirmarPago(monto, creditoPrevio, tipoPago, pagaCon, vuelto))
            return;

        try
        {
            var resultado = _logica.RegistrarPago(_clienteSeleccionado, monto, Sesion.IdUsuario, pago);

            // Confirmación visible de que el pago quedó registrado
            decimal deudaRestante = CalculadorCuentaCorriente.CalcularDeudaNeta(resultado.DeudaPendiente, resultado.CreditoResultante);
            string mensajePagado = deudaRestante > 0m
                ? $"PAGADO\n\nSe registró el pago de {monto:C2} ({tipoPago}).\nTodavía queda una deuda estimada de {deudaRestante:C2} (estimativo, sujeto a cambios por aumentos de precios)."
                : $"PAGADO\n\nSe registró el pago de {monto:C2} ({tipoPago}).\nLa cuenta quedó saldada.";
            if (vuelto > 0m)
                mensajePagado += $"\n\nPaga con: {pagaCon:C2}\nVUELTO: {vuelto:C2}";
            MessageBox.Show(this, mensajePagado, "Pagado", MessageBoxButtons.OK, MessageBoxIcon.Information);

            MostrarComprobantePago(resultado, _clienteSeleccionado, creditoPrevio, tipoPago, pagaCon, vuelto);

            CargarCuenta();
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(this, ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show(this, ex.Message, "No se pudo registrar el pago", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Ocurrió un error al registrar el pago: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void InitializeComponent()
    {

    }

    // Preview de confirmación: antes de mandar el pago a la bd, se muestra al cajero qué va a pasar con la plata:
    // cuánto se suma al crédito previo, cuántas unidades se terminan saldando, y si queda algo de crédito para la próxima vez
    private bool ConfirmarPago(decimal monto, decimal creditoPrevio, string tipoPago, decimal pagaCon = 0m, decimal vuelto = 0m)
    {
        if (_resumenActual == null) return true;

        decimal creditoTotalAAplicar = creditoPrevio + monto;
        var montosPendientes = _resumenActual.Pendientes.Select(p => p.Monto).ToList();
        var aplicacion = CalculadorCuentaCorriente.AplicarCredito(montosPendientes, creditoTotalAAplicar);

        var texto = new System.Text.StringBuilder();
        texto.AppendLine($"Vas a registrar un pago de {monto:C2}.");
        if (_resumenActual.DeudaNeta > 0m)
            texto.AppendLine($"Deuda estimada: {_resumenActual.DeudaNeta:C2} (estimativo: puede variar si hay aumentos de precios).");
        texto.AppendLine($"Tipo de pago: {tipoPago}.");
        if (pagaCon > 0m)
            texto.AppendLine($"El cliente paga con {pagaCon:C2}. Vuelto a entregar: {vuelto:C2}.");

        if (creditoPrevio > 0m)
        {
            texto.AppendLine();
            texto.AppendLine($"El cliente ya tenía un crédito de {creditoPrevio:C2} de un pago anterior.");
            texto.AppendLine($"Se suma al pago de hoy: {creditoPrevio:C2} + {monto:C2} = {creditoTotalAAplicar:C2} a aplicar contra la deuda.");
        }

        texto.AppendLine();
        texto.AppendLine(aplicacion.CantidadSaldada == 1
            ? "Se va a saldar 1 producto/turno."
            : $"Se van a saldar {aplicacion.CantidadSaldada} productos/turnos.");

        texto.AppendLine();
        texto.AppendLine(aplicacion.CreditoRemanente > 0m
            ? $"Va a quedar un crédito a favor de {aplicacion.CreditoRemanente:C2} para el próximo pago."
            : "No va a quedar crédito pendiente.");

        texto.AppendLine();
        texto.AppendLine("¿Confirmás el pago?");

        var resultado = MessageBox.Show(this, texto.ToString(), "Confirmar pago", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
        return resultado == DialogResult.Yes;
    }

    // Al cobrar cuenta corriente se abre la misma pantalla de comprobante que usa el punto de venta.
    // Cada pago tiene su propia venta, por eso puede emitirse como factura aunque no haya saldado productos completos.
    private void MostrarComprobantePago(ResultadoPagoCuentaCorriente resultado, Cliente cliente, decimal creditoPrevio, string tipoPago, decimal pagaCon = 0m, decimal vuelto = 0m)
    {
        var datos = new DatosVentaParaComprobante
        {
            IdVenta = resultado.IdVentaPago ?? (resultado.ItemsPagados.Count > 0 ? resultado.ItemsPagados[0].IdVenta : 0),
            Total = resultado.MontoRecibido,
            NombreCliente = $"{cliente.Persona.Nombre} {cliente.Persona.Apellido}",
            EmailCliente = cliente.Email,
            MetodoPago = tipoPago,
            CuitCliente = cliente.Persona.Cuit,
            CondicionIvaCliente = cliente.Persona.CondicionIva,
            Items = resultado.ItemsPagados
                .GroupBy(item => item.NombreProducto)
                .Select(g => new DetalleComprobante { Nombre = g.Key, Cantidad = g.Count(), PrecioUnitario = g.Sum(i => i.Monto) / g.Count() })
                .ToList(),
            ItemsRemito = resultado.ItemsPendientes
                .GroupBy(item => item.NombreProducto)
                .Select(g => new DetalleComprobante { Nombre = g.Key, Cantidad = g.Count(), PrecioUnitario = g.Sum(i => i.Monto) / g.Count() })
                .ToList(),
            SaldoFavor = resultado.SaldoAFavor,
            CreditoRestante = resultado.CreditoResultante,
            DeudaEstimada = CalculadorCuentaCorriente.CalcularDeudaNeta(resultado.DeudaPendiente, resultado.CreditoResultante),
            CreditoPrevioAplicado = creditoPrevio,
            PagoCon = pagaCon,
            Vuelto = vuelto
        };

        using var comprobante = new FrmSeleccionComprobante(datos);
        comprobante.ShowDialog(FindForm());
    }
}