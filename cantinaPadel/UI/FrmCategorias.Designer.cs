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
            label4 = new Label();
            txtPorcentajeGanancia = new TextBox();
            lblPorcentajeGanancia = new Label();
            txtNombre = new TextBox();
            btnGuardar = new Button();
            label3 = new Label();
            btnBajaAlta = new Button();
            btnNuevo = new Button();
            dgvCategorias = new DataGridView();
            panel1 = new Panel();
            contextMenuStrip1 = new ContextMenuStrip(components);
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
            panelFiltros.Name = "panelFiltros";
            panelFiltros.Size = new Size(882, 65);
            panelFiltros.TabIndex = 0;
            // 
            // cmbEstado
            // 
            cmbEstado.FormattingEnabled = true;
            cmbEstado.Items.AddRange(new object[] { "Todos", "Activos", "Inactivos" });
            cmbEstado.Location = new Point(547, 20);
            cmbEstado.Name = "cmbEstado";
            cmbEstado.Size = new Size(151, 28);
            cmbEstado.TabIndex = 3;
            cmbEstado.SelectedIndexChanged += cmbEstado_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(473, 23);
            label2.Name = "label2";
            label2.Size = new Size(57, 20);
            label2.TabIndex = 2;
            label2.Text = "Estado:";
            // 
            // txtBuscar
            // 
            txtBuscar.Location = new Point(108, 20);
            txtBuscar.MaxLength = 25;
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new Size(193, 27);
            txtBuscar.TabIndex = 1;
            txtBuscar.TextChanged += txtBuscar_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(47, 23);
            label1.Name = "label1";
            label1.Size = new Size(55, 20);
            label1.TabIndex = 0;
            label1.Text = "Buscar:";
            // 
            // panelDatos
            // 
            panelDatos.BackColor = SystemColors.Info;
            panelDatos.Controls.Add(label4);
            panelDatos.Controls.Add(txtPorcentajeGanancia);
            panelDatos.Controls.Add(lblPorcentajeGanancia);
            panelDatos.Controls.Add(txtNombre);
            panelDatos.Controls.Add(btnGuardar);
            panelDatos.Controls.Add(label3);
            panelDatos.Dock = DockStyle.Right;
            panelDatos.Location = new Point(698, 65);
            panelDatos.Name = "panelDatos";
            panelDatos.Size = new Size(184, 388);
            panelDatos.TabIndex = 1;
            // 
            // label4
            // 
            label4.AutoEllipsis = true;
            label4.ForeColor = SystemColors.ControlDarkDark;
            label4.Location = new Point(22, 48);
            label4.Margin = new Padding(2, 0, 2, 0);
            label4.Name = "label4";
            label4.Size = new Size(155, 58);
            label4.TabIndex = 2;
            label4.Text = "Selecciona una categoría para editar.";
            // 
            // txtPorcentajeGanancia
            // 
            txtPorcentajeGanancia.Location = new Point(29, 238);
            txtPorcentajeGanancia.MaxLength = 4;
            txtPorcentajeGanancia.Name = "txtPorcentajeGanancia";
            txtPorcentajeGanancia.Size = new Size(125, 27);
            txtPorcentajeGanancia.TabIndex = 2;
            // 
            // lblPorcentajeGanancia
            // 
            lblPorcentajeGanancia.AutoSize = true;
            lblPorcentajeGanancia.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblPorcentajeGanancia.Location = new Point(15, 209);
            lblPorcentajeGanancia.Name = "lblPorcentajeGanancia";
            lblPorcentajeGanancia.Size = new Size(94, 20);
            lblPorcentajeGanancia.TabIndex = 3;
            lblPorcentajeGanancia.Text = "% Ganancia:";
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(29, 172);
            txtNombre.MaxLength = 25;
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(125, 27);
            txtNombre.TabIndex = 1;
            // 
            // btnGuardar
            // 
            btnGuardar.BackColor = Color.White;
            btnGuardar.FlatStyle = FlatStyle.Flat;
            btnGuardar.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnGuardar.Location = new Point(45, 299);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(94, 29);
            btnGuardar.TabIndex = 1;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = false;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(15, 138);
            label3.Name = "label3";
            label3.Size = new Size(163, 20);
            label3.TabIndex = 0;
            label3.Text = "Nombre de Categoría:";
            // 
            // btnBajaAlta
            // 
            btnBajaAlta.BackColor = Color.White;
            btnBajaAlta.FlatStyle = FlatStyle.Flat;
            btnBajaAlta.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnBajaAlta.Location = new Point(434, 18);
            btnBajaAlta.Name = "btnBajaAlta";
            btnBajaAlta.Size = new Size(161, 29);
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
            btnNuevo.Location = new Point(206, 18);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(94, 29);
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
            dgvCategorias.Location = new Point(0, 65);
            dgvCategorias.Name = "dgvCategorias";
            dgvCategorias.RowHeadersWidth = 51;
            dgvCategorias.Size = new Size(698, 327);
            dgvCategorias.TabIndex = 3;
            dgvCategorias.SelectionChanged += dgvCategorias_SelectionChanged;
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.Info;
            panel1.Controls.Add(btnBajaAlta);
            panel1.Controls.Add(btnNuevo);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 392);
            panel1.Margin = new Padding(2, 2, 2, 2);
            panel1.Name = "panel1";
            panel1.Size = new Size(698, 61);
            panel1.TabIndex = 3;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new Size(32, 32);
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(61, 4);
            // 
            // FrmCategorias
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(882, 453);
            Controls.Add(dgvCategorias);
            Controls.Add(panel1);
            Controls.Add(panelDatos);
            Controls.Add(panelFiltros);
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
        private TextBox txtPorcentajeGanancia;
        private Label lblPorcentajeGanancia;
        private Button btnBajaAlta;
        private Button btnGuardar;
        private Button btnNuevo;
        private DataGridView dgvCategorias;
        private Panel panel1;
        private ContextMenuStrip contextMenuStrip1;
        private Label label4;
    }
}
