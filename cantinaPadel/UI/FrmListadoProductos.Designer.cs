namespace cantinaPadel.UI
{
    partial class FrmListadoProductos
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
            panelFiltros = new Panel();
            cmbMarcaFiltro = new ComboBox();
            lblMarca = new Label();
            cmbCategoriaFiltro = new ComboBox();
            lblCategoria = new Label();
            txtBuscar = new TextBox();
            lblBuscar = new Label();
            tabControlMain = new TabControl();
            tabPageProductos = new TabPage();
            dgvProductos = new DataGridView();
            panelAcciones = new Panel();
            btnBajaLogica = new Button();
            btnModificar = new Button();
            btnNuevo = new Button();
            tabPageMarcas = new TabPage();
            tabPageCategorias = new TabPage();
            tabPageActualizacionPrecios = new TabPage();
            panelFiltros.SuspendLayout();
            tabControlMain.SuspendLayout();
            tabPageProductos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvProductos).BeginInit();
            panelAcciones.SuspendLayout();
            SuspendLayout();
            // 
            // panelFiltros
            // 
            panelFiltros.BackColor = Color.Gold;
            panelFiltros.Controls.Add(cmbMarcaFiltro);
            panelFiltros.Controls.Add(lblMarca);
            panelFiltros.Controls.Add(cmbCategoriaFiltro);
            panelFiltros.Controls.Add(lblCategoria);
            panelFiltros.Controls.Add(txtBuscar);
            panelFiltros.Controls.Add(lblBuscar);
            panelFiltros.Dock = DockStyle.Top;
            panelFiltros.Location = new Point(0, 0);
            panelFiltros.Margin = new Padding(5, 5, 5, 5);
            panelFiltros.Name = "panelFiltros";
            panelFiltros.Size = new Size(1706, 96);
            panelFiltros.TabIndex = 0;
            // 
            // cmbMarcaFiltro
            // 
            cmbMarcaFiltro.FormattingEnabled = true;
            cmbMarcaFiltro.Location = new Point(1365, 26);
            cmbMarcaFiltro.Margin = new Padding(5, 5, 5, 5);
            cmbMarcaFiltro.Name = "cmbMarcaFiltro";
            cmbMarcaFiltro.Size = new Size(290, 40);
            cmbMarcaFiltro.TabIndex = 5;
            // 
            // lblMarca
            // 
            lblMarca.AutoSize = true;
            lblMarca.Location = new Point(1268, 30);
            lblMarca.Margin = new Padding(5, 0, 5, 0);
            lblMarca.Name = "lblMarca";
            lblMarca.Size = new Size(84, 32);
            lblMarca.TabIndex = 4;
            lblMarca.Text = "Marca:";
            //
            // cmbCategoriaFiltro
            // 
            cmbCategoriaFiltro.FormattingEnabled = true;
            cmbCategoriaFiltro.Location = new Point(942, 26);
            cmbCategoriaFiltro.Margin = new Padding(5, 5, 5, 5);
            cmbCategoriaFiltro.Name = "cmbCategoriaFiltro";
            cmbCategoriaFiltro.Size = new Size(290, 40);
            cmbCategoriaFiltro.TabIndex = 3;
            // 
            // lblCategoria
            // 
            lblCategoria.AutoSize = true;
            lblCategoria.Location = new Point(812, 30);
            lblCategoria.Margin = new Padding(5, 0, 5, 0);
            lblCategoria.Name = "lblCategoria";
            lblCategoria.Size = new Size(121, 32);
            lblCategoria.TabIndex = 2;
            lblCategoria.Text = "Categoría:";
            // 
            // txtBuscar
            // 
            txtBuscar.Location = new Point(390, 26);
            txtBuscar.Margin = new Padding(5, 5, 5, 5);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new Size(388, 39);
            txtBuscar.TabIndex = 1;
            // 
            // lblBuscar
            // 
            lblBuscar.AutoSize = true;
            lblBuscar.Location = new Point(34, 30);
            lblBuscar.Margin = new Padding(5, 0, 5, 0);
            lblBuscar.Name = "lblBuscar";
            lblBuscar.Size = new Size(294, 32);
            lblBuscar.TabIndex = 0;
            lblBuscar.Text = "Buscar (nombre o código):";
            // 
            // tabControlMain
            // 
            tabControlMain.Controls.Add(tabPageProductos);
            tabControlMain.Controls.Add(tabPageMarcas);
            tabControlMain.Controls.Add(tabPageCategorias);
            tabControlMain.Controls.Add(tabPageActualizacionPrecios);
            tabControlMain.Dock = DockStyle.Fill;
            tabControlMain.Location = new Point(0, 0);
            tabControlMain.Margin = new Padding(5, 5, 5, 5);
            tabControlMain.Name = "tabControlMain";
            tabControlMain.SelectedIndex = 0;
            tabControlMain.Size = new Size(1722, 768);
            tabControlMain.TabIndex = 3;
            // 
            // tabPageProductos
            // 
            tabPageProductos.BackColor = SystemColors.Info;
            tabPageProductos.Controls.Add(dgvProductos);
            tabPageProductos.Controls.Add(panelAcciones);
            tabPageProductos.Controls.Add(panelFiltros);
            tabPageProductos.Location = new Point(8, 46);
            tabPageProductos.Margin = new Padding(5, 5, 5, 5);
            tabPageProductos.Name = "tabPageProductos";
            tabPageProductos.Size = new Size(1706, 714);
            tabPageProductos.TabIndex = 0;
            tabPageProductos.Text = "Productos";
            // 
            // dgvProductos
            // 
            dgvProductos.BackgroundColor = Color.White;
            dgvProductos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProductos.Dock = DockStyle.Fill;
            dgvProductos.GridColor = SystemColors.Menu;
            dgvProductos.Location = new Point(0, 96);
            dgvProductos.Margin = new Padding(5, 5, 5, 5);
            dgvProductos.Name = "dgvProductos";
            dgvProductos.RowHeadersWidth = 51;
            dgvProductos.Size = new Size(1706, 538);
            dgvProductos.TabIndex = 2;
            // 
            // panelAcciones
            // 
            panelAcciones.BackColor = SystemColors.Info;
            panelAcciones.Controls.Add(btnBajaLogica);
            panelAcciones.Controls.Add(btnModificar);
            panelAcciones.Controls.Add(btnNuevo);
            panelAcciones.Dock = DockStyle.Bottom;
            panelAcciones.Location = new Point(0, 634);
            panelAcciones.Margin = new Padding(5, 5, 5, 5);
            panelAcciones.Name = "panelAcciones";
            panelAcciones.Size = new Size(1706, 80);
            panelAcciones.TabIndex = 1;
            // 
            // btnBajaLogica
            // 
            btnBajaLogica.BackColor = Color.White;
            btnBajaLogica.FlatStyle = FlatStyle.Flat;
            btnBajaLogica.Location = new Point(975, 14);
            btnBajaLogica.Margin = new Padding(5, 5, 5, 5);
            btnBajaLogica.Name = "btnBajaLogica";
            btnBajaLogica.Size = new Size(244, 46);
            btnBajaLogica.TabIndex = 2;
            btnBajaLogica.Text = "Activar / Desactivar";
            btnBajaLogica.UseVisualStyleBackColor = false;
            // 
            // btnModificar
            // 
            btnModificar.BackColor = Color.White;
            btnModificar.FlatStyle = FlatStyle.Flat;
            btnModificar.Location = new Point(682, 14);
            btnModificar.Margin = new Padding(5, 5, 5, 5);
            btnModificar.Name = "btnModificar";
            btnModificar.Size = new Size(211, 46);
            btnModificar.TabIndex = 1;
            btnModificar.Text = "Modificar";
            btnModificar.UseVisualStyleBackColor = false;
            // 
            // btnNuevo
            // 
            btnNuevo.BackColor = Color.White;
            btnNuevo.FlatStyle = FlatStyle.Flat;
            btnNuevo.Location = new Point(325, 14);
            btnNuevo.Margin = new Padding(5, 5, 5, 5);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(276, 46);
            btnNuevo.TabIndex = 0;
            btnNuevo.Text = "Nuevo Producto";
            btnNuevo.UseVisualStyleBackColor = false;
            // 
            // tabPageMarcas
            // 
            tabPageMarcas.Location = new Point(8, 46);
            tabPageMarcas.Margin = new Padding(5, 5, 5, 5);
            tabPageMarcas.Name = "tabPageMarcas";
            tabPageMarcas.Size = new Size(1706, 714);
            tabPageMarcas.TabIndex = 1;
            tabPageMarcas.Text = "Marcas";
            tabPageMarcas.UseVisualStyleBackColor = true;
            // 
            // tabPageCategorias
            // 
            tabPageCategorias.Location = new Point(8, 46);
            tabPageCategorias.Margin = new Padding(5, 5, 5, 5);
            tabPageCategorias.Name = "tabPageCategorias";
            tabPageCategorias.Size = new Size(1706, 714);
            tabPageCategorias.TabIndex = 2;
            tabPageCategorias.Text = "Categorías";
            tabPageCategorias.UseVisualStyleBackColor = true;
            // 
            // tabPageActualizacionPrecios
            // 
            tabPageActualizacionPrecios.Location = new Point(8, 46);
            tabPageActualizacionPrecios.Margin = new Padding(5, 5, 5, 5);
            tabPageActualizacionPrecios.Name = "tabPageActualizacionPrecios";
            tabPageActualizacionPrecios.Size = new Size(1706, 714);
            tabPageActualizacionPrecios.TabIndex = 3;
            tabPageActualizacionPrecios.Text = "Actualización de Precios";
            tabPageActualizacionPrecios.UseVisualStyleBackColor = true;
            // 
            // FrmListadoProductos
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1722, 768);
            Controls.Add(tabControlMain);
            Margin = new Padding(5, 5, 5, 5);
            Name = "FrmListadoProductos";
            Text = "Listado de Productos";
            Load += FrmListadoProductos_Load;
            panelFiltros.ResumeLayout(false);
            panelFiltros.PerformLayout();
            tabControlMain.ResumeLayout(false);
            tabPageProductos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvProductos).EndInit();
            panelAcciones.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panelFiltros;
        private Label lblBuscar;
        private TextBox txtBuscar;
        private Label lblCategoria;
        private ComboBox cmbCategoriaFiltro;
        private Label lblMarca;
        private ComboBox cmbMarcaFiltro;
        private TabControl tabControlMain;
        private TabPage tabPageProductos;
        private TabPage tabPageMarcas;
        private TabPage tabPageCategorias;
        private Panel panelAcciones;
        private Button btnBajaLogica;
        private Button btnModificar;
        private Button btnNuevo;
        private DataGridView dgvProductos;
        private TabPage tabPageActualizacionPrecios;

    }
}