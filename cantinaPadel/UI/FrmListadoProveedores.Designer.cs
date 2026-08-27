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
            panelFiltros = new System.Windows.Forms.Panel();
            cmbEstado = new System.Windows.Forms.ComboBox();
            label2 = new System.Windows.Forms.Label();
            txtBuscarNombre = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            panelAcciones = new System.Windows.Forms.Panel();
            btnBajaLogica = new System.Windows.Forms.Button();
            btnModificar = new System.Windows.Forms.Button();
            btnNuevo = new System.Windows.Forms.Button();
            dgvProveedores = new System.Windows.Forms.DataGridView();
            panelFiltros.SuspendLayout();
            panelAcciones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvProveedores).BeginInit();
            SuspendLayout();
            // 
            // panelFiltros
            // 
            panelFiltros.BackColor = System.Drawing.Color.Gold;
            panelFiltros.Controls.Add(cmbEstado);
            panelFiltros.Controls.Add(label2);
            panelFiltros.Controls.Add(txtBuscarNombre);
            panelFiltros.Controls.Add(label1);
            panelFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            panelFiltros.Location = new System.Drawing.Point(0, 0);
            panelFiltros.Margin = new System.Windows.Forms.Padding(5);
            panelFiltros.Name = "panelFiltros";
            panelFiltros.Size = new System.Drawing.Size(1433, 96);
            panelFiltros.TabIndex = 0;
            // 
            // cmbEstado
            // 
            cmbEstado.FormattingEnabled = true;
            cmbEstado.Items.AddRange(new object[] { "Todos", "Activos", "Inactivos" });
            cmbEstado.Location = new System.Drawing.Point(908, 26);
            cmbEstado.Margin = new System.Windows.Forms.Padding(5);
            cmbEstado.Name = "cmbEstado";
            cmbEstado.Size = new System.Drawing.Size(243, 40);
            cmbEstado.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(806, 30);
            label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(89, 32);
            label2.TabIndex = 2;
            label2.Text = "Estado:";
            // 
            // txtBuscarNombre
            // 
            txtBuscarNombre.Location = new System.Drawing.Point(375, 26);
            txtBuscarNombre.Margin = new System.Windows.Forms.Padding(5);
            txtBuscarNombre.Name = "txtBuscarNombre";
            txtBuscarNombre.Size = new System.Drawing.Size(384, 39);
            txtBuscarNombre.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(34, 30);
            label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(325, 32);
            label1.TabIndex = 0;
            label1.Text = "Buscar por Nombre/Empresa:";
            // 
            // panelAcciones
            // 
            panelAcciones.BackColor = System.Drawing.SystemColors.Info;
            panelAcciones.Controls.Add(btnBajaLogica);
            panelAcciones.Controls.Add(btnModificar);
            panelAcciones.Controls.Add(btnNuevo);
            panelAcciones.Dock = System.Windows.Forms.DockStyle.Bottom;
            panelAcciones.Location = new System.Drawing.Point(0, 645);
            panelAcciones.Margin = new System.Windows.Forms.Padding(5);
            panelAcciones.Name = "panelAcciones";
            panelAcciones.Size = new System.Drawing.Size(1433, 80);
            panelAcciones.TabIndex = 1;
            // 
            // btnBajaLogica
            // 
            btnBajaLogica.BackColor = System.Drawing.Color.White;
            btnBajaLogica.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnBajaLogica.Location = new System.Drawing.Point(928, 14);
            btnBajaLogica.Margin = new System.Windows.Forms.Padding(5);
            btnBajaLogica.Name = "btnBajaLogica";
            btnBajaLogica.Size = new System.Drawing.Size(226, 46);
            btnBajaLogica.TabIndex = 2;
            btnBajaLogica.Text = "Activar/ Desactivar";
            btnBajaLogica.UseVisualStyleBackColor = false;
            // 
            // btnModificar
            // 
            btnModificar.BackColor = System.Drawing.Color.White;
            btnModificar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnModificar.Location = new System.Drawing.Point(637, 14);
            btnModificar.Margin = new System.Windows.Forms.Padding(5);
            btnModificar.Name = "btnModificar";
            btnModificar.Size = new System.Drawing.Size(180, 46);
            btnModificar.TabIndex = 1;
            btnModificar.Text = "Modificar";
            btnModificar.UseVisualStyleBackColor = false;
            // 
            // btnNuevo
            // 
            btnNuevo.BackColor = System.Drawing.Color.White;
            btnNuevo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnNuevo.Location = new System.Drawing.Point(268, 14);
            btnNuevo.Margin = new System.Windows.Forms.Padding(5);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new System.Drawing.Size(224, 46);
            btnNuevo.TabIndex = 0;
            btnNuevo.Text = "Nuevo Proveedor";
            btnNuevo.UseVisualStyleBackColor = false;
            // 
            // dgvProveedores
            // 
            dgvProveedores.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dgvProveedores.BackgroundColor = System.Drawing.Color.White;
            dgvProveedores.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProveedores.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvProveedores.GridColor = System.Drawing.SystemColors.Menu;
            dgvProveedores.Location = new System.Drawing.Point(0, 96);
            dgvProveedores.Margin = new System.Windows.Forms.Padding(5);
            dgvProveedores.Name = "dgvProveedores";
            dgvProveedores.RowHeadersWidth = 51;
            dgvProveedores.Size = new System.Drawing.Size(1433, 549);
            dgvProveedores.TabIndex = 2;
            // 
            // FrmListadoProveedores
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1433, 725);
            Controls.Add(dgvProveedores);
            Controls.Add(panelAcciones);
            Controls.Add(panelFiltros);
            Margin = new System.Windows.Forms.Padding(5);
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
        private System.Windows.Forms.DataGridView dgvProveedores;
    }
}