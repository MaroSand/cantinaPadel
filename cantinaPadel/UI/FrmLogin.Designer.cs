namespace cantinaPadel
{
    partial class FrmLogin
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            lblUsuario = new Label();
            txtUsuario = new TextBox();
            lblContrasenia = new Label();
            txtContrasenia = new TextBox();
            btnIngresar = new Button();
            panel1 = new Panel();
            panel2 = new Panel();
            panel3 = new Panel();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = SystemColors.Info;
            label1.Font = new Font("Trebuchet MS", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.ActiveCaptionText;
            label1.Location = new Point(278, 89);
            label1.Margin = new Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new Size(275, 46);
            label1.TabIndex = 0;
            label1.Text = "INICIAR SESIÓN";
            // 
            // lblUsuario
            // 
            lblUsuario.AutoSize = true;
            lblUsuario.BackColor = SystemColors.Info;
            lblUsuario.ForeColor = SystemColors.ActiveCaptionText;
            lblUsuario.Location = new Point(234, 209);
            lblUsuario.Margin = new Padding(5, 0, 5, 0);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(99, 32);
            lblUsuario.TabIndex = 1;
            lblUsuario.Text = "Usuario:";
            // 
            // txtUsuario
            // 
            txtUsuario.Location = new Point(399, 206);
            txtUsuario.Margin = new Padding(5);
            txtUsuario.MaxLength = 25;
            txtUsuario.Name = "txtUsuario";
            txtUsuario.Size = new Size(201, 39);
            txtUsuario.TabIndex = 3;
            txtUsuario.TextChanged += txtUsuario_TextChanged;
            txtUsuario.KeyPress += txtUsuario_KeyPress;
            // 
            // lblContrasenia
            // 
            lblContrasenia.AutoSize = true;
            lblContrasenia.BackColor = SystemColors.Info;
            lblContrasenia.ForeColor = SystemColors.ActiveCaptionText;
            lblContrasenia.Location = new Point(234, 298);
            lblContrasenia.Margin = new Padding(5, 0, 5, 0);
            lblContrasenia.Name = "lblContrasenia";
            lblContrasenia.Size = new Size(139, 32);
            lblContrasenia.TabIndex = 4;
            lblContrasenia.Text = "Contraseña:";
            // 
            // txtContrasenia
            // 
            txtContrasenia.Location = new Point(399, 291);
            txtContrasenia.Margin = new Padding(5);
            txtContrasenia.MaxLength = 8;
            txtContrasenia.Name = "txtContrasenia";
            txtContrasenia.Size = new Size(201, 39);
            txtContrasenia.TabIndex = 7;
            txtContrasenia.UseSystemPasswordChar = true;
            txtContrasenia.TextChanged += txtContrasenia_TextChanged;
            txtContrasenia.KeyPress += txtUsuario_KeyPress;
            // 
            // btnIngresar
            // 
            btnIngresar.BackColor = Color.White;
            btnIngresar.FlatStyle = FlatStyle.Flat;
            btnIngresar.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnIngresar.ForeColor = SystemColors.ActiveCaptionText;
            btnIngresar.Location = new Point(342, 445);
            btnIngresar.Margin = new Padding(5);
            btnIngresar.Name = "btnIngresar";
            btnIngresar.Size = new Size(153, 72);
            btnIngresar.TabIndex = 8;
            btnIngresar.Text = "Ingresar";
            btnIngresar.UseVisualStyleBackColor = false;
            btnIngresar.Click += btnIngresar_Click;
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.Info;
            panel1.Controls.Add(btnIngresar);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(txtContrasenia);
            panel1.Controls.Add(lblUsuario);
            panel1.Controls.Add(lblContrasenia);
            panel1.Controls.Add(txtUsuario);
            panel1.Location = new Point(24, 51);
            panel1.Name = "panel1";
            panel1.Size = new Size(850, 565);
            panel1.TabIndex = 9;
            // 
            // panel2
            // 
            panel2.BackColor = Color.Gold;
            panel2.Controls.Add(panel1);
            panel2.ForeColor = Color.DarkGreen;
            panel2.Location = new Point(37, 122);
            panel2.Name = "panel2";
            panel2.Size = new Size(900, 655);
            panel2.TabIndex = 10;
            // 
            // panel3
            // 
            panel3.BackColor = Color.White;
            panel3.ForeColor = Color.White;
            panel3.Location = new Point(28, 107);
            panel3.Name = "panel3";
            panel3.Size = new Size(919, 684);
            panel3.TabIndex = 11;
            // 
            // FrmLogin
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Info;
            ClientSize = new Size(974, 929);
            Controls.Add(panel2);
            Controls.Add(panel3);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(5);
            MaximizeBox = false;
            Name = "FrmLogin";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Acceso al Sistema - Complejo Pádel";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private Label lblUsuario;
        private TextBox txtUsuario;
        private Label lblContrasenia;
        private TextBox txtContrasenia;
        private Button btnIngresar;
        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
    }
}
