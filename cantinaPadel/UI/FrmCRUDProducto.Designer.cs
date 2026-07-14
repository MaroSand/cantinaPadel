namespace cantinaPadel.UI
{
    partial class FrmCRUDProducto
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
            lblStockMinimo = new Label();
            txtStockMinimo = new TextBox();
            lblStockActual = new Label();
            txtStockActual = new TextBox();
            lblPrecioConIva = new Label();
            lblPrecioConIvaValor = new Label();
            lblPrecioVenta = new Label();
            txtPrecioVenta = new TextBox();
            lblPrecioCosto = new Label();
            txtPrecioCosto = new TextBox();
            lblMarca = new Label();
            cmbMarca = new ComboBox();
            lblCategoria = new Label();
            cmbCategoria = new ComboBox();
            lblCodigoBarras = new Label();
            txtCodigoBarras = new TextBox();
            lblNombre = new Label();
            txtNombre = new TextBox();
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
            panelHeader.Size = new Size(1000, 46);
            panelHeader.TabIndex = 0;
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTitulo.Location = new Point(27, 9);
            lblTitulo.Margin = new Padding(2, 0, 2, 0);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(211, 30);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Datos del Producto";
            // 
            // panelCuerpo
            // 
            panelCuerpo.BackColor = SystemColors.Info;
            panelCuerpo.Controls.Add(lblStockMinimo);
            panelCuerpo.Controls.Add(txtStockMinimo);
            panelCuerpo.Controls.Add(lblStockActual);
            panelCuerpo.Controls.Add(txtStockActual);
            panelCuerpo.Controls.Add(lblPrecioConIva);
            panelCuerpo.Controls.Add(lblPrecioConIvaValor);
            panelCuerpo.Controls.Add(lblPrecioVenta);
            panelCuerpo.Controls.Add(txtPrecioVenta);
            panelCuerpo.Controls.Add(lblPrecioCosto);
            panelCuerpo.Controls.Add(txtPrecioCosto);
             // Se eliminaron campos Proveedor y Tipo por mejora de UX
            panelCuerpo.Controls.Add(lblMarca);
            panelCuerpo.Controls.Add(cmbMarca);
            panelCuerpo.Controls.Add(lblCategoria);
            panelCuerpo.Controls.Add(cmbCategoria);
            panelCuerpo.Controls.Add(lblCodigoBarras);
            panelCuerpo.Controls.Add(txtCodigoBarras);
            panelCuerpo.Controls.Add(lblNombre);
            panelCuerpo.Controls.Add(txtNombre);
            panelCuerpo.Dock = DockStyle.Fill;
            panelCuerpo.Location = new Point(0, 46);
            panelCuerpo.Margin = new Padding(2);
            panelCuerpo.Name = "panelCuerpo";
            panelCuerpo.Size = new Size(1000, 420);
            panelCuerpo.TabIndex = 1;
            // 
            // lblNombre
            // 
            lblNombre.AutoSize = true;
            lblNombre.Location = new Point(27, 40);
            lblNombre.Margin = new Padding(2, 0, 2, 0);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new Size(80, 20);
            lblNombre.TabIndex = 0;
            lblNombre.Text = "Nombre: *";
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(230, 37);
            txtNombre.Margin = new Padding(2);
            txtNombre.MaxLength = 100;
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(220, 27);
            txtNombre.TabIndex = 0;
            // 
            // lblCodigoBarras
            // 
            lblCodigoBarras.AutoSize = true;
            lblCodigoBarras.Location = new Point(27, 101);
            lblCodigoBarras.Margin = new Padding(2, 0, 2, 0);
            lblCodigoBarras.Name = "lblCodigoBarras";
            lblCodigoBarras.Size = new Size(163, 20);
            lblCodigoBarras.TabIndex = 1;
            lblCodigoBarras.Text = "Código de Barras: ";
            // 
            // txtCodigoBarras
            // 
            txtCodigoBarras.Location = new Point(230, 98);
            txtCodigoBarras.Margin = new Padding(2);
            txtCodigoBarras.MaxLength = 50;
            txtCodigoBarras.Name = "txtCodigoBarras";
            txtCodigoBarras.Size = new Size(220, 27);
            txtCodigoBarras.TabIndex = 1;
            // 
            // lblCategoria
            // 
            lblCategoria.AutoSize = true;
            lblCategoria.Location = new Point(27, 162);
            lblCategoria.Margin = new Padding(2, 0, 2, 0);
            lblCategoria.Name = "lblCategoria";
            lblCategoria.Size = new Size(98, 20);
            lblCategoria.TabIndex = 2;
            lblCategoria.Text = "Categoría: *";
            // 
            // cmbCategoria
            // 
            cmbCategoria.FormattingEnabled = true;
            cmbCategoria.Location = new Point(230, 159);
            cmbCategoria.Margin = new Padding(2);
            cmbCategoria.Name = "cmbCategoria";
            cmbCategoria.Size = new Size(220, 28);
            cmbCategoria.TabIndex = 2;
            // 
            // lblMarca
            // 
            lblMarca.AutoSize = true;
            lblMarca.Location = new Point(27, 223);
            lblMarca.Margin = new Padding(2, 0, 2, 0);
            lblMarca.Name = "lblMarca";
            lblMarca.Size = new Size(54, 20);
            lblMarca.TabIndex = 3;
            lblMarca.Text = "Marca:";
            // 
            // cmbMarca
            // 
            cmbMarca.FormattingEnabled = true;
            cmbMarca.Location = new Point(230, 220);
            cmbMarca.Margin = new Padding(2);
            cmbMarca.Name = "cmbMarca";
            cmbMarca.Size = new Size(220, 28);
            cmbMarca.TabIndex = 3;
            // 
            // Campos Proveedor y Tipo eliminados para simplificar formulario
            // 
            // lblPrecioCosto
            // 
            lblPrecioCosto.AutoSize = true;
            lblPrecioCosto.Location = new Point(480, 40);
            lblPrecioCosto.Margin = new Padding(2, 0, 2, 0);
            lblPrecioCosto.Name = "lblPrecioCosto";
            lblPrecioCosto.Size = new Size(102, 20);
            lblPrecioCosto.TabIndex = 6;
            lblPrecioCosto.Text = "Precio Costo:";
            // 
            // txtPrecioCosto
            // 
            txtPrecioCosto.Location = new Point(750, 37);
            txtPrecioCosto.Margin = new Padding(2);
            txtPrecioCosto.MaxLength = 12;
            txtPrecioCosto.Name = "txtPrecioCosto";
            txtPrecioCosto.Size = new Size(165, 27);
            txtPrecioCosto.TabIndex = 6;
            // 
            // lblPrecioVenta
            // 
            lblPrecioVenta.AutoSize = true;
            lblPrecioVenta.Location = new Point(480, 101);
            lblPrecioVenta.Margin = new Padding(2, 0, 2, 0);
            lblPrecioVenta.Name = "lblPrecioVenta";
            lblPrecioVenta.Size = new Size(175, 20);
            lblPrecioVenta.TabIndex = 7;
            lblPrecioVenta.Text = "Precio Venta (s/IVA): *";
            // 
            // txtPrecioVenta
            // 
            txtPrecioVenta.Location = new Point(750, 98);
            txtPrecioVenta.Margin = new Padding(2);
            txtPrecioVenta.MaxLength = 12;
            txtPrecioVenta.Name = "txtPrecioVenta";
            txtPrecioVenta.Size = new Size(165, 27);
            txtPrecioVenta.TabIndex = 7;
            // 
            // lblPrecioConIva
            // 
            lblPrecioConIva.AutoSize = true;
            lblPrecioConIva.Location = new Point(480, 162);
            lblPrecioConIva.Margin = new Padding(2, 0, 2, 0);
            lblPrecioConIva.Name = "lblPrecioConIva";
            lblPrecioConIva.Size = new Size(178, 20);
            lblPrecioConIva.TabIndex = 8;
            lblPrecioConIva.Text = "Precio Final (IVA 21%):";
            // 
            // lblPrecioConIvaValor
            // 
            lblPrecioConIvaValor.BackColor = Color.White;
            lblPrecioConIvaValor.BorderStyle = BorderStyle.FixedSingle;
            lblPrecioConIvaValor.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPrecioConIvaValor.Location = new Point(750, 159);
            lblPrecioConIvaValor.Margin = new Padding(2, 0, 2, 0);
            lblPrecioConIvaValor.Name = "lblPrecioConIvaValor";
            lblPrecioConIvaValor.Size = new Size(165, 27);
            lblPrecioConIvaValor.TabIndex = 8;
            lblPrecioConIvaValor.Text = "—";
            lblPrecioConIvaValor.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblStockActual
            // 
            lblStockActual.AutoSize = true;
            lblStockActual.Location = new Point(480, 223);
            lblStockActual.Margin = new Padding(2, 0, 2, 0);
            lblStockActual.Name = "lblStockActual";
            lblStockActual.Size = new Size(101, 20);
            lblStockActual.TabIndex = 9;
            lblStockActual.Text = "Stock Actual:";
            // 
            // txtStockActual
            // 
            txtStockActual.Location = new Point(750, 220);
            txtStockActual.Margin = new Padding(2);
            txtStockActual.MaxLength = 6;
            txtStockActual.Name = "txtStockActual";
            txtStockActual.Size = new Size(165, 27);
            txtStockActual.TabIndex = 9;
            // 
            // lblStockMinimo
            // 
            lblStockMinimo.AutoSize = true;
            lblStockMinimo.Location = new Point(480, 284);
            lblStockMinimo.Margin = new Padding(2, 0, 2, 0);
            lblStockMinimo.Name = "lblStockMinimo";
            lblStockMinimo.Size = new Size(108, 20);
            lblStockMinimo.TabIndex = 10;
            lblStockMinimo.Text = "Stock Mínimo:";
            // 
            // txtStockMinimo
            // 
            txtStockMinimo.Location = new Point(750, 281);
            txtStockMinimo.Margin = new Padding(2);
            txtStockMinimo.MaxLength = 6;
            txtStockMinimo.Name = "txtStockMinimo";
            txtStockMinimo.Size = new Size(165, 27);
            txtStockMinimo.TabIndex = 10;
            // 
            // panelFooter
            // 
            panelFooter.BackColor = SystemColors.Info;
            panelFooter.Controls.Add(btnGuardar);
            panelFooter.Controls.Add(btnCancelar);
            panelFooter.Dock = DockStyle.Bottom;
            panelFooter.Location = new Point(0, 466);
            panelFooter.Margin = new Padding(2);
            panelFooter.Name = "panelFooter";
            panelFooter.Size = new Size(1000, 62);
            panelFooter.TabIndex = 2;
            // 
            // btnGuardar
            // 
            btnGuardar.BackColor = Color.ForestGreen;
            btnGuardar.FlatStyle = FlatStyle.Flat;
            btnGuardar.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnGuardar.ForeColor = Color.White;
            btnGuardar.Location = new Point(520, 10);
            btnGuardar.Margin = new Padding(2);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(380, 41);
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
            btnCancelar.Location = new Point(100, 10);
            btnCancelar.Margin = new Padding(2);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(380, 41);
            btnCancelar.TabIndex = 1;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = false;
            // 
            // FrmCRUDProducto
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 528);
            Controls.Add(panelCuerpo);
            Controls.Add(panelFooter);
            Controls.Add(panelHeader);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmCRUDProducto";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Producto";
            Load += FrmCRUDProducto_Load;
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
        private Label    lblNombre;
        private TextBox  txtNombre;
        private Label    lblCodigoBarras;
        private TextBox  txtCodigoBarras;
        private Label    lblCategoria;
        private ComboBox cmbCategoria;
        private Label    lblMarca;
        private ComboBox cmbMarca;
        // campos Proveedor y Tipo eliminados
        private Label    lblPrecioCosto;
        private TextBox  txtPrecioCosto;
        private Label    lblPrecioVenta;
        private TextBox  txtPrecioVenta;
        private Label    lblPrecioConIva;
        private Label    lblPrecioConIvaValor;
        private Label    lblStockActual;
        private TextBox  txtStockActual;
        private Label    lblStockMinimo;
        private TextBox  txtStockMinimo;
        private Button   btnGuardar;
        private Button   btnCancelar;
    }
}