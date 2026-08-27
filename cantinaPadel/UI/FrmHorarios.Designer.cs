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
            panelFiltros = new System.Windows.Forms.Panel();
            cmbEstado = new System.Windows.Forms.ComboBox();
            label1 = new System.Windows.Forms.Label();
            cmbFiltroCancha = new System.Windows.Forms.ComboBox();
            label2 = new System.Windows.Forms.Label();
            panelDatos = new System.Windows.Forms.Panel();
            dtpHoraFin = new System.Windows.Forms.DateTimePicker();
            label6 = new System.Windows.Forms.Label();
            dtpHoraInicio = new System.Windows.Forms.DateTimePicker();
            label5 = new System.Windows.Forms.Label();
            cmbDiaSemana = new System.Windows.Forms.ComboBox();
            label4 = new System.Windows.Forms.Label();
            cmbCancha = new System.Windows.Forms.ComboBox();
            label3 = new System.Windows.Forms.Label();
            btnGuardar = new System.Windows.Forms.Button();
            panelAcciones = new System.Windows.Forms.Panel();
            btnBajaAlta = new System.Windows.Forms.Button();
            btnNuevo = new System.Windows.Forms.Button();
            dgvHorarios = new System.Windows.Forms.DataGridView();
            panelFiltros.SuspendLayout();
            panelDatos.SuspendLayout();
            panelAcciones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvHorarios).BeginInit();
            SuspendLayout();
            // 
            // panelFiltros
            // 
            panelFiltros.BackColor = System.Drawing.Color.Gold;
            panelFiltros.Controls.Add(cmbEstado);
            panelFiltros.Controls.Add(label1);
            panelFiltros.Controls.Add(cmbFiltroCancha);
            panelFiltros.Controls.Add(label2);
            panelFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            panelFiltros.Location = new System.Drawing.Point(0, 0);
            panelFiltros.Name = "panelFiltros";
            panelFiltros.Size = new System.Drawing.Size(950, 65);
            panelFiltros.TabIndex = 0;
            // 
            // cmbEstado
            // 
            cmbEstado.FormattingEnabled = true;
            cmbEstado.Items.AddRange(new object[] { "Todos", "Activos", "Inactivos" });
            cmbEstado.Location = new System.Drawing.Point(600, 22);
            cmbEstado.Name = "cmbEstado";
            cmbEstado.Size = new System.Drawing.Size(151, 40);
            cmbEstado.TabIndex = 3;
            cmbEstado.SelectedIndexChanged += cmbEstado_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(537, 25);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(89, 32);
            label1.TabIndex = 2;
            label1.Text = "Estado:";
            // 
            // cmbFiltroCancha
            // 
            cmbFiltroCancha.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbFiltroCancha.FormattingEnabled = true;
            cmbFiltroCancha.Location = new System.Drawing.Point(115, 23);
            cmbFiltroCancha.Name = "cmbFiltroCancha";
            cmbFiltroCancha.Size = new System.Drawing.Size(187, 40);
            cmbFiltroCancha.TabIndex = 1;
            cmbFiltroCancha.SelectedIndexChanged += cmbFiltroCancha_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(54, 25);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(97, 32);
            label2.TabIndex = 0;
            label2.Text = "Cancha:";
            // 
            // panelDatos
            // 
            panelDatos.BackColor = System.Drawing.SystemColors.Info;
            panelDatos.Controls.Add(dtpHoraFin);
            panelDatos.Controls.Add(label6);
            panelDatos.Controls.Add(dtpHoraInicio);
            panelDatos.Controls.Add(label5);
            panelDatos.Controls.Add(cmbDiaSemana);
            panelDatos.Controls.Add(label4);
            panelDatos.Controls.Add(cmbCancha);
            panelDatos.Controls.Add(label3);
            panelDatos.Controls.Add(btnGuardar);
            panelDatos.Dock = System.Windows.Forms.DockStyle.Right;
            panelDatos.Location = new System.Drawing.Point(716, 65);
            panelDatos.Name = "panelDatos";
            panelDatos.Size = new System.Drawing.Size(234, 388);
            panelDatos.TabIndex = 1;
            // 
            // dtpHoraFin
            // 
            dtpHoraFin.Location = new System.Drawing.Point(22, 300);
            dtpHoraFin.Name = "dtpHoraFin";
            dtpHoraFin.Size = new System.Drawing.Size(180, 39);
            dtpHoraFin.TabIndex = 8;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)0));
            label6.Location = new System.Drawing.Point(22, 275);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(153, 32);
            label6.TabIndex = 7;
            label6.Text = "Hora de Fin:";
            // 
            // dtpHoraInicio
            // 
            dtpHoraInicio.Location = new System.Drawing.Point(22, 230);
            dtpHoraInicio.Name = "dtpHoraInicio";
            dtpHoraInicio.Size = new System.Drawing.Size(180, 39);
            dtpHoraInicio.TabIndex = 6;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)0));
            label5.Location = new System.Drawing.Point(22, 205);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(183, 32);
            label5.TabIndex = 5;
            label5.Text = "Hora de Inicio:";
            // 
            // cmbDiaSemana
            // 
            cmbDiaSemana.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbDiaSemana.FormattingEnabled = true;
            cmbDiaSemana.Location = new System.Drawing.Point(22, 160);
            cmbDiaSemana.Name = "cmbDiaSemana";
            cmbDiaSemana.Size = new System.Drawing.Size(180, 40);
            cmbDiaSemana.TabIndex = 4;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)0));
            label4.Location = new System.Drawing.Point(22, 135);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(217, 32);
            label4.TabIndex = 3;
            label4.Text = "Día de la Semana:";
            // 
            // cmbCancha
            // 
            cmbCancha.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbCancha.FormattingEnabled = true;
            cmbCancha.Location = new System.Drawing.Point(22, 90);
            cmbCancha.Name = "cmbCancha";
            cmbCancha.Size = new System.Drawing.Size(180, 40);
            cmbCancha.TabIndex = 1;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)0));
            label3.Location = new System.Drawing.Point(22, 65);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(103, 32);
            label3.TabIndex = 0;
            label3.Text = "Cancha:";
            // 
            // btnGuardar
            // 
            btnGuardar.BackColor = System.Drawing.Color.White;
            btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnGuardar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)0));
            btnGuardar.Location = new System.Drawing.Point(67, 345);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new System.Drawing.Size(89, 29);
            btnGuardar.TabIndex = 9;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = false;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // panelAcciones
            // 
            panelAcciones.BackColor = System.Drawing.SystemColors.Info;
            panelAcciones.Controls.Add(btnBajaAlta);
            panelAcciones.Controls.Add(btnNuevo);
            panelAcciones.Dock = System.Windows.Forms.DockStyle.Bottom;
            panelAcciones.Location = new System.Drawing.Point(0, 396);
            panelAcciones.Name = "panelAcciones";
            panelAcciones.Size = new System.Drawing.Size(716, 57);
            panelAcciones.TabIndex = 2;
            // 
            // btnBajaAlta
            // 
            btnBajaAlta.BackColor = System.Drawing.Color.White;
            btnBajaAlta.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnBajaAlta.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)0));
            btnBajaAlta.Location = new System.Drawing.Point(447, 16);
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
            btnNuevo.Location = new System.Drawing.Point(199, 16);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new System.Drawing.Size(94, 29);
            btnNuevo.TabIndex = 0;
            btnNuevo.Text = "Nuevo";
            btnNuevo.UseVisualStyleBackColor = false;
            btnNuevo.Click += btnNuevo_Click;
            // 
            // dgvHorarios
            // 
            dgvHorarios.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dgvHorarios.BackgroundColor = System.Drawing.Color.White;
            dgvHorarios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvHorarios.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvHorarios.Location = new System.Drawing.Point(0, 65);
            dgvHorarios.Name = "dgvHorarios";
            dgvHorarios.RowHeadersWidth = 51;
            dgvHorarios.Size = new System.Drawing.Size(716, 331);
            dgvHorarios.TabIndex = 3;
            dgvHorarios.SelectionChanged += dgvHorarios_SelectionChanged;
            // 
            // FrmHorarios
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(950, 453);
            Controls.Add(dgvHorarios);
            Controls.Add(panelAcciones);
            Controls.Add(panelDatos);
            Controls.Add(panelFiltros);
            Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
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