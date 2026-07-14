namespace cantinaPadel.UI
{
    partial class FrmCategorias
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
            components = new System.ComponentModel.Container();
            panelFiltros = new Panel();
            cmbEstado = new ComboBox();
            label2 = new Label();
            txtBuscar = new TextBox();
            label1 = new Label();
            panelDatos = new Panel();
            txtNombre = new TextBox();
            btnGuardar = new Button();
            label3 = new Label();
            btnBajaAlta = new Button();
            btnNuevo = new Button();
            dgvCategorias = new DataGridView();
            panel1 = new Panel();
            contextMenuStrip1 = new ContextMenuStrip(components);
            label4 = new Label();
            panelFiltros.SuspendLayout();
            panelDatos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCategorias).BeginInit();
            panel1.SuspendLayout();
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
            cmbEstado.Location = new Point(889, 32);
            cmbEstado.Margin = new Padding(5);
            cmbEstado.Name = "cmbEstado";
            cmbEstado.Size = new Size(243, 40);
            cmbEstado.TabIndex = 3;
            cmbEstado.SelectedIndexChanged += cmbEstado_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(769, 37);
            label2.Margin = new Padding(5, 0, 5, 0);
            label2.Name = "label2";
            label2.Size = new Size(89, 32);
            label2.TabIndex = 2;
            label2.Text = "Estado:";
            // 
            // txtBuscar
            // 
            txtBuscar.Location = new Point(176, 32);
            txtBuscar.Margin = new Padding(5);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new Size(311, 39);
            txtBuscar.TabIndex = 1;
            txtBuscar.TextChanged += txtBuscar_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(76, 37);
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
            txtNombre.Location = new Point(47, 275);
            txtNombre.Margin = new Padding(5);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(201, 39);
            txtNombre.TabIndex = 1;
            // 
            // btnGuardar
            // 
            btnGuardar.BackColor = Color.White;
            btnGuardar.FlatStyle = FlatStyle.Flat;
            btnGuardar.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnGuardar.Location = new Point(73, 388);
            btnGuardar.Margin = new Padding(5);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(153, 46);
            btnGuardar.TabIndex = 1;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = false;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(24, 221);
            label3.Margin = new Padding(5, 0, 5, 0);
            label3.Name = "label3";
            label3.Size = new Size(267, 32);
            label3.TabIndex = 0;
            label3.Text = "Nombre de Categoría:";
            // 
            // btnBajaAlta
            // 
            btnBajaAlta.BackColor = Color.White;
            btnBajaAlta.FlatStyle = FlatStyle.Flat;
            btnBajaAlta.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnBajaAlta.Location = new Point(705, 28);
            btnBajaAlta.Margin = new Padding(5);
            btnBajaAlta.Name = "btnBajaAlta";
            btnBajaAlta.Size = new Size(262, 46);
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
            btnNuevo.Location = new Point(334, 28);
            btnNuevo.Margin = new Padding(5);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(153, 46);
            btnNuevo.TabIndex = 0;
            btnNuevo.Text = "Nuevo";
            btnNuevo.UseVisualStyleBackColor = false;
            btnNuevo.Click += btnNuevo_Click;
            // 
            // dgvCategorias
            // 
            dgvCategorias.BackgroundColor = Color.White;
            dgvCategorias.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCategorias.Dock = DockStyle.Fill;
            dgvCategorias.Location = new Point(0, 104);
            dgvCategorias.Margin = new Padding(5);
            dgvCategorias.Name = "dgvCategorias";
            dgvCategorias.RowHeadersWidth = 51;
            dgvCategorias.Size = new Size(1134, 524);
            dgvCategorias.TabIndex = 3;
            dgvCategorias.SelectionChanged += dgvCategorias_SelectionChanged;
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.Info;
            panel1.Controls.Add(btnBajaAlta);
            panel1.Controls.Add(btnNuevo);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 628);
            panel1.Name = "panel1";
            panel1.Size = new Size(1134, 97);
            panel1.TabIndex = 3;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new Size(32, 32);
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(61, 4);
            // 
            // label4
            // 
            label4.AutoEllipsis = true;
            label4.ForeColor = SystemColors.ControlDarkDark;
            label4.Location = new Point(35, 76);
            label4.Name = "label4";
            label4.Size = new Size(252, 92);
            label4.TabIndex = 2;
            label4.Text = "Selecciona una categoría para editar.";
            // 
            // FrmCategorias
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1433, 725);
            Controls.Add(dgvCategorias);
            Controls.Add(panel1);
            Controls.Add(panelDatos);
            Controls.Add(panelFiltros);
            Margin = new Padding(5);
            Name = "FrmCategorias";
            Text = "Gestión de Categorías";
            Load += FrmCategorias_Load;
            panelFiltros.ResumeLayout(false);
            panelFiltros.PerformLayout();
            panelDatos.ResumeLayout(false);
            panelDatos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCategorias).EndInit();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panelFiltros;
        private ComboBox cmbEstado;
        private Label label2;
        private TextBox txtBuscar;
        private Label label1;
        private Panel panelDatos;
        private TextBox txtNombre;
        private Label label3;
        private Button btnBajaAlta;
        private Button btnGuardar;
        private Button btnNuevo;
        private DataGridView dgvCategorias;
        private Panel panel1;
        private ContextMenuStrip contextMenuStrip1;
        private Label label4;
    }
}