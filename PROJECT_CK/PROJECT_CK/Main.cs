using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Guna.UI2.WinForms;
using QuanLyMuaBanXeMay;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using ClosedXML.Excel;

namespace PROJECT_CK
{
    public partial class Main : Form
    {
        private readonly QuanLyNhapXe_Kho quanLyNhapXe_Kho;
        private readonly string _username;
        private readonly string _role;
        private readonly string _connectionString;
        private int currentPhieuNhapID;
        private List<Dictionary<string, string>> tempXeList = new List<Dictionary<string, string>>();
        private DataTable dtXeTonKho;
        private int maPhieuCurrent;
        System.Drawing.Image editIcon = Properties.Resources.chinhsua;
        System.Drawing.Image deleteIcon = Properties.Resources.xoa;
        public System.Globalization.CultureInfo cultureInfoVN = new System.Globalization.CultureInfo("vi-VN");
        private QuanLyNhanVien qlnv = new QuanLyNhanVien();
        private DataTable dtChiTietDonHang;
        public QuanLyKhachHang quanLyKH;
        public int IdKH = 0;
        public Main(string username, string role, string connectionString)
        {
            InitializeComponent();
            _username = username;
            _role = role;
            _connectionString = connectionString;
            quanLyNhapXe_Kho = new QuanLyNhapXe_Kho();
            quanLyKH = new QuanLyKhachHang();
            ApplyRolePermissions();
        }

        private void ApplyRolePermissions()
        {
            if (_role == "RoleAdmin")
            {
            }
            else if (_role == "RoleUser")
            {
                tabControlTrangChu.TabPages.Clear();
                tabControlTrangChu.TabPages.Add(tabPageThongBao);
                tabControlMain.TabPages.Remove(tabPageQLNV);
                tabControlMain.TabPages.Remove(tabPageQLNX);
                tabControlMain.TabPages.Remove(tabPageQLDL);
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            LoadPhieuNhap();
            LoadThongKe();
            LoadNhaCungCap();
            LoadDanhMucComboboxes();
            cbbNguonNhapChiTiet.SelectedIndex = 0;
            cbbNguonNhapNX.SelectedIndex = 0;
            cbbHangXeFilter.SelectedIndex = -1; // Giả sử "Tất cả" là index 0
            cbbLoaiXeFilter.SelectedIndex = -1;
            cbbMauSacFilter.SelectedIndex = -1;
            cbbTinhTrangFilter.SelectedIndex = -1;
            cbbDonGiaOperator.SelectedIndex = -1;
            LoadDanhSachXeNhap();
            LoadBarChart();
            LoadPieChart();
            LoadTongQuanTonKho();
            LoadXeTonKho();
            LoadTextBoxAutoComplete();
            //---
            LoadDataKhachHang();
            LoadDataLichHen();
            //--- Quản lý Nhân viên ---
            LoadNhanVienList();
            LoadChamCongList();
            LoadThongBaoList();
            LoadBangLuong();
            SetupCbbTieuChiNV(); 
            LoadTatCaBaoCao();
            //Bán xe
            radioAll.Checked = true;
            btnSuaUD.Visible = false;
            btnXoaUD.Visible = false;
            KhoiTaoChiTietDonHang();
            LoadActiveUuDaiToComboBox();
            guna2TabControl2.TabPages.Remove(tabPageXuLyDH);
            guna2TabControl2.TabPages.Remove(tabPageChiTietHoaDon);
            

        }

        private void LoadPhieuNhap()
        {
            DataTable dt = quanLyNhapXe_Kho.GetDanhSachPhieuNhap();
            dgvDanhSachPhieuNhap.DataSource = dt;

            dgvDanhSachPhieuNhap.Columns["MaPhieu"].HeaderText = "Mã Phiếu";
            dgvDanhSachPhieuNhap.Columns["NgayNhap"].HeaderText = "Ngày Nhập";
            dgvDanhSachPhieuNhap.Columns["LoaiNhap"].HeaderText = "Loại Nhập";
            dgvDanhSachPhieuNhap.Columns["TongTien"].HeaderText = "Tổng Tiền";

            dgvDanhSachPhieuNhap.Columns["NgayNhap"].DefaultCellStyle.Format = "dd/MM/yyyy";

            if (!dgvDanhSachPhieuNhap.Columns.Contains("LuaChon"))
            {
                DataGridViewButtonColumn thaoTacColumn = new DataGridViewButtonColumn();
                thaoTacColumn.Name = "LuaChon";
                thaoTacColumn.HeaderText = "Thao tác";
                dgvDanhSachPhieuNhap.Columns.Add(thaoTacColumn);
            }
        }

        public void LoadThongKe()
        {
            DataTable dt = quanLyNhapXe_Kho.GetThongKePhieuNhap(null, null, null, null, null);

            if (dt.Rows.Count > 0)
            {
                txtTongSoPhieuNhap.Text = dt.Rows[0]["TongSoPhieu"]?.ToString() ?? "0";
                txtTongSoLuongXeNhap.Text = dt.Rows[0]["TongSoLuongXeNhap"]?.ToString() ?? "0";
                txtTongGiaTriNhap.Text = string.Format("{0:N0} VND", dt.Rows[0]["TongGiaTriNhap"] ?? 0);
            }
            else
            {
                txtTongSoPhieuNhap.Text = "0";
                txtTongSoLuongXeNhap.Text = "0";
                txtTongGiaTriNhap.Text = "0 VND";
            }
        }
        private void LoadNhaCungCap()
        {
            dgvNCC.DataSource = quanLyNhapXe_Kho.GetAllNhaCungCap();
            dgvNCC.Columns["NCCID"].HeaderText = "Nhà cung cấp ID";
            dgvNCC.Columns["TenNCC"].HeaderText = "Tên nhà cung cấp";
            dgvNCC.Columns["DiaChi"].HeaderText = "Địa chỉ";
            dgvNCC.Columns["SoDienThoai"].HeaderText = "Số điện thoại";
        }

        private void LoadDanhMucComboboxes()
        {
            // Hãng xe
            DataTable dtHangXe = quanLyNhapXe_Kho.ExecuteDataTable("sp_GetDanhMucByLoai", CommandType.StoredProcedure,
                new SqlParameter("@LoaiDanhMuc", "HangXe"));
            cboHangXe.DataSource = dtHangXe;
            cboHangXe.DisplayMember = "GiaTri";   // Giá trị hiển thị
            cboHangXe.ValueMember = "DanhMucID";  // Giá trị ẩn ID
            cbbHangXeNX.DataSource = dtHangXe;
            cbbHangXeNX.DisplayMember = "GiaTri";   // Giá trị hiển thị
            cbbHangXeNX.ValueMember = "DanhMucID";
            cbbHangXeFilter.DataSource = dtHangXe;
            cbbHangXeFilter.DisplayMember = "GiaTri";   // Giá trị hiển thị
            cbbHangXeFilter.ValueMember = "DanhMucID";

            // Loại xe
            DataTable dtLoaiXe = quanLyNhapXe_Kho.ExecuteDataTable("sp_GetDanhMucByLoai", CommandType.StoredProcedure,
                new SqlParameter("@LoaiDanhMuc", "LoaiXe"));
            cboLoaiXe.DataSource = dtLoaiXe;
            cboLoaiXe.DisplayMember = "GiaTri";
            cboLoaiXe.ValueMember = "DanhMucID";
            cbbLoaiXeNX.DataSource = dtLoaiXe;
            cbbLoaiXeNX.DisplayMember = "GiaTri";
            cbbLoaiXeNX.ValueMember = "DanhMucID";
            cbbLoaiXeFilter.DataSource = dtLoaiXe;
            cbbLoaiXeFilter.DisplayMember = "GiaTri";
            cbbLoaiXeFilter.ValueMember = "DanhMucID";

            // Màu sắc
            DataTable dtMauSac = quanLyNhapXe_Kho.ExecuteDataTable("sp_GetDanhMucByLoai", CommandType.StoredProcedure,
                new SqlParameter("@LoaiDanhMuc", "MauSac"));
            cbbMauSac.DataSource = dtMauSac;
            cbbMauSac.DisplayMember = "GiaTri";
            cbbMauSac.ValueMember = "DanhMucID";
            cbbMauSacNX.DataSource = dtMauSac;
            cbbMauSacNX.DisplayMember = "GiaTri";
            cbbMauSacNX.ValueMember = "DanhMucID";
            cbbMauSacFilter.DataSource = dtMauSac;
            cbbMauSacFilter.DisplayMember = "GiaTri";
            cbbMauSacFilter.ValueMember = "DanhMucID";

            DataTable dtTinhTrang = quanLyNhapXe_Kho.ExecuteDataTable("sp_GetDanhMucByLoai", CommandType.StoredProcedure,
                new SqlParameter("@LoaiDanhMuc", "TinhTrang"));
            cbbTinhTrangCT.DataSource = dtTinhTrang;
            cbbTinhTrangCT.DisplayMember = "GiaTri";
            cbbTinhTrangCT.ValueMember = "DanhMucID";
            cbbTinhTrangNX.DataSource = dtTinhTrang;
            cbbTinhTrangNX.DisplayMember = "GiaTri";
            cbbTinhTrangNX.ValueMember = "DanhMucID";
            cbbTinhTrangFilter.DataSource = dtTinhTrang;
            cbbTinhTrangFilter.DisplayMember = "GiaTri";
            cbbTinhTrangFilter.ValueMember = "DanhMucID";
        }

        private void LoadDanhSachXeNhap()
        {
            dgvDanhSachChiTietNhapXe.Columns.Clear();
            dgvDanhSachChiTietNhapXe.Columns.Add("HinhAnh", "Hình Ảnh");
            dgvDanhSachChiTietNhapXe.Columns.Add("TenXe", "Tên Xe");
            dgvDanhSachChiTietNhapXe.Columns.Add("HangXe", "Hãng Xe");
            dgvDanhSachChiTietNhapXe.Columns.Add("LoaiXe", "Loại Xe");
            dgvDanhSachChiTietNhapXe.Columns.Add("MauSac", "Màu Sắc");
            dgvDanhSachChiTietNhapXe.Columns.Add("NamSX", "Năm SX");
            dgvDanhSachChiTietNhapXe.Columns.Add("SoKhung", "Số Khung");
            dgvDanhSachChiTietNhapXe.Columns.Add("SoMay", "Số Máy");
            dgvDanhSachChiTietNhapXe.Columns.Add("DungTich", "Dung Tích");
            dgvDanhSachChiTietNhapXe.Columns.Add("TinhTrang", "Tình Trạng");
            dgvDanhSachChiTietNhapXe.Columns.Add("DonGia", "Đơn Giá");
            dgvDanhSachChiTietNhapXe.Columns.Add("MoTa", "Mô Tả");
            // Thiết lập định dạng cho cột Đơn Giá
            if (dgvDanhSachChiTietNhapXe.Columns["DonGia"] != null)
            {
                dgvDanhSachChiTietNhapXe.Columns["DonGia"].DefaultCellStyle.Format = "N0";
            }
            // Thiết lập chiều cao hàng để hiển thị hình ảnh tốt hơn
            dgvDanhSachChiTietNhapXe.RowTemplate.Height = 100;
            // Thiết lập chế độ hiển thị hình ảnh trong cột Hình Ảnh
            DataGridViewImageColumn imgCol = new DataGridViewImageColumn();
            imgCol.Name = "HinhAnh";
            imgCol.HeaderText = "Hình Ảnh";
            imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
            int colIndex = dgvDanhSachChiTietNhapXe.Columns["HinhAnh"].Index;
            dgvDanhSachChiTietNhapXe.Columns.RemoveAt(colIndex);
            dgvDanhSachChiTietNhapXe.Columns.Insert(colIndex, imgCol);
        }

        private void LoadBarChart()
        {
            // Xóa dữ liệu cũ
            chartBar.Series.Clear();
            chartBar.ChartAreas.Clear();

            // Tạo ChartArea
            ChartArea chartArea = new ChartArea("ChartArea1");
            chartBar.ChartAreas.Add(chartArea); // Đảm bảo chỉ gọi add trực tiếp

            // Tạo Series cho cột
            Series series = new Series("SoLuong");
            series.ChartType = SeriesChartType.Column;

            // Lấy tiêu chí từ ComboBox
            string criteria = cbbBarCriteria.SelectedItem?.ToString() ?? "Hãng xe";
            DataTable dt = quanLyNhapXe_Kho.GetDataForBarChart(criteria);

            // Thêm dữ liệu vào Series
            foreach (DataRow row in dt.Rows)
            {
                series.Points.AddXY(row["Category"].ToString(), Convert.ToInt32(row["SoLuong"]));
            }

            // Thêm Series vào Chart
            chartBar.Series.Add(series);

            // Tùy chỉnh giao diện
            chartBar.BackColor = Color.White;
        }

        private void LoadPieChart()
        {
            // Xóa dữ liệu cũ
            chartPie.Series.Clear();
            chartPie.ChartAreas.Clear();

            // Tạo ChartArea
            ChartArea chartArea = new ChartArea("ChartArea1");
            chartPie.ChartAreas.Add(chartArea);

            // Tạo Series cho tròn
            Series series = new Series("TiLe");
            series.ChartType = SeriesChartType.Pie;

            // Lấy tiêu chí từ ComboBox
            string criteria = cbbPieCriteria.SelectedItem?.ToString() ?? "Tình trạng";
            DataTable dt = quanLyNhapXe_Kho.GetDataForPieChart(criteria);

            // Thêm dữ liệu vào Series
            foreach (DataRow row in dt.Rows)
            {
                series.Points.AddXY(row["Category"].ToString(), Convert.ToInt32(row["SoLuong"]));
            }

            // Thêm Series vào Chart
            chartPie.Series.Add(series);

            // Định dạng
            series["PieLabelStyle"] = "Outside";
            series["PieLineColor"] = "Black";
            series.Color = Color.FromArgb(255, 99, 132);

            // Tùy chỉnh giao diện
            chartPie.BackColor = Color.White;
        }
        private void LoadTongQuanTonKho()
        {
            try
            {
                int tongSoXeTonKho = quanLyNhapXe_Kho.GetTongSoXeTonKho();
                decimal tongGiaTriTonKho = quanLyNhapXe_Kho.GetTongGiaTriTonKho();
                decimal tongLoiNhuan = quanLyNhapXe_Kho.GetTongLoiNhuanXe();
                decimal tongGiaTriNhap = quanLyNhapXe_Kho.GetTongGiaTriNhap();
                int tongSoXeNhap = quanLyNhapXe_Kho.GetTongSoXeNhap();

                txtTongSoXeTonKho.Text = tongSoXeTonKho.ToString();
                txtTongGiaTriTonKho.Text = string.Format("{0:N0} VNĐ", tongGiaTriTonKho);
                txtTongLoiNhuan.Text = string.Format("{0:N0} VNĐ", tongLoiNhuan);
                txtTong.Text = string.Format("{0:N0} VNĐ", tongGiaTriNhap);
                txtTongXeNhap.Text = tongSoXeNhap.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thông tin tồn kho: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadXeTonKho()
        {
            try
            {
                dtXeTonKho = quanLyNhapXe_Kho.GetAllXeTonKho();

                // Nếu chưa có cột ảnh thì thêm mới
                if (!dtXeTonKho.Columns.Contains("HinhAnhImg"))
                    dtXeTonKho.Columns.Add("HinhAnhImg", typeof(System.Drawing.Image));

                string projectDir = Directory.GetParent(Application.StartupPath).Parent.FullName;

                foreach (DataRow row in dtXeTonKho.Rows)
                {
                    System.Drawing.Image img = null;
                    try
                    {
                        string relativePath = row["HinhAnh"]?.ToString();
                        if (!string.IsNullOrEmpty(relativePath))
                        {
                            string fullPath = Path.Combine(projectDir, relativePath);
                            if (File.Exists(fullPath))
                            {
                                // Dùng stream để tránh lock file
                                using (var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                                {
                                    img = System.Drawing.Image.FromStream(fs);
                                }
                            }
                        }
                    }
                    catch
                    {
                        img = null; // Nếu có lỗi, giữ nguyên null
                    }

                    row["HinhAnhImg"] = img;
                }

                dgvTonKhoChiTiet.DataSource = dtXeTonKho;
                FormatTonKhoGrid();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải xe tồn kho: {ex.Message}",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatTonKhoGrid()
        {
            dgvTonKhoChiTiet.RowTemplate.Height = 100;

            // Ẩn cột không cần
            if (dgvTonKhoChiTiet.Columns["XeCTID"] != null)
                dgvTonKhoChiTiet.Columns["XeCTID"].Visible = false;
            if (dgvTonKhoChiTiet.Columns["HinhAnh"] != null)
                dgvTonKhoChiTiet.Columns["HinhAnh"].Visible = false;

            // Cột ảnh
            if (dgvTonKhoChiTiet.Columns["HinhAnhImg"] is DataGridViewImageColumn imgCol)
            {
                imgCol.HeaderText = "Hình ảnh";
                imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
                imgCol.DisplayIndex = 0;
            }

            // Header text
            dgvTonKhoChiTiet.Columns["TenXe"].HeaderText = "Tên xe";
            dgvTonKhoChiTiet.Columns["HangXe"].HeaderText = "Hãng xe";
            dgvTonKhoChiTiet.Columns["LoaiXe"].HeaderText = "Loại xe";
            dgvTonKhoChiTiet.Columns["MauSac"].HeaderText = "Màu sắc";
            dgvTonKhoChiTiet.Columns["NamSX"].HeaderText = "Năm SX";
            dgvTonKhoChiTiet.Columns["DungTich"].HeaderText = "Dung tích";
            dgvTonKhoChiTiet.Columns["SoKhung"].HeaderText = "Số khung";
            dgvTonKhoChiTiet.Columns["SoMay"].HeaderText = "Số máy";
            dgvTonKhoChiTiet.Columns["TinhTrang"].HeaderText = "Tình trạng";
            dgvTonKhoChiTiet.Columns["DonGia"].HeaderText = "Đơn giá";
            dgvTonKhoChiTiet.Columns["DonGia"].DefaultCellStyle.Format = "N0";
        }

        private void LoadTextBoxAutoComplete()
        {
            LoadAutoCompleteForTextBox(txtTenNCC_KH, cbbNguonNhapChiTiet.Text, isTen: true);
            LoadAutoCompleteForTextBox(txtSDTNCC_KH, cbbNguonNhapChiTiet.Text, isTen: false);
            LoadAutoCompleteForTextBox(txtTenNCC_KHNX, cbbNguonNhapNX.Text, isTen: true);
            LoadAutoCompleteForTextBox(txtSDTNCC_KHNX, cbbNguonNhapNX.Text, isTen: false);
        }

        private void LoadAutoCompleteForTextBox(Guna2TextBox textBox, string loaiNhap, bool isTen)
        {
            List<string> items = null;
            try
            {
                if (string.IsNullOrEmpty(loaiNhap) || (!loaiNhap.Equals("NCC", StringComparison.OrdinalIgnoreCase) && !loaiNhap.Equals("KhachHang", StringComparison.OrdinalIgnoreCase)))
                {
                    // Nếu không hợp lệ, không làm gì hoặc đặt giá trị mặc định
                    textBox.AutoCompleteMode = AutoCompleteMode.None;
                    textBox.AutoCompleteSource = AutoCompleteSource.None;
                    textBox.AutoCompleteCustomSource = null;
                    return;
                }
                if (loaiNhap == "NCC")
                {
                    items = isTen ? quanLyNhapXe_Kho.GetTenNhaCungCap() : quanLyNhapXe_Kho.GetSoDTNhaCungCap();
                }
                else if (loaiNhap == "KhachHang")
                {
                    items = isTen ? quanLyNhapXe_Kho.GetTenKhachHang() : quanLyNhapXe_Kho.GetSoDTKhachHang();
                }
                AutoCompleteStringCollection autoComplete = new AutoCompleteStringCollection();
                autoComplete.AddRange(items.ToArray());

                textBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                textBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
                textBox.AutoCompleteCustomSource = autoComplete;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thiết lập AutoComplete: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadThongTinChungPhieuNhap(int phieuNhapID)
        {

            DataTable dtPhieuNhap = quanLyNhapXe_Kho.GetThongTinChungPhieuNhap(phieuNhapID);
            if (dtPhieuNhap.Rows.Count > 0)
            {
                DataRow row = dtPhieuNhap.Rows[0];

                txtMaPhieuNhapChiTiet.Text = row["PhieuNhapID"].ToString();
                dtNgayNhapChiTiet.Value = Convert.ToDateTime(row["NgayNhap"]);
                cbbNguonNhapChiTiet.Text = row["LoaiNhap"].ToString();

                // Xử lý thông tin NCC hoặc Khách hàng
                if (row["LoaiNhap"].ToString() == "NCC")
                {
                    txtTenNCC_KH.Text = row["TenNCC"]?.ToString() ?? "";
                    txtSDTNCC_KH.Text = row["SoDienThoaiNCC"]?.ToString() ?? "";
                    txtDiaChiNCC_KH.Text = row["DiaChiNCC"]?.ToString() ?? "";
                }
                else if (row["LoaiNhap"].ToString() == "KhachHang")
                {
                    txtTenNCC_KH.Text = row["TenKhachHang"]?.ToString() ?? "";
                    txtSDTNCC_KH.Text = row["SoDienThoaiKH"]?.ToString() ?? "";
                    txtDiaChiNCC_KH.Text = row["DiaChiKH"]?.ToString() ?? "";
                }
            }
        }

        private void LoadChiTietXeNhap(int phieuNhapID)
        {
            DataTable dtChiTiet = quanLyNhapXe_Kho.GetDanhSachXeNhap(phieuNhapID);

            // Check if DataTable is null or empty
            if (dtChiTiet == null || dtChiTiet.Columns.Count == 0)
            {
                MessageBox.Show("No data returned for PhieuNhapID: " + phieuNhapID, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dgvDanhSachChiTietXeNhap.DataSource = null;
                return;
            }

            dgvDanhSachChiTietXeNhap.DataSource = dtChiTiet;

            // Hide columns if they exist
            if (dgvDanhSachChiTietXeNhap.Columns.Contains("HinhAnh"))
                dgvDanhSachChiTietXeNhap.Columns["HinhAnh"].Visible = false;
            if (dgvDanhSachChiTietXeNhap.Columns.Contains("MoTa"))
                dgvDanhSachChiTietXeNhap.Columns["MoTa"].Visible = false;
            if (dgvDanhSachChiTietXeNhap.Columns.Contains("PhieuNhapID"))
                dgvDanhSachChiTietXeNhap.Columns["PhieuNhapID"].Visible = false;
            if (dgvDanhSachChiTietXeNhap.Columns.Contains("XeCTID"))
                dgvDanhSachChiTietXeNhap.Columns["XeCTID"].Visible = false;

            // Set header text for columns if they exist
            if (dgvDanhSachChiTietXeNhap.Columns.Contains("TenXe"))
                dgvDanhSachChiTietXeNhap.Columns["TenXe"].HeaderText = "Tên Xe";
            if (dgvDanhSachChiTietXeNhap.Columns.Contains("HangXe"))
                dgvDanhSachChiTietXeNhap.Columns["HangXe"].HeaderText = "Hãng Xe";
            if (dgvDanhSachChiTietXeNhap.Columns.Contains("LoaiXe"))
                dgvDanhSachChiTietXeNhap.Columns["LoaiXe"].HeaderText = "Loại Xe";
            if (dgvDanhSachChiTietXeNhap.Columns.Contains("MauSac"))
                dgvDanhSachChiTietXeNhap.Columns["MauSac"].HeaderText = "Màu Sắc";
            if (dgvDanhSachChiTietXeNhap.Columns.Contains("NamSX"))
                dgvDanhSachChiTietXeNhap.Columns["NamSX"].HeaderText = "Năm Sản Xuất";
            if (dgvDanhSachChiTietXeNhap.Columns.Contains("DungTich"))
                dgvDanhSachChiTietXeNhap.Columns["DungTich"].HeaderText = "Dung Tích";
            if (dgvDanhSachChiTietXeNhap.Columns.Contains("SoKhung"))
                dgvDanhSachChiTietXeNhap.Columns["SoKhung"].HeaderText = "Số Khung";
            if (dgvDanhSachChiTietXeNhap.Columns.Contains("SoMay"))
                dgvDanhSachChiTietXeNhap.Columns["SoMay"].HeaderText = "Số Máy";
            if (dgvDanhSachChiTietXeNhap.Columns.Contains("TinhTrang"))
                dgvDanhSachChiTietXeNhap.Columns["TinhTrang"].HeaderText = "Tình Trạng";
            if (dgvDanhSachChiTietXeNhap.Columns.Contains("DonGia"))
            {
                dgvDanhSachChiTietXeNhap.Columns["DonGia"].HeaderText = "Đơn Giá";
                dgvDanhSachChiTietXeNhap.Columns["DonGia"].DefaultCellStyle.Format = "N0";
            }
            if (!dgvDanhSachChiTietXeNhap.Columns.Contains("LuaChon"))
            {
                DataGridViewButtonColumn thaoTacColumn = new DataGridViewButtonColumn();
                thaoTacColumn.Name = "LuaChon";
                thaoTacColumn.HeaderText = "Thao tác";
                dgvDanhSachChiTietXeNhap.Columns.Add(thaoTacColumn);
            }
        }

        private void UpdateThongKe()
        {
            DateTime? tuNgay = dtTuNgay.Value;
            DateTime? denNgay = dtDenNgay.Value;
            string loaiNhap = cbbNguonNhap.SelectedItem?.ToString() ?? "All";
            int? nccID = string.IsNullOrWhiteSpace(txtMaNCC.Text) ? (int?)null : Convert.ToInt32(txtMaNCC.Text);
            int? khID = string.IsNullOrWhiteSpace(txtMaKH.Text) ? (int?)null : Convert.ToInt32(txtMaKH.Text);

            DataTable dt = quanLyNhapXe_Kho.GetThongKePhieuNhap(tuNgay, denNgay, loaiNhap, nccID, khID);

            if (dt.Rows.Count > 0)
            {
                txtTongSoPhieuNhap.Text = dt.Rows[0]["TongSoPhieu"].ToString();
                txtTongSoLuongXeNhap.Text = dt.Rows[0]["TongSoLuongXeNhap"].ToString();
                txtTongGiaTriNhap.Text = string.Format("{0:N0} VND", dt.Rows[0]["TongGiaTriNhap"]);
            }
        }

        private void btnpTimKiem_Click(object sender, EventArgs e)
        {
            DateTime? tuNgay = dtTuNgay.Value;
            DateTime? denNgay = dtDenNgay.Value;

            string loaiNhap = cbbNguonNhap.Text;
            int? nccID = string.IsNullOrWhiteSpace(txtMaNCC.Text) ? (int?)null : int.Parse(txtMaNCC.Text);
            int? khID = string.IsNullOrWhiteSpace(txtMaKH.Text) ? (int?)null : int.Parse(txtMaKH.Text);

            DataTable dt = quanLyNhapXe_Kho.TimKiemPhieuNhap(tuNgay, denNgay, loaiNhap, nccID, khID);

            dgvDanhSachPhieuNhap.AutoGenerateColumns = false;
            dgvDanhSachPhieuNhap.DataSource = dt;
            UpdateThongKe();
        }

        private void dgvDanhSachPhieuNhap_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvDanhSachPhieuNhap.Columns["LuaChon"].Index)
            {
                e.PaintBackground(e.CellBounds, true);

                int padding = 20;
                int iconSize = 50;

                int totalWidth = (iconSize * 2) + padding;
                int startX = e.CellBounds.X + (e.CellBounds.Width - totalWidth) / 2;
                int startY = e.CellBounds.Y + (e.CellBounds.Height - iconSize) / 2;

                Rectangle Sua = new Rectangle(startX, startY, iconSize, iconSize);
                e.Graphics.DrawImage(editIcon, Sua);

                Rectangle Xoa = new Rectangle(startX + iconSize + padding, startY, iconSize, iconSize);
                e.Graphics.DrawImage(deleteIcon, Xoa);

                e.Handled = true;
            }
        }

        private void dgvDanhSachPhieuNhap_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvDanhSachPhieuNhap.Rows[e.RowIndex].IsNewRow)
                return;

            object cellValue = dgvDanhSachPhieuNhap.Rows[e.RowIndex].Cells["MaPhieu"].Value;

            if (cellValue == null || cellValue == DBNull.Value)
                return;

            int phieuNhapID = Convert.ToInt32(cellValue);
            currentPhieuNhapID = phieuNhapID;

            if (e.ColumnIndex == dgvDanhSachPhieuNhap.Columns["LuaChon"].Index)
            {
                int padding = 20;
                int iconSize = 50;
                int totalWidth = (iconSize * 2) + padding;

                Rectangle cellBounds = dgvDanhSachPhieuNhap.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);

                int startX = cellBounds.X + (cellBounds.Width - totalWidth) / 2;
                int startY = cellBounds.Y + (cellBounds.Height - iconSize) / 2;

                Rectangle suaRect = new Rectangle(startX, startY, iconSize, iconSize);
                Rectangle xoaRect = new Rectangle(startX + iconSize + padding, startY, iconSize, iconSize);

                Point clickPoint = System.Windows.Forms.Cursor.Position;
                clickPoint = dgvDanhSachPhieuNhap.PointToClient(clickPoint);

                if (suaRect.Contains(clickPoint))
                {
                    LoadThongTinChungPhieuNhap(phieuNhapID);
                    LoadChiTietXeNhap(phieuNhapID);
                    tabConTrolQLNX.SelectedTab = tabPageCTPN;
                }
                else if (xoaRect.Contains(clickPoint))
                {
                    DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa phiếu nhập này không?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            if (quanLyNhapXe_Kho.DeletePhieuNhap(phieuNhapID))
                            {
                                MessageBox.Show("Xóa phiếu nhập thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadPhieuNhap();
                                LoadThongKe();
                                LoadBarChart();
                                LoadPieChart();
                                LoadTongQuanTonKho();
                                LoadXeTonKho();
                            }
                            else
                            {
                                MessageBox.Show("Không thể xóa phiếu nhập. Vui lòng thử lại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (SqlException ex)
                        {
                            MessageBox.Show($"Lỗi SQL khi xóa phiếu nhập: {ex.Message}\nSố lỗi: {ex.Number}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Lỗi khi xóa phiếu nhập: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void dgvDanhSachChiTietXeNhap_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvDanhSachChiTietXeNhap.Columns["LuaChon"].Index)
            {
                e.PaintBackground(e.CellBounds, true);

                int iconSize = 30;

                int startX = e.CellBounds.X + (e.CellBounds.Width - iconSize) / 2;
                int startY = e.CellBounds.Y + (e.CellBounds.Height - iconSize) / 2;

                Rectangle rect = new Rectangle(startX, startY, iconSize, iconSize);
                e.Graphics.DrawImage(deleteIcon, rect);

                e.Handled = true;
            }
        }

        private void dgvDanhSachChiTietXeNhap_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Nếu click vào header hoặc ngoài phạm vi -> bỏ qua
                if (e.RowIndex < 0 || e.RowIndex >= dgvDanhSachChiTietXeNhap.Rows.Count)
                    return;

                // Nếu click vào cột "LuaChon" -> xử lý nút xóa
                if (e.ColumnIndex == dgvDanhSachChiTietXeNhap.Columns["LuaChon"].Index)
                {
                    int iconSize = 30;

                    Rectangle cellBounds = dgvDanhSachChiTietXeNhap.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                    int startX = cellBounds.X + (cellBounds.Width - iconSize) / 2;
                    int startY = cellBounds.Y + (cellBounds.Height - iconSize) / 2;

                    Rectangle xoaRect = new Rectangle(startX, startY, iconSize, iconSize);
                    Point clickPoint = System.Windows.Forms.Cursor.Position; // Sửa lại thành System.Windows.Forms.Cursor.Position
                    clickPoint = dgvDanhSachChiTietXeNhap.PointToClient(clickPoint); // Chuyển đổi tọa độ

                    if (xoaRect.Contains(clickPoint))
                    {
                        int xeCTID = Convert.ToInt32(dgvDanhSachChiTietXeNhap.Rows[e.RowIndex].Cells["XeCTID"].Value);

                        DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa xe này không?", "Xác nhận xóa",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            if (quanLyNhapXe_Kho.DeleteXeFromPhieuNhap(currentPhieuNhapID, xeCTID))
                            {
                                MessageBox.Show("Xóa xe thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadChiTietXeNhap(currentPhieuNhapID); // load lại dữ liệu
                                LoadBarChart();
                                LoadPieChart();
                                LoadTongQuanTonKho();
                                LoadXeTonKho();
                            }
                            else
                            {
                                MessageBox.Show("Không thể xóa xe. Vui lòng thử lại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        return; // tránh chạy xuống dưới khi vừa xóa
                    }
                }

                // Lấy row hiện tại
                DataGridViewRow row = dgvDanhSachChiTietXeNhap.Rows[e.RowIndex];

                // Đưa dữ liệu lên control
                cboHangXe.Text = row.Cells["HangXe"].Value?.ToString();
                cboLoaiXe.Text = row.Cells["LoaiXe"].Value?.ToString();
                cbbMauSac.Text = row.Cells["MauSac"].Value?.ToString();
                txtNSX.Text = row.Cells["NamSX"].Value?.ToString();
                txtTenXe.Text = row.Cells["TenXe"].Value?.ToString();
                txtMoTa.Text = row.Cells["MoTa"].Value?.ToString();
                txtSoKhungCT.Text = row.Cells["SoKhung"].Value?.ToString();
                txtSoMayCT.Text = row.Cells["SoMay"].Value?.ToString();
                cbbTinhTrangCT.Text = row.Cells["TinhTrang"].Value?.ToString();
                txtDonGia.Text = row.Cells["DonGia"].Value?.ToString();
                txtDungTichCT.Text = row.Cells["DungTich"].Value?.ToString();

                // Load hình ảnh
                string relativePath = row.Cells["HinhAnh"].Value?.ToString();
                if (!string.IsNullOrEmpty(relativePath))
                {
                    string projectDir = Directory.GetParent(Application.StartupPath).Parent.FullName;
                    string fullPath = Path.Combine(projectDir, relativePath);

                    if (File.Exists(fullPath))
                    {
                        // Giải phóng ảnh cũ
                        if (pbHinhAnh.Image != null)
                        {
                            pbHinhAnh.Image.Dispose();
                            pbHinhAnh.Image = null;
                        }

                        using (var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                        {
                            pbHinhAnh.Image = System.Drawing.Image.FromStream(fs);
                        }
                        pbHinhAnh.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                    else
                    {
                        pbHinhAnh.Image = Properties.Resources.placeholder;
                    }

                    txtHinhAnhSua.Text = relativePath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xử lý dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnpSuaPhieuNhap_Click(object sender, EventArgs e)
        {
            try
            {
                int phieuNhapID = int.Parse(txtMaPhieuNhapChiTiet.Text);
                DateTime ngayNhap = dtNgayNhapChiTiet.Value;
                string loaiNhap = cbbNguonNhapChiTiet.Text.Trim();

                // Lấy thông tin NCC hoặc Khách hàng từ các textbox
                string ten = txtTenNCC_KH.Text;
                string soDT = txtSDTNCC_KH.Text;
                string diaChi = txtDiaChiNCC_KH.Text;

                bool updated = quanLyNhapXe_Kho.UpdatePhieuNhap(phieuNhapID, ngayNhap, loaiNhap, ten, soDT, diaChi);

                if (updated)
                {
                    MessageBox.Show("Cập nhật phiếu nhập thành công!", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadPhieuNhap();
                    LoadBarChart();
                    LoadPieChart();
                    LoadTongQuanTonKho();
                    LoadXeTonKho();
                }
                else
                {
                    MessageBox.Show("Không có dữ liệu nào được cập nhật.", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật phiếu nhập: " + ex.Message, "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnpSuaThongTinXe_Click(object sender, EventArgs e)
        {
            try
            {
                dtXeTonKho = quanLyNhapXe_Kho.GetAllXeTonKho();

                // Nếu chưa có cột ảnh thì thêm mới
                if (!dtXeTonKho.Columns.Contains("HinhAnhImg"))
                    dtXeTonKho.Columns.Add("HinhAnhImg", typeof(System.Drawing.Image));

                string projectDir = Directory.GetParent(Application.StartupPath).Parent.FullName;

                foreach (DataRow row in dtXeTonKho.Rows)
                {
                    System.Drawing.Image img = null;
                    try
                    {
                        string relativePath = row["HinhAnh"]?.ToString();
                        if (!string.IsNullOrEmpty(relativePath))
                        {
                            string fullPath = Path.Combine(projectDir, relativePath);
                            if (File.Exists(fullPath))
                            {
                                // Dùng stream để tránh lock file
                                using (var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                                {
                                    img = System.Drawing.Image.FromStream(fs);
                                }
                            }
                        }
                    }
                    catch
                    {
                        img = Properties.Resources.placeholder; // Ảnh mặc định nếu lỗi
                    }

                    row["HinhAnhImg"] = img;
                }

                dgvTonKhoChiTiet.DataSource = dtXeTonKho;
                FormatTonKhoGrid();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải xe tồn kho: {ex.Message}",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnpChonHinhCT_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Chọn hình ảnh";
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string sourcePath = ofd.FileName;
                    string fileName = Path.GetFileName(sourcePath);

                    // Thư mục Images trong project
                    string projectDir = Directory.GetParent(Application.StartupPath).Parent.FullName;
                    string destDir = Path.Combine(projectDir, "Images");
                    if (!Directory.Exists(destDir))
                    {
                        Directory.CreateDirectory(destDir);
                    }

                    string destPath = Path.Combine(destDir, fileName);

                    // Giải phóng ảnh cũ trong PictureBox (nếu có) để tránh lock
                    if (pbHinhAnh.Image != null)
                    {
                        pbHinhAnh.Image.Dispose();
                        pbHinhAnh.Image = null;
                    }

                    // Nếu file nguồn KHÁC file đích thì mới copy
                    if (!string.Equals(sourcePath, destPath, StringComparison.OrdinalIgnoreCase))
                    {
                        File.Copy(sourcePath, destPath, true);
                    }

                    // Load ảnh vào PictureBox mà không lock file
                    using (var fs = new FileStream(destPath, FileMode.Open, FileAccess.Read))
                    {
                        pbHinhAnh.Image = System.Drawing.Image.FromStream(fs);
                    }
                    pbHinhAnh.SizeMode = PictureBoxSizeMode.Zoom;

                    // Lưu relative path vào TextBox để ghi DB
                    string relativePath = $"Images/{fileName}";
                    txtHinhAnhSua.Text = relativePath;
                }
            }
        }

        private void btnpChonHinhNX_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Chọn hình ảnh";
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string sourcePath = ofd.FileName;
                    string fileName = Path.GetFileName(sourcePath);

                    // Thư mục Images trong project
                    string projectDir = Directory.GetParent(Application.StartupPath).Parent.FullName;
                    string destDir = Path.Combine(projectDir, "Images");
                    if (!Directory.Exists(destDir))
                    {
                        Directory.CreateDirectory(destDir);
                    }

                    string destPath = Path.Combine(destDir, fileName);

                    // Nếu file nguồn KHÁC file đích thì mới copy
                    if (!string.Equals(sourcePath, destPath, StringComparison.OrdinalIgnoreCase))
                    {
                        File.Copy(sourcePath, destPath, true);
                    }

                    // Lưu relative path vào TextBox để ghi DB
                    string relativePath = $"Images/{fileName}";
                    txtHinhAnhNX.Text = relativePath;
                }
            }
        }

        private void TinhTongGiaTri()
        {
            try
            {
                decimal tongGiaTri = 0;
                foreach (var xe in tempXeList)
                {
                    if (decimal.TryParse(xe["DonGia"], out decimal donGia))
                    {
                        tongGiaTri += donGia;
                    }
                }
                // Định dạng số tiền (ví dụ: 1.234.567 VNĐ)
                txtTongGiaTriNX.Text = tongGiaTri.ToString("#,##0") + " VNĐ";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tính tổng giá trị: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtTongGiaTriNX.Text = "0 VNĐ";
            }
        }

        private void btnpThemXeNX_Click(object sender, EventArgs e)
        {
            string soKhung = txtSoKhungNX.Text.Trim();
            string soMay = txtSoMayNX.Text.Trim();

            if (string.IsNullOrWhiteSpace(cbbTinhTrangNX.Text))
            {
                MessageBox.Show("Vui lòng chọn tình trạng xe!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbbHangXeNX.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn hãng xe!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbbLoaiXeNX.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn loại xe!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbbMauSacNX.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn màu sắc!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTenXeNX.Text))
            {
                MessageBox.Show("Vui lòng nhập tên xe!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtNamSanXuatNX.Text))
            {
                MessageBox.Show("Vui lòng nhập năm sản xuất!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra input cơ bản (bạn có thể thêm nhiều hơn)
            if (string.IsNullOrWhiteSpace(soKhung) || string.IsNullOrWhiteSpace(soMay))
            {
                MessageBox.Show("Số khung và số máy không được để trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Bước 1: Kiểm tra trùng lặp trong bảng tạm (tempXeList)
            bool trungTrongTam = tempXeList.Any(xe =>
                xe["SoKhung"].Equals(soKhung, StringComparison.OrdinalIgnoreCase) ||
                xe["SoMay"].Equals(soMay, StringComparison.OrdinalIgnoreCase));

            if (trungTrongTam)
            {
                MessageBox.Show("Số khung hoặc số máy đã tồn tại trong danh sách xe chờ xác nhận!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Bước 2: Kiểm tra trùng lặp với database
            int ketQuaCheckDB = quanLyNhapXe_Kho.CheckXeExists(soKhung, soMay);
            if (ketQuaCheckDB == 1)
            {
                MessageBox.Show("Số khung đã tồn tại trong hệ thống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (ketQuaCheckDB == 2)
            {
                MessageBox.Show("Số máy đã tồn tại trong hệ thống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                if (!int.TryParse(txtNamSanXuatNX.Text, out int namSX) || namSX < 1900 || namSX > DateTime.Now.Year + 1)
                {
                    MessageBox.Show("Năm sản xuất không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!decimal.TryParse(txtDonGiaNX.Text, out decimal donGia) || donGia <= 0)
                {
                    MessageBox.Show("Đơn giá không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var xe = new Dictionary<string, string>
        {
            { "SoKhung", txtSoKhungNX.Text.Trim() },
            { "SoMay", txtSoMayNX.Text.Trim() },
            { "TenXe", txtTenXeNX.Text.Trim() },
            { "HangXe", cbbHangXeNX.Text.Trim() },
            { "LoaiXe", cbbLoaiXeNX.Text.Trim() },
            { "MauSac", cbbMauSacNX.Text.Trim() },
            { "NamSX", txtNamSanXuatNX.Text.Trim() },
            { "DonGia", txtDonGiaNX.Text.Trim() },
            { "HinhAnh", txtHinhAnhNX.Text.Trim() },
            { "MoTa", txtMoTaNX.Text.Trim() },
            { "TinhTrang", cbbTinhTrangNX.Text.Trim()  },
            { "DungTich", txtDungTichNX.Text.Trim()  }

        };

                tempXeList.Add(xe);

                System.Drawing.Image img = null;
                try
                {
                    string projectDir = Directory.GetParent(Application.StartupPath).Parent.FullName;
                    string fullPath = Path.Combine(projectDir, xe["HinhAnh"]);
                    if (File.Exists(fullPath))
                    {
                        img = System.Drawing.Image.FromFile(fullPath);
                    }
                }
                catch
                {
                    // Nếu không load được ảnh thì để null hoặc icon mặc định
                    img = null;
                }

                dgvDanhSachChiTietNhapXe.Rows.Add(
                    img,
                    xe["TenXe"],
                    xe["HangXe"],
                    xe["LoaiXe"],
                    xe["MauSac"],
                    xe["NamSX"],
                    xe["SoKhung"],
                    xe["SoMay"],
                    xe["TinhTrang"],
                    xe["DonGia"],
                    xe["DungTich"],
                    xe["MoTa"]);

                TinhTongGiaTri();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm xe: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnpXoaXeNX_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có row nào được chọn không
            if (dgvDanhSachChiTietNhapXe.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một xe để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Hiển thị thông báo xác nhận xóa
            DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa xe này không?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Lấy chỉ số của row được chọn
                    int rowIndex = dgvDanhSachChiTietNhapXe.CurrentRow.Index;

                    // Xóa phần tử tương ứng trong tempXeList
                    if (rowIndex < tempXeList.Count)
                    {
                        tempXeList.RemoveAt(rowIndex);
                    }

                    // Xóa row khỏi DataGridView
                    dgvDanhSachChiTietNhapXe.Rows.RemoveAt(rowIndex);

                    MessageBox.Show("Xóa xe thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    TinhTongGiaTri();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa xe: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void resetFormNhapXe()
        {
            tempXeList.Clear();
            txtTongGiaTriNX.Text = "0 VNĐ";
            txtSoKhungNX.Clear();
            txtSoMayNX.Clear();
            txtTenXeNX.Clear();
            txtNamSanXuatNX.Clear();
            txtDonGiaNX.Clear();
            txtHinhAnhNX.Clear();
            txtMoTaNX.Clear();
            cbbTinhTrangNX.SelectedIndex = -1;
            txtDungTichNX.Clear();
            dgvDanhSachChiTietNhapXe.Rows.Clear();
            cbbNguonNhapNX.SelectedIndex = 0;
            txtTenNCC_KHNX.Clear();
            txtSDTNCC_KHNX.Clear();
            txtDiaChiNX.Clear();
            cbbHangXeNX.SelectedIndex = -1;
            cbbLoaiXeNX.SelectedIndex = -1;
            cbbMauSacNX.SelectedIndex = -1;
        }

        private void btnXacNhanNX_Click(object sender, EventArgs e)
        {
            try
            {
                if (tempXeList.Count == 0)
                {
                    MessageBox.Show("Vui lòng thêm ít nhất một xe!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DateTime ngayNhap = dtNgayNhapNX.Value;
                string loaiNhap = cbbNguonNhapNX.Text.Trim();
                string ten = txtTenNCC_KHNX.Text.Trim();
                string soDT = txtSDTNCC_KHNX.Text.Trim();
                string diaChi = txtDiaChiNX.Text.Trim();

                // Kiểm tra thông tin NCC hoặc Khách hàng
                if (loaiNhap == "NCC")
                {
                    if (string.IsNullOrWhiteSpace(ten) || string.IsNullOrWhiteSpace(soDT))
                    {
                        MessageBox.Show("Vui lòng nhập đầy đủ thông tin nhà cung cấp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else if (loaiNhap == "KhachHang")
                {
                    if (string.IsNullOrWhiteSpace(ten) || string.IsNullOrWhiteSpace(soDT))
                    {
                        MessageBox.Show("Vui lòng nhập đầy đủ thông tin khách hàng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Loại nhập không hợp lệ! Vui lòng chọn NCC hoặc KhachHang.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int phieuNhapID = quanLyNhapXe_Kho.InsertPhieuNhapWithXe(ngayNhap, loaiNhap, ten, soDT, diaChi, tempXeList);
                maPhieuCurrent = phieuNhapID;

                if (phieuNhapID > 0)
                {
                    MessageBox.Show($"Tạo phiếu nhập thành công! Mã phiếu: {phieuNhapID}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadPhieuNhap();
                    LoadBarChart();
                    LoadPieChart();
                    LoadTongQuanTonKho();
                    LoadXeTonKho();
                    LoadThongKe();
                    resetFormNhapXe();
                    btnXuatPhieuNX.Enabled = true;


                }
                else
                {
                    MessageBox.Show("Không thể tạo phiếu nhập.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xác nhận phiếu nhập: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXuatPhieuNX_Click(object sender, EventArgs e)
        {
            GeneratePhieuNhapExcel(maPhieuCurrent);
            btnXuatPhieuNX.Enabled = false;
        }

        private void GeneratePhieuNhapExcel(int maPhieuNhap)
        {
            try
            {
                // Lấy thông tin phiếu
                DataTable dtPhieuNhap = quanLyNhapXe_Kho.GetThongTinChungPhieuNhap(maPhieuNhap);

                if (dtPhieuNhap.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy phiếu nhập.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataRow rowPhieu = dtPhieuNhap.Rows[0];
                string ngayNhap = rowPhieu["NgayNhap"].ToString();
                string loaiNhap = rowPhieu["LoaiNhap"].ToString();
                string nhaCungCap = rowPhieu["TenNCC"]?.ToString() ?? "N/A";
                string soDienThoaiNCC = rowPhieu["SoDienThoaiNCC"]?.ToString() ?? "N/A";
                string diaChiNCC = rowPhieu["DiaChiNCC"]?.ToString() ?? "N/A";
                string tenKhachHang = rowPhieu["TenKhachHang"]?.ToString() ?? "N/A";
                string soDienThoaiKH = rowPhieu["SoDienThoaiKH"]?.ToString() ?? "N/A";
                string diaChiKH = rowPhieu["DiaChiKH"]?.ToString() ?? "N/A";

                DataTable dtXeNhap = quanLyNhapXe_Kho.GetDanhSachXeNhap(maPhieuNhap);

                decimal tongGiaTri = dtXeNhap.AsEnumerable()
                    .Where(r => r["DonGia"] != DBNull.Value)
                    .Sum(r => Convert.ToDecimal(r["DonGia"]));
                int tongSoLuong = dtXeNhap.Rows.Count;

                using (var workbook = new XLWorkbook())
                {
                    var ws = workbook.Worksheets.Add("Phiếu nhập");

                    // ======================= HEADER ===========================
                    ws.Cell(1, 1).Value = "CỬA HÀNG MUA BÁN XE MÁY";
                    ws.Range(1, 1, 1, 6).Merge().Style
                        .Font.SetBold().Font.SetFontSize(12)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell(2, 1).Value = "PHIẾU NHẬP KHO XE MÁY";
                    ws.Range(2, 1, 2, 6).Merge().Style
                        .Font.SetBold().Font.SetFontSize(16)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    // ======================= THÔNG TIN ========================
                    int row = 4;
                    ws.Cell(row++, 1).Value = "Mã phiếu: " + maPhieuNhap;
                    ws.Cell(row++, 1).Value = "Ngày nhập: " + ngayNhap;
                    ws.Cell(row++, 1).Value = "Loại nhập: " + loaiNhap;

                    if (loaiNhap == "NCC")
                    {
                        ws.Cell(row++, 1).Value = "Nhà cung cấp: " + nhaCungCap;
                        ws.Cell(row++, 1).Value = "SĐT NCC: " + soDienThoaiNCC;
                        ws.Cell(row++, 1).Value = "Địa chỉ NCC: " + diaChiNCC;
                    }
                    else if (loaiNhap == "KhachHang")
                    {
                        ws.Cell(row++, 1).Value = "Khách hàng: " + tenKhachHang;
                        ws.Cell(row++, 1).Value = "SĐT KH: " + soDienThoaiKH;
                        ws.Cell(row++, 1).Value = "Địa chỉ KH: " + diaChiKH;
                    }


                    // ======================= DANH SÁCH XE ======================
                    row += 1;
                    string[] headers = { "Tên xe", "Hãng xe", "Loại xe", "Màu sắc", "Năm SX",
                             "Dung tích", "Số khung", "Số máy", "Tình trạng", "Đơn giá" };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        ws.Cell(row, i + 1).Value = headers[i];
                        ws.Cell(row, i + 1).Style.Font.SetBold();
                        ws.Cell(row, i + 1).Style.Fill.SetBackgroundColor(XLColor.LightGray);
                        ws.Cell(row, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    }

                    // Dữ liệu
                    for (int i = 0; i < dtXeNhap.Rows.Count; i++)
                    {
                        DataRow r = dtXeNhap.Rows[i];
                        for (int j = 0; j < headers.Length; j++)
                        {
                            ws.Cell(row + i + 1, j + 1).Value = r[j]?.ToString();
                            ws.Cell(row + i + 1, j + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }
                    }

                    row += dtXeNhap.Rows.Count + 1;
                    ws.Cell(row, 1).Value = "Tổng số lượng:";
                    ws.Cell(row, 10).Value = tongSoLuong;

                    row++;
                    ws.Cell(row, 1).Value = "Tổng giá trị:";
                    ws.Cell(row, 10).Value = tongGiaTri.ToString("N0") + " VNĐ";

                    // ======================= CHỮ KÝ ============================
                    row += 3;
                    ws.Cell(row, 2).Value = "Khách hàng";
                    ws.Cell(row, 8).Value = "Nhân viên";

                    ws.Columns().AdjustToContents();

                    // ======================= SAVE ==============================
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "Excel Files|*.xlsx";
                        sfd.Title = "Chọn nơi lưu phiếu nhập";
                        sfd.FileName = $"PhieuNhap_{maPhieuNhap}.xlsx";

                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            workbook.SaveAs(sfd.FileName);
                            MessageBox.Show($"Phiếu nhập đã được xuất thành công tại:\n{sfd.FileName}",
                                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnpTimKiemTonKho_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtXeTonKho == null || dtXeTonKho.Rows.Count == 0)
                {
                    MessageBox.Show("Dữ liệu xe tồn kho chưa được tải. Vui lòng tải lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string filterExpression = BuildFilterExpression();

                DataView dv = new DataView(dtXeTonKho);
                if (!string.IsNullOrEmpty(filterExpression))
                {
                    dv.RowFilter = filterExpression;
                }
                else
                {
                    dv.RowFilter = "";
                }

                dgvTonKhoChiTiet.DataSource = dv;
                FormatTonKhoGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lọc: {ex.Message}\nStackTrace: {ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string BuildFilterExpression()
        {
            string filter = "";
            bool hasFilter = false;

            // Lọc TenXe (TextBox)
            if (!string.IsNullOrEmpty(txtTenXeFilter.Text?.Trim()))
            {
                filter += $"TenXe LIKE '%{txtTenXeFilter.Text.Trim().Replace("'", "''")}%'";
                hasFilter = true;
            }

            // Lọc HangXe (dùng Text thay vì SelectedItem.ToString())
            string hangXeValue = cbbHangXeFilter.Text;
            if (!string.IsNullOrEmpty(hangXeValue) && hangXeValue != "Tất cả")
            {
                filter += hasFilter ? " AND " : "";
                filter += $"HangXe = '{hangXeValue.Replace("'", "''")}'";
                hasFilter = true;
            }

            // Lọc LoaiXe
            string loaiXeValue = cbbLoaiXeFilter.Text;
            if (!string.IsNullOrEmpty(loaiXeValue) && loaiXeValue != "Tất cả")
            {
                filter += hasFilter ? " AND " : "";
                filter += $"LoaiXe = '{loaiXeValue.Replace("'", "''")}'";
                hasFilter = true;
            }

            // Lọc MauSac
            string mauSacValue = cbbMauSacFilter.Text;
            if (!string.IsNullOrEmpty(mauSacValue) && mauSacValue != "Tất cả")
            {
                filter += hasFilter ? " AND " : "";
                filter += $"MauSac = '{mauSacValue.Replace("'", "''")}'";
                hasFilter = true;
            }
            // Lọc TinhTrang
            string tinhTrangValue = cbbTinhTrangFilter.Text;
            if (!string.IsNullOrEmpty(tinhTrangValue) && tinhTrangValue != "Tất cả")
            {
                filter += hasFilter ? " AND " : "";
                filter += $"TinhTrang = '{tinhTrangValue.Replace("'", "''")}'";
                hasFilter = true;
            }

            // Lọc NamSX (TextBox)
            if (!string.IsNullOrEmpty(txtNamSXFilter.Text?.Trim()))
            {
                if (int.TryParse(txtNamSXFilter.Text.Trim(), out int namSX))
                {
                    filter += hasFilter ? " AND " : "";
                    filter += $"Convert(NamSX, 'System.Int32') = {namSX}";
                    hasFilter = true;
                }
                else
                {
                    MessageBox.Show("Năm sản xuất phải là số nguyên.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return "";
                }
            }

            // Lọc DungTich (TextBox) - số thực
            if (!string.IsNullOrEmpty(txtDungTichFilter.Text?.Trim()))
            {
                if (decimal.TryParse(txtDungTichFilter.Text.Trim(), out decimal dungTich))
                {
                    filter += hasFilter ? " AND " : "";
                    filter += $"Convert(DungTich, 'System.Decimal') = {dungTich}";
                    hasFilter = true;
                }
                else
                {
                    MessageBox.Show("Dung tích phải là số thực hợp lệ (ví dụ: 50.0, 110.5).", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return "";
                }
            }

            // Lọc DonGia
            if (!string.IsNullOrEmpty(txtDonGiaFilter.Text) && decimal.TryParse(txtDonGiaFilter.Text, out decimal gia))
            {
                filter += hasFilter ? " AND " : "";
                filter += $"Convert(DonGia, 'System.Decimal') {cbbDonGiaOperator.Text} {gia}";
                hasFilter = true;
            }

            // Lọc Số khung
            if (!string.IsNullOrEmpty(txtSoKhungFilter.Text?.Trim()))
            {
                filter += hasFilter ? " AND " : "";
                filter += $"SoKhung LIKE '%{txtSoKhungFilter.Text.Trim().Replace("'", "''")}%'";
                hasFilter = true;
            }

            // Lọc Số máy
            if (!string.IsNullOrEmpty(txtSoMayFilter.Text?.Trim()))
            {
                filter += hasFilter ? " AND " : "";
                filter += $"SoMay LIKE '%{txtSoMayFilter.Text.Trim().Replace("'", "''")}%'";
                hasFilter = true;
            }



            return filter;
        }

        private void btnpResetTonKho_Click(object sender, EventArgs e)
        {
            try
            {
                cbbHangXeFilter.SelectedIndex = -1;
                cbbLoaiXeFilter.SelectedIndex = -1;
                cbbMauSacFilter.SelectedIndex = -1;
                cbbTinhTrangFilter.SelectedIndex = -1;
                cbbDonGiaOperator.SelectedIndex = -1;
                txtDonGiaFilter.Clear();
                txtTenXeFilter.Clear();
                txtNamSXFilter.Clear();
                txtDungTichFilter.Clear();
                txtSoKhungFilter.Clear();
                txtSoMayFilter.Clear();

                dgvTonKhoChiTiet.DataSource = dtXeTonKho;
                FormatTonKhoGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa lọc: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnpThemNCC_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenNCC.Text) || string.IsNullOrWhiteSpace(txtDiaChiNCC.Text) || string.IsNullOrWhiteSpace(txtSDTNCC.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin nhà cung cấp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (quanLyNhapXe_Kho.InsertNhaCungCap(txtTenNCC.Text, txtDiaChiNCC.Text, txtSDTNCC.Text))
            {
                MessageBox.Show("Thêm NCC thành công!");
                LoadNhaCungCap();
            }
        }

        private void btnpXoaNCC_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtNCCID.Text, out int nccID))
            {
                int result = quanLyNhapXe_Kho.DeleteNhaCungCap(nccID);

                switch (result)
                {
                    case 0:
                        MessageBox.Show("Xóa nhà cung cấp thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadNhaCungCap();
                        break;
                    case 1:
                        MessageBox.Show("Nhà cung cấp không tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    case 2:
                        MessageBox.Show("Nhà cung cấp đang được sử dụng trong phiếu nhập. Vui lòng xóa hoặc cập nhật trước.",
                            "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    default:
                        MessageBox.Show("Không thể xóa nhà cung cấp. Có lỗi xảy ra.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập ID nhà cung cấp hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnpSuaNCC_Click(object sender, EventArgs e)
        {
            int id = int.Parse(txtNCCID.Text);
            if (quanLyNhapXe_Kho.UpdateNhaCungCap(id, txtTenNCC.Text, txtDiaChiNCC.Text, txtSDTNCC.Text))
            {
                MessageBox.Show("Cập nhật NCC thành công!");
                LoadNhaCungCap();
            }
        }

        private void btnResetNCC_Click(object sender, EventArgs e)
        {
            txtNCCID.Clear();
            txtTenNCC.Clear();
            txtDiaChiNCC.Clear();
            txtSDTNCC.Clear();
        }

        private void dgvNCC_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvNCC.Rows[e.RowIndex];

                txtNCCID.Text = row.Cells["NCCID"].Value.ToString();
                txtTenNCC.Text = row.Cells["TenNCC"].Value.ToString();
                txtDiaChiNCC.Text = row.Cells["DiaChi"].Value.ToString();
                txtSDTNCC.Text = row.Cells["SoDienThoai"].Value.ToString();
            }
        }

        private void btnpThemDanhMuc_Click(object sender, EventArgs e)
        {
            string loaiDanhMuc = cbbLoaiDanhMuc.Text.Trim();
            string giaTri = txtGiaTri.Text.Trim();

            if (string.IsNullOrEmpty(loaiDanhMuc))
            {
                MessageBox.Show("Vui lòng chọn loại danh mục!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!KiemTraGiaTriHopLe(giaTri))
                return;

            try
            {
                bool result = quanLyNhapXe_Kho.InsertDanhMuc(loaiDanhMuc, giaTri);
                if (result)
                {
                    MessageBox.Show($"Thêm danh mục '{giaTri}' thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDanhMuc(loaiDanhMuc);
                    txtGiaTri.Clear();
                }
                else
                {
                    MessageBox.Show($"Giá trị '{giaTri}' đã tồn tại trong danh mục '{loaiDanhMuc}'!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm danh mục: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool KiemTraGiaTriHopLe(string giaTri)
        {
            if (string.IsNullOrWhiteSpace(giaTri))
            {
                MessageBox.Show("Giá trị danh mục không được để trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (giaTri.Length > 100)
            {
                MessageBox.Show("Giá trị danh mục không được dài quá 100 ký tự!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!Regex.IsMatch(giaTri, @"^[a-zA-Z0-9\s]+$"))
            {
                MessageBox.Show("Giá trị danh mục chỉ được chứa chữ cái, số và khoảng trắng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void LoadDanhMuc(string loaiDanhMuc)
        {
            DataTable dt = quanLyNhapXe_Kho.GetDanhMucByLoai(loaiDanhMuc);
            dgvDanhMuc.DataSource = dt;
            if (dgvDanhMuc.Columns["DanhMucID"] != null)
            {
                dgvDanhMuc.Columns["DanhMucID"].Visible = false;
            }
            txtGiaTri.Clear();
            btnpSuaDanhMuc.Enabled = false;
            btnpXoaDanhMuc.Enabled = false;
        }

        private void btnpXoaDanhMuc_Click(object sender, EventArgs e)
        {
            if (dgvDanhMuc.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một danh mục để xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string loaiDanhMuc = cbbLoaiDanhMuc.Text.Trim();
            var row = dgvDanhMuc.SelectedRows[0];

            int danhMucID = Convert.ToInt32(row.Cells["DanhMucID"].Value);
            string giaTri = row.Cells["GiaTri"].Value?.ToString();

            if (MessageBox.Show($"Bạn có chắc muốn xóa danh mục '{giaTri}'?",
                                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    bool result = quanLyNhapXe_Kho.DeleteDanhMuc(danhMucID);
                    if (result)
                    {
                        MessageBox.Show($"Xóa danh mục '{giaTri}' thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDanhMuc(loaiDanhMuc);
                    }
                    else
                    {
                        MessageBox.Show($"Không thể xóa danh mục '{giaTri}' vì nó đang được sử dụng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa danh mục: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnpSuaDanhMuc_Click(object sender, EventArgs e)
        {
            if (dgvDanhMuc.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một danh mục để sửa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string loaiDanhMuc = cbbLoaiDanhMuc.Text.Trim();
            string giaTri = txtGiaTri.Text.Trim();

            if (!KiemTraGiaTriHopLe(giaTri))
                return;

            var row = dgvDanhMuc.SelectedRows[0];
            int danhMucID = Convert.ToInt32(row.Cells["DanhMucID"].Value);

            try
            {
                bool result = quanLyNhapXe_Kho.UpdateDanhMuc(danhMucID, giaTri);
                if (result)
                {
                    MessageBox.Show($"Sửa danh mục thành '{giaTri}' thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDanhMuc(loaiDanhMuc);
                    txtGiaTri.Clear();
                }
                else
                {
                    MessageBox.Show($"Giá trị '{giaTri}' đã tồn tại trong danh mục '{loaiDanhMuc}'!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi sửa danh mục: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbbLoaiDanhMuc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbLoaiDanhMuc.SelectedItem != null)
            {
                LoadDanhMuc(cbbLoaiDanhMuc.SelectedItem.ToString());
                txtGiaTri.Clear(); // Xóa trường nhập liệu
                btnpThemDanhMuc.Enabled = true;
                btnpSuaDanhMuc.Enabled = false;
                btnpXoaDanhMuc.Enabled = false;
            }
        }

        private void dgvDanhMuc_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDanhMuc.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvDanhMuc.SelectedRows[0];
                txtGiaTri.Text = row.Cells["GiaTri"].Value.ToString();
                btnpSuaDanhMuc.Enabled = true;
                btnpXoaDanhMuc.Enabled = true;
            }
            else
            {
                txtGiaTri.Clear();
                btnpSuaDanhMuc.Enabled = false;
                btnpXoaDanhMuc.Enabled = false;
            }
        }

        //--------------------------------------------------------------------------------------------------

        private void LoadDataKhachHang()
        {
            try
            {
                string sql = @"SELECT * FROM dbo.fn_DSKH() ORDER BY KhachHangID";
                DataTable dt = quanLyKH.ExecuteQuery(sql);
                dgvDSKH.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load dữ liệu: " + ex.Message, "Thông báo lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadLoaiKH()
        {
            string query = "SELECT MaLoaiKH, TenLoaiKH FROM PhanLoaiKH";
            try
            {
                DataTable dtLoaiKH = quanLyKH.ExecuteQuery(query);
                cbbLoaiKH.DataSource = dtLoaiKH;
                cbbLoaiKH.DisplayMember = "TenLoaiKH";
                cbbLoaiKH.ValueMember = "MaLoaiKH";
                cbbLoaiKH.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi nạp dữ liệu: " + ex.Message);
            }
        }
        private void dgvDSKH_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvDSKH.CurrentRow != null && dgvDSKH.CurrentRow.Index >= 0)
            {
                LoadLoaiKH();

                DataGridViewRow row = dgvDSKH.CurrentRow;
                txtMaKH.Text = row.Cells["MaKH"].Value?.ToString() ?? "";
                txtHoTenKH.Text = row.Cells["HoTen"].Value?.ToString() ?? "";
                txtSoDT.Text = row.Cells["SDT_KH"].Value?.ToString() ?? "";
                txtDiaChi.Text = row.Cells["DiaChi"].Value?.ToString() ?? "";
                string tenLoaiKH = row.Cells["LoaiKH"].Value?.ToString() ?? "";
                cbbLoaiKH.SelectedIndex = cbbLoaiKH.FindStringExact(tenLoaiKH);
            }
            else
            {
                txtMaKH.Clear();
                txtHoTenKH.Clear();
                txtSoDT.Clear();
                txtDiaChi.Clear();
                cbbLoaiKH.Text = "";
            }
        }
        private void btnThemKH_Click(object sender, EventArgs e)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@HoTen", txtHoTenKH?.Text ?? string.Empty },
                    { "@TenLoaiKH", cbbLoaiKH.Text.ToString() },
                    { "@DiaChi", txtDiaChi?.Text ?? string.Empty },
                    { "@SoDienThoai", txtSoDT?.Text ?? string.Empty },

                };
                int rows = quanLyKH.ExecuteNonQuery("sp_InsertKhachHang", parameters);

                if (rows > 0)
                    MessageBox.Show("Thêm khách hàng thành công!");
                else
                    MessageBox.Show("Không thêm được khách hàng.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            LoadDataKhachHang();
        }

        private void btnXoaKH_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMaKH.Text))
                {
                    MessageBox.Show("Vui lòng chọn khách hàng cần xóa!");
                    return;
                }

                DialogResult dr = MessageBox.Show(
                    "Bạn có chắc chắn muốn xóa khách hàng này?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dr == DialogResult.Yes)
                {
                    var parameters = new Dictionary<string, object>
                    {
                        { "@MaKH", txtMaKH.Text }
                    };

                    int rows = quanLyKH.ExecuteNonQuery("sp_DeleteKhachHang", parameters);

                    if (rows > 0)
                        MessageBox.Show("Xóa khách hàng thành công!");
                    else
                        MessageBox.Show("Không thể xóa khách hàng.");

                    LoadDataKhachHang();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnSuaKH_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMaKH.Text))
                {
                    MessageBox.Show("Vui lòng chọn khách hàng cần cập nhật!");
                    return;
                }

                var parameters = new Dictionary<string, object>
                {
                    { "@MaKH", txtMaKH?.Text ?? string.Empty },
                    { "@HoTen", txtHoTenKH?.Text ?? string.Empty },
                    { "@TenLoaiKH", cbbLoaiKH.Text.ToString() },
                    { "@DiaChi", txtDiaChi?.Text ?? string.Empty },
                    { "@SoDienThoai", txtSoDT?.Text ?? string.Empty },

                };

                int rows = quanLyKH.ExecuteNonQuery("sp_UpdateKhachHang", parameters);

                if (rows > 0)
                    MessageBox.Show("Cập nhật khách hàng thành công!");
                else
                    MessageBox.Show("Không cập nhật được khách hàng.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            LoadDataKhachHang();
        }

        private void btnTimKiemKH_Click(object sender, EventArgs e)
        {
            string loaiTim = cbbLoaiTim.SelectedItem?.ToString();
            string tuKhoa = txtThongTinTimKiem.Text.Trim();

            if (string.IsNullOrEmpty(tuKhoa))
            {
                MessageBox.Show("Vui lòng nhập thông tin cần tìm!");
                LoadDataKhachHang();
                return;
            }

            string query = "";
            SqlParameter param = null;

            switch (loaiTim)
            {
                case "Họ tên":
                    query = "SELECT * FROM dbo.fn_FindKhachHangByHoTen(@HoTen)";
                    param = new SqlParameter("@HoTen", tuKhoa);
                    break;

                case "SĐT":
                    query = "SELECT * FROM dbo.fn_FindKhachHangBySDT(@SDT)";
                    param = new SqlParameter("@SDT", tuKhoa);
                    break;

                case "Loại KH":
                    query = "SELECT * FROM dbo.fn_FindKhachHangByLoaiKH(@TenLoaiKH)";
                    param = new SqlParameter("@TenLoaiKH", tuKhoa);
                    break;

                default:
                    MessageBox.Show("Vui lòng chọn loại tìm kiếm!");
                    return;
            }

            try
            {
                DataTable dt = quanLyKH.ExecuteQuery(query, param);

                if (dt.Rows.Count > 0)
                {
                    dgvDSKH.DataSource = dt;
                }
                else
                {
                    MessageBox.Show("Không tìm thấy khách hàng nào.");
                    var schema = (dgvDSKH.DataSource as DataTable)?.Clone() ?? new DataTable();
                    dgvDSKH.DataSource = schema;
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm: " + ex.Message);
            }
        }

        public void LoadDataLichHen()
        {
            try
            {
                string sql = "SELECT * FROM dbo.fn_LichHenBaoTri_KhachHang()";
                DataTable dt = quanLyKH.ExecuteQuery(sql);
                dgvLichHen.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load dữ liệu: " + ex.Message, "Thông báo lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnThemLH_Click(object sender, EventArgs e)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@SoKhung", txtXeCTID.Text.ToString() },
                    { "@NgayHen", dtpNgayHen.Value },
                    { "@CaLamViec", cbbCaLamViec.Text },
                    { "@LyDoHen", txtLyDoHen.Text.ToString() },
                    { "@GhiChu", txtGhiChu.Text.ToString()}
                };
                quanLyKH.ExecuteNonQuery("sp_DatLichHen", parameters);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            LoadDataLichHen();
        }


        private void dgvLichHen_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvLichHen.CurrentRow != null && dgvLichHen.CurrentRow.Index >= 0)
            {


                int maLichHen = Convert.ToInt32(dgvLichHen.CurrentRow.Cells["MaLH"].Value);
                string query = "SELECT * FROM dbo.fn_GetLichHen(@MaLichHen)";

                SqlParameter param = new SqlParameter("@MaLichHen", maLichHen);


                DataTable dt = quanLyKH.ExecuteQuery(query, param);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    txtMaLichHen.Text = maLichHen.ToString();
                    txtXeCTID.Text = row["XeCTID"].ToString();
                    dtpNgayHen.Value = Convert.ToDateTime(row["NgayHen"]);
                    dtpNgayTao.Value = Convert.ToDateTime(row["NgayTao"]);
                    cbbCaLamViec.Text = row["CaLamViec"].ToString();
                    txtLyDoHen.Text = row["LyDoHen"].ToString();
                    txtGhiChu.Text = row["GhiChu"].ToString();
                    txtTrangThaiHen.Text = row["TrangThaiHen"].ToString();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy thông tin lịch hẹn!");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một lịch hẹn để cập nhật!");
            }
        }

        private void btnCapNhatLH_Click(object sender, EventArgs e)
        {
            if (dgvLichHen.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn dòng cần sửa");
                return;
            }
            int maLH = Convert.ToInt32(dgvLichHen.SelectedRows[0].Cells["MaLH"].Value);

            var result = MessageBox.Show(
                "Bạn có chắc muốn sửa dòng này?",
                "Xác nhận sửa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            try
            {
                var parameters = new Dictionary<string, object>
                    {
                        { "@MaLichHen", maLH },
                        { "@NgayHen", dtpNgayHen.Value },
                        { "@CaLamViec", cbbCaLamViec.Text },
                        { "@LyDoHen", txtLyDoHen.Text.ToString() },
                        { "@GhiChu", txtGhiChu.Text.ToString()},
                        { "@TrangThaiHen", txtTrangThaiHen.Text.ToString() },
                    };

                quanLyKH.ExecuteNonQuery("sp_UpdateLichHen", parameters);
                MessageBox.Show("Cập nhật Lich hen thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            LoadDataLichHen();


        }

        private void btnHuyLH_Click(object sender, EventArgs e)
        {
            if (dgvLichHen.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn dòng cần hủy");
                return;
            }
            int maLH = Convert.ToInt32(dgvLichHen.SelectedRows[0].Cells["MaLH"].Value);

            var result = MessageBox.Show(
                "Bạn có chắc muốn hủy lịch này?",
                "Xác nhận hủy",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@MaLichHen", maLH }
                };
                quanLyKH.ExecuteNonQuery("sp_HuyLichHen", parameters);
                MessageBox.Show("Hủy Lich hen thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnTimKiemLH_Click(object sender, EventArgs e)
        {
            DateTime dtNgayhen = Convert.ToDateTime(dtpTimKiemNgayHen.Value);
            string query = "SELECT * FROM dbo.fn_FindLichHen_NgayHen(@NgayHen)";

            SqlParameter param = new SqlParameter("@NgayHen", dtNgayhen);


            DataTable dt = quanLyKH.ExecuteQuery(query, param);

            dgvLichHen.DataSource = dt;
        }

        private void btnLoadTongQuan_Click(object sender, EventArgs e)
        {
            LoadTatCaBaoCao();

            MessageBox.Show("Đã cập nhật dữ liệu tổng quan!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void LoadNhanVienList()
        {
            try
            {
                dgvDSNV.DataSource = qlnv.GetNhanVienList();

                // Đặt lại tiêu đề cột để hiển thị cho người dùng
                if (dgvDSNV.Columns.Contains("MaNV")) dgvDSNV.Columns["MaNV"].HeaderText = "Mã NV";
                if (dgvDSNV.Columns.Contains("HoTenNV")) dgvDSNV.Columns["HoTenNV"].HeaderText = "Họ tên NV";
                if (dgvDSNV.Columns.Contains("NgaySinh")) dgvDSNV.Columns["NgaySinh"].HeaderText = "Ngày sinh";
                if (dgvDSNV.Columns.Contains("GioiTinh")) dgvDSNV.Columns["GioiTinh"].HeaderText = "Giới tính";
                if (dgvDSNV.Columns.Contains("SoDT")) dgvDSNV.Columns["SoDT"].HeaderText = "Số ĐT";
                if (dgvDSNV.Columns.Contains("Email")) dgvDSNV.Columns["Email"].HeaderText = "Email";
                if (dgvDSNV.Columns.Contains("ChucVu")) dgvDSNV.Columns["ChucVu"].HeaderText = "Chức vụ";
                if (dgvDSNV.Columns.Contains("LuongCB"))
                {
                    dgvDSNV.Columns["LuongCB"].HeaderText = "Lương cơ bản";
                    dgvDSNV.Columns["LuongCB"].DefaultCellStyle.Format = "N0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách nhân viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnThemNV_Click(object sender, EventArgs e)
        {
            Them_Sua_NV f = new Them_Sua_NV(0);
            if (f.ShowDialog() == DialogResult.OK)
            {
                LoadNhanVienList();
            }
        }

        private void btnCapNhatNV_Click(object sender, EventArgs e)
        {
            if (dgvDSNV.SelectedRows.Count > 0)
            {
                int maNV = Convert.ToInt32(dgvDSNV.SelectedRows[0].Cells["MaNV"].Value);
                Them_Sua_NV f = new Them_Sua_NV(maNV);
                if (f.ShowDialog() == DialogResult.OK)
                {
                    LoadNhanVienList();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một nhân viên để cập nhật.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnXoaNV_Click(object sender, EventArgs e)
        {
            if (dgvDSNV.SelectedRows.Count > 0)
            {
                int maNV = Convert.ToInt32(dgvDSNV.SelectedRows[0].Cells["MaNV"].Value);
                string hoTen = dgvDSNV.SelectedRows[0].Cells["HoTenNV"].Value.ToString();

                if (MessageBox.Show($"Xóa nhân viên {hoTen}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        qlnv.DeleteNhanVien(maNV);
                        MessageBox.Show("Xóa nhân viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadNhanVienList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void LoadChamCongList()
        {
            try
            {
                dgvChamCong.DataSource = qlnv.GetChamCongList();

                if (dgvChamCong.Columns.Contains("MaChamCong")) dgvChamCong.Columns["MaChamCong"].HeaderText = "Mã CC";
                if (dgvChamCong.Columns.Contains("MaNV")) dgvChamCong.Columns["MaNV"].HeaderText = "Mã NV";
                if (dgvChamCong.Columns.Contains("HoTenNV")) dgvChamCong.Columns["HoTenNV"].HeaderText = "Họ tên";
                if (dgvChamCong.Columns.Contains("NgayLamViec")) dgvChamCong.Columns["NgayLamViec"].HeaderText = "Ngày làm";
                if (dgvChamCong.Columns.Contains("TgVaoLam")) dgvChamCong.Columns["TgVaoLam"].HeaderText = "Giờ vào";
                if (dgvChamCong.Columns.Contains("TgTanCa")) dgvChamCong.Columns["TgTanCa"].HeaderText = "Giờ tan";
                if (dgvChamCong.Columns.Contains("TrangThai")) dgvChamCong.Columns["TrangThai"].HeaderText = "Trạng thái";
                if (dgvChamCong.Columns.Contains("LyDoNghiPhep")) dgvChamCong.Columns["LyDoNghiPhep"].HeaderText = "Lý do nghỉ";
                if (dgvChamCong.Columns.Contains("TongThoiGianLamViec")) dgvChamCong.Columns["TongThoiGianLamViec"].HeaderText = "Tổng giờ làm";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách chấm công: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnThemCC_Click(object sender, EventArgs e)
        {
            Them_Sua_ChamCong f = new Them_Sua_ChamCong(0);
            if (f.ShowDialog() == DialogResult.OK)
            {
                LoadChamCongList();
            }
        }

        private void btnCapNhatCC_Click(object sender, EventArgs e)
        {
            if (dgvChamCong.SelectedRows.Count > 0)
            {
                int maCC = Convert.ToInt32(dgvChamCong.SelectedRows[0].Cells["MaChamCong"].Value);
                Them_Sua_ChamCong f = new Them_Sua_ChamCong(maCC);
                if (f.ShowDialog() == DialogResult.OK)
                {
                    LoadChamCongList();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một bản ghi chấm công để cập nhật.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnXoaCC_Click(object sender, EventArgs e)
        {
            if (dgvChamCong.SelectedRows.Count > 0)
            {
                int maCC = Convert.ToInt32(dgvChamCong.SelectedRows[0].Cells["MaChamCong"].Value);

                if (MessageBox.Show($"Xóa bản ghi chấm công Mã CC: {maCC}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        qlnv.DeleteChamCong(maCC);
                        MessageBox.Show("Xóa chấm công thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadChamCongList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void LoadThongBaoList()
        {
            try
            {
                dgvThongBao.DataSource = qlnv.GetThongBaoList();

                if (dgvThongBao.Columns.Contains("MaThongBao")) dgvThongBao.Columns["MaThongBao"].HeaderText = "Mã TB";
                if (dgvThongBao.Columns.Contains("TieuDe")) dgvThongBao.Columns["TieuDe"].HeaderText = "Tiêu đề";
                if (dgvThongBao.Columns.Contains("LoaiThongBao")) dgvThongBao.Columns["LoaiThongBao"].HeaderText = "Loại thông báo";
                if (dgvThongBao.Columns.Contains("NoiDung")) dgvThongBao.Columns["NoiDung"].HeaderText = "Nội dung";
                if (dgvThongBao.Columns.Contains("NgayTao")) dgvThongBao.Columns["NgayTao"].HeaderText = "Ngày tạo";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách thông báo: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvThongBao_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvThongBao.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvThongBao.SelectedRows[0];
                object noiDungValue = selectedRow.Cells["NoiDung"].Value;

                if (noiDungValue != null)
                {
                    txtNoiDungChiTiet.Text = noiDungValue.ToString();
                }
                else
                {
                    txtNoiDungChiTiet.Text = "Không có nội dung chi tiết.";
                }
            }
            else
            {
                txtNoiDungChiTiet.Clear();
            }
        }

        private void btnThemThongBao_Click(object sender, EventArgs e)
        {
            ThemThongBao f = new ThemThongBao();
            if (f.ShowDialog() == DialogResult.OK)
            {
                LoadThongBaoList();
            }
        }

        private void btnXoaThongBao_Click(object sender, EventArgs e)
        {
            if (dgvThongBao.SelectedRows.Count > 0)
            {
                int maTB = Convert.ToInt32(dgvThongBao.SelectedRows[0].Cells["MaThongBao"].Value);

                if (MessageBox.Show($"Xóa thông báo Mã TB: {maTB}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        qlnv.DeleteThongBao(maTB);
                        MessageBox.Show("Xóa thông báo thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadThongBaoList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnTaiLaiThongBao_Click(object sender, EventArgs e)
        {
            LoadThongBaoList();
            MessageBox.Show("Tải lại danh sách thông báo thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnTimKiemTTNV_Click(object sender, EventArgs e)
        {
            try
            {
                string searchText = txtTimKiemTTNV.Text.Trim();
                dgvDSNV.DataSource = qlnv.SearchNhanVien(searchText);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm nhân viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ThucHienTimKiemChamCong()
        {
            try
            {
                DateTime? ngayLamViec = dtpChamCong.Value.Date;
                string maNVText = txtTimKiemCC.Text.Trim();
                dgvChamCong.DataSource = qlnv.SearchChamCong(ngayLamViec, maNVText);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm chấm công: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTimKiemCC_Click(object sender, EventArgs e)
        {
            ThucHienTimKiemChamCong();
        }

        private void dtpChamCong_ValueChanged(object sender, EventArgs e)
        {
            ThucHienTimKiemChamCong();
        }

        private int selectedMaNVLuong = -1;
        private DataTable currentBangLuong = null;

        private void LoadBangLuong()
        {
            try
            {
                DateTime selectedMonth = dtpLuong.Value.Date;
                currentBangLuong = qlnv.GetBangLuongList(selectedMonth);
                dgvBangLuong.DataSource = currentBangLuong;
                FormatDGVLuong();

                if (dgvBangLuong.Rows.Count > 0)
                {
                    dgvBangLuong.Rows[0].Selected = true;
                    dgvBangLuong_CellClick(null, new DataGridViewCellEventArgs(0, 0));
                }
                else
                {
                    ClearChiTietLuong();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải bảng lương: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatDGVLuong()
        {
            if (dgvBangLuong.Columns.Contains("LuongThucNhanTamTinh"))
            {
                dgvBangLuong.Columns["LuongThucNhanTamTinh"].HeaderText = "Lương tạm tính";
                dgvBangLuong.Columns["LuongThucNhanTamTinh"].DefaultCellStyle.Format = "N0";
            }
            if (dgvBangLuong.Columns.Contains("LuongCB"))
            {
                dgvBangLuong.Columns["LuongCB"].HeaderText = "Lương cơ bản";
                dgvBangLuong.Columns["LuongCB"].Visible = false;
            }
            if (dgvBangLuong.Columns.Contains("MaNV")) dgvBangLuong.Columns["MaNV"].HeaderText = "Mã NV";
            if (dgvBangLuong.Columns.Contains("HoTenNV")) dgvBangLuong.Columns["HoTenNV"].HeaderText = "Họ tên NV";
            if (dgvBangLuong.Columns.Contains("ChucVu")) dgvBangLuong.Columns["ChucVu"].HeaderText = "Chức vụ";
            if (dgvBangLuong.Columns.Contains("ThangNamHienThi")) dgvBangLuong.Columns["ThangNamHienThi"].HeaderText = "Tháng/Năm";
            if (dgvBangLuong.Columns.Contains("TongGioLam")) dgvBangLuong.Columns["TongGioLam"].HeaderText = "Tổng giờ làm";
            if (dgvBangLuong.Columns.Contains("LuongThucNhan")) dgvBangLuong.Columns["LuongThucNhan"].HeaderText = "Lương thực nhận";
            if (dgvBangLuong.Columns.Contains("KhoanThuong")) dgvBangLuong.Columns["KhoanThuong"].HeaderText = "Khoản thưởng";
            if (dgvBangLuong.Columns.Contains("KhoanKhauTru")) dgvBangLuong.Columns["KhoanKhauTru"].HeaderText = "Khoản khấu trừ";
        }

        private void ClearChiTietLuong()
        {
            txtLuongCoBan.Text = "0";
            txtKhoanThuongPhuCap.Text = "0";
            txtKhoanKhauTru.Text = "0";
            txtLuongThucNhan.Text = "0";
            selectedMaNVLuong = -1;
        }

        private void dtpLuong_ValueChanged(object sender, EventArgs e)
        {
            LoadBangLuong();
        }

        private void dgvBangLuong_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvBangLuong.Rows.Count) return;

            DataGridViewRow row = dgvBangLuong.Rows[e.RowIndex];

            if (row.Cells["MaNV"].Value == null || row.Cells["MaNV"].Value == DBNull.Value) return;

            selectedMaNVLuong = Convert.ToInt32(row.Cells["MaNV"].Value);
            decimal luongTamTinh = Convert.ToDecimal(row.Cells["LuongThucNhanTamTinh"].Value);
            decimal luongCoBan = qlnv.GetLuongCoBanByMaNV(selectedMaNVLuong);
            decimal phuCapThuong = 0;
            decimal khauTru = 0;

            string thuongText = txtKhoanThuongPhuCap.Text.Replace(",", "").Replace(".", "");
            if (!decimal.TryParse(thuongText, out phuCapThuong))
            {
                phuCapThuong = 0;
            }
            txtKhoanThuongPhuCap.Text = phuCapThuong.ToString("N0");


            string khauTruText = txtKhoanKhauTru.Text.Replace(",", "").Replace(".", "");
            if (!decimal.TryParse(khauTruText, out khauTru))
            {
                khauTru = 0;
            }
            txtKhoanKhauTru.Text = khauTru.ToString("N0");

            decimal luongThucNhanCuoi = luongTamTinh + phuCapThuong - khauTru;
            txtLuongCoBan.Text = luongCoBan.ToString("N0");
            txtLuongThucNhan.Text = luongThucNhanCuoi.ToString("N0");
        }

        private void UpdateLuongThucNhan(object sender, EventArgs e)
        {
            if (selectedMaNVLuong == -1 || dgvBangLuong.SelectedRows.Count == 0) return;

            DataGridViewRow row = dgvBangLuong.SelectedRows[0];
            decimal luongTamTinh = Convert.ToDecimal(row.Cells["LuongThucNhanTamTinh"].Value);

            decimal phuCapThuong = 0;
            decimal khauTru = 0;
            string thuongText = txtKhoanThuongPhuCap.Text.Replace(",", "").Replace(".", "");
            if (decimal.TryParse(thuongText, out decimal tempThuong))
            {
                phuCapThuong = tempThuong;
                txtKhoanThuongPhuCap.Text = phuCapThuong.ToString("N0");
            }

            string khauTruText = txtKhoanKhauTru.Text.Replace(",", "").Replace(".", "");
            if (decimal.TryParse(khauTruText, out decimal tempKhauTru))
            {
                khauTru = tempKhauTru;
                txtKhoanKhauTru.Text = khauTru.ToString("N0");
            }
            decimal luongThucNhanCuoi = luongTamTinh + phuCapThuong - khauTru;
            txtLuongThucNhan.Text = luongThucNhanCuoi.ToString("N0");
        }

        private void btnTinhLuong_Click(object sender, EventArgs e)
        {
            if (selectedMaNVLuong == -1 || dgvBangLuong.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một nhân viên trong bảng lương trước khi tính.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DateTime thangTinhLuong = dtpLuong.Value.Date;
                decimal luongThucNhan = decimal.Parse(txtLuongThucNhan.Text.Replace(",", "").Replace(".", ""));
                decimal khoanThuong = decimal.Parse(txtKhoanThuongPhuCap.Text.Replace(",", "").Replace(".", ""));
                decimal khoanKhauTru = decimal.Parse(txtKhoanKhauTru.Text.Replace(",", "").Replace(".", ""));

                bool success = qlnv.LuuBangLuong(
                    selectedMaNVLuong,
                    thangTinhLuong,
                    luongThucNhan,
                    khoanThuong,
                    khoanKhauTru
                );

                if (success)
                {
                    MessageBox.Show($"Đã lưu bảng lương tháng {thangTinhLuong.Month}/{thangTinhLuong.Year} cho Mã NV: {selectedMaNVLuong} thành công!", "Hoàn thành", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBangLuong();
                }
                else
                {
                    MessageBox.Show("Lưu bảng lương thất bại. Vui lòng kiểm tra lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tính và lưu lương: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToExcel(DataTable dt, string sheetName)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel Workbook|*.xlsx", FileName = $"{sheetName}_{dtpLuong.Value.ToString("yyyyMM")}" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                        excelApp.Visible = false;
                        Microsoft.Office.Interop.Excel.Workbook workbook = excelApp.Workbooks.Add(Type.Missing);
                        Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];
                        worksheet.Name = sheetName;

                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            worksheet.Cells[1, i + 1] = dt.Columns[i].ColumnName;
                        }

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                worksheet.Cells[i + 2, j + 1] = dt.Rows[i][j].ToString();
                            }
                        }
                        workbook.SaveAs(sfd.FileName);
                        workbook.Close();
                        excelApp.Quit();

                        MessageBox.Show("Xuất file Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xuất file Excel: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            if (currentBangLuong == null || currentBangLuong.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu bảng lương để xuất.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ExportToExcel(currentBangLuong, "BangLuong");
        }

        private void LoadTatCaBaoCao()
        {
            try
            {
                string criteriaNV = "ChucVu";

                if (cbbTieuChiNV.SelectedItem != null)
                {
                    if (cbbTieuChiNV.SelectedItem.ToString() == "Giới tính")
                        criteriaNV = "GioiTinh";
                    else
                        criteriaNV = "ChucVu";
                }


                DataTable dtLuongBar = qlnv.GetDataForBarChartLuongTong();
                LoadBarChart(chartLuongTong, dtLuongBar, "Thang", "TongLuong", "Tổng lương", "Biểu đồ tổng lương");

                DataTable dtLuongLine = qlnv.GetDataForLineChartLuong();
                LoadLineChart(chartLuongTB, dtLuongLine, "Thang", "LuongTrungBinh", "Lương TB", "Biểu đồ xu hướng lương TB");

                DataTable dtChamCong = qlnv.GetDataForBarChartChamCong();
                LoadBarChart(chartChamCong, dtChamCong, "TrangThai", "SoLuong", "Số lượng", "Biểu đồ chấm công");

                DataTable dtNhanVien = qlnv.GetDataForPieChartNhanVien(criteriaNV);
                LoadPieChart(chartNhanVien, dtNhanVien, "Category", "Count", $"Biểu đồ nhân viên theo {criteriaNV}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu báo cáo: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadBarChart(Chart chartControl, DataTable dt, string xValue, string yValue, string seriesName, string chartTitle)
        {
            chartControl.Series.Clear();
            chartControl.Titles.Clear();

            Series series = chartControl.Series.Add(seriesName);
            series.ChartType = SeriesChartType.Column;
            series.IsValueShownAsLabel = true;
            series.LegendText = seriesName;

            series.XValueMember = xValue;
            series.YValueMembers = yValue;
            chartControl.DataSource = dt;
            chartControl.DataBind();

            if (chartControl.ChartAreas.Count > 0)
            {
                chartControl.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                chartControl.ChartAreas[0].AxisY.Minimum = 0;
            }
            chartControl.Titles.Add(chartTitle);
        }

        private void LoadLineChart(Chart chartControl, DataTable dt, string xValue, string yValue, string seriesName, string chartTitle)
        {
            chartControl.Series.Clear();
            chartControl.Titles.Clear();

            Series series = chartControl.Series.Add(seriesName);
            series.ChartType = SeriesChartType.Line;
            series.MarkerStyle = MarkerStyle.Circle;
            series.MarkerSize = 8;
            series.BorderWidth = 3;
            series.LegendText = seriesName;
            series.XValueMember = xValue;
            series.YValueMembers = yValue;
            chartControl.DataSource = dt;
            chartControl.DataBind();

            if (chartControl.ChartAreas.Count > 0)
            {
                chartControl.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                chartControl.ChartAreas[0].AxisY.Minimum = 0;
            }
            chartControl.Titles.Add(chartTitle);
        }

        private void LoadPieChart(Chart chartControl, DataTable dt, string nameValue, string yValue, string chartTitle)
        {
            chartControl.Series.Clear();
            chartControl.Titles.Clear();

            Series series = chartControl.Series.Add("Default");
            series.ChartType = SeriesChartType.Pie;
            series.IsValueShownAsLabel = true;
            series.Label = "#VALY";
            series.LegendText = "#VALX (#PERCENT)";

            chartControl.DataSource = dt;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string label = dt.Rows[i][nameValue].ToString();
                double value = Convert.ToDouble(dt.Rows[i][yValue]);
                series.Points.AddXY(label, value);
            }
            chartControl.Titles.Add(chartTitle);
        }

        public void VeBieuDoDoanhThuNam(DataTable dtDoanhThu)
        {
            chart3.Series.Clear();
            chart3.ChartAreas.Clear();
            decimal tongtien = 0M;

            if (dtDoanhThu == null || dtDoanhThu.Rows.Count == 0)
            {
                // Bạn có thể muốn hiển thị thông báo ở đây
                chart3.Series.Clear();
                return;
            }

            // 1. Setup Chart Area và Định Dạng
            ChartArea chartArea = new ChartArea("MainArea");
            chartArea.AxisX.Title = "Tháng";
            chartArea.AxisX.Interval = 1;
            chartArea.AxisY.Title = "Doanh Thu";
            chart3.ChartAreas.Add(chartArea);

            // Tùy chỉnh định dạng trục Y để trông giống mẫu (có thể cần chia cho 1 tỷ nếu giá trị lớn)
            chart3.ChartAreas["MainArea"].AxisY.LabelStyle.Format = "N0"; // Định dạng số không có thập phân
            chart3.ChartAreas["MainArea"].AxisY.LabelStyle.Font = new Font("Arial", 10, FontStyle.Regular);

            // 2. Lấy Năm và Chia Dữ Liệu
            // Sử dụng Linq để lấy năm nhỏ nhất (Năm cũ) và năm lớn nhất (Năm mới)
            int nam1 = dtDoanhThu.AsEnumerable().Min(row => row.Field<int>("Nam"));
            int nam2 = dtDoanhThu.AsEnumerable().Max(row => row.Field<int>("Nam"));
            tongtien = dtDoanhThu.AsEnumerable()
           .Where(row => row.Field<int>("Nam") == nam2)
           .Sum(row => Convert.ToDecimal(row["TongDoanhThu"]));

            // 3. Tạo Series cho Từng Năm (Giống màu trong mẫu)

            // Series 1: Năm Cũ (Màu Vàng - Yellow)
            Series seriesNam1 = new Series(nam1.ToString())
            {
                ChartArea = "MainArea",
                ChartType = SeriesChartType.Column,
                IsValueShownAsLabel = false,
                Color = Color.Gold, // Màu vàng
                LegendText = nam1.ToString(),
                Legend = "LegendDoanhThu"
            };

            // Series 2: Năm Mới (Màu Cam - Orange)
            Series seriesNam2 = new Series(nam2.ToString())
            {
                ChartArea = "MainArea",
                ChartType = SeriesChartType.Column,
                IsValueShownAsLabel = false,
                Color = Color.Orange, // Màu cam
                LegendText = nam2.ToString(),
                Legend = "LegendDoanhThu"
            };


            for (int month = 1; month <= 12; month++)
            {

                DataRow[] rowsNam1 = dtDoanhThu.Select($"Nam = {nam1} AND Thang = {month}");
                Decimal dtNam1 = rowsNam1.Length > 0 ? Convert.ToDecimal(rowsNam1[0]["TongDoanhThu"]) : 0m;

                DataRow[] rowsNam2 = dtDoanhThu.Select($"Nam = {nam2} AND Thang = {month}");
                Decimal dtNam2 = rowsNam2.Length > 0 ? Convert.ToDecimal(rowsNam2[0]["TongDoanhThu"]) : 0m;

                string XLabel = $"Tháng {month}";
                seriesNam1.Points.AddXY(XLabel, dtNam1);
                seriesNam2.Points.AddXY(XLabel, dtNam2);
            }


            chart3.Series.Add(seriesNam1);
            chart3.Series.Add(seriesNam2);


            chart3.Legends.Clear();
            Legend legend = new Legend("LegendDoanhThu")
            {
                Docking = Docking.Bottom,
                Alignment = StringAlignment.Center,
                Font = new Font("Arial", 10, FontStyle.Regular)
            };
            chart3.Legends.Add(legend);

            chart3.BringToFront();
            chart3.Invalidate();
            label21.Text = string.Format(cultureInfoVN, "{0:N0}", tongtien) + " VNĐ";
        }
        public void LoadPieChartForMonthlyOrders(Chart chartControl, DataTable dataTable)
        {

            chartControl.Series.Clear();

            chartControl.Legends.Clear();
            ChartArea chartArea = chartControl.ChartAreas[0];
            chartArea.Position = new ElementPosition(0, 10, 60, 80); // X=0, Y=20, Width=50%, Height=75%
            chartArea.BackColor = Color.Transparent;
            Legend customLegend = new Legend("CustomLegend")
            {
                Docking = Docking.Right, // Đặt chú thích bên phải
                Alignment = StringAlignment.Near,
                // Điều chỉnh vị trí thủ công (X=50, Y=20, Width=45%, Height=75%)
                Position = new ElementPosition(60, 10, 40, 80),
                LegendStyle = LegendStyle.Table,
                IsEquallySpacedItems = true, // Căn chỉnh các mục cho đều nhau
                TextWrapThreshold = 15
            };
            chartControl.Legends.Add(customLegend);


            Series series = new Series("MonthlyOrders")
            {
                ChartType = SeriesChartType.Pie,

                LegendText = "#VALX",
                IsValueShownAsLabel = true,

                Label = "#PERCENT"
            };


            DataView dv = dataTable.DefaultView;
            bool hasData = false;

            foreach (DataRowView row in dv)
            {
                // Đảm bảo cột có tồn tại và giá trị là số
                if (row["SoLuongDonHang"] != DBNull.Value && Convert.ToInt32(row["SoLuongDonHang"]) > 0)
                {
                    int thang = Convert.ToInt32(row["Thang"]);
                    int soLuong = Convert.ToInt32(row["SoLuongDonHang"]);


                    int pointIndex = series.Points.AddY(soLuong);

                    DataPoint point = series.Points[pointIndex];

                    // Thiết lập nhãn X (tên danh mục)
                    point.AxisLabel = thang.ToString();

                    point.LegendText = $"Tháng {thang} ({soLuong} đơn)";

                    point.Label = $"{point.AxisLabel} - #PERCENT";
                    hasData = true;
                }
            }

            // Nếu không có dữ liệu > 0, dừng lại
            if (!hasData)
            {
                // Có thể thêm một điểm dữ liệu "Không có dữ liệu" nếu cần
                chartControl.Titles.Add("Không có dữ liệu đơn hàng trong năm này.");
                return;
            }

            // 4. Tùy chỉnh hiển thị
            series["PieLabelStyle"] = "Outside";
            series["PieStartAngle"] = "270";

            // 5. Thêm Series vào Chart Control
            chartControl.Series.Add(series);
        }

        private void SetupCbbTieuChiNV()
        {
            if (cbbTieuChiNV == null) return;

            cbbTieuChiNV.Items.Clear();

            cbbTieuChiNV.Items.Add("Chức vụ");
            cbbTieuChiNV.Items.Add("Giới tính");

            if (cbbTieuChiNV.Items.Count > 0)
            {
                cbbTieuChiNV.SelectedIndex = 0;
            }
        }
        private void dtpBaoCao_ValueChanged(object sender, EventArgs e)
        {
            LoadTatCaBaoCao();
        }
        private void cbbTieuChiNV_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadTatCaBaoCao();
        }
        private void btnTaiLaiBaoCao_Click(object sender, EventArgs e)
        {
            LoadTatCaBaoCao();
        }

        private void tabControlTrangChu_Selected(object sender, TabControlEventArgs e)
        {
            if (tabControlTrangChu.SelectedTab == tabPageDoanhThu)
            {
                VeBieuDoDoanhThuNam(QuanLyBanXe.GetDoanhThuNamHienTai());
                LoadPieChartForMonthlyOrders(chart2, QuanLyBanXe.LayBaoCaoSoDonHangTheoThang(DateTime.Now.Year));
                label22.Text = QuanLyBanXe.LayTongSoDonHangTheoNam(DateTime.Now.Year).ToString("N0");
            }
            
            else if (tabControlTrangChu.SelectedTab == tabPageXeBanChay)
            {
                //LoadTatCaBaoCao();
            }
            else if (tabControlTrangChu.SelectedTab == tabPageXeDSThap)
            {
                //LoadTatCaBaoCao();
            }
        }

        private void loadBanchay_Click(object sender, EventArgs e)
        {
            try
            {
                guna2ComboBox19.SelectedIndex = -1;
                DataTable data = QuanLyBanXe.GetDanhSachBanChay(guna2DateTimePicker8.Value, guna2DateTimePicker7.Value,null);

                // 2. Gán DataTable làm nguồn dữ liệu (AutoGenerateColumns = true)
                dgvDMXBC.DataSource = data;
                if (dgvDMXBC.Columns.Count >= 3)
                {
                    // Tên cột trong C# phải khớp với tên cột trả về từ SQL (STT, TenSanPham, TongSoLuongBan)

                    // Cột 1: Số Thứ Tự
                    dgvDMXBC.Columns["STT"].HeaderText = "STT";
                    dgvDMXBC.Columns["STT"].Width = 50; // Đặt độ rộng cố định

                    // Cột 2: Tên Sản Phẩm (có Hãng và Phiên bản)
                    dgvDMXBC.Columns["TenSanPham"].HeaderText = "Tên Sản Phẩm";
                    dgvDMXBC.Columns["TenSanPham"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; // Tự động lấp đầy không gian

                    // Cột 3: Số Lượng Bán
                    dgvDMXBC.Columns["TongSoLuongBan"].HeaderText = "Số Lượng Bán";
                    dgvDMXBC.Columns["TongSoLuongBan"].Width = 300;

                    // Căn lề số lượng
                    dgvDMXBC.Columns["TongSoLuongBan"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                // Tùy chọn: Ngăn người dùng chỉnh sửa dữ liệu thống kê
                dgvDMXBC.ReadOnly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);


            }
        }

        private void guna2ComboBox19_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string type;
                if (guna2ComboBox19.SelectedIndex == 0)
                {
                    type = "TayGa";
                }
                else if (guna2ComboBox19.SelectedIndex == 1)
                {
                    type = "XeSo";
                }
                else if (guna2ComboBox19.SelectedIndex == 2)
                {
                    type = "ConTay";
                }
                else if (guna2ComboBox19.SelectedIndex == 3)
                {
                    type = "Dien";
                }
                else type = null;
                DataTable data = QuanLyBanXe.GetDanhSachBanChay(guna2DateTimePicker8.Value, guna2DateTimePicker7.Value, type);

                // 2. Gán DataTable làm nguồn dữ liệu (AutoGenerateColumns = true)
                dgvDMXBC.DataSource = data;
                if (dgvDMXBC.Columns.Count >= 3)
                {
                    // Tên cột trong C# phải khớp với tên cột trả về từ SQL (STT, TenSanPham, TongSoLuongBan)

                    // Cột 1: Số Thứ Tự
                    dgvDMXBC.Columns["STT"].HeaderText = "STT";
                    dgvDMXBC.Columns["STT"].Width = 50; // Đặt độ rộng cố định

                    // Cột 2: Tên Sản Phẩm (có Hãng và Phiên bản)
                    dgvDMXBC.Columns["TenSanPham"].HeaderText = "Tên Sản Phẩm";
                    dgvDMXBC.Columns["TenSanPham"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; // Tự động lấp đầy không gian

                    // Cột 3: Số Lượng Bán
                    dgvDMXBC.Columns["TongSoLuongBan"].HeaderText = "Số Lượng Bán";
                    dgvDMXBC.Columns["TongSoLuongBan"].Width = 300;

                    // Căn lề số lượng
                    dgvDMXBC.Columns["TongSoLuongBan"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                // Tùy chọn: Ngăn người dùng chỉnh sửa dữ liệu thống kê
                dgvDMXBC.ReadOnly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);


            }
        }

        private void loadBancham_Click(object sender, EventArgs e)
        {
            try
            {
                guna2ComboBox20.SelectedIndex = -1;
                DataTable data = QuanLyBanXe.GetDanhSachBanCham(null);

                // 2. Gán DataTable làm nguồn dữ liệu (AutoGenerateColumns = true)
                dgvBancham.DataSource = data;
                if (dgvBancham.Columns.Count >= 3)
                {
                    // Tên cột trong C# phải khớp với tên cột trả về từ SQL (STT, TenSanPham, TongSoLuongBan)

                    // Cột 1: Số Thứ Tự
                    dgvBancham.Columns["STT"].HeaderText = "STT";
                    dgvBancham.Columns["STT"].Width = 50; // Đặt độ rộng cố định

                    // Cột 2: Tên Sản Phẩm (có Hãng và Phiên bản)
                    dgvBancham.Columns["TenSanPham"].HeaderText = "Tên Sản Phẩm";
                    dgvBancham.Columns["TenSanPham"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; // Tự động lấp đầy không gian

                    // Cột 3: Số Lượng Bán
                    dgvBancham.Columns["TongSoLuongBan"].HeaderText = "Số Lượng Bán";
                    dgvBancham.Columns["TongSoLuongBan"].Width = 300;

                    // Căn lề số lượng
                    dgvBancham.Columns["TongSoLuongBan"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                // Tùy chọn: Ngăn người dùng chỉnh sửa dữ liệu thống kê
                dgvBancham.ReadOnly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);


            }
        }

        private void guna2ComboBox20_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string type;
                if (guna2ComboBox20.SelectedIndex == 0)
                {
                    type = "TayGa";
                }
                else if (guna2ComboBox20.SelectedIndex == 1)
                {
                    type = "XeSo";
                }
                else if (guna2ComboBox20.SelectedIndex == 2)
                {
                    type = "ConTay";
                }
                else if (guna2ComboBox20.SelectedIndex == 3)
                {
                    type = "Dien";
                }
                else type = null;
                DataTable data = QuanLyBanXe.GetDanhSachBanCham(type);

                // 2. Gán DataTable làm nguồn dữ liệu (AutoGenerateColumns = true)
                dgvBancham.DataSource = data;
                if (dgvBancham.Columns.Count >= 3)
                {
                    // Tên cột trong C# phải khớp với tên cột trả về từ SQL (STT, TenSanPham, TongSoLuongBan)

                    // Cột 1: Số Thứ Tự
                    dgvBancham.Columns["STT"].HeaderText = "STT";
                    dgvBancham.Columns["STT"].Width = 50; // Đặt độ rộng cố định

                    // Cột 2: Tên Sản Phẩm (có Hãng và Phiên bản)
                    dgvBancham.Columns["TenSanPham"].HeaderText = "Tên Sản Phẩm";
                    dgvBancham.Columns["TenSanPham"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; // Tự động lấp đầy không gian

                    // Cột 3: Số Lượng Bán
                    dgvBancham.Columns["TongSoLuongBan"].HeaderText = "Số Lượng Bán";
                    dgvBancham.Columns["TongSoLuongBan"].Width = 300;

                    // Căn lề số lượng
                    dgvBancham.Columns["TongSoLuongBan"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                // Tùy chọn: Ngăn người dùng chỉnh sửa dữ liệu thống kê
                dgvBancham.ReadOnly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);


            }
        }
        public void LoadDanhsachUudai(DataGridView dgv, DataTable ds)
        {
            if (dgv == null || ds == null)
            {
                
                MessageBox.Show("Lỗi: DataGridView hoặc DataTable không được khởi tạo.", "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // 1. Ngắt liên kết nguồn dữ liệu hiện tại (nếu có)
                dgv.DataSource = null;

                // 2. Gán DataTable làm nguồn dữ liệu
                dgv.DataSource = ds;

                // Tùy chọn: Tinh chỉnh giao diện DataGridView sau khi load
                // Ví dụ: Đặt lại tiêu đề cột
                dgv.Columns["MaUuDai"].HeaderText = "Mã Ưu Đãi"; 
                dgv.Columns["TenUuDai"].HeaderText = "Tên Chương Trình";
                dgv.Columns["GiaTriGiam"].HeaderText = "Giá trị";
                dgv.Columns["DieuKienToiThieu"].HeaderText = "Đơn Tối Thiểu";
                dgv.Columns["LoaiUuDai"].HeaderText = "Loại";
                dgv.Columns["NgayBatDau"].HeaderText = "Từ";
                dgv.Columns["NgayKetThuc"].HeaderText = "Đến";
                dgv.Columns["TrangThaiHieuLuc"].HeaderText = "Trạng thái";
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi load dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTimuudai_Click(object sender, EventArgs e)
        {
            string status;
            if(radioAll.Checked)
            {
                status = "TatCa";
            }
            else if (radioOn.Checked)
            {
                status = "KichHoat";
            }
            else if(radioOff.Checked)
            {
                status = "NgungKichHoat";
            }
            else
            {
                status = null;
            }
            LoadDanhsachUudai(dgvDsUudai, QuanLyBanXe.GetDanhSachUuDaiCombined(txtTimUudai.Text,status));
        }

        private void radioAll_CheckedChanged(object sender, EventArgs e)
        {
            LoadDanhsachUudai(dgvDsUudai, QuanLyBanXe.GetDanhSachUuDaiCombined(txtTimUudai.Text, "TatCa"));
        }

        private void radioOn_CheckedChanged(object sender, EventArgs e)
        {
            LoadDanhsachUudai(dgvDsUudai, QuanLyBanXe.GetDanhSachUuDaiCombined(txtTimUudai.Text, "KichHoat"));
        }

        private void radioOff_CheckedChanged(object sender, EventArgs e)
        {
            LoadDanhsachUudai(dgvDsUudai, QuanLyBanXe.GetDanhSachUuDaiCombined(txtTimUudai.Text, "NgungKichHoat"));
        }

        private void btnALLUudai_Click(object sender, EventArgs e)
        {
            txtTimUudai.Text = "";
            radioAll.Checked = true;
            LoadDanhsachUudai(dgvDsUudai, QuanLyBanXe.GetDanhSachUuDaiCombined(null, "TatCa"));
        }

        private void btnThemUudai_Click(object sender, EventArgs e)
        {
            ThemKhuyenMai themKhuyenMai = new ThemKhuyenMai();
            themKhuyenMai.Show();
            themKhuyenMai.FormClosed += (s, args) =>
            {
                // Ép kiểu đối tượng gửi sự kiện (s) về lại Form ThemKhuyenMai
                ThemKhuyenMai closedForm = s as ThemKhuyenMai;

                // Kiểm tra biến cờ DataChanged
                if (closedForm != null && closedForm.DataChanged)
                {
                    // Chỉ tải lại dữ liệu nếu DataChanged là TRUE
                    LoadDanhsachUudai(
                        dgvDsUudai,
                        QuanLyBanXe.GetDanhSachUuDaiCombined(null, "TatCa")
                    );
                    LoadActiveUuDaiToComboBox();
                }
                
            };
            

        }
        
        string selectedMaUuDai = null;
        private void dgvDsUudai_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (_role == "RoleUser")
                return;
            btnSuaUD.Visible = true;
            btnXoaUD.Visible = true;
            selectedMaUuDai = dgvDsUudai.Rows[e.RowIndex].Cells["MaUuDai"].Value.ToString();
        }

        private void btnSuaUD_Click(object sender, EventArgs e)
        {
            ThemKhuyenMai themKhuyenMai = new ThemKhuyenMai(selectedMaUuDai);
            themKhuyenMai.Show();
            themKhuyenMai.FormClosed += (s, args) =>
            {
                // Ép kiểu đối tượng gửi sự kiện (s) về lại Form ThemKhuyenMai
                ThemKhuyenMai closedForm = s as ThemKhuyenMai;

                // Kiểm tra biến cờ DataChanged
                if (closedForm != null && closedForm.DataChanged)
                {
                    // Chỉ tải lại dữ liệu nếu DataChanged là TRUE
                    LoadDanhsachUudai(
                        dgvDsUudai,
                        QuanLyBanXe.GetDanhSachUuDaiCombined(null, "TatCa")
                    );
                }

            };
            btnSuaUD.Visible = false;
            btnXoaUD.Visible = false;
            LoadActiveUuDaiToComboBox();

        }

        private void btnXoaUD_Click(object sender, EventArgs e)
        {
            QuanLyBanXe.DeleteUuDai(selectedMaUuDai);
            btnSuaUD.Visible = false;
            btnXoaUD.Visible = false;
            DataTable currentDataSource = dgvDsUudai.DataSource as DataTable;

            if (currentDataSource != null)
            {
                DataRow[] rowsToDelete = currentDataSource.Select($"MaUuDai = '{selectedMaUuDai}'");

                if (rowsToDelete.Length > 0)
                {
                    rowsToDelete[0].Delete(); 
                    currentDataSource.AcceptChanges();
                }
            }
            LoadActiveUuDaiToComboBox();

        }
        private void KhoiTaoChiTietDonHang()
        {
            dtChiTietDonHang = new DataTable();

            dtChiTietDonHang.Columns.Add("TenXe", typeof(string));
            dtChiTietDonHang.Columns.Add("Hangxe", typeof(string));
            dtChiTietDonHang.Columns.Add("Loaixe", typeof(string));
            dtChiTietDonHang.Columns.Add("Mau", typeof(string));
            dtChiTietDonHang.Columns.Add("Namsx", typeof(string));
            dtChiTietDonHang.Columns.Add("Tinhtrang", typeof(string));

            // Cột dữ liệu chính/chi tiết (SK, SM dùng để kiểm tra và là mã chính):
            dtChiTietDonHang.Columns.Add("SK", typeof(string));         // Số Khung
            dtChiTietDonHang.Columns.Add("SM", typeof(string));         // Số Máy

            // Cột giá trị và tính toán:
            dtChiTietDonHang.Columns.Add("Gia", typeof(decimal));       // Đơn giá

            dgvDSSP.DataSource = dtChiTietDonHang;
            // Thiết lập Header Text
            dgvDSSP.Columns["TenXe"].HeaderText = "Tên xe";
            dgvDSSP.Columns["Hangxe"].HeaderText = "Hãng xe";
            dgvDSSP.Columns["Loaixe"].HeaderText = "Loại xe";
            dgvDSSP.Columns["Mau"].HeaderText = "Màu sắc";
            dgvDSSP.Columns["Namsx"].HeaderText = "Năm SX";
            dgvDSSP.Columns["Tinhtrang"].HeaderText = "Tình trạng";

            dgvDSSP.Columns["SK"].HeaderText = "Số Khung";
            dgvDSSP.Columns["SM"].HeaderText = "Số Máy";

            
            dgvDSSP.Columns["Gia"].HeaderText = "Đơn giá";

            dgvDSSP.Columns["Gia"].DefaultCellStyle.Format = "N0";


            DataGridViewButtonColumn btnXoa = new DataGridViewButtonColumn();
            btnXoa.Name = "ColXoa"; // Đặt tên cho cột để dễ dàng tham chiếu
            btnXoa.Text = "Xóa";
            btnXoa.HeaderText = "";
            btnXoa.UseColumnTextForButtonValue = true;

            // Thêm cột nút vào lưới chi tiết đơn hàng
            dgvDSSP.Columns.Add(btnXoa);

            // Bổ sung: Gán sự kiện xử lý click vào nút
            dgvDSSP.CellContentClick += dgvDSSP_CellContentClick;

        }

        private void dgvTonKhoChiTiet_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvTonKhoChiTiet.CurrentRow == null) return;

            // Sử dụng biến cấp Form dtChiTietDonHang trực tiếp
            DataTable dtDonHang = dtChiTietDonHang;

            // Kiểm tra tính hợp lệ của DataSource
            if (dtDonHang == null)
            {
                MessageBox.Show("Lỗi: Nguồn dữ liệu đơn hàng chưa được khởi tạo.", "Lỗi Hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                DataGridViewRow selectedRow = dgvTonKhoChiTiet.CurrentRow;

                // 1. LẤY TẤT CẢ DỮ LIỆU CẦN THIẾT TRƯỚC HỘP THOẠI
                string soKhung = selectedRow.Cells["SoKhung"].Value.ToString();
                string soMay = selectedRow.Cells["SoMay"].Value.ToString();
                string tenXe = selectedRow.Cells["TenXe"].Value.ToString();
                string hangXe = selectedRow.Cells["Hangxe"].Value.ToString();
                string loaiXe = selectedRow.Cells["Loaixe"].Value.ToString();
                string mauXe = selectedRow.Cells["MauSac"].Value.ToString();
                string namSx = selectedRow.Cells["NamSX"].Value.ToString();
                string tinhTrang = selectedRow.Cells["Tinhtrang"].Value.ToString();
                decimal donGia = Convert.ToDecimal(selectedRow.Cells["DonGia"].Value);

                // 2. HIỂN THỊ HỘP THOẠI XÁC NHẬN
                string message = $"Bạn có muốn thêm xe '{tenXe}' vào đơn hàng không?";
                string caption = "Xác nhận Sản phẩm";

                DialogResult result = MessageBox.Show(message, caption,
                                                     MessageBoxButtons.YesNo,
                                                     MessageBoxIcon.Question);

                // 3. XỬ LÝ KHI NGƯỜI DÙNG CHỌN YES (THÊM SẢN PHẨM)
                if (result == DialogResult.Yes)
                {
                    // --- Logic đã sửa nằm ở đây ---

                    // Kiểm tra tính duy nhất (SK và SM)
                    string filterExpression = $"SK = '{soKhung}' OR SM = '{soMay}'";
                    DataRow[] existingRows = dtDonHang.Select(filterExpression);

                    if (existingRows.Length > 0)
                    {
                        MessageBox.Show($"Xe '{tenXe}' đã được thêm vào chi tiết đơn hàng. Không thể thêm cùng một chiếc xe hai lần.", "Lỗi Trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        // THÊM DÒNG MỚI VÀO DATATABLE
                        DataRow newRow = dtDonHang.NewRow();

                        // ÁNH XẠ DỮ LIỆU
                        newRow["TenXe"] = tenXe;
                        newRow["Hangxe"] = hangXe;
                        newRow["Loaixe"] = loaiXe;
                        newRow["Mau"] = mauXe;
                        newRow["Namsx"] = namSx;
                        newRow["Tinhtrang"] = tinhTrang;
                        newRow["SK"] = soKhung;
                        newRow["SM"] = soMay;

                        // Gán giá trị và tính toán
                        newRow["Gia"] = donGia;
                        
                        

                        dtDonHang.Rows.Add(newRow);
                        MessageBox.Show($"Đã thêm xe '{tenXe}' vào đơn hàng.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    // Đồng bộ DataGridView với DataTable
                    dtDonHang.AcceptChanges();
                    CapNhatTongKetDonHang();
                }
                else if (result == DialogResult.No) { return; }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi trong quá trình lấy giá trị hoặc xử lý logic
                MessageBox.Show("Lỗi khi thêm sản phẩm vào đơn hàng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void dgvDSSP_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.ColumnIndex == dgvDSSP.Columns["ColXoa"].Index && e.RowIndex >= 0)
            {
                // QUAN TRỌNG: Nếu click vào dòng trống/dòng mới -> Dừng ngay
                if (dgvDSSP.Rows[e.RowIndex].IsNewRow)
                {
                    return; // Dừng, không làm gì nếu không phải dòng dữ liệu
                }

                // --- LOGIC XÓA (CHỈ CHẠY TRÊN DÒNG CÓ DỮ LIỆU) ---

                try
                {
                    DataRowView drv = (DataRowView)dgvDSSP.Rows[e.RowIndex].DataBoundItem;
                    string tenXeCanXoa = drv["TenXe"]?.ToString() ?? "sản phẩm này";

                    // 2. Xác nhận xóa
                    DialogResult result = MessageBox.Show(
                        $"Bạn có chắc chắn muốn xóa xe '{tenXeCanXoa}' khỏi đơn hàng không?",
                        "Xác nhận Xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        // 3. Xóa dòng khỏi DataTable
                        drv.Row.Delete();
                        dtChiTietDonHang.AcceptChanges();

                        MessageBox.Show("Đã xóa sản phẩm khỏi đơn hàng.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // CapNhatTongTienDonHang(); 
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvDSSP_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvDSSP.Columns["ColXoa"].Index)
            {
                // 2. Nếu là dòng mới (chưa có dữ liệu) -> KHÔNG VẼ NÚT
                if (dgvDSSP.Rows[e.RowIndex].IsNewRow)
                {
                    e.PaintBackground(e.CellBounds, true);
                    e.Handled = true; // Ngăn vẽ nút mặc định
                    return;
                }

                // 3. Nếu không phải dòng mới -> VẼ NÚT MÀU ĐỎ

                // Vô hiệu hóa việc vẽ mặc định của WinForms
                e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentBackground);

                // Thiết lập vùng vẽ nút (cách lề 3 pixel)
                Rectangle buttonBounds = new Rectangle(
                    e.CellBounds.X + 3,
                    e.CellBounds.Y + 3,
                    e.CellBounds.Width - 6,
                    e.CellBounds.Height - 6
                );

                

                // Vẽ lại nút với màu nền ĐỎ
                ControlPaint.DrawButton(e.Graphics, buttonBounds, ButtonState.Normal);

                // Vẽ chữ "Xóa" màu trắng
                TextRenderer.DrawText(e.Graphics,
                                      "Xóa",
                                      e.CellStyle.Font,
                                      buttonBounds,
                                      Color.Red,
                                      TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                e.Handled = true; // Đánh dấu sự kiện đã được xử lý
            }
        }
        private void CapNhatTongKetDonHang()
        {
            // Khởi tạo tổng tiền và tổng số lượng
            decimal tongTien = 0;
            int tongSoLuong = 0;

            // Kiểm tra DataTable có hợp lệ không
            if (dtChiTietDonHang == null) return;

            foreach (DataRow row in dtChiTietDonHang.Rows)
            {
                try
                {
                    // Tính Tổng Tiền
                    if (row["Gia"] != DBNull.Value && row["Gia"] != null)
                    {
                        tongTien += Convert.ToDecimal(row["Gia"]);
                    }
                    tongSoLuong++; 
                    
                }
                catch (Exception ex)
                {
                    // Bỏ qua dòng bị lỗi định dạng và có thể ghi log
                    Console.WriteLine("Lỗi khi tính tổng: " + ex.Message);
                }
            }


            txtTongTienDH.Text = tongTien.ToString("N0") + " VNĐ"; // Định dạng số có dấu phẩy
            txtTongSoLuongDH.Text = tongSoLuong.ToString();

            
        }
        private void LoadActiveUuDaiToComboBox()
        {
            // Giả định ComboBox của bạn có tên là 'cboMaUD'
            try
            {
                // 1. Lấy dữ liệu từ Service
                DataTable dtActiveUuDai = QuanLyBanXe.GetActiveUuDaiCodes();

                // 2. Thêm dòng mặc định "Không áp dụng" (Rất quan trọng)
                DataRow defaultRow = dtActiveUuDai.NewRow();
                defaultRow["MaUuDai"] = string.Empty; // Mã rỗng

                // Cần đảm bảo DataTable có cột TenUuDai nếu bạn muốn dùng dòng này
                // Nếu không, bạn cần thay thế bằng một cột đã tồn tại trong DataTable (ví dụ: MaUuDai)
                // Nếu SP chỉ trả về MaUuDai, bạn phải thêm cột TenUuDai vào DataTable trước khi thêm dòng này
                // (Chúng ta sẽ bỏ qua TenUuDai cho đơn giản)

                // Chỉ thêm giá trị nếu có cột MaUuDai
                if (dtActiveUuDai.Columns.Contains("MaUuDai"))
                {
                    defaultRow["MaUuDai"] = "";
                }

                dtActiveUuDai.Rows.InsertAt(defaultRow, 0);

                // 3. Binding dữ liệu
                cbUD.DataSource = dtActiveUuDai;

                // Đặt cả hai thuộc tính là "MaUuDai"
                cbUD.DisplayMember = "MaUuDai"; // Hiển thị MaUuDai
                cbUD.ValueMember = "MaUuDai";   // Giá trị được lấy

                // 4. Chọn dòng mặc định (dòng đầu tiên)
                cbUD.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách ưu đãi: " + ex.Message, "Lỗi Tải Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTaoDonHang_Click(object sender, EventArgs e)
        {
            if (dtChiTietDonHang.Rows.Count == 0)
            {
                MessageBox.Show("Vui lòng thêm sản phẩm vào đơn hàng trước khi tạo đơn.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            guna2TabControl2.TabPages.Add(tabPageXuLyDH);
            guna2TabControl2.SelectedTab = tabPageXuLyDH;
            lblTongtien.Text = "Tồng tiền (" + txtTongSoLuongDH.Text + ") sản phẩm";
            lblGiatri.Text = txtTongTienDH.Text;
            lblTongcong.Text = txtTongTienDH.Text;
            btnTaoDonHang.Enabled = false;
        }

        private void cbUD_SelectedIndexChanged(object sender, EventArgs e)
        {
            decimal tongTienTruocGiam;
            decimal tienGiam = 0;
            string maUuDaiText = cbUD.SelectedValue?.ToString() ?? string.Empty;

            // 1. Lấy Tổng tiền trước giảm giá (BẮT BUỘC)
            string cleanTongTien = txtTongTienDH.Text.Replace(" VNĐ", "").Replace(",", "");
            if (!decimal.TryParse(cleanTongTien, out tongTienTruocGiam))
            {
                // Nếu tổng tiền không hợp lệ, không thể tính toán giảm giá
                // Chỉ cần đặt lại giao diện giảm giá và return
                CapNhatGiaoDienUuDai(0, tongTienTruocGiam);
                return;
            }

            // 2. CHỈ XỬ LÝ KHI CÓ MÃ ƯU ĐÃI ĐƯỢC CHỌN
            if (!string.IsNullOrEmpty(maUuDaiText))
            {
                try
                {
                    DataTable dtUuDai = QuanLyBanXe.GetUuDaiByMa(maUuDaiText);

                    if (dtUuDai == null || dtUuDai.Rows.Count == 0)
                    {
                        // Mã không tồn tại, đặt lại lựa chọn
                        MessageBox.Show($"Mã Ưu Đãi '{maUuDaiText}' không tồn tại trong hệ thống.", "Lỗi Ưu đãi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        cbUD.SelectedIndex = 0;
                        return;
                    }

                    DataRow row = dtUuDai.Rows[0];

                    // Lấy thông tin
                    string loaiUuDai = row["LoaiUuDai"].ToString().ToUpper();
                    decimal giaTriGiam = Convert.ToDecimal(row["GiaTriGiam"]);
                    DateTime from = Convert.ToDateTime(row["NgayBatDau"]);
                    DateTime to = Convert.ToDateTime(row["NgayKetThuc"]);

                    // 3. KIỂM TRA HIỆU LỰC
                    if (DateTime.Today >= from.Date && DateTime.Today <= to.Date)
                    {
                        // Tính toán tiền giảm
                        if (loaiUuDai == "PHANTRAM")
                        {
                            tienGiam = tongTienTruocGiam * (giaTriGiam / 100);
                        }
                        else if (loaiUuDai == "TRUCTIEP")
                        {
                            tienGiam = Math.Min(giaTriGiam, tongTienTruocGiam);
                        }
                    }
                    else
                    {
                        // Ưu đãi hết hạn
                        MessageBox.Show($"Ưu đãi '{maUuDaiText}' đã hết hạn sử dụng. Không thể áp dụng.", "Lỗi Ưu đãi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        cbUD.SelectedIndex = 0; // Đặt lại về không áp dụng
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xử lý ưu đãi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // 4. Cập nhật giao diện với số tiền giảm và tổng tiền mới
            CapNhatGiaoDienUuDai(tienGiam, tongTienTruocGiam);
        }
        private void CapNhatGiaoDienUuDai(decimal tienGiam, decimal tongTienTruocGiam)
        {
            decimal tongTienSauGiam = tongTienTruocGiam - tienGiam;

            // Ví dụ: Giả định bạn có lblTienGiam và lblTongTienCuoiCung
            lblUudai.Text = tienGiam.ToString("N0") + " VNĐ";
            lblTongcong.Text = tongTienSauGiam.ToString("N0") + " VNĐ"; 
        }

        private void btnKiemTra_Click(object sender, EventArgs e)
        {
            string soDienThoai = txtSoDT_XLDH.Text.Trim();

            // 1. Kiểm tra đầu vào phía Client (Thêm lớp bảo vệ)
            if (string.IsNullOrEmpty(soDienThoai))
            {
                MessageBox.Show("Vui lòng nhập Số điện thoại để kiểm tra.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoDT_XLDH.Focus();
                return;
            }

            try
            {
                // Gọi hàm tìm kiếm, kết quả có thể là DataTable chứa 0 hoặc 1 dòng
                DataTable ketQuaDT = QuanLyBanXe.TimKhachHangBangSDT(soDienThoai);

                if (ketQuaDT != null && ketQuaDT.Rows.Count > 0)
                {
                    // Tìm thấy khách hàng
                    DataRow row = ketQuaDT.Rows[0];

                    // ⚠️ CHÚ Ý: Đảm bảo tên cột khớp với tên trong thủ tục SELECT!
                    // Tên cột trong SP là "HoTen" và "DiaChi", không phải "TenKH"
                    txtHoTen_XLDH.Text = row["HoTen"].ToString();
                    txtDiaChi_XLDH.Text = row["DiaChi"].ToString();
                    IdKH = Convert.ToInt32(row["KhachHangID"]);

                    MessageBox.Show("Tìm thấy khách hàng trong hệ thống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Khách hàng mới. Vui lòng nhập thông tin Khách hàng mới.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    txtHoTen_XLDH.Clear();
                    txtDiaChi_XLDH.Clear();
                    txtHoTen_XLDH.Focus();
                }
            }
            catch (System.Data.SqlClient.SqlException sqlEx)
            {
                MessageBox.Show($"Lỗi kiểm tra dữ liệu: {sqlEx.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi không xác định: {ex.Message}", "Lỗi Hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private DataTable TaoDataTableChiTietDonHang()
        {
            DataTable dt = new DataTable();
    
            // Cấu trúc cột phải khớp với ChiTietDonHangType trong SQL
            dt.Columns.Add("XeCTID", typeof(int));
            dt.Columns.Add("SoLuong", typeof(int));
            dt.Columns.Add("DonGia", typeof(decimal));
    
            return dt;
        }
        private async void btnXacNhanDH_Click(object sender, EventArgs e)
        {
            try
            {
                // Vô hiệu hóa nút và đổi con trỏ chuột ngay từ đầu
                btnTaoDonHang.Enabled = false;
                this.Cursor = Cursors.WaitCursor;

                // BƯỚC 1: THÊM KHÁCH HÀNG MỚI (NẾU CẦN)
                if (IdKH == 0)
                {
                    // Validation thông tin khách hàng trước khi gọi DB
                    if (string.IsNullOrWhiteSpace(txtHoTen_XLDH.Text) || string.IsNullOrWhiteSpace(txtSoDT_XLDH.Text))
                    {
                        MessageBox.Show("Vui lòng nhập đầy đủ Họ Tên và Số Điện Thoại cho khách hàng mới.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return; // Dừng lại ngay lập tức
                    }

                    var parameters = new Dictionary<string, object>
            {
                { "@HoTen", txtHoTen_XLDH.Text },
                { "@TenLoaiKH", "Khách mua xe" },
                { "@DiaChi", txtDiaChi_XLDH.Text },
                { "@SoDienThoai", txtSoDT_XLDH.Text },
            };

                    // Nên tạo một phiên bản async cho ExecuteScalar để không làm đơ UI
                    object result = await QuanLyBanXe.ExecuteScalarAsync("sp_InsertKhachHangMoi", parameters);

                    if (result != null && result != DBNull.Value)
                    {
                        IdKH = Convert.ToInt32(result);
                        MessageBox.Show($"Thêm khách hàng mới thành công! Mã KH: {IdKH}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // SỬA 2: Nếu không thêm được khách hàng, phải DỪNG lại
                        MessageBox.Show("Không thêm được khách hàng. Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // Rất quan trọng: Thoát khỏi phương thức
                    }
                }

                // BƯỚC 2: THU THẬP DỮ LIỆU ĐƠN HÀNG
                string phuongThuc = cbPTTT.SelectedItem.ToString();
                string maUuDai = cbUD.SelectedValue?.ToString() ?? string.Empty;

                DataTable dtChiTiet = new DataTable();
                dtChiTiet.Columns.Add("SoKhung", typeof(string));
                dtChiTiet.Columns.Add("SoMay", typeof(string));
                dtChiTiet.Columns.Add("SoLuong", typeof(int));
                dtChiTiet.Columns.Add("DonGia", typeof(decimal));

                DataTable dtHienThi = new DataTable();
                dtHienThi.Columns.Add("TenSanPham", typeof(string));
                dtHienThi.Columns.Add("SoKhung", typeof(string));
                dtHienThi.Columns.Add("SoMay", typeof(string));
                dtHienThi.Columns.Add("DonGia", typeof(decimal));

                foreach (DataGridViewRow row in dgvDSSP.Rows)
                {
                    if (row.IsNewRow) continue;
                    // ... (code thêm row vào dtChiTiet và dtHienThi giữ nguyên)
                    string tenXe = row.Cells["TenXe"].Value.ToString();
                    string soKhung = row.Cells["SK"].Value.ToString();
                    string soMay = row.Cells["SM"].Value.ToString();
                    decimal donGia = Convert.ToDecimal(row.Cells["Gia"].Value);
                    dtChiTiet.Rows.Add(soKhung, soMay, 1, donGia);
                    dtHienThi.Rows.Add(tenXe, soKhung, soMay, donGia);
                }

                if (dtChiTiet.Rows.Count == 0)
                {
                    MessageBox.Show("Vui lòng thêm xe vào đơn hàng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // BƯỚC 3: GỌI TẠO ĐƠN HÀNG
                int maDonHangMoi = await QuanLyBanXe.TaoDonHangMoiAsync(
                    IdKH, dtChiTiet, phuongThuc, null, maUuDai, null
                );

                // BƯỚC 4: HIỂN THỊ KẾT QUẢ
                MessageBox.Show($"Tạo đơn hàng thành công! Mã đơn hàng mới là: {maDonHangMoi}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // ... (phần code hiển thị thông tin lên tab chi tiết giữ nguyên)
                guna2TabControl2.TabPages.Add(tabPageChiTietHoaDon);
                guna2TabControl2.SelectedTab = tabPageChiTietHoaDon;
                guna2DataGridView4.DataSource = dtHienThi;
                guna2DataGridView4.Columns["TenSanPham"].HeaderText = "Tên Sản Phẩm";

                guna2DataGridView4.Columns["SoKhung"].HeaderText = "Số Khung";

                guna2DataGridView4.Columns["SoMay"].HeaderText = "Số Máy";

                guna2DataGridView4.Columns["DonGia"].HeaderText = "Đơn Giá";
                lbNgay.Text = DateTime.Now.ToString("dd/MM/yyyy");

                lbTenNV.Text = _username;

                lbTenKH.Text = txtHoTen_XLDH.Text;

                lbSoDT.Text = txtSoDT_XLDH.Text;

                lbDiaChi.Text = txtDiaChi_XLDH.Text;

                if (lblUudai.Text != "0 VNĐ")

                {

                    lbTru.Text = "-" + lblUudai.Text;

                }

                else

                    lbTru.Text = "";

                lbTongtien.Text = lblTongcong.Text;
            }
            catch (Exception ex) // Bắt TẤT CẢ các lỗi có thể xảy ra trong quá trình
            {
                MessageBox.Show(ex.Message, "Đã xảy ra lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally // Khối này LUÔN LUÔN được thực thi
            {
                btnTaoDonHang.Enabled = true;
                this.Cursor = Cursors.Default;
            }


        }

        private void btnHuyDH_Click(object sender, EventArgs e)
        {

            guna2TabControl2.TabPages.Remove(tabPageXuLyDH);
            guna2TabControl2.SelectedTab = tabPageDSSP;
            btnTaoDonHang.Enabled = true;
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            if (tabPageXuLyDH != null)
                guna2TabControl2.TabPages.Remove(tabPageXuLyDH);

            if (tabPageChiTietHoaDon != null)
                guna2TabControl2.TabPages.Remove(tabPageChiTietHoaDon);

            guna2TabControl2.SelectedTab = tabPageDSSP;

            if (dtChiTietDonHang != null)
            {
                dtChiTietDonHang.Rows.Clear();
            }
            
            else if (dgvDSSP.DataSource == null)
            {
                dgvDSSP.Rows.Clear();
            }
            
            IdKH = 0;
            txtDiaChi_XLDH.Clear();
            txtHoTen_XLDH.Clear();
            txtSoDT_XLDH.Clear();

            if (cbUD.Items.Count > 0)
            {
                cbUD.SelectedIndex = 0;
            }

            lblUudai.Text = "0 VNĐ";
            lblTongcong.Text = "0 VNĐ";
            txtTongSoLuongDH.Text = "0";
            txtTongTienDH.Text = "0 VNĐ";

        }

        private void cbbBarCriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadBarChart();
        }

        private void cbbPieCriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPieChart();
        }

        private void txtSDTNCC_KH_Leave(object sender, EventArgs e)
        {
            string loai = cbbNguonNhapChiTiet.SelectedItem?.ToString();
            string soDT = txtSDTNCC_KH.Text.Trim();

            if (string.IsNullOrEmpty(loai) || string.IsNullOrEmpty(soDT))
                return;

            var thongTin = quanLyNhapXe_Kho.TimThongTinTheoSDT(loai, soDT);

            if (thongTin.HasValue)
            {
                txtTenNCC_KH.Text = thongTin.Value.Ten;
                txtDiaChiNCC_KH.Text = thongTin.Value.DiaChi;
            }
            else
            {
                // Không tìm thấy → xóa cũ
                txtTenNCC_KH.Clear();
                txtDiaChiNCC_KH.Clear();
            }
        }

        private void txtSDTNCC_KHNX_Leave(object sender, EventArgs e)
        {
            string loai = cbbNguonNhapNX.SelectedItem?.ToString();
            string soDT = txtSDTNCC_KHNX.Text.Trim();

            if (string.IsNullOrEmpty(loai) || string.IsNullOrEmpty(soDT))
                return;

            var thongTin = quanLyNhapXe_Kho.TimThongTinTheoSDT(loai, soDT);

            if (thongTin.HasValue)
            {
                txtTenNCC_KHNX.Text = thongTin.Value.Ten;
                txtDiaChiNX.Text = thongTin.Value.DiaChi;
            }
            else
            {
                // Không tìm thấy → xóa cũ
                txtTenNCC_KHNX.Clear();
                txtDiaChiNX.Clear();
            }
        }
    }
}
