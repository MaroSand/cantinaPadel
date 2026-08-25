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
            cmbProveedorFiltro = new ComboBox();
            lblProveedor = new Label();
            panelAcciones = new Panel();
            btnConfirmar = new Button();
            nudPorcentaje = new NumericUpDown();
            rbPorcentaje = new RadioButton();
            rbManual = new RadioButton();
            nudPrecioManual = new NumericUpDown();
            btnAplicarPrecioManual = new Button();
            lblAyudaPrecio = new Label();
            dgvPreview = new DataGridView();
            panelFiltros.SuspendLayout();
            panelAcciones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudPorcentaje).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudPrecioManual).BeginInit();
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
            panelFiltros.Controls.Add(cmbProveedorFiltro);
            panelFiltros.Controls.Add(lblProveedor);
            panelFiltros.Dock = DockStyle.Top;
            panelFiltros.Location = new Point(0, 0);
            panelFiltros.Margin = new Padding(6, 4, 6, 4);
            panelFiltros.Name = "panelFiltros";
            panelFiltros.Size = new Size(1723, 113);
            panelFiltros.TabIndex = 0;
            // 
            // txtProductoFiltro
            // 
            txtProductoFiltro.Location = new Point(33, 56);
            txtProductoFiltro.Margin = new Padding(6, 4, 6, 4);
            txtProductoFiltro.MaxLength = 100;
            txtProductoFiltro.Name = "txtProductoFiltro";
            txtProductoFiltro.PlaceholderText = "Nombre, código, marca o categoría...";
            txtProductoFiltro.Size = new Size(517, 39);
            txtProductoFiltro.TabIndex = 1;
            // 
            // lblProducto
            // 
            lblProducto.AutoSize = true;
            lblProducto.Location = new Point(33, 20);
            lblProducto.Margin = new Padding(6, 0, 6, 0);
            lblProducto.Name = "lblProducto";
            lblProducto.Size = new Size(192, 32);
            lblProducto.TabIndex = 0;
            lblProducto.Text = "Buscar producto:";
            // 
            // cmbMarcaFiltro
            // 
            cmbMarcaFiltro.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMarcaFiltro.FormattingEnabled = true;
            cmbMarcaFiltro.Location = new Point(958, 55);
            cmbMarcaFiltro.Margin = new Padding(6, 4, 6, 4);
            cmbMarcaFiltro.Name = "cmbMarcaFiltro";
            cmbMarcaFiltro.Size = new Size(323, 40);
            cmbMarcaFiltro.TabIndex = 4;
            // 
            // lblMarca
            // 
            lblMarca.AutoSize = true;
            lblMarca.Location = new Point(958, 20);
            lblMarca.Margin = new Padding(6, 0, 6, 0);
            lblMarca.Name = "lblMarca";
            lblMarca.Size = new Size(84, 32);
            lblMarca.TabIndex = 3;
            lblMarca.Text = "Marca:";
            // 
            // cmbCategoriaFiltro
            // 
            cmbCategoriaFiltro.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategoriaFiltro.FormattingEnabled = true;
            cmbCategoriaFiltro.Location = new Point(594, 55);
            cmbCategoriaFiltro.Margin = new Padding(6, 4, 6, 4);
            cmbCategoriaFiltro.Name = "cmbCategoriaFiltro";
            cmbCategoriaFiltro.Size = new Size(323, 40);
            cmbCategoriaFiltro.TabIndex = 3;
            // 
            // lblCategoria
            // 
            lblCategoria.AutoSize = true;
            lblCategoria.Location = new Point(594, 20);
            lblCategoria.Margin = new Padding(6, 0, 6, 0);
            lblCategoria.Name = "lblCategoria";
            lblCategoria.Size = new Size(121, 32);
            lblCategoria.TabIndex = 2;
            lblCategoria.Text = "Categoría:";
            // 
            // cmbProveedorFiltro
            // 
            cmbProveedorFiltro.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbProveedorFiltro.FormattingEnabled = true;
            cmbProveedorFiltro.Location = new Point(1322, 55);
            cmbProveedorFiltro.Margin = new Padding(6, 4, 6, 4);
            cmbProveedorFiltro.Name = "cmbProveedorFiltro";
            cmbProveedorFiltro.Size = new Size(323, 40);
            cmbProveedorFiltro.TabIndex = 5;
            // 
            // lblProveedor
            // 
            lblProveedor.AutoSize = true;
            lblProveedor.Location = new Point(1322, 20);
            lblProveedor.Margin = new Padding(6, 0, 6, 0);
            lblProveedor.Name = "lblProveedor";
            lblProveedor.Size = new Size(130, 32);
            lblProveedor.TabIndex = 5;
            lblProveedor.Text = "Proveedor:";
            // 
            // panelAcciones
            // 
            panelAcciones.BackColor = SystemColors.Info;
            panelAcciones.Controls.Add(btnConfirmar);
            panelAcciones.Controls.Add(nudPorcentaje);
            panelAcciones.Controls.Add(rbPorcentaje);
            panelAcciones.Controls.Add(rbManual);
            panelAcciones.Controls.Add(nudPrecioManual);
            panelAcciones.Controls.Add(btnAplicarPrecioManual);
            panelAcciones.Controls.Add(lblAyudaPrecio);
            panelAcciones.Dock = DockStyle.Bottom;
            panelAcciones.Location = new Point(0, 657);
            panelAcciones.Margin = new Padding(6, 4, 6, 4);
            panelAcciones.Name = "panelAcciones";
            panelAcciones.Size = new Size(1723, 111);
            panelAcciones.TabIndex = 1;
            // 
            // btnConfirmar
            // 
            btnConfirmar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnConfirmar.BackColor = Color.White;
            btnConfirmar.FlatStyle = FlatStyle.Flat;
            btnConfirmar.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnConfirmar.Location = new Point(1441, 30);
            btnConfirmar.Margin = new Padding(6, 4, 6, 4);
            btnConfirmar.Name = "btnConfirmar";
            btnConfirmar.Size = new Size(241, 47);
            btnConfirmar.TabIndex = 6;
            btnConfirmar.Text = "Confirmar";
            btnConfirmar.UseVisualStyleBackColor = false;
            // 
            // nudPorcentaje
            // 
            nudPorcentaje.DecimalPlaces = 2;
            nudPorcentaje.Location = new Point(220, 15);
            nudPorcentaje.Margin = new Padding(6, 4, 6, 4);
            nudPorcentaje.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            nudPorcentaje.Name = "nudPorcentaje";
            nudPorcentaje.Size = new Size(140, 39);
            nudPorcentaje.TabIndex = 0;
            // 
            // rbPorcentaje
            // 
            rbPorcentaje.AutoSize = true;
            rbPorcentaje.Checked = true;
            rbPorcentaje.Location = new Point(33, 18);
            rbPorcentaje.Margin = new Padding(6, 0, 6, 0);
            rbPorcentaje.Name = "rbPorcentaje";
            rbPorcentaje.Size = new Size(180, 36);
            rbPorcentaje.TabIndex = 0;
            rbPorcentaje.TabStop = true;
            rbPorcentaje.Text = "Porcentaje (%):";
            rbPorcentaje.UseVisualStyleBackColor = true;
            // 
            // rbManual
            // 
            rbManual.AutoSize = true;
            rbManual.Location = new Point(400, 18);
            rbManual.Margin = new Padding(6, 0, 6, 0);
            rbManual.Name = "rbManual";
            rbManual.Size = new Size(190, 36);
            rbManual.TabIndex = 3;
            rbManual.Text = "Precio manual ($):";
            rbManual.UseVisualStyleBackColor = true;
            // 
            // nudPrecioManual
            // 
            nudPrecioManual.DecimalPlaces = 2;
            nudPrecioManual.Location = new Point(620, 15);
            nudPrecioManual.Margin = new Padding(6, 4, 6, 4);
            nudPrecioManual.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            nudPrecioManual.Name = "nudPrecioManual";
            nudPrecioManual.Size = new Size(160, 39);
            nudPrecioManual.TabIndex = 4;
            // 
            // btnAplicarPrecioManual
            // 
            btnAplicarPrecioManual.BackColor = Color.White;
            btnAplicarPrecioManual.FlatStyle = FlatStyle.Flat;
            btnAplicarPrecioManual.Location = new Point(800, 10);
            btnAplicarPrecioManual.Margin = new Padding(6, 4, 6, 4);
            btnAplicarPrecioManual.Name = "btnAplicarPrecioManual";
            btnAplicarPrecioManual.Size = new Size(240, 47);
            btnAplicarPrecioManual.TabIndex = 5;
            btnAplicarPrecioManual.Text = "Aplicar a tildados";
            btnAplicarPrecioManual.UseVisualStyleBackColor = false;
            // 
            // lblAyudaPrecio
            // 
            lblAyudaPrecio.AutoSize = true;
            lblAyudaPrecio.ForeColor = SystemColors.GrayText;
            lblAyudaPrecio.Location = new Point(33, 68);
            lblAyudaPrecio.Margin = new Padding(6, 0, 6, 0);
            lblAyudaPrecio.Name = "lblAyudaPrecio";
            lblAyudaPrecio.Size = new Size(590, 32);
            lblAyudaPrecio.TabIndex = 2;
            lblAyudaPrecio.Text = "Tip: tildá los productos en \"Aplicar\" y usá \"Precio manual\" para asignarles un precio puntual.";
            // 
            // dgvPreview
            // 
            dgvPreview.BackgroundColor = Color.White;
            dgvPreview.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPreview.Dock = DockStyle.Fill;
            dgvPreview.GridColor = SystemColors.Menu;
            dgvPreview.Location = new Point(0, 113);
            dgvPreview.Margin = new Padding(6, 4, 6, 4);
            dgvPreview.Name = "dgvPreview";
            dgvPreview.RowHeadersWidth = 51;
            dgvPreview.Size = new Size(1723, 544);
            dgvPreview.TabIndex = 2;
            // 
            // FrmActualizacionPrecios
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1723, 768);
            Controls.Add(dgvPreview);
            Controls.Add(panelAcciones);
            Controls.Add(panelFiltros);
            Margin = new Padding(6, 4, 6, 4);
            Name = "FrmActualizacionPrecios";
            Text = "Actualización de Precios";
            Load += FrmActualizacionPrecios_Load;
            panelFiltros.ResumeLayout(false);
            panelFiltros.PerformLayout();
            panelAcciones.ResumeLayout(false);
            panelAcciones.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudPorcentaje).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudPrecioManual).EndInit();
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
        private Label lblProveedor;
        private ComboBox cmbProveedorFiltro;
        private Panel panelAcciones;
        private RadioButton rbPorcentaje;
        private NumericUpDown nudPorcentaje;
        private RadioButton rbManual;
        private NumericUpDown nudPrecioManual;
        private Button btnAplicarPrecioManual;
        private Label lblAyudaPrecio;
        private Button btnConfirmar;
        private DataGridView dgvPreview;
    }
}