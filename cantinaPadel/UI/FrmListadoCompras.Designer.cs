namespace cantinaPadel.UI
{
    partial class FrmListadoCompras
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
            panelAcciones = new Panel();
            btnAnular = new Button();
            btnDetalle = new Button();
            btnNueva = new Button();
            lblResumen = new Label();
            panelFiltros = new Panel();
            btnLimpiar = new Button();
            chkIncluirAnuladas = new CheckBox();
            txtProducto = new TextBox();
            lblProducto = new Label();
            dtpHasta = new DateTimePicker();
            lblHasta = new Label();
            dtpDesde = new DateTimePicker();
            lblDesde = new Label();
            cmbProveedor = new ComboBox();
            lblProveedor = new Label();
            dgvCompras = new DataGridView();
            panelAcciones.SuspendLayout();
            panelFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCompras).BeginInit();
            SuspendLayout();
            // 
            // panelAcciones
            // 
            panelAcciones.BackColor = SystemColors.Info;
            panelAcciones.Controls.Add(btnAnular);
            panelAcciones.Controls.Add(btnDetalle);
            panelAcciones.Controls.Add(btnNueva);
            panelAcciones.Controls.Add(lblResumen);
            panelAcciones.Dock = DockStyle.Bottom;
            panelAcciones.Location = new Point(0, 364);
            panelAcciones.Name = "panelAcciones";
            panelAcciones.Size = new Size(1344, 123);
            panelAcciones.TabIndex = 0;
            // 
            // btnAnular
            // 
            btnAnular.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAnular.BackColor = Color.IndianRed;
            btnAnular.FlatStyle = FlatStyle.Flat;
            btnAnular.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAnular.ForeColor = Color.White;
            btnAnular.Location = new Point(434, 36);
            btnAnular.Name = "btnAnular";
            btnAnular.Size = new Size(270, 46);
            btnAnular.TabIndex = 1;
            btnAnular.Text = "Anular compra";
            btnAnular.UseVisualStyleBackColor = false;
            // 
            // btnDetalle
            // 
            btnDetalle.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDetalle.BackColor = Color.Khaki;
            btnDetalle.FlatStyle = FlatStyle.Flat;
            btnDetalle.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnDetalle.Location = new Point(730, 36);
            btnDetalle.Name = "btnDetalle";
            btnDetalle.Size = new Size(270, 46);
            btnDetalle.TabIndex = 2;
            btnDetalle.Text = "Ver detalle";
            btnDetalle.UseVisualStyleBackColor = false;
            // 
            // btnNueva
            // 
            btnNueva.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnNueva.BackColor = Color.ForestGreen;
            btnNueva.FlatStyle = FlatStyle.Flat;
            btnNueva.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnNueva.ForeColor = Color.White;
            btnNueva.Location = new Point(1031, 36);
            btnNueva.Name = "btnNueva";
            btnNueva.Size = new Size(270, 46);
            btnNueva.TabIndex = 3;
            btnNueva.Text = "Nueva compra";
            btnNueva.UseVisualStyleBackColor = false;
            // 
            // lblResumen
            // 
            lblResumen.Location = new Point(34, 20);
            lblResumen.Name = "lblResumen";
            lblResumen.Size = new Size(670, 40);
            lblResumen.TabIndex = 0;
            lblResumen.Text = "label1";
            lblResumen.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panelFiltros
            // 
            panelFiltros.BackColor = Color.Gold;
            panelFiltros.Controls.Add(btnLimpiar);
            panelFiltros.Controls.Add(chkIncluirAnuladas);
            panelFiltros.Controls.Add(txtProducto);
            panelFiltros.Controls.Add(lblProducto);
            panelFiltros.Controls.Add(dtpHasta);
            panelFiltros.Controls.Add(lblHasta);
            panelFiltros.Controls.Add(dtpDesde);
            panelFiltros.Controls.Add(lblDesde);
            panelFiltros.Controls.Add(cmbProveedor);
            panelFiltros.Controls.Add(lblProveedor);
            panelFiltros.Dock = DockStyle.Top;
            panelFiltros.Location = new Point(0, 0);
            panelFiltros.Name = "panelFiltros";
            panelFiltros.Size = new Size(1344, 84);
            panelFiltros.TabIndex = 1;
            // 
            // btnLimpiar
            // 
            btnLimpiar.BackColor = Color.White;
            btnLimpiar.FlatStyle = FlatStyle.Flat;
            btnLimpiar.Location = new Point(1150, 87);
            btnLimpiar.Name = "btnLimpiar";
            btnLimpiar.Size = new Size(170, 46);
            btnLimpiar.TabIndex = 9;
            btnLimpiar.Text = "Restablecer filtros";
            btnLimpiar.UseVisualStyleBackColor = false;
            // 
            // chkIncluirAnuladas
            // 
            chkIncluirAnuladas.AutoSize = true;
            chkIncluirAnuladas.Location = new Point(620, 89);
            chkIncluirAnuladas.Name = "chkIncluirAnuladas";
            chkIncluirAnuladas.Size = new Size(134, 24);
            chkIncluirAnuladas.TabIndex = 8;
            chkIncluirAnuladas.Text = "Incluir anuladas";
            chkIncluirAnuladas.UseVisualStyleBackColor = true;
            // 
            // txtProducto
            // 
            txtProducto.Location = new Point(180, 87);
            txtProducto.Name = "txtProducto";
            txtProducto.PlaceholderText = "Nombre del producto...";
            txtProducto.Size = new Size(400, 27);
            txtProducto.TabIndex = 7;
            // 
            // lblProducto
            // 
            lblProducto.AutoSize = true;
            lblProducto.Location = new Point(39, 90);
            lblProducto.Name = "lblProducto";
            lblProducto.Size = new Size(72, 20);
            lblProducto.TabIndex = 6;
            lblProducto.Text = "Producto:";
            // 
            // dtpHasta
            // 
            dtpHasta.Location = new Point(1090, 26);
            dtpHasta.Name = "dtpHasta";
            dtpHasta.Size = new Size(230, 27);
            dtpHasta.TabIndex = 5;
            // 
            // lblHasta
            // 
            lblHasta.AutoSize = true;
            lblHasta.Location = new Point(990, 30);
            lblHasta.Name = "lblHasta";
            lblHasta.Size = new Size(50, 20);
            lblHasta.TabIndex = 4;
            lblHasta.Text = "Hasta:";
            // 
            // dtpDesde
            // 
            dtpDesde.Location = new Point(730, 26);
            dtpDesde.Name = "dtpDesde";
            dtpDesde.Size = new Size(230, 27);
            dtpDesde.TabIndex = 3;
            // 
            // lblDesde
            // 
            lblDesde.AutoSize = true;
            lblDesde.Location = new Point(620, 30);
            lblDesde.Name = "lblDesde";
            lblDesde.Size = new Size(54, 20);
            lblDesde.TabIndex = 2;
            lblDesde.Text = "Desde:";
            // 
            // cmbProveedor
            // 
            cmbProveedor.FormattingEnabled = true;
            cmbProveedor.Location = new Point(180, 26);
            cmbProveedor.Name = "cmbProveedor";
            cmbProveedor.Size = new Size(400, 28);
            cmbProveedor.TabIndex = 1;
            // 
            // lblProveedor
            // 
            lblProveedor.AutoSize = true;
            lblProveedor.Location = new Point(34, 30);
            lblProveedor.Name = "lblProveedor";
            lblProveedor.Size = new Size(80, 20);
            lblProveedor.TabIndex = 0;
            lblProveedor.Text = "Proveedor:";
            // 
            // dgvCompras
            // 
            dgvCompras.BackgroundColor = Color.White;
            dgvCompras.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCompras.Dock = DockStyle.Fill;
            dgvCompras.GridColor = SystemColors.Menu;
            dgvCompras.Location = new Point(0, 84);
            dgvCompras.Name = "dgvCompras";
            dgvCompras.RowHeadersWidth = 51;
            dgvCompras.Size = new Size(1344, 280);
            dgvCompras.TabIndex = 2;
            // 
            // FrmListadoCompras
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1344, 487);
            Controls.Add(dgvCompras);
            Controls.Add(panelFiltros);
            Controls.Add(panelAcciones);
            Name = "FrmListadoCompras";
            Text = "Listado de Compras";
            panelAcciones.ResumeLayout(false);
            panelFiltros.ResumeLayout(false);
            panelFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCompras).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelAcciones;
        private Panel panelFiltros;
        private DataGridView dgvCompras;
        private Label lblDesde;
        private ComboBox cmbProveedor;
        private Label lblProveedor;
        private TextBox txtProducto;
        private Label lblProducto;
        private DateTimePicker dtpHasta;
        private Label lblHasta;
        private DateTimePicker dtpDesde;
        private Label lblResumen;
        private Button btnLimpiar;
        private CheckBox chkIncluirAnuladas;
        private Button btnNueva;
        private Button btnDetalle;
        private Button btnAnular;
    }
}
