namespace PROJECT_CK
{
    partial class Chitiet
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
            this.lblTenxe = new System.Windows.Forms.Label();
            this.txtTenSp = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.lblTongsoluong = new System.Windows.Forms.Label();
            this.tang = new System.Windows.Forms.Label();
            this.giam = new System.Windows.Forms.Label();
            this.txtSoluong = new Guna.UI2.WinForms.Guna2TextBox();
            this.btnThemdonhang = new Guna.UI2.WinForms.Guna2Button();
            this.txtMota = new Guna.UI2.WinForms.Guna2TextBox();
            this.SuspendLayout();
            // 
            // lblTenxe
            // 
            this.lblTenxe.AutoSize = true;
            this.lblTenxe.Font = new System.Drawing.Font("Segoe UI Black", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTenxe.ForeColor = System.Drawing.Color.Black;
            this.lblTenxe.Location = new System.Drawing.Point(185, 23);
            this.lblTenxe.Name = "lblTenxe";
            this.lblTenxe.Size = new System.Drawing.Size(0, 45);
            this.lblTenxe.TabIndex = 12;
            this.lblTenxe.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtTenSp
            // 
            this.txtTenSp.AutoSize = false;
            this.txtTenSp.BackColor = System.Drawing.Color.Transparent;
            this.txtTenSp.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTenSp.ForeColor = System.Drawing.Color.Black;
            this.txtTenSp.Location = new System.Drawing.Point(12, 16);
            this.txtTenSp.Name = "txtTenSp";
            this.txtTenSp.Size = new System.Drawing.Size(538, 49);
            this.txtTenSp.TabIndex = 34;
            this.txtTenSp.Text = "TênSP";
            this.txtTenSp.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.txtTenSp.Click += new System.EventHandler(this.txtTenSp_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(79, 320);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 25);
            this.label3.TabIndex = 33;
            this.label3.Text = "Số lượng";
            // 
            // lblTongsoluong
            // 
            this.lblTongsoluong.AutoSize = true;
            this.lblTongsoluong.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTongsoluong.Location = new System.Drawing.Point(197, 320);
            this.lblTongsoluong.Name = "lblTongsoluong";
            this.lblTongsoluong.Size = new System.Drawing.Size(176, 25);
            this.lblTongsoluong.TabIndex = 30;
            this.lblTongsoluong.Text = "Số lượng trong kho";
            // 
            // tang
            // 
            this.tang.AutoSize = true;
            this.tang.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tang.Location = new System.Drawing.Point(176, 395);
            this.tang.Name = "tang";
            this.tang.Size = new System.Drawing.Size(27, 29);
            this.tang.TabIndex = 29;
            this.tang.Text = "+";
            // 
            // giam
            // 
            this.giam.AutoSize = true;
            this.giam.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.giam.Location = new System.Drawing.Point(42, 395);
            this.giam.Name = "giam";
            this.giam.Size = new System.Drawing.Size(21, 29);
            this.giam.TabIndex = 28;
            this.giam.Text = "-";
            this.giam.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtSoluong
            // 
            this.txtSoluong.BorderRadius = 10;
            this.txtSoluong.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtSoluong.DefaultText = "";
            this.txtSoluong.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtSoluong.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtSoluong.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtSoluong.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtSoluong.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtSoluong.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSoluong.ForeColor = System.Drawing.Color.Red;
            this.txtSoluong.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtSoluong.Location = new System.Drawing.Point(70, 395);
            this.txtSoluong.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.txtSoluong.Name = "txtSoluong";
            this.txtSoluong.PlaceholderForeColor = System.Drawing.Color.Red;
            this.txtSoluong.PlaceholderText = "";
            this.txtSoluong.SelectedText = "";
            this.txtSoluong.Size = new System.Drawing.Size(99, 29);
            this.txtSoluong.TabIndex = 27;
            this.txtSoluong.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnThemdonhang
            // 
            this.btnThemdonhang.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnThemdonhang.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnThemdonhang.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnThemdonhang.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnThemdonhang.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnThemdonhang.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnThemdonhang.ForeColor = System.Drawing.Color.White;
            this.btnThemdonhang.Location = new System.Drawing.Point(219, 383);
            this.btnThemdonhang.Name = "btnThemdonhang";
            this.btnThemdonhang.Size = new System.Drawing.Size(240, 57);
            this.btnThemdonhang.TabIndex = 26;
            this.btnThemdonhang.Text = "Thêm vào đơn hàng";
            // 
            // txtMota
            // 
            this.txtMota.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtMota.DefaultText = "";
            this.txtMota.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtMota.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtMota.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtMota.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtMota.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtMota.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMota.ForeColor = System.Drawing.Color.Black;
            this.txtMota.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtMota.Location = new System.Drawing.Point(47, 74);
            this.txtMota.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.txtMota.Multiline = true;
            this.txtMota.Name = "txtMota";
            this.txtMota.PlaceholderText = "";
            this.txtMota.ReadOnly = true;
            this.txtMota.SelectedText = "";
            this.txtMota.Size = new System.Drawing.Size(465, 223);
            this.txtMota.TabIndex = 23;
            // 
            // Chitiet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(562, 476);
            this.Controls.Add(this.txtTenSp);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblTongsoluong);
            this.Controls.Add(this.tang);
            this.Controls.Add(this.giam);
            this.Controls.Add(this.txtSoluong);
            this.Controls.Add(this.btnThemdonhang);
            this.Controls.Add(this.txtMota);
            this.Controls.Add(this.lblTenxe);
            this.Name = "Chitiet";
            this.Text = "Chitiet";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTenxe;
        private Guna.UI2.WinForms.Guna2HtmlLabel txtTenSp;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblTongsoluong;
        private System.Windows.Forms.Label tang;
        private System.Windows.Forms.Label giam;
        private Guna.UI2.WinForms.Guna2TextBox txtSoluong;
        private Guna.UI2.WinForms.Guna2Button btnThemdonhang;
        private Guna.UI2.WinForms.Guna2TextBox txtMota;
    }
}