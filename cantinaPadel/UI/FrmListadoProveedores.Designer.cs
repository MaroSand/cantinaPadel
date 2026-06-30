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
            panelFiltros.Name = "panelFiltros";
            panelFiltros.Size = new Size(800, 60);
            panelFiltros.TabIndex = 0;
            // 
            // cmbEstado
            // 
            cmbEstado.FormattingEnabled = true;
            cmbEstado.Items.AddRange(new object[] { "Todos", "Activos", "Inactivos" });
            cmbEstado.Location = new Point(559, 16);
            cmbEstado.Name = "cmbEstado";
            cmbEstado.Size = new Size(151, 28);
            cmbEstado.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(496, 19);
            label2.Name = "label2";
            label2.Size = new Size(57, 20);
            label2.TabIndex = 2;
            label2.Text = "Estado:";
            // 
            // txtBuscarNombre
            // 
            txtBuscarNombre.Location = new Point(231, 16);
            txtBuscarNombre.Name = "txtBuscarNombre";
            txtBuscarNombre.Size = new Size(238, 27);
            txtBuscarNombre.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(21, 19);
            label1.Name = "label1";
            label1.Size = new Size(204, 20);
            label1.TabIndex = 0;
            label1.Text = "Buscar por Nombre/Empresa:";
            // 
            // panelAcciones
            // 
            panelAcciones.Controls.Add(btnBajaLogica);
            panelAcciones.Controls.Add(btnModificar);
            panelAcciones.Controls.Add(btnNuevo);
            panelAcciones.Dock = DockStyle.Bottom;
            panelAcciones.Location = new Point(0, 400);
            panelAcciones.Name = "panelAcciones";
            panelAcciones.Size = new Size(800, 50);
            panelAcciones.TabIndex = 1;
            // 
            // btnBajaLogica
            // 
            btnBajaLogica.Location = new Point(559, 9);
            btnBajaLogica.Name = "btnBajaLogica";
            btnBajaLogica.Size = new Size(139, 29);
            btnBajaLogica.TabIndex = 2;
            btnBajaLogica.Text = "Dar de Baja / Alta";
            btnBajaLogica.UseVisualStyleBackColor = true;
            // 
            // btnModificar
            // 
            btnModificar.Location = new Point(349, 9);
            btnModificar.Name = "btnModificar";
            btnModificar.Size = new Size(111, 29);
            btnModificar.TabIndex = 1;
            btnModificar.Text = "Modificar";
            btnModificar.UseVisualStyleBackColor = true;
            // 
            // btnNuevo
            // 
            btnNuevo.Location = new Point(116, 9);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(138, 29);
            btnNuevo.TabIndex = 0;
            btnNuevo.Text = "Nuevo Proveedor";
            btnNuevo.UseVisualStyleBackColor = true;
            // 
            // dgvProveedores
            // 
            dgvProveedores.BackgroundColor = SystemColors.Info;
            dgvProveedores.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProveedores.Dock = DockStyle.Fill;
            dgvProveedores.GridColor = SystemColors.Menu;
            dgvProveedores.Location = new Point(0, 60);
            dgvProveedores.Name = "dgvProveedores";
            dgvProveedores.RowHeadersWidth = 51;
            dgvProveedores.Size = new Size(800, 340);
            dgvProveedores.TabIndex = 2;
            // 
            // FrmListadoProveedores
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(dgvProveedores);
            Controls.Add(panelAcciones);
            Controls.Add(panelFiltros);
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