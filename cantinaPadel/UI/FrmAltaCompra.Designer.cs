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
            panelHeader = new Panel();
            lblTitulo = new Label();
            panelDatos = new Panel();
            txtComprobante = new TextBox();
            lblComprobante = new Label();
            dtpFecha = new DateTimePicker();
            lblFecha = new Label();
            cmbProveedor = new ComboBox();
            lblProveedor = new Label();
            panelBuscador = new Panel();
            lblInfoProducto = new Label();
            btnAgregar = new Button();
            txtCostoUnitario = new TextBox();
            lblCosto = new Label();
            nudCantidad = new NumericUpDown();
            lblCantidad = new Label();
            cmbResultados = new ComboBox();
            lblResultados = new Label();
            txtBuscarProducto = new TextBox();
            lblBuscarProducto = new Label();
            panelAcciones = new Panel();
            btnCancelar = new Button();
            btnConfirmar = new Button();
            lblTotal = new Label();
            btnQuitar = new Button();
            dgvCompra = new DataGridView();
            panelHeader.SuspendLayout();
            panelDatos.SuspendLayout();
            panelBuscador.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudCantidad).BeginInit();
            panelAcciones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCompra).BeginInit();
            SuspendLayout();
            // 
            // panelHeader
            // 
            panelHeader.BackColor = Color.Gold;
            panelHeader.Controls.Add(lblTitulo);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Name = "panelHeader";
            panelHeader.Size = new Size(951, 74);
            panelHeader.TabIndex = 0;
            // 
            // lblTitulo
            // 
            lblTitulo.Anchor = AnchorStyles.Top;
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 13.2000008F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitulo.Location = new Point(361, 24);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(172, 31);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Nueva Compra";
            // 
            // panelDatos
            // 
            panelDatos.BackColor = SystemColors.Info;
            panelDatos.Controls.Add(txtComprobante);
            panelDatos.Controls.Add(lblComprobante);
            panelDatos.Controls.Add(dtpFecha);
            panelDatos.Controls.Add(lblFecha);
            panelDatos.Controls.Add(cmbProveedor);
            panelDatos.Controls.Add(lblProveedor);
            panelDatos.Dock = DockStyle.Top;
            panelDatos.Location = new Point(0, 74);
            panelDatos.Name = "panelDatos";
            panelDatos.Size = new Size(951, 110);
            panelDatos.TabIndex = 1;
            // 
            // txtComprobante
            // 
            txtComprobante.Location = new Point(686, 14);
            txtComprobante.MaxLength = 30;
            txtComprobante.Name = "txtComprobante";
            txtComprobante.Size = new Size(184, 27);
            txtComprobante.TabIndex = 5;
            // 
            // lblComprobante
            // 
            lblComprobante.AutoSize = true;
            lblComprobante.Location = new Point(438, 17);
            lblComprobante.Name = "lblComprobante";
            lblComprobante.Size = new Size(242, 20);
            lblComprobante.TabIndex = 4;
            lblComprobante.Text = "N° de comprobante del proveedor:";
            // 
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
            lblFecha.Location = new Point(28, 76);
            lblFecha.Name = "lblFecha";
            lblFecha.Size = new Size(50, 20);
            lblFecha.TabIndex = 2;
            lblFecha.Text = "Fecha:";
            // 
            // cmbProveedor
            // 
            cmbProveedor.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbProveedor.FormattingEnabled = true;
            cmbProveedor.Location = new Point(124, 13);
            cmbProveedor.Name = "cmbProveedor";
            cmbProveedor.Size = new Size(284, 28);
            cmbProveedor.TabIndex = 1;
            // 
            // lblProveedor
            // 
            lblProveedor.AutoSize = true;
            lblProveedor.Location = new Point(28, 17);
            lblProveedor.Name = "lblProveedor";
            lblProveedor.Size = new Size(90, 20);
            lblProveedor.TabIndex = 0;
            lblProveedor.Text = "Proveedor: *";
            // 
            // panelBuscador
            // 
            panelBuscador.BackColor = SystemColors.Info;
            panelBuscador.Controls.Add(lblInfoProducto);
            panelBuscador.Controls.Add(btnAgregar);
            panelBuscador.Controls.Add(txtCostoUnitario);
            panelBuscador.Controls.Add(lblCosto);
            panelBuscador.Controls.Add(nudCantidad);
            panelBuscador.Controls.Add(lblCantidad);
            panelBuscador.Controls.Add(cmbResultados);
            panelBuscador.Controls.Add(lblResultados);
            panelBuscador.Controls.Add(txtBuscarProducto);
            panelBuscador.Controls.Add(lblBuscarProducto);
            panelBuscador.Dock = DockStyle.Top;
            panelBuscador.Location = new Point(0, 184);
            panelBuscador.Name = "panelBuscador";
            panelBuscador.Size = new Size(951, 140);
            panelBuscador.TabIndex = 2;
            // 
            // lblInfoProducto
            // 
            lblInfoProducto.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblInfoProducto.ForeColor = Color.DimGray;
            lblInfoProducto.Location = new Point(-3, 110);
            lblInfoProducto.Name = "lblInfoProducto";
            lblInfoProducto.Size = new Size(951, 30);
            lblInfoProducto.TabIndex = 9;
            lblInfoProducto.Text = "label1";
            // 
            // btnAgregar
            // 
            btnAgregar.BackColor = Color.White;
            btnAgregar.FlatStyle = FlatStyle.Flat;
            btnAgregar.Location = new Point(547, 65);
            btnAgregar.Name = "btnAgregar";
            btnAgregar.Size = new Size(197, 36);
            btnAgregar.TabIndex = 8;
            btnAgregar.Text = "Agregar";
            btnAgregar.UseVisualStyleBackColor = false;
            // 
            // txtCostoUnitario
            // 
            txtCostoUnitario.Location = new Point(341, 70);
            txtCostoUnitario.MaxLength = 14;
            txtCostoUnitario.Name = "txtCostoUnitario";
            txtCostoUnitario.Size = new Size(82, 27);
            txtCostoUnitario.TabIndex = 7;
            txtCostoUnitario.TextAlign = HorizontalAlignment.Right;
            // 
            // lblCosto
            // 
            lblCosto.AutoSize = true;
            lblCosto.Location = new Point(208, 73);
            lblCosto.Name = "lblCosto";
            lblCosto.Size = new Size(127, 20);
            lblCosto.TabIndex = 6;
            lblCosto.Text = "Costo unitario ($):";
            // 
            // nudCantidad
            // 
            nudCantidad.Location = new Point(106, 70);
            nudCantidad.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            nudCantidad.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudCantidad.Name = "nudCantidad";
            nudCantidad.Size = new Size(72, 27);
            nudCantidad.TabIndex = 5;
            nudCantidad.ThousandsSeparator = true;
            nudCantidad.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lblCantidad
            // 
            lblCantidad.AutoSize = true;
            lblCantidad.Location = new Point(28, 72);
            lblCantidad.Name = "lblCantidad";
            lblCantidad.Size = new Size(72, 20);
            lblCantidad.TabIndex = 4;
            lblCantidad.Text = "Cantidad:";
            // 
            // cmbResultados
            // 
            cmbResultados.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbResultados.FormattingEnabled = true;
            cmbResultados.Location = new Point(547, 15);
            cmbResultados.Name = "cmbResultados";
            cmbResultados.Size = new Size(323, 28);
            cmbResultados.TabIndex = 3;
            // 
            // lblResultados
            // 
            lblResultados.AutoSize = true;
            lblResultados.Location = new Point(438, 22);
            lblResultados.Name = "lblResultados";
            lblResultados.Size = new Size(103, 20);
            lblResultados.TabIndex = 2;
            lblResultados.Text = "Coincidencias:";
            // 
            // txtBuscarProducto
            // 
            txtBuscarProducto.Location = new Point(154, 15);
            txtBuscarProducto.MaxLength = 100;
            txtBuscarProducto.Name = "txtBuscarProducto";
            txtBuscarProducto.Size = new Size(254, 27);
            txtBuscarProducto.TabIndex = 1;
            // 
            // lblBuscarProducto
            // 
            lblBuscarProducto.AutoSize = true;
            lblBuscarProducto.Location = new Point(28, 22);
            lblBuscarProducto.Name = "lblBuscarProducto";
            lblBuscarProducto.Size = new Size(120, 20);
            lblBuscarProducto.TabIndex = 0;
            lblBuscarProducto.Text = "Buscar producto:";
            // 
            // panelAcciones
            // 
            panelAcciones.BackColor = SystemColors.Info;
            panelAcciones.Controls.Add(btnCancelar);
            panelAcciones.Controls.Add(btnConfirmar);
            panelAcciones.Controls.Add(lblTotal);
            panelAcciones.Controls.Add(btnQuitar);
            panelAcciones.Dock = DockStyle.Bottom;
            panelAcciones.Location = new Point(0, 516);
            panelAcciones.Name = "panelAcciones";
            panelAcciones.Size = new Size(951, 80);
            panelAcciones.TabIndex = 3;
            // 
            // btnCancelar
            // 
            btnCancelar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancelar.BackColor = Color.IndianRed;
            btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCancelar.ForeColor = Color.White;
            btnCancelar.Location = new Point(780, 28);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(159, 33);
            btnCancelar.TabIndex = 3;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = false;
            // 
            // btnConfirmar
            // 
            btnConfirmar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnConfirmar.BackColor = Color.ForestGreen;
            btnConfirmar.FlatStyle = FlatStyle.Flat;
            btnConfirmar.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnConfirmar.ForeColor = Color.White;
            btnConfirmar.Location = new Point(593, 28);
            btnConfirmar.Name = "btnConfirmar";
            btnConfirmar.Size = new Size(163, 33);
            btnConfirmar.TabIndex = 2;
            btnConfirmar.Text = "Confirmar compra";
            btnConfirmar.UseVisualStyleBackColor = false;
            // 
            // lblTotal
            // 
            lblTotal.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblTotal.AutoSize = true;
            lblTotal.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotal.Location = new Point(349, 28);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(143, 31);
            lblTotal.TabIndex = 1;
            lblTotal.Text = "Total: $ 0,00";
            // 
            // btnQuitar
            // 
            btnQuitar.Anchor = AnchorStyles.Left;
            btnQuitar.BackColor = Color.White;
            btnQuitar.FlatStyle = FlatStyle.Flat;
            btnQuitar.Location = new Point(84, 28);
            btnQuitar.Name = "btnQuitar";
            btnQuitar.Size = new Size(94, 33);
            btnQuitar.TabIndex = 0;
            btnQuitar.Text = "Quitar producto";
            btnQuitar.UseVisualStyleBackColor = false;
            // 
            // dgvCompra
            // 
            dgvCompra.BackgroundColor = Color.White;
            dgvCompra.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCompra.Dock = DockStyle.Fill;
            dgvCompra.Location = new Point(0, 324);
            dgvCompra.Name = "dgvCompra";
            dgvCompra.RowHeadersWidth = 51;
            dgvCompra.Size = new Size(951, 192);
            dgvCompra.TabIndex = 4;
            // 
            // FrmAltaCompra
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(951, 596);
            Controls.Add(dgvCompra);
            Controls.Add(panelAcciones);
            Controls.Add(panelBuscador);
            Controls.Add(panelDatos);
            Controls.Add(panelHeader);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmAltaCompra";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Nueva Compra";
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            panelDatos.ResumeLayout(false);
            panelDatos.PerformLayout();
            panelBuscador.ResumeLayout(false);
            panelBuscador.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudCantidad).EndInit();
            panelAcciones.ResumeLayout(false);
            panelAcciones.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCompra).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelHeader;
        private Label lblTitulo;
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
        private ComboBox cmbResultados;
        private Label lblResultados;
        private TextBox txtBuscarProducto;
        private Label lblBuscarProducto;
        private Label lblInfoProducto;
        private TextBox txtCostoUnitario;
        private Label lblCosto;
        private Panel panelAcciones;
        private Label lblTotal;
        private Button btnQuitar;
        private Button btnCancelar;
        private Button btnConfirmar;
        private DataGridView dgvCompra;
        private Button btnAgregar;
    }
}