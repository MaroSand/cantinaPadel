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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panelFiltros = new System.Windows.Forms.Panel();
            txtProductoFiltro = new System.Windows.Forms.TextBox();
            lblProducto = new System.Windows.Forms.Label();
            cmbMarcaFiltro = new System.Windows.Forms.ComboBox();
            lblMarca = new System.Windows.Forms.Label();
            cmbCategoriaFiltro = new System.Windows.Forms.ComboBox();
            lblCategoria = new System.Windows.Forms.Label();
            cmbProveedorFiltro = new System.Windows.Forms.ComboBox();
            lblProveedor = new System.Windows.Forms.Label();
            panelAcciones = new System.Windows.Forms.Panel();
            btnConfirmar = new System.Windows.Forms.Button();
            nudPorcentaje = new System.Windows.Forms.NumericUpDown();
            rbPorcentaje = new System.Windows.Forms.RadioButton();
            rbManual = new System.Windows.Forms.RadioButton();
            nudPrecioManual = new System.Windows.Forms.NumericUpDown();
            btnAplicarPrecioManual = new System.Windows.Forms.Button();
            lblAyudaPrecio = new System.Windows.Forms.Label();
            dgvPreview = new System.Windows.Forms.DataGridView();
            panelFiltros.SuspendLayout();
            panelAcciones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudPorcentaje).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudPrecioManual).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvPreview).BeginInit();
            SuspendLayout();
            // 
            // panelFiltros
            // 
            panelFiltros.BackColor = System.Drawing.Color.Gold;
            panelFiltros.Controls.Add(txtProductoFiltro);
            panelFiltros.Controls.Add(lblProducto);
            panelFiltros.Controls.Add(cmbMarcaFiltro);
            panelFiltros.Controls.Add(lblMarca);
            panelFiltros.Controls.Add(cmbCategoriaFiltro);
            panelFiltros.Controls.Add(lblCategoria);
            panelFiltros.Controls.Add(cmbProveedorFiltro);
            panelFiltros.Controls.Add(lblProveedor);
            panelFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            panelFiltros.Location = new System.Drawing.Point(0, 0);
            panelFiltros.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            panelFiltros.Name = "panelFiltros";
            panelFiltros.Size = new System.Drawing.Size(1723, 113);
            panelFiltros.TabIndex = 0;
            // 
            // txtProductoFiltro
            // 
            txtProductoFiltro.Location = new System.Drawing.Point(33, 56);
            txtProductoFiltro.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            txtProductoFiltro.MaxLength = 100;
            txtProductoFiltro.Name = "txtProductoFiltro";
            txtProductoFiltro.PlaceholderText = "Nombre, código, marca o categoría...";
            txtProductoFiltro.Size = new System.Drawing.Size(517, 39);
            txtProductoFiltro.TabIndex = 1;
            // 
            // lblProducto
            // 
            lblProducto.AutoSize = true;
            lblProducto.Location = new System.Drawing.Point(33, 20);
            lblProducto.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            lblProducto.Name = "lblProducto";
            lblProducto.Size = new System.Drawing.Size(192, 32);
            lblProducto.TabIndex = 0;
            lblProducto.Text = "Buscar producto:";
            // 
            // cmbMarcaFiltro
            // 
            cmbMarcaFiltro.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbMarcaFiltro.FormattingEnabled = true;
            cmbMarcaFiltro.Location = new System.Drawing.Point(958, 55);
            cmbMarcaFiltro.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            cmbMarcaFiltro.Name = "cmbMarcaFiltro";
            cmbMarcaFiltro.Size = new System.Drawing.Size(323, 40);
            cmbMarcaFiltro.TabIndex = 4;
            // 
            // lblMarca
            // 
            lblMarca.AutoSize = true;
            lblMarca.Location = new System.Drawing.Point(958, 20);
            lblMarca.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            lblMarca.Name = "lblMarca";
            lblMarca.Size = new System.Drawing.Size(84, 32);
            lblMarca.TabIndex = 3;
            lblMarca.Text = "Marca:";
            // 
            // cmbCategoriaFiltro
            // 
            cmbCategoriaFiltro.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbCategoriaFiltro.FormattingEnabled = true;
            cmbCategoriaFiltro.Location = new System.Drawing.Point(594, 55);
            cmbCategoriaFiltro.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            cmbCategoriaFiltro.Name = "cmbCategoriaFiltro";
            cmbCategoriaFiltro.Size = new System.Drawing.Size(323, 40);
            cmbCategoriaFiltro.TabIndex = 3;
            // 
            // lblCategoria
            // 
            lblCategoria.AutoSize = true;
            lblCategoria.Location = new System.Drawing.Point(594, 20);
            lblCategoria.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            lblCategoria.Name = "lblCategoria";
            lblCategoria.Size = new System.Drawing.Size(121, 32);
            lblCategoria.TabIndex = 2;
            lblCategoria.Text = "Categoría:";
            // 
            // cmbProveedorFiltro
            // 
            cmbProveedorFiltro.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbProveedorFiltro.FormattingEnabled = true;
            cmbProveedorFiltro.Location = new System.Drawing.Point(1322, 55);
            cmbProveedorFiltro.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            cmbProveedorFiltro.Name = "cmbProveedorFiltro";
            cmbProveedorFiltro.Size = new System.Drawing.Size(323, 40);
            cmbProveedorFiltro.TabIndex = 5;
            // 
            // lblProveedor
            // 
            lblProveedor.AutoSize = true;
            lblProveedor.Location = new System.Drawing.Point(1322, 20);
            lblProveedor.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            lblProveedor.Name = "lblProveedor";
            lblProveedor.Size = new System.Drawing.Size(128, 32);
            lblProveedor.TabIndex = 5;
            lblProveedor.Text = "Proveedor:";
            // 
            // panelAcciones
            // 
            panelAcciones.BackColor = System.Drawing.SystemColors.Info;
            panelAcciones.Controls.Add(btnConfirmar);
            panelAcciones.Controls.Add(nudPorcentaje);
            panelAcciones.Controls.Add(rbPorcentaje);
            panelAcciones.Controls.Add(rbManual);
            panelAcciones.Controls.Add(nudPrecioManual);
            panelAcciones.Controls.Add(btnAplicarPrecioManual);
            panelAcciones.Controls.Add(lblAyudaPrecio);
            panelAcciones.Dock = System.Windows.Forms.DockStyle.Bottom;
            panelAcciones.Location = new System.Drawing.Point(0, 657);
            panelAcciones.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            panelAcciones.Name = "panelAcciones";
            panelAcciones.Size = new System.Drawing.Size(1723, 111);
            panelAcciones.TabIndex = 1;
            // 
            // btnConfirmar
            // 
            btnConfirmar.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
            btnConfirmar.BackColor = System.Drawing.Color.White;
            btnConfirmar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnConfirmar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)0));
            btnConfirmar.Location = new System.Drawing.Point(1441, 30);
            btnConfirmar.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            btnConfirmar.Name = "btnConfirmar";
            btnConfirmar.Size = new System.Drawing.Size(241, 47);
            btnConfirmar.TabIndex = 6;
            btnConfirmar.Text = "Confirmar";
            btnConfirmar.UseVisualStyleBackColor = false;
            // 
            // nudPorcentaje
            // 
            nudPorcentaje.DecimalPlaces = 2;
            nudPorcentaje.Location = new System.Drawing.Point(220, 15);
            nudPorcentaje.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            nudPorcentaje.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            nudPorcentaje.Name = "nudPorcentaje";
            nudPorcentaje.Size = new System.Drawing.Size(140, 39);
            nudPorcentaje.TabIndex = 0;
            // 
            // rbPorcentaje
            // 
            rbPorcentaje.AutoSize = true;
            rbPorcentaje.Checked = true;
            rbPorcentaje.Location = new System.Drawing.Point(33, 18);
            rbPorcentaje.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            rbPorcentaje.Name = "rbPorcentaje";
            rbPorcentaje.Size = new System.Drawing.Size(202, 36);
            rbPorcentaje.TabIndex = 0;
            rbPorcentaje.TabStop = true;
            rbPorcentaje.Text = "Porcentaje (%):";
            rbPorcentaje.UseVisualStyleBackColor = true;
            // 
            // rbManual
            // 
            rbManual.AutoSize = true;
            rbManual.Location = new System.Drawing.Point(400, 18);
            rbManual.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            rbManual.Name = "rbManual";
            rbManual.Size = new System.Drawing.Size(235, 36);
            rbManual.TabIndex = 3;
            rbManual.Text = "Precio manual ($):";
            rbManual.UseVisualStyleBackColor = true;
            // 
            // nudPrecioManual
            // 
            nudPrecioManual.DecimalPlaces = 2;
            nudPrecioManual.Location = new System.Drawing.Point(620, 15);
            nudPrecioManual.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            nudPrecioManual.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            nudPrecioManual.Name = "nudPrecioManual";
            nudPrecioManual.Size = new System.Drawing.Size(160, 39);
            nudPrecioManual.TabIndex = 4;
            // 
            // btnAplicarPrecioManual
            // 
            btnAplicarPrecioManual.BackColor = System.Drawing.Color.White;
            btnAplicarPrecioManual.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnAplicarPrecioManual.Location = new System.Drawing.Point(800, 10);
            btnAplicarPrecioManual.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            btnAplicarPrecioManual.Name = "btnAplicarPrecioManual";
            btnAplicarPrecioManual.Size = new System.Drawing.Size(240, 47);
            btnAplicarPrecioManual.TabIndex = 5;
            btnAplicarPrecioManual.Text = "Aplicar a tildados";
            btnAplicarPrecioManual.UseVisualStyleBackColor = false;
            // 
            // lblAyudaPrecio
            // 
            lblAyudaPrecio.AutoSize = true;
            lblAyudaPrecio.ForeColor = System.Drawing.SystemColors.GrayText;
            lblAyudaPrecio.Location = new System.Drawing.Point(33, 68);
            lblAyudaPrecio.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            lblAyudaPrecio.Name = "lblAyudaPrecio";
            lblAyudaPrecio.Size = new System.Drawing.Size(992, 32);
            lblAyudaPrecio.TabIndex = 2;
            lblAyudaPrecio.Text = ("Tip: tildá los productos en \"Aplicar\" y usá \"Precio manual\" para asignarles un pr" + "ecio puntual.");
            // 
            // dgvPreview
            // 
            dgvPreview.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dgvPreview.BackgroundColor = System.Drawing.Color.White;
            dgvPreview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvPreview.GridColor = System.Drawing.SystemColors.Menu;
            dgvPreview.Location = new System.Drawing.Point(0, 113);
            dgvPreview.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            dgvPreview.Name = "dgvPreview";
            dgvPreview.RowHeadersWidth = 51;
            dgvPreview.Size = new System.Drawing.Size(1723, 544);
            dgvPreview.TabIndex = 2;
            // 
            // FrmActualizacionPrecios
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1723, 768);
            Controls.Add(dgvPreview);
            Controls.Add(panelAcciones);
            Controls.Add(panelFiltros);
            Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
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
        private System.Windows.Forms.DataGridView dgvPreview;
    }
}