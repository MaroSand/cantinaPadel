namespace cantinaPadel.UI
{
    partial class FrmCanchas
    {
        private System.ComponentModel.IContainer components = null;

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
            panelDatos = new System.Windows.Forms.Panel();
            label4 = new System.Windows.Forms.Label();
            txtPrecioHora = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            txtNombre = new System.Windows.Forms.TextBox();
            btnGuardar = new System.Windows.Forms.Button();
            label3 = new System.Windows.Forms.Label();
            panelAcciones = new System.Windows.Forms.Panel();
            btnBajaAlta = new System.Windows.Forms.Button();
            btnNuevo = new System.Windows.Forms.Button();
            dgvCanchas = new System.Windows.Forms.DataGridView();
            panelFiltros.SuspendLayout();
            panelDatos.SuspendLayout();
            panelAcciones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCanchas).BeginInit();
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
            cmbEstado.Location = new System.Drawing.Point(507, 22);
            cmbEstado.Name = "cmbEstado";
            cmbEstado.Size = new System.Drawing.Size(151, 40);
            cmbEstado.TabIndex = 3;
            cmbEstado.SelectedIndexChanged += cmbEstado_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(444, 25);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(89, 32);
            label2.TabIndex = 2;
            label2.Text = "Estado:";
            // 
            // txtBuscar
            // 
            txtBuscar.Location = new System.Drawing.Point(115, 23);
            txtBuscar.MaxLength = 100;
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new System.Drawing.Size(187, 39);
            txtBuscar.TabIndex = 1;
            txtBuscar.TextChanged += txtBuscar_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(54, 25);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(88, 32);
            label1.TabIndex = 0;
            label1.Text = "Buscar:";
            // 
            // panelDatos
            // 
            panelDatos.BackColor = System.Drawing.SystemColors.Info;
            panelDatos.Controls.Add(label4);
            panelDatos.Controls.Add(txtPrecioHora);
            panelDatos.Controls.Add(label5);
            panelDatos.Controls.Add(txtNombre);
            panelDatos.Controls.Add(btnGuardar);
            panelDatos.Controls.Add(label3);
            panelDatos.Dock = System.Windows.Forms.DockStyle.Right;
            panelDatos.Location = new System.Drawing.Point(682, 65);
            panelDatos.Name = "panelDatos";
            panelDatos.Size = new System.Drawing.Size(200, 388);
            panelDatos.TabIndex = 1;
            // 
            // label4
            // 
            label4.AutoEllipsis = true;
            label4.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            label4.Location = new System.Drawing.Point(22, 30);
            label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(145, 46);
            label4.TabIndex = 4;
            label4.Text = "Selecciona una cancha para editar.";
            // 
            // txtPrecioHora
            // 
            txtPrecioHora.Location = new System.Drawing.Point(21, 222);
            txtPrecioHora.MaxLength = 12;
            txtPrecioHora.Name = "txtPrecioHora";
            txtPrecioHora.Size = new System.Drawing.Size(137, 39);
            txtPrecioHora.TabIndex = 2;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)0));
            label5.Location = new System.Drawing.Point(22, 189);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(203, 32);
            label5.TabIndex = 3;
            label5.Text = "Precio por Hora:";
            // 
            // txtNombre
            // 
            txtNombre.Location = new System.Drawing.Point(21, 133);
            txtNombre.MaxLength = 40;
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new System.Drawing.Size(137, 39);
            txtNombre.TabIndex = 1;
            // 
            // btnGuardar
            // 
            btnGuardar.BackColor = System.Drawing.Color.White;
            btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnGuardar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)0));
            btnGuardar.Location = new System.Drawing.Point(45, 279);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new System.Drawing.Size(89, 29);
            btnGuardar.TabIndex = 5;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = false;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)0));
            label3.Location = new System.Drawing.Point(21, 101);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(239, 32);
            label3.TabIndex = 0;
            label3.Text = "Nombre de Cancha:";
            // 
            // panelAcciones
            // 
            panelAcciones.BackColor = System.Drawing.SystemColors.Info;
            panelAcciones.Controls.Add(btnBajaAlta);
            panelAcciones.Controls.Add(btnNuevo);
            panelAcciones.Dock = System.Windows.Forms.DockStyle.Bottom;
            panelAcciones.Location = new System.Drawing.Point(0, 396);
            panelAcciones.Name = "panelAcciones";
            panelAcciones.Size = new System.Drawing.Size(682, 57);
            panelAcciones.TabIndex = 2;
            // 
            // btnBajaAlta
            // 
            btnBajaAlta.BackColor = System.Drawing.Color.White;
            btnBajaAlta.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnBajaAlta.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)0));
            btnBajaAlta.Location = new System.Drawing.Point(429, 16);
            btnBajaAlta.Name = "btnBajaAlta";
            btnBajaAlta.Size = new System.Drawing.Size(173, 29);
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
            btnNuevo.Location = new System.Drawing.Point(181, 16);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new System.Drawing.Size(94, 29);
            btnNuevo.TabIndex = 0;
            btnNuevo.Text = "Nuevo";
            btnNuevo.UseVisualStyleBackColor = false;
            btnNuevo.Click += btnNuevo_Click;
            // 
            // dgvCanchas
            // 
            dgvCanchas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dgvCanchas.BackgroundColor = System.Drawing.Color.White;
            dgvCanchas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCanchas.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvCanchas.Location = new System.Drawing.Point(0, 65);
            dgvCanchas.Name = "dgvCanchas";
            dgvCanchas.RowHeadersWidth = 51;
            dgvCanchas.Size = new System.Drawing.Size(682, 331);
            dgvCanchas.TabIndex = 3;
            dgvCanchas.SelectionChanged += dgvCanchas_SelectionChanged;
            // 
            // FrmCanchas
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(882, 453);
            Controls.Add(dgvCanchas);
            Controls.Add(panelAcciones);
            Controls.Add(panelDatos);
            Controls.Add(panelFiltros);
            Margin = new System.Windows.Forms.Padding(5);
            Text = "Gestión de Canchas";
            Load += FrmCanchas_Load;
            panelFiltros.ResumeLayout(false);
            panelFiltros.PerformLayout();
            panelDatos.ResumeLayout(false);
            panelDatos.PerformLayout();
            panelAcciones.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvCanchas).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelFiltros;
        private Label label1;
        private TextBox txtBuscar;
        private Label label2;
        private ComboBox cmbEstado;
        private System.Windows.Forms.Panel panelDatos;
        private TextBox txtNombre;
        private Label label3;
        private TextBox txtPrecioHora;
        private Label label5;
        private System.Windows.Forms.Panel panelAcciones;
        private Button btnBajaAlta;
        private Button btnGuardar;
        private Button btnNuevo;
        private System.Windows.Forms.DataGridView dgvCanchas;
        private Label label4;
    }
}