using BUS;
using DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace GUI
{
    public class FormMain : Form
    {
        // ================= SERVICES =================
        private readonly BanBidaBUS _banBUS = new();
        private readonly MenuDichVuBUS _dvBUS = new();
        private readonly HoaDonBUS _hdBUS = new();
        private readonly ChiTietHoaDonBUS _ctBUS = new();

        // ================= CONTROLS =================
        private FlowLayoutPanel flpBan;          // sơ đồ bàn
        private Panel pnlRight;        // panel phải
        private Label lblTitle;
        private Label lblInfoBan;      // thông tin bàn đang chọn
        private Label lblTongTien;
        private ComboBox cboDichVu;
        private NumericUpDown numSoLuong;
        private Button btnThemDichVu;
        private Button btnXoaDong;
        private Button btnMoBan;
        private Button btnThanhToan;
        private DataGridView dgvChiTiet;

        // ================= STATE =================
        private int _maBanChon = -1;
        private int _maHDChon = -1;

        public FormMain()
        {
            // Form properties
            Text = "Quản lý quán Billiards - 3-Tier Demo";
            Width = 1280;
            Height = 720;
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 10F);
            BackColor = Color.FromArgb(245, 245, 245);

            BuildUI();
            LoadDanhSachBan();
            LoadDanhSachDichVu();
        }

        // ============================================================
        // BUILD UI BẰNG CODE (không dùng Designer)
        // ============================================================
        private void BuildUI()
        {
            // ---- Panel TRÁI: sơ đồ bàn ----
            var pnlLeft = new Panel
            {
                Dock = DockStyle.Left,
                Width = 780,
                Padding = new Padding(12),
                BackColor = Color.White
            };

            lblTitle = new Label
            {
                Text = "SƠ ĐỒ BÀN BIDA",
                Dock = DockStyle.Top,
                Height = 40,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 80, 150),
                TextAlign = ContentAlignment.MiddleCenter
            };

            flpBan = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(6),
                BackColor = Color.FromArgb(250, 250, 250)
            };
            flpBan.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.LightGray);
                e.Graphics.DrawRectangle(pen, 0, 0, flpBan.Width - 1, flpBan.Height - 1);
            };

            pnlLeft.Controls.Add(flpBan);
            pnlLeft.Controls.Add(lblTitle);

            // ---- Panel PHẢI ----
            pnlRight = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12),
                BackColor = Color.White
            };

            lblInfoBan = new Label
            {
                Text = "Chưa chọn bàn",
                Dock = DockStyle.Top,
                Height = 44,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.DarkSlateGray,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(4, 0, 0, 0)
            };

            // Group gọi dịch vụ
            var pnlGoiDV = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(0, 6, 0, 6)
            };
            var lblDV = new Label { Text = "Dịch vụ:", Left = 4, Top = 14, Width = 60 };
            cboDichVu = new ComboBox
            {
                Left = 66,
                Top = 10,
                Width = 220,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "HienThi",
                ValueMember = "MaDV"
            };
            var lblSL = new Label { Text = "SL:", Left = 296, Top = 14, Width = 30 };
            numSoLuong = new NumericUpDown { Left = 328, Top = 10, Width = 60, Minimum = 1, Maximum = 999, Value = 1 };
            btnThemDichVu = new Button
            {
                Text = "Thêm",
                Left = 398,
                Top = 8,
                Width = 80,
                Height = 30,
                BackColor = Color.FromArgb(30, 130, 200),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnThemDichVu.FlatAppearance.BorderSize = 0;
            btnThemDichVu.Click += BtnThemDichVu_Click;

            btnXoaDong = new Button
            {
                Text = "Xóa dòng",
                Left = 484,
                Top = 8,
                Width = 90,
                Height = 30,
                BackColor = Color.FromArgb(200, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnXoaDong.FlatAppearance.BorderSize = 0;
            btnXoaDong.Click += BtnXoaDong_Click;

            pnlGoiDV.Controls.AddRange(new Control[]
                { lblDV, cboDichVu, lblSL, numSoLuong, btnThemDichVu, btnXoaDong });

            // DataGridView chi tiết
            dgvChiTiet = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                BackgroundColor = Color.WhiteSmoke,
                BorderStyle = BorderStyle.FixedSingle
            };
            dgvChiTiet.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvChiTiet.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(220, 235, 250);

            // Panel tổng + nút hành động
            var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 110 };

            lblTongTien = new Label
            {
                Text = "Tổng tiền: 0 đ",
                Dock = DockStyle.Top,
                Height = 40,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.Crimson,
                TextAlign = ContentAlignment.MiddleRight
            };

            var pnlBtn = new Panel { Dock = DockStyle.Fill };
            btnMoBan = new Button
            {
                Text = "MỞ BÀN",
                Dock = DockStyle.Left,
                Width = 180,
                BackColor = Color.FromArgb(40, 160, 70),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnMoBan.FlatAppearance.BorderSize = 0;
            btnMoBan.Click += BtnMoBan_Click;

            btnThanhToan = new Button
            {
                Text = "THANH TOÁN",
                Dock = DockStyle.Right,
                Width = 200,
                BackColor = Color.FromArgb(210, 90, 20),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnThanhToan.FlatAppearance.BorderSize = 0;
            btnThanhToan.Click += BtnThanhToan_Click;

            pnlBtn.Controls.Add(btnThanhToan);
            pnlBtn.Controls.Add(btnMoBan);

            pnlBottom.Controls.Add(pnlBtn);
            pnlBottom.Controls.Add(lblTongTien);

            // Ráp panel phải
            pnlRight.Controls.Add(dgvChiTiet);
            pnlRight.Controls.Add(pnlBottom);
            pnlRight.Controls.Add(pnlGoiDV);
            pnlRight.Controls.Add(lblInfoBan);

            // Ráp vào form
            Controls.Add(pnlRight);
            Controls.Add(pnlLeft);
        }

        // ============================================================
        // LOAD DỮ LIỆU
        // ============================================================
        private void LoadDanhSachBan()
        {
            flpBan.Controls.Clear();
            List<BanBidaDTO> list = _banBUS.GetAll();
            foreach (var ban in list)
            {
                var btn = new Button
                {
                    Width = 160,
                    Height = 110,
                    Margin = new Padding(10),
                    Tag = ban.MaBan,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    Text = $"{ban.TenBan}\n({ban.LoaiBan})\n{ban.GiaTheoGio:N0} đ/h\n[{ban.TrangThai}]",
                    ForeColor = Color.White,
                    BackColor = ban.TrangThai == "Trống"
                                    ? Color.FromArgb(46, 160, 67)     // xanh lá
                                    : Color.FromArgb(230, 120, 30),   // cam
                    Cursor = Cursors.Hand
                };
                btn.FlatAppearance.BorderSize = 1;
                btn.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 60);
                btn.Click += BtnBan_Click;
                flpBan.Controls.Add(btn);
            }
        }

        private void LoadDanhSachDichVu()
        {
            var list = _dvBUS.GetAll();
            var view = new List<dynamic>();
            foreach (var dv in list)
                view.Add(new { dv.MaDV, HienThi = $"{dv.TenDV} - {dv.DonGia:N0}đ (tồn: {dv.TonKho})" });

            cboDichVu.DataSource = view;
            cboDichVu.DisplayMember = "HienThi";
            cboDichVu.ValueMember = "MaDV";
        }

        // ============================================================
        // SỰ KIỆN CLICK BÀN
        // ============================================================
        private void BtnBan_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int maBan) return;
            _maBanChon = maBan;

            var ban = _banBUS.GetByMaBan(maBan);
            if (ban == null) return;

            lblInfoBan.Text = $"{ban.TenBan}  |  {ban.LoaiBan}  |  {ban.GiaTheoGio:N0} đ/h  |  {ban.TrangThai}";

            if (ban.TrangThai == "Trống")
            {
                _maHDChon = -1;
                dgvChiTiet.DataSource = null;
                lblTongTien.Text = "Tổng tiền: 0 đ";
                btnMoBan.Enabled = true;
                btnThanhToan.Enabled = false;
                SetGoiDichVuEnabled(false);
            }
            else // Đang chơi
            {
                var hd = _hdBUS.GetActiveByMaBan(maBan);
                if (hd == null)
                {
                    MessageBox.Show("Không tìm thấy hóa đơn mở của bàn này!",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                _maHDChon = hd.MaHD;
                LoadChiTiet();
                CapNhatTongTien();
                btnMoBan.Enabled = false;
                btnThanhToan.Enabled = true;
                SetGoiDichVuEnabled(true);
            }
        }

        private void SetGoiDichVuEnabled(bool enabled)
        {
            cboDichVu.Enabled = enabled;
            numSoLuong.Enabled = enabled;
            btnThemDichVu.Enabled = enabled;
            btnXoaDong.Enabled = enabled;
        }

        // ============================================================
        // MỞ BÀN
        // ============================================================
        private void BtnMoBan_Click(object? sender, EventArgs e)
        {
            if (_maBanChon < 0)
            {
                MessageBox.Show("Vui lòng chọn một bàn trống!");
                return;
            }
            try
            {
                _hdBUS.MoBan(_maBanChon);
                LoadDanhSachBan();          // refresh màu sắc / trạng thái
                BtnBan_Click(GetBanButton(_maBanChon), EventArgs.Empty);
                MessageBox.Show("Đã mở bàn thành công!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi mở bàn: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Button? GetBanButton(int maBan)
        {
            foreach (Control c in flpBan.Controls)
                if (c is Button b && b.Tag is int id && id == maBan) return b;
            return null;
        }

        // ============================================================
        // GỌI DỊCH VỤ
        // ============================================================
        private void LoadChiTiet()
        {
            if (_maHDChon <= 0) { dgvChiTiet.DataSource = null; return; }
            // DÙNG DISCONNECTED: Fill DataTable bằng SqlDataAdapter (tầng DAL)
            DataTable dt = _ctBUS.GetChiTietDataTable(_maHDChon);
            dgvChiTiet.DataSource = dt;

            // Đặt lại HeaderText thân thiện
            if (dgvChiTiet.Columns.Count > 0)
            {
                dgvChiTiet.Columns["MaDV"].HeaderText = "Mã";
                dgvChiTiet.Columns["TenDV"].HeaderText = "Tên dịch vụ";
                dgvChiTiet.Columns["SoLuong"].HeaderText = "SL";
                dgvChiTiet.Columns["DonGia"].HeaderText = "Đơn giá";
                dgvChiTiet.Columns["ThanhTien"].HeaderText = "Thành tiền";
                dgvChiTiet.Columns["MaDV"].FillWeight = 30;
                dgvChiTiet.Columns["TenDV"].FillWeight = 180;
                dgvChiTiet.Columns["SoLuong"].FillWeight = 40;
                dgvChiTiet.Columns["DonGia"].FillWeight = 80;
                dgvChiTiet.Columns["ThanhTien"].FillWeight = 100;
            }
        }

        private void BtnThemDichVu_Click(object? sender, EventArgs e)
        {
            if (_maHDChon <= 0) return;
            if (cboDichVu.SelectedValue is not int maDV) return;

            int soLuong = (int)numSoLuong.Value;
            var dv = _dvBUS.GetByMaDV(maDV);
            if (dv == null) return;

            _ctBUS.AddOrUpdate(_maHDChon, maDV, soLuong, dv.DonGia);
            LoadChiTiet();
            CapNhatTongTien();
        }

        private void BtnXoaDong_Click(object? sender, EventArgs e)
        {
            if (_maHDChon <= 0) return;
            if (dgvChiTiet.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn dòng cần xóa!");
                return;
            }
            if (dgvChiTiet.SelectedRows[0].Cells["MaDV"].Value is not int maDV) return;

            var confirm = MessageBox.Show("Xóa dòng dịch vụ này?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            _ctBUS.Delete(_maHDChon, maDV);
            LoadChiTiet();
            CapNhatTongTien();
        }

        // ============================================================
        // TÍNH TỔNG TIỀN (live)
        // ============================================================
        private void CapNhatTongTien()
        {
            if (_maHDChon <= 0 || _maBanChon <= 0)
            {
                lblTongTien.Text = "Tổng tiền: 0 đ";
                return;
            }
            var (tienGio, tienDV, tong) = _hdBUS.TinhTamTinh(_maHDChon, _maBanChon);
            lblTongTien.Text =
                $"Giờ: {tienGio:N0} đ  +  DV: {tienDV:N0} đ   →   TỔNG: {tong:N0} đ";
        }

        // ============================================================
        // THANH TOÁN
        // ============================================================
        private void BtnThanhToan_Click(object? sender, EventArgs e)
        {
            if (_maHDChon <= 0 || _maBanChon <= 0) return;

            // Xem trước
            var (tienGio, tienDV, tong) = _hdBUS.TinhTamTinh(_maHDChon, _maBanChon);
            string preview =
                $"XÁC NHẬN THANH TOÁN\n\n" +
                $"Tiền giờ : {tienGio:N0} đ\n" +
                $"Dịch vụ  : {tienDV:N0} đ\n" +
                $"-------------------------\n" +
                $"TỔNG     : {tong:N0} đ\n\n" +
                $"Bạn có chắc chắn thanh toán?";

            if (MessageBox.Show(preview, "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            var (ok, tongTien) = _hdBUS.ThanhToan(_maHDChon, _maBanChon);
            if (ok)
            {
                MessageBox.Show($"Thanh toán thành công!\nTổng: {tongTien:N0} đ",
                    "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Reset trạng thái
                _maHDChon = -1;
                dgvChiTiet.DataSource = null;
                lblTongTien.Text = "Tổng tiền: 0 đ";
                lblInfoBan.Text = "Chưa chọn bàn";
                btnMoBan.Enabled = false;
                btnThanhToan.Enabled = false;
                SetGoiDichVuEnabled(false);

                LoadDanhSachBan();     // refresh sơ đồ
                LoadDanhSachDichVu();  // refresh tồn kho (nếu muốn thấy thay đổi)
            }
            else
            {
                MessageBox.Show("Thanh toán thất bại! Đã rollback transaction.",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}