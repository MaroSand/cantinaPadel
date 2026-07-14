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
            panelFiltros.BackColor = Color.Gold;
            panelFiltros.Controls.Add(cmbEstado);
            panelFiltros.Controls.Add(label2);
            panelFiltros.Controls.Add(txtBuscarNombre);
            panelFiltros.Controls.Add(label1);
            panelFiltros.Dock = DockStyle.Top;
            panelFiltros.Location = new Point(0, 0);
            panelFiltros.Margin = new Padding(5, 5, 5, 5);
            panelFiltros.Name = "panelFiltros";
            panelFiltros.Size = new Size(1433, 96);
            panelFiltros.TabIndex = 0;
            // 
            // cmbEstado
            // 
            cmbEstado.FormattingEnabled = true;
            cmbEstado.Items.AddRange(new object[] { "Todos", "Activos", "Inactivos" });
            cmbEstado.Location = new Point(988, 30);
            cmbEstado.Margin = new Padding(5, 5, 5, 5);
            cmbEstado.Name = "cmbEstado";
            cmbEstado.Size = new Size(243, 40);
            cmbEstado.TabIndex = 3;
            cmbEstado.SelectedIndexChanged += cmbEstado_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(848, 30);
            label2.Margin = new Padding(5, 0, 5, 0);
            label2.Name = "label2";
            label2.Size = new Size(89, 32);
            label2.TabIndex = 2;
            label2.Text = "Estado:";
            // 
            // txtBuscarNombre
            // 
            txtBuscarNombre.Location = new Point(375, 26);
            txtBuscarNombre.Margin = new Padding(5, 5, 5, 5);
            txtBuscarNombre.MaxLength = 30;
            txtBuscarNombre.Name = "txtBuscarNombre";
            txtBuscarNombre.Size = new Size(384, 39);
            txtBuscarNombre.TabIndex = 1;
            txtBuscarNombre.TextChanged += txtBuscarNombre_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(34, 30);
            label1.Margin = new Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new Size(323, 32);
            label1.TabIndex = 0;
            label1.Text = "Buscar por Nombre/Apellido:";
            // 
            // panelAcciones
            // 
            panelAcciones.BackColor = SystemColors.Info;
            panelAcciones.Controls.Add(btnBajaLogica);
            panelAcciones.Controls.Add(btnModificar);
            panelAcciones.Controls.Add(btnNuevo);
            panelAcciones.Dock = DockStyle.Bottom;
            panelAcciones.Location = new Point(0, 645);
            panelAcciones.Margin = new Padding(5, 5, 5, 5);
            panelAcciones.Name = "panelAcciones";
            panelAcciones.Size = new Size(1433, 80);
            panelAcciones.TabIndex = 1;
            // 
            // btnBajaLogica
            // 
            btnBajaLogica.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnBajaLogica.BackColor = Color.White;
            btnBajaLogica.FlatStyle = FlatStyle.Flat;
            btnBajaLogica.Location = new Point(1008, 14);
            btnBajaLogica.Margin = new Padding(5, 5, 5, 5);
            btnBajaLogica.Name = "btnBajaLogica";
            btnBajaLogica.Size = new Size(226, 46);
            btnBajaLogica.TabIndex = 2;
            btnBajaLogica.Text = "Activar/ Desactivar";
            btnBajaLogica.UseVisualStyleBackColor = false;
            btnBajaLogica.Click += btnCambiarEstado_Click;
            // 
            // btnModificar
            // 
            btnModificar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnModificar.BackColor = Color.White;
            btnModificar.FlatStyle = FlatStyle.Flat;
            btnModificar.Location = new Point(655, 14);
            btnModificar.Margin = new Padding(5, 5, 5, 5);
            btnModificar.Name = "btnModificar";
            btnModificar.Size = new Size(180, 46);
            btnModificar.TabIndex = 1;
            btnModificar.Text = "Modificar";
            btnModificar.UseVisualStyleBackColor = false;
            btnModificar.Click += btnModificar_Click;
            // 
            // btnNuevo
            // 
            btnNuevo.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnNuevo.BackColor = Color.White;
            btnNuevo.FlatStyle = FlatStyle.Flat;
            btnNuevo.Location = new Point(270, 14);
            btnNuevo.Margin = new Padding(5, 5, 5, 5);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(224, 46);
            btnNuevo.TabIndex = 0;
            btnNuevo.Text = "Nuevo Empleado";
            btnNuevo.UseVisualStyleBackColor = false;
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
            dgvEmpleados.Location = new Point(0, 96);
            dgvEmpleados.Margin = new Padding(5, 5, 5, 5);
            dgvEmpleados.Name = "dgvEmpleados";
            dgvEmpleados.RowHeadersVisible = false;
            dgvEmpleados.RowHeadersWidth = 51;
            dgvEmpleados.Size = new Size(1433, 549);
            dgvEmpleados.TabIndex = 2;
            dgvEmpleados.SelectionChanged += dgvEmpleados_SelectionChanged;
            // 
            // FrmListadoEmpleados
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1433, 725);
            Controls.Add(dgvEmpleados);
            Controls.Add(panelAcciones);
            Controls.Add(panelFiltros);
            Margin = new Padding(5, 5, 5, 5);
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