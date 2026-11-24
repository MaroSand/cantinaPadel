namespace cantinaPadel.UI
{
    partial class FrmAltaCompra
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
            txtComprobante = new TextBox();
            lblComprobante = new Label();
            dtpFecha = new DateTimePicker();
            lblFecha = new Label();
            cmbProveedor = new ComboBox();
            lblProveedor = new Label();
            panelBuscador = new Panel();
            lblInfoProducto = new Label();
            txtCostoUnitario = new TextBox();
            lblCosto = new Label();
            nudCantidad = new NumericUpDown();
            lblCantidad = new Label();
            txtBuscarProducto = new ComboBox();
            lblBuscarProducto = new Label();
            panelDetalle = new Panel();
            lblTituloDetalle = new Label();
            panelAcciones = new Panel();
            btnVaciar = new Button();
            btnCancelar = new Button();
            btnConfirmar = new Button();
            lblTotal = new Label();
            btnQuitar = new Button();
            dgvCompra = new DataGridView();
            panelDatos.SuspendLayout();
            panelBuscador.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudCantidad).BeginInit();
            panelDetalle.SuspendLayout();
            panelAcciones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCompra).BeginInit();
            SuspendLayout();
            // 
            // panelDatos
            // 
            panelDatos.BackColor = Color.White;
            panelDatos.BorderStyle = BorderStyle.FixedSingle;
            panelDatos.Controls.Add(txtComprobante);
            panelDatos.Controls.Add(lblComprobante);
            panelDatos.Controls.Add(dtpFecha);
            panelDatos.Controls.Add(lblFecha);
            panelDatos.Controls.Add(cmbProveedor);
            panelDatos.Controls.Add(lblProveedor);
            panelDatos.Dock = DockStyle.Top;
            panelDatos.Location = new Point(511, 49);
            panelDatos.Name = "panelDatos";
            panelDatos.Size = new Size(625, 161);
            panelDatos.TabIndex = 1;
            // 
            // txtComprobante
            // 
            txtComprobante.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtComprobante.Location = new Point(124, 123);
            txtComprobante.MaxLength = 30;
            txtComprobante.Name = "txtComprobante";
            txtComprobante.PlaceholderText = "Ej.: A-0001-00001234";
            txtComprobante.Size = new Size(469, 27);
            txtComprobante.TabIndex = 5;
            // 
            // lblComprobante
            // 
            lblComprobante.AutoSize = true;
            lblComprobante.Location = new Point(30, 127);
            lblComprobante.Name = "lblComprobante";
            lblComprobante.Size = new Size(242, 20);
            lblComprobante.TabIndex = 4;
            lblComprobante.Text = "Factura/remito (opcional):";
            // dtpFecha
            // 
            dtpFecha.Format = DateTimePickerFormat.Short;
            dtpFecha.Location = new Point(124, 69);
            dtpFecha.Name = "dtpFecha";
            dtpFecha.Size = new Size(122, 27);
            dtpFecha.TabIndex = 3;
            // 
            // lblFecha
            // 
            lblFecha.AutoSize = true;
            lblFecha.Location = new Point(30, 73);
            lblFecha.Name = "lblFecha";
            lblFecha.Size = new Size(50, 20);
            lblFecha.TabIndex = 2;
            lblFecha.Text = "Fecha:";
            // 
            // cmbProveedor
            // 
            cmbProveedor.DropDownStyle = ComboBoxStyle.DropDown;
            cmbProveedor.FormattingEnabled = true;
            cmbProveedor.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbProveedor.Location = new Point(124, 15);
            cmbProveedor.Name = "cmbProveedor";
            cmbProveedor.Size = new Size(469, 28);
            cmbProveedor.TabIndex = 1;
            // lblProveedor
            // 
            lblProveedor.AutoSize = true;
            lblProveedor.Location = new Point(30, 19);
            lblProveedor.Name = "lblProveedor";
            lblProveedor.Size = new Size(90, 20);
            lblProveedor.TabIndex = 0;
            lblProveedor.Text = "Proveedor: *";
            // 
            // panelBuscador
            // 
            panelBuscador.BackColor = Color.White;
            panelBuscador.BorderStyle = BorderStyle.FixedSingle;
            panelBuscador.Controls.Add(lblInfoProducto);
            panelBuscador.Controls.Add(txtCostoUnitario);
            panelBuscador.Controls.Add(lblCosto);
            panelBuscador.Controls.Add(nudCantidad);
            panelBuscador.Controls.Add(lblCantidad);
            panelBuscador.Controls.Add(txtBuscarProducto);
            panelBuscador.Controls.Add(lblBuscarProducto);
            panelBuscador.Dock = DockStyle.Top;
            panelBuscador.Location = new Point(28, 49);
            panelBuscador.Name = "panelBuscador";
            panelBuscador.Size = new Size(468, 161);
            panelBuscador.TabIndex = 2;
            // 
            // lblInfoProducto
            // 
            lblInfoProducto.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblInfoProducto.ForeColor = Color.DimGray;
            lblInfoProducto.AutoEllipsis = true;
            lblInfoProducto.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblInfoProducto.Location = new Point(30, 126);
            lblInfoProducto.Name = "lblInfoProducto";
            lblInfoProducto.Size = new Size(416, 22);
            lblInfoProducto.TabIndex = 9;
            lblInfoProducto.Text = string.Empty;
            // 
            // txtCostoUnitario
            // 
            txtCostoUnitario.Location = new Point(162, 87);
            txtCostoUnitario.MaxLength = 14;
            txtCostoUnitario.Name = "txtCostoUnitario";
            txtCostoUnitario.Size = new Size(135, 27);
            txtCostoUnitario.TabIndex = 7;
            txtCostoUnitario.TextAlign = HorizontalAlignment.Right;
            // 
            // lblCosto
            // 
            lblCosto.AutoSize = true;
            lblCosto.Location = new Point(30, 90);
            lblCosto.Name = "lblCosto";
            lblCosto.Size = new Size(127, 20);
            lblCosto.TabIndex = 6;
            lblCosto.Text = "Costo unitario ($):";
            // 
            // nudCantidad
            // 
            nudCantidad.Location = new Point(385, 43);
            nudCantidad.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            nudCantidad.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudCantidad.Name = "nudCantidad";
            nudCantidad.Size = new Size(70, 27);
            nudCantidad.TabIndex = 5;
            nudCantidad.ThousandsSeparator = true;
            nudCantidad.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lblCantidad
            // 
            lblCantidad.AutoSize = true;
            lblCantidad.Location = new Point(341, 45);
            lblCantidad.Name = "lblCantidad";
            lblCantidad.Size = new Size(42, 20);
            lblCantidad.TabIndex = 4;
            lblCantidad.Text = "Cant.";
            // 
            // txtBuscarProducto
            // 
            txtBuscarProducto.DropDownStyle = ComboBoxStyle.DropDown;
            txtBuscarProducto.FormattingEnabled = true;
            txtBuscarProducto.Location = new Point(30, 42);
            txtBuscarProducto.MaxDropDownItems = 10;
            txtBuscarProducto.MaxLength = 100;
            txtBuscarProducto.Name = "txtBuscarProducto";
            txtBuscarProducto.DropDownHeight = 240;
            txtBuscarProducto.DropDownWidth = 620;
            txtBuscarProducto.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            txtBuscarProducto.Size = new Size(305, 28);
            txtBuscarProducto.TabIndex = 1;
            //
            // lblBuscarProducto
            //
            lblBuscarProducto.AutoSize = true;
            lblBuscarProducto.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblBuscarProducto.Location = new Point(30, 19);
            lblBuscarProducto.Name = "lblBuscarProducto";
            lblBuscarProducto.Size = new Size(325, 20);
            lblBuscarProducto.TabIndex = 0;
            lblBuscarProducto.Text = "Buscar producto (nombre o código de barras):";
            // 
            // panelDetalle
            //
            panelDetalle.BackColor = Color.White;
            panelDetalle.BorderStyle = BorderStyle.FixedSingle;
            panelDetalle.Controls.Add(btnVaciar);
            panelDetalle.Controls.Add(btnQuitar);
            panelDetalle.Controls.Add(dgvCompra);
            panelDetalle.Controls.Add(lblTituloDetalle);
            panelDetalle.Dock = DockStyle.Fill;
            panelDetalle.Location = new Point(0, 324);
            panelDetalle.Name = "panelDetalle";
            panelDetalle.Size = new Size(1180, 364);
            panelDetalle.TabIndex = 3;
            //
            // lblTituloDetalle
            //
            lblTituloDetalle.AutoSize = true;
            lblTituloDetalle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTituloDetalle.Location = new Point(41, 19);
            lblTituloDetalle.Name = "lblTituloDetalle";
            lblTituloDetalle.Size = new Size(132, 20);
            lblTituloDetalle.TabIndex = 10;
            lblTituloDetalle.Text = "Detalle de compra";
            // 
            // panelAcciones
            //
            panelAcciones.BackColor = Color.White;
            panelAcciones.BorderStyle = BorderStyle.FixedSingle;
            panelAcciones.Controls.Add(btnCancelar);
            panelAcciones.Controls.Add(btnConfirmar);
            panelAcciones.Controls.Add(lblTotal);
            panelAcciones.Dock = DockStyle.Bottom;
            panelAcciones.Location = new Point(0, 670);
            panelAcciones.Name = "panelAcciones";
            panelAcciones.Size = new Size(1180, 70);
            panelAcciones.TabIndex = 4;
            // 
            // btnCancelar
            // 
            btnCancelar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancelar.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCancelar.Location = new Point(956, 14);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(176, 42);
            btnCancelar.TabIndex = 3;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            // 
            // btnConfirmar
            // 
            btnConfirmar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnConfirmar.BackColor = Color.Green;
            btnConfirmar.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnConfirmar.ForeColor = Color.White;
            btnConfirmar.Location = new Point(680, 14);
            btnConfirmar.Name = "btnConfirmar";
            btnConfirmar.Size = new Size(260, 42);
            btnConfirmar.TabIndex = 2;
            btnConfirmar.Text = "Revisar compra";
            btnConfirmar.UseVisualStyleBackColor = false;
            // 
            // lblTotal
            // 
            lblTotal.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            lblTotal.AutoSize = true;
            lblTotal.BackColor = Color.Transparent;
            lblTotal.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotal.Location = new Point(30, 16);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(143, 31);
            lblTotal.TabIndex = 1;
            lblTotal.Text = "Total: $ 0,00";
            // 
            // btnQuitar
            // 
            btnQuitar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnQuitar.BackColor = Color.FromArgb(255, 192, 192);
            btnQuitar.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnQuitar.Location = new Point(41, 300);
            btnQuitar.Name = "btnQuitar";
            btnQuitar.Size = new Size(176, 40);
            btnQuitar.TabIndex = 5;
            btnQuitar.Text = "Quitar";
            btnQuitar.UseVisualStyleBackColor = false;
            // 
            // btnVaciar
            //
            btnVaciar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnVaciar.BackColor = Color.Firebrick;
            btnVaciar.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnVaciar.ForeColor = SystemColors.ButtonHighlight;
            btnVaciar.Location = new Point(253, 300);
            btnVaciar.Name = "btnVaciar";
            btnVaciar.Size = new Size(176, 40);
            btnVaciar.TabIndex = 6;
            btnVaciar.Text = "Vaciar compra";
            btnVaciar.UseVisualStyleBackColor = false;
            // 
            // dgvCompra
            //
            dgvCompra.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvCompra.BackgroundColor = SystemColors.ButtonHighlight;
            dgvCompra.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCompra.Location = new Point(41, 42);
            dgvCompra.Name = "dgvCompra";
            dgvCompra.RowHeadersWidth = 51;
            dgvCompra.Size = new Size(1089, 248);
            dgvCompra.TabIndex = 4;
            // 
            // FrmAltaCompra
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(250, 248, 240);
            ClientSize = new Size(1180, 740);
            Controls.Add(panelDetalle);
            Controls.Add(panelAcciones);
            Controls.Add(panelBuscador);
            Controls.Add(panelDatos);
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimumSize = new Size(920, 660);
            MaximizeBox = true;
            MinimizeBox = false;
            Name = "FrmAltaCompra";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Nueva Compra";
            panelDatos.ResumeLayout(false);
            panelDatos.PerformLayout();
            panelBuscador.ResumeLayout(false);
            panelBuscador.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudCantidad).EndInit();
            panelDetalle.ResumeLayout(false);
            panelDetalle.PerformLayout();
            panelAcciones.ResumeLayout(false);
            panelAcciones.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCompra).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelDatos;
        private ComboBox cmbProveedor;
        private Label lblProveedor;
        private TextBox txtComprobante;
        private Label lblComprobante;
        private DateTimePicker dtpFecha;
        private Label lblFecha;
        private Panel panelBuscador;
        private NumericUpDown nudCantidad;
        private Label lblCantidad;
        private ComboBox txtBuscarProducto;
        private Label lblBuscarProducto;
        private Panel panelDetalle;
        private Label lblTituloDetalle;
        private Label lblInfoProducto;
        private TextBox txtCostoUnitario;
        private Label lblCosto;
        private Panel panelAcciones;
        private Label lblTotal;
        private Button btnQuitar;
        private Button btnVaciar;
        private Button btnCancelar;
        private Button btnConfirmar;
        private DataGridView dgvCompra;
    }
}
