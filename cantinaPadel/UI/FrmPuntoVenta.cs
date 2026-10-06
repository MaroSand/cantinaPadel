using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.UI
{
    public partial class FrmPuntoVenta : Form
    {
        // Se declaran las variables globales
        private readonly LogicaProducto _logicaProducto;
        private readonly LogicaCarrito _logicaCarrito;
        private readonly LogicaCliente _logicaCliente;
        private readonly LogicaVenta _logicaVenta;
        private readonly LogicaCuentaCorriente _logicaCuentaCorriente;

        // Reemplaza al modal FrmMetodoPago. Los checks se comportan como selección única (tipo radio buttons)
        private List<CheckBox> _checksMetodoPago = new();
        private bool _actualizandoChecksMetodoPago;

        // Timer de debounce para el autocompletado cada tecla reinicia el timer de 300ms, recién cuando el usuario deja de tipear por ese tiempo,
        // se dispara la búsqueda real
        private readonly System.Windows.Forms.Timer _debounceBusqueda = new() { Interval = 300 };

        // Evita reentrancia: cuando se repuebla cmbResultados, SelectedIndex cambia y dispara SelectionChangeCommitted, que a su vez llama a AgregarProductoAlCarrito
        private bool _actualizandoComboResultados;

        // Cliente elegido para la venta. Si es null, se asume Consumidor Final
        private Cliente? _clienteVenta;

        // Indica si la pantalla se abrió desde la pestaña de Cuenta Corriente (true) o desde la de Punto de Venta (false)
        private readonly bool _abrirEnCuentaCorriente;

        public FrmPuntoVenta() : this(abrirEnCuentaCorriente: false) { }

        public FrmPuntoVenta(bool abrirEnCuentaCorriente)
        {
            InitializeComponent();
            _logicaProducto = new LogicaProducto();
            _logicaCarrito = new LogicaCarrito();
            _logicaCliente = new LogicaCliente();
            _logicaVenta = new LogicaVenta();
            _logicaCuentaCorriente = new LogicaCuentaCorriente();
            _abrirEnCuentaCorriente = abrirEnCuentaCorriente;

            this.Load += FrmPuntoVenta_Load;
        }

        private void FrmPuntoVenta_Load(object sender, EventArgs e)
        {
            ConfigurarGrillaCarrito();

            // Se suscriben los eventos de controles
            txtBuscarProducto.KeyDown += txtBuscarProducto_KeyDown;
            txtBuscarProducto.TextChanged += txtBuscarProducto_TextChanged;
            cmbResultados.SelectionChangeCommitted += cmbResultados_SelectionChangeCommitted;
            btnQuitarDelCarrito.Click += btnQuitarDelCarrito_Click;
            btnVaciarCarrito.Click += btnVaciarCarrito_Click;
            nudCantidad.KeyDown += nudCantidad_KeyDown;
            btnConfirmarVenta.Click += btnConfirmarVenta_Click;
            btnAgregarCliente.Click += btnAgregarCliente_Click;
            btnAgregarTurno.Click += btnAgregarTurno_Click;
            btnQuitarCliente.Click += btnQuitarCliente_Click;
            btnVerCuentaCorriente.Click += btnVerCuentaCorriente_Click;
            _debounceBusqueda.Tick += _debounceBusqueda_Tick;

            ConfigurarMetodoPago();

            ActualizarLabelTotal();
            ActualizarLabelCliente();

            if (_abrirEnCuentaCorriente)
            {
                tabsPrincipal.SelectedTab = tabCuentaCorriente;
                if (_clienteVenta != null)
                    cuentaCorrienteControl1.CargarCliente(_clienteVenta);
            }
            else
            {
                txtBuscarProducto.Focus();
            }
        }

        // Si ya hay un cliente elegido para la venta, se lo pasa directo a la cuenta corriente (evita buscarlo dos veces)
        // Si todavía es Consumidor Final, igual se cambia de pestaña y el usuario busca ahí al cliente que quiera consultar
        private void btnVerCuentaCorriente_Click(object? sender, EventArgs e)
        {
            if (_clienteVenta != null)
                cuentaCorrienteControl1.CargarCliente(_clienteVenta);

            tabsPrincipal.SelectedTab = tabCuentaCorriente;
        }

        // Método de pago embebido en la pantalla
        // Los checks se comportan como radio buttons (selección única)
        private void ConfigurarMetodoPago()
        {
            _checksMetodoPago = new List<CheckBox> { chkEfectivo, chkTransferencia, chkTarjeta, chkBilleteraVirtual, chkCuentaCorriente };
            foreach (var chk in _checksMetodoPago)
                chk.CheckedChanged += (sender, _) => SeleccionarMetodoPagoUnico((CheckBox)sender!);

            // Cuenta Corriente requiere cliente identificado: si todavía no se eligió ninguno (sigue en Consumidor Final), se avisa al tildarlo y
            // se vuelve a Efectivo, en vez de dejar avanzar y recién cortar en btnConfirmarVenta_Click
            chkCuentaCorriente.CheckedChanged += (_, _) =>
            {
                if (chkCuentaCorriente.Checked && _clienteVenta == null)
                {
                    MessageBox.Show(this,
                        "Cuenta Corriente requiere un cliente. Usá \"Agregar Cliente\" o \"Agregar Turno\" primero.",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkEfectivo.Checked = true;
                }
            };

            ConfigurarPagoEfectivo();
        }

        // Engancha los eventos del campo "Paga con" (los controles están en el diseñador)
        private void ConfigurarPagoEfectivo()
        {
            txtPagaCon.TextChanged += (_, _) => ActualizarVuelto();
            txtPagaCon.KeyDown += (_, e) =>
            {
                // Enter confirma la venta, para cobrar sin usar el mouse
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; btnConfirmarVenta.PerformClick(); }
            };
            chkEfectivo.CheckedChanged += (_, _) => ActualizarVuelto();
            ActualizarVuelto();
        }

        // Muestra u oculta el panel de efectivo y recalcula el vuelto con el total actual del carrito
        private void ActualizarVuelto()
        {
            pnlEfectivo.Visible = chkEfectivo.Checked;
            if (!chkEfectivo.Checked) return;

            decimal total = Math.Round(_logicaCarrito.Total, 2);
            if (!CalculadorVuelto.TryParsearMonto(txtPagaCon.Text, out var pagaCon) || pagaCon <= 0m)
            {
                lblVuelto.ForeColor = Color.DimGray;
                lblVuelto.Text = "Vuelto: -";
                return;
            }

            if (pagaCon < total)
            {
                lblVuelto.ForeColor = Color.Firebrick;
                lblVuelto.Text = $"Falta: {total - pagaCon:C2}";
            }
            else
            {
                lblVuelto.ForeColor = Color.ForestGreen;
                lblVuelto.Text = $"Vuelto: {pagaCon - total:C2}";
            }
        }

        private void SeleccionarMetodoPagoUnico(CheckBox seleccionado)
        {
            if (_actualizandoChecksMetodoPago) return; //los cambios de abajo no deben re-disparar este handler
            _actualizandoChecksMetodoPago = true;
            try
            {
                if (!seleccionado.Checked)
                {
                    seleccionado.Checked = true; // no se permite dejar todo destildado
                    return;
                }

                foreach (var chk in _checksMetodoPago.Where(chk => chk != seleccionado))
                    chk.Checked = false;
            }
            finally
            {
                _actualizandoChecksMetodoPago = false;
            }
        }

        private MetodoPago ObtenerMetodoPagoSeleccionado()
        {
            if (chkTransferencia.Checked) return MetodoPago.Transferencia;
            if (chkTarjeta.Checked) return MetodoPago.Tarjeta;
            if (chkBilleteraVirtual.Checked) return MetodoPago.BilleteraVirtual;
            if (chkCuentaCorriente.Checked) return MetodoPago.CuentaCorriente;
            return MetodoPago.Efectivo;
        }

        // Vuelve el panel de método de pago a su estado por defecto (Efectivo), para la próxima venta
        private void ResetearMetodoPago()
        {
            _actualizandoChecksMetodoPago = true;
            try
            {
                foreach (var chk in _checksMetodoPago)
                    chk.Checked = false;
                chkEfectivo.Checked = true;
            }
            finally
            {
                _actualizandoChecksMetodoPago = false;
            }

            txtPagaCon.Clear();
            ActualizarVuelto();
        }

        // Saldo a favor real (el cliente pagó de más), leído de la bd porque el objeto Cliente de la pantalla puede estar desactualizado
        // La venta ya quedó registrada, así que un fallo acá no debe impedir emitir el comprobante
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

        private void ConfigurarGrillaCarrito()
        {
            dgvCarrito.AutoGenerateColumns = false;
            dgvCarrito.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCarrito.MultiSelect = false;
            dgvCarrito.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;

            dgvCarrito.Columns.Clear();
            dgvCarrito.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "IdProducto", DataPropertyName = "IdProducto", Visible = false });
            dgvCarrito.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "Nombre", DataPropertyName = "Nombre", HeaderText = "Producto", Width = 180, ReadOnly = true });
            dgvCarrito.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Cantidad",
                DataPropertyName = "Cantidad",
                HeaderText = "Cant.",
                Width = 60,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvCarrito.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PrecioUnitario",
                DataPropertyName = "PrecioUnitario",
                HeaderText = "P. Unit.",
                Width = 90,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvCarrito.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Subtotal",
                DataPropertyName = "Subtotal",
                HeaderText = "Subtotal",
                Width = 100,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
        }

        // Cantidad escrita como prefijo en la barra de búsqueda: "3*coca" -> 3 unidades de "coca"
        // Devuelve (null, texto) si no hay prefijo
        private static (int? cantidad, string texto) SepararCantidad(string bruto)
        {
            var m = Regex.Match((bruto ?? string.Empty).Trim(), @"^(\d{1,3})\s*\*\s*(.*)$");
            return m.Success
                ? (int.Parse(m.Groups[1].Value), m.Groups[2].Value.Trim())
                : (null, (bruto ?? string.Empty).Trim());
        }

        // Lee la cantidad tal como está escrita en el NumericUpDown, aunque todavía no haya confirmado el valor
        // (el NumericUpDown recién actualiza Value al perder el foco o al validar)
        private int LeerCantidad()
        {
            if (int.TryParse(nudCantidad.Text, out int c))
                return Math.Max((int)nudCantidad.Minimum, Math.Min((int)nudCantidad.Maximum, c));
            return (int)nudCantidad.Value;
        }

        // Barra de búsqueda por nombre y por código de barras
        // El lector HID "tipea" el código muy rápido, como cada tecla reinicia el debounce, la búsqueda recién se
        // dispara cuando el lector termina de tipear, sin necesidad de que el lector mande un Enter. Si el texto
        // ingresado coincide exacto con el código de barras de un producto, se lo agrega directo al carrito
        // si no, se muestran las coincidencias por nombre en cmbResultados

        // Con las flechas Arriba/Abajo se navega el combo sin sacar el foco del textbox
        // Enter agrega la opción resaltada, si no hay ninguna resaltada, busca ya mismo
        private void txtBuscarProducto_KeyDown(object sender, KeyEventArgs e)
        {
            // F2 salta al campo de cantidad (con el número seleccionado para pisarlo escribiendo)
            if (e.KeyCode == Keys.F2)
            {
                e.SuppressKeyPress = true;
                nudCantidad.Focus();
                nudCantidad.Select(0, nudCantidad.Text.Length);
                return;
            }
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
            {
                e.SuppressKeyPress = true;
                MoverSeleccionCombo(e.KeyCode == Keys.Down ? 1 : -1);
                return;
            }

            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                // Si el usuario tipeó y apretó Enter antes de que venza el debounce, el combo todavía muestra los
                // resultados del texto anterior. Se busca ya mismo con el texto actual para no agregar un producto equivocado
                bool busquedaPendiente = _debounceBusqueda.Enabled;
                if (busquedaPendiente)
                {
                    _debounceBusqueda.Stop();
                    BuscarProductosPorNombre(avisarSiNoHayResultados: true);
                }

                // Si hay una opción resaltada en el combo, Enter la agrega directo
                if (cmbResultados.SelectedItem is ProductoComboItem resaltado)
                {
                    AgregarProductoAlCarrito(resaltado.Producto, limpiarBusqueda: true);
                    return;
                }

                if (!busquedaPendiente)
                    BuscarProductosPorNombre(avisarSiNoHayResultados: true);
            }
        }

        // Enter desde el campo de cantidad: si ya hay un producto resaltado en el combo (porque se buscó
        // antes de tocar la cantidad), lo agrega directo. Si no, manda el foco a la barra de búsqueda para
        // que el flujo "cantidad primero, después busco el producto" también funcione con Enter.
        private void nudCantidad_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;

            if (cmbResultados.SelectedItem is ProductoComboItem resaltado)
            {
                AgregarProductoAlCarrito(resaltado.Producto, limpiarBusqueda: true);
            }
            else
            {
                txtBuscarProducto.Focus();
                txtBuscarProducto.SelectAll();
            }
        }


        // Mueve la selección del combo de resultados hacia arriba o abajo según la dirección indicada (1 = abajo, -1 = arriba)
        private void MoverSeleccionCombo(int direccion)
        {
            if (cmbResultados.Items.Count == 0) return;

            int nuevoIndice = cmbResultados.SelectedIndex + direccion;
            nuevoIndice = Math.Max(0, Math.Min(cmbResultados.Items.Count - 1, nuevoIndice));
            cmbResultados.SelectedIndex = nuevoIndice;
        }

        // Cada vez que se tipea algo en la barra de búsqueda, se reinicia el timer de debounce. Si el texto tiene menos de 2 caracteres, se limpia el combo y no se hace la búsqueda
        private void txtBuscarProducto_TextChanged(object sender, EventArgs e)
        {
            _debounceBusqueda.Stop();

            if (txtBuscarProducto.Text.Trim().Length < 2)
            {
                LimpiarCombo();
                return;
            }

            _debounceBusqueda.Start();
        }

        private void _debounceBusqueda_Tick(object? sender, EventArgs e)
        {
            _debounceBusqueda.Stop();
            BuscarProductosPorNombre();
        }


        // Busca productos por nombre y los muestra en cmbResultados. Si avisarSiNoHayResultados es true y no hay resultados, se muestra un mensaje de aviso
        private void BuscarProductosPorNombre(bool avisarSiNoHayResultados = false)
        {
            // Si el texto trae un prefijo de cantidad ("3*coca"), se busca solo por la parte del producto
            var (cantidadPrefijo, textoLimpio) = SepararCantidad(txtBuscarProducto.Text);

            // Solo se escribió la cantidad ("3*"): todavía no hay nada que buscar
            if (cantidadPrefijo != null && string.IsNullOrWhiteSpace(textoLimpio))
            {
                LimpiarCombo();
                return;
            }

            string? texto = string.IsNullOrWhiteSpace(textoLimpio) ? null : textoLimpio;

            List<Producto> resultados;
            try
            {
                resultados = _logicaProducto.Buscar(texto, null, null, activo: true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar productos: {ex.Message}",
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Si lo tipeado coincide exacto con un código de barras
                if (texto != null)
                {
                    Producto? porCodigo = resultados
                        .FirstOrDefault(p => string.Equals(p.CodigoBarras, texto, StringComparison.OrdinalIgnoreCase));

                    if (porCodigo != null)
                    {
                        AgregarProductoAlCarrito(porCodigo, limpiarBusqueda: true);
                        return;
                    }
                }

                MostrarResultadosEnCombo(resultados);

                // Mientras se tipea (debounce), si no hay resultados la lista del combo queda en blanco
                // El aviso de "no encontrado" se muestra cuando el usuario presiona Enter y no hay nada para mostrarle
                if (avisarSiNoHayResultados && resultados.Count == 0 && texto != null)
                {
                    MessageBox.Show($"No se encontró ningún producto para '{texto}'.",
                        "Producto no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error inesperado al mostrar los resultados ({ex.GetType().Name}): {ex.Message}",
                    "Error de interfaz", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Muestra los resultados de la búsqueda en cmbResultados, con el stock disponible descontando lo que ya está en el carrito
        private void MostrarResultadosEnCombo(List<Producto> resultados)
        {
            _actualizandoComboResultados = true;
            try
            {
                // Se limpia el combo antes de agregar los resultados, para que no queden residuos de búsquedas anteriores
                cmbResultados.DroppedDown = false;
                cmbResultados.SelectedIndex = -1;
                cmbResultados.Items.Clear();

                foreach (var p in resultados)
                {
                    int stockDisponible = Math.Max(0, p.StockActual - _logicaCarrito.ObtenerCantidadEnCarrito(p.IdProducto));
                    cmbResultados.Items.Add(new ProductoComboItem(p, stockDisponible));
                }

                // Si hay resultados, se selecciona el primero para que el usuario pueda agregarlo con Enter sin tener que mover la selección
                cmbResultados.SelectedIndex = cmbResultados.Items.Count > 0 ? 0 : -1;


            }
            finally
            {
                _actualizandoComboResultados = false;
            }
        }

        private void LimpiarCombo()
        {
            _actualizandoComboResultados = true;
            try
            {
                cmbResultados.DroppedDown = false;
                cmbResultados.SelectedIndex = -1;
                cmbResultados.Items.Clear();
            }
            finally
            {
                _actualizandoComboResultados = false;
            }
        }

        // El usuario selecciona un producto del combo de resultados: se agrega al carrito y se limpia la barra de búsqueda
        private void cmbResultados_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (_actualizandoComboResultados) return;
            if (cmbResultados.SelectedItem is not ProductoComboItem item) return;

            AgregarProductoAlCarrito(item.Producto, limpiarBusqueda: true);
        }

        // Agrega un producto al carrito. limpiarBusqueda=true después de un agregado exitoso (por código de
        // barras o por selección del combo): deja la barra de búsqueda lista para la próxima búsqueda/escaneo
        // La cantidad sale del prefijo "3*" de la barra de búsqueda si existe; si no, del campo de cantidad
        private void AgregarProductoAlCarrito(Producto producto, bool limpiarBusqueda = false)
        {
            var (cantidadPrefijo, _) = SepararCantidad(txtBuscarProducto.Text);
            int cantidad = cantidadPrefijo ?? LeerCantidad();

            try
            {
                _logicaCarrito.AgregarProducto(producto, cantidad);
                RefrescarUI(); // actualiza la grilla y el total

                nudCantidad.Value = 1;
                nudCantidad.Text = "1"; // por si había un valor escrito sin confirmar

                if (limpiarBusqueda)
                {
                    txtBuscarProducto.Clear();
                    LimpiarCombo();
                    txtBuscarProducto.Focus();
                }
            }
            catch (ArgumentException ex)
            {
                // Si falla (por ejemplo stock insuficiente) no se pierde lo que el usuario había escrito
                MessageBox.Show(ex.Message, "No se pudo agregar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Carrito
        private void ActualizarGrillaCarrito()
        {
            dgvCarrito.DataSource = _logicaCarrito.Items.Select(i => new FilaCarritoUI
            {
                IdProducto = i.Producto.IdProducto,
                Nombre = i.Producto.Nombre,
                Cantidad = i.Cantidad,
                PrecioUnitario = i.PrecioUnitario,
                Subtotal = i.Subtotal
            }).ToList();
        }

        private void ActualizarLabelTotal()
        {
            lblTotal.Text = $"Total: {_logicaCarrito.Total:C2}";
            ActualizarVuelto();
        }

        // Refresca lo que depende del estado del carrito: la grilla del carrito y el total
        private void RefrescarUI()
        {
            ActualizarGrillaCarrito();
            ActualizarLabelTotal();
        }


        private void btnQuitarDelCarrito_Click(object sender, EventArgs e)
        {
            if (dgvCarrito.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un producto del carrito para quitar.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var celda = dgvCarrito.CurrentRow.Cells["IdProducto"];
            if (celda?.Value == null || !int.TryParse(celda.Value.ToString(), out int idProducto))
                return;

            _logicaCarrito.QuitarProducto(idProducto);
            RefrescarUI();
        }

        private void btnVaciarCarrito_Click(object sender, EventArgs e)
        {
            if (_logicaCarrito.CantidadItems == 0) return;

            var confirmacion = MessageBox.Show("¿Vaciar todo el carrito?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion != DialogResult.Yes) return;

            _logicaCarrito.Vaciar();
            RefrescarUI();
        }

        // Confirmar venta
        private void btnConfirmarVenta_Click(object sender, EventArgs e)
        {
            if (_logicaCarrito.CantidadItems == 0)
            {
                MessageBox.Show("El carrito está vacío.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }


            // Antes de confirmar la venta, se valida que el stock de cada producto no haya cambiado desde que se agregó al carrito
            var faltantes = new System.Text.StringBuilder();
            foreach (var item in _logicaCarrito.Items)
            {
                var productoActual = _logicaProducto.ObtenerPorId(item.Producto.IdProducto);
                if (productoActual == null || !productoActual.Activo || productoActual.StockActual < item.Cantidad)
                {
                    int disponible = productoActual?.StockActual ?? 0;
                    faltantes.AppendLine($"- {item.Producto.Nombre}: pedido {item.Cantidad}, disponible {disponible}");
                }
            }

            if (faltantes.Length > 0)
            {
                MessageBox.Show(
                    "El stock cambió desde que armaste el carrito. Ajustá las cantidades:\n\n" + faltantes,
                    "Stock insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var metodo = ObtenerMetodoPagoSeleccionado();
            if (metodo == MetodoPago.CuentaCorriente && _clienteVenta == null)
            {
                MessageBox.Show(this, "Cuenta Corriente requiere un cliente. Seleccionar \"Agregar Cliente\" .",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Efectivo: "Paga con" es opcional; si se informó, tiene que ser un número válido y alcanzar para el total
            decimal pagaCon = 0m;
            decimal vuelto = 0m;
            if (metodo == MetodoPago.Efectivo)
            {
                if (!CalculadorVuelto.TryParsearMonto(txtPagaCon.Text, out pagaCon))
                {
                    MessageBox.Show(this, "El monto con el que paga el cliente no es válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPagaCon.Focus();
                    txtPagaCon.SelectAll();
                    return;
                }

                decimal totalACobrar = Math.Round(_logicaCarrito.Items.Sum(i => i.Subtotal), 2);
                var errorPago = CalculadorVuelto.Validar(totalACobrar, pagaCon);
                if (errorPago != null)
                {
                    MessageBox.Show(this, errorPago, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPagaCon.Focus();
                    txtPagaCon.SelectAll();
                    return;
                }

                vuelto = CalculadorVuelto.Calcular(totalACobrar, pagaCon);
            }

            try
            {
                var pago = new PagoVenta { Metodo = metodo };
                var cliente = _clienteVenta ?? _logicaVenta.ObtenerConsumidorFinal();
                var datos = new DatosVentaParaComprobante
                {
                    IdVenta = 0, // todavía no existe: la venta se registra recién después de confirmar el comprobante
                    Total = Math.Round(_logicaCarrito.Items.Sum(i => i.Subtotal), 2),
                    NombreCliente = $"{cliente.Persona.Nombre} {cliente.Persona.Apellido}",
                    EmailCliente = cliente.Email,
                    MetodoPago = pago.FormaPago,
                    CuitCliente = cliente.Persona.Cuit,
                    CondicionIvaCliente = cliente.Persona.CondicionIva,
                    Items = _logicaCarrito.Items.Select(i => new DetalleComprobante { Nombre = i.Producto.Nombre, Cantidad = i.Cantidad, PrecioUnitario = i.PrecioUnitario }).ToList(),
                    SaldoFavor = ObtenerSaldoAFavor(cliente),
                    PagoCon = pagaCon,
                    Vuelto = vuelto
                };

                // El diálogo de comprobante es el último paso del cobro y se muestra ANTES de registrar la venta.
                // Si el usuario cancela, no se registra nada y se conservan los productos del carrito, el cliente
                // y el método de pago elegidos, para poder corregir o reintentar sin volver a cargar todo
                using var comprobante = new FrmSeleccionComprobante(datos);
                if (comprobante.ShowDialog(this) != DialogResult.OK || comprobante.ComprobanteGenerado == null)
                {
                    txtBuscarProducto.Focus();
                    return;
                }

                var venta = _logicaVenta.ConfirmarVenta(_logicaCarrito.Items, _clienteVenta, pago, Sesion.IdUsuario);
                comprobante.ComprobanteGenerado.IdVenta = venta.IdVenta;
                _logicaVenta.ActualizarTipoComprobante(venta.IdVenta, comprobante.ComprobanteGenerado.Tipo);

                _logicaCarrito.Vaciar();
                _clienteVenta = null; // la venta terminó: la próxima arranca de nuevo con Consumidor Final
                ActualizarLabelCliente();
                ResetearMetodoPago();
                RefrescarUI();

                // El vuelto se avisa después de limpiar la pantalla, para que no quede en pantalla de la venta anterior
                if (vuelto > 0m)
                    MessageBox.Show(this, $"Venta registrada.\n\nTotal: {datos.Total:C2}\nPaga con: {pagaCon:C2}\n\nVUELTO: {vuelto:C2}",
                        "Vuelto", MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtBuscarProducto.Focus();
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
                MessageBox.Show(this, $"Ocurrió un error al registrar la venta: {DescribirError(ex)}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string DescribirError(Exception ex)
        {
            var detalle = new System.Text.StringBuilder(ex.Message);
            var interna = ex.InnerException;
            while (interna != null)
            {
                detalle.Append(" -> ").Append(interna.Message);
                interna = interna.InnerException;
            }
            return detalle.ToString();
        }


        // Actualiza el label que muestra el cliente elegido para la venta, o "Consumidor Final" si no hay ninguno. También habilita o deshabilita el botón de quitar cliente según corresponda
        private void ActualizarLabelCliente()
        {
            lblClienteVenta.Text = _clienteVenta == null
                ? "Cliente: Consumidor Final"
                : $"Cliente: {_clienteVenta.Persona.Nombre} {_clienteVenta.Persona.Apellido}";
            btnQuitarCliente.Enabled = _clienteVenta != null;
        }


        // Abre el formulario de listado de clientes en modo selección. Si el usuario elige uno, se guarda como cliente de la venta
        private void btnAgregarCliente_Click(object? sender, EventArgs e)
        {
            using var frmClientes = new FrmListadoClientes(modoSeleccion: true);

            if (frmClientes.ShowDialog(this) == DialogResult.OK && frmClientes.ClienteSeleccionado != null)
            {
                _clienteVenta = frmClientes.ClienteSeleccionado;
                ActualizarLabelCliente();
            }

            txtBuscarProducto.Focus();
        }

        private void btnQuitarCliente_Click(object? sender, EventArgs e)
        {
            _clienteVenta = null;
            ActualizarLabelCliente();
            txtBuscarProducto.Focus();
        }

        // Abre el formulario de registro de turno. Si el usuario confirma un turno, se intenta cargar el cliente del último alquiler y asignarlo a la venta
        private void btnAgregarTurno_Click(object? sender, EventArgs e)
        {
            var area = Screen.FromControl(this).WorkingArea;

            using var frmTurno = new FrmAlquilerDia
            {
                StartPosition = FormStartPosition.CenterScreen,
                ShowInTaskbar = false,
                MinimizeBox = false,
                Size = new Size(Math.Min(1200, area.Width - 40), Math.Min(800, area.Height - 40))
            };
            frmTurno.ShowDialog(this);

            if (frmTurno.IdClienteUltimoAlquiler > 0)
            {
                try
                {
                    var cliente = _logicaCliente.ObtenerPorId(frmTurno.IdClienteUltimoAlquiler);
                    if (cliente != null)
                    {
                        _clienteVenta = cliente;
                        ActualizarLabelCliente();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"El turno se registró, pero no se pudo cargar el cliente en la venta: {ex.Message}",
                        "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            txtBuscarProducto.Focus();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _debounceBusqueda.Stop();
            _debounceBusqueda.Dispose();
            base.OnFormClosed(e);
        }
    }

    // Clase auxiliar para mostrar los items del carrito en la grilla
    public class FilaCarritoUI
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }

    // Item de cmbResultados: envuelve el Producto encontrado
    public class ProductoComboItem
    {
        public Producto Producto { get; }
        public int IdProducto => Producto.IdProducto;
        public string Descripcion { get; }

        public ProductoComboItem(Producto producto, int stockDisponible)
        {
            Producto = producto;
            Descripcion = producto.Nombre;
        }

        public override string ToString() => Descripcion;
    }
}