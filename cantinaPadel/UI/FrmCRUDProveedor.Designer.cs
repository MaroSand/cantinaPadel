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
            panelFooter = new Panel();
            btnGuardar = new Button();
            btnCancelar = new Button();
            panelHeader.SuspendLayout();
            panelCuerpo.SuspendLayout();
            panelFooter.SuspendLayout();
            SuspendLayout();
            // 
            // panelHeader
            // 
            panelHeader.BackColor = Color.Gold;
            panelHeader.Controls.Add(lblTitulo);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Margin = new Padding(2);
            panelHeader.Name = "panelHeader";
            panelHeader.Size = new Size(882, 46);
            panelHeader.TabIndex = 0;
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTitulo.Location = new Point(27, 9);
            lblTitulo.Margin = new Padding(2, 0, 2, 0);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(225, 30);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Datos del Proveedor";
            // 
            // panelCuerpo
            // 
            panelCuerpo.BackColor = SystemColors.Info;
            panelCuerpo.Controls.Add(lblNombreEmpresa);
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
            panelCuerpo.Location = new Point(0, 46);
            panelCuerpo.Margin = new Padding(2);
            panelCuerpo.Name = "panelCuerpo";
            panelCuerpo.Size = new Size(882, 345);
            panelCuerpo.TabIndex = 1;
            // 
            // lblNombreEmpresa
            // 
            lblNombreEmpresa.AutoSize = true;
            lblNombreEmpresa.Location = new Point(27, 39);
            lblNombreEmpresa.Margin = new Padding(2, 0, 2, 0);
            lblNombreEmpresa.Name = "lblNombreEmpresa";
            lblNombreEmpresa.Size = new Size(149, 20);
            lblNombreEmpresa.TabIndex = 0;
            lblNombreEmpresa.Text = "Nombre de Empresa:";
            // 
            // txtNombreEmpresa
            // 
            txtNombreEmpresa.Location = new Point(187, 36);
            txtNombreEmpresa.Margin = new Padding(2);
            txtNombreEmpresa.MaxLength = 50;
            txtNombreEmpresa.Name = "txtNombreEmpresa";
            txtNombreEmpresa.Size = new Size(165, 27);
            txtNombreEmpresa.TabIndex = 0;
            // 
            // lblApellido
            // 
            lblApellido.AutoSize = true;
            lblApellido.Location = new Point(27, 117);
            lblApellido.Margin = new Padding(2, 0, 2, 0);
            lblApellido.Name = "lblApellido";
            lblApellido.Size = new Size(79, 20);
            lblApellido.TabIndex = 1;
            lblApellido.Text = "Apellido: *";
            // 
            // txtApellido
            // 
            txtApellido.Location = new Point(187, 110);
            txtApellido.Margin = new Padding(2);
            txtApellido.MaxLength = 50;
            txtApellido.Name = "txtApellido";
            txtApellido.Size = new Size(165, 27);
            txtApellido.TabIndex = 1;
            // 
            // lblNombre
            // 
            lblNombre.AutoSize = true;
            lblNombre.Location = new Point(27, 185);
            lblNombre.Margin = new Padding(2, 0, 2, 0);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new Size(77, 20);
            lblNombre.TabIndex = 2;
            lblNombre.Text = "Nombre: *";
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(187, 185);
            txtNombre.Margin = new Padding(2);
            txtNombre.MaxLength = 50;
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(165, 27);
            txtNombre.TabIndex = 2;
            // 
            // lblDni
            // 
            lblDni.AutoSize = true;
            lblDni.Location = new Point(27, 260);
            lblDni.Margin = new Padding(2, 0, 2, 0);
            lblDni.Name = "lblDni";
            lblDni.Size = new Size(38, 20);
            lblDni.TabIndex = 3;
            lblDni.Text = "DNI:";
            // 
            // txtDni
            // 
            txtDni.Location = new Point(187, 260);
            txtDni.Margin = new Padding(2);
            txtDni.MaxLength = 8;
            txtDni.Name = "txtDni";
            txtDni.Size = new Size(165, 27);
            txtDni.TabIndex = 3;
            // 
            // lblCuit
            // 
            lblCuit.AutoSize = true;
            lblCuit.Location = new Point(431, 39);
            lblCuit.Margin = new Padding(2, 0, 2, 0);
            lblCuit.Name = "lblCuit";
            lblCuit.Size = new Size(168, 20);
            lblCuit.TabIndex = 4;
            lblCuit.Text = "CUIT (XX-XXXXXXXX-X):";
            // 
            // txtCuit
            // 
            txtCuit.Location = new Point(603, 36);
            txtCuit.Margin = new Padding(2);
            txtCuit.MaxLength = 13;
            txtCuit.Name = "txtCuit";
            txtCuit.Size = new Size(165, 27);
            txtCuit.TabIndex = 4;
            // 
            // lblCondicionIva
            // 
            lblCondicionIva.AutoSize = true;
            lblCondicionIva.Location = new Point(431, 117);
            lblCondicionIva.Margin = new Padding(2, 0, 2, 0);
            lblCondicionIva.Name = "lblCondicionIva";
            lblCondicionIva.Size = new Size(105, 20);
            lblCondicionIva.TabIndex = 5;
            lblCondicionIva.Text = "Condición IVA:";
            // 
            // cmbCondicionIva
            // 
            cmbCondicionIva.FormattingEnabled = true;
            cmbCondicionIva.Location = new Point(603, 117);
            cmbCondicionIva.Margin = new Padding(2);
            cmbCondicionIva.Name = "cmbCondicionIva";
            cmbCondicionIva.Size = new Size(165, 28);
            cmbCondicionIva.TabIndex = 5;
            // 
            // lblTelefono
            // 
            lblTelefono.AutoSize = true;
            lblTelefono.Location = new Point(431, 192);
            lblTelefono.Margin = new Padding(2, 0, 2, 0);
            lblTelefono.Name = "lblTelefono";
            lblTelefono.Size = new Size(70, 20);
            lblTelefono.TabIndex = 6;
            lblTelefono.Text = "Teléfono:";
            // 
            // txtTelefono
            // 
            txtTelefono.Location = new Point(603, 192);
            txtTelefono.Margin = new Padding(2);
            txtTelefono.MaxLength = 20;
            txtTelefono.Name = "txtTelefono";
            txtTelefono.Size = new Size(165, 27);
            txtTelefono.TabIndex = 6;
            // 
            // lblDireccion
            // 
            lblDireccion.AutoSize = true;
            lblDireccion.Location = new Point(431, 267);
            lblDireccion.Margin = new Padding(2, 0, 2, 0);
            lblDireccion.Name = "lblDireccion";
            lblDireccion.Size = new Size(75, 20);
            lblDireccion.TabIndex = 7;
            lblDireccion.Text = "Dirección:";
            // 
            // txtDireccion
            // 
            txtDireccion.Location = new Point(603, 267);
            txtDireccion.Margin = new Padding(2);
            txtDireccion.MaxLength = 100;
            txtDireccion.Name = "txtDireccion";
            txtDireccion.Size = new Size(165, 27);
            txtDireccion.TabIndex = 7;
            // 
            // panelFooter
            // 
            panelFooter.BackColor = SystemColors.Info;
            panelFooter.Controls.Add(btnGuardar);
            panelFooter.Controls.Add(btnCancelar);
            panelFooter.Dock = DockStyle.Bottom;
            panelFooter.Location = new Point(0, 391);
            panelFooter.Margin = new Padding(2);
            panelFooter.Name = "panelFooter";
            panelFooter.Size = new Size(882, 62);
            panelFooter.TabIndex = 2;
            // 
            // btnGuardar
            // 
            btnGuardar.BackColor = Color.ForestGreen;
            btnGuardar.FlatStyle = FlatStyle.Flat;
            btnGuardar.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnGuardar.ForeColor = Color.White;
            btnGuardar.Location = new Point(467, 10);
            btnGuardar.Margin = new Padding(2);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(322, 41);
            btnGuardar.TabIndex = 0;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = false;
            // 
            // btnCancelar
            // 
            btnCancelar.BackColor = Color.IndianRed;
            btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCancelar.ForeColor = Color.White;
            btnCancelar.Location = new Point(87, 10);
            btnCancelar.Margin = new Padding(2);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(322, 41);
            btnCancelar.TabIndex = 1;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = false;
            // 
            // FrmCRUDProveedor
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(882, 453);
            Controls.Add(panelCuerpo);
            Controls.Add(panelFooter);
            Controls.Add(panelHeader);
            FormBorderStyle = FormBorderStyle.FixedDialog;
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
            panelFooter.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel    panelHeader;
        private Panel    panelCuerpo;
        private Panel    panelFooter;
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
        private Button   btnGuardar;
        private Button   btnCancelar;
    }
}
