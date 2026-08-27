namespace cantinaPadel.UI
{
    partial class FrmHorarios
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
            panelFiltros = new Panel();
            cmbEstado = new ComboBox();
            label1 = new Label();
            cmbFiltroCancha = new ComboBox();
            label2 = new Label();
            panelDatos = new Panel();
            dtpHoraFin = new DateTimePicker();
            label6 = new Label();
            dtpHoraInicio = new DateTimePicker();
            label5 = new Label();
            cmbDiaSemana = new ComboBox();
            label4 = new Label();
            cmbCancha = new ComboBox();
            label3 = new Label();
            btnGuardar = new Button();
            panelAcciones = new Panel();
            btnBajaAlta = new Button();
            btnNuevo = new Button();
            dgvHorarios = new DataGridView();
            panelFiltros.SuspendLayout();
            panelDatos.SuspendLayout();
            panelAcciones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvHorarios).BeginInit();
            SuspendLayout();
            // 
            // panelFiltros
            // 
            panelFiltros.BackColor = Color.Gold;
            panelFiltros.Controls.Add(cmbEstado);
            panelFiltros.Controls.Add(label1);
            panelFiltros.Controls.Add(cmbFiltroCancha);
            panelFiltros.Controls.Add(label2);
            panelFiltros.Dock = DockStyle.Top;
            panelFiltros.Location = new Point(0, 0);
            panelFiltros.Margin = new Padding(2, 2, 2, 2);
            panelFiltros.Name = "panelFiltros";
            panelFiltros.Size = new Size(872, 69);
            panelFiltros.TabIndex = 0;
            // 
            // cmbEstado
            // 
            cmbEstado.FormattingEnabled = true;
            cmbEstado.Items.AddRange(new object[] { "Todos", "Activos", "Inactivos" });
            cmbEstado.Location = new Point(597, 23);
            cmbEstado.Margin = new Padding(2, 2, 2, 2);
            cmbEstado.Name = "cmbEstado";
            cmbEstado.Size = new Size(131, 28);
            cmbEstado.TabIndex = 3;
            cmbEstado.SelectedIndexChanged += cmbEstado_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(514, 26);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(57, 20);
            label1.TabIndex = 2;
            label1.Text = "Estado:";
            // 
            // cmbFiltroCancha
            // 
            cmbFiltroCancha.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFiltroCancha.FormattingEnabled = true;
            cmbFiltroCancha.Location = new Point(176, 26);
            cmbFiltroCancha.Margin = new Padding(2, 2, 2, 2);
            cmbFiltroCancha.Name = "cmbFiltroCancha";
            cmbFiltroCancha.Size = new Size(117, 28);
            cmbFiltroCancha.TabIndex = 1;
            cmbFiltroCancha.SelectedIndexChanged += cmbFiltroCancha_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(112, 29);
            label2.Margin = new Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new Size(60, 20);
            label2.TabIndex = 0;
            label2.Text = "Cancha:";
            // 
            // panelDatos
            // 
            panelDatos.BackColor = SystemColors.Info;
            panelDatos.Controls.Add(dtpHoraFin);
            panelDatos.Controls.Add(label6);
            panelDatos.Controls.Add(dtpHoraInicio);
            panelDatos.Controls.Add(label5);
            panelDatos.Controls.Add(cmbDiaSemana);
            panelDatos.Controls.Add(label4);
            panelDatos.Controls.Add(cmbCancha);
            panelDatos.Controls.Add(label3);
            panelDatos.Controls.Add(btnGuardar);
            panelDatos.Dock = DockStyle.Right;
            panelDatos.Location = new Point(728, 69);
            panelDatos.Margin = new Padding(2, 2, 2, 2);
            panelDatos.Name = "panelDatos";
            panelDatos.Size = new Size(144, 438);
            panelDatos.TabIndex = 1;
            // 
            // dtpHoraFin
            // 
            dtpHoraFin.Location = new Point(12, 296);
            dtpHoraFin.Margin = new Padding(2, 2, 2, 2);
            dtpHoraFin.Name = "dtpHoraFin";
            dtpHoraFin.Size = new Size(112, 27);
            dtpHoraFin.TabIndex = 8;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.Location = new Point(21, 274);
            label6.Margin = new Padding(2, 0, 2, 0);
            label6.Name = "label6";
            label6.Size = new Size(93, 20);
            label6.TabIndex = 7;
            label6.Text = "Hora de Fin:";
            // 
            // dtpHoraInicio
            // 
            dtpHoraInicio.Location = new Point(12, 205);
            dtpHoraInicio.Margin = new Padding(2, 2, 2, 2);
            dtpHoraInicio.Name = "dtpHoraInicio";
            dtpHoraInicio.Size = new Size(112, 27);
            dtpHoraInicio.TabIndex = 6;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(12, 183);
            label5.Margin = new Padding(2, 0, 2, 0);
            label5.Name = "label5";
            label5.Size = new Size(110, 20);
            label5.TabIndex = 5;
            label5.Text = "Hora de Inicio:";
            // 
            // cmbDiaSemana
            // 
            cmbDiaSemana.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDiaSemana.FormattingEnabled = true;
            cmbDiaSemana.Location = new Point(12, 130);
            cmbDiaSemana.Margin = new Padding(2, 2, 2, 2);
            cmbDiaSemana.Name = "cmbDiaSemana";
            cmbDiaSemana.Size = new Size(112, 28);
            cmbDiaSemana.TabIndex = 4;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.Location = new Point(4, 108);
            label4.Margin = new Padding(2, 0, 2, 0);
            label4.Name = "label4";
            label4.Size = new Size(132, 20);
            label4.TabIndex = 3;
            label4.Text = "Día de la Semana:";
            // 
            // cmbCancha
            // 
            cmbCancha.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCancha.FormattingEnabled = true;
            cmbCancha.Location = new Point(14, 52);
            cmbCancha.Margin = new Padding(2, 2, 2, 2);
            cmbCancha.Name = "cmbCancha";
            cmbCancha.Size = new Size(112, 28);
            cmbCancha.TabIndex = 1;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(34, 30);
            label3.Margin = new Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new Size(63, 20);
            label3.TabIndex = 0;
            label3.Text = "Cancha:";
            // 
            // btnGuardar
            // 
            btnGuardar.BackColor = Color.White;
            btnGuardar.FlatStyle = FlatStyle.Flat;
            btnGuardar.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnGuardar.Location = new Point(21, 373);
            btnGuardar.Margin = new Padding(2, 2, 2, 2);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(105, 41);
            btnGuardar.TabIndex = 9;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = false;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // panelAcciones
            // 
            panelAcciones.BackColor = SystemColors.Info;
            panelAcciones.Controls.Add(btnBajaAlta);
            panelAcciones.Controls.Add(btnNuevo);
            panelAcciones.Dock = DockStyle.Bottom;
            panelAcciones.Location = new Point(0, 414);
            panelAcciones.Margin = new Padding(2, 2, 2, 2);
            panelAcciones.Name = "panelAcciones";
            panelAcciones.Size = new Size(728, 93);
            panelAcciones.TabIndex = 2;
            // 
            // btnBajaAlta
            // 
            btnBajaAlta.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBajaAlta.BackColor = Color.White;
            btnBajaAlta.FlatStyle = FlatStyle.Flat;
            btnBajaAlta.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnBajaAlta.Location = new Point(420, 24);
            btnBajaAlta.Margin = new Padding(2, 2, 2, 2);
            btnBajaAlta.Name = "btnBajaAlta";
            btnBajaAlta.Size = new Size(161, 43);
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
            btnNuevo.Location = new Point(176, 24);
            btnNuevo.Margin = new Padding(2, 2, 2, 2);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(161, 46);
            btnNuevo.TabIndex = 0;
            btnNuevo.Text = "Nuevo";
            btnNuevo.UseVisualStyleBackColor = false;
            btnNuevo.Click += btnNuevo_Click;
            // 
            // dgvHorarios
            // 
            dgvHorarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHorarios.BackgroundColor = Color.White;
            dgvHorarios.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvHorarios.Dock = DockStyle.Fill;
            dgvHorarios.Location = new Point(0, 69);
            dgvHorarios.Margin = new Padding(2, 2, 2, 2);
            dgvHorarios.Name = "dgvHorarios";
            dgvHorarios.RowHeadersWidth = 51;
            dgvHorarios.Size = new Size(728, 345);
            dgvHorarios.TabIndex = 3;
            dgvHorarios.SelectionChanged += dgvHorarios_SelectionChanged;
            // 
            // FrmHorarios
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(872, 507);
            Controls.Add(dgvHorarios);
            Controls.Add(panelAcciones);
            Controls.Add(panelDatos);
            Controls.Add(panelFiltros);
            Name = "FrmHorarios";
            Text = "Gestión de Horarios de Cancha";
            Load += FrmHorarios_Load;
            panelFiltros.ResumeLayout(false);
            panelFiltros.PerformLayout();
            panelDatos.ResumeLayout(false);
            panelDatos.PerformLayout();
            panelAcciones.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvHorarios).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelFiltros;
        private Label label2;
        private ComboBox cmbFiltroCancha;
        private Label label1;
        private ComboBox cmbEstado;
        private Panel panelDatos;
        private Label label3;
        private ComboBox cmbCancha;
        private Label label4;
        private ComboBox cmbDiaSemana;
        private Label label5;
        private DateTimePicker dtpHoraInicio;
        private Label label6;
        private DateTimePicker dtpHoraFin;
        private Button btnGuardar;
        private Panel panelAcciones;
        private Button btnBajaAlta;
        private Button btnNuevo;
        private System.Windows.Forms.DataGridView dgvHorarios;
    }
}