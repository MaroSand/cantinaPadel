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
            panelFiltros = new System.Windows.Forms.Panel();
            cmbEstado = new System.Windows.Forms.ComboBox();
            label2 = new System.Windows.Forms.Label();
            txtBuscar = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            panelAcciones = new System.Windows.Forms.Panel();
            btnBajaLogica = new System.Windows.Forms.Button();
            btnModificar = new System.Windows.Forms.Button();
            btnNuevo = new System.Windows.Forms.Button();
            dgvClientes = new System.Windows.Forms.DataGridView();
            panelFiltros.SuspendLayout();
            panelAcciones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvClientes).BeginInit();
            SuspendLayout();
            // 
            // panelFiltros
            // 
            panelFiltros.BackColor = System.Drawing.Color.Gold;
            panelFiltros.Controls.Add(cmbEstado);
            panelFiltros.Controls.Add(label2);
            panelFiltros.Controls.Add(txtBuscar);
            panelFiltros.Controls.Add(label1);
            panelFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            panelFiltros.Location = new System.Drawing.Point(0, 0);
            panelFiltros.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            panelFiltros.Name = "panelFiltros";
            panelFiltros.Size = new System.Drawing.Size(1433, 128);
            panelFiltros.TabIndex = 0;
            // 
            // cmbEstado
            // 
            cmbEstado.FormattingEnabled = true;
            cmbEstado.Items.AddRange(new object[] { "Todos", "Activos", "Inactivos" });
            cmbEstado.Location = new System.Drawing.Point(1170, 34);
            cmbEstado.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            cmbEstado.Name = "cmbEstado";
            cmbEstado.Size = new System.Drawing.Size(220, 40);
            cmbEstado.TabIndex = 3;
            cmbEstado.SelectedIndexChanged += cmbEstado_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(1058, 40);
            label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(89, 32);
            label2.TabIndex = 2;
            label2.Text = "Estado:";
            // 
            // txtBuscar
            // 
            txtBuscar.Location = new System.Drawing.Point(502, 34);
            txtBuscar.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new System.Drawing.Size(516, 39);
            txtBuscar.TabIndex = 1;
            txtBuscar.TextChanged += txtBuscar_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(39, 40);
            label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(364, 32);
            label1.TabIndex = 0;
            label1.Text = "Buscar por Nombre, Email o DNI:";
            // 
            // panelAcciones
            // 
            panelAcciones.BackColor = System.Drawing.SystemColors.Info;
            panelAcciones.Controls.Add(btnBajaLogica);
            panelAcciones.Controls.Add(btnModificar);
            panelAcciones.Controls.Add(btnNuevo);
            panelAcciones.Dock = System.Windows.Forms.DockStyle.Bottom;
            panelAcciones.Location = new System.Drawing.Point(0, 618);
            panelAcciones.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            panelAcciones.Name = "panelAcciones";
            panelAcciones.Size = new System.Drawing.Size(1433, 107);
            panelAcciones.TabIndex = 1;
            // 
            // btnBajaLogica
            // 
            btnBajaLogica.BackColor = System.Drawing.Color.White;
            btnBajaLogica.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnBajaLogica.Location = new System.Drawing.Point(1038, 19);
            btnBajaLogica.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            btnBajaLogica.Name = "btnBajaLogica";
            btnBajaLogica.Size = new System.Drawing.Size(258, 62);
            btnBajaLogica.TabIndex = 2;
            btnBajaLogica.Text = "Activar/ Desactivar";
            btnBajaLogica.UseVisualStyleBackColor = false;
            btnBajaLogica.Click += btnBajaLogica_Click;
            // 
            // btnModificar
            // 
            btnModificar.BackColor = System.Drawing.Color.White;
            btnModificar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnModificar.Location = new System.Drawing.Point(648, 19);
            btnModificar.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            btnModificar.Name = "btnModificar";
            btnModificar.Size = new System.Drawing.Size(206, 62);
            btnModificar.TabIndex = 1;
            btnModificar.Text = "Modificar";
            btnModificar.UseVisualStyleBackColor = false;
            btnModificar.Click += btnModificar_Click;
            // 
            // btnNuevo
            // 
            btnNuevo.BackColor = System.Drawing.Color.White;
            btnNuevo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnNuevo.Location = new System.Drawing.Point(216, 19);
            btnNuevo.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new System.Drawing.Size(257, 62);
            btnNuevo.TabIndex = 0;
            btnNuevo.Text = "Nuevo Cliente";
            btnNuevo.UseVisualStyleBackColor = false;
            btnNuevo.Click += btnNuevo_Click;
            // 
            // dgvClientes
            // 
            dgvClientes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dgvClientes.BackgroundColor = System.Drawing.Color.White;
            dgvClientes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvClientes.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvClientes.GridColor = System.Drawing.SystemColors.Menu;
            dgvClientes.Location = new System.Drawing.Point(0, 128);
            dgvClientes.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            dgvClientes.Name = "dgvClientes";
            dgvClientes.RowHeadersWidth = 51;
            dgvClientes.Size = new System.Drawing.Size(1433, 490);
            dgvClientes.TabIndex = 2;
            // 
            // FrmListadoClientes
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1433, 725);
            Controls.Add(dgvClientes);
            Controls.Add(panelAcciones);
            Controls.Add(panelFiltros);
            Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
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
        private System.Windows.Forms.DataGridView dgvClientes;
        private Label label2;
        private ComboBox cmbEstado;
    }
}