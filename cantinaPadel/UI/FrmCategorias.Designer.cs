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
            panelFiltros = new System.Windows.Forms.Panel();
            cmbEstado = new System.Windows.Forms.ComboBox();
            label2 = new System.Windows.Forms.Label();
            txtBuscar = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            panelDatos = new System.Windows.Forms.Panel();
            label4 = new System.Windows.Forms.Label();
            txtPorcentajeGanancia = new System.Windows.Forms.TextBox();
            lblPorcentajeGanancia = new System.Windows.Forms.Label();
            txtNombre = new System.Windows.Forms.TextBox();
            btnGuardar = new System.Windows.Forms.Button();
            label3 = new System.Windows.Forms.Label();
            btnBajaAlta = new System.Windows.Forms.Button();
            btnNuevo = new System.Windows.Forms.Button();
            dgvCategorias = new System.Windows.Forms.DataGridView();
            panel1 = new System.Windows.Forms.Panel();
            contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(components);
            panelFiltros.SuspendLayout();
            panelDatos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCategorias).BeginInit();
            panel1.SuspendLayout();
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
            panelFiltros.Name = "panelFiltros";
            panelFiltros.Size = new System.Drawing.Size(882, 65);
            panelFiltros.TabIndex = 0;
            // 
            // cmbEstado
            // 
            cmbEstado.FormattingEnabled = true;
            cmbEstado.Items.AddRange(new object[] { "Todos", "Activos", "Inactivos" });
            cmbEstado.Location = new System.Drawing.Point(547, 20);
            cmbEstado.Name = "cmbEstado";
            cmbEstado.Size = new System.Drawing.Size(151, 40);
            cmbEstado.TabIndex = 3;
            cmbEstado.SelectedIndexChanged += cmbEstado_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(473, 23);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(89, 32);
            label2.TabIndex = 2;
            label2.Text = "Estado:";
            // 
            // txtBuscar
            // 
            txtBuscar.Location = new System.Drawing.Point(108, 20);
            txtBuscar.MaxLength = 25;
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new System.Drawing.Size(193, 39);
            txtBuscar.TabIndex = 1;
            txtBuscar.TextChanged += txtBuscar_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(47, 23);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(88, 32);
            label1.TabIndex = 0;
            label1.Text = "Buscar:";
            // 
            // panelDatos
            // 
            panelDatos.BackColor = System.Drawing.SystemColors.Info;
            panelDatos.Controls.Add(label4);
            panelDatos.Controls.Add(txtPorcentajeGanancia);
            panelDatos.Controls.Add(lblPorcentajeGanancia);
            panelDatos.Controls.Add(txtNombre);
            panelDatos.Controls.Add(btnGuardar);
            panelDatos.Controls.Add(label3);
            panelDatos.Dock = System.Windows.Forms.DockStyle.Right;
            panelDatos.Location = new System.Drawing.Point(698, 65);
            panelDatos.Name = "panelDatos";
            panelDatos.Size = new System.Drawing.Size(184, 388);
            panelDatos.TabIndex = 1;
            // 
            // label4
            // 
            label4.AutoEllipsis = true;
            label4.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            label4.Location = new System.Drawing.Point(22, 48);
            label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(155, 58);
            label4.TabIndex = 2;
            label4.Text = "Selecciona una categoría para editar.";
            // 
            // txtPorcentajeGanancia
            // 
            txtPorcentajeGanancia.Location = new System.Drawing.Point(29, 238);
            txtPorcentajeGanancia.MaxLength = 4;
            txtPorcentajeGanancia.Name = "txtPorcentajeGanancia";
            txtPorcentajeGanancia.Size = new System.Drawing.Size(125, 39);
            txtPorcentajeGanancia.TabIndex = 2;
            // 
            // lblPorcentajeGanancia
            // 
            lblPorcentajeGanancia.AutoSize = true;
            lblPorcentajeGanancia.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)0));
            lblPorcentajeGanancia.Location = new System.Drawing.Point(15, 209);
            lblPorcentajeGanancia.Name = "lblPorcentajeGanancia";
            lblPorcentajeGanancia.Size = new System.Drawing.Size(154, 32);
            lblPorcentajeGanancia.TabIndex = 3;
            lblPorcentajeGanancia.Text = "% Ganancia:";
            // 
            // txtNombre
            // 
            txtNombre.Location = new System.Drawing.Point(29, 172);
            txtNombre.MaxLength = 25;
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new System.Drawing.Size(125, 39);
            txtNombre.TabIndex = 1;
            // 
            // btnGuardar
            // 
            btnGuardar.BackColor = System.Drawing.Color.White;
            btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnGuardar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)0));
            btnGuardar.Location = new System.Drawing.Point(45, 299);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new System.Drawing.Size(94, 29);
            btnGuardar.TabIndex = 1;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = false;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)0));
            label3.Location = new System.Drawing.Point(15, 138);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(267, 32);
            label3.TabIndex = 0;
            label3.Text = "Nombre de Categoría:";
            // 
            // btnBajaAlta
            // 
            btnBajaAlta.BackColor = System.Drawing.Color.White;
            btnBajaAlta.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnBajaAlta.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)0));
            btnBajaAlta.Location = new System.Drawing.Point(434, 18);
            btnBajaAlta.Name = "btnBajaAlta";
            btnBajaAlta.Size = new System.Drawing.Size(161, 29);
            btnBajaAlta.TabIndex = 2;
            btnBajaAlta.Text = "Activar/ Desactivar";
            btnBajaAlta.UseVisualStyleBackColor = false;
            btnBajaAlta.Click += btnBajaAlta_Click;
            // 
            // btnNuevo
            // 
            btnNuevo.BackColor = System.Drawing.Color.White;
            btnNuevo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnNuevo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)0));
            btnNuevo.Location = new System.Drawing.Point(206, 18);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new System.Drawing.Size(94, 29);
            btnNuevo.TabIndex = 0;
            btnNuevo.Text = "Nuevo";
            btnNuevo.UseVisualStyleBackColor = false;
            btnNuevo.Click += btnNuevo_Click;
            // 
            // dgvCategorias
            // 
            dgvCategorias.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dgvCategorias.BackgroundColor = System.Drawing.Color.White;
            dgvCategorias.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCategorias.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvCategorias.Location = new System.Drawing.Point(0, 65);
            dgvCategorias.Name = "dgvCategorias";
            dgvCategorias.RowHeadersWidth = 51;
            dgvCategorias.Size = new System.Drawing.Size(698, 327);
            dgvCategorias.TabIndex = 3;
            dgvCategorias.SelectionChanged += dgvCategorias_SelectionChanged;
            // 
            // panel1
            // 
            panel1.BackColor = System.Drawing.SystemColors.Info;
            panel1.Controls.Add(btnBajaAlta);
            panel1.Controls.Add(btnNuevo);
            panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel1.Location = new System.Drawing.Point(0, 392);
            panel1.Margin = new System.Windows.Forms.Padding(2);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(698, 61);
            panel1.TabIndex = 3;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // FrmCategorias
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(882, 453);
            Controls.Add(dgvCategorias);
            Controls.Add(panel1);
            Controls.Add(panelDatos);
            Controls.Add(panelFiltros);
            Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
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
        private System.Windows.Forms.DataGridView dgvCategorias;
        private Panel panel1;
        private ContextMenuStrip contextMenuStrip1;
        private Label label4;
    }
}
