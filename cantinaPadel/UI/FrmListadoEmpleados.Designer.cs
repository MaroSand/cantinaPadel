namespace cantinaPadel.UI
{
    partial class FrmListadoEmpleados
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
            dgvEmpleados = new DataGridView();
            panelFiltros.SuspendLayout();
            panelAcciones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvEmpleados).BeginInit();
            SuspendLayout();
            // 
            // panelFiltros
            // 
            panelFiltros.BackColor = SystemColors.Info;
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
            cmbEstado.Items.AddRange(new object[] { "Todos", "", "Activos", "", "Inactivos" });
            cmbEstado.Location = new Point(559, 16);
            cmbEstado.Name = "cmbEstado";
            cmbEstado.Size = new Size(151, 28);
            cmbEstado.TabIndex = 3;
            cmbEstado.SelectedIndexChanged += cmbEstado_SelectedIndexChanged;
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
            txtBuscarNombre.MaxLength = 30;
            txtBuscarNombre.Name = "txtBuscarNombre";
            txtBuscarNombre.Size = new Size(238, 27);
            txtBuscarNombre.TabIndex = 1;
            txtBuscarNombre.TextChanged += txtBuscarNombre_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(21, 19);
            label1.Name = "label1";
            label1.Size = new Size(204, 20);
            label1.TabIndex = 0;
            label1.Text = "Buscar por Nombre/Apellido:";
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
            btnBajaLogica.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnBajaLogica.FlatStyle = FlatStyle.Flat;
            btnBajaLogica.Location = new Point(559, 9);
            btnBajaLogica.Name = "btnBajaLogica";
            btnBajaLogica.Size = new Size(139, 29);
            btnBajaLogica.TabIndex = 2;
            btnBajaLogica.Text = " Dar de Baja/ Alta";
            btnBajaLogica.UseVisualStyleBackColor = true;
            btnBajaLogica.Click += btnCambiarEstado_Click;
            // 
            // btnModificar
            // 
            btnModificar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnModificar.FlatStyle = FlatStyle.Flat;
            btnModificar.Location = new Point(349, 9);
            btnModificar.Name = "btnModificar";
            btnModificar.Size = new Size(111, 29);
            btnModificar.TabIndex = 1;
            btnModificar.Text = "Modificar";
            btnModificar.UseVisualStyleBackColor = true;
            btnModificar.Click += btnModificar_Click;
            // 
            // btnNuevo
            // 
            btnNuevo.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnNuevo.FlatStyle = FlatStyle.Flat;
            btnNuevo.Location = new Point(116, 9);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(138, 29);
            btnNuevo.TabIndex = 0;
            btnNuevo.Text = "Nuevo Empleado";
            btnNuevo.UseVisualStyleBackColor = true;
            btnNuevo.Click += btnNuevo_Click;
            // 
            // dgvEmpleados
            // 
            dgvEmpleados.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvEmpleados.BackgroundColor = Color.White;
            dgvEmpleados.BorderStyle = BorderStyle.None;
            dgvEmpleados.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvEmpleados.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvEmpleados.GridColor = SystemColors.Menu;
            dgvEmpleados.Location = new Point(0, 60);
            dgvEmpleados.Name = "dgvEmpleados";
            dgvEmpleados.RowHeadersVisible = false;
            dgvEmpleados.RowHeadersWidth = 51;
            dgvEmpleados.Size = new Size(800, 340);
            dgvEmpleados.TabIndex = 2;
            dgvEmpleados.SelectionChanged += dgvEmpleados_SelectionChanged;
            // 
            // FrmListadoEmpleados
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(dgvEmpleados);
            Controls.Add(panelAcciones);
            Controls.Add(panelFiltros);
            Name = "FrmListadoEmpleados";
            Text = "Empleados";
            Load += FrmListadoEmpleados_Load;
            panelFiltros.ResumeLayout(false);
            panelFiltros.PerformLayout();
            panelAcciones.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvEmpleados).EndInit();
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
        private DataGridView dgvEmpleados;
    }
}