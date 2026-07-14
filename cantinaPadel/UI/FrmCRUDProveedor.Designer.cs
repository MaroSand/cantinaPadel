namespace cantinaPadel.UI
{
    partial class FrmCRUDProveedor
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
            panelHeader = new Panel();
            lblTitulo = new Label();
            panelCuerpo = new Panel();
            lblNombreEmpresa = new Label();
            txtNombreEmpresa = new TextBox();
            lblApellido = new Label();
            txtApellido = new TextBox();
            lblNombre = new Label();
            txtNombre = new TextBox();
            lblDni = new Label();
            txtDni = new TextBox();
            lblCuit = new Label();
            txtCuit = new TextBox();
            lblCondicionIva = new Label();
            cmbCondicionIva = new ComboBox();
            lblTelefono = new Label();
            txtTelefono = new TextBox();
            lblDireccion = new Label();
            txtDireccion = new TextBox();
            btnCancelar = new Button();
            btnGuardar = new Button();
            panelHeader.SuspendLayout();
            panelCuerpo.SuspendLayout();
            SuspendLayout();
            // 
            // panelHeader
            // 
            panelHeader.BackColor = Color.Gold;
            panelHeader.Controls.Add(lblTitulo);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Name = "panelHeader";
            panelHeader.Size = new Size(1433, 74);
            panelHeader.TabIndex = 0;
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTitulo.Location = new Point(44, 14);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(357, 47);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Datos del Proveedor";
            // 
            // panelCuerpo
            // 
            panelCuerpo.BackColor = SystemColors.Info;
            panelCuerpo.Controls.Add(btnGuardar);
            panelCuerpo.Controls.Add(lblNombreEmpresa);
            panelCuerpo.Controls.Add(btnCancelar);
            panelCuerpo.Controls.Add(txtNombreEmpresa);
            panelCuerpo.Controls.Add(lblApellido);
            panelCuerpo.Controls.Add(txtApellido);
            panelCuerpo.Controls.Add(lblNombre);
            panelCuerpo.Controls.Add(txtNombre);
            panelCuerpo.Controls.Add(lblDni);
            panelCuerpo.Controls.Add(txtDni);
            panelCuerpo.Controls.Add(lblCuit);
            panelCuerpo.Controls.Add(txtCuit);
            panelCuerpo.Controls.Add(lblCondicionIva);
            panelCuerpo.Controls.Add(cmbCondicionIva);
            panelCuerpo.Controls.Add(lblTelefono);
            panelCuerpo.Controls.Add(txtTelefono);
            panelCuerpo.Controls.Add(lblDireccion);
            panelCuerpo.Controls.Add(txtDireccion);
            panelCuerpo.Dock = DockStyle.Fill;
            panelCuerpo.Location = new Point(0, 74);
            panelCuerpo.Name = "panelCuerpo";
            panelCuerpo.Size = new Size(1433, 651);
            panelCuerpo.TabIndex = 1;
            // 
            // lblNombreEmpresa
            // 
            lblNombreEmpresa.AutoSize = true;
            lblNombreEmpresa.Location = new Point(44, 62);
            lblNombreEmpresa.Name = "lblNombreEmpresa";
            lblNombreEmpresa.Size = new Size(238, 32);
            lblNombreEmpresa.TabIndex = 0;
            lblNombreEmpresa.Text = "Nombre de Empresa:";
            // 
            // txtNombreEmpresa
            // 
            txtNombreEmpresa.Location = new Point(304, 58);
            txtNombreEmpresa.MaxLength = 50;
            txtNombreEmpresa.Name = "txtNombreEmpresa";
            txtNombreEmpresa.Size = new Size(266, 39);
            txtNombreEmpresa.TabIndex = 0;
            // 
            // lblApellido
            // 
            lblApellido.AutoSize = true;
            lblApellido.Location = new Point(44, 187);
            lblApellido.Name = "lblApellido";
            lblApellido.Size = new Size(124, 32);
            lblApellido.TabIndex = 1;
            lblApellido.Text = "Apellido: *";
            // 
            // txtApellido
            // 
            txtApellido.Location = new Point(304, 176);
            txtApellido.MaxLength = 50;
            txtApellido.Name = "txtApellido";
            txtApellido.Size = new Size(266, 39);
            txtApellido.TabIndex = 1;
            // 
            // lblNombre
            // 
            lblNombre.AutoSize = true;
            lblNombre.Location = new Point(44, 296);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new Size(124, 32);
            lblNombre.TabIndex = 2;
            lblNombre.Text = "Nombre: *";
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(304, 296);
            txtNombre.MaxLength = 50;
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(266, 39);
            txtNombre.TabIndex = 2;
            // 
            // lblDni
            // 
            lblDni.AutoSize = true;
            lblDni.Location = new Point(44, 416);
            lblDni.Name = "lblDni";
            lblDni.Size = new Size(60, 32);
            lblDni.TabIndex = 3;
            lblDni.Text = "DNI:";
            // 
            // txtDni
            // 
            txtDni.Location = new Point(304, 416);
            txtDni.MaxLength = 8;
            txtDni.Name = "txtDni";
            txtDni.Size = new Size(266, 39);
            txtDni.TabIndex = 3;
            // 
            // lblCuit
            // 
            lblCuit.AutoSize = true;
            lblCuit.Location = new Point(700, 62);
            lblCuit.Name = "lblCuit";
            lblCuit.Size = new Size(264, 32);
            lblCuit.TabIndex = 4;
            lblCuit.Text = "CUIT (XX-XXXXXXXX-X):";
            // 
            // txtCuit
            // 
            txtCuit.Location = new Point(980, 58);
            txtCuit.MaxLength = 13;
            txtCuit.Name = "txtCuit";
            txtCuit.Size = new Size(266, 39);
            txtCuit.TabIndex = 4;
            // 
            // lblCondicionIva
            // 
            lblCondicionIva.AutoSize = true;
            lblCondicionIva.Location = new Point(700, 187);
            lblCondicionIva.Name = "lblCondicionIva";
            lblCondicionIva.Size = new Size(169, 32);
            lblCondicionIva.TabIndex = 5;
            lblCondicionIva.Text = "Condición IVA:";
            // 
            // cmbCondicionIva
            // 
            cmbCondicionIva.FormattingEnabled = true;
            cmbCondicionIva.Location = new Point(980, 187);
            cmbCondicionIva.Name = "cmbCondicionIva";
            cmbCondicionIva.Size = new Size(266, 40);
            cmbCondicionIva.TabIndex = 5;
            // 
            // lblTelefono
            // 
            lblTelefono.AutoSize = true;
            lblTelefono.Location = new Point(700, 307);
            lblTelefono.Name = "lblTelefono";
            lblTelefono.Size = new Size(112, 32);
            lblTelefono.TabIndex = 6;
            lblTelefono.Text = "Teléfono:";
            // 
            // txtTelefono
            // 
            txtTelefono.Location = new Point(980, 307);
            txtTelefono.MaxLength = 20;
            txtTelefono.Name = "txtTelefono";
            txtTelefono.Size = new Size(266, 39);
            txtTelefono.TabIndex = 6;
            // 
            // lblDireccion
            // 
            lblDireccion.AutoSize = true;
            lblDireccion.Location = new Point(700, 427);
            lblDireccion.Name = "lblDireccion";
            lblDireccion.Size = new Size(119, 32);
            lblDireccion.TabIndex = 7;
            lblDireccion.Text = "Dirección:";
            // 
            // txtDireccion
            // 
            txtDireccion.Location = new Point(980, 427);
            txtDireccion.MaxLength = 100;
            txtDireccion.Name = "txtDireccion";
            txtDireccion.Size = new Size(266, 39);
            txtDireccion.TabIndex = 7;
            // 
            // btnCancelar
            // 
            btnCancelar.BackColor = Color.IndianRed;
            btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCancelar.ForeColor = Color.White;
            btnCancelar.Location = new Point(155, 517);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(523, 66);
            btnCancelar.TabIndex = 1;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = false;
            // 
            // btnGuardar
            // 
            btnGuardar.BackColor = Color.ForestGreen;
            btnGuardar.FlatStyle = FlatStyle.Flat;
            btnGuardar.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnGuardar.ForeColor = Color.White;
            btnGuardar.Location = new Point(748, 517);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(523, 66);
            btnGuardar.TabIndex = 0;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = false;
            // 
            // FrmCRUDProveedor
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1433, 725);
            Controls.Add(panelCuerpo);
            Controls.Add(panelHeader);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(5, 5, 5, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmCRUDProveedor";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Proveedor";
            Load += FrmCRUDProveedor_Load;
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            panelCuerpo.ResumeLayout(false);
            panelCuerpo.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel    panelHeader;
        private Panel    panelCuerpo;
        private Label    lblTitulo;
        private Label    lblNombreEmpresa;
        private System.Windows.Forms.TextBox txtNombreEmpresa;
        private Label    lblNombre;
        private TextBox  txtNombre;
        private Label    lblApellido;
        private TextBox  txtApellido;
        private Label    lblDni;
        private TextBox  txtDni;
        private Label    lblCuit;
        private TextBox  txtCuit;
        private Label    lblCondicionIva;
        private ComboBox cmbCondicionIva;
        private Label    lblTelefono;
        private TextBox  txtTelefono;
        private Label    lblDireccion;
        private TextBox  txtDireccion;
        private Button btnGuardar;
        private Button btnCancelar;
    }
}
