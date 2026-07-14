namespace cantinaPadel.UI
{
    partial class FrmActualizacionPrecios
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            panelFiltros = new Panel();
            txtProductoFiltro = new TextBox();
            lblProducto = new Label();
            cmbMarcaFiltro = new ComboBox();
            lblMarca = new Label();
            cmbCategoriaFiltro = new ComboBox();
            lblCategoria = new Label();
            panelAcciones = new Panel();
            btnConfirmar = new Button();
            nudPorcentaje = new NumericUpDown();
            lblPorcentaje = new Label();
            lblAyudaPrecio = new Label();
            dgvPreview = new DataGridView();
            panelFiltros.SuspendLayout();
            panelAcciones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudPorcentaje).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvPreview).BeginInit();
            SuspendLayout();
            // 
            // panelFiltros
            // 
            panelFiltros.BackColor = Color.Gold;
            panelFiltros.Controls.Add(txtProductoFiltro);
            panelFiltros.Controls.Add(lblProducto);
            panelFiltros.Controls.Add(cmbMarcaFiltro);
            panelFiltros.Controls.Add(lblMarca);
            panelFiltros.Controls.Add(cmbCategoriaFiltro);
            panelFiltros.Controls.Add(lblCategoria);
            panelFiltros.Dock = DockStyle.Top;
            panelFiltros.Location = new Point(0, 0);
            panelFiltros.Margin = new Padding(3, 2, 3, 2);
            panelFiltros.Name = "panelFiltros";
            panelFiltros.Size = new Size(928, 45);
            panelFiltros.TabIndex = 0;
            // 
            // txtProductoFiltro
            // 
            // Búsqueda libre: nombre, código, marca o categoría. Se dispara sola
            // (con un pequeño debounce) al tipear, sin botón.
            txtProductoFiltro.Location = new Point(18, 12);
            txtProductoFiltro.Margin = new Padding(3, 2, 3, 2);
            txtProductoFiltro.Name = "txtProductoFiltro";
            txtProductoFiltro.Size = new Size(280, 23);
            txtProductoFiltro.TabIndex = 1;
            txtProductoFiltro.PlaceholderText = "Nombre, código, marca o categoría...";
            // 
            // lblProducto
            // 
            lblProducto.AutoSize = true;
            lblProducto.Location = new Point(18, -3);
            lblProducto.Name = "lblProducto";
            lblProducto.Size = new Size(102, 15);
            lblProducto.TabIndex = 0;
            lblProducto.Text = "Buscar producto:";
            // 
            // cmbCategoriaFiltro
            // 
            // Combina en simultáneo con el texto y con Marca (AND). Incluye
            // una opción "Todas las categorías" (Id null) para no filtrar.
            cmbCategoriaFiltro.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategoriaFiltro.FormattingEnabled = true;
            cmbCategoriaFiltro.Location = new Point(320, 12);
            cmbCategoriaFiltro.Margin = new Padding(3, 2, 3, 2);
            cmbCategoriaFiltro.Name = "cmbCategoriaFiltro";
            cmbCategoriaFiltro.Size = new Size(176, 23);
            cmbCategoriaFiltro.TabIndex = 3;
            // 
            // lblCategoria
            // 
            lblCategoria.AutoSize = true;
            lblCategoria.Location = new Point(320, -3);
            lblCategoria.Name = "lblCategoria";
            lblCategoria.Size = new Size(61, 15);
            lblCategoria.TabIndex = 2;
            lblCategoria.Text = "Categoría:";
            // 
            // cmbMarcaFiltro
            // 
            cmbMarcaFiltro.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMarcaFiltro.FormattingEnabled = true;
            cmbMarcaFiltro.Location = new Point(516, 12);
            cmbMarcaFiltro.Margin = new Padding(3, 2, 3, 2);
            cmbMarcaFiltro.Name = "cmbMarcaFiltro";
            cmbMarcaFiltro.Size = new Size(176, 23);
            cmbMarcaFiltro.TabIndex = 4;
            // 
            // lblMarca
            // 
            lblMarca.AutoSize = true;
            lblMarca.Location = new Point(516, -3);
            lblMarca.Name = "lblMarca";
            lblMarca.Size = new Size(43, 15);
            lblMarca.TabIndex = 3;
            lblMarca.Text = "Marca:";
            // 
            // panelAcciones
            // 
            panelAcciones.Controls.Add(btnConfirmar);
            panelAcciones.Controls.Add(nudPorcentaje);
            panelAcciones.Controls.Add(lblPorcentaje);
            panelAcciones.Controls.Add(lblAyudaPrecio);
            panelAcciones.Dock = DockStyle.Bottom;
            panelAcciones.Location = new Point(0, 322);
            panelAcciones.Margin = new Padding(3, 2, 3, 2);
            panelAcciones.Name = "panelAcciones";
            panelAcciones.Size = new Size(928, 38);
            panelAcciones.TabIndex = 1;
            // 
            // btnConfirmar
            // 
            btnConfirmar.FlatStyle = FlatStyle.Flat;
            btnConfirmar.Location = new Point(780, 7);
            btnConfirmar.Margin = new Padding(3, 2, 3, 2);
            btnConfirmar.Name = "btnConfirmar";
            btnConfirmar.Size = new Size(130, 22);
            btnConfirmar.TabIndex = 2;
            btnConfirmar.Text = "Confirmar";
            btnConfirmar.UseVisualStyleBackColor = true;
            // 
            // nudPorcentaje
            // 
            // Recalcula en vivo (ValueChanged) el "Precio Nuevo" de todas
            // las filas que no fueron editadas a mano en la grilla.
            nudPorcentaje.DecimalPlaces = 2;
            nudPorcentaje.Location = new Point(149, 10);
            nudPorcentaje.Margin = new Padding(3, 2, 3, 2);
            nudPorcentaje.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            nudPorcentaje.Minimum = new decimal(new int[] { 90, 0, 0, int.MinValue });
            nudPorcentaje.Name = "nudPorcentaje";
            nudPorcentaje.Size = new Size(88, 23);
            nudPorcentaje.TabIndex = 0;
            // 
            // lblPorcentaje
            // 
            lblPorcentaje.AutoSize = true;
            lblPorcentaje.Location = new Point(18, 11);
            lblPorcentaje.Name = "lblPorcentaje";
            lblPorcentaje.Size = new Size(87, 15);
            lblPorcentaje.TabIndex = 0;
            lblPorcentaje.Text = "Porcentaje (%):";
            // 
            // lblAyudaPrecio
            // 
            // Aviso de que "Precio Nuevo" también se puede tipear a mano.
            lblAyudaPrecio.AutoSize = true;
            lblAyudaPrecio.ForeColor = SystemColors.GrayText;
            lblAyudaPrecio.Location = new Point(262, 11);
            lblAyudaPrecio.Name = "lblAyudaPrecio";
            lblAyudaPrecio.Size = new Size(330, 15);
            lblAyudaPrecio.TabIndex = 1;
            lblAyudaPrecio.Text = "Tip: podés tipear el \"Precio Nuevo\" directo en la grilla.";
            // 
            // dgvPreview
            // 
            dgvPreview.BackgroundColor = SystemColors.Info;
            dgvPreview.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPreview.Dock = DockStyle.Fill;
            dgvPreview.GridColor = SystemColors.Menu;
            dgvPreview.Location = new Point(0, 45);
            dgvPreview.Margin = new Padding(3, 2, 3, 2);
            dgvPreview.Name = "dgvPreview";
            dgvPreview.ReadOnly = false;
            dgvPreview.RowHeadersWidth = 51;
            dgvPreview.Size = new Size(928, 277);
            dgvPreview.TabIndex = 2;
            // 
            // FrmActualizacionPrecios
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(928, 360);
            Controls.Add(dgvPreview);
            Controls.Add(panelAcciones);
            Controls.Add(panelFiltros);
            Margin = new Padding(3, 2, 3, 2);
            Name = "FrmActualizacionPrecios";
            Text = "Actualización Masiva de Precios";
            Load += FrmActualizacionPrecios_Load;
            panelFiltros.ResumeLayout(false);
            panelFiltros.PerformLayout();
            panelAcciones.ResumeLayout(false);
            panelAcciones.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudPorcentaje).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvPreview).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelFiltros;
        private Label lblCategoria;
        private ComboBox cmbCategoriaFiltro;
        private Label lblMarca;
        private ComboBox cmbMarcaFiltro;
        private Label lblProducto;
        private TextBox txtProductoFiltro;
        private Panel panelAcciones;
        private Label lblPorcentaje;
        private NumericUpDown nudPorcentaje;
        private Label lblAyudaPrecio;
        private Button btnConfirmar;
        private DataGridView dgvPreview;
    }
}