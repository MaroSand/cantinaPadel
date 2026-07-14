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
            tabPageMarcas = new TabPage();
            tabPageActualizacionPrecios = new TabPage();
            tabPageCategorias = new TabPage();
            panelAcciones = new Panel();
            btnBajaLogica = new Button();
            btnModificar = new Button();
            btnNuevo = new Button();
            dgvProductos = new DataGridView();
            panelFiltros.SuspendLayout();
            panelAcciones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvProductos).BeginInit();
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
             panelFiltros.Name = "panelFiltros";
             panelFiltros.Size = new Size(1060, 60);
             panelFiltros.TabIndex = 0;
            // 
            // cmbMarcaFiltro
            // 
            cmbMarcaFiltro.FormattingEnabled = true;
            cmbMarcaFiltro.Location = new Point(840, 16);
            cmbMarcaFiltro.Name = "cmbMarcaFiltro";
            cmbMarcaFiltro.Size = new Size(180, 28);
            cmbMarcaFiltro.TabIndex = 5;
            // 
            // lblMarca
            // 
            lblMarca.AutoSize = true;
            lblMarca.Location = new Point(780, 19);
            lblMarca.Name = "lblMarca";
            lblMarca.Size = new Size(54, 20);
            lblMarca.TabIndex = 4;
            lblMarca.Text = "Marca:";
            // 
            // cmbCategoriaFiltro
            // 
            cmbCategoriaFiltro.FormattingEnabled = true;
            cmbCategoriaFiltro.Location = new Point(580, 16);
            cmbCategoriaFiltro.Name = "cmbCategoriaFiltro";
            cmbCategoriaFiltro.Size = new Size(180, 28);
            cmbCategoriaFiltro.TabIndex = 3;
            // 
            // lblCategoria
            // 
            lblCategoria.AutoSize = true;
            lblCategoria.Location = new Point(500, 19);
            lblCategoria.Name = "lblCategoria";
            lblCategoria.Size = new Size(78, 20);
            lblCategoria.TabIndex = 2;
            lblCategoria.Text = "Categoría:";
            // 
            // txtBuscar
            // 
            txtBuscar.Location = new Point(240, 16);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new Size(240, 27);
            txtBuscar.TabIndex = 1;
            // 
            // lblBuscar
            // 
            lblBuscar.AutoSize = true;
            lblBuscar.Location = new Point(21, 19);
            lblBuscar.Name = "lblBuscar";
            lblBuscar.Size = new Size(210, 20);
            lblBuscar.TabIndex = 0;
            lblBuscar.Text = "Buscar (nombre o código):";
            // 
            // panelAcciones
            // 
             panelAcciones.Controls.Add(btnBajaLogica);
             panelAcciones.Controls.Add(btnModificar);
             panelAcciones.Controls.Add(btnNuevo);
             panelAcciones.Dock = DockStyle.Bottom;
             panelAcciones.Location = new Point(0, 430);
             panelAcciones.Name = "panelAcciones";
             panelAcciones.Size = new Size(1060, 50);
             panelAcciones.TabIndex = 1;
            // 
            // btnBajaLogica
            // 
            btnBajaLogica.FlatStyle = FlatStyle.Flat;
            btnBajaLogica.Location = new Point(600, 9);
            btnBajaLogica.Name = "btnBajaLogica";
            btnBajaLogica.Size = new Size(150, 29);
            btnBajaLogica.TabIndex = 2;
            btnBajaLogica.Text = "Dar de Baja";
            btnBajaLogica.UseVisualStyleBackColor = true;
            // 
            // btnModificar
            // 
            btnModificar.FlatStyle = FlatStyle.Flat;
            btnModificar.Location = new Point(420, 9);
            btnModificar.Name = "btnModificar";
            btnModificar.Size = new Size(130, 29);
            btnModificar.TabIndex = 1;
            btnModificar.Text = "Modificar";
            btnModificar.UseVisualStyleBackColor = true;
            // 
            // btnNuevo
            // 
            btnNuevo.FlatStyle = FlatStyle.Flat;
            btnNuevo.Location = new Point(200, 9);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(170, 29);
            btnNuevo.TabIndex = 0;
            btnNuevo.Text = "Nuevo Producto";
            btnNuevo.UseVisualStyleBackColor = true;
            // 
            // dgvProductos
            // 
             dgvProductos.BackgroundColor = SystemColors.Info;
             dgvProductos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
             dgvProductos.Dock = DockStyle.Fill;
             dgvProductos.GridColor = SystemColors.Menu;
             dgvProductos.Location = new Point(0, 60);
             dgvProductos.Name = "dgvProductos";
             dgvProductos.RowHeadersWidth = 51;
             dgvProductos.Size = new Size(1060, 370);
             dgvProductos.TabIndex = 2;
            // 
            // tabControlMain
            // 
            tabControlMain.Dock = DockStyle.Fill;
            tabControlMain.Location = new Point(0, 0);
            tabControlMain.Name = "tabControlMain";
            tabControlMain.SelectedIndex = 0;
            tabControlMain.Size = new Size(1060, 480);
            tabControlMain.TabIndex = 3;
            // 
            // tabPageProductos
            // 
            tabPageProductos.Text = "Productos";
            tabPageProductos.UseVisualStyleBackColor = true;
            tabPageProductos.Controls.Add(dgvProductos);
            tabPageProductos.Controls.Add(panelAcciones);
            tabPageProductos.Controls.Add(panelFiltros);
            // 
            // tabPageMarcas
            // 
            tabPageMarcas.Text = "Marcas";
            tabPageMarcas.UseVisualStyleBackColor = true;
            // 
            // tabPageCategorias
            // 
            tabPageCategorias.Text = "Categorías";
            tabPageCategorias.UseVisualStyleBackColor = true;

            // tabPageActualizacionPrecios
            // 
            tabPageActualizacionPrecios.Text = "Actualización de Precios";
            tabPageActualizacionPrecios.UseVisualStyleBackColor = true;
            // 
            // Add pages to tabControl
            // 
            tabControlMain.TabPages.AddRange(new TabPage[] { tabPageProductos, tabPageMarcas, tabPageCategorias, tabPageActualizacionPrecios });

            // 
            // FrmListadoProductos
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1060, 480);
            Controls.Add(tabControlMain);
            Name = "FrmListadoProductos";
            Text = "Listado de Productos";
            Load += FrmListadoProductos_Load;
            panelFiltros.ResumeLayout(false);
            panelFiltros.PerformLayout();
            panelAcciones.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvProductos).EndInit();
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