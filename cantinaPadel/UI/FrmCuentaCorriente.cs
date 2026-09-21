using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.UI;

// US-16: pantalla de Cuenta Corriente.
// - Busca clientes por DNI, apellido o nombre.
// - Muestra el detalle de los productos que el cliente tiene pendientes de
//   pago (una fila por unidad, ya actualizada si el precio del producto
//   cambió mientras estaba pendiente).
// - Permite registrar un pago (total o parcial): el sistema calcula cuántos
//   productos completos alcanza a cubrir y los marca como pagados; lo que
//   sobra queda como saldo a favor del cliente.
public class FrmCuentaCorriente : Form
{
    private readonly LogicaCuentaCorriente _logica;

    private readonly TextBox _txtBuscarCliente = new() { Width = 260 };
    private readonly DataGridView _dgvClientes = new();
    private readonly DataGridView _dgvDeuda = new();
    private readonly Label _lblClienteSeleccionado = new() { AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold), Text = "Ningún cliente seleccionado" };
    private readonly Label _lblSaldoFavor = new() { AutoSize = true, Text = "Saldo a favor: $0,00" };
    private readonly Label _lblDeudaTotal = new() { AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold), Text = "Total adeudado: $0,00" };
    private readonly NumericUpDown _nudMonto = new() { DecimalPlaces = 2, Maximum = 99999999, Minimum = 0, Width = 140 };
    private readonly Button _btnRegistrarPago = new() { Text = "Registrar pago", AutoSize = true, BackColor = Color.PaleGreen };

    private List<Cliente> _clientesEncontrados = new();
    private Cliente? _clienteSeleccionado;
    private List<ItemDeudaCliente> _pendientes = new();

    public FrmCuentaCorriente() : this(new LogicaCuentaCorriente()) { }

    internal FrmCuentaCorriente(LogicaCuentaCorriente logica)
    {
        _logica = logica;
        InicializarComponentes();
    }

    private void InicializarComponentes()
    {
        Text = "Cuenta Corriente";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(820, 620);

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
        _lblSaldoFavor.Margin = new Padding(24, 4, 0, 0);
        encabezadoCuenta.Controls.Add(_lblSaldoFavor);
        layout.Controls.Add(encabezadoCuenta, 0, 2);

        ConfigurarGrillaDeuda();
        layout.Controls.Add(_dgvDeuda, 0, 3);

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

    private void CargarCuenta()
    {
        if (_clienteSeleccionado == null) return;

        try
        {
            _pendientes = _logica.ObtenerPendientes(_clienteSeleccionado.IdCliente);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "No se pudo cargar la cuenta corriente", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        _lblClienteSeleccionado.Text = $"{_clienteSeleccionado.Persona.Apellido}, {_clienteSeleccionado.Persona.Nombre} (DNI {_clienteSeleccionado.Persona.Dni})";
        _lblSaldoFavor.Text = $"Saldo a favor: {Math.Max(_clienteSeleccionado.SaldoCuentaCorriente, 0m):C2}";
        _dgvDeuda.DataSource = _pendientes.ToList();
        decimal total = _pendientes.Sum(p => p.Monto);
        _lblDeudaTotal.Text = $"Total adeudado: {total:C2}";
        _nudMonto.Maximum = Math.Max(total, 1);
        _nudMonto.Value = 0;
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

            // El pago ya devuelve el saldo a favor resultante: no hace falta
            // volver a consultar la base para refrescar la pantalla.
            _clienteSeleccionado.SaldoCuentaCorriente = resultado.SaldoFavorResultante;
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

    // Comprobante simple del pago: qué productos quedaron saldados y cuánto
    // saldo a favor le queda al cliente (lo pedido: "mostrar el saldo a favor
    // en el ticket/comprobante").
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
        texto.AppendLine($"Saldo a favor del cliente: {resultado.SaldoFavorResultante:C2}");

        MessageBox.Show(this, texto.ToString(), "Comprobante de pago - Cuenta Corriente", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
