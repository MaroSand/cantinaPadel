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
            cmbProductoFiltro = new ComboBox();
            lblProducto = new Label();
            cmbMarcaFiltro = new ComboBox();
            lblMarca = new Label();
            cmbCategoriaFiltro = new ComboBox();
            lblCategoria = new Label();
            cmbCriterio = new ComboBox();
            lblCriterio = new Label();
            panelAcciones = new Panel();
            btnConfirmar = new Button();
            btnPrevisualizar = new Button();
            nudPorcentaje = new NumericUpDown();
            lblPorcentaje = new Label();
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
            panelFiltros.Controls.Add(cmbProductoFiltro);
            panelFiltros.Controls.Add(lblProducto);
            panelFiltros.Controls.Add(cmbMarcaFiltro);
            panelFiltros.Controls.Add(lblMarca);
            panelFiltros.Controls.Add(cmbCategoriaFiltro);
            panelFiltros.Controls.Add(lblCategoria);
            panelFiltros.Controls.Add(cmbCriterio);
            panelFiltros.Controls.Add(lblCriterio);
            panelFiltros.Dock = DockStyle.Top;
            panelFiltros.Location = new Point(0, 0);
            panelFiltros.Margin = new Padding(3, 2, 3, 2);
            panelFiltros.Name = "panelFiltros";
            panelFiltros.Size = new Size(928, 45);
            panelFiltros.TabIndex = 0;
            // 
            // cmbProductoFiltro
            // 
            cmbProductoFiltro.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbProductoFiltro.FormattingEnabled = true;
            cmbProductoFiltro.Location = new Point(315, 12);
            cmbProductoFiltro.Margin = new Padding(3, 2, 3, 2);
            cmbProductoFiltro.Name = "cmbProductoFiltro";
            cmbProductoFiltro.Size = new Size(228, 23);
            cmbProductoFiltro.TabIndex = 3;
            // 
            // lblProducto
            // 
            lblProducto.AutoSize = true;
            lblProducto.Location = new Point(315, -3);
            lblProducto.Name = "lblProducto";
            lblProducto.Size = new Size(59, 15);
            lblProducto.TabIndex = 2;
            lblProducto.Text = "Producto:";
            // 
            // cmbMarcaFiltro
            // 
            cmbMarcaFiltro.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMarcaFiltro.FormattingEnabled = true;
            cmbMarcaFiltro.Location = new Point(315, 12);
            cmbMarcaFiltro.Margin = new Padding(3, 2, 3, 2);
            cmbMarcaFiltro.Name = "cmbMarcaFiltro";
            cmbMarcaFiltro.Size = new Size(176, 23);
            cmbMarcaFiltro.TabIndex = 3;
            // 
            // lblMarca
            // 
            lblMarca.AutoSize = true;
            lblMarca.Location = new Point(315, -3);
            lblMarca.Name = "lblMarca";
            lblMarca.Size = new Size(43, 15);
            lblMarca.TabIndex = 2;
            lblMarca.Text = "Marca:";
            // 
            // cmbCategoriaFiltro
            // 
            cmbCategoriaFiltro.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategoriaFiltro.FormattingEnabled = true;
            cmbCategoriaFiltro.Location = new Point(315, 12);
            cmbCategoriaFiltro.Margin = new Padding(3, 2, 3, 2);
            cmbCategoriaFiltro.Name = "cmbCategoriaFiltro";
            cmbCategoriaFiltro.Size = new Size(176, 23);
            cmbCategoriaFiltro.TabIndex = 3;
            // 
            // lblCategoria
            // 
            lblCategoria.AutoSize = true;
            lblCategoria.Location = new Point(315, -3);
            lblCategoria.Name = "lblCategoria";
            lblCategoria.Size = new Size(61, 15);
            lblCategoria.TabIndex = 2;
            lblCategoria.Text = "Categoría:";
            // 
            // cmbCriterio
            // 
            cmbCriterio.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCriterio.FormattingEnabled = true;
            cmbCriterio.Location = new Point(131, 12);
            cmbCriterio.Margin = new Padding(3, 2, 3, 2);
            cmbCriterio.Name = "cmbCriterio";
            cmbCriterio.Size = new Size(158, 23);
            cmbCriterio.TabIndex = 1;
            // 
            // lblCriterio
            // 
            lblCriterio.AutoSize = true;
            lblCriterio.Location = new Point(18, 14);
            lblCriterio.Name = "lblCriterio";
            lblCriterio.Size = new Size(49, 15);
            lblCriterio.TabIndex = 0;
            lblCriterio.Text = "Criterio:";
            // 
            // panelAcciones
            // 
            panelAcciones.Controls.Add(btnConfirmar);
            panelAcciones.Controls.Add(btnPrevisualizar);
            panelAcciones.Controls.Add(nudPorcentaje);
            panelAcciones.Controls.Add(lblPorcentaje);
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
            btnConfirmar.Location = new Point(411, 7);
            btnConfirmar.Margin = new Padding(3, 2, 3, 2);
            btnConfirmar.Name = "btnConfirmar";
            btnConfirmar.Size = new Size(158, 22);
            btnConfirmar.TabIndex = 3;
            btnConfirmar.Text = "Confirmar Actualización";
            btnConfirmar.UseVisualStyleBackColor = true;
            // 
            // btnPrevisualizar
            // 
            btnPrevisualizar.FlatStyle = FlatStyle.Flat;
            btnPrevisualizar.Location = new Point(262, 7);
            btnPrevisualizar.Margin = new Padding(3, 2, 3, 2);
            btnPrevisualizar.Name = "btnPrevisualizar";
            btnPrevisualizar.Size = new Size(131, 22);
            btnPrevisualizar.TabIndex = 2;
            btnPrevisualizar.Text = "Previsualizar";
            btnPrevisualizar.UseVisualStyleBackColor = true;
            // 
            // nudPorcentaje
            // 
            nudPorcentaje.DecimalPlaces = 2;
            nudPorcentaje.Location = new Point(149, 10);
            nudPorcentaje.Margin = new Padding(3, 2, 3, 2);
            nudPorcentaje.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            nudPorcentaje.Minimum = new decimal(new int[] { 90, 0, 0, int.MinValue });
            nudPorcentaje.Name = "nudPorcentaje";
            nudPorcentaje.Size = new Size(88, 23);
            nudPorcentaje.TabIndex = 1;
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
            // dgvPreview
            // 
            dgvPreview.BackgroundColor = SystemColors.Info;
            dgvPreview.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPreview.Dock = DockStyle.Fill;
            dgvPreview.GridColor = SystemColors.Menu;
            dgvPreview.Location = new Point(0, 45);
            dgvPreview.Margin = new Padding(3, 2, 3, 2);
            dgvPreview.Name = "dgvPreview";
            dgvPreview.ReadOnly = true;
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
        private Label lblCriterio;
        private ComboBox cmbCriterio;
        private Label lblCategoria;
        private ComboBox cmbCategoriaFiltro;
        private Label lblMarca;
        private ComboBox cmbMarcaFiltro;
        private Label lblProducto;
        private ComboBox cmbProductoFiltro;
        private Panel panelAcciones;
        private Label lblPorcentaje;
        private NumericUpDown nudPorcentaje;
        private Button btnPrevisualizar;
        private Button btnConfirmar;
        private DataGridView dgvPreview;
    }
}