namespace cantinaPadel.UI
{
    partial class FrmListadoClientes
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
            txtBuscar = new TextBox();
            label1 = new Label();
            panelAcciones = new Panel();
            btnBajaLogica = new Button();
            btnModificar = new Button();
            btnNuevo = new Button();
            dgvClientes = new DataGridView();
            panelFiltros.SuspendLayout();
            panelAcciones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvClientes).BeginInit();
            SuspendLayout();
            // 
            // panelFiltros
            // 
            panelFiltros.BackColor = Color.Gold;
            panelFiltros.Controls.Add(cmbEstado);
            panelFiltros.Controls.Add(label2);
            panelFiltros.Controls.Add(txtBuscar);
            panelFiltros.Controls.Add(label1);
            panelFiltros.Dock = DockStyle.Top;
            panelFiltros.Location = new Point(0, 0);
            panelFiltros.Margin = new Padding(5, 6, 5, 6);
            panelFiltros.Name = "panelFiltros";
            panelFiltros.Size = new Size(1433, 128);
            panelFiltros.TabIndex = 0;
            // 
            // cmbEstado
            // 
            cmbEstado.FormattingEnabled = true;
            cmbEstado.Items.AddRange(new object[] { "Todos", "Activos", "Inactivos" });
            cmbEstado.Location = new Point(1170, 34);
            cmbEstado.Margin = new Padding(5, 6, 5, 6);
            cmbEstado.Name = "cmbEstado";
            cmbEstado.Size = new Size(220, 40);
            cmbEstado.TabIndex = 3;
            cmbEstado.SelectedIndexChanged += cmbEstado_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(1058, 40);
            label2.Margin = new Padding(5, 0, 5, 0);
            label2.Name = "label2";
            label2.Size = new Size(89, 32);
            label2.TabIndex = 2;
            label2.Text = "Estado:";
            // 
            // txtBuscar
            // 
            txtBuscar.Location = new Point(502, 34);
            txtBuscar.Margin = new Padding(5, 6, 5, 6);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new Size(516, 39);
            txtBuscar.TabIndex = 1;
            txtBuscar.TextChanged += txtBuscar_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(39, 40);
            label1.Margin = new Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new Size(364, 32);
            label1.TabIndex = 0;
            label1.Text = "Buscar por Nombre, Email o DNI:";
            // 
            // panelAcciones
            // 
            panelAcciones.BackColor = SystemColors.Info;
            panelAcciones.Controls.Add(btnBajaLogica);
            panelAcciones.Controls.Add(btnModificar);
            panelAcciones.Controls.Add(btnNuevo);
            panelAcciones.Dock = DockStyle.Bottom;
            panelAcciones.Location = new Point(0, 618);
            panelAcciones.Margin = new Padding(5, 6, 5, 6);
            panelAcciones.Name = "panelAcciones";
            panelAcciones.Size = new Size(1433, 107);
            panelAcciones.TabIndex = 1;
            // 
            // btnBajaLogica
            // 
            btnBajaLogica.BackColor = Color.White;
            btnBajaLogica.FlatStyle = FlatStyle.Flat;
            btnBajaLogica.Location = new Point(1038, 19);
            btnBajaLogica.Margin = new Padding(5, 6, 5, 6);
            btnBajaLogica.Name = "btnBajaLogica";
            btnBajaLogica.Size = new Size(258, 62);
            btnBajaLogica.TabIndex = 2;
            btnBajaLogica.Text = "Activar/ Desactivar";
            btnBajaLogica.UseVisualStyleBackColor = false;
            btnBajaLogica.Click += btnBajaLogica_Click;
            // 
            // btnModificar
            // 
            btnModificar.BackColor = Color.White;
            btnModificar.FlatStyle = FlatStyle.Flat;
            btnModificar.Location = new Point(648, 19);
            btnModificar.Margin = new Padding(5, 6, 5, 6);
            btnModificar.Name = "btnModificar";
            btnModificar.Size = new Size(206, 62);
            btnModificar.TabIndex = 1;
            btnModificar.Text = "Modificar";
            btnModificar.UseVisualStyleBackColor = false;
            btnModificar.Click += btnModificar_Click;
            // 
            // btnNuevo
            // 
            btnNuevo.BackColor = Color.White;
            btnNuevo.FlatStyle = FlatStyle.Flat;
            btnNuevo.Location = new Point(216, 19);
            btnNuevo.Margin = new Padding(5, 6, 5, 6);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(257, 62);
            btnNuevo.TabIndex = 0;
            btnNuevo.Text = "Nuevo Cliente";
            btnNuevo.UseVisualStyleBackColor = false;
            btnNuevo.Click += btnNuevo_Click;
            // 
            // dgvClientes
            // 
            dgvClientes.BackgroundColor = Color.White;
            dgvClientes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvClientes.Dock = DockStyle.Fill;
            dgvClientes.GridColor = SystemColors.Menu;
            dgvClientes.Location = new Point(0, 128);
            dgvClientes.Margin = new Padding(5, 6, 5, 6);
            dgvClientes.Name = "dgvClientes";
            dgvClientes.RowHeadersWidth = 51;
            dgvClientes.Size = new Size(1433, 490);
            dgvClientes.TabIndex = 2;
            // 
            // FrmListadoClientes
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1433, 725);
            Controls.Add(dgvClientes);
            Controls.Add(panelAcciones);
            Controls.Add(panelFiltros);
            Margin = new Padding(5, 6, 5, 6);
            Name = "FrmListadoClientes";
            Text = "Form1";
            Load += FrmListadoClientes_Load;
            panelFiltros.ResumeLayout(false);
            panelFiltros.PerformLayout();
            panelAcciones.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvClientes).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelFiltros;
        private Label label1;
        private TextBox txtBuscar;
        private Panel panelAcciones;
        private Button btnBajaLogica;
        private Button btnModificar;
        private Button btnNuevo;
        private DataGridView dgvClientes;
        private Label label2;
        private ComboBox cmbEstado;
    }
}