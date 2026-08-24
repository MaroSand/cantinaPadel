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
            panelHeader = new System.Windows.Forms.Panel();
            lblTitulo = new System.Windows.Forms.Label();
            panelCuerpo = new System.Windows.Forms.Panel();
            btnGuardar = new System.Windows.Forms.Button();
            lblNombreEmpresa = new System.Windows.Forms.Label();
            btnCancelar = new System.Windows.Forms.Button();
            txtNombreEmpresa = new System.Windows.Forms.TextBox();
            lblApellido = new System.Windows.Forms.Label();
            txtApellido = new System.Windows.Forms.TextBox();
            lblNombre = new System.Windows.Forms.Label();
            txtNombre = new System.Windows.Forms.TextBox();
            lblDni = new System.Windows.Forms.Label();
            txtDni = new System.Windows.Forms.TextBox();
            lblCuit = new System.Windows.Forms.Label();
            txtCuit = new System.Windows.Forms.TextBox();
            lblCondicionIva = new System.Windows.Forms.Label();
            cmbCondicionIva = new System.Windows.Forms.ComboBox();
            lblTelefono = new System.Windows.Forms.Label();
            txtTelefono = new System.Windows.Forms.TextBox();
            lblDireccion = new System.Windows.Forms.Label();
            txtDireccion = new System.Windows.Forms.TextBox();
            panelHeader.SuspendLayout();
            panelCuerpo.SuspendLayout();
            SuspendLayout();
            // 
            // panelHeader
            // 
            panelHeader.BackColor = System.Drawing.Color.Gold;
            panelHeader.Controls.Add(lblTitulo);
            panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            panelHeader.Location = new System.Drawing.Point(0, 0);
            panelHeader.Name = "panelHeader";
            panelHeader.Size = new System.Drawing.Size(1433, 74);
            panelHeader.TabIndex = 0;
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            lblTitulo.Location = new System.Drawing.Point(44, 14);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new System.Drawing.Size(357, 47);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Datos del Proveedor";
            // 
            // panelCuerpo
            // 
            panelCuerpo.BackColor = System.Drawing.SystemColors.Info;
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
            panelCuerpo.Dock = System.Windows.Forms.DockStyle.Fill;
            panelCuerpo.Location = new System.Drawing.Point(0, 74);
            panelCuerpo.Name = "panelCuerpo";
            panelCuerpo.Size = new System.Drawing.Size(1433, 664);
            panelCuerpo.TabIndex = 1;
            // 
            // btnGuardar
            // 
            btnGuardar.BackColor = System.Drawing.Color.ForestGreen;
            btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnGuardar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)0));
            btnGuardar.ForeColor = System.Drawing.Color.White;
            btnGuardar.Location = new System.Drawing.Point(748, 517);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new System.Drawing.Size(523, 66);
            btnGuardar.TabIndex = 0;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = false;
            // 
            // lblNombreEmpresa
            // 
            lblNombreEmpresa.AutoSize = true;
            lblNombreEmpresa.ForeColor = System.Drawing.Color.Black;
            lblNombreEmpresa.Location = new System.Drawing.Point(44, 62);
            lblNombreEmpresa.Name = "lblNombreEmpresa";
            lblNombreEmpresa.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            lblNombreEmpresa.Size = new System.Drawing.Size(260, 32);
            lblNombreEmpresa.TabIndex = 0;
            lblNombreEmpresa.Text = "Nombre de Empresa: *";
            // 
            // btnCancelar
            // 
            btnCancelar.BackColor = System.Drawing.Color.IndianRed;
            btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnCancelar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)0));
            btnCancelar.ForeColor = System.Drawing.Color.White;
            btnCancelar.Location = new System.Drawing.Point(155, 517);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new System.Drawing.Size(523, 66);
            btnCancelar.TabIndex = 1;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = false;
            // 
            // txtNombreEmpresa
            // 
            txtNombreEmpresa.Location = new System.Drawing.Point(304, 58);
            txtNombreEmpresa.MaxLength = 50;
            txtNombreEmpresa.Name = "txtNombreEmpresa";
            txtNombreEmpresa.Size = new System.Drawing.Size(266, 39);
            txtNombreEmpresa.TabIndex = 0;
            // 
            // lblApellido
            // 
            lblApellido.AutoSize = true;
            lblApellido.Location = new System.Drawing.Point(44, 187);
            lblApellido.Name = "lblApellido";
            lblApellido.Size = new System.Drawing.Size(124, 32);
            lblApellido.TabIndex = 1;
            lblApellido.Text = "Apellido: *";
            // 
            // txtApellido
            // 
            txtApellido.Location = new System.Drawing.Point(304, 176);
            txtApellido.MaxLength = 50;
            txtApellido.Name = "txtApellido";
            txtApellido.Size = new System.Drawing.Size(266, 39);
            txtApellido.TabIndex = 1;
            // 
            // lblNombre
            // 
            lblNombre.AutoSize = true;
            lblNombre.Location = new System.Drawing.Point(44, 296);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new System.Drawing.Size(124, 32);
            lblNombre.TabIndex = 2;
            lblNombre.Text = "Nombre: *";
            // 
            // txtNombre
            // 
            txtNombre.Location = new System.Drawing.Point(304, 296);
            txtNombre.MaxLength = 50;
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new System.Drawing.Size(266, 39);
            txtNombre.TabIndex = 2;
            // 
            // lblDni
            // 
            lblDni.AutoSize = true;
            lblDni.Location = new System.Drawing.Point(44, 416);
            lblDni.Name = "lblDni";
            lblDni.Size = new System.Drawing.Size(60, 32);
            lblDni.TabIndex = 3;
            lblDni.Text = "DNI:";
            // 
            // txtDni
            // 
            txtDni.Location = new System.Drawing.Point(304, 416);
            txtDni.MaxLength = 8;
            txtDni.Name = "txtDni";
            txtDni.Size = new System.Drawing.Size(266, 39);
            txtDni.TabIndex = 3;
            // 
            // lblCuit
            // 
            lblCuit.AutoSize = true;
            lblCuit.Location = new System.Drawing.Point(700, 62);
            lblCuit.Name = "lblCuit";
            lblCuit.Size = new System.Drawing.Size(264, 32);
            lblCuit.TabIndex = 4;
            lblCuit.Text = "CUIT (XX-XXXXXXXX-X):";
            // 
            // txtCuit
            // 
            txtCuit.Location = new System.Drawing.Point(980, 58);
            txtCuit.MaxLength = 13;
            txtCuit.Name = "txtCuit";
            txtCuit.Size = new System.Drawing.Size(266, 39);
            txtCuit.TabIndex = 4;
            // 
            // lblCondicionIva
            // 
            lblCondicionIva.AutoSize = true;
            lblCondicionIva.Location = new System.Drawing.Point(700, 187);
            lblCondicionIva.Name = "lblCondicionIva";
            lblCondicionIva.Size = new System.Drawing.Size(169, 32);
            lblCondicionIva.TabIndex = 5;
            lblCondicionIva.Text = "Condición IVA:";
            // 
            // cmbCondicionIva
            // 
            cmbCondicionIva.FormattingEnabled = true;
            cmbCondicionIva.Location = new System.Drawing.Point(980, 187);
            cmbCondicionIva.Name = "cmbCondicionIva";
            cmbCondicionIva.Size = new System.Drawing.Size(266, 40);
            cmbCondicionIva.TabIndex = 5;
            // 
            // lblTelefono
            // 
            lblTelefono.AutoSize = true;
            lblTelefono.Location = new System.Drawing.Point(700, 307);
            lblTelefono.Name = "lblTelefono";
            lblTelefono.Size = new System.Drawing.Size(112, 32);
            lblTelefono.TabIndex = 6;
            lblTelefono.Text = "Teléfono:";
            // 
            // txtTelefono
            // 
            txtTelefono.Location = new System.Drawing.Point(980, 307);
            txtTelefono.MaxLength = 20;
            txtTelefono.Name = "txtTelefono";
            txtTelefono.Size = new System.Drawing.Size(266, 39);
            txtTelefono.TabIndex = 6;
            // 
            // lblDireccion
            // 
            lblDireccion.AutoSize = true;
            lblDireccion.Location = new System.Drawing.Point(700, 427);
            lblDireccion.Name = "lblDireccion";
            lblDireccion.Size = new System.Drawing.Size(119, 32);
            lblDireccion.TabIndex = 7;
            lblDireccion.Text = "Dirección:";
            // 
            // txtDireccion
            // 
            txtDireccion.Location = new System.Drawing.Point(980, 427);
            txtDireccion.MaxLength = 100;
            txtDireccion.Name = "txtDireccion";
            txtDireccion.Size = new System.Drawing.Size(266, 39);
            txtDireccion.TabIndex = 7;
            // 
            // FrmCRUDProveedor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1433, 738);
            Controls.Add(panelCuerpo);
            Controls.Add(panelHeader);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(5);
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
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
        private System.Windows.Forms.Panel panelCuerpo;
        private Label    lblTitulo;
        private System.Windows.Forms.Label lblNombreEmpresa;
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
