using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.UI
{
    // Alta de una compra a un proveedor
    public partial class FrmAltaCompra : Form
    {
        // Importes y costos siempre con formato argentino: 1.500,50
        private static readonly CultureInfo Es = new("es-AR");

        private readonly LogicaCompra _logicaCompra;
        private readonly LogicaProducto _logicaProducto;
        private readonly LogicaCarritoCompra _carrito;
        private readonly ErrorProvider _errores = new() { BlinkStyle = ErrorBlinkStyle.NeverBlink };

        // Timer de debounce del buscador: cada tecla lo reinicia y la búsqueda real se hace cuando se deja de tipear 300ms
        // Como el lector de código de barras "tipea" muy rápido, la búsqueda se dispara recién cuando termina de leer
        private readonly System.Windows.Forms.Timer _debounceBusqueda = new() { Interval = 300 };

        // Evita reentrancia al repoblar cmbResultados (cambia SelectedIndex y eso dispara eventos)
        private bool _actualizandoCombo;

        // Evita que al completar el texto del buscador por código (al elegir un producto) se dispare una nueva búsqueda
        private bool _suprimirBusqueda;

        // Producto elegido en el buscador, a la espera de cantidad y costo para agregarlo a la compra
        private Producto? _productoElegido;

        // Cantidad escrita como prefijo en el buscador ("10*coca")
        private int? _cantidadPrefijo;

        // El listado la consulta al cerrar este formulario para saber si tiene que recargar sus datos
        public bool CompraRegistrada { get; private set; }

        // Se pone en true cuando el usuario ya confirmó que quiere descartar la compra en curso, para no preguntarle dos veces al cerrar
        private bool _descartando;

        public FrmAltaCompra()
        {
            InitializeComponent();
            _logicaCompra = new LogicaCompra();
            _logicaProducto = new LogicaProducto();
            _carrito = new LogicaCarritoCompra();

            Load += FrmAltaCompra_Load;
            FormClosing += FrmAltaCompra_FormClosing;
            Disposed += (_, _) => _errores.Dispose();
        }

        // ================= Carga inicial =================

        private void FrmAltaCompra_Load(object? sender, EventArgs e)
        {
            ConfigurarControles();
            ConfigurarGrilla();
            SuscribirEventos();

            if (!CargarProveedores())
            {
                Close();
                return;
            }

            ActualizarTotal();
            ActualizarEstadoAcciones();
            cmbProveedor.Focus();
        }

        // Propiedades importantes que también se fijan acá por si en el Designer quedó alguna sin configurar
        private void ConfigurarControles()
        {
            Text = "Nueva Compra";
            CancelButton = btnCancelar;

            cmbProveedor.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbResultados.DropDownStyle = ComboBoxStyle.DropDownList;

            // La fecha de la compra no puede ser futura. Por defecto, hoy
            dtpFecha.Format = DateTimePickerFormat.Short;
            dtpFecha.MaxDate = DateTime.Today;
            dtpFecha.Value = DateTime.Today;

            txtComprobante.MaxLength = Compra.LargoMaximoComprobante;
            _errores.ContainerControl = this;
            txtBuscarProducto.MaxLength = 100;
            txtBuscarProducto.PlaceholderText = "Nombre o código de barras...";
            txtCostoUnitario.MaxLength = 14;
            txtCostoUnitario.TextAlign = HorizontalAlignment.Right;

            nudCantidad.DecimalPlaces = 0;
            nudCantidad.Minimum = 1;
            nudCantidad.Maximum = DetalleCompra.CantidadMaxima;
            nudCantidad.ThousandsSeparator = true;
            nudCantidad.Value = 1;

            lblInfoProducto.Text = string.Empty;
        }

        private void SuscribirEventos()
        {
            txtBuscarProducto.TextChanged += txtBuscarProducto_TextChanged;
            txtBuscarProducto.KeyDown += txtBuscarProducto_KeyDown;
            cmbResultados.SelectionChangeCommitted += cmbResultados_SelectionChangeCommitted;
            nudCantidad.KeyDown += nudCantidad_KeyDown;
            txtCostoUnitario.KeyPress += txtCostoUnitario_KeyPress;
            txtCostoUnitario.KeyDown += txtCostoUnitario_KeyDown;
            txtCostoUnitario.Leave += txtCostoUnitario_Leave;
            txtCostoUnitario.TextChanged += (_, _) => _errores.SetError(txtCostoUnitario, string.Empty);
            cmbProveedor.SelectedIndexChanged += (_, _) => _errores.SetError(cmbProveedor, string.Empty);
            txtBuscarProducto.TextChanged += (_, _) => _errores.SetError(txtBuscarProducto, string.Empty);
            btnAgregar.Click += btnAgregar_Click;
            btnQuitar.Click += btnQuitar_Click;
            dgvCompra.KeyDown += dgvCompra_KeyDown;
            btnConfirmar.Click += btnConfirmar_Click;
            btnCancelar.Click += btnCancelar_Click;
            dgvCompra.SelectionChanged += (_, _) => ActualizarEstadoAcciones();
            _debounceBusqueda.Tick += _debounceBusqueda_Tick;
            Disposed += (_, _) => _debounceBusqueda.Dispose();
        }

        // El combo arranca sin proveedor preseleccionado: hay que elegirlo explícitamente
        private bool CargarProveedores()
        {
            try
            {
                var proveedores = _logicaCompra.ObtenerProveedoresActivos();

                cmbProveedor.Items.Clear();
                cmbProveedor.Items.AddRange(proveedores.Cast<object>().ToArray());
                cmbProveedor.SelectedIndex = -1;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudieron cargar los proveedores: {ex.Message}",
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void ConfigurarGrilla()
        {
            dgvCompra.AutoGenerateColumns = false;
            dgvCompra.AllowUserToAddRows = false;
            dgvCompra.AllowUserToDeleteRows = false;
            dgvCompra.ReadOnly = true;
            dgvCompra.MultiSelect = false;
            dgvCompra.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCompra.RowHeadersVisible = false;
            dgvCompra.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Name es la clave para Cells["..."], DataPropertyName bindea con el objeto de la fila
            dgvCompra.Columns.Clear();
            dgvCompra.Columns.Add(new DataGridViewTextBoxColumn { Name = "IdProducto", DataPropertyName = "IdProducto", Visible = false });
            dgvCompra.Columns.Add(new DataGridViewTextBoxColumn { Name = "Codigo", DataPropertyName = "Codigo", HeaderText = "Código", FillWeight = 22 });
            dgvCompra.Columns.Add(new DataGridViewTextBoxColumn { Name = "Producto", DataPropertyName = "Producto", HeaderText = "Producto", FillWeight = 50 });
            dgvCompra.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Cantidad",
                DataPropertyName = "Cantidad",
                HeaderText = "Cantidad",
                FillWeight = 14,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", FormatProvider = Es, Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvCompra.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CostoUnitario",
                DataPropertyName = "CostoUnitario",
                HeaderText = "Costo unitario",
                FillWeight = 20,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", FormatProvider = Es, Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvCompra.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Subtotal",
                DataPropertyName = "Subtotal",
                HeaderText = "Subtotal",
                FillWeight = 20,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", FormatProvider = Es, Alignment = DataGridViewContentAlignment.MiddleRight }
            });
        }

        // Buscador de productos

        private void txtBuscarProducto_TextChanged(object? sender, EventArgs e)
        {
            if (_suprimirBusqueda) return;

            _debounceBusqueda.Stop();

            // Si el usuario vuelve a escribir en el buscador, se descarta el producto que había elegido
            DescartarProductoElegido();

            if (SepararCantidad(txtBuscarProducto.Text).texto.Length < 2)
            {
                LimpiarCombo();
                return;
            }

            _debounceBusqueda.Start();
        }

        private void _debounceBusqueda_Tick(object? sender, EventArgs e)
        {
            _debounceBusqueda.Stop();
            BuscarProductos();
        }

        private void txtBuscarProducto_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                MoverSeleccionCombo(e.KeyCode == Keys.Down ? 1 : -1);
                return;
            }

            if (e.KeyCode != Keys.Enter) return;

            e.Handled = true;
            e.SuppressKeyPress = true;

            // Si se apretó Enter antes de que venza el debounce, el combo todavía muestra resultados del texto anterior:
            // se busca ya mismo con el texto actual para no elegir un producto equivocado
            bool busquedaPendiente = _debounceBusqueda.Enabled;
            if (busquedaPendiente)
            {
                _debounceBusqueda.Stop();
                BuscarProductos(avisarSiNoHayResultados: true);
            }

            if (_productoElegido != null)
            {
                IrACantidad();
                return;
            }

            if (cmbResultados.SelectedItem is ProductoItem resaltado)
            {
                ElegirProducto(resaltado.Producto);
                return;
            }

            if (!busquedaPendiente)
                BuscarProductos(avisarSiNoHayResultados: true);
        }

        private void cmbResultados_SelectionChangeCommitted(object? sender, EventArgs e)
        {
            if (_actualizandoCombo) return;
            if (cmbResultados.SelectedItem is ProductoItem item)
                ElegirProducto(item.Producto);
        }

        private void BuscarProductos(bool avisarSiNoHayResultados = false)
        {
            var (cantidadPrefijo, texto) = SepararCantidad(txtBuscarProducto.Text);
            _cantidadPrefijo = cantidadPrefijo;

            if (texto.Length < 2)
            {
                LimpiarCombo();
                return;
            }

            List<Producto> resultados;
            try
            {
                // La "Hora Padel" de cada cancha figura como producto, pero no es mercadería que se le compre a un proveedor
                resultados = _logicaProducto.Buscar(texto, null, null, activo: true)
                    .Where(p => p.Categoria?.Nombre != "Hora Padel")
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar productos: {ex.Message}",
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Coincidencia exacta con un código de barras (lector): se elige directo
            var porCodigo = resultados.FirstOrDefault(p => string.Equals(p.CodigoBarras, texto, StringComparison.OrdinalIgnoreCase));
            if (porCodigo != null)
            {
                ElegirProducto(porCodigo);
                return;
            }

            MostrarResultados(resultados);

            if (resultados.Count == 0)
            {
                // Mientras se tipea solo queda la lista vacía; el aviso aparece cuando se aprieta Enter, y es un texto en pantalla (no una ventana)
                if (avisarSiNoHayResultados)
                    MostrarInfo($"No se encontró ningún producto para “{texto}”. Revise que esté bien escrito o pruebe con el código de barras.", esError: true);
            }
            else
            {
                lblInfoProducto.Text = string.Empty;
            }
        }

        private void MostrarResultados(List<Producto> resultados)
        {
            _actualizandoCombo = true;
            try
            {
                cmbResultados.DroppedDown = false;
                cmbResultados.SelectedIndex = -1;
                cmbResultados.Items.Clear();

                foreach (var p in resultados)
                    cmbResultados.Items.Add(new ProductoItem(p));

                // Queda resaltado el primero, así se elige con Enter sin tener que moverse
                cmbResultados.SelectedIndex = cmbResultados.Items.Count > 0 ? 0 : -1;
            }
            finally
            {
                _actualizandoCombo = false;
            }
        }

        private void LimpiarCombo()
        {
            _actualizandoCombo = true;
            try
            {
                cmbResultados.DroppedDown = false;
                cmbResultados.SelectedIndex = -1;
                cmbResultados.Items.Clear();
            }
            finally
            {
                _actualizandoCombo = false;
            }
        }

        private void MoverSeleccionCombo(int direccion)
        {
            if (cmbResultados.Items.Count == 0) return;

            int nuevoIndice = Math.Max(0, Math.Min(cmbResultados.Items.Count - 1, cmbResultados.SelectedIndex + direccion));
            cmbResultados.SelectedIndex = nuevoIndice;
        }

        // El producto queda elegido: se muestra su nombre en el buscador, se sugiere su costo actual y el foco pasa a la cantidad
        private void ElegirProducto(Producto producto)
        {
            _productoElegido = producto;
            _errores.SetError(txtBuscarProducto, string.Empty);

            _suprimirBusqueda = true;
            try { txtBuscarProducto.Text = producto.Nombre; }
            finally { _suprimirBusqueda = false; }

            LimpiarCombo();

            nudCantidad.Value = Math.Max(nudCantidad.Minimum, Math.Min(nudCantidad.Maximum, _cantidadPrefijo ?? 1));
            txtCostoUnitario.Text = producto.PrecioCosto > 0 ? producto.PrecioCosto.ToString("N2", Es) : string.Empty;

            // Se muestra el stock y el costo actual para poder comparar con el costo de esta compra
            string costoActual = producto.PrecioCosto > 0 ? producto.PrecioCosto.ToString("C2", Es) : "sin costo cargado";
            MostrarInfo($"Stock actual: {producto.StockActual:N0}   ·   Costo actual: {costoActual}", esError: false);

            IrACantidad();
        }

        private void DescartarProductoElegido()
        {
            _productoElegido = null;
            _cantidadPrefijo = null;
        }

        private void IrACantidad()
        {
            nudCantidad.Focus();
            nudCantidad.Select(0, nudCantidad.Text.Length);
        }

        private void MostrarInfo(string mensaje, bool esError)
        {
            lblInfoProducto.ForeColor = esError ? Color.Firebrick : Color.DimGray;
            lblInfoProducto.Text = mensaje;
        }

        // "10*coca" -> (10, "coca"). Devuelve (null, texto) si no hay prefijo de cantidad
        private static (int? cantidad, string texto) SepararCantidad(string bruto)
        {
            var m = Regex.Match((bruto ?? string.Empty).Trim(), @"^(\d{1,4})\s*\*\s*(.*)$");
            return m.Success
                ? (int.Parse(m.Groups[1].Value), m.Groups[2].Value.Trim())
                : (null, (bruto ?? string.Empty).Trim());
        }

        // Cantidad y costo

        private void nudCantidad_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.Handled = true;
            e.SuppressKeyPress = true;

            txtCostoUnitario.Focus();
            txtCostoUnitario.SelectAll();
        }

        private void txtCostoUnitario_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.Handled = true;
            e.SuppressKeyPress = true;

            AgregarAlCarrito();
        }

        // El costo solo admite números y una coma para los centavos (los puntos de los miles se agregan solos al salir del campo)
        private void txtCostoUnitario_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;

            bool esDigito = e.KeyChar >= '0' && e.KeyChar <= '9';
            bool esComaValida = e.KeyChar == ',' && !txtCostoUnitario.Text.Contains(',');
            if (!esDigito && !esComaValida)
                e.Handled = true;
        }

        // Al salir del campo se muestra prolijo: 1500 -> 1.500,00
        private void txtCostoUnitario_Leave(object? sender, EventArgs e)
        {
            if (TryLeerCosto(out decimal costo))
                txtCostoUnitario.Text = costo.ToString("N2", Es);
        }

        // Lee el costo tal como está escrito: acepta 1500, 1500,5, 1.500,50
        private bool TryLeerCosto(out decimal costo)
        {
            string texto = txtCostoUnitario.Text.Trim().Replace(".", string.Empty).Replace(',', '.');
            return decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out costo);
        }

        // Lee la cantidad tal como está escrita en el NumericUpDown, aunque todavía no haya confirmado el valor
        // (el NumericUpDown recién actualiza Value al perder el foco o al validar)
        private int LeerCantidad()
        {
            string texto = nudCantidad.Text.Replace(".", string.Empty).Replace(",", string.Empty);
            if (int.TryParse(texto, out int c))
                return Math.Max((int)nudCantidad.Minimum, Math.Min((int)nudCantidad.Maximum, c));
            return (int)nudCantidad.Value;
        }

        // Compra en armado
        private void btnAgregar_Click(object? sender, EventArgs e) => AgregarAlCarrito();

        private void AgregarAlCarrito()
        {
            if (_productoElegido == null)
            {
                _errores.SetError(txtBuscarProducto, "Busque y elija un producto de la lista.");
                MostrarInfo("Busque el producto y selecciónelo en la lista de coincidencias.", esError: true);
                txtBuscarProducto.Focus();
                return;
            }

            if (!TryLeerCosto(out decimal costo) || costo <= 0m)
            {
                string mensaje = string.IsNullOrWhiteSpace(txtCostoUnitario.Text)
                    ? "Ingrese el costo unitario."
                    : !TryLeerCosto(out _)
                        ? "Use un monto válido, por ejemplo 1.500,50."
                        : "El costo debe ser mayor que $0.";
                _errores.SetError(txtCostoUnitario, mensaje);
                MostrarInfo(mensaje, esError: true);
                txtCostoUnitario.Focus();
                txtCostoUnitario.SelectAll();
                return;
            }

            int cantidad = LeerCantidad();

            try
            {
                var producto = _productoElegido;
                bool sumado = _carrito.AgregarProducto(producto, cantidad, costo);

                RefrescarGrilla();
                ActualizarTotal();
                LimpiarEntrada();

                if (sumado)
                {
                    int cantidadTotal = _carrito.Items.First(i => i.Producto.IdProducto == producto.IdProducto).Cantidad;
                    MostrarInfo($"“{producto.Nombre}” ya estaba en la compra: se sumó la cantidad (ahora {cantidadTotal:N0}) y se usó el último costo ingresado.", esError: false);
                }

                txtBuscarProducto.Focus();
            }
            catch (ArgumentException ex)
            {
                // Si falla no se pierde lo que el usuario había cargado
                MessageBox.Show(ex.Message, "No se pudo agregar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Deja la zona de carga lista para el próximo producto
        private void LimpiarEntrada()
        {
            DescartarProductoElegido();

            _suprimirBusqueda = true;
            try { txtBuscarProducto.Clear(); }
            finally { _suprimirBusqueda = false; }

            LimpiarCombo();
            nudCantidad.Value = 1;
            txtCostoUnitario.Clear();
            _errores.SetError(txtCostoUnitario, string.Empty);
            _errores.SetError(txtBuscarProducto, string.Empty);
            lblInfoProducto.Text = string.Empty;
        }

        private void btnQuitar_Click(object? sender, EventArgs e) => QuitarProductoSeleccionado();

        // Con la grilla seleccionada, la tecla Supr también quita la línea
        private void dgvCompra_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Delete) return;
            e.Handled = true;
            QuitarProductoSeleccionado();
        }

        private void QuitarProductoSeleccionado()
        {
            if (dgvCompra.CurrentRow == null)
            {
                MessageBox.Show("Seleccione el producto que quiere quitar de la compra.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int idProducto = (int)dgvCompra.CurrentRow.Cells["IdProducto"].Value;
            _carrito.QuitarProducto(idProducto);

            RefrescarGrilla();
            ActualizarTotal();
        }

        private void RefrescarGrilla()
        {
            dgvCompra.DataSource = _carrito.Items.Select(i => new
            {
                IdProducto = i.Producto.IdProducto,
                Codigo = i.Producto.CodigoBarras ?? string.Empty,
                Producto = i.Producto.Nombre,
                i.Cantidad,
                CostoUnitario = i.PrecioCostoUnitario,
                i.Subtotal
            }).ToList();
            ActualizarEstadoAcciones();
        }

        private void ActualizarTotal() => lblTotal.Text = $"Total: {_carrito.Total.ToString("C2", Es)}";

        private void ActualizarEstadoAcciones()
        {
            btnConfirmar.Enabled = _carrito.Items.Count > 0;
            btnQuitar.Enabled = _carrito.Items.Count > 0 && dgvCompra.CurrentRow != null;
        }

        //Confirmar / cancelar 

        private void btnConfirmar_Click(object? sender, EventArgs e)
        {
            if (cmbProveedor.SelectedItem is not ProveedorOpcion proveedor)
            {
                _errores.SetError(cmbProveedor, "Seleccione el proveedor de la compra.");
                cmbProveedor.Focus();
                return;
            }

            if (_carrito.Items.Count == 0)
            {
                MessageBox.Show("Agregue al menos un producto a la compra.",
                    "Compra vacía", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtBuscarProducto.Focus();
                return;
            }

            try
            {
                var items = _carrito.Items.ToList();
                var impacto = _logicaCompra.PrevisualizarImpactoCompra(proveedor.IdProveedor, items);
                if (!ConfirmarImpactoCompra(proveedor, impacto, _carrito.Total)) return;

                // El usuario activo y la fecha/hora de registro los resuelve LogicaCompra a partir de la sesión y la fecha elegida
                var compra = _logicaCompra.RegistrarCompra(
                    proveedor.IdProveedor, dtpFecha.Value.Date, txtComprobante.Text, items, Sesion.IdUsuario);

                CompraRegistrada = true;
                MessageBox.Show($"La compra N° {compra.IdCompra} se registró correctamente.",
                    "Compra registrada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            catch (ArgumentException ex)
            {
                // Error de validación: se muestra el motivo y se conserva todo lo cargado para corregirlo
                MessageBox.Show(ex.Message, "No se pudo registrar la compra", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo registrar la compra y no se guardó nada: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Vista previa de los cambios de inventario y precios antes de confirmar la compra.
        private bool ConfirmarImpactoCompra(
            ProveedorOpcion proveedor,
            PrevisualizacionImpactoCompra impacto,
            decimal total)
        {
            using var vista = new Form
            {
                Text = "Revisar impacto de la compra",
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                Font = new Font("Segoe UI", 9F),
                BackColor = SystemColors.Info,
                Size = new Size(1080, 540)
            };

            var contenido = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(14),
                ColumnCount = 1,
                RowCount = 4,
                BackColor = SystemColors.Info
            };
            contenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            contenido.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            contenido.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            contenido.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            contenido.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var titulo = new Label
            {
                Text = $"Proveedor: {proveedor.Texto}    ·    Total de la compra: {total.ToString("C2", Es)}",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Margin = new Padding(4, 4, 4, 12)
            };

            var grilla = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            grilla.Columns.Add(new DataGridViewTextBoxColumn
            { DataPropertyName = "Producto", HeaderText = "Producto", FillWeight = 28 });
            grilla.Columns.Add(new DataGridViewTextBoxColumn
            { DataPropertyName = "Cantidad", HeaderText = "Ingresa", FillWeight = 9 });
            grilla.Columns.Add(new DataGridViewTextBoxColumn
            { DataPropertyName = "Stock", HeaderText = "Stock: actual + ingreso = nuevo", FillWeight = 19 });
            grilla.Columns.Add(new DataGridViewTextBoxColumn
            { DataPropertyName = "Costo", HeaderText = "Costo unitario: actual → compra", FillWeight = 20 });
            grilla.Columns.Add(new DataGridViewTextBoxColumn
            { DataPropertyName = "Precio", HeaderText = "Precio de venta base: actual → nuevo", FillWeight = 23 });
            grilla.Columns.Add(new DataGridViewTextBoxColumn
            { DataPropertyName = "Proveedor", HeaderText = "Proveedor: anterior → nuevo", FillWeight = 21 });

            grilla.DataSource = impacto.Productos.Select(p => new
            {
                Producto = p.Nombre,
                Cantidad = p.Cantidad.ToString("N0", Es),
                Stock = $"{p.StockActual.ToString("N0", Es)} + {p.Cantidad.ToString("N0", Es)} = {p.StockNuevo.ToString("N0", Es)}",
                Costo = $"{p.CostoActual.ToString("C2", Es)} → {p.CostoCompra.ToString("C2", Es)}",
                Precio = p.RecalculaPrecio
                    ? $"{p.PrecioVentaActual.ToString("C2", Es)} → {p.PrecioVentaNuevo!.Value.ToString("C2", Es)}"
                    : $"{p.PrecioVentaActual.ToString("C2", Es)} → sin cambio ({p.MotivoPrecioSinCambio})",
                Proveedor = $"{p.ProveedorActual} → {p.ProveedorNuevo}"
            }).ToList();

            var aclaracion = new Label
            {
                Text = "El precio de venta se actualiza si el stock anterior era cero o si el costo de compra supera al registrado. Varios y Sin Categoria conservan el precio manual.",
                AutoSize = true,
                ForeColor = Color.DimGray,
                Margin = new Padding(4, 10, 4, 8)
            };

            var acciones = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false
            };
            var registrar = new Button
            {
                Text = "Registrar compra",
                Size = new Size(176, 40),
                BackColor = Color.Green,
                ForeColor = Color.White,
                UseVisualStyleBackColor = false,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Margin = new Padding(6)
            };
            var volver = new Button
            {
                Text = "Volver",
                Size = new Size(176, 40),
                BackColor = Color.Gold,
                ForeColor = Color.Black,
                UseVisualStyleBackColor = false,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Margin = new Padding(6)
            };
            acciones.Controls.Add(registrar);
            acciones.Controls.Add(volver);

            contenido.Controls.Add(titulo, 0, 0);
            contenido.Controls.Add(grilla, 0, 1);
            contenido.Controls.Add(aclaracion, 0, 2);
            contenido.Controls.Add(acciones, 0, 3);
            vista.Controls.Add(contenido);
            vista.AcceptButton = registrar;
            vista.CancelButton = volver;
            registrar.Click += (_, _) =>
            {
                vista.DialogResult = DialogResult.Yes;
                vista.Close();
            };
            volver.Click += (_, _) =>
            {
                vista.DialogResult = DialogResult.No;
                vista.Close();
            };

            return vista.ShowDialog(this) == DialogResult.Yes;
        }

        // Cancelar (o Esc): si ya hay productos cargados, se pide confirmación para no perder el trabajo por accidente
        private void btnCancelar_Click(object? sender, EventArgs e)
        {
            if (_carrito.Items.Count > 0 && !ConfirmarDescarte()) return;

            _descartando = true;
            Close();
        }

        // Lo mismo si se cierra con la X de la ventana
        private void FrmAltaCompra_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (CompraRegistrada || _descartando || _carrito.Items.Count == 0) return;
            if (e.CloseReason != CloseReason.UserClosing) return;

            if (!ConfirmarDescarte())
                e.Cancel = true;
        }

        private bool ConfirmarDescarte()
            => MessageBox.Show("Hay productos cargados en la compra. ¿Desea descartarla?",
                   "Descartar compra", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

        // Elemento del combo de resultados: muestra el nombre (y el código de barras si tiene) y guarda el producto completo
        private sealed class ProductoItem
        {
            public Producto Producto { get; }

            public ProductoItem(Producto producto) => Producto = producto;

            public override string ToString()
                => string.IsNullOrWhiteSpace(Producto.CodigoBarras) ? Producto.Nombre : $"{Producto.Nombre}  [{Producto.CodigoBarras}]";
        }
    }
}
