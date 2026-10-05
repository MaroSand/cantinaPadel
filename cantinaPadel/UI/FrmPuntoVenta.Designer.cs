namespace cantinaPadel.UI
{
    partial class FrmPuntoVenta
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
            lblBuscarProducto = new Label();
            txtBuscarProducto = new TextBox();
            panel2 = new Panel();
            cmbResultados = new ComboBox();
            panel3 = new Panel();
            btnVaciarCarrito = new Button();
            dgvCarrito = new DataGridView();
            label1 = new Label();
            btnQuitarDelCarrito = new Button();
            panel4 = new Panel();
            btnConfirmarVenta = new Button();
            lblTotal = new Label();
            btnAgregarCliente = new Button();
            btnAgregarTurno = new Button();
            lblClienteVenta = new Label();
            btnQuitarCliente = new Button();
            btnVerCuentaCorriente = new Button();
            grpMetodoPago = new GroupBox();
            flujoMetodoPago = new FlowLayoutPanel();
            chkEfectivo = new CheckBox();
            chkTransferencia = new CheckBox();
            chkTarjeta = new CheckBox();
            chkBilleteraVirtual = new CheckBox();
            chkCuentaCorriente = new CheckBox();
            tabsPrincipal = new TabControl();
            tabVenta = new TabPage();
            tabCuentaCorriente = new TabPage();
            cuentaCorrienteControl1 = new CuentaCorrienteControl();
            nudCantidad = new NumericUpDown();
            label2 = new Label();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCarrito).BeginInit();
            panel4.SuspendLayout();
            grpMetodoPago.SuspendLayout();
            flujoMetodoPago.SuspendLayout();
            tabsPrincipal.SuspendLayout();
            tabVenta.SuspendLayout();
            tabCuentaCorriente.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudCantidad).BeginInit();
            SuspendLayout();
            // 
            // lblBuscarProducto
            // 
            lblBuscarProducto.AutoSize = true;
            lblBuscarProducto.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblBuscarProducto.Location = new Point(30, 19);
            lblBuscarProducto.Name = "lblBuscarProducto";
            lblBuscarProducto.Size = new Size(325, 20);
            lblBuscarProducto.TabIndex = 2;
            lblBuscarProducto.Text = "Buscar producto (nombre o código de barras):";
            // 
            // txtBuscarProducto
            // 
            txtBuscarProducto.Location = new Point(30, 42);
            txtBuscarProducto.MaxLength = 100;
            txtBuscarProducto.Name = "txtBuscarProducto";
            txtBuscarProducto.Size = new Size(305, 27);
            txtBuscarProducto.TabIndex = 3;
            // 
            // panel2
            // 
            panel2.BackColor = Color.Gold;
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(label2);
            panel2.Controls.Add(nudCantidad);
            panel2.Controls.Add(cmbResultados);
            panel2.Controls.Add(lblBuscarProducto);
            panel2.Controls.Add(txtBuscarProducto);
            panel2.Location = new Point(28, 49);
            panel2.Name = "panel2";
            panel2.Size = new Size(468, 161);
            panel2.TabIndex = 7;
            // 
            // cmbResultados
            // 
            cmbResultados.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbResultados.FormattingEnabled = true;
            cmbResultados.Location = new Point(30, 91);
            cmbResultados.Name = "cmbResultados";
            cmbResultados.Size = new Size(305, 28);
            cmbResultados.TabIndex = 4;
            // 
            // panel3
            // 
            panel3.BackColor = Color.Gold;
            panel3.BorderStyle = BorderStyle.FixedSingle;
            panel3.Controls.Add(btnVaciarCarrito);
            panel3.Controls.Add(dgvCarrito);
            panel3.Controls.Add(label1);
            panel3.Controls.Add(btnQuitarDelCarrito);
            panel3.Location = new Point(28, 225);
            panel3.Name = "panel3";
            panel3.Size = new Size(1031, 299);
            panel3.TabIndex = 8;
            // 
            // btnVaciarCarrito
            // 
            btnVaciarCarrito.BackColor = Color.Firebrick;
            btnVaciarCarrito.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnVaciarCarrito.ForeColor = SystemColors.ButtonHighlight;
            btnVaciarCarrito.Location = new Point(253, 245);
            btnVaciarCarrito.Name = "btnVaciarCarrito";
            btnVaciarCarrito.Size = new Size(176, 40);
            btnVaciarCarrito.TabIndex = 6;
            btnVaciarCarrito.Text = "Vaciar carrito";
            btnVaciarCarrito.UseVisualStyleBackColor = false;
            // 
            // dgvCarrito
            // 
            dgvCarrito.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            dgvCarrito.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCarrito.BackgroundColor = SystemColors.ButtonHighlight;
            dgvCarrito.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCarrito.Location = new Point(41, 42);
            dgvCarrito.Name = "dgvCarrito";
            dgvCarrito.RowHeadersWidth = 51;
            dgvCarrito.Size = new Size(940, 197);
            dgvCarrito.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(41, 19);
            label1.Name = "label1";
            label1.Size = new Size(57, 20);
            label1.TabIndex = 2;
            label1.Text = "Carrito";
            // 
            // btnQuitarDelCarrito
            // 
            btnQuitarDelCarrito.BackColor = Color.FromArgb(255, 192, 192);
            btnQuitarDelCarrito.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnQuitarDelCarrito.Location = new Point(41, 245);
            btnQuitarDelCarrito.Name = "btnQuitarDelCarrito";
            btnQuitarDelCarrito.Size = new Size(176, 40);
            btnQuitarDelCarrito.TabIndex = 5;
            btnQuitarDelCarrito.Text = "Quitar";
            btnQuitarDelCarrito.UseVisualStyleBackColor = false;
            // 
            // panel4
            // 
            panel4.BackColor = Color.Gold;
            panel4.BorderStyle = BorderStyle.FixedSingle;
            panel4.Controls.Add(btnConfirmarVenta);
            panel4.Controls.Add(lblTotal);
            panel4.Location = new Point(28, 631);
            panel4.Name = "panel4";
            panel4.Size = new Size(1031, 66);
            panel4.TabIndex = 9;
            // 
            // btnConfirmarVenta
            // 
            btnConfirmarVenta.BackColor = Color.Green;
            btnConfirmarVenta.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnConfirmarVenta.ForeColor = Color.White;
            btnConfirmarVenta.Location = new Point(721, 14);
            btnConfirmarVenta.Name = "btnConfirmarVenta";
            btnConfirmarVenta.Size = new Size(260, 42);
            btnConfirmarVenta.TabIndex = 1;
            btnConfirmarVenta.Text = "Confirmar venta";
            btnConfirmarVenta.UseVisualStyleBackColor = false;
            // 
            // lblTotal
            // 
            lblTotal.AutoSize = true;
            lblTotal.BackColor = Color.Transparent;
            lblTotal.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotal.Location = new Point(30, 16);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(137, 31);
            lblTotal.TabIndex = 0;
            lblTotal.Text = "Total: $0,00";
            // 
            // btnAgregarCliente
            // 
            btnAgregarCliente.BackColor = Color.Gold;
            btnAgregarCliente.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAgregarCliente.Location = new Point(532, 59);
            btnAgregarCliente.Name = "btnAgregarCliente";
            btnAgregarCliente.Size = new Size(200, 40);
            btnAgregarCliente.TabIndex = 10;
            btnAgregarCliente.Text = "Agregar Cliente";
            btnAgregarCliente.UseVisualStyleBackColor = false;
            // 
            // btnAgregarTurno
            // 
            btnAgregarTurno.BackColor = Color.Goldenrod;
            btnAgregarTurno.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAgregarTurno.Location = new Point(532, 109);
            btnAgregarTurno.Name = "btnAgregarTurno";
            btnAgregarTurno.Size = new Size(200, 40);
            btnAgregarTurno.TabIndex = 11;
            btnAgregarTurno.Text = "Agregar Turno";
            btnAgregarTurno.UseVisualStyleBackColor = false;
            // 
            // lblClienteVenta
            // 
            lblClienteVenta.AutoEllipsis = true;
            lblClienteVenta.BackColor = Color.Transparent;
            lblClienteVenta.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblClienteVenta.Location = new Point(752, 68);
            lblClienteVenta.Name = "lblClienteVenta";
            lblClienteVenta.Size = new Size(390, 40);
            lblClienteVenta.TabIndex = 12;
            lblClienteVenta.Text = "Cliente: Consumidor Final";
            // 
            // btnQuitarCliente
            // 
            btnQuitarCliente.Location = new Point(752, 109);
            btnQuitarCliente.Name = "btnQuitarCliente";
            btnQuitarCliente.Size = new Size(200, 40);
            btnQuitarCliente.TabIndex = 13;
            btnQuitarCliente.Text = "Quitar Cliente";
            btnQuitarCliente.UseVisualStyleBackColor = true;
            // 
            // btnVerCuentaCorriente
            // 
            btnVerCuentaCorriente.BackColor = Color.LightSteelBlue;
            btnVerCuentaCorriente.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnVerCuentaCorriente.Location = new Point(532, 159);
            btnVerCuentaCorriente.Name = "btnVerCuentaCorriente";
            btnVerCuentaCorriente.Size = new Size(200, 40);
            btnVerCuentaCorriente.TabIndex = 14;
            btnVerCuentaCorriente.Text = "Ver Cuenta Corriente";
            btnVerCuentaCorriente.UseVisualStyleBackColor = false;
            // 
            // grpMetodoPago
            // 
            grpMetodoPago.Controls.Add(flujoMetodoPago);
            grpMetodoPago.Location = new Point(28, 531);
            grpMetodoPago.Name = "grpMetodoPago";
            grpMetodoPago.Size = new Size(1031, 92);
            grpMetodoPago.TabIndex = 15;
            grpMetodoPago.TabStop = false;
            grpMetodoPago.Text = "Método de pago";
            // 
            // flujoMetodoPago
            // 
            flujoMetodoPago.Controls.Add(chkEfectivo);
            flujoMetodoPago.Controls.Add(chkTransferencia);
            flujoMetodoPago.Controls.Add(chkTarjeta);
            flujoMetodoPago.Controls.Add(chkBilleteraVirtual);
            flujoMetodoPago.Controls.Add(chkCuentaCorriente);
            flujoMetodoPago.Dock = DockStyle.Fill;
            flujoMetodoPago.Location = new Point(3, 23);
            flujoMetodoPago.Name = "flujoMetodoPago";
            flujoMetodoPago.Padding = new Padding(8);
            flujoMetodoPago.Size = new Size(1025, 66);
            flujoMetodoPago.TabIndex = 0;
            // 
            // chkEfectivo
            // 
            chkEfectivo.AutoSize = true;
            chkEfectivo.Checked = true;
            chkEfectivo.CheckState = CheckState.Checked;
            chkEfectivo.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            chkEfectivo.Location = new Point(14, 14);
            chkEfectivo.Margin = new Padding(6, 6, 24, 6);
            chkEfectivo.Name = "chkEfectivo";
            chkEfectivo.Size = new Size(87, 24);
            chkEfectivo.TabIndex = 0;
            chkEfectivo.Text = "Efectivo";
            chkEfectivo.UseVisualStyleBackColor = true;
            // 
            // chkTransferencia
            // 
            chkTransferencia.AutoSize = true;
            chkTransferencia.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            chkTransferencia.Location = new Point(131, 14);
            chkTransferencia.Margin = new Padding(6, 6, 24, 6);
            chkTransferencia.Name = "chkTransferencia";
            chkTransferencia.Size = new Size(125, 24);
            chkTransferencia.TabIndex = 1;
            chkTransferencia.Text = "Transferencia";
            chkTransferencia.UseVisualStyleBackColor = true;
            // 
            // chkTarjeta
            // 
            chkTarjeta.AutoSize = true;
            chkTarjeta.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            chkTarjeta.Location = new Point(286, 14);
            chkTarjeta.Margin = new Padding(6, 6, 24, 6);
            chkTarjeta.Name = "chkTarjeta";
            chkTarjeta.Size = new Size(79, 24);
            chkTarjeta.TabIndex = 2;
            chkTarjeta.Text = "Tarjeta";
            chkTarjeta.UseVisualStyleBackColor = true;
            // 
            // chkBilleteraVirtual
            // 
            chkBilleteraVirtual.AutoSize = true;
            chkBilleteraVirtual.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            chkBilleteraVirtual.Location = new Point(395, 14);
            chkBilleteraVirtual.Margin = new Padding(6, 6, 24, 6);
            chkBilleteraVirtual.Name = "chkBilleteraVirtual";
            chkBilleteraVirtual.Size = new Size(140, 24);
            chkBilleteraVirtual.TabIndex = 3;
            chkBilleteraVirtual.Text = "Billetera Virtual";
            chkBilleteraVirtual.UseVisualStyleBackColor = true;
            // 
            // chkCuentaCorriente
            // 
            chkCuentaCorriente.AutoSize = true;
            chkCuentaCorriente.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            chkCuentaCorriente.Location = new Point(565, 14);
            chkCuentaCorriente.Margin = new Padding(6, 6, 24, 6);
            chkCuentaCorriente.Name = "chkCuentaCorriente";
            chkCuentaCorriente.Size = new Size(149, 24);
            chkCuentaCorriente.TabIndex = 4;
            chkCuentaCorriente.Text = "Cuenta Corriente";
            chkCuentaCorriente.UseVisualStyleBackColor = true;
            // 
            // tabsPrincipal
            // 
            tabsPrincipal.Controls.Add(tabVenta);
            tabsPrincipal.Controls.Add(tabCuentaCorriente);
            tabsPrincipal.Dock = DockStyle.Fill;
            tabsPrincipal.Location = new Point(0, 0);
            tabsPrincipal.Name = "tabsPrincipal";
            tabsPrincipal.SelectedIndex = 0;
            tabsPrincipal.Size = new Size(1172, 849);
            tabsPrincipal.TabIndex = 0;
            // 
            // tabVenta
            // 
            tabVenta.BackColor = Color.FromArgb(255, 255, 192);
            tabVenta.Controls.Add(btnVerCuentaCorriente);
            tabVenta.Controls.Add(btnQuitarCliente);
            tabVenta.Controls.Add(lblClienteVenta);
            tabVenta.Controls.Add(btnAgregarTurno);
            tabVenta.Controls.Add(btnAgregarCliente);
            tabVenta.Controls.Add(panel4);
            tabVenta.Controls.Add(grpMetodoPago);
            tabVenta.Controls.Add(panel3);
            tabVenta.Controls.Add(panel2);
            tabVenta.Location = new Point(4, 29);
            tabVenta.Name = "tabVenta";
            tabVenta.Padding = new Padding(3);
            tabVenta.Size = new Size(1164, 816);
            tabVenta.TabIndex = 0;
            tabVenta.Text = "Venta";
            // 
            // tabCuentaCorriente
            // 
            tabCuentaCorriente.BackColor = Color.FromArgb(255, 255, 192);
            tabCuentaCorriente.Controls.Add(cuentaCorrienteControl1);
            tabCuentaCorriente.Location = new Point(4, 29);
            tabCuentaCorriente.Name = "tabCuentaCorriente";
            tabCuentaCorriente.Padding = new Padding(3);
            tabCuentaCorriente.Size = new Size(1164, 816);
            tabCuentaCorriente.TabIndex = 1;
            tabCuentaCorriente.Text = "Cuenta Corriente";
            // 
            // cuentaCorrienteControl1
            // 
            cuentaCorrienteControl1.BackColor = Color.FromArgb(255, 255, 192);
            cuentaCorrienteControl1.Dock = DockStyle.Fill;
            cuentaCorrienteControl1.Location = new Point(3, 3);
            cuentaCorrienteControl1.Name = "cuentaCorrienteControl1";
            cuentaCorrienteControl1.Size = new Size(1158, 810);
            cuentaCorrienteControl1.TabIndex = 0;
            // 
            // nudCantidad
            // 
            nudCantidad.Location = new Point(385, 43);
            nudCantidad.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudCantidad.Name = "nudCantidad";
            nudCantidad.Size = new Size(52, 27);
            nudCantidad.TabIndex = 7;
            nudCantidad.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(341, 45);
            label2.Name = "label2";
            label2.Size = new Size(42, 20);
            label2.TabIndex = 8;
            label2.Text = "Cant.";
            // 
            // FrmPuntoVenta
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(255, 255, 192);
            ClientSize = new Size(1172, 849);
            Controls.Add(tabsPrincipal);
            Name = "FrmPuntoVenta";
            Text = "FrmPuntoVenta";
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCarrito).EndInit();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            grpMetodoPago.ResumeLayout(false);
            flujoMetodoPago.ResumeLayout(false);
            flujoMetodoPago.PerformLayout();
            tabsPrincipal.ResumeLayout(false);
            tabVenta.ResumeLayout(false);
            tabCuentaCorriente.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)nudCantidad).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Label lblBuscarProducto;
        private TextBox txtBuscarProducto;
        private Panel panel2;
        private Panel panel3;
        private DataGridView dgvCarrito;
        private Label label1;
        private Button btnQuitarDelCarrito;
        private Panel panel4;
        private Button btnConfirmarVenta;
        private Label lblTotal;
        private Button btnVaciarCarrito;
        private ComboBox cmbResultados;
        private Button btnAgregarCliente;
        private Button btnAgregarTurno;
        private Label lblClienteVenta;
        private Button btnQuitarCliente;
        private Button btnVerCuentaCorriente;
        private GroupBox grpMetodoPago;
        private FlowLayoutPanel flujoMetodoPago;
        private CheckBox chkEfectivo;
        private CheckBox chkTransferencia;
        private CheckBox chkTarjeta;
        private CheckBox chkBilleteraVirtual;
        private CheckBox chkCuentaCorriente;
        private TabControl tabsPrincipal;
        private TabPage tabVenta;
        private TabPage tabCuentaCorriente;
        private CuentaCorrienteControl cuentaCorrienteControl1;
        private NumericUpDown nudCantidad;
        private Label label2;
    }
}