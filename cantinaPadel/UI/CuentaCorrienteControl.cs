using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.UI;

public class CuentaCorrienteControl : UserControl
{
    private readonly LogicaCuentaCorriente _logica;

    private readonly TextBox _txtBuscarCliente = new() { Width = 260 };
    private readonly DataGridView _dgvClientes = new();
    private readonly DataGridView _dgvDeuda = new();
    private readonly Label _lblClienteSeleccionado = new() { AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold), Text = "Ningún cliente seleccionado" };
    private readonly Label _lblCredito = new() { AutoSize = true };
    private readonly Label _lblDeudaTotal = new() { AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold), Text = "Total a cobrar: $0,00" };

    private readonly Label _lblEstadoVacio = new()
    {
        AutoSize = false,
        Dock = DockStyle.Fill,
        TextAlign = ContentAlignment.MiddleCenter,
        ForeColor = Color.DimGray,
        Font = new Font("Segoe UI", 10, FontStyle.Italic),
        Text = "Buscá un cliente por DNI, apellido o nombre para ver su cuenta corriente."
    };
    private readonly NumericUpDown _nudMonto = new() { DecimalPlaces = 2, Maximum = 99999999, Minimum = 0, Width = 140 };
    private readonly Button _btnRegistrarPago = new() { Text = "Registrar pago", AutoSize = true, BackColor = Color.PaleGreen };

    private List<Cliente> _clientesEncontrados = new();
    private Cliente? _clienteSeleccionado;

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
        var btnBuscar = new Button { Text = "Buscar cliente", AutoSize = true };
        btnBuscar.Click += (_, _) => BuscarClientes();
        _txtBuscarCliente.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; BuscarClientes(); } };
        busqueda.Controls.Add(new Label { Text = "Cliente (DNI, apellido o nombre)", AutoSize = true, Margin = new Padding(0, 7, 6, 0) });
        busqueda.Controls.Add(_txtBuscarCliente);
        busqueda.Controls.Add(btnBuscar);
        layout.Controls.Add(busqueda, 0, 0);

        ConfigurarGrillaClientes();
        layout.Controls.Add(_dgvClientes, 0, 1);

        var encabezadoCuenta = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true, Padding = new Padding(0, 10, 0, 4) };
        encabezadoCuenta.Controls.Add(_lblClienteSeleccionado);
        _lblCredito.Margin = new Padding(24, 4, 0, 0);
        encabezadoCuenta.Controls.Add(_lblCredito);
        layout.Controls.Add(encabezadoCuenta, 0, 2);

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
        panelPago.Controls.Add(_nudMonto);
        _btnRegistrarPago.Click += btnRegistrarPago_Click;
        panelPago.Controls.Add(_btnRegistrarPago);
        layout.Controls.Add(panelPago, 0, 4);

        Controls.Add(layout);

        MostrarEstadoVacio();
    }

    private void MostrarEstadoVacio()
    {
        _lblEstadoVacio.Visible = true;
        _dgvDeuda.Visible = false;
        _nudMonto.Enabled = false;
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
    }

    private void BuscarClientes()
    {
        try
        {
            _clientesEncontrados = _logica.BuscarClientes(_txtBuscarCliente.Text);
            _dgvClientes.DataSource = _clientesEncontrados
                .Select(c => new { c.IdCliente, c.Persona.Dni, c.Persona.Apellido, c.Persona.Nombre, c.Email })
                .ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "No se pudo buscar clientes", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SeleccionarClienteDeGrilla()
    {
        if (_dgvClientes.CurrentRow?.Cells["IdCliente"].Value is not int idCliente) return;
        var cliente = _clientesEncontrados.FirstOrDefault(c => c.IdCliente == idCliente);
        if (cliente == null) return;

        _clienteSeleccionado = cliente;
        CargarCuenta();
    }

    // Punto de entrada para integrarse con Punto de Venta: si el cajero ya eligió un cliente para la
    // venta, "Ver Cuenta Corriente" llama a este método y evita que el cliente se tenga que buscar dos
    // veces (ver punto 5 de las correcciones: el modal de método de pago no debería obligar a re-elegirlo).
    public void CargarCliente(Cliente cliente)
    {
        if (cliente == null) return;

        _clienteSeleccionado = cliente;
        _clientesEncontrados = new List<Cliente> { cliente };
        _txtBuscarCliente.Text = $"{cliente.Persona.Apellido}, {cliente.Persona.Nombre}";
        _dgvClientes.DataSource = _clientesEncontrados
            .Select(c => new { c.IdCliente, c.Persona.Dni, c.Persona.Apellido, c.Persona.Nombre, c.Email })
            .ToList();

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

        _lblEstadoVacio.Visible = false;
        _dgvDeuda.Visible = true;

        _lblClienteSeleccionado.Text = $"{_clienteSeleccionado.Persona.Apellido}, {_clienteSeleccionado.Persona.Nombre} (DNI {_clienteSeleccionado.Persona.Dni})";
        _lblCredito.Text = DescribirCredito(resumen);
        _dgvDeuda.DataSource = resumen.Pendientes.ToList();

        // Von Restorff / visibilidad del estado del sistema: si hay deuda se resalta en rojo, si está
        // saldada se muestra en verde. El monto por sí solo no siempre se nota a simple vista en una
        // pantalla con mucha información (grilla de deuda arriba, panel de pago abajo).
        bool hayDeuda = resumen.DeudaNeta > 0m;
        _lblDeudaTotal.ForeColor = hayDeuda ? Color.Firebrick : Color.ForestGreen;
        _lblDeudaTotal.Text = hayDeuda ? $"Total a cobrar: {resumen.DeudaNeta:C2}" : "Sin deuda pendiente";

        // No se puede cobrar más de lo que se debe (evita generar un saldo a
        // favor que no corresponde).
        _nudMonto.Value = 0;
        _nudMonto.Maximum = hayDeuda ? resumen.DeudaNeta : 0;
        _nudMonto.Enabled = hayDeuda;
        _btnRegistrarPago.Enabled = hayDeuda;
    }

    // "Saldo a favor" solo aparece si el cliente pagó de más. Si debe algo, lo
    // que ya entregó para el próximo producto se muestra como pago parcial.
    private static string DescribirCredito(ResumenCuentaCorriente resumen)
    {
        if (resumen.SaldoAFavor > 0m)
            return $"Saldo a favor: {resumen.SaldoAFavor:C2}";
        if (resumen.PagosParcialesAcreditados > 0m)
            return $"Ya entregó a cuenta: {resumen.PagosParcialesAcreditados:C2} (descontado del total)";
        return string.Empty;
    }

    private void btnRegistrarPago_Click(object? sender, EventArgs e)
    {
        if (_clienteSeleccionado == null)
        {
            MessageBox.Show(this, "Buscá y seleccioná un cliente primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        decimal monto = _nudMonto.Value;
        if (monto <= 0)
        {
            MessageBox.Show(this, "Ingresá un monto mayor a cero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var resultado = _logica.RegistrarPago(_clienteSeleccionado, monto, Sesion.IdUsuario);
            MostrarResumenPago(resultado);

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

    // Comprobante simple del pago: qué productos quedaron saldados y cuánto
    // queda pendiente.
    // TODO (punto 9 de las correcciones, pendiente de otra iteración): esto todavía no registra un
    // Comprobante real en la base -- solo informa en pantalla. Se resuelve junto con la regla de
    // remito/factura (puntos 6, 7 y 9).
    private void MostrarResumenPago(ResultadoPagoCuentaCorriente resultado)
    {
        var texto = new System.Text.StringBuilder();
        texto.AppendLine($"Pago recibido: {resultado.MontoRecibido:C2}");
        texto.AppendLine();

        if (resultado.ItemsPagados.Count > 0)
        {
            texto.AppendLine($"Productos saldados ({resultado.ItemsPagados.Count}):");
            foreach (var item in resultado.ItemsPagados)
                texto.AppendLine($"  - {item.NombreProducto} ({item.Monto:C2})");
        }
        else
        {
            texto.AppendLine("El pago no alcanzó a cubrir ningún producto completo.");
        }

        texto.AppendLine();
        texto.AppendLine($"Productos que siguen pendientes: {resultado.ItemsPendientes.Count} ({resultado.DeudaPendiente:C2})");

        decimal acreditado = Math.Min(resultado.CreditoResultante, resultado.DeudaPendiente);
        if (acreditado > 0m)
            texto.AppendLine($"Entregado a cuenta del próximo producto: {acreditado:C2} (falta {resultado.DeudaPendiente - acreditado:C2})");
        if (resultado.SaldoAFavor > 0m)
            texto.AppendLine($"Saldo a favor del cliente: {resultado.SaldoAFavor:C2}");

        MessageBox.Show(this, texto.ToString(), "Comprobante de pago - Cuenta Corriente", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}