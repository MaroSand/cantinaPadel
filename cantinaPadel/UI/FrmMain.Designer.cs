namespace cantinaPadel
{
    partial class FrmMain
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
            pnlSidebar = new Panel();
            pnlSidebarFooter = new Panel();
            btnCerrarSesion = new Button();
            pnlNav = new Panel();
            btnReportes = new Button();
            btnCaja = new Button();
            btnCanchas = new Button();
            btnTurnos = new Button();
            btnCompras = new Button();
            btnStock = new Button();
            btnPuntoVenta = new Button();
            btnProveedores = new Button();
            btnEmpleados = new Button();
            btnClientes = new Button();
            btnInicio = new Button();
            pnlSidebarHeader = new Panel();
            lblRolUsuario = new Label();
            lblSistema = new Label();
            pnlContenido = new Panel();
            pnlTopbar = new Panel();
            lblUsuario = new Label();
            lblTituloModulo = new Label();
            pnlSidebar.SuspendLayout();
            pnlSidebarFooter.SuspendLayout();
            pnlNav.SuspendLayout();
            pnlSidebarHeader.SuspendLayout();
            pnlContenido.SuspendLayout();
            pnlTopbar.SuspendLayout();
            SuspendLayout();
            // 
            // pnlSidebar
            // 
            pnlSidebar.BackColor = Color.White;
            pnlSidebar.Controls.Add(pnlSidebarFooter);
            pnlSidebar.Controls.Add(pnlNav);
            pnlSidebar.Controls.Add(pnlSidebarHeader);
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Location = new Point(0, 0);
            pnlSidebar.Margin = new Padding(5, 6, 5, 6);
            pnlSidebar.Name = "pnlSidebar";
            pnlSidebar.Size = new Size(372, 1286);
            pnlSidebar.TabIndex = 0;
            // 
            // pnlSidebarFooter
            // 
            pnlSidebarFooter.Controls.Add(btnCerrarSesion);
            pnlSidebarFooter.Dock = DockStyle.Bottom;
            pnlSidebarFooter.Location = new Point(0, 1179);
            pnlSidebarFooter.Margin = new Padding(5, 6, 5, 6);
            pnlSidebarFooter.Name = "pnlSidebarFooter";
            pnlSidebarFooter.Size = new Size(372, 107);
            pnlSidebarFooter.TabIndex = 2;
            // 
            // btnCerrarSesion
            // 
            btnCerrarSesion.BackColor = Color.White;
            btnCerrarSesion.Dock = DockStyle.Fill;
            btnCerrarSesion.FlatAppearance.BorderSize = 0;
            btnCerrarSesion.FlatStyle = FlatStyle.Flat;
            btnCerrarSesion.ForeColor = Color.Red;
            btnCerrarSesion.Location = new Point(0, 0);
            btnCerrarSesion.Margin = new Padding(5, 6, 5, 6);
            btnCerrarSesion.Name = "btnCerrarSesion";
            btnCerrarSesion.Padding = new Padding(18, 0, 0, 0);
            btnCerrarSesion.Size = new Size(372, 107);
            btnCerrarSesion.TabIndex = 5;
            btnCerrarSesion.Text = "Cerrar sesión";
            btnCerrarSesion.TextAlign = ContentAlignment.MiddleLeft;
            btnCerrarSesion.UseVisualStyleBackColor = false;
            btnCerrarSesion.Click += btnCerrarSesion_Click;
            // 
            // pnlNav
            // 
            pnlNav.Controls.Add(btnReportes);
            pnlNav.Controls.Add(btnCaja);
            pnlNav.Controls.Add(btnCanchas);
            pnlNav.Controls.Add(btnTurnos);
            pnlNav.Controls.Add(btnCompras);
            pnlNav.Controls.Add(btnStock);
            pnlNav.Controls.Add(btnPuntoVenta);
            pnlNav.Controls.Add(btnProveedores);
            pnlNav.Controls.Add(btnEmpleados);
            pnlNav.Controls.Add(btnClientes);
            pnlNav.Controls.Add(btnInicio);
            pnlNav.Dock = DockStyle.Fill;
            pnlNav.Location = new Point(0, 149);
            pnlNav.Margin = new Padding(5, 6, 5, 6);
            pnlNav.Name = "pnlNav";
            pnlNav.Size = new Size(372, 1137);
            pnlNav.TabIndex = 1;
            // 
            // btnReportes
            // 
            btnReportes.BackColor = SystemColors.Info;
            btnReportes.Dock = DockStyle.Top;
            btnReportes.FlatAppearance.BorderSize = 0;
            btnReportes.FlatStyle = FlatStyle.Flat;
            btnReportes.Location = new Point(0, 960);
            btnReportes.Margin = new Padding(5, 6, 5, 6);
            btnReportes.Name = "btnReportes";
            btnReportes.Padding = new Padding(18, 0, 0, 0);
            btnReportes.Size = new Size(372, 96);
            btnReportes.TabIndex = 10;
            btnReportes.Text = "Reportes";
            btnReportes.TextAlign = ContentAlignment.MiddleLeft;
            btnReportes.UseVisualStyleBackColor = false;
            btnReportes.Click += btnReportes_Click;
            // 
            // btnCaja
            // 
            btnCaja.BackColor = Color.White;
            btnCaja.Dock = DockStyle.Top;
            btnCaja.FlatAppearance.BorderSize = 0;
            btnCaja.FlatStyle = FlatStyle.Flat;
            btnCaja.Location = new Point(0, 864);
            btnCaja.Margin = new Padding(5, 6, 5, 6);
            btnCaja.Name = "btnCaja";
            btnCaja.Padding = new Padding(18, 0, 0, 0);
            btnCaja.Size = new Size(372, 96);
            btnCaja.TabIndex = 7;
            btnCaja.Text = "Caja";
            btnCaja.TextAlign = ContentAlignment.MiddleLeft;
            btnCaja.UseVisualStyleBackColor = false;
            btnCaja.Click += btnCaja_Click;
            // 
            // btnCanchas
            // 
            btnCanchas.BackColor = SystemColors.Info;
            btnCanchas.Dock = DockStyle.Top;
            btnCanchas.FlatAppearance.BorderSize = 0;
            btnCanchas.FlatStyle = FlatStyle.Flat;
            btnCanchas.Location = new Point(0, 768);
            btnCanchas.Margin = new Padding(5, 6, 5, 6);
            btnCanchas.Name = "btnCanchas";
            btnCanchas.Padding = new Padding(18, 0, 0, 0);
            btnCanchas.Size = new Size(372, 96);
            btnCanchas.TabIndex = 6;
            btnCanchas.Text = "Canchas";
            btnCanchas.TextAlign = ContentAlignment.MiddleLeft;
            btnCanchas.UseVisualStyleBackColor = false;
            btnCanchas.Click += btnCanchas_Click;
            // 
            // btnTurnos
            // 
            btnTurnos.BackColor = Color.White;
            btnTurnos.Dock = DockStyle.Top;
            btnTurnos.FlatAppearance.BorderSize = 0;
            btnTurnos.FlatStyle = FlatStyle.Flat;
            btnTurnos.Location = new Point(0, 672);
            btnTurnos.Margin = new Padding(5, 6, 5, 6);
            btnTurnos.Name = "btnTurnos";
            btnTurnos.Padding = new Padding(18, 0, 0, 0);
            btnTurnos.Size = new Size(372, 96);
            btnTurnos.TabIndex = 5;
            btnTurnos.Text = "Turnos";
            btnTurnos.TextAlign = ContentAlignment.MiddleLeft;
            btnTurnos.UseVisualStyleBackColor = false;
            btnTurnos.Click += btnTurnos_Click;
            // 
            // btnCompras
            // 
            btnCompras.BackColor = SystemColors.Info;
            btnCompras.Dock = DockStyle.Top;
            btnCompras.FlatAppearance.BorderSize = 0;
            btnCompras.FlatStyle = FlatStyle.Flat;
            btnCompras.Location = new Point(0, 576);
            btnCompras.Margin = new Padding(5, 6, 5, 6);
            btnCompras.Name = "btnCompras";
            btnCompras.Padding = new Padding(18, 0, 0, 0);
            btnCompras.Size = new Size(372, 96);
            btnCompras.TabIndex = 4;
            btnCompras.Text = "Compras";
            btnCompras.TextAlign = ContentAlignment.MiddleLeft;
            btnCompras.UseVisualStyleBackColor = false;
            btnCompras.Click += btnCompras_Click;
            // 
            // btnStock
            // 
            btnStock.BackColor = Color.White;
            btnStock.Dock = DockStyle.Top;
            btnStock.FlatAppearance.BorderSize = 0;
            btnStock.FlatStyle = FlatStyle.Flat;
            btnStock.Location = new Point(0, 480);
            btnStock.Margin = new Padding(5, 6, 5, 6);
            btnStock.Name = "btnStock";
            btnStock.Padding = new Padding(18, 0, 0, 0);
            btnStock.Size = new Size(372, 96);
            btnStock.TabIndex = 3;
            btnStock.Text = "Inventario";
            btnStock.TextAlign = ContentAlignment.MiddleLeft;
            btnStock.UseVisualStyleBackColor = false;
            btnStock.Click += btnStock_Click;
            // 
            // btnPuntoVenta
            // 
            btnPuntoVenta.BackColor = SystemColors.Info;
            btnPuntoVenta.Dock = DockStyle.Top;
            btnPuntoVenta.FlatAppearance.BorderSize = 0;
            btnPuntoVenta.FlatStyle = FlatStyle.Flat;
            btnPuntoVenta.Location = new Point(0, 384);
            btnPuntoVenta.Margin = new Padding(5, 6, 5, 6);
            btnPuntoVenta.Name = "btnPuntoVenta";
            btnPuntoVenta.Padding = new Padding(18, 0, 0, 0);
            btnPuntoVenta.Size = new Size(372, 96);
            btnPuntoVenta.TabIndex = 2;
            btnPuntoVenta.Text = "Punto de Venta";
            btnPuntoVenta.TextAlign = ContentAlignment.MiddleLeft;
            btnPuntoVenta.UseVisualStyleBackColor = false;
            btnPuntoVenta.Click += btnPuntoVenta_Click;
            // 
            // btnProveedores
            // 
            btnProveedores.BackColor = Color.White;
            btnProveedores.Dock = DockStyle.Top;
            btnProveedores.FlatAppearance.BorderSize = 0;
            btnProveedores.FlatStyle = FlatStyle.Flat;
            btnProveedores.Location = new Point(0, 288);
            btnProveedores.Margin = new Padding(5, 6, 5, 6);
            btnProveedores.Name = "btnProveedores";
            btnProveedores.Padding = new Padding(18, 0, 0, 0);
            btnProveedores.Size = new Size(372, 96);
            btnProveedores.TabIndex = 8;
            btnProveedores.Text = "Proveedores";
            btnProveedores.TextAlign = ContentAlignment.MiddleLeft;
            btnProveedores.UseVisualStyleBackColor = false;
            btnProveedores.Click += btnProveedores_Click;
            // 
            // btnEmpleados
            // 
            btnEmpleados.BackColor = SystemColors.Info;
            btnEmpleados.Dock = DockStyle.Top;
            btnEmpleados.FlatAppearance.BorderSize = 0;
            btnEmpleados.FlatStyle = FlatStyle.Flat;
            btnEmpleados.Location = new Point(0, 192);
            btnEmpleados.Margin = new Padding(5, 6, 5, 6);
            btnEmpleados.Name = "btnEmpleados";
            btnEmpleados.Padding = new Padding(18, 0, 0, 0);
            btnEmpleados.Size = new Size(372, 96);
            btnEmpleados.TabIndex = 9;
            btnEmpleados.Text = "Empleados";
            btnEmpleados.TextAlign = ContentAlignment.MiddleLeft;
            btnEmpleados.UseVisualStyleBackColor = false;
            btnEmpleados.Click += btnEmpleados_Click;
            // 
            // btnClientes
            // 
            btnClientes.BackColor = Color.White;
            btnClientes.Dock = DockStyle.Top;
            btnClientes.FlatAppearance.BorderSize = 0;
            btnClientes.FlatStyle = FlatStyle.Flat;
            btnClientes.Location = new Point(0, 96);
            btnClientes.Margin = new Padding(5, 6, 5, 6);
            btnClientes.Name = "btnClientes";
            btnClientes.Padding = new Padding(18, 0, 0, 0);
            btnClientes.Size = new Size(372, 96);
            btnClientes.TabIndex = 1;
            btnClientes.Text = "Clientes";
            btnClientes.TextAlign = ContentAlignment.MiddleLeft;
            btnClientes.UseVisualStyleBackColor = false;
            btnClientes.Click += btnClientes_Click;
            // 
            // btnInicio
            // 
            btnInicio.BackColor = SystemColors.Info;
            btnInicio.Dock = DockStyle.Top;
            btnInicio.FlatAppearance.BorderSize = 0;
            btnInicio.FlatStyle = FlatStyle.Flat;
            btnInicio.Location = new Point(0, 0);
            btnInicio.Margin = new Padding(5, 6, 5, 6);
            btnInicio.Name = "btnInicio";
            btnInicio.Padding = new Padding(18, 0, 0, 0);
            btnInicio.Size = new Size(372, 96);
            btnInicio.TabIndex = 0;
            btnInicio.Text = "Inicio";
            btnInicio.TextAlign = ContentAlignment.MiddleLeft;
            btnInicio.UseVisualStyleBackColor = false;
            btnInicio.Click += btnInicio_Click;
            // 
            // pnlSidebarHeader
            // 
            pnlSidebarHeader.BackColor = Color.Gold;
            pnlSidebarHeader.Controls.Add(lblRolUsuario);
            pnlSidebarHeader.Controls.Add(lblSistema);
            pnlSidebarHeader.Dock = DockStyle.Top;
            pnlSidebarHeader.Location = new Point(0, 0);
            pnlSidebarHeader.Margin = new Padding(5, 6, 5, 6);
            pnlSidebarHeader.Name = "pnlSidebarHeader";
            pnlSidebarHeader.Size = new Size(372, 149);
            pnlSidebarHeader.TabIndex = 0;
            // 
            // lblRolUsuario
            // 
            lblRolUsuario.AutoSize = true;
            lblRolUsuario.Font = new Font("Microsoft Sans Serif", 8.25F);
            lblRolUsuario.ForeColor = Color.Gray;
            lblRolUsuario.Location = new Point(23, 78);
            lblRolUsuario.Margin = new Padding(5, 0, 5, 0);
            lblRolUsuario.Name = "lblRolUsuario";
            lblRolUsuario.Size = new Size(147, 26);
            lblRolUsuario.TabIndex = 1;
            lblRolUsuario.Text = "Administrador";
            // 
            // lblSistema
            // 
            lblSistema.AutoSize = true;
            lblSistema.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSistema.Location = new Point(23, 26);
            lblSistema.Margin = new Padding(5, 0, 5, 0);
            lblSistema.Name = "lblSistema";
            lblSistema.Size = new Size(180, 29);
            lblSistema.TabIndex = 0;
            lblSistema.Text = "Cantina y Pádel";
            // 
            // pnlContenido
            // 
            pnlContenido.BackColor = Color.White;
            pnlContenido.Controls.Add(pnlTopbar);
            pnlContenido.Dock = DockStyle.Fill;
            pnlContenido.Location = new Point(372, 0);
            pnlContenido.Margin = new Padding(5, 6, 5, 6);
            pnlContenido.Name = "pnlContenido";
            pnlContenido.Size = new Size(1638, 1286);
            pnlContenido.TabIndex = 1;
            // 
            // pnlTopbar
            // 
            pnlTopbar.BackColor = SystemColors.Info;
            pnlTopbar.Controls.Add(lblUsuario);
            pnlTopbar.Controls.Add(lblTituloModulo);
            pnlTopbar.Dock = DockStyle.Top;
            pnlTopbar.Location = new Point(0, 0);
            pnlTopbar.Margin = new Padding(5, 6, 5, 6);
            pnlTopbar.Name = "pnlTopbar";
            pnlTopbar.Size = new Size(1638, 107);
            pnlTopbar.TabIndex = 0;
            // 
            // lblUsuario
            // 
            lblUsuario.AutoSize = true;
            lblUsuario.Font = new Font("Microsoft Sans Serif", 8.25F);
            lblUsuario.ForeColor = Color.Gray;
            lblUsuario.Location = new Point(1443, 26);
            lblUsuario.Margin = new Padding(5, 0, 5, 0);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(138, 26);
            lblUsuario.TabIndex = 1;
            lblUsuario.Text = "Usuario · Rol";
            // 
            // lblTituloModulo
            // 
            lblTituloModulo.AutoSize = true;
            lblTituloModulo.Font = new Font("Microsoft Sans Serif", 8.25F);
            lblTituloModulo.ForeColor = Color.Black;
            lblTituloModulo.Location = new Point(297, 26);
            lblTituloModulo.Margin = new Padding(5, 0, 5, 0);
            lblTituloModulo.Name = "lblTituloModulo";
            lblTituloModulo.Size = new Size(124, 26);
            lblTituloModulo.TabIndex = 0;
            lblTituloModulo.Text = "Dashboard ";
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(2010, 1286);
            Controls.Add(pnlContenido);
            Controls.Add(pnlSidebar);
            Margin = new Padding(5, 6, 5, 6);
            MinimumSize = new Size(2023, 1319);
            Name = "FrmMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Cantina y Pádel";
            Load += FrmMain_Load;
            pnlSidebar.ResumeLayout(false);
            pnlSidebarFooter.ResumeLayout(false);
            pnlNav.ResumeLayout(false);
            pnlSidebarHeader.ResumeLayout(false);
            pnlSidebarHeader.PerformLayout();
            pnlContenido.ResumeLayout(false);
            pnlTopbar.ResumeLayout(false);
            pnlTopbar.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlSidebar;
        private Panel pnlContenido;
        private Panel pnlTopbar;
        private Label lblTituloModulo;
        private Label lblUsuario;
        private Panel pnlSidebarHeader;
        private Label lblRolUsuario;
        private Label lblSistema;
        private Panel pnlNav;
        private Button btnInicio;
        private Button btnProveedores;
        private Button btnCaja;
        private Button btnCanchas;
        private Button btnTurnos;
        private Button btnCompras;
        private Button btnStock;
        private Button btnPuntoVenta;
        private Button btnClientes;
        private Panel pnlSidebarFooter;
        private Button btnCerrarSesion;
        private Button btnReportes;
        private Button btnEmpleados;
    }
}