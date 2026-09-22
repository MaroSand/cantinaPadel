namespace cantinaPadel.UI
{
    partial class FrmSeleccionComprobante
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
            label1 = new Label();
            panelCuerpo = new Panel();
            lblTotalTexto = new Label();
            lblTotalValor = new Label();
            grpTipo = new GroupBox();
            rbTicket = new RadioButton();
            rbFacturaA = new RadioButton();
            rbFacturaB = new RadioButton();
            rbFacturaC = new RadioButton();
            rbRemito = new RadioButton();
            grpEntrega = new GroupBox();
            rbImprimir = new RadioButton();
            rbNoEmitir = new RadioButton();
            rbEmail = new RadioButton();
            panel1 = new Panel();
            btnCancelar = new Button();
            btnConfirmar = new Button();
            panelHeader.SuspendLayout();
            panelCuerpo.SuspendLayout();
            grpTipo.SuspendLayout();
            grpEntrega.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panelHeader
            // 
            panelHeader.BackColor = Color.Gold;
            panelHeader.Controls.Add(label1);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Name = "panelHeader";
            panelHeader.Size = new Size(404, 50);
            panelHeader.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(80, 9);
            label1.Name = "label1";
            label1.Size = new Size(217, 25);
            label1.TabIndex = 0;
            label1.Text = "Comprobante de venta";
            // 
            // panelCuerpo
            // 
            panelCuerpo.Controls.Add(panel1);
            panelCuerpo.Controls.Add(grpEntrega);
            panelCuerpo.Controls.Add(grpTipo);
            panelCuerpo.Controls.Add(lblTotalValor);
            panelCuerpo.Controls.Add(lblTotalTexto);
            panelCuerpo.Dock = DockStyle.Fill;
            panelCuerpo.Location = new Point(0, 50);
            panelCuerpo.Name = "panelCuerpo";
            panelCuerpo.Size = new Size(404, 431);
            panelCuerpo.TabIndex = 1;
            panelCuerpo.Paint += panelCuerpo_Paint;
            // 
            // lblTotalTexto
            // 
            lblTotalTexto.AutoSize = true;
            lblTotalTexto.Location = new Point(44, 35);
            lblTotalTexto.Name = "lblTotalTexto";
            lblTotalTexto.Size = new Size(85, 15);
            lblTotalTexto.TabIndex = 0;
            lblTotalTexto.Text = "Total a cobrar :";
            // 
            // lblTotalValor
            // 
            lblTotalValor.AutoSize = true;
            lblTotalValor.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotalValor.Location = new Point(219, 35);
            lblTotalValor.Name = "lblTotalValor";
            lblTotalValor.Size = new Size(61, 25);
            lblTotalValor.TabIndex = 1;
            lblTotalValor.Text = "$0,00";
            // 
            // grpTipo
            // 
            grpTipo.Controls.Add(rbRemito);
            grpTipo.Controls.Add(rbFacturaC);
            grpTipo.Controls.Add(rbFacturaB);
            grpTipo.Controls.Add(rbFacturaA);
            grpTipo.Controls.Add(rbTicket);
            grpTipo.Location = new Point(44, 79);
            grpTipo.Name = "grpTipo";
            grpTipo.Size = new Size(275, 140);
            grpTipo.TabIndex = 2;
            grpTipo.TabStop = false;
            grpTipo.Text = "Tipo de comprobante";
            // 
            // rbTicket
            // 
            rbTicket.AutoSize = true;
            rbTicket.Location = new Point(18, 22);
            rbTicket.Name = "rbTicket";
            rbTicket.Size = new Size(57, 19);
            rbTicket.TabIndex = 0;
            rbTicket.TabStop = true;
            rbTicket.Text = "Ticket";
            rbTicket.UseVisualStyleBackColor = true;
            // 
            // rbFacturaA
            // 
            rbFacturaA.AutoSize = true;
            rbFacturaA.Location = new Point(119, 22);
            rbFacturaA.Name = "rbFacturaA";
            rbFacturaA.Size = new Size(75, 19);
            rbFacturaA.TabIndex = 1;
            rbFacturaA.TabStop = true;
            rbFacturaA.Text = "Factura A";
            rbFacturaA.UseVisualStyleBackColor = true;
            // 
            // rbFacturaB
            // 
            rbFacturaB.AutoSize = true;
            rbFacturaB.Location = new Point(18, 65);
            rbFacturaB.Name = "rbFacturaB";
            rbFacturaB.Size = new Size(74, 19);
            rbFacturaB.TabIndex = 2;
            rbFacturaB.TabStop = true;
            rbFacturaB.Text = "Factura B";
            rbFacturaB.UseVisualStyleBackColor = true;
            // 
            // rbFacturaC
            // 
            rbFacturaC.AutoSize = true;
            rbFacturaC.Location = new Point(119, 65);
            rbFacturaC.Name = "rbFacturaC";
            rbFacturaC.Size = new Size(75, 19);
            rbFacturaC.TabIndex = 0;
            rbFacturaC.TabStop = true;
            rbFacturaC.Text = "Factura C";
            rbFacturaC.UseVisualStyleBackColor = true;
            // 
            // rbRemito
            // 
            rbRemito.AutoSize = true;
            rbRemito.Location = new Point(18, 108);
            rbRemito.Name = "rbRemito";
            rbRemito.Size = new Size(66, 19);
            rbRemito.TabIndex = 3;
            rbRemito.TabStop = true;
            rbRemito.Text = "Remito";
            rbRemito.UseVisualStyleBackColor = true;
            // 
            // grpEntrega
            // 
            grpEntrega.Controls.Add(rbEmail);
            grpEntrega.Controls.Add(rbNoEmitir);
            grpEntrega.Controls.Add(rbImprimir);
            grpEntrega.Location = new Point(44, 252);
            grpEntrega.Name = "grpEntrega";
            grpEntrega.Size = new Size(275, 100);
            grpEntrega.TabIndex = 3;
            grpEntrega.TabStop = false;
            grpEntrega.Text = "Entrega al cliente";
            // 
            // rbImprimir
            // 
            rbImprimir.AutoSize = true;
            rbImprimir.Location = new Point(16, 21);
            rbImprimir.Name = "rbImprimir";
            rbImprimir.Size = new Size(71, 19);
            rbImprimir.TabIndex = 0;
            rbImprimir.TabStop = true;
            rbImprimir.Text = "Imprimir";
            rbImprimir.UseVisualStyleBackColor = true;
            // 
            // rbNoEmitir
            // 
            rbNoEmitir.AutoSize = true;
            rbNoEmitir.Location = new Point(12, 57);
            rbNoEmitir.Name = "rbNoEmitir";
            rbNoEmitir.Size = new Size(75, 19);
            rbNoEmitir.TabIndex = 1;
            rbNoEmitir.TabStop = true;
            rbNoEmitir.Text = "No emitir";
            rbNoEmitir.UseVisualStyleBackColor = true;
            // 
            // rbEmail
            // 
            rbEmail.AutoSize = true;
            rbEmail.Location = new Point(119, 21);
            rbEmail.Name = "rbEmail";
            rbEmail.Size = new Size(110, 19);
            rbEmail.TabIndex = 2;
            rbEmail.TabStop = true;
            rbEmail.Text = "Enviar por email";
            rbEmail.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Controls.Add(btnConfirmar);
            panel1.Controls.Add(btnCancelar);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 371);
            panel1.Name = "panel1";
            panel1.Size = new Size(404, 60);
            panel1.TabIndex = 4;
            // 
            // btnCancelar
            // 
            btnCancelar.Location = new Point(71, 14);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(75, 23);
            btnCancelar.TabIndex = 0;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            // 
            // btnConfirmar
            // 
            btnConfirmar.BackColor = SystemColors.ControlLightLight;
            btnConfirmar.Location = new Point(219, 14);
            btnConfirmar.Name = "btnConfirmar";
            btnConfirmar.Size = new Size(75, 23);
            btnConfirmar.TabIndex = 1;
            btnConfirmar.Text = "Confirmar";
            btnConfirmar.UseVisualStyleBackColor = false;
            btnConfirmar.Click += btnConfirmar_Click;
            // 
            // FrmSeleccionComprobante
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(404, 481);
            Controls.Add(panelCuerpo);
            Controls.Add(panelHeader);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmSeleccionComprobante";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Comprobante de venta";
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            panelCuerpo.ResumeLayout(false);
            panelCuerpo.PerformLayout();
            grpTipo.ResumeLayout(false);
            grpTipo.PerformLayout();
            grpEntrega.ResumeLayout(false);
            grpEntrega.PerformLayout();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panelHeader;
        private Label label1;
        private Panel panelCuerpo;
        private Label lblTotalValor;
        private Label lblTotalTexto;
        private GroupBox grpTipo;
        private RadioButton rbFacturaC;
        private RadioButton rbFacturaB;
        private RadioButton rbFacturaA;
        private RadioButton rbTicket;
        private RadioButton rbRemito;
        private GroupBox grpEntrega;
        private RadioButton rbEmail;
        private RadioButton rbNoEmitir;
        private RadioButton rbImprimir;
        private Panel panel1;
        private Button btnConfirmar;
        private Button btnCancelar;
    }
}