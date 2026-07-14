using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.UI
{
    public partial class FrmCRUDProducto : Form
    {
        private readonly LogicaProducto _logicaProducto;
        private Producto? _productoEdicion;

        // Constructor para alta de nuevo producto
        public FrmCRUDProducto()
        {
            InitializeComponent();
            _logicaProducto = new LogicaProducto();
            _productoEdicion = null;
            this.Text = "Nuevo Producto";
        }

        // Constructor para modificar un producto existente
        public FrmCRUDProducto(Producto producto) : this()
        {
            _productoEdicion = producto;
            this.Text = "Modificar Producto";
        }

        private void FrmCRUDProducto_Load(object sender, EventArgs e)
        {
            // Se suscriben los eventos de controles
            btnGuardar.Click += btnGuardar_Click;
            btnCancelar.Click += btnCancelar_Click;
            txtCodigoBarras.KeyDown += txtCodigoBarras_KeyDown;
            txtPrecioCosto.KeyPress += TxtDecimal_KeyPress;
            txtPrecioVenta.KeyPress += TxtDecimal_KeyPress;
            txtPrecioVenta.TextChanged += txtPrecioVenta_TextChanged;
            txtStockActual.KeyPress += TxtEntero_KeyPress;
            txtStockMinimo.KeyPress += TxtEntero_KeyPress;

            CargarCombos();

            if (_productoEdicion != null)
            {
                CargarDatosEnFormulario();
            }
            else
            {
                // Foco directo en el código de barras para poder arrancar el alta escaneando
                txtCodigoBarras.Focus();
            }

            ActualizarPrecioConIva();
        }

        // Carga los combos de Categoría y Marca. Marca
        // es opcional, por eso lleva una opción "(Ninguna/o)" con valor null.
        private void CargarCombos()
        {
            try
            {
                cmbCategoria.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbCategoria.DisplayMember = "Nombre";
                cmbCategoria.ValueMember = "IdCategoria";
                cmbCategoria.DataSource = _logicaProducto.ObtenerCategoriasActivas();
                cmbCategoria.SelectedIndex = -1;

                var marcas = _logicaProducto.ObtenerMarcasActivas()
                    .Select(m => new { Id = (int?)m.IdMarca, Nombre = m.Nombre })
                    .ToList();
                marcas.Insert(0, new { Id = (int?)null, Nombre = "(Sin marca)" });

                cmbMarca.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbMarca.DisplayMember = "Nombre";
                cmbMarca.ValueMember = "Id";
                cmbMarca.DataSource = marcas;
                cmbMarca.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar con la base de datos: {ex.Message}",
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Rellena los campos cuando es una modificación
        private void CargarDatosEnFormulario()
        {
            // Evitar pasar null directamente a Text o a SelectedValue porque puede
            // provocar excepciones internas de WinForms (CurrencyManager.Find con key null)
            txtNombre.Text = _productoEdicion!.Nombre ?? string.Empty;
            txtCodigoBarras.Text = _productoEdicion.CodigoBarras ?? string.Empty;
            txtPrecioCosto.Text = _productoEdicion.PrecioCosto.ToString("0.00", CultureInfo.CurrentCulture);
            txtPrecioVenta.Text = _productoEdicion.PrecioVenta.ToString("0.00", CultureInfo.CurrentCulture);
            txtStockActual.Text = _productoEdicion.StockActual.ToString();
            txtStockMinimo.Text = _productoEdicion.StockMinimo.ToString();

            // Seleccionamos la categoría solo si el valor es válido (no 0). Si no existe,
            // dejamos SelectedIndex en -1 para que no dispare CurrencyManager.Find con key null.
            if (_productoEdicion.IdCategoria != 0)
            {
                try
                {
                    cmbCategoria.SelectedValue = _productoEdicion.IdCategoria;
                }
                catch
                {
                    cmbCategoria.SelectedIndex = -1;
                }
            }
            else
            {
                cmbCategoria.SelectedIndex = -1;
            }

            // Para la marca, IdMarca es nullable; asignamos SelectedValue solo si tiene valor.
            if (_productoEdicion.IdMarca.HasValue)
            {
                try
                {
                    cmbMarca.SelectedValue = _productoEdicion.IdMarca.Value;
                }
                catch
                {
                    // Si por alguna razón el valor no se encuentra en el DataSource,
                    // seleccionamos la primera opción ("(Sin marca)") que insertamos en carga.
                    cmbMarca.SelectedIndex = 0;
                }
            }
            else
            {
                cmbMarca.SelectedIndex = 0;
            }
        }

        // Recalcula en vivo el precio final con IVA a medida que se tipea el precio de venta
        private void txtPrecioVenta_TextChanged(object sender, EventArgs e) => ActualizarPrecioConIva();

        private void ActualizarPrecioConIva()
        {
            if (decimal.TryParse(txtPrecioVenta.Text, NumberStyles.Number, CultureInfo.CurrentCulture,
                    out decimal precioVenta))
            {
                decimal precioConIva = _logicaProducto.CalcularPrecioConIva(precioVenta);
                lblPrecioConIvaValor.Text = precioConIva.ToString("C2", CultureInfo.CurrentCulture);
            }
            else
            {
                lblPrecioConIvaValor.Text = "—";
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            decimal.TryParse(txtPrecioCosto.Text, NumberStyles.Number, CultureInfo.CurrentCulture,
                out decimal precioCosto);
            decimal.TryParse(txtPrecioVenta.Text, NumberStyles.Number, CultureInfo.CurrentCulture,
                out decimal precioVenta);
            int.TryParse(txtStockActual.Text, out int stockActual);
            int.TryParse(txtStockMinimo.Text, out int stockMinimo);

            // Arma el objeto con los datos del formulario; la validación de negocio
            // (obligatoriedad, unicidad del código de barras, etc.) queda en la BLL
            var producto = new Producto
            {
                Nombre = txtNombre.Text.Trim(),
                CodigoBarras = string.IsNullOrWhiteSpace(txtCodigoBarras.Text) ? null : txtCodigoBarras.Text.Trim(),
                IdCategoria = cmbCategoria.SelectedValue is int idCategoria ? idCategoria : 0,
                IdMarca = cmbMarca.SelectedValue as int?,
                PrecioCosto = precioCosto,
                PrecioVenta = precioVenta,
                StockActual = stockActual,
                StockMinimo = stockMinimo
            };

            try
            {
                (bool ok, string error) resultado;

                if (_productoEdicion == null)
                {
                    // Alta
                    resultado = _logicaProducto.Agregar(producto);
                }
                else
                {
                    // Modificación: conserva el ID y el estado original
                    producto.IdProducto = _productoEdicion.IdProducto;
                    producto.Activo = _productoEdicion.Activo;

                    resultado = _logicaProducto.Modificar(producto);
                }

                if (!resultado.ok)
                {
                    MessageBox.Show(resultado.error, "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show("Producto guardado correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // Soporte para lector de código de barras (HID): el lector "tipea" el código
        // y termina enviando Enter. Se anula ese Enter (no debe disparar Guardar) y
        // se avanza directamente al siguiente campo, como si se hubiera tipeado a mano.
        private void txtCodigoBarras_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            e.SuppressKeyPress = true;
            SelectNextControl(txtCodigoBarras, true, true, true, true);
        }

        // Solo permite dígitos, control (borrar) y el separador decimal de la cultura actual
        private void TxtDecimal_KeyPress(object sender, KeyPressEventArgs e)
        {
            string separador = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar.ToString() != separador)
            {
                e.Handled = true;
            }
        }

        // Solo permite dígitos y control (borrar), para Stock Actual / Stock Mínimo
        private void TxtEntero_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}