using cantinaPadel.BLL;
using cantinaPadel.Models;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace cantinaPadel.UI;

internal static class EstiloCaja
{
    public static void Preparar(Form f, string titulo)
    {
        f.Text = titulo; f.StartPosition = FormStartPosition.CenterParent; f.Font = new Font("Segoe UI", 9F);
        f.BackColor = SystemColors.Info; f.FormBorderStyle = FormBorderStyle.FixedDialog; f.MaximizeBox = false; f.MinimizeBox = false;
    }
    public static Button Boton(string texto, Color color) => new() { Text = texto, AutoSize = true, Padding = new Padding(8, 4, 8, 4), BackColor = color, UseVisualStyleBackColor = false, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Margin = new Padding(6) };
    public static Label Etiqueta(string texto, bool titulo = false) => new() { Text = texto, AutoSize = true, Margin = new Padding(8), Font = new Font("Segoe UI", titulo ? 13F : 9F, titulo ? FontStyle.Bold : FontStyle.Regular) };

    // Mismo estilo que los botones de Punto de Venta: Segoe UI 9 negrita, tamaño fijo, color sólido y texto contrastado.
    public static Button BotonPuntoVenta(string texto, Color color) => new()
    {
        Text = texto,
        Size = new Size(200, 42),
        BackColor = color,
        ForeColor = Color.White,
        UseVisualStyleBackColor = false,
        Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0),
        Margin = new Padding(8)
    };

    // Apila los controles en una sola columna, centrados horizontalmente y en bloque verticalmente dentro del formulario.
    public static TableLayoutPanel Centrar(params Control[] controles)
    {
        var tabla = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = controles.Length + 2, Padding = new Padding(20) };
        tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        for (int i = 0; i < controles.Length; i++)
        {
            var c = controles[i];
            if (c is Label l) { l.TextAlign = ContentAlignment.MiddleCenter; l.MaximumSize = new Size(360, 0); }
            c.Anchor = AnchorStyles.None;
            tabla.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tabla.Controls.Add(c, 0, i + 1);
        }
        tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        return tabla;
    }
}

// Campo de monto en pesos: solo dígitos (sin signo, punto ni coma), sin pegar, sin flechas y con largo máximo.
// Mientras se escribe se muestra formateado ("$ 1.234.567"); el punto y el "$" los pone el control, no el usuario.
internal sealed class TextBoxMonto : TextBox
{
    private const int WM_PASTE = 0x0302;
    public const int MaxDigitos = 9;
    private static readonly NumberFormatInfo FormatoPesos = new() { NumberGroupSeparator = ".", NumberDecimalDigits = 0 };
    private bool _formateando;

    public TextBoxMonto()
    {
        MaxLength = 13;                              // "$ 999.999.999"
        Width = 220;
        TextAlign = HorizontalAlignment.Center;
        PlaceholderText = "$ 0";
        ShortcutsEnabled = false;                    // bloquea Ctrl+V, Shift+Insert, etc.
        ContextMenuStrip = new ContextMenuStrip();   // menú contextual vacío: no hay "Pegar"
        AllowDrop = false;
        ImeMode = ImeMode.Disable;
    }

    private static string Digitos(string texto) => new(texto.Where(char.IsAsciiDigit).ToArray());

    protected override void OnKeyPress(KeyPressEventArgs e)
    {
        if (!char.IsControl(e.KeyChar))
        {
            if (!char.IsAsciiDigit(e.KeyChar)) e.Handled = true;
            else if (Digitos(Text).Length - Digitos(SelectedText).Length >= MaxDigitos) e.Handled = true;
        }
        base.OnKeyPress(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        // El cursor siempre queda al final para que el formato no se desordene.
        if (e.KeyCode is Keys.Left or Keys.Up or Keys.Home or Keys.PageUp or Keys.Delete)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
        base.OnKeyDown(e);
    }

    protected override void OnMouseUp(MouseEventArgs mevent)
    {
        base.OnMouseUp(mevent);
        SelectionStart = Text.Length;
        SelectionLength = 0;
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_PASTE) return;               // ignora cualquier pegado que llegue por mensaje de Windows
        base.WndProc(ref m);
    }

    protected override void OnTextChanged(EventArgs e)
    {
        if (_formateando) return;

        var digitos = Digitos(Text);                 // también limpia texto soltado con drag & drop
        if (digitos.Length > MaxDigitos) digitos = digitos[..MaxDigitos];
        var formateado = digitos.Length == 0
            ? ""
            : "$ " + long.Parse(digitos, CultureInfo.InvariantCulture).ToString("N0", FormatoPesos);

        if (formateado != Text)
        {
            _formateando = true;
            try { Text = formateado; SelectionStart = Text.Length; }
            finally { _formateando = false; }
        }
        base.OnTextChanged(e);
    }

    public bool TryObtenerMonto(out decimal monto)
    {
        monto = 0;
        var digitos = Digitos(Text);
        return digitos.Length > 0 && decimal.TryParse(digitos, NumberStyles.None, CultureInfo.InvariantCulture, out monto) && monto > 0;
    }
}

public class FrmGestionCaja : Form
{
    private readonly LogicaCaja _logica = new();
    private readonly DataGridView _historial = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = false, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, BackgroundColor = Color.White };
    private readonly DataGridView _historialEfectivo = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = false, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, BackgroundColor = Color.White };
    private readonly Button _abrir = EstiloCaja.Boton("Abrir caja", Color.ForestGreen);
    private readonly Button _cerrar = EstiloCaja.Boton("Cerrar caja", Color.Gold);
    private readonly Button _retiro = EstiloCaja.Boton("Retirar efectivo", Color.IndianRed);
    private readonly Button _agregarEfectivo = EstiloCaja.Boton("Agregar efectivo", Color.ForestGreen);
    private readonly Label _estado = EstiloCaja.Etiqueta("");

    public FrmGestionCaja()
    {
        EstiloCaja.Preparar(this, "Gestión de Caja"); Size = new Size(880, 570);
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(18), RowCount = 3, ColumnCount = 1 };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize)); root.RowStyles.Add(new RowStyle(SizeType.AutoSize)); root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.Controls.Add(EstiloCaja.Etiqueta("Caja", true));
        var bar = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
        bar.Controls.AddRange(new Control[] { _abrir, _cerrar, _retiro, _agregarEfectivo, _estado }); root.Controls.Add(bar);
        _historial.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TurnoCaja.IdCaja), HeaderText = "Caja empleado", Width = 85 });
        _historial.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TurnoCaja.NombreEmpleado), HeaderText = "Empleado", Width = 150 });
        _historial.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TurnoCaja.FechaApertura), HeaderText = "Apertura", Width = 150, DefaultCellStyle = new() { Format = "g" } });
        _historial.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TurnoCaja.MontoApertura), HeaderText = "Efectivo inicial", Width = 130, DefaultCellStyle = new() { Format = "C2" } });
        _historial.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TurnoCaja.FechaCierre), HeaderText = "Cierre", Width = 150, DefaultCellStyle = new() { Format = "g" } });
        _historial.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TurnoCaja.CierreEfectivo), HeaderText = "Ventas efectivo", Width = 115, DefaultCellStyle = new() { Format = "C2" } });
        _historial.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TurnoCaja.EfectivoFinal), HeaderText = "Efectivo contado", Width = 125, DefaultCellStyle = new() { Format = "C2" } });
        _historial.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TurnoCaja.EfectivoEsperado), HeaderText = "Efectivo esperado", Width = 125, DefaultCellStyle = new() { Format = "C2" } });
        _historial.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TurnoCaja.DiferenciaEfectivo), HeaderText = "Diferencia", Width = 110, DefaultCellStyle = new() { Format = "C2" } });
        _historial.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TurnoCaja.MotivoDiferencia), HeaderText = "Motivo diferencia", Width = 180 });
        _historial.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TurnoCaja.CierreTarjeta), HeaderText = "Tarjeta", Width = 115, DefaultCellStyle = new() { Format = "C2" } });
        _historial.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TurnoCaja.CierreTransferencia), HeaderText = "Transferencia / MP", Width = 145, DefaultCellStyle = new() { Format = "C2" } });
        _historial.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TurnoCaja.Estado), HeaderText = "Estado", Width = 100 });
        _historialEfectivo.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(MovimientoEfectivoHistorial.Fecha), HeaderText = "Fecha y hora", Width = 180, DefaultCellStyle = new() { Format = "g" } });
        _historialEfectivo.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(MovimientoEfectivoHistorial.Tipo), HeaderText = "Movimiento", Width = 120 });
        _historialEfectivo.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(MovimientoEfectivoHistorial.Monto), HeaderText = "Monto", Width = 130, DefaultCellStyle = new() { Format = "C2" } });
        _historialEfectivo.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(MovimientoEfectivoHistorial.Usuario), HeaderText = "Registrado por", Width = 180 });
        var pestañas = new TabControl { Dock = DockStyle.Fill };
        var tabCajas = new TabPage("Turnos de caja"); tabCajas.Controls.Add(_historial);
        var tabMovimientos = new TabPage("Ingresos y retiros"); tabMovimientos.Controls.Add(_historialEfectivo);
        pestañas.TabPages.Add(tabCajas); pestañas.TabPages.Add(tabMovimientos);
        root.Controls.Add(pestañas); Controls.Add(root);
        _abrir.Click += (_, _) => { using var f = new FrmAperturaCaja(_logica); if (f.ShowDialog(this) == DialogResult.OK) Actualizar(); };
        _cerrar.Click += (_, _) => { var caja = Sesion.Rol == "Admin" ? _logica.ObtenerCajaAbiertaGeneral() : _logica.ObtenerCajaAbierta(Sesion.IdUsuario); if (caja == null) { MessageBox.Show(this, Sesion.Rol == "Admin" ? "No hay una caja abierta." : "No tenés una caja abierta.", "Caja", MessageBoxButtons.OK, MessageBoxIcon.Information); return; } using var f = new FrmCierreCaja(_logica, caja); if (f.ShowDialog(this) == DialogResult.OK) Actualizar(); };
        _retiro.Click += (_, _) => { var caja = _logica.ObtenerCajaAbiertaGeneral(); if (caja == null) { MessageBox.Show(this, "No hay una caja abierta.", "Caja", MessageBoxButtons.OK, MessageBoxIcon.Information); return; } using var f = new FrmRetiroEfectivo(_logica, caja); f.ShowDialog(this); Actualizar(); };
        _agregarEfectivo.Click += (_, _) => { var caja = _logica.ObtenerCajaAbiertaGeneral(); if (caja == null) { MessageBox.Show(this, "No hay una caja abierta.", "Caja", MessageBoxButtons.OK, MessageBoxIcon.Information); return; } using var f = new FrmIngresoEfectivo(_logica, caja); f.ShowDialog(this); Actualizar(); };
        Load += (_, _) => Actualizar();
    }

    private void Actualizar()
    {
        try
        {
            var abierta = _logica.ObtenerCajaAbiertaGeneral();
            var propia = abierta?.IdEmpleado == Sesion.IdUsuario;
            _estado.Text = abierta == null ? "Caja física cerrada" : $"Caja abierta por {abierta.NombreEmpleado} · Efectivo en caja: {_logica.ObtenerEfectivoDisponible(abierta.IdTurnoCaja):C2}";
            _abrir.Enabled = abierta == null;
            // El empleado cierra solo su caja; el Admin puede cerrar la de cualquiera (por ejemplo, si quedó abierta por un corte de luz)
            _cerrar.Enabled = abierta != null && (propia || Sesion.Rol == "Admin");
            _retiro.Visible = Sesion.Rol == "Admin";
            _retiro.Enabled = abierta != null;
            _agregarEfectivo.Enabled = abierta != null;
            _historial.DataSource = Sesion.Rol == "Admin" ? _logica.ObtenerTodoHistorial(Sesion.Rol) : _logica.ObtenerHistorial(Sesion.IdUsuario);
            _historialEfectivo.DataSource = _logica.ObtenerHistorialEfectivo(Sesion.IdUsuario, Sesion.Rol);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Error al cargar los datos de caja: {ex.Message}", "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

public class FrmAperturaCaja : Form
{
    private readonly LogicaCaja _logica;
    public FrmAperturaCaja(LogicaCaja? logica = null)
    {
        _logica = logica ?? new LogicaCaja(); EstiloCaja.Preparar(this, "Apertura de Caja"); Size = new Size(470, 250);
        var root = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20), FlowDirection = FlowDirection.TopDown, WrapContents = false };
        var heredado = _logica.ObtenerMontoSiguienteApertura();
        root.Controls.Add(EstiloCaja.Etiqueta($"Efectivo apertura: {heredado:C2}", true));
        var guardar = EstiloCaja.Boton("Abrir caja", Color.ForestGreen); root.Controls.Add(guardar); Controls.Add(root);
        guardar.Click += (_, _) => { try { _logica.AbrirCaja(Sesion.IdUsuario); DialogResult = DialogResult.OK; Close(); } catch (Exception ex) { MessageBox.Show(this, ex.Message, "No se pudo abrir la caja", MessageBoxButtons.OK, MessageBoxIcon.Warning); } };
    }
}

public class FrmIngresoEfectivo : Form
{
    private readonly LogicaCaja _logica;
    private readonly TurnoCaja _caja;
    private readonly TextBoxMonto _monto = new();
    private readonly TextBox _contrasenaAdmin = new() { Width = 220, MaxLength = 8, UseSystemPasswordChar = true, TextAlign = HorizontalAlignment.Center };

    public FrmIngresoEfectivo(LogicaCaja logica, TurnoCaja caja)
    {
        _logica = logica; _caja = caja; EstiloCaja.Preparar(this, "Agregar efectivo"); Size = new Size(470, 360);
        var agregar = EstiloCaja.BotonPuntoVenta("Registrar ingreso", Color.Green);
        Controls.Add(EstiloCaja.Centrar(
            EstiloCaja.Etiqueta("El administrador registra el efectivo que entrega para el cambio."),
            EstiloCaja.Etiqueta("Monto a ingresar"), _monto,
            EstiloCaja.Etiqueta("Contraseña del administrador"), _contrasenaAdmin,
            agregar));
        agregar.Click += (_, _) =>
        {
            if (!_monto.TryObtenerMonto(out var monto))
            {
                MessageBox.Show(this, "Ingresá un monto mayor a cero.", "Monto inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _monto.Focus();
                return;
            }
            try
            {
                _logica.AgregarEfectivo(_caja.IdTurnoCaja, monto, _contrasenaAdmin.Text);
                MessageBox.Show(this, "Ingreso de efectivo registrado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK; Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "No se pudo registrar el ingreso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        };
    }
}

public class FrmCierreCaja : Form
{
    private readonly LogicaCaja _logica; private readonly TurnoCaja _caja;
    private readonly NumericUpDown _efectivoContado = new() { Minimum = 0m, Maximum = 999999999m, DecimalPlaces = 2, ThousandsSeparator = true, Width = 180 };
    private readonly TextBox _motivoDiferencia = new() { Width = 280, MaxLength = 250 };
    private readonly decimal _efectivoEsperado;
    public FrmCierreCaja(LogicaCaja logica, TurnoCaja caja)
    {
        _logica = logica; _caja = caja; EstiloCaja.Preparar(this, "Cierre de Caja"); Size = new Size(760, 500);
        _efectivoEsperado = _logica.ObtenerEfectivoDisponible(caja.IdTurnoCaja);
        _efectivoContado.Value = Math.Min(_efectivoEsperado, _efectivoContado.Maximum);
        var r = _logica.ObtenerResumen(caja.IdTurnoCaja);
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20), ColumnCount = 1, RowCount = 5 };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize)); root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); root.RowStyles.Add(new RowStyle(SizeType.AutoSize)); root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.Controls.Add(EstiloCaja.Etiqueta("Revisá los ingresos del turno antes de confirmar el cierre", true), 0, 0);
        var apertura = new Label { Text = $"Caja de {caja.NombreEmpleado} · Fondo inicial de efectivo: {caja.MontoApertura:C2}", Dock = DockStyle.Fill, Padding = new Padding(12), BackColor = Color.White, Font = new Font("Segoe UI", 10F, FontStyle.Bold), AutoSize = true };
        root.Controls.Add(apertura, 0, 1);
        var grilla = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.FixedSingle, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false };
        grilla.EnableHeadersVisualStyles = false; grilla.ColumnHeadersDefaultCellStyle.BackColor = Color.Gold; grilla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        grilla.Columns.Add(new DataGridViewTextBoxColumn { Name = "Metodo", HeaderText = "Método de pago", ReadOnly = true });
        grilla.Columns.Add(new DataGridViewTextBoxColumn { Name = "Ventas", HeaderText = "Ventas cobradas", ReadOnly = true, DefaultCellStyle = new() { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight } });
        grilla.Columns.Add(new DataGridViewTextBoxColumn { Name = "CuentaCorriente", HeaderText = "Cobros de cuenta corriente", ReadOnly = true, DefaultCellStyle = new() { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight } });
        grilla.Columns.Add(new DataGridViewTextBoxColumn { Name = "Total", HeaderText = "Total", ReadOnly = true, DefaultCellStyle = new() { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight, Font = new Font("Segoe UI", 9F, FontStyle.Bold) } });
        grilla.Rows.Add("Efectivo", r.Efectivo - r.CobrosCuentaCorrienteEfectivo, r.CobrosCuentaCorrienteEfectivo, r.Efectivo);
        grilla.Rows.Add("Tarjeta", r.Tarjeta - r.CobrosCuentaCorrienteTarjeta, r.CobrosCuentaCorrienteTarjeta, r.Tarjeta);
        grilla.Rows.Add("Transferencia / Mercado Pago", r.Transferencia - r.CobrosCuentaCorrienteTransferencia, r.CobrosCuentaCorrienteTransferencia, r.Transferencia);
        root.Controls.Add(grilla, 0, 2);
        var pie = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, AutoSize = true, Padding = new Padding(0, 12, 0, 0) };
        var datosConteo = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, WrapContents = true };
        datosConteo.Controls.Add(EstiloCaja.Etiqueta($"Efectivo esperado: {_efectivoEsperado:C2}"));
        datosConteo.Controls.Add(EstiloCaja.Etiqueta("Efectivo contado")); datosConteo.Controls.Add(_efectivoContado);
        datosConteo.Controls.Add(EstiloCaja.Etiqueta("Motivo (si hay diferencia)")); datosConteo.Controls.Add(_motivoDiferencia);
        var efectivo = new Label { Text = $"Diferencia: {_efectivoContado.Value - _efectivoEsperado:C2}", AutoSize = true, Padding = new Padding(12), BackColor = Color.White, Font = new Font("Segoe UI", 11F, FontStyle.Bold), Anchor = AnchorStyles.Left };
        _efectivoContado.ValueChanged += (_, _) => efectivo.Text = $"Diferencia: {_efectivoContado.Value - _efectivoEsperado:C2}";
        var acciones = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill };
        var confirmar = EstiloCaja.Boton("Confirmar cierre", Color.ForestGreen); var cancelar = EstiloCaja.Boton("Cancelar", Color.IndianRed);
        acciones.Controls.Add(confirmar); acciones.Controls.Add(cancelar); pie.Controls.Add(efectivo, 0, 0); pie.Controls.Add(acciones, 1, 0);
        root.Controls.Add(datosConteo, 0, 3); root.Controls.Add(pie, 0, 4); Controls.Add(root);
        confirmar.Click += (_, _) => { try { _logica.CerrarCaja(caja.IdTurnoCaja, Sesion.IdUsuario, _efectivoContado.Value, _motivoDiferencia.Text, Sesion.Rol); DialogResult = DialogResult.OK; Close(); } catch (Exception ex) { MessageBox.Show(this, ex.Message, "No se pudo cerrar la caja", MessageBoxButtons.OK, MessageBoxIcon.Warning); } };
        cancelar.Click += (_, _) => Close();
    }
}

public class FrmRetiroEfectivo : Form
{
    private readonly LogicaCaja _logica; private readonly TurnoCaja _caja;
    private readonly TextBoxMonto _monto = new();
    private readonly TextBox _contrasena = new() { UseSystemPasswordChar = true, Width = 220, MaxLength = 8, TextAlign = HorizontalAlignment.Center };
    public FrmRetiroEfectivo(LogicaCaja logica, TurnoCaja caja)
    {
        _logica = logica; _caja = caja; EstiloCaja.Preparar(this, "Retiro de Efectivo"); Size = new Size(440, 340);
        var retirar = EstiloCaja.BotonPuntoVenta("Confirmar retiro", Color.Firebrick);
        Controls.Add(EstiloCaja.Centrar(
            EstiloCaja.Etiqueta($"Disponible: {_logica.ObtenerEfectivoDisponible(caja.IdTurnoCaja):C2}"),
            EstiloCaja.Etiqueta("Monto a retirar"), _monto,
            EstiloCaja.Etiqueta("Contraseña del administrador"), _contrasena,
            retirar));
        retirar.Click += (_, _) =>
        {
            if (!_monto.TryObtenerMonto(out var monto))
            {
                MessageBox.Show(this, "Ingresá un monto mayor a cero.", "Monto inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _monto.Focus();
                return;
            }
            try
            {
                _logica.RetirarEfectivo(_caja.IdTurnoCaja, monto, _contrasena.Text);
                MessageBox.Show(this, "Retiro registrado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK; Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "No se pudo registrar el retiro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        };
    }
}