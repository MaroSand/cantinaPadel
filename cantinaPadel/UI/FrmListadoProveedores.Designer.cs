namespace cantinaPadel.UI
{
    partial class FrmListadoProveedores
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
            cmbEstado = new ComboBox();
            label2 = new Label();
            txtBuscarNombre = new TextBox();
            label1 = new Label();
            panelAcciones = new Panel();
            btnBajaLogica = new Button();
            btnModificar = new Button();
            btnNuevo = new Button();
            dgvProveedores = new DataGridView();
            panelFiltros.SuspendLayout();
            panelAcciones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvProveedores).BeginInit();
            SuspendLayout();
            // 
            // panelFiltros
            // 
            panelFiltros.BackColor = Color.Gold;
            panelFiltros.Controls.Add(cmbEstado);
            panelFiltros.Controls.Add(label2);
            panelFiltros.Controls.Add(txtBuscarNombre);
            panelFiltros.Controls.Add(label1);
            panelFiltros.Dock = DockStyle.Top;
            panelFiltros.Location = new Point(0, 0);
            panelFiltros.Margin = new Padding(5, 5, 5, 5);
            panelFiltros.Name = "panelFiltros";
            panelFiltros.Size = new Size(1433, 96);
            panelFiltros.TabIndex = 0;
            // 
            // cmbEstado
            // 
            cmbEstado.FormattingEnabled = true;
            cmbEstado.Items.AddRange(new object[] { "Todos", "Activos", "Inactivos" });
            cmbEstado.Location = new Point(908, 26);
            cmbEstado.Margin = new Padding(5, 5, 5, 5);
            cmbEstado.Name = "cmbEstado";
            cmbEstado.Size = new Size(243, 40);
            cmbEstado.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(806, 30);
            label2.Margin = new Padding(5, 0, 5, 0);
            label2.Name = "label2";
            label2.Size = new Size(89, 32);
            label2.TabIndex = 2;
            label2.Text = "Estado:";
            // 
            // txtBuscarNombre
            // 
            txtBuscarNombre.Location = new Point(375, 26);
            txtBuscarNombre.Margin = new Padding(5, 5, 5, 5);
            txtBuscarNombre.Name = "txtBuscarNombre";
            txtBuscarNombre.Size = new Size(384, 39);
            txtBuscarNombre.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(34, 30);
            label1.Margin = new Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new Size(325, 32);
            label1.TabIndex = 0;
            label1.Text = "Buscar por Nombre/Empresa:";
            // 
            // panelAcciones
            // 
            panelAcciones.BackColor = SystemColors.Info;
            panelAcciones.Controls.Add(btnBajaLogica);
            panelAcciones.Controls.Add(btnModificar);
            panelAcciones.Controls.Add(btnNuevo);
            panelAcciones.Dock = DockStyle.Bottom;
            panelAcciones.Location = new Point(0, 645);
            panelAcciones.Margin = new Padding(5, 5, 5, 5);
            panelAcciones.Name = "panelAcciones";
            panelAcciones.Size = new Size(1433, 80);
            panelAcciones.TabIndex = 1;
            // 
            // btnBajaLogica
            // 
            btnBajaLogica.BackColor = Color.White;
            btnBajaLogica.FlatStyle = FlatStyle.Flat;
            btnBajaLogica.Location = new Point(928, 14);
            btnBajaLogica.Margin = new Padding(5, 5, 5, 5);
            btnBajaLogica.Name = "btnBajaLogica";
            btnBajaLogica.Size = new Size(226, 46);
            btnBajaLogica.TabIndex = 2;
            btnBajaLogica.Text = "Activar/ Desactivar";
            btnBajaLogica.UseVisualStyleBackColor = false;
            // 
            // btnModificar
            // 
            btnModificar.BackColor = Color.White;
            btnModificar.FlatStyle = FlatStyle.Flat;
            btnModificar.Location = new Point(637, 14);
            btnModificar.Margin = new Padding(5, 5, 5, 5);
            btnModificar.Name = "btnModificar";
            btnModificar.Size = new Size(180, 46);
            btnModificar.TabIndex = 1;
            btnModificar.Text = "Modificar";
            btnModificar.UseVisualStyleBackColor = false;
            // 
            // btnNuevo
            // 
            btnNuevo.BackColor = Color.White;
            btnNuevo.FlatStyle = FlatStyle.Flat;
            btnNuevo.Location = new Point(268, 14);
            btnNuevo.Margin = new Padding(5, 5, 5, 5);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(224, 46);
            btnNuevo.TabIndex = 0;
            btnNuevo.Text = "Nuevo Proveedor";
            btnNuevo.UseVisualStyleBackColor = false;
            // 
            // dgvProveedores
            // 
            dgvProveedores.BackgroundColor = Color.White;
            dgvProveedores.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProveedores.Dock = DockStyle.Fill;
            dgvProveedores.GridColor = SystemColors.Menu;
            dgvProveedores.Location = new Point(0, 96);
            dgvProveedores.Margin = new Padding(5, 5, 5, 5);
            dgvProveedores.Name = "dgvProveedores";
            dgvProveedores.RowHeadersWidth = 51;
            dgvProveedores.Size = new Size(1433, 549);
            dgvProveedores.TabIndex = 2;
            // 
            // FrmListadoProveedores
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1433, 725);
            Controls.Add(dgvProveedores);
            Controls.Add(panelAcciones);
            Controls.Add(panelFiltros);
            Margin = new Padding(5, 5, 5, 5);
            Name = "FrmListadoProveedores";
            Text = "Listado de Proveedores";
            Load += FrmListadoProveedores_Load;
            panelFiltros.ResumeLayout(false);
            panelFiltros.PerformLayout();
            panelAcciones.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvProveedores).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelFiltros;
        private Label label1;
        private ComboBox cmbEstado;
        private Label label2;
        private TextBox txtBuscarNombre;
        private Panel panelAcciones;
        private Button btnBajaLogica;
        private Button btnModificar;
        private Button btnNuevo;
        private DataGridView dgvProveedores;
    }
}