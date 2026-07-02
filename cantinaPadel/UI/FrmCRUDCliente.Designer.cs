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
            label1 = new Label();
            txtNombre = new TextBox();
            txtApellido = new TextBox();
            label2 = new Label();
            label3 = new Label();
            txtDni = new TextBox();
            label4 = new Label();
            txtTelefono = new TextBox();
            label5 = new Label();
            txtEmail = new TextBox();
            btnGuardar = new Button();
            btnCancelar = new Button();
            panelDatos.SuspendLayout();
            SuspendLayout();
            // 
            // panelDatos
            // 
            panelDatos.BackColor = Color.White;
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
            panelDatos.Name = "panelDatos";
            panelDatos.Size = new Size(484, 361);
            panelDatos.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(20, 20);
            label1.Name = "label1";
            label1.Size = new Size(54, 15);
            label1.TabIndex = 0;
            label1.Text = "Nombre:";
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(120, 17);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(300, 23);
            txtNombre.TabIndex = 1;
            // 
            // txtApellido
            // 
            txtApellido.Location = new Point(120, 57);
            txtApellido.Name = "txtApellido";
            txtApellido.Size = new Size(300, 23);
            txtApellido.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(20, 60);
            label2.Name = "label2";
            label2.Size = new Size(54, 15);
            label2.TabIndex = 3;
            label2.Text = "Apellido:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(20, 100);
            label3.Name = "label3";
            label3.Size = new Size(30, 15);
            label3.TabIndex = 4;
            label3.Text = "DNI:";
            // 
            // txtDni
            // 
            txtDni.Location = new Point(120, 97);
            txtDni.Name = "txtDni";
            txtDni.Size = new Size(300, 23);
            txtDni.TabIndex = 5;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(20, 140);
            label4.Name = "label4";
            label4.Size = new Size(56, 15);
            label4.TabIndex = 6;
            label4.Text = "Teléfono:";
            // 
            // txtTelefono
            // 
            txtTelefono.Location = new Point(120, 137);
            txtTelefono.Name = "txtTelefono";
            txtTelefono.Size = new Size(300, 23);
            txtTelefono.TabIndex = 7;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(20, 180);
            label5.Name = "label5";
            label5.Size = new Size(47, 15);
            label5.TabIndex = 8;
            label5.Text = "Email: *";
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(120, 177);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(300, 23);
            txtEmail.TabIndex = 9;
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(120, 240);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(120, 35);
            btnGuardar.TabIndex = 10;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = true;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // btnCancelar
            // 
            btnCancelar.Location = new Point(260, 240);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(120, 35);
            btnCancelar.TabIndex = 11;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // FrmCRUDCliente
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(484, 361);
            Controls.Add(panelDatos);
            Name = "FrmCRUDCliente";
            Text = "Nuevo Cliente\n";
            panelDatos.ResumeLayout(false);
            panelDatos.PerformLayout();
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
    }
}