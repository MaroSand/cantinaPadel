namespace cantinaPadel.UI
{
    partial class FrmCRUDEmpleado
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
            btnGuardar = new Button();
            btnCancelar = new Button();
            label1 = new Label();
            txtDni = new TextBox();
            label2 = new Label();
            txtApellido = new TextBox();
            label3 = new Label();
            txtNombre = new TextBox();
            label4 = new Label();
            txtTelefono = new TextBox();
            label5 = new Label();
            txtUsuario = new TextBox();
            txtContrasena = new TextBox();
            label6 = new Label();
            label7 = new Label();
            cmbRol = new ComboBox();
            SuspendLayout();
            // 
            // btnGuardar
            // 
            btnGuardar.BackColor = Color.ForestGreen;
            btnGuardar.FlatStyle = FlatStyle.Flat;
            btnGuardar.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnGuardar.ForeColor = Color.White;
            btnGuardar.Location = new Point(686, 400);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(94, 29);
            btnGuardar.TabIndex = 0;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = false;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // btnCancelar
            // 
            btnCancelar.BackColor = Color.IndianRed;
            btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCancelar.ForeColor = Color.White;
            btnCancelar.Location = new Point(560, 400);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(94, 29);
            btnCancelar.TabIndex = 1;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = false;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(34, 41);
            label1.Name = "label1";
            label1.Size = new Size(35, 20);
            label1.TabIndex = 2;
            label1.Text = "DNI";
            // 
            // txtDni
            // 
            txtDni.Location = new Point(140, 38);
            txtDni.MaxLength = 8;
            txtDni.Name = "txtDni";
            txtDni.Size = new Size(125, 27);
            txtDni.TabIndex = 3;
            txtDni.KeyPress += txtDni_KeyPress;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(31, 97);
            label2.Name = "label2";
            label2.Size = new Size(66, 20);
            label2.TabIndex = 4;
            label2.Text = "Apellido";
            // 
            // txtApellido
            // 
            txtApellido.Location = new Point(140, 94);
            txtApellido.MaxLength = 30;
            txtApellido.Name = "txtApellido";
            txtApellido.Size = new Size(125, 27);
            txtApellido.TabIndex = 5;
            txtApellido.KeyPress += txtApellido_KeyPress;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(31, 153);
            label3.Name = "label3";
            label3.Size = new Size(64, 20);
            label3.TabIndex = 6;
            label3.Text = "Nombre";
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(140, 150);
            txtNombre.MaxLength = 30;
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(125, 27);
            txtNombre.TabIndex = 7;
            txtNombre.KeyPress += txtNombre_KeyPress;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(31, 209);
            label4.Name = "label4";
            label4.Size = new Size(67, 20);
            label4.TabIndex = 8;
            label4.Text = "Teléfono";
            // 
            // txtTelefono
            // 
            txtTelefono.Location = new Point(140, 206);
            txtTelefono.MaxLength = 10;
            txtTelefono.Name = "txtTelefono";
            txtTelefono.Size = new Size(125, 27);
            txtTelefono.TabIndex = 9;
            txtTelefono.KeyPress += txtTelefono_KeyPress;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(34, 263);
            label5.Name = "label5";
            label5.Size = new Size(59, 20);
            label5.TabIndex = 10;
            label5.Text = "Usuario";
            // 
            // txtUsuario
            // 
            txtUsuario.Location = new Point(140, 263);
            txtUsuario.MaxLength = 15;
            txtUsuario.Name = "txtUsuario";
            txtUsuario.Size = new Size(125, 27);
            txtUsuario.TabIndex = 11;
            // 
            // txtContrasena
            // 
            txtContrasena.Location = new Point(140, 320);
            txtContrasena.MaxLength = 9;
            txtContrasena.Name = "txtContrasena";
            txtContrasena.Size = new Size(125, 27);
            txtContrasena.TabIndex = 12;
            txtContrasena.UseSystemPasswordChar = true;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(31, 323);
            label6.Name = "label6";
            label6.Size = new Size(83, 20);
            label6.TabIndex = 13;
            label6.Text = "Contraseña";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(34, 374);
            label7.Name = "label7";
            label7.Size = new Size(31, 20);
            label7.TabIndex = 14;
            label7.Text = "Rol";
            // 
            // cmbRol
            // 
            cmbRol.FormattingEnabled = true;
            cmbRol.Items.AddRange(new object[] { "Admin", "Empleado" });
            cmbRol.Location = new Point(140, 374);
            cmbRol.Name = "cmbRol";
            cmbRol.Size = new Size(125, 28);
            cmbRol.TabIndex = 15;
            // 
            // FrmCRUDEmpleado
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(800, 450);
            Controls.Add(cmbRol);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(txtContrasena);
            Controls.Add(txtUsuario);
            Controls.Add(label5);
            Controls.Add(txtTelefono);
            Controls.Add(label4);
            Controls.Add(txtNombre);
            Controls.Add(label3);
            Controls.Add(txtApellido);
            Controls.Add(label2);
            Controls.Add(txtDni);
            Controls.Add(label1);
            Controls.Add(btnCancelar);
            Controls.Add(btnGuardar);
            Name = "FrmCRUDEmpleado";
            Text = "Empleado";
            Load += FrmCRUDEmpleado_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnGuardar;
        private Button btnCancelar;
        private Label label1;
        private TextBox txtDni;
        private Label label2;
        private TextBox txtApellido;
        private Label label3;
        private TextBox txtNombre;
        private Label label4;
        private TextBox txtTelefono;
        private Label label5;
        private TextBox txtUsuario;
        private TextBox txtContrasena;
        private Label label6;
        private Label label7;
        private ComboBox cmbRol;
    }
}