namespace cantinaPadel.UI
{
    partial class FrmCRUDCliente
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
            panelDatos = new Panel();
            btnCancelar = new Button();
            btnGuardar = new Button();
            txtEmail = new TextBox();
            label5 = new Label();
            txtTelefono = new TextBox();
            label4 = new Label();
            txtDni = new TextBox();
            label3 = new Label();
            label2 = new Label();
            txtApellido = new TextBox();
            txtNombre = new TextBox();
            label1 = new Label();
            panel1 = new Panel();
            label6 = new Label();
            panelDatos.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panelDatos
            // 
            panelDatos.BackColor = SystemColors.Info;
            panelDatos.Controls.Add(panel1);
            panelDatos.Controls.Add(btnCancelar);
            panelDatos.Controls.Add(btnGuardar);
            panelDatos.Controls.Add(txtEmail);
            panelDatos.Controls.Add(label5);
            panelDatos.Controls.Add(txtTelefono);
            panelDatos.Controls.Add(label4);
            panelDatos.Controls.Add(txtDni);
            panelDatos.Controls.Add(label3);
            panelDatos.Controls.Add(label2);
            panelDatos.Controls.Add(txtApellido);
            panelDatos.Controls.Add(txtNombre);
            panelDatos.Controls.Add(label1);
            panelDatos.Dock = DockStyle.Fill;
            panelDatos.Location = new Point(0, 0);
            panelDatos.Margin = new Padding(3, 4, 3, 4);
            panelDatos.Name = "panelDatos";
            panelDatos.Size = new Size(882, 453);
            panelDatos.TabIndex = 0;
            // 
            // btnCancelar
            // 
            btnCancelar.BackColor = Color.IndianRed;
            btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCancelar.ForeColor = Color.White;
            btnCancelar.Location = new Point(104, 333);
            btnCancelar.Margin = new Padding(3, 4, 3, 4);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(322, 41);
            btnCancelar.TabIndex = 11;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = false;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // btnGuardar
            // 
            btnGuardar.BackColor = Color.ForestGreen;
            btnGuardar.FlatStyle = FlatStyle.Flat;
            btnGuardar.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnGuardar.ForeColor = Color.White;
            btnGuardar.Location = new Point(479, 333);
            btnGuardar.Margin = new Padding(3, 4, 3, 4);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(322, 41);
            btnGuardar.TabIndex = 10;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = false;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(137, 267);
            txtEmail.Margin = new Padding(3, 4, 3, 4);
            txtEmail.MaxLength = 100;
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(342, 27);
            txtEmail.TabIndex = 9;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(31, 267);
            label5.Name = "label5";
            label5.Size = new Size(59, 20);
            label5.TabIndex = 8;
            label5.Text = "Email: *";
            // 
            // txtTelefono
            // 
            txtTelefono.Location = new Point(137, 215);
            txtTelefono.Margin = new Padding(3, 4, 3, 4);
            txtTelefono.MaxLength = 20;
            txtTelefono.Name = "txtTelefono";
            txtTelefono.Size = new Size(342, 27);
            txtTelefono.TabIndex = 7;
            txtTelefono.KeyPress += txtTelefono_KeyPress;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(31, 222);
            label4.Name = "label4";
            label4.Size = new Size(70, 20);
            label4.TabIndex = 6;
            label4.Text = "Teléfono:";
            // 
            // txtDni
            // 
            txtDni.Location = new Point(137, 173);
            txtDni.Margin = new Padding(3, 4, 3, 4);
            txtDni.MaxLength = 8;
            txtDni.Name = "txtDni";
            txtDni.Size = new Size(342, 27);
            txtDni.TabIndex = 5;
            txtDni.KeyPress += txtDni_KeyPress;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(31, 180);
            label3.Name = "label3";
            label3.Size = new Size(38, 20);
            label3.TabIndex = 4;
            label3.Text = "DNI:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(32, 134);
            label2.Name = "label2";
            label2.Size = new Size(69, 20);
            label2.TabIndex = 3;
            label2.Text = "Apellido:";
            // 
            // txtApellido
            // 
            txtApellido.Location = new Point(137, 127);
            txtApellido.Margin = new Padding(3, 4, 3, 4);
            txtApellido.MaxLength = 100;
            txtApellido.Name = "txtApellido";
            txtApellido.Size = new Size(342, 27);
            txtApellido.TabIndex = 2;
            txtApellido.KeyPress += txtNombreApellido_KeyPress;
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(137, 81);
            txtNombre.Margin = new Padding(3, 4, 3, 4);
            txtNombre.MaxLength = 100;
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(342, 27);
            txtNombre.TabIndex = 1;
            txtNombre.KeyPress += txtNombreApellido_KeyPress;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(34, 84);
            label1.Name = "label1";
            label1.Size = new Size(67, 20);
            label1.TabIndex = 0;
            label1.Text = "Nombre:";
            // 
            // panel1
            // 
            panel1.BackColor = Color.Gold;
            panel1.Controls.Add(label6);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(882, 47);
            panel1.TabIndex = 12;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 13.2000008F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.Location = new Point(32, 9);
            label6.Name = "label6";
            label6.Size = new Size(196, 31);
            label6.TabIndex = 0;
            label6.Text = "Datos del Cliente";
            // 
            // FrmCRUDCliente
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(882, 453);
            Controls.Add(panelDatos);
            Margin = new Padding(3, 4, 3, 4);
            Name = "FrmCRUDCliente";
            Text = "Nuevo Cliente\n";
            panelDatos.ResumeLayout(false);
            panelDatos.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelDatos;
        private TextBox txtNombre;
        private Label label1;
        private TextBox txtApellido;
        private Label label2;
        private TextBox txtDni;
        private Label label3;
        private Label label4;
        private Label label5;
        private TextBox txtTelefono;
        private Button btnCancelar;
        private Button btnGuardar;
        private TextBox txtEmail;
        private Panel panel1;
        private Label label6;
    }
}