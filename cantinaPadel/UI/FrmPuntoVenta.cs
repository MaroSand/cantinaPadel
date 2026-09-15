using System;
using System.Collections.Generic;
using System.Linq;
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

        // Timer de debounce para el autocompletado
        // cada tecla reinicia el timer de 300ms, recién cuando el usuario deja de tipear por ese tiempo, se dispara la búsqueda real
        private readonly System.Windows.Forms.Timer _debounceBusqueda = new() { Interval = 300 };

        // Últimos productos traídos por la búsqueda. Se cachean acá en vez de re-consultar la base para poder recalcular el "stock disponible"
        // que se ve en dgvResultadosBusqueda cada vez que el carrito cambia
        private List<Producto> _ultimosResultados = new();

        public FrmPuntoVenta()
        {
            InitializeComponent();
            _logicaProducto = new LogicaProducto();
            _logicaCarrito = new LogicaCarrito();

            this.Load += FrmPuntoVenta_Load;
        }

        private void FrmPuntoVenta_Load(object sender, EventArgs e)
        {
            ConfigurarGrillaResultados();
            ConfigurarGrillaCarrito();

            // Se suscriben los eventos de controles
            txtBuscarProducto.KeyDown += txtBuscarProducto_KeyDown;
            txtBuscarProducto.TextChanged += txtBuscarProducto_TextChanged;
            dgvResultadosBusqueda.CellDoubleClick += dgvResultadosBusqueda_CellDoubleClick;
            btnAgregarAlCarrito.Click += btnAgregarAlCarrito_Click;
            btnQuitarDelCarrito.Click += btnQuitarDelCarrito_Click;
            btnVaciarCarrito.Click += btnVaciarCarrito_Click;
            dgvCarrito.CellEndEdit += dgvCarrito_CellEndEdit;
            dgvCarrito.EditingControlShowing += dgvCarrito_EditingControlShowing;
            btnConfirmarVenta.Click += btnConfirmarVenta_Click;
            _debounceBusqueda.Tick += _debounceBusqueda_Tick;

            ActualizarLabelTotal();
            txtBuscarProducto.Focus();
        }

        // Configuración de grillas
        private void ConfigurarGrillaResultados()
        {
            dgvResultadosBusqueda.AutoGenerateColumns = false;
            dgvResultadosBusqueda.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvResultadosBusqueda.ReadOnly = true;
            dgvResultadosBusqueda.MultiSelect = false;

            dgvResultadosBusqueda.Columns.Clear();
            dgvResultadosBusqueda.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "IdProducto", DataPropertyName = "IdProducto", Visible = false });
            dgvResultadosBusqueda.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "Nombre", DataPropertyName = "Nombre", HeaderText = "Nombre", Width = 180 });
            dgvResultadosBusqueda.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "Marca", DataPropertyName = "Marca", HeaderText = "Marca", Width = 110 });
            dgvResultadosBusqueda.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "StockDisponible",
                DataPropertyName = "StockDisponible",
                HeaderText = "Stock",
                Width = 60,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvResultadosBusqueda.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PrecioConIva",
                DataPropertyName = "PrecioConIva",
                HeaderText = "Precio",
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle
                { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvResultadosBusqueda.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "CodigoBarras", DataPropertyName = "CodigoBarras", HeaderText = "Código", Width = 100 });
        }

        private void ConfigurarGrillaCarrito()
        {
            dgvCarrito.AutoGenerateColumns = false;
            dgvCarrito.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCarrito.MultiSelect = false;
            // Solo la columna Cantidad es editable; el resto son de solo lectura
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
                ReadOnly = false,
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

        // Código de barras
        // El lector HID "tipea" el código y termina mandando Enter. Si encuentra el producto y hay stock, lo agrega
        private void txtCodigoBarras_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            e.SuppressKeyPress = true;

            string codigo = txtCodigoBarras.Text.Trim();
            txtCodigoBarras.Clear();

            if (string.IsNullOrWhiteSpace(codigo))
            {
                txtCodigoBarras.Focus();
                return;
            }

            try
            {
                Producto? producto = _logicaProducto.ObtenerPorCodigoBarras(codigo);

                if (producto == null)
                {
                    MessageBox.Show($"No se encontró ningún producto con el código '{codigo}'.",
                        "Producto no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _logicaCarrito.AgregarProducto(producto, 1);
                RefrescarUI();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "No se pudo agregar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar el producto: {ex.Message}",
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                txtCodigoBarras.Focus();
            }
        }

        // Búsqueda por nombre
        private void txtBuscarProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            e.SuppressKeyPress = true;
            _debounceBusqueda.Stop(); // Enter busca ya, no hace falta esperar el debounce
            BuscarProductosPorNombre();
        }

        // Autocompletado en tiempo real: cada tecla reinicia el timer de debounce
        // Con menos de 2 caracteres no se busca y se limpia la grilla de resultados
        private void txtBuscarProducto_TextChanged(object sender, EventArgs e)
        {
            _debounceBusqueda.Stop();

            if (txtBuscarProducto.Text.Trim().Length < 2)
            {
                dgvResultadosBusqueda.DataSource = null;
                return;
            }

            _debounceBusqueda.Start();
        }

        private void _debounceBusqueda_Tick(object? sender, EventArgs e)
        {
            _debounceBusqueda.Stop();
            BuscarProductosPorNombre();
        }

        private void BuscarProductosPorNombre()
        {
            try
            {
                string? texto = string.IsNullOrWhiteSpace(txtBuscarProducto.Text)
                    ? null
                    : txtBuscarProducto.Text.Trim();

                _ultimosResultados = _logicaProducto.Buscar(texto, null, null, activo: true);
                ActualizarGrillaResultados();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar productos: {ex.Message}",
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Repinta dgvResultadosBusqueda a partir de _ultimosResultados (sin volver a consultar bs), mostrando
        // "stock disponible" = stock real - lo que ya hay de ese producto en el carrito. Se llama después de cualquier cambio
        // en el carrito para que el número se actualice en el momento
        private void ActualizarGrillaResultados()
        {
            dgvResultadosBusqueda.DataSource = _ultimosResultados.Select(p => new FilaResultadoUI
            {
                IdProducto = p.IdProducto,
                Nombre = p.Nombre,
                Marca = p.Marca?.Nombre ?? "-",
                CodigoBarras = p.CodigoBarras,
                PrecioConIva = p.PrecioConIva,
                StockDisponible = Math.Max(0, p.StockActual - _logicaCarrito.ObtenerCantidadEnCarrito(p.IdProducto))
            }).ToList();
        }

        // Doble click en un resultado también agrega al carrito, como atajo
        private void dgvResultadosBusqueda_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            AgregarSeleccionDeResultadosAlCarrito();
        }

        private void btnAgregarAlCarrito_Click(object sender, EventArgs e)
        {
            AgregarSeleccionDeResultadosAlCarrito();
        }

        private void AgregarSeleccionDeResultadosAlCarrito()
        {
            if (dgvResultadosBusqueda.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un producto de la lista para agregar.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var celda = dgvResultadosBusqueda.CurrentRow.Cells["IdProducto"];
            if (celda?.Value == null || !int.TryParse(celda.Value.ToString(), out int idProducto))
            {
                MessageBox.Show("No se pudo determinar el producto seleccionado.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Producto? producto = _logicaProducto.ObtenerPorId(idProducto);
                if (producto == null) return;

                _logicaCarrito.AgregarProducto(producto, 1);
                RefrescarUI();
            }
            catch (ArgumentException ex)
            {
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
        }

        // Refresca las tres cosas que dependen del estado del carrito: la grilla del carrito, el total, y el "stock disponible" que se ve en los
        // resultados de búsqueda. Se llama después de cualquier cambio al carrito
        private void RefrescarUI()
        {
            ActualizarGrillaCarrito();
            ActualizarLabelTotal();
            ActualizarGrillaResultados();
        }

        // Bloquea que se tipee cualquier cosa que no sea un dígito en la celda
        // "Cantidad" del carrito. El control de edición de un DataGridView se reutiliza entre celdas, por eso se saca el handler antes de
        // agregarlo de nuevo (si no, quedaría suscripto una vez por cada celda editada)
        private void dgvCarrito_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvCarrito.CurrentCell?.OwningColumn?.Name != "Cantidad") return;

            if (e.Control is TextBox txtEdicion)
            {
                txtEdicion.KeyPress -= CantidadTextBox_KeyPress;
                txtEdicion.KeyPress += CantidadTextBox_KeyPress;
            }
        }

        private void CantidadTextBox_KeyPress(object? sender, KeyPressEventArgs e)
        {
            // Se permiten dígitos y teclas de control (Backspace, Delete, etc.)
            // Nada de "-", ",", "." ni letras: la cantidad es un entero positivo
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        // El usuario edita la celda "Cantidad" a mano en la grilla del carrito
        private void dgvCarrito_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCarrito.Columns[e.ColumnIndex].Name != "Cantidad") return;

            var filaCarrito = dgvCarrito.Rows[e.RowIndex];
            var celdaId = filaCarrito.Cells["IdProducto"];
            var celdaCantidad = filaCarrito.Cells["Cantidad"];

            if (celdaId?.Value == null || !int.TryParse(celdaId.Value.ToString(), out int idProducto))
                return;

            if (!int.TryParse(celdaCantidad?.Value?.ToString(), out int cantidadNueva))
            {
                MessageBox.Show("Ingrese una cantidad numérica válida.",
                    "Cantidad inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ActualizarGrillaCarrito(); // se descarta la edición inválida
                return;
            }

            try
            {
                _logicaCarrito.ActualizarCantidad(idProducto, cantidadNueva);
                RefrescarUI();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "No se pudo actualizar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ActualizarGrillaCarrito(); // vuelve a mostrar la cantidad anterior
            }
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

        // Confirmar venta (handoff a FrmMetodoPago — US-14, Facundo G.)
        private void btnConfirmarVenta_Click(object sender, EventArgs e)
        {
            if (_logicaCarrito.CantidadItems == 0)
            {
                MessageBox.Show("El carrito está vacío.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Revalidación final contra stock "fresco" de la base: el carrito vive en memoria mientras se arma la venta, así que el stock pudo
            // haber cambiado desde que se agregó cada ítem
            // Si algo ya no alcanza, se avisa y se corta acá
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

            using var frmMetodoPago = new FrmMetodoPago(_logicaCarrito.Items);
            if (frmMetodoPago.ShowDialog(this) == DialogResult.OK)
            {
                _logicaCarrito.Vaciar();
                RefrescarUI();
                txtBuscarProducto.Focus();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _debounceBusqueda.Stop();
            _debounceBusqueda.Dispose();
            base.OnFormClosed(e);
        }
    }

    // Fila de UI para dgvCarrito. Se usa una clase (y no un tipo anónimo) a propósito: los tipos anónimos generan propiedades de solo lectura
    // (sin "set"), y como la columna "Cantidad" es editable, el DataGridView necesita poder escribir el valor nuevo de vuelta en el objeto de
    // origen al confirmar la edición de la celda
    public class FilaCarritoUI
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }

    // Fila de UI para dgvResultadosBusqueda. StockDisponible ya viene calculado (stock real del producto - lo que ya está en el carrito)
    public class FilaResultadoUI
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string? CodigoBarras { get; set; }
        public decimal PrecioConIva { get; set; }
        public int StockDisponible { get; set; }
    }
}
