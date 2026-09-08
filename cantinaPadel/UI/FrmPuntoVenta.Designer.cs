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
            dgvResultadosBusqueda = new DataGridView();
            btnAgregarAlCarrito = new Button();
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
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCarrito).BeginInit();
            panel4.SuspendLayout();
            SuspendLayout();
            // 
            // lblBuscarProducto
            // 
            lblBuscarProducto.AutoSize = true;
            lblBuscarProducto.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblBuscarProducto.Location = new Point(30, 19);
            lblBuscarProducto.Name = "lblBuscarProducto";
            lblBuscarProducto.Size = new Size(266, 20);
            lblBuscarProducto.TabIndex = 2;
            lblBuscarProducto.Text = "Buscar producto (nombre o código de barras):";
            // 
            // txtBuscarProducto
            // 
            txtBuscarProducto.Location = new Point(30, 42);
            txtBuscarProducto.MaxLength = 100;
            txtBuscarProducto.Name = "txtBuscarProducto";
            txtBuscarProducto.Size = new Size(951, 27);
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
            dgvResultadosBusqueda.Size = new Size(951, 158);
            dgvResultadosBusqueda.TabIndex = 4;
            // 
            // btnAgregarAlCarrito
            // 
            btnAgregarAlCarrito.BackColor = SystemColors.Info;
            btnAgregarAlCarrito.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAgregarAlCarrito.Location = new Point(303, 245);
            btnAgregarAlCarrito.Name = "btnAgregarAlCarrito";
            btnAgregarAlCarrito.Size = new Size(355, 40);
            btnAgregarAlCarrito.TabIndex = 5;
            btnAgregarAlCarrito.Text = "Agregar al carrito";
            btnAgregarAlCarrito.UseVisualStyleBackColor = false;
            // 
            // panel2
            // 
            panel2.BackColor = Color.Gold;
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(dgvResultadosBusqueda);
            panel2.Controls.Add(lblBuscarProducto);
            panel2.Controls.Add(btnAgregarAlCarrito);
            panel2.Controls.Add(txtBuscarProducto);
            panel2.Location = new Point(28, 49);
            panel2.Name = "panel2";
            panel2.Size = new Size(1031, 304);
            panel2.TabIndex = 7;
            // 
            // panel3
            // 
            panel3.BackColor = Color.Gold;
            panel3.BorderStyle = BorderStyle.FixedSingle;
            panel3.Controls.Add(btnVaciarCarrito);
            panel3.Controls.Add(dgvCarrito);
            panel3.Controls.Add(label1);
            panel3.Controls.Add(btnQuitarDelCarrito);
            panel3.Location = new Point(28, 392);
            panel3.Name = "panel3";
            panel3.Size = new Size(1031, 299);
            panel3.TabIndex = 8;
            // 
            // btnVaciarCarrito
            // 
            btnVaciarCarrito.BackColor = Color.White;
            btnVaciarCarrito.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnVaciarCarrito.Location = new Point(552, 245);
            btnVaciarCarrito.Name = "btnVaciarCarrito";
            btnVaciarCarrito.Size = new Size(314, 40);
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
            btnQuitarDelCarrito.BackColor = Color.FromArgb(255, 224, 192);
            btnQuitarDelCarrito.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnQuitarDelCarrito.Location = new Point(192, 245);
            btnQuitarDelCarrito.Name = "btnQuitarDelCarrito";
            btnQuitarDelCarrito.Size = new Size(296, 40);
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
            panel4.Location = new Point(28, 729);
            panel4.Name = "panel4";
            panel4.Size = new Size(1031, 66);
            panel4.TabIndex = 9;
            // 
            // btnConfirmarVenta
            // 
            btnConfirmarVenta.BackColor = Color.FromArgb(255, 255, 192);
            btnConfirmarVenta.FlatStyle = FlatStyle.System;
            btnConfirmarVenta.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnConfirmarVenta.Location = new Point(829, 14);
            btnConfirmarVenta.Name = "btnConfirmarVenta";
            btnConfirmarVenta.Size = new Size(152, 42);
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
            // FrmPuntoVenta
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            BackColor = Color.FromArgb(255, 255, 192);
            ClientSize = new Size(1172, 849);
            Controls.Add(panel4);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Name = "FrmPuntoVenta";
            Text = "FrmPuntoVenta";
            ((System.ComponentModel.ISupportInitialize)dgvResultadosBusqueda).EndInit();
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

        private Label lblBuscarProducto;
        private TextBox txtBuscarProducto;
        private DataGridView dgvResultadosBusqueda;
        private Button btnAgregarAlCarrito;
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