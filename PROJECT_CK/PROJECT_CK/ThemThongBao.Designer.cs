namespace PROJECT_CK
{
    partial class ThemThongBao
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
            this.btnLuu = new Guna.UI2.WinForms.Guna2Button();
            this.groupBoxChiTiet = new Guna.UI2.WinForms.Guna2GroupBox();
            this.txtNoiDung = new System.Windows.Forms.RichTextBox();
            this.txtTieuDeThongBao = new Guna.UI2.WinForms.Guna2TextBox();
            this.cbLoaiThongBao = new Guna.UI2.WinForms.Guna2ComboBox();
            this.label40 = new System.Windows.Forms.Label();
            this.groupBoxChiTiet.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLuu
            // 
            this.btnLuu.BorderColor = System.Drawing.Color.Gold;
            this.btnLuu.BorderRadius = 15;
            this.btnLuu.BorderThickness = 3;
            this.btnLuu.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnLuu.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnLuu.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnLuu.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnLuu.FillColor = System.Drawing.Color.ForestGreen;
            this.btnLuu.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLuu.ForeColor = System.Drawing.Color.White;
            this.btnLuu.Location = new System.Drawing.Point(266, 438);
            this.btnLuu.Name = "btnLuu";
            this.btnLuu.Size = new System.Drawing.Size(253, 64);
            this.btnLuu.TabIndex = 46;
            this.btnLuu.Text = "Thêm thông báo";
            this.btnLuu.Click += new System.EventHandler(this.btnThemThongBao_Click);
            // 
            // groupBoxChiTiet
            // 
            this.groupBoxChiTiet.BorderColor = System.Drawing.Color.Black;
            this.groupBoxChiTiet.Controls.Add(this.txtNoiDung);
            this.groupBoxChiTiet.CustomBorderColor = System.Drawing.Color.ForestGreen;
            this.groupBoxChiTiet.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxChiTiet.ForeColor = System.Drawing.Color.White;
            this.groupBoxChiTiet.Location = new System.Drawing.Point(103, 175);
            this.groupBoxChiTiet.Name = "groupBoxChiTiet";
            this.groupBoxChiTiet.Size = new System.Drawing.Size(592, 255);
            this.groupBoxChiTiet.TabIndex = 44;
            this.groupBoxChiTiet.Text = "Nội dung ";
            this.groupBoxChiTiet.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtNoiDung
            // 
            this.txtNoiDung.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNoiDung.ForeColor = System.Drawing.Color.Black;
            this.txtNoiDung.Location = new System.Drawing.Point(19, 58);
            this.txtNoiDung.Name = "txtNoiDung";
            this.txtNoiDung.Size = new System.Drawing.Size(557, 177);
            this.txtNoiDung.TabIndex = 0;
            this.txtNoiDung.Text = "";
            // 
            // txtTieuDeThongBao
            // 
            this.txtTieuDeThongBao.BorderColor = System.Drawing.Color.DarkGreen;
            this.txtTieuDeThongBao.BorderRadius = 10;
            this.txtTieuDeThongBao.BorderThickness = 2;
            this.txtTieuDeThongBao.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtTieuDeThongBao.DefaultText = "";
            this.txtTieuDeThongBao.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtTieuDeThongBao.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtTieuDeThongBao.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtTieuDeThongBao.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtTieuDeThongBao.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtTieuDeThongBao.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTieuDeThongBao.ForeColor = System.Drawing.Color.Black;
            this.txtTieuDeThongBao.HoverState.BorderColor = System.Drawing.Color.Silver;
            this.txtTieuDeThongBao.Location = new System.Drawing.Point(150, 33);
            this.txtTieuDeThongBao.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtTieuDeThongBao.MaxLength = 0;
            this.txtTieuDeThongBao.Name = "txtTieuDeThongBao";
            this.txtTieuDeThongBao.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.txtTieuDeThongBao.PlaceholderText = "Tiêu đề thông báo...";
            this.txtTieuDeThongBao.SelectedText = "";
            this.txtTieuDeThongBao.Size = new System.Drawing.Size(502, 38);
            this.txtTieuDeThongBao.TabIndex = 81;
            // 
            // cbLoaiThongBao
            // 
            this.cbLoaiThongBao.BackColor = System.Drawing.Color.Transparent;
            this.cbLoaiThongBao.BorderColor = System.Drawing.Color.DarkGreen;
            this.cbLoaiThongBao.BorderRadius = 10;
            this.cbLoaiThongBao.BorderThickness = 2;
            this.cbLoaiThongBao.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbLoaiThongBao.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLoaiThongBao.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cbLoaiThongBao.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cbLoaiThongBao.Font = new System.Drawing.Font("Segoe UI Semibold", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbLoaiThongBao.ForeColor = System.Drawing.Color.Black;
            this.cbLoaiThongBao.ItemHeight = 35;
            this.cbLoaiThongBao.Items.AddRange(new object[] {
            "Họ tên",
            "CCCD",
            "SĐT"});
            this.cbLoaiThongBao.Location = new System.Drawing.Point(150, 113);
            this.cbLoaiThongBao.Name = "cbLoaiThongBao";
            this.cbLoaiThongBao.Size = new System.Drawing.Size(502, 41);
            this.cbLoaiThongBao.TabIndex = 98;
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label40.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label40.Location = new System.Drawing.Point(304, 77);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(176, 31);
            this.label40.TabIndex = 99;
            this.label40.Text = "Loại thông báo";
            this.label40.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ThemThongBao
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(800, 514);
            this.Controls.Add(this.label40);
            this.Controls.Add(this.cbLoaiThongBao);
            this.Controls.Add(this.txtTieuDeThongBao);
            this.Controls.Add(this.btnLuu);
            this.Controls.Add(this.groupBoxChiTiet);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ThemThongBao";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thêm thông báo";
            this.groupBoxChiTiet.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Button btnLuu;
        private Guna.UI2.WinForms.Guna2GroupBox groupBoxChiTiet;
        private System.Windows.Forms.RichTextBox txtNoiDung;
        private Guna.UI2.WinForms.Guna2TextBox txtTieuDeThongBao;
        private Guna.UI2.WinForms.Guna2ComboBox cbLoaiThongBao;
        private System.Windows.Forms.Label label40;
    }
}