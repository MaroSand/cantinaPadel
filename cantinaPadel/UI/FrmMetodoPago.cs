using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.UI;

public class FrmMetodoPago : Form
{
    private readonly LogicaVenta _logicaVenta;
    private readonly LogicaCuentaCorriente _logicaCuentaCorriente;
    private readonly IReadOnlyCollection<ItemCarrito> _items;
    private readonly decimal _total;
    private readonly TextBox _txtBuscarCliente = new() { Width = 260 };
    private readonly DataGridView _dgvClientes = new();
    private readonly Label _lblCliente = new() { AutoSize = true, Text = "Cliente: Consumidor Final (por defecto)" };
    private readonly CheckBox _chkEfectivo = new() { Text = "Efectivo", AutoSize = true, Checked = true };
    private readonly CheckBox _chkTransferencia = new() { Text = "Transferencia", AutoSize = true };
    private readonly CheckBox _chkTarjeta = new() { Text = "Tarjeta", AutoSize = true };
    private readonly CheckBox _chkBilleteraVirtual = new() { Text = "Billetera Virtual", AutoSize = true };
    private readonly CheckBox _chkCuentaCorriente = new() { Text = "Cuenta Corriente", AutoSize = true };
    private List<CheckBox> _checksMetodoPago = new();
    private bool _actualizandoChecks;
    private Cliente? _clienteSeleccionado;
    private List<Cliente> _clientesEncontrados = new();

    // Constructor principal: recibe los ítems de venta y opcionalmente un cliente preseleccionado.
    public FrmMetodoPago(IReadOnlyCollection<ItemCarrito> items, Cliente? clientePreseleccionado = null)
        : this(items, new LogicaVenta(), clientePreseleccionado) { }

    internal FrmMetodoPago(IReadOnlyCollection<ItemCarrito> items, LogicaVenta logicaVenta, Cliente? clientePreseleccionado = null, LogicaCuentaCorriente? logicaCuentaCorriente = null)
    {
        _logicaCuentaCorriente = logicaCuentaCorriente ?? new LogicaCuentaCorriente();
        _items = items?.ToList() ?? throw new ArgumentException("Los ítems de venta son obligatorios.");
        _total = Math.Round(_items.Sum(i => i.Subtotal), 2);
        _logicaVenta = logicaVenta;
        InicializarComponentes();

        if (clientePreseleccionado != null)
            PrecargarCliente(clientePreseleccionado);
    }

    private void InicializarComponentes()
    {
        Text = "Método de pago";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(650, 520);

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(18), ColumnCount = 1, RowCount = 5 };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var lblTotal = new Label { AutoSize = true, Font = new Font("Segoe UI", 14, FontStyle.Bold), Text = $"Total a cobrar: {_total:C2}", Margin = new Padding(0, 0, 0, 12) };
        layout.Controls.Add(lblTotal, 0, 0);

        var busqueda = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
        var btnBuscar = new Button { Text = "Buscar cliente", AutoSize = true };
        var btnSeleccionar = new Button { Text = "Seleccionar cliente", AutoSize = true };
        btnBuscar.Click += (_, _) => BuscarClientes();
        btnSeleccionar.Click += (_, _) => SeleccionarCliente();
        _txtBuscarCliente.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; BuscarClientes(); } };
        busqueda.Controls.Add(new Label { Text = "Cliente", AutoSize = true, Margin = new Padding(0, 7, 6, 0) });
        busqueda.Controls.Add(_txtBuscarCliente);
        busqueda.Controls.Add(btnBuscar);
        busqueda.Controls.Add(btnSeleccionar);
        busqueda.Controls.Add(_lblCliente);
        layout.Controls.Add(busqueda, 0, 1);

        ConfigurarGrillaClientes();
        layout.Controls.Add(_dgvClientes, 0, 2);

        var pagos = new GroupBox { Text = "Método de pago", Dock = DockStyle.Fill, Height = 95 };
        var flujoPagos = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(8), WrapContents = true };
        _checksMetodoPago = new List<CheckBox> { _chkEfectivo, _chkTransferencia, _chkTarjeta, _chkBilleteraVirtual, _chkCuentaCorriente };
        foreach (var chk in _checksMetodoPago)
        {
            chk.CheckedChanged += (sender, _) => SeleccionarMetodoUnico((CheckBox)sender!);
            flujoPagos.Controls.Add(chk);
        }

        // Si el usuario tilda Cuenta Corriente, y no hay cliente seleccionado, se enfoca el textbox de búsqueda.
        _chkCuentaCorriente.CheckedChanged += (_, _) =>
        {
            if (_chkCuentaCorriente.Checked && _clienteSeleccionado == null)
                _txtBuscarCliente.Focus();
        };
        pagos.Controls.Add(flujoPagos);
        layout.Controls.Add(pagos, 0, 3);

        var acciones = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.RightToLeft };
        var btnCancelar = new Button { Text = "Cancelar", AutoSize = true, DialogResult = DialogResult.Cancel };
        var btnConfirmar = new Button { Text = "Confirmar venta", AutoSize = true, BackColor = Color.PaleGreen };
        btnConfirmar.Click += btnConfirmar_Click;
        acciones.Controls.Add(btnConfirmar);
        acciones.Controls.Add(btnCancelar);
        layout.Controls.Add(acciones, 0, 4);
        Controls.Add(layout);
        AcceptButton = btnConfirmar;
        CancelButton = btnCancelar;
    }

    // Los checks se comportan como selección única (tipo radio buttons).
    private void SeleccionarMetodoUnico(CheckBox seleccionado)
    {
        if (_actualizandoChecks) return; 
        _actualizandoChecks = true;
        try
        {
            if (!seleccionado.Checked)
            {
                seleccionado.Checked = true;
                return;
            }

            foreach (var chk in _checksMetodoPago.Where(chk => chk != seleccionado))
                chk.Checked = false;
        }
        finally
        {
            _actualizandoChecks = false;
        }
    }

    private MetodoPago ObtenerMetodoSeleccionado()
    {
        if (_chkTransferencia.Checked) return MetodoPago.Transferencia;
        if (_chkTarjeta.Checked) return MetodoPago.Tarjeta;
        if (_chkBilleteraVirtual.Checked) return MetodoPago.BilleteraVirtual;
        if (_chkCuentaCorriente.Checked) return MetodoPago.CuentaCorriente;
        return MetodoPago.Efectivo;
    }

    private void ConfigurarGrillaClientes()
    {
        _dgvClientes.Dock = DockStyle.Fill;
        _dgvClientes.AutoGenerateColumns = false;
        _dgvClientes.ReadOnly = true;
        _dgvClientes.AllowUserToAddRows = false;
        _dgvClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { Name = "IdCliente", DataPropertyName = "IdCliente", Visible = false });
        _dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre", HeaderText = "Nombre", Width = 180 });
        _dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Dni", HeaderText = "DNI", Width = 100 });
        _dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Email", HeaderText = "Email", Width = 220 });
        _dgvClientes.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) SeleccionarCliente(); };
    }

    private void BuscarClientes()
    {
        try
        {
            _clientesEncontrados = _logicaVenta.BuscarClientes(_txtBuscarCliente.Text);
            MostrarClientesEnGrilla();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "No se pudo buscar clientes", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void MostrarClientesEnGrilla()
    {
        _dgvClientes.DataSource = _clientesEncontrados
            .Select(c => new { c.IdCliente, Nombre = $"{c.Persona.Nombre} {c.Persona.Apellido}", c.Persona.Dni, c.Email })
            .ToList();
    }

    // Deja el cliente elegido en el punto de venta ya seleccionado y visible, sin que haya que buscarlo de nuevo
    private void PrecargarCliente(Cliente cliente)
    {
        _clienteSeleccionado = cliente;
        _clientesEncontrados = new List<Cliente> { cliente };
        MostrarClientesEnGrilla();
        _lblCliente.Text = $"Cliente seleccionado: {cliente.Persona.Nombre} {cliente.Persona.Apellido}";
    }

    private void SeleccionarCliente()
    {
        if (_dgvClientes.CurrentRow?.Cells["IdCliente"].Value is not int idCliente) return;
        var cliente = _clientesEncontrados.FirstOrDefault(c => c.IdCliente == idCliente);
        if (cliente == null) return;
        _clienteSeleccionado = cliente;
        _lblCliente.Text = $"Cliente seleccionado: {cliente.Persona.Nombre} {cliente.Persona.Apellido}";
    }


    // Obtiene el saldo a favor del cliente seleccionado, si es que tiene uno. Si ocurre un error, devuelve 0.
    private decimal ObtenerSaldoAFavor(Cliente cliente)
    {
        try
        {
            return _logicaCuentaCorriente.ObtenerResumen(cliente.IdCliente).SaldoAFavor;
        }
        catch (Exception)
        {
            return 0m;
        }
    }

    private void btnConfirmar_Click(object? sender, EventArgs e)
    {
        try
        {
            var metodo = ObtenerMetodoSeleccionado();
            if (metodo == MetodoPago.CuentaCorriente && _clienteSeleccionado == null)
            {
                MessageBox.Show(this, "Cuenta Corriente requiere seleccionar un cliente.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var pago = new PagoVenta { Metodo = metodo };
            var venta = _logicaVenta.ConfirmarVenta(_items, _clienteSeleccionado, pago, Sesion.IdUsuario);
            var cliente = _clienteSeleccionado ?? _logicaVenta.ObtenerConsumidorFinal();
            var datos = new DatosVentaParaComprobante
            {
                IdVenta = venta.IdVenta,
                Total = venta.Total,
                NombreCliente = $"{cliente.Persona.Nombre} {cliente.Persona.Apellido}",
                EmailCliente = cliente.Email,
                MetodoPago = pago.FormaPago,
                CuitCliente = cliente.Persona.Cuit,
                CondicionIvaCliente = cliente.Persona.CondicionIva,
                Items = _items.Select(i => new DetalleComprobante { Nombre = i.Producto.Nombre, Cantidad = i.Cantidad, PrecioUnitario = i.PrecioUnitario }).ToList(),
                SaldoFavor = ObtenerSaldoAFavor(cliente)
            };

            using var comprobante = new FrmSeleccionComprobante(datos);
            if (comprobante.ShowDialog(this) == DialogResult.OK && comprobante.ComprobanteGenerado != null)
                _logicaVenta.ActualizarTipoComprobante(venta.IdVenta, comprobante.ComprobanteGenerado.Tipo);

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(this, ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show(this, ex.Message, "No se pudo confirmar la venta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Ocurrió un error al registrar la venta: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}