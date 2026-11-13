using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using LibraryManagement.BUS;
using LibraryManagement.DTO;

namespace LibraryManagement.GUI
{
    public class MainForm : Form
    {
        // ============ BUSINESS LAYER ============
        private readonly DocGiaBUS _docGiaBUS = new();
        private readonly DauSachBUS _dauSachBUS = new();
        private readonly PhieuMuonBUS _phieuMuonBUS = new();

        // ============ CONTROLS CHUNG ============
        private TabControl tabMain = null!;

        // ---- TAB 1: Độc giả ----
        private DataGridView dgvDocGia = null!;
        private TextBox txtMaDG = null!, txtTenDG = null!, txtDiaChi = null!, txtSDT = null!;
        private DateTimePicker dtpNgaySinh = null!, dtpNgayHetHan = null!;
        private ComboBox cboGioiTinh = null!;
        private Button btnThemDG = null!, btnSuaDG = null!, btnXoaDG = null!, btnLamMoiDG = null!;

        // ---- TAB 2: Đầu sách ----
        private DataGridView dgvDauSach = null!;
        private TextBox txtMaSach = null!, txtTenSach = null!, txtTacGia = null!, txtNXB = null!, txtTheLoai = null!;
        private NumericUpDown numNamXB = null!, numTong = null!, numCon = null!;
        private Button btnThemS = null!, btnSuaS = null!, btnXoaS = null!, btnLamMoiS = null!;

        // ---- TAB 3: Mượn / Trả ----
        private ComboBox cboDocGia = null!, cboSach = null!;
        private NumericUpDown numSoLuong = null!;
        private DateTimePicker dtpNgayMuon = null!, dtpHanTra = null!;
        private Button btnLapPhieu = null!;
        private DataGridView dgvPhieu = null!;
        private Button btnTraSach = null!, btnLoadPhieu = null!;

        // ---- TAB 4: Quá hạn + Thống kê ----
        private DataGridView dgvQuaHan = null!, dgvThongKe = null!;
        private Button btnLoadQuaHan = null!, btnThongKe = null!;

        // ============ CONSTRUCTOR ============
        public MainForm()
        {
            InitializeComponent();
            LoadAllData();
        }

        // ============ KHỞI TẠO GIAO DIỆN ============
        private void InitializeComponent()
        {
            Text = "Hệ thống Quản lý Mượn trả Sách Thư viện";
            Size = new Size(1150, 780);
            StartPosition = FormStartPosition.CenterScreen;

            tabMain = new TabControl { Dock = DockStyle.Fill };
            tabMain.TabPages.Add(BuildTabDocGia());
            tabMain.TabPages.Add(BuildTabDauSach());
            tabMain.TabPages.Add(BuildTabMuonTra());
            tabMain.TabPages.Add(BuildTabCanhBao());

            Controls.Add(tabMain);
        }

        // ------------------------------------------------
        // TAB 1: QUẢN LÝ ĐỘC GIẢ
        // ------------------------------------------------
        private TabPage BuildTabDocGia()
        {
            var tab = new TabPage("Quản lý Độc giả") { Padding = new Padding(10) };

            dgvDocGia = new DataGridView
            {
                Location = new Point(10, 10),
                Size = new Size(1100, 320),
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false
            };
            dgvDocGia.CellClick += DgvDocGia_CellClick;
            tab.Controls.Add(dgvDocGia);

            int y = 350, xLbl = 20, xTxt = 130;
            tab.Controls.Add(MakeLabel("Mã độc giả:", xLbl, y));
            txtMaDG = MakeTextBox(xTxt, y); tab.Controls.Add(txtMaDG);

            tab.Controls.Add(MakeLabel("Họ tên:", xLbl, y + 35));
            txtTenDG = MakeTextBox(xTxt, y + 35); tab.Controls.Add(txtTenDG);

            tab.Controls.Add(MakeLabel("Ngày sinh:", xLbl, y + 70));
            dtpNgaySinh = new DateTimePicker { Location = new Point(xTxt, y + 70), Width = 200, Format = DateTimePickerFormat.Short };
            tab.Controls.Add(dtpNgaySinh);

            tab.Controls.Add(MakeLabel("Giới tính:", xLbl, y + 105));
            cboGioiTinh = new ComboBox { Location = new Point(xTxt, y + 105), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cboGioiTinh.Items.AddRange(new object[] { "Nam", "Nữ", "Khác" });
            cboGioiTinh.SelectedIndex = 0;
            tab.Controls.Add(cboGioiTinh);

            tab.Controls.Add(MakeLabel("Địa chỉ:", xLbl, y + 140));
            txtDiaChi = MakeTextBox(xTxt, y + 140); tab.Controls.Add(txtDiaChi);

            int x2 = 450;
            tab.Controls.Add(MakeLabel("Số ĐT:", x2, y));
            txtSDT = MakeTextBox(x2 + 90, y); tab.Controls.Add(txtSDT);

            tab.Controls.Add(MakeLabel("Ngày hết hạn thẻ:", x2, y + 35));
            dtpNgayHetHan = new DateTimePicker { Location = new Point(x2 + 140, y + 35), Width = 200, Format = DateTimePickerFormat.Short };
            tab.Controls.Add(dtpNgayHetHan);

            btnThemDG = MakeButton("Thêm", x2, y + 100); btnThemDG.Click += BtnThemDG_Click; tab.Controls.Add(btnThemDG);
            btnSuaDG = MakeButton("Sửa", x2 + 90, y + 100); btnSuaDG.Click += BtnSuaDG_Click; tab.Controls.Add(btnSuaDG);
            btnXoaDG = MakeButton("Xóa", x2 + 180, y + 100); btnXoaDG.Click += BtnXoaDG_Click; tab.Controls.Add(btnXoaDG);
            btnLamMoiDG = MakeButton("Làm mới", x2 + 270, y + 100); btnLamMoiDG.Click += BtnLamMoiDG_Click; tab.Controls.Add(btnLamMoiDG);

            return tab;
        }

        // ------------------------------------------------
        // TAB 2: QUẢN LÝ ĐẦU SÁCH
        // ------------------------------------------------
        private TabPage BuildTabDauSach()
        {
            var tab = new TabPage("Quản lý Đầu sách") { Padding = new Padding(10) };

            dgvDauSach = new DataGridView
            {
                Location = new Point(10, 10),
                Size = new Size(1100, 320),
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false
            };
            dgvDauSach.CellClick += DgvDauSach_CellClick;
            tab.Controls.Add(dgvDauSach);

            int y = 350, xLbl = 20, xTxt = 130;
            tab.Controls.Add(MakeLabel("Mã sách:", xLbl, y));
            txtMaSach = MakeTextBox(xTxt, y); tab.Controls.Add(txtMaSach);

            tab.Controls.Add(MakeLabel("Tên sách:", xLbl, y + 35));
            txtTenSach = MakeTextBox(xTxt, y + 35); tab.Controls.Add(txtTenSach);

            tab.Controls.Add(MakeLabel("Tác giả:", xLbl, y + 70));
            txtTacGia = MakeTextBox(xTxt, y + 70); tab.Controls.Add(txtTacGia);

            tab.Controls.Add(MakeLabel("NXB:", xLbl, y + 105));
            txtNXB = MakeTextBox(xTxt, y + 105); tab.Controls.Add(txtNXB);

            tab.Controls.Add(MakeLabel("Thể loại:", xLbl, y + 140));
            txtTheLoai = MakeTextBox(xTxt, y + 140); tab.Controls.Add(txtTheLoai);

            int x2 = 450;
            tab.Controls.Add(MakeLabel("Năm XB:", x2, y));
            numNamXB = new NumericUpDown { Location = new Point(x2 + 90, y), Width = 200, Maximum = 2100, Minimum = 1900, Value = 2020 };
            tab.Controls.Add(numNamXB);

            tab.Controls.Add(MakeLabel("Số lượng tổng:", x2, y + 35));
            numTong = new NumericUpDown { Location = new Point(x2 + 120, y + 35), Width = 170, Maximum = 10000 };
            tab.Controls.Add(numTong);

            tab.Controls.Add(MakeLabel("Số lượng còn:", x2, y + 70));
            numCon = new NumericUpDown { Location = new Point(x2 + 120, y + 70), Width = 170, Maximum = 10000 };
            tab.Controls.Add(numCon);

            btnThemS = MakeButton("Thêm", x2, y + 100); btnThemS.Click += BtnThemS_Click; tab.Controls.Add(btnThemS);
            btnSuaS = MakeButton("Sửa", x2 + 90, y + 100); btnSuaS.Click += BtnSuaS_Click; tab.Controls.Add(btnSuaS);
            btnXoaS = MakeButton("Xóa", x2 + 180, y + 100); btnXoaS.Click += BtnXoaS_Click; tab.Controls.Add(btnXoaS);
            btnLamMoiS = MakeButton("Làm mới", x2 + 270, y + 100); btnLamMoiS.Click += BtnLamMoiS_Click; tab.Controls.Add(btnLamMoiS);

            return tab;
        }

        // ------------------------------------------------
        // TAB 3: MƯỢN / TRẢ SÁCH
        // ------------------------------------------------
        private TabPage BuildTabMuonTra()
        {
            var tab = new TabPage("Mượn / Trả sách") { Padding = new Padding(10) };

            var grpMuon = new GroupBox { Text = "Lập phiếu mượn", Location = new Point(10, 10), Size = new Size(1100, 180) };
            tab.Controls.Add(grpMuon);

            grpMuon.Controls.Add(MakeLabel("Độc giả:", 20, 25));
            cboDocGia = new ComboBox { Location = new Point(120, 22), Width = 320, DropDownStyle = ComboBoxStyle.DropDownList };
            grpMuon.Controls.Add(cboDocGia);

            grpMuon.Controls.Add(MakeLabel("Đầu sách:", 470, 25));
            cboSach = new ComboBox { Location = new Point(560, 22), Width = 320, DropDownStyle = ComboBoxStyle.DropDownList };
            grpMuon.Controls.Add(cboSach);

            grpMuon.Controls.Add(MakeLabel("Số lượng:", 20, 65));
            numSoLuong = new NumericUpDown { Location = new Point(120, 62), Width = 100, Minimum = 1, Maximum = 1000, Value = 1 };
            grpMuon.Controls.Add(numSoLuong);

            grpMuon.Controls.Add(MakeLabel("Ngày mượn:", 250, 65));
            dtpNgayMuon = new DateTimePicker { Location = new Point(340, 62), Width = 150, Format = DateTimePickerFormat.Short, Value = DateTime.Now };
            grpMuon.Controls.Add(dtpNgayMuon);

            grpMuon.Controls.Add(MakeLabel("Hạn trả:", 520, 65));
            dtpHanTra = new DateTimePicker { Location = new Point(600, 62), Width = 150, Format = DateTimePickerFormat.Short, Value = DateTime.Now.AddDays(14) };
            grpMuon.Controls.Add(dtpHanTra);

            btnLapPhieu = MakeButton("Lập phiếu mượn", 780, 60, 200);
            btnLapPhieu.Click += BtnLapPhieu_Click;
            grpMuon.Controls.Add(btnLapPhieu);

            var grpList = new GroupBox { Text = "Danh sách phiếu mượn", Location = new Point(10, 200), Size = new Size(1100, 500) };
            tab.Controls.Add(grpList);

            dgvPhieu = new DataGridView
            {
                Location = new Point(10, 25),
                Size = new Size(1080, 430),
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false
            };
            grpList.Controls.Add(dgvPhieu);

            btnLoadPhieu = MakeButton("Tải lại", 10, 465, 120); btnLoadPhieu.Click += BtnLoadPhieu_Click; grpList.Controls.Add(btnLoadPhieu);
            btnTraSach = MakeButton("Trả sách đã chọn", 140, 465, 180); btnTraSach.Click += BtnTraSach_Click; grpList.Controls.Add(btnTraSach);

            return tab;
        }

        // ------------------------------------------------
        // TAB 4: CẢNH BÁO QUÁ HẠN + THỐNG KÊ
        // ------------------------------------------------
        private TabPage BuildTabCanhBao()
        {
            var tab = new TabPage("Cảnh báo & Thống kê") { Padding = new Padding(10) };

            var grp1 = new GroupBox { Text = "Phiếu mượn quá hạn", Location = new Point(10, 10), Size = new Size(1100, 320) };
            tab.Controls.Add(grp1);
            dgvQuaHan = new DataGridView
            {
                Location = new Point(10, 25),
                Size = new Size(1080, 250),
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false
            };
            grp1.Controls.Add(dgvQuaHan);
            btnLoadQuaHan = MakeButton("Tải danh sách quá hạn", 10, 285, 200);
            btnLoadQuaHan.Click += BtnLoadQuaHan_Click;
            grp1.Controls.Add(btnLoadQuaHan);

            var grp2 = new GroupBox { Text = "Thống kê số lượt mượn theo thể loại", Location = new Point(10, 340), Size = new Size(1100, 350) };
            tab.Controls.Add(grp2);
            dgvThongKe = new DataGridView
            {
                Location = new Point(10, 25),
                Size = new Size(1080, 280),
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false
            };
            grp2.Controls.Add(dgvThongKe);
            btnThongKe = MakeButton("Thống kê", 10, 315, 160);
            btnThongKe.Click += BtnThongKe_Click;
            grp2.Controls.Add(btnThongKe);

            return tab;
        }

        // ============ HELPERS TẠO CONTROL ============
        private static Label MakeLabel(string text, int x, int y) =>
            new() { Text = text, Location = new Point(x, y), AutoSize = true, Font = new Font("Segoe UI", 9.5f) };

        private static TextBox MakeTextBox(int x, int y) =>
            new() { Location = new Point(x, y), Width = 260 };

        private static Button MakeButton(string text, int x, int y, int width = 80) =>
            new() { Text = text, Location = new Point(x, y), Size = new Size(width, 32) };

        // ============ LOAD DỮ LIỆU BAN ĐẦU ============
        private void LoadAllData()
        {
            LoadDocGia();
            LoadDauSach();
            LoadCombos();
            LoadPhieu();
        }

        private void LoadDocGia()
        {
            dgvDocGia.DataSource = null;
            dgvDocGia.DataSource = _docGiaBUS.GetAll()
                .Select(d => new { d.MaDocGia, d.HoTen, d.NgaySinh, d.GioiTinh, d.DiaChi, d.SoDienThoai, d.NgayHetHanThe })
                .ToList();
        }

        private void LoadDauSach()
        {
            dgvDauSach.DataSource = null;
            dgvDauSach.DataSource = _dauSachBUS.GetAll();
        }

        private void LoadCombos()
        {
            cboDocGia.DataSource = _docGiaBUS.GetAll();
            cboDocGia.DisplayMember = "HoTen";
            cboDocGia.ValueMember = "MaDocGia";

            cboSach.DataSource = _dauSachBUS.GetAll();
            cboSach.DisplayMember = "TenSach";
            cboSach.ValueMember = "MaSach";
        }

        private void LoadPhieu()
        {
            dgvPhieu.DataSource = null;
            dgvPhieu.DataSource = _phieuMuonBUS.GetAll()
                .Select(p => new {
                    p.MaPhieu,
                    p.MaDocGia,
                    p.HoTenDocGia,
                    p.NgayMuon,
                    p.HanTra,
                    p.TrangThai
                }).ToList();
        }

        // ============ EVENT HANDLERS: TAB ĐỘC GIẢ ============
        private void DgvDocGia_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvDocGia.Rows[e.RowIndex];
            txtMaDG.Text = row.Cells["MaDocGia"].Value?.ToString();
            txtTenDG.Text = row.Cells["HoTen"].Value?.ToString();
            if (DateTime.TryParse(row.Cells["NgaySinh"].Value?.ToString(), out var ns))
                dtpNgaySinh.Value = ns;
            cboGioiTinh.Text = row.Cells["GioiTinh"].Value?.ToString();
            txtDiaChi.Text = row.Cells["DiaChi"].Value?.ToString();
            txtSDT.Text = row.Cells["SoDienThoai"].Value?.ToString();
            if (DateTime.TryParse(row.Cells["NgayHetHanThe"].Value?.ToString(), out var nh))
                dtpNgayHetHan.Value = nh;
        }

        private void BtnThemDG_Click(object? sender, EventArgs e)
        {
            try
            {
                _docGiaBUS.Insert(new DocGiaDTO
                {
                    MaDocGia = txtMaDG.Text.Trim(),
                    HoTen = txtTenDG.Text.Trim(),
                    NgaySinh = dtpNgaySinh.Value.Date,
                    GioiTinh = cboGioiTinh.Text,
                    DiaChi = txtDiaChi.Text.Trim(),
                    SoDienThoai = txtSDT.Text.Trim(),
                    NgayHetHanThe = dtpNgayHetHan.Value.Date
                });
                MessageBox.Show("Thêm độc giả thành công!");
                LoadDocGia();
                LoadCombos();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message, "Thông báo"); }
        }

        private void BtnSuaDG_Click(object? sender, EventArgs e)
        {
            try
            {
                _docGiaBUS.Update(new DocGiaDTO
                {
                    MaDocGia = txtMaDG.Text.Trim(),
                    HoTen = txtTenDG.Text.Trim(),
                    NgaySinh = dtpNgaySinh.Value.Date,
                    GioiTinh = cboGioiTinh.Text,
                    DiaChi = txtDiaChi.Text.Trim(),
                    SoDienThoai = txtSDT.Text.Trim(),
                    NgayHetHanThe = dtpNgayHetHan.Value.Date
                });
                MessageBox.Show("Cập nhật thành công!");
                LoadDocGia();
                LoadCombos();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message, "Thông báo"); }
        }

        private void BtnXoaDG_Click(object? sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Xóa độc giả này?", "Xác nhận",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                _docGiaBUS.Delete(txtMaDG.Text.Trim());
                MessageBox.Show("Đã xóa!");
                LoadDocGia();
                LoadCombos();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message, "Thông báo"); }
        }

        private void BtnLamMoiDG_Click(object? sender, EventArgs e)
        {
            txtMaDG.Clear(); txtTenDG.Clear(); txtDiaChi.Clear(); txtSDT.Clear();
            dtpNgaySinh.Value = DateTime.Now;
            dtpNgayHetHan.Value = DateTime.Now.AddYears(1);
            cboGioiTinh.SelectedIndex = 0;
        }

        // ============ EVENT HANDLERS: TAB ĐẦU SÁCH ============
        private void DgvDauSach_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvDauSach.Rows[e.RowIndex];
            txtMaSach.Text = row.Cells["MaSach"].Value?.ToString();
            txtTenSach.Text = row.Cells["TenSach"].Value?.ToString();
            txtTacGia.Text = row.Cells["TacGia"].Value?.ToString();
            txtNXB.Text = row.Cells["NhaXuatBan"].Value?.ToString();
            txtTheLoai.Text = row.Cells["TheLoai"].Value?.ToString();
            numNamXB.Value = Convert.ToDecimal(row.Cells["NamXuatBan"].Value ?? 0);
            numTong.Value = Convert.ToDecimal(row.Cells["SoLuongTong"].Value ?? 0);
            numCon.Value = Convert.ToDecimal(row.Cells["SoLuongCon"].Value ?? 0);
        }

        private void BtnThemS_Click(object? sender, EventArgs e)
        {
            try
            {
                _dauSachBUS.Insert(new DauSachDTO
                {
                    MaSach = txtMaSach.Text.Trim(),
                    TenSach = txtTenSach.Text.Trim(),
                    TacGia = txtTacGia.Text.Trim(),
                    NhaXuatBan = txtNXB.Text.Trim(),
                    NamXuatBan = (int)numNamXB.Value,
                    TheLoai = txtTheLoai.Text.Trim(),
                    SoLuongTong = (int)numTong.Value,
                    SoLuongCon = (int)numCon.Value
                });
                MessageBox.Show("Thêm đầu sách thành công!");
                LoadDauSach(); LoadCombos();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message, "Thông báo"); }
        }

        private void BtnSuaS_Click(object? sender, EventArgs e)
        {
            try
            {
                _dauSachBUS.Update(new DauSachDTO
                {
                    MaSach = txtMaSach.Text.Trim(),
                    TenSach = txtTenSach.Text.Trim(),
                    TacGia = txtTacGia.Text.Trim(),
                    NhaXuatBan = txtNXB.Text.Trim(),
                    NamXuatBan = (int)numNamXB.Value,
                    TheLoai = txtTheLoai.Text.Trim(),
                    SoLuongTong = (int)numTong.Value,
                    SoLuongCon = (int)numCon.Value
                });
                MessageBox.Show("Cập nhật đầu sách thành công!");
                LoadDauSach(); LoadCombos();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message, "Thông báo"); }
        }

        private void BtnXoaS_Click(object? sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Xóa đầu sách này?", "Xác nhận",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                _dauSachBUS.Delete(txtMaSach.Text.Trim());
                MessageBox.Show("Đã xóa!");
                LoadDauSach(); LoadCombos();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message, "Thông báo"); }
        }

        private void BtnLamMoiS_Click(object? sender, EventArgs e)
        {
            txtMaSach.Clear(); txtTenSach.Clear(); txtTacGia.Clear();
            txtNXB.Clear(); txtTheLoai.Clear();
            numNamXB.Value = 2020; numTong.Value = 1; numCon.Value = 1;
        }

        // ============ EVENT HANDLERS: TAB MƯỢN/TRẢ ============
        private void BtnLapPhieu_Click(object? sender, EventArgs e)
        {
            try
            {
                if (cboDocGia.SelectedValue == null || cboSach.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn độc giả và sách!");
                    return;
                }
                var pm = new PhieuMuonDTO
                {
                    MaPhieu = "PM" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    MaDocGia = cboDocGia.SelectedValue!.ToString()!,
                    NgayMuon = dtpNgayMuon.Value.Date,
                    HanTra = dtpHanTra.Value.Date
                };
                _phieuMuonBUS.LapPhieuMuon(pm, cboSach.SelectedValue!.ToString()!, (int)numSoLuong.Value);
                MessageBox.Show("Lập phiếu mượn thành công!\nMã phiếu: " + pm.MaPhieu);
                LoadPhieu(); LoadDauSach();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message, "Thông báo"); }
        }

        private void BtnLoadPhieu_Click(object? sender, EventArgs e) => LoadPhieu();

        private void BtnTraSach_Click(object? sender, EventArgs e)
        {
            try
            {
                if (dgvPhieu.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn phiếu cần trả!");
                    return;
                }
                var maPhieu = dgvPhieu.SelectedRows[0].Cells["MaPhieu"].Value?.ToString();
                var trangThai = dgvPhieu.SelectedRows[0].Cells["TrangThai"].Value?.ToString();
                if (trangThai == "Đã trả")
                {
                    MessageBox.Show("Phiếu này đã được trả rồi!");
                    return;
                }
                if (MessageBox.Show($"Trả sách cho phiếu {maPhieu}?", "Xác nhận",
                        MessageBoxButtons.YesNo) != DialogResult.Yes) return;

                _phieuMuonBUS.TraSach(maPhieu!);
                MessageBox.Show("Trả sách thành công! Đã cộng lại số lượng vào kho.");
                LoadPhieu(); LoadDauSach();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message, "Thông báo"); }
        }

        // ============ EVENT HANDLERS: TAB CẢNH BÁO + THỐNG KÊ ============
        private void BtnLoadQuaHan_Click(object? sender, EventArgs e)
        {
            try
            {
                var list = _phieuMuonBUS.GetQuaHan();
                dgvQuaHan.DataSource = list.Select(p => new {
                    p.MaPhieu,
                    p.MaDocGia,
                    p.HoTenDocGia,
                    p.NgayMuon,
                    p.HanTra,
                    p.TrangThai,
                    p.SoNgayQuaHan
                }).ToList();
                if (list.Count == 0)
                    MessageBox.Show("Không có phiếu quá hạn.");
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message, "Thông báo"); }
        }

        private void BtnThongKe_Click(object? sender, EventArgs e)
        {
            try
            {
                dgvThongKe.DataSource = _phieuMuonBUS.ThongKeTheoTheLoai();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message, "Thông báo"); }
        }
    }
}