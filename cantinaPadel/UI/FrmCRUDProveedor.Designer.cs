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
            panelHeader      = new Panel();
            panelCuerpo      = new Panel();
            panelFooter      = new Panel();
            lblTitulo        = new Label();
            lblNombre        = new Label();
            txtNombre        = new TextBox();
            lblApellido      = new Label();
            txtApellido      = new TextBox();
            lblDni           = new Label();
            txtDni           = new TextBox();
            lblCuit          = new Label();
            txtCuit          = new TextBox();
            lblCondicionIva  = new Label();
            cmbCondicionIva  = new ComboBox();
            lblTelefono      = new Label();
            txtTelefono      = new TextBox();
            lblDireccion     = new Label();
            txtDireccion     = new TextBox();
            lblNombreEmpresa = new Label();
            txtNombreEmpresa = new TextBox();
            btnGuardar       = new Button();
            btnCancelar      = new Button();
            panelHeader.SuspendLayout();
            panelCuerpo.SuspendLayout();
            panelFooter.SuspendLayout();
            SuspendLayout();
            //
            // panelHeader
            //
            panelHeader.BackColor = Color.Gold;
            panelHeader.Controls.Add(lblTitulo);
            panelHeader.Dock     = DockStyle.Top;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Name     = "panelHeader";
            panelHeader.Size     = new Size(500, 50);
            panelHeader.TabIndex = 0;
            //
            // lblTitulo
            //
            lblTitulo.AutoSize = true;
            lblTitulo.Font     = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTitulo.Location = new Point(16, 12);
            lblTitulo.Name     = "lblTitulo";
            lblTitulo.Text     = "Datos del Proveedor";
            //
            // panelFooter
            //
            panelFooter.Controls.Add(btnGuardar);
            panelFooter.Controls.Add(btnCancelar);
            panelFooter.Dock     = DockStyle.Bottom;
            panelFooter.Location = new Point(0, 400);
            panelFooter.Name     = "panelFooter";
            panelFooter.Size     = new Size(500, 50);
            panelFooter.TabIndex = 2;
            //
            // btnGuardar
            //
            btnGuardar.Location = new Point(270, 10);
            btnGuardar.Name     = "btnGuardar";
            btnGuardar.Size     = new Size(100, 29);
            btnGuardar.TabIndex = 0;
            btnGuardar.Text     = "Guardar";
            btnGuardar.UseVisualStyleBackColor = true;
            //
            // btnCancelar
            //
            btnCancelar.Location = new Point(380, 10);
            btnCancelar.Name     = "btnCancelar";
            btnCancelar.Size     = new Size(100, 29);
            btnCancelar.TabIndex = 1;
            btnCancelar.Text     = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            //
            // panelCuerpo
            //
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
            panelCuerpo.Dock     = DockStyle.Fill;
            panelCuerpo.Location = new Point(0, 50);
            panelCuerpo.Name     = "panelCuerpo";
            panelCuerpo.Size     = new Size(500, 350);
            panelCuerpo.TabIndex = 1;
            //
            // lblNombreEmpresa
            //
            lblNombreEmpresa.AutoSize = true;
            lblNombreEmpresa.Location = new Point(16, 20);
            lblNombreEmpresa.Name     = "lblNombreEmpresa";
            lblNombreEmpresa.Text     = "Nombre de Empresa:";
            //
            // txtNombreEmpresa
            //
            txtNombreEmpresa.Location = new Point(200, 17);
            txtNombreEmpresa.Name     = "txtNombreEmpresa";
            txtNombreEmpresa.Size     = new Size(265, 27);
            txtNombreEmpresa.TabIndex = 0;
            //
            // lblApellido
            //
            lblApellido.AutoSize = true;
            lblApellido.Location = new Point(16, 60);
            lblApellido.Name     = "lblApellido";
            lblApellido.Text     = "Apellido: *";
            //
            // txtApellido
            //
            txtApellido.Location = new Point(200, 57);
            txtApellido.Name     = "txtApellido";
            txtApellido.Size     = new Size(265, 27);
            txtApellido.TabIndex = 1;
            //
            // lblNombre
            //
            lblNombre.AutoSize = true;
            lblNombre.Location = new Point(16, 100);
            lblNombre.Name     = "lblNombre";
            lblNombre.Text     = "Nombre: *";
            //
            // txtNombre
            //
            txtNombre.Location = new Point(200, 97);
            txtNombre.Name     = "txtNombre";
            txtNombre.Size     = new Size(265, 27);
            txtNombre.TabIndex = 2;
            //
            // lblDni
            //
            lblDni.AutoSize = true;
            lblDni.Location = new Point(16, 140);
            lblDni.Name     = "lblDni";
            lblDni.Text     = "DNI:";
            //
            // txtDni
            //
            txtDni.Location = new Point(200, 137);
            txtDni.Name     = "txtDni";
            txtDni.Size     = new Size(130, 27);
            txtDni.TabIndex = 3;
            //
            // lblCuit
            //
            lblCuit.AutoSize = true;
            lblCuit.Location = new Point(16, 180);
            lblCuit.Name     = "lblCuit";
            lblCuit.Text     = "CUIT (XX-XXXXXXXX-X):";
            //
            // txtCuit
            //
            txtCuit.Location  = new Point(200, 177);
            txtCuit.Name      = "txtCuit";
            txtCuit.Size      = new Size(130, 27);
            txtCuit.MaxLength = 13;
            txtCuit.TabIndex  = 4;
            //
            // lblCondicionIva
            //
            lblCondicionIva.AutoSize = true;
            lblCondicionIva.Location = new Point(16, 220);
            lblCondicionIva.Name     = "lblCondicionIva";
            lblCondicionIva.Text     = "Condición IVA:";
            //
            // cmbCondicionIva
            //
            cmbCondicionIva.FormattingEnabled = true;
            cmbCondicionIva.Items.AddRange(new object[]
            {
                "Responsable Inscripto",
                "Monotributista",
                "Exento",
                "Consumidor Final"
            });
            cmbCondicionIva.Location = new Point(200, 217);
            cmbCondicionIva.Name     = "cmbCondicionIva";
            cmbCondicionIva.Size     = new Size(200, 28);
            cmbCondicionIva.TabIndex = 5;
            //
            // lblTelefono
            //
            lblTelefono.AutoSize = true;
            lblTelefono.Location = new Point(16, 262);
            lblTelefono.Name     = "lblTelefono";
            lblTelefono.Text     = "Teléfono:";
            //
            // txtTelefono
            //
            txtTelefono.Location = new Point(200, 259);
            txtTelefono.Name     = "txtTelefono";
            txtTelefono.Size     = new Size(200, 27);
            txtTelefono.TabIndex = 6;
            //
            // lblDireccion
            //
            lblDireccion.AutoSize = true;
            lblDireccion.Location = new Point(16, 302);
            lblDireccion.Name     = "lblDireccion";
            lblDireccion.Text     = "Dirección:";
            //
            // txtDireccion
            //
            txtDireccion.Location = new Point(200, 299);
            txtDireccion.Name     = "txtDireccion";
            txtDireccion.Size     = new Size(265, 27);
            txtDireccion.TabIndex = 7;
            //
            // FrmCRUDProveedor
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode       = AutoScaleMode.Font;
            ClientSize          = new Size(500, 450);
            FormBorderStyle     = FormBorderStyle.FixedDialog;
            MaximizeBox         = false;
            MinimizeBox         = false;
            StartPosition       = FormStartPosition.CenterParent;
            Controls.Add(panelCuerpo);
            Controls.Add(panelFooter);
            Controls.Add(panelHeader);
            Name  = "FrmCRUDProveedor";
            Text  = "Proveedor";
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
        private TextBox  txtNombreEmpresa;
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