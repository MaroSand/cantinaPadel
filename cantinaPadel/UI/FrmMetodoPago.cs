using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.UI;

public class FrmMetodoPago : Form
{
    private readonly LogicaVenta _logicaVenta;
    private readonly IReadOnlyCollection<ItemCarrito> _items;
    private readonly decimal _total;
    private readonly TextBox _txtBuscarCliente = new() { Width = 260 };
    private readonly DataGridView _dgvClientes = new();
    private readonly Label _lblCliente = new() { AutoSize = true, Text = "Cliente: Consumidor Final (por defecto)" };
    private readonly TextBox _txtEfectivo = new() { Width = 120, Text = "0" };
    private readonly TextBox _txtTransferencia = new() { Width = 120, Text = "0" };
    private readonly Label _lblRestante = new() { AutoSize = true };
    private Cliente? _clienteSeleccionado;
    private List<Cliente> _clientesEncontrados = new();

    public FrmMetodoPago(IReadOnlyCollection<ItemCarrito> items)
        : this(items, new LogicaVenta()) { }

    internal FrmMetodoPago(IReadOnlyCollection<ItemCarrito> items, LogicaVenta logicaVenta)
    {
        _items = items?.ToList() ?? throw new ArgumentException("Los ítems de venta son obligatorios.");
        _total = Math.Round(_items.Sum(i => i.Subtotal), 2);
        _logicaVenta = logicaVenta;
        InicializarComponentes();
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

        var pagos = new GroupBox { Text = "Importes recibidos", Dock = DockStyle.Fill, Height = 95 };
        var flujoPagos = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(8), WrapContents = true };
        flujoPagos.Controls.Add(CrearCampo("Efectivo", _txtEfectivo));
        flujoPagos.Controls.Add(CrearCampo("Transferencia", _txtTransferencia));
        flujoPagos.Controls.Add(_lblRestante);
        pagos.Controls.Add(flujoPagos);
        _txtEfectivo.TextChanged += (_, _) => ActualizarRestante();
        _txtTransferencia.TextChanged += (_, _) => ActualizarRestante();
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
        ActualizarRestante();
    }

    private static Control CrearCampo(string texto, Control control)
    {
        var panel = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.TopDown, Margin = new Padding(0, 0, 22, 0) };
        panel.Controls.Add(new Label { Text = texto, AutoSize = true });
        panel.Controls.Add(control);
        return panel;
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
            _dgvClientes.DataSource = _clientesEncontrados
                .Select(c => new { c.IdCliente, Nombre = $"{c.Persona.Nombre} {c.Persona.Apellido}", c.Persona.Dni, c.Email })
                .ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "No se pudo buscar clientes", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SeleccionarCliente()
    {
        if (_dgvClientes.CurrentRow?.Cells["IdCliente"].Value is not int idCliente) return;
        var cliente = _clientesEncontrados.FirstOrDefault(c => c.IdCliente == idCliente);
        if (cliente == null) return;
        _clienteSeleccionado = cliente;
        _lblCliente.Text = $"Cliente seleccionado: {cliente.Persona.Nombre} {cliente.Persona.Apellido}";
    }

    private void ActualizarRestante()
    {
        decimal efectivo = LeerImporte(_txtEfectivo.Text);
        decimal transferencia = LeerImporte(_txtTransferencia.Text);
        _lblRestante.Text = $"Diferencia: {_total - efectivo - transferencia:C2}";
    }

    private static decimal LeerImporte(string texto) => decimal.TryParse(texto, out var importe) ? importe : -1;

    private void btnConfirmar_Click(object? sender, EventArgs e)
    {
        try
        {
            var pago = new PagoVenta { Efectivo = LeerImporte(_txtEfectivo.Text), Transferencia = LeerImporte(_txtTransferencia.Text) };
            var venta = _logicaVenta.ConfirmarVenta(_items, _clienteSeleccionado, pago, Sesion.IdUsuario);
            var cliente = _clienteSeleccionado ?? _logicaVenta.ObtenerConsumidorFinal();
            var datos = new DatosVentaParaComprobante
            {
                IdVenta = venta.IdVenta,
                Total = venta.Total,
                NombreCliente = $"{cliente.Persona.Nombre} {cliente.Persona.Apellido}",
                EmailCliente = cliente.Email,
                MetodoPago = pago.FormaPago,
                Items = _items.Select(i => new DetalleComprobante { Nombre = i.Producto.Nombre, Cantidad = i.Cantidad, PrecioUnitario = i.PrecioUnitario }).ToList()
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
