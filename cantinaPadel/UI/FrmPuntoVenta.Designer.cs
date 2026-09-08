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
            lblCodigoBarras = new Label();
            txtCodigoBarras = new TextBox();
            lblBuscarProducto = new Label();
            txtBuscarProducto = new TextBox();
            dgvResultadosBusqueda = new DataGridView();
            btnAgregarAlCarrito = new Button();
            panel1 = new Panel();
            panel2 = new Panel();
            panel3 = new Panel();
            btnVaciarCarrito = new Button();
            dgvCarrito = new DataGridView();
            label1 = new Label();
            btnQuitarDelCarrito = new Button();
            panel4 = new Panel();
            btnConfirmarVenta = new Button();
            lblTotal = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvResultadosBusqueda).BeginInit();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCarrito).BeginInit();
            panel4.SuspendLayout();
            SuspendLayout();
            // 
            // lblCodigoBarras
            // 
            lblCodigoBarras.AutoSize = true;
            lblCodigoBarras.Location = new Point(19, 21);
            lblCodigoBarras.Name = "lblCodigoBarras";
            lblCodigoBarras.Size = new Size(127, 20);
            lblCodigoBarras.TabIndex = 0;
            lblCodigoBarras.Text = "Código de barras:";
            // 
            // txtCodigoBarras
            // 
            txtCodigoBarras.Location = new Point(152, 18);
            txtCodigoBarras.MaxLength = 100;
            txtCodigoBarras.Name = "txtCodigoBarras";
            txtCodigoBarras.Size = new Size(547, 27);
            txtCodigoBarras.TabIndex = 1;
            // 
            // lblBuscarProducto
            // 
            lblBuscarProducto.AutoSize = true;
            lblBuscarProducto.Location = new Point(30, 19);
            lblBuscarProducto.Name = "lblBuscarProducto";
            lblBuscarProducto.Size = new Size(120, 20);
            lblBuscarProducto.TabIndex = 2;
            lblBuscarProducto.Text = "Buscar producto:";
            // 
            // txtBuscarProducto
            // 
            txtBuscarProducto.Location = new Point(30, 42);
            txtBuscarProducto.MaxLength = 30;
            txtBuscarProducto.Name = "txtBuscarProducto";
            txtBuscarProducto.Size = new Size(289, 27);
            txtBuscarProducto.TabIndex = 3;
            // 
            // dgvResultadosBusqueda
            // 
            dgvResultadosBusqueda.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvResultadosBusqueda.BackgroundColor = SystemColors.ButtonHighlight;
            dgvResultadosBusqueda.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvResultadosBusqueda.Location = new Point(30, 81);
            dgvResultadosBusqueda.Name = "dgvResultadosBusqueda";
            dgvResultadosBusqueda.RowHeadersWidth = 51;
            dgvResultadosBusqueda.Size = new Size(438, 158);
            dgvResultadosBusqueda.TabIndex = 4;
            // 
            // btnAgregarAlCarrito
            // 
            btnAgregarAlCarrito.BackColor = Color.Gold;
            btnAgregarAlCarrito.Location = new Point(325, 245);
            btnAgregarAlCarrito.Name = "btnAgregarAlCarrito";
            btnAgregarAlCarrito.Size = new Size(143, 40);
            btnAgregarAlCarrito.TabIndex = 5;
            btnAgregarAlCarrito.Text = "Agregar al carrito";
            btnAgregarAlCarrito.UseVisualStyleBackColor = false;
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.Info;
            panel1.Controls.Add(lblCodigoBarras);
            panel1.Controls.Add(txtCodigoBarras);
            panel1.Location = new Point(28, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(742, 61);
            panel1.TabIndex = 6;
            // 
            // panel2
            // 
            panel2.BackColor = SystemColors.Info;
            panel2.Controls.Add(dgvResultadosBusqueda);
            panel2.Controls.Add(lblBuscarProducto);
            panel2.Controls.Add(btnAgregarAlCarrito);
            panel2.Controls.Add(txtBuscarProducto);
            panel2.Location = new Point(28, 133);
            panel2.Name = "panel2";
            panel2.Size = new Size(502, 304);
            panel2.TabIndex = 7;
            // 
            // panel3
            // 
            panel3.BackColor = SystemColors.Info;
            panel3.Controls.Add(btnVaciarCarrito);
            panel3.Controls.Add(dgvCarrito);
            panel3.Controls.Add(label1);
            panel3.Controls.Add(btnQuitarDelCarrito);
            panel3.Location = new Point(560, 133);
            panel3.Name = "panel3";
            panel3.Size = new Size(477, 304);
            panel3.TabIndex = 8;
            // 
            // btnVaciarCarrito
            // 
            btnVaciarCarrito.BackColor = Color.FromArgb(255, 224, 192);
            btnVaciarCarrito.Location = new Point(329, 245);
            btnVaciarCarrito.Name = "btnVaciarCarrito";
            btnVaciarCarrito.Size = new Size(119, 40);
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
            dgvCarrito.Location = new Point(29, 42);
            dgvCarrito.Name = "dgvCarrito";
            dgvCarrito.RowHeadersWidth = 51;
            dgvCarrito.Size = new Size(419, 197);
            dgvCarrito.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(29, 19);
            label1.Name = "label1";
            label1.Size = new Size(54, 20);
            label1.TabIndex = 2;
            label1.Text = "Carrito";
            // 
            // btnQuitarDelCarrito
            // 
            btnQuitarDelCarrito.BackColor = Color.FromArgb(254, 243, 199);
            btnQuitarDelCarrito.Location = new Point(29, 245);
            btnQuitarDelCarrito.Name = "btnQuitarDelCarrito";
            btnQuitarDelCarrito.Size = new Size(120, 40);
            btnQuitarDelCarrito.TabIndex = 5;
            btnQuitarDelCarrito.Text = "Quitar";
            btnQuitarDelCarrito.UseVisualStyleBackColor = false;
            // 
            // panel4
            // 
            panel4.BackColor = SystemColors.Info;
            panel4.Controls.Add(btnConfirmarVenta);
            panel4.Controls.Add(lblTotal);
            panel4.Location = new Point(28, 469);
            panel4.Name = "panel4";
            panel4.Size = new Size(1009, 62);
            panel4.TabIndex = 9;
            // 
            // btnConfirmarVenta
            // 
            btnConfirmarVenta.BackColor = Color.FromArgb(255, 255, 192);
            btnConfirmarVenta.FlatStyle = FlatStyle.System;
            btnConfirmarVenta.Location = new Point(828, 7);
            btnConfirmarVenta.Name = "btnConfirmarVenta";
            btnConfirmarVenta.Size = new Size(152, 45);
            btnConfirmarVenta.TabIndex = 1;
            btnConfirmarVenta.Text = "Confirmar venta";
            btnConfirmarVenta.UseVisualStyleBackColor = false;
            // 
            // lblTotal
            // 
            lblTotal.AutoSize = true;
            lblTotal.BackColor = Color.Transparent;
            lblTotal.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotal.Location = new Point(30, 19);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(92, 20);
            lblTotal.TabIndex = 0;
            lblTotal.Text = "Total: $0,00";
            // 
            // FrmPuntoVenta
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Gold;
            ClientSize = new Size(1131, 574);
            Controls.Add(panel4);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Name = "FrmPuntoVenta";
            Text = "FrmPuntoVenta";
            ((System.ComponentModel.ISupportInitialize)dgvResultadosBusqueda).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCarrito).EndInit();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label lblCodigoBarras;
        private TextBox txtCodigoBarras;
        private Label lblBuscarProducto;
        private TextBox txtBuscarProducto;
        private DataGridView dgvResultadosBusqueda;
        private Button btnAgregarAlCarrito;
        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        private DataGridView dgvCarrito;
        private Label label1;
        private Button btnQuitarDelCarrito;
        private Panel panel4;
        private Button btnConfirmarVenta;
        private Label lblTotal;
        private Button btnVaciarCarrito;
    }
}