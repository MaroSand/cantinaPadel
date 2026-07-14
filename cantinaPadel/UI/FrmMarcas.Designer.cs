namespace cantinaPadel.UI
{
    partial class FrmMarcas
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
            panelDatos = new Panel();
            txtNombre = new TextBox();
            btnGuardar = new Button();
            label3 = new Label();
            panelAcciones = new Panel();
            btnBajaAlta = new Button();
            btnNuevo = new Button();
            dgvMarcas = new DataGridView();
            label4 = new Label();
            panelFiltros.SuspendLayout();
            panelDatos.SuspendLayout();
            panelAcciones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMarcas).BeginInit();
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
            panelFiltros.Margin = new Padding(5);
            panelFiltros.Name = "panelFiltros";
            panelFiltros.Size = new Size(1433, 104);
            panelFiltros.TabIndex = 0;
            // 
            // cmbEstado
            // 
            cmbEstado.FormattingEnabled = true;
            cmbEstado.Items.AddRange(new object[] { "Todos", "Activos", "Inactivos" });
            cmbEstado.Location = new Point(824, 35);
            cmbEstado.Margin = new Padding(5);
            cmbEstado.Name = "cmbEstado";
            cmbEstado.Size = new Size(243, 40);
            cmbEstado.TabIndex = 3;
            cmbEstado.SelectedIndexChanged += cmbEstado_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(722, 40);
            label2.Margin = new Padding(5, 0, 5, 0);
            label2.Name = "label2";
            label2.Size = new Size(89, 32);
            label2.TabIndex = 2;
            label2.Text = "Estado:";
            // 
            // txtBuscar
            // 
            txtBuscar.Location = new Point(187, 37);
            txtBuscar.Margin = new Padding(5);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new Size(301, 39);
            txtBuscar.TabIndex = 1;
            txtBuscar.TextChanged += txtBuscar_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(88, 40);
            label1.Margin = new Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new Size(88, 32);
            label1.TabIndex = 0;
            label1.Text = "Buscar:";
            // 
            // panelDatos
            // 
            panelDatos.BackColor = SystemColors.Info;
            panelDatos.Controls.Add(label4);
            panelDatos.Controls.Add(txtNombre);
            panelDatos.Controls.Add(btnGuardar);
            panelDatos.Controls.Add(label3);
            panelDatos.Dock = DockStyle.Right;
            panelDatos.Location = new Point(1134, 104);
            panelDatos.Margin = new Padding(5);
            panelDatos.Name = "panelDatos";
            panelDatos.Size = new Size(299, 621);
            panelDatos.TabIndex = 1;
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(36, 272);
            txtNombre.Margin = new Padding(5);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(220, 39);
            txtNombre.TabIndex = 1;
            // 
            // btnGuardar
            // 
            btnGuardar.BackColor = Color.White;
            btnGuardar.FlatStyle = FlatStyle.Flat;
            btnGuardar.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnGuardar.Location = new Point(73, 392);
            btnGuardar.Margin = new Padding(5);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(145, 46);
            btnGuardar.TabIndex = 1;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = false;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(36, 216);
            label3.Margin = new Padding(5, 0, 5, 0);
            label3.Name = "label3";
            label3.Size = new Size(228, 32);
            label3.TabIndex = 0;
            label3.Text = "Nombre de Marca:";
            // 
            // panelAcciones
            // 
            panelAcciones.BackColor = SystemColors.Info;
            panelAcciones.Controls.Add(btnBajaAlta);
            panelAcciones.Controls.Add(btnNuevo);
            panelAcciones.Dock = DockStyle.Bottom;
            panelAcciones.Location = new Point(0, 634);
            panelAcciones.Margin = new Padding(5);
            panelAcciones.Name = "panelAcciones";
            panelAcciones.Size = new Size(1134, 91);
            panelAcciones.TabIndex = 2;
            // 
            // btnBajaAlta
            // 
            btnBajaAlta.BackColor = Color.White;
            btnBajaAlta.FlatStyle = FlatStyle.Flat;
            btnBajaAlta.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnBajaAlta.Location = new Point(697, 26);
            btnBajaAlta.Margin = new Padding(5);
            btnBajaAlta.Name = "btnBajaAlta";
            btnBajaAlta.Size = new Size(281, 46);
            btnBajaAlta.TabIndex = 2;
            btnBajaAlta.Text = "Activar/ Desactivar";
            btnBajaAlta.UseVisualStyleBackColor = false;
            btnBajaAlta.Click += btnBajaAlta_Click;
            // 
            // btnNuevo
            // 
            btnNuevo.BackColor = Color.White;
            btnNuevo.FlatStyle = FlatStyle.Flat;
            btnNuevo.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnNuevo.Location = new Point(294, 26);
            btnNuevo.Margin = new Padding(5);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(153, 46);
            btnNuevo.TabIndex = 0;
            btnNuevo.Text = "Nuevo";
            btnNuevo.UseVisualStyleBackColor = false;
            btnNuevo.Click += btnNuevo_Click;
            // 
            // dgvMarcas
            // 
            dgvMarcas.BackgroundColor = Color.White;
            dgvMarcas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMarcas.Dock = DockStyle.Fill;
            dgvMarcas.Location = new Point(0, 104);
            dgvMarcas.Margin = new Padding(5);
            dgvMarcas.Name = "dgvMarcas";
            dgvMarcas.RowHeadersWidth = 51;
            dgvMarcas.Size = new Size(1134, 530);
            dgvMarcas.TabIndex = 3;
            dgvMarcas.SelectionChanged += dgvMarcas_SelectionChanged;
            // 
            // label4
            // 
            label4.AutoEllipsis = true;
            label4.ForeColor = SystemColors.ControlDarkDark;
            label4.Location = new Point(36, 78);
            label4.Name = "label4";
            label4.Size = new Size(235, 74);
            label4.TabIndex = 2;
            label4.Text = "Selecciona una marca para editar.";
            // 
            // FrmMarcas
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1433, 725);
            Controls.Add(dgvMarcas);
            Controls.Add(panelAcciones);
            Controls.Add(panelDatos);
            Controls.Add(panelFiltros);
            Margin = new Padding(5);
            Name = "FrmMarcas";
            Text = "Gestión de Marcas";
            Load += FrmMarcas_Load;
            panelFiltros.ResumeLayout(false);
            panelFiltros.PerformLayout();
            panelDatos.ResumeLayout(false);
            panelDatos.PerformLayout();
            panelAcciones.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvMarcas).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelFiltros;
        private Label label1;
        private TextBox txtBuscar;
        private Label label2;
        private ComboBox cmbEstado;
        private Panel panelDatos;
        private TextBox txtNombre;
        private Label label3;
        private Panel panelAcciones;
        private Button btnBajaAlta;
        private Button btnGuardar;
        private Button btnNuevo;
        private DataGridView dgvMarcas;
        private Label label4;
    }
}