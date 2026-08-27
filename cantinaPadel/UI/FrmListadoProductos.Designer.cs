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
            panelFiltros = new System.Windows.Forms.Panel();
            cmbEstadoFiltro = new System.Windows.Forms.ComboBox();
            lblEstado = new System.Windows.Forms.Label();
            cmbMarcaFiltro = new System.Windows.Forms.ComboBox();
            lblMarca = new System.Windows.Forms.Label();
            cmbCategoriaFiltro = new System.Windows.Forms.ComboBox();
            lblCategoria = new System.Windows.Forms.Label();
            txtBuscar = new System.Windows.Forms.TextBox();
            lblBuscar = new System.Windows.Forms.Label();
            tabControlMain = new System.Windows.Forms.TabControl();
            tabPageProductos = new System.Windows.Forms.TabPage();
            dgvProductos = new System.Windows.Forms.DataGridView();
            panelAcciones = new System.Windows.Forms.Panel();
            btnBajaLogica = new System.Windows.Forms.Button();
            btnModificar = new System.Windows.Forms.Button();
            btnNuevo = new System.Windows.Forms.Button();
            tabPageMarcas = new System.Windows.Forms.TabPage();
            tabPageCategorias = new System.Windows.Forms.TabPage();
            tabPageActualizacionPrecios = new System.Windows.Forms.TabPage();
            panelFiltros.SuspendLayout();
            tabControlMain.SuspendLayout();
            tabPageProductos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvProductos).BeginInit();
            panelAcciones.SuspendLayout();
            SuspendLayout();
            // 
            // panelFiltros
            // 
            panelFiltros.BackColor = System.Drawing.Color.Gold;
            panelFiltros.Controls.Add(cmbEstadoFiltro);
            panelFiltros.Controls.Add(lblEstado);
            panelFiltros.Controls.Add(cmbMarcaFiltro);
            panelFiltros.Controls.Add(lblMarca);
            panelFiltros.Controls.Add(cmbCategoriaFiltro);
            panelFiltros.Controls.Add(lblCategoria);
            panelFiltros.Controls.Add(txtBuscar);
            panelFiltros.Controls.Add(lblBuscar);
            panelFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            panelFiltros.Location = new System.Drawing.Point(0, 0);
            panelFiltros.Margin = new System.Windows.Forms.Padding(5);
            panelFiltros.Name = "panelFiltros";
            panelFiltros.Size = new System.Drawing.Size(1706, 96);
            panelFiltros.TabIndex = 0;
            // 
            // cmbEstadoFiltro
            // 
            cmbEstadoFiltro.FormattingEnabled = true;
            cmbEstadoFiltro.Location = new System.Drawing.Point(1560, 26);
            cmbEstadoFiltro.Margin = new System.Windows.Forms.Padding(5);
            cmbEstadoFiltro.Name = "cmbEstadoFiltro";
            cmbEstadoFiltro.Size = new System.Drawing.Size(135, 40);
            cmbEstadoFiltro.TabIndex = 7;
            // 
            // lblEstado
            // 
            lblEstado.AutoSize = true;
            lblEstado.Location = new System.Drawing.Point(1460, 30);
            lblEstado.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            lblEstado.Name = "lblEstado";
            lblEstado.Size = new System.Drawing.Size(89, 32);
            lblEstado.TabIndex = 6;
            lblEstado.Text = "Estado:";
            // 
            // cmbMarcaFiltro
            // 
            cmbMarcaFiltro.FormattingEnabled = true;
            cmbMarcaFiltro.Location = new System.Drawing.Point(1215, 26);
            cmbMarcaFiltro.Margin = new System.Windows.Forms.Padding(5);
            cmbMarcaFiltro.Name = "cmbMarcaFiltro";
            cmbMarcaFiltro.Size = new System.Drawing.Size(220, 40);
            cmbMarcaFiltro.TabIndex = 5;
            // 
            // lblMarca
            // 
            lblMarca.AutoSize = true;
            lblMarca.Location = new System.Drawing.Point(1130, 30);
            lblMarca.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            lblMarca.Name = "lblMarca";
            lblMarca.Size = new System.Drawing.Size(84, 32);
            lblMarca.TabIndex = 4;
            lblMarca.Text = "Marca:";
            // 
            // cmbCategoriaFiltro
            // 
            cmbCategoriaFiltro.FormattingEnabled = true;
            cmbCategoriaFiltro.Location = new System.Drawing.Point(865, 26);
            cmbCategoriaFiltro.Margin = new System.Windows.Forms.Padding(5);
            cmbCategoriaFiltro.Name = "cmbCategoriaFiltro";
            cmbCategoriaFiltro.Size = new System.Drawing.Size(240, 40);
            cmbCategoriaFiltro.TabIndex = 3;
            // 
            // lblCategoria
            // 
            lblCategoria.AutoSize = true;
            lblCategoria.Location = new System.Drawing.Point(735, 30);
            lblCategoria.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            lblCategoria.Name = "lblCategoria";
            lblCategoria.Size = new System.Drawing.Size(121, 32);
            lblCategoria.TabIndex = 2;
            lblCategoria.Text = "Categoría:";
            // 
            // txtBuscar
            // 
            txtBuscar.Location = new System.Drawing.Point(390, 26);
            txtBuscar.Margin = new System.Windows.Forms.Padding(5);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new System.Drawing.Size(315, 39);
            txtBuscar.TabIndex = 1;
            // 
            // lblBuscar
            // 
            lblBuscar.AutoSize = true;
            lblBuscar.Location = new System.Drawing.Point(34, 30);
            lblBuscar.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            lblBuscar.Name = "lblBuscar";
            lblBuscar.Size = new System.Drawing.Size(294, 32);
            lblBuscar.TabIndex = 0;
            lblBuscar.Text = "Buscar (nombre o código):";
            // 
            // tabControlMain
            // 
            tabControlMain.Controls.Add(tabPageProductos);
            tabControlMain.Controls.Add(tabPageMarcas);
            tabControlMain.Controls.Add(tabPageCategorias);
            tabControlMain.Controls.Add(tabPageActualizacionPrecios);
            tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControlMain.Location = new System.Drawing.Point(0, 0);
            tabControlMain.Margin = new System.Windows.Forms.Padding(5);
            tabControlMain.Name = "tabControlMain";
            tabControlMain.SelectedIndex = 0;
            tabControlMain.Size = new System.Drawing.Size(1722, 768);
            tabControlMain.TabIndex = 3;
            // 
            // tabPageProductos
            // 
            tabPageProductos.BackColor = System.Drawing.SystemColors.Info;
            tabPageProductos.Controls.Add(dgvProductos);
            tabPageProductos.Controls.Add(panelAcciones);
            tabPageProductos.Controls.Add(panelFiltros);
            tabPageProductos.Location = new System.Drawing.Point(8, 46);
            tabPageProductos.Margin = new System.Windows.Forms.Padding(5);
            tabPageProductos.Name = "tabPageProductos";
            tabPageProductos.Size = new System.Drawing.Size(1706, 714);
            tabPageProductos.TabIndex = 0;
            tabPageProductos.Text = "Productos";
            // 
            // dgvProductos
            // 
            dgvProductos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dgvProductos.BackgroundColor = System.Drawing.Color.White;
            dgvProductos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProductos.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvProductos.GridColor = System.Drawing.SystemColors.Menu;
            dgvProductos.Location = new System.Drawing.Point(0, 96);
            dgvProductos.Margin = new System.Windows.Forms.Padding(5);
            dgvProductos.Name = "dgvProductos";
            dgvProductos.RowHeadersWidth = 51;
            dgvProductos.Size = new System.Drawing.Size(1706, 538);
            dgvProductos.TabIndex = 2;
            // 
            // panelAcciones
            // 
            panelAcciones.BackColor = System.Drawing.SystemColors.Info;
            panelAcciones.Controls.Add(btnBajaLogica);
            panelAcciones.Controls.Add(btnModificar);
            panelAcciones.Controls.Add(btnNuevo);
            panelAcciones.Dock = System.Windows.Forms.DockStyle.Bottom;
            panelAcciones.Location = new System.Drawing.Point(0, 634);
            panelAcciones.Margin = new System.Windows.Forms.Padding(5);
            panelAcciones.Name = "panelAcciones";
            panelAcciones.Size = new System.Drawing.Size(1706, 80);
            panelAcciones.TabIndex = 1;
            // 
            // btnBajaLogica
            // 
            btnBajaLogica.BackColor = System.Drawing.Color.White;
            btnBajaLogica.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnBajaLogica.Location = new System.Drawing.Point(975, 14);
            btnBajaLogica.Margin = new System.Windows.Forms.Padding(5);
            btnBajaLogica.Name = "btnBajaLogica";
            btnBajaLogica.Size = new System.Drawing.Size(244, 46);
            btnBajaLogica.TabIndex = 2;
            btnBajaLogica.Text = "Activar / Desactivar";
            btnBajaLogica.UseVisualStyleBackColor = false;
            // 
            // btnModificar
            // 
            btnModificar.BackColor = System.Drawing.Color.White;
            btnModificar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnModificar.Location = new System.Drawing.Point(682, 14);
            btnModificar.Margin = new System.Windows.Forms.Padding(5);
            btnModificar.Name = "btnModificar";
            btnModificar.Size = new System.Drawing.Size(211, 46);
            btnModificar.TabIndex = 1;
            btnModificar.Text = "Modificar";
            btnModificar.UseVisualStyleBackColor = false;
            // 
            // btnNuevo
            // 
            btnNuevo.BackColor = System.Drawing.Color.White;
            btnNuevo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnNuevo.Location = new System.Drawing.Point(325, 14);
            btnNuevo.Margin = new System.Windows.Forms.Padding(5);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new System.Drawing.Size(276, 46);
            btnNuevo.TabIndex = 0;
            btnNuevo.Text = "Nuevo Producto";
            btnNuevo.UseVisualStyleBackColor = false;
            // 
            // tabPageMarcas
            // 
            tabPageMarcas.Location = new System.Drawing.Point(8, 46);
            tabPageMarcas.Margin = new System.Windows.Forms.Padding(5);
            tabPageMarcas.Name = "tabPageMarcas";
            tabPageMarcas.Size = new System.Drawing.Size(1706, 714);
            tabPageMarcas.TabIndex = 1;
            tabPageMarcas.Text = "Marcas";
            tabPageMarcas.UseVisualStyleBackColor = true;
            // 
            // tabPageCategorias
            // 
            tabPageCategorias.Location = new System.Drawing.Point(8, 46);
            tabPageCategorias.Margin = new System.Windows.Forms.Padding(5);
            tabPageCategorias.Name = "tabPageCategorias";
            tabPageCategorias.Size = new System.Drawing.Size(1706, 714);
            tabPageCategorias.TabIndex = 2;
            tabPageCategorias.Text = "Categorías";
            tabPageCategorias.UseVisualStyleBackColor = true;
            // 
            // tabPageActualizacionPrecios
            // 
            tabPageActualizacionPrecios.Location = new System.Drawing.Point(8, 46);
            tabPageActualizacionPrecios.Margin = new System.Windows.Forms.Padding(5);
            tabPageActualizacionPrecios.Name = "tabPageActualizacionPrecios";
            tabPageActualizacionPrecios.Size = new System.Drawing.Size(1706, 714);
            tabPageActualizacionPrecios.TabIndex = 3;
            tabPageActualizacionPrecios.Text = "Actualización de Precios";
            tabPageActualizacionPrecios.UseVisualStyleBackColor = true;
            // 
            // FrmListadoProductos
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1722, 768);
            Controls.Add(tabControlMain);
            Margin = new System.Windows.Forms.Padding(5);
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
        private Label lblEstado;
        private ComboBox cmbEstadoFiltro;
        private TabControl tabControlMain;
        private TabPage tabPageProductos;
        private TabPage tabPageMarcas;
        private TabPage tabPageCategorias;
        private Panel panelAcciones;
        private Button btnBajaLogica;
        private Button btnModificar;
        private Button btnNuevo;
        private System.Windows.Forms.DataGridView dgvProductos;
        private TabPage tabPageActualizacionPrecios;

    }
}
