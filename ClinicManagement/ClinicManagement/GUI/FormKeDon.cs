using System.ComponentModel;
using ClinicManagement.BUS;
using ClinicManagement.DTO;

namespace ClinicManagement.GUI
{
    // Lớp phụ dùng làm dòng cho giỏ thuốc
    public class ChiTietGio
    {
        public int MaThuoc { get; set; }
        public string TenThuoc { get; set; } = string.Empty;
        public string DonViTinh { get; set; } = string.Empty;
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
        public string? LieuDung { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
    }

    public class FormKeDon : Form
    {
        private readonly BenhNhanBUS bnBUS = new BenhNhanBUS();
        private readonly ThuocBUS thuocBUS = new ThuocBUS();
        private readonly DonThuocBUS donBUS = new DonThuocBUS();

        // --- Thông tin bệnh nhân ---
        private TextBox txtSDT = null!, txtHoTen = null!, txtNamSinh = null!,
                         txtTrieuChung = null!, txtChanDoan = null!;
        private Label lblThongTinBN = null!;

        // --- Chọn thuốc ---
        private ComboBox cboThuoc = null!;
        private Label lblDVT = null!, lblDonGia = null!, lblTon = null!;
        private TextBox txtSoLuong = null!, txtLieuDung = null!;

        // --- Giỏ thuốc ---
        private DataGridView dgvGio = null!;
        private readonly BindingList<ChiTietGio> gio = new();

        // --- Thanh toán ---
        private TextBox txtTienKham = null!;
        private Label lblTongTien = null!;
        private Button btnLuu = null!, btnThanhToan = null!;

        private int? maHoSoHienTai = null;
        private int? maDonHienTai = null;

        public FormKeDon()
        {
            Text = "Kê đơn & Thanh toán";
            Width = 1000;
            Height = 720;
            StartPosition = FormStartPosition.CenterScreen;

            BuildUI();
            LoadThuoc();
        }

        // -----------------------------------------------------------------
        //  Xây dựng UI bằng code
        // -----------------------------------------------------------------
        private void BuildUI()
        {
            // ============ KHỐI BỆNH NHÂN ============
            Controls.Add(new Label { Text = "SĐT:", Left = 20, Top = 23, Width = 45 });
            txtSDT = new TextBox { Left = 65, Top = 20, Width = 150 };
            Controls.Add(txtSDT);

            Controls.Add(new Label { Text = "Họ tên:", Left = 230, Top = 23, Width = 55 });
            txtHoTen = new TextBox { Left = 285, Top = 20, Width = 180 };
            Controls.Add(txtHoTen);

            Controls.Add(new Label { Text = "Năm sinh:", Left = 480, Top = 23, Width = 70 });
            txtNamSinh = new TextBox { Left = 550, Top = 20, Width = 80 };
            Controls.Add(txtNamSinh);

            var btnTiepNhan = new Button
            {
                Text = "Tiếp nhận / Tạo hồ sơ",
                Left = 650,
                Top = 18,
                Width = 180,
                Height = 30
            };
            btnTiepNhan.Click += BtnTiepNhan_Click;
            Controls.Add(btnTiepNhan);

            Controls.Add(new Label { Text = "Triệu chứng:", Left = 20, Top = 58, Width = 80 });
            txtTrieuChung = new TextBox { Left = 105, Top = 55, Width = 300 };
            Controls.Add(txtTrieuChung);

            Controls.Add(new Label { Text = "Chẩn đoán:", Left = 420, Top = 58, Width = 75 });
            txtChanDoan = new TextBox { Left = 500, Top = 55, Width = 330 };
            Controls.Add(txtChanDoan);

            lblThongTinBN = new Label
            {
                Left = 20,
                Top = 95,
                Width = 940,
                Height = 22,
                ForeColor = Color.DarkGreen,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            Controls.Add(lblThongTinBN);

            // ============ KHỐI CHỌN THUỐC ============
            Controls.Add(new Label { Text = "Chọn thuốc:", Left = 20, Top = 135, Width = 80 });
            cboThuoc = new ComboBox
            {
                Left = 105,
                Top = 132,
                Width = 280,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboThuoc.SelectedIndexChanged += CboThuoc_SelectedIndexChanged;
            Controls.Add(cboThuoc);

            lblDVT = new Label { Left = 400, Top = 137, Width = 100 };
            lblDonGia = new Label { Left = 510, Top = 137, Width = 140 };
            lblTon = new Label { Left = 660, Top = 137, Width = 150 };
            Controls.Add(lblDVT); Controls.Add(lblDonGia); Controls.Add(lblTon);

            Controls.Add(new Label { Text = "Số lượng:", Left = 20, Top = 175, Width = 60 });
            txtSoLuong = new TextBox { Left = 85, Top = 172, Width = 80 };
            Controls.Add(txtSoLuong);

            Controls.Add(new Label { Text = "Liều dùng:", Left = 185, Top = 175, Width = 65 });
            txtLieuDung = new TextBox { Left = 255, Top = 172, Width = 280 };
            Controls.Add(txtLieuDung);

            var btnThemThuoc = new Button
            {
                Text = "Thêm thuốc",
                Left = 555,
                Top = 170,
                Width = 120,
                Height = 30
            };
            btnThemThuoc.Click += BtnThemThuoc_Click;
            Controls.Add(btnThemThuoc);

            var btnXoaDong = new Button
            {
                Text = "Xóa dòng",
                Left = 685,
                Top = 170,
                Width = 100,
                Height = 30
            };
            btnXoaDong.Click += BtnXoaDong_Click;
            Controls.Add(btnXoaDong);

            // ============ GRID GIỎ THUỐC ============
            dgvGio = new DataGridView
            {
                Left = 20,
                Top = 215,
                Width = 940,
                Height = 340,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvGio.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mã", DataPropertyName = "MaThuoc", FillWeight = 10 });
            dgvGio.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tên thuốc", DataPropertyName = "TenThuoc", FillWeight = 30 });
            dgvGio.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ĐVT", DataPropertyName = "DonViTinh", FillWeight = 10 });
            dgvGio.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Đơn giá",
                DataPropertyName = "DonGia",
                FillWeight = 15,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" }
            });
            dgvGio.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "SL", DataPropertyName = "SoLuong", FillWeight = 8 });
            dgvGio.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Liều dùng", DataPropertyName = "LieuDung", FillWeight = 15 });
            dgvGio.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Thành tiền",
                DataPropertyName = "ThanhTien",
                FillWeight = 15,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" }
            });
            dgvGio.DataSource = gio;
            Controls.Add(dgvGio);

            // ============ KHỐI THANH TOÁN ============
            Controls.Add(new Label { Text = "Tiền khám:", Left = 20, Top = 575, Width = 75 });
            txtTienKham = new TextBox { Left = 100, Top = 572, Width = 120, Text = "50000" };
            txtTienKham.TextChanged += (s, e) => CapNhatTongTien();
            Controls.Add(txtTienKham);

            lblTongTien = new Label
            {
                Left = 250,
                Top = 577,
                Width = 380,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.DarkRed
            };
            Controls.Add(lblTongTien);

            btnLuu = new Button
            {
                Text = "Lưu & Xuất đơn",
                Left = 650,
                Top = 568,
                Width = 140,
                Height = 34,
                BackColor = Color.LightGreen
            };
            btnLuu.Click += BtnLuu_Click;
            Controls.Add(btnLuu);

            btnThanhToan = new Button
            {
                Text = "Thanh toán",
                Left = 800,
                Top = 568,
                Width = 160,
                Height = 34,
                BackColor = Color.Khaki,
                Enabled = false
            };
            btnThanhToan.Click += BtnThanhToan_Click;
            Controls.Add(btnThanhToan);

            CapNhatTongTien();
        }

        // -----------------------------------------------------------------
        //  Xử lý sự kiện
        // -----------------------------------------------------------------
        private void LoadThuoc()
        {
            try
            {
                var ds = thuocBUS.LayTatCa();
                cboThuoc.DataSource = ds;
                cboThuoc.DisplayMember = "TenThuoc";
                cboThuoc.ValueMember = "MaThuoc";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách thuốc: " + ex.Message);
            }
        }

        private void CboThuoc_SelectedIndexChanged(object? s, EventArgs e)
        {
            if (cboThuoc.SelectedItem is Thuoc t)
            {
                lblDVT.Text = "ĐVT: " + t.DonViTinh;
                lblDonGia.Text = "Đơn giá: " + t.DonGia.ToString("N0");
                lblTon.Text = "Tồn kho: " + t.SoLuongTon;
            }
        }

        private void BtnTiepNhan_Click(object? s, EventArgs e)
        {
            try
            {
                string sdt = txtSDT.Text.Trim();
                if (string.IsNullOrEmpty(sdt))
                {
                    MessageBox.Show("Vui lòng nhập SĐT.");
                    return;
                }

                BenhNhan? existing = bnBUS.TimTheoSDT(sdt);
                string hoTen;
                int namSinh;

                if (existing != null)
                {
                    hoTen = existing.HoTen;
                    namSinh = existing.NamSinh;
                    txtHoTen.Text = existing.HoTen;
                    txtNamSinh.Text = existing.NamSinh.ToString();
                    txtHoTen.ReadOnly = true;
                    txtNamSinh.ReadOnly = true;
                }
                else
                {
                    hoTen = txtHoTen.Text.Trim();
                    if (string.IsNullOrEmpty(hoTen))
                    {
                        MessageBox.Show("Bệnh nhân mới - vui lòng nhập họ tên.");
                        return;
                    }
                    if (!int.TryParse(txtNamSinh.Text.Trim(), out namSinh))
                    {
                        MessageBox.Show("Năm sinh không hợp lệ.");
                        return;
                    }
                }

                var (bn, maHoSo) = bnBUS.TiepNhan(
                    sdt, hoTen, namSinh,
                    txtTrieuChung.Text.Trim(), txtChanDoan.Text.Trim());

                maHoSoHienTai = maHoSo;
                lblThongTinBN.Text =
                    $"✔ Mã BN: {bn.MaBN} | {bn.HoTen} | Năm sinh: {bn.NamSinh} | Mã hồ sơ: {maHoSo}";

                btnLuu.Enabled = true;
                btnThanhToan.Enabled = false;
                maDonHienTai = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tiếp nhận: " + ex.Message);
            }
        }

        private void BtnThemThuoc_Click(object? s, EventArgs e)
        {
            try
            {
                if (maHoSoHienTai == null)
                {
                    MessageBox.Show("Vui lòng tiếp nhận bệnh nhân trước.");
                    return;
                }
                if (cboThuoc.SelectedItem is not Thuoc t)
                {
                    MessageBox.Show("Chưa chọn thuốc.");
                    return;
                }
                if (!int.TryParse(txtSoLuong.Text.Trim(), out int sl) || sl <= 0)
                {
                    MessageBox.Show("Số lượng phải là số nguyên dương.");
                    return;
                }

                // Kiểm tra tồn kho phía client (DAL sẽ kiểm lại trong transaction)
                int daCo = gio.Where(x => x.MaThuoc == t.MaThuoc).Sum(x => x.SoLuong);
                if (daCo + sl > t.SoLuongTon)
                {
                    MessageBox.Show($"Không đủ tồn kho. Còn {t.SoLuongTon - daCo}.");
                    return;
                }

                var exist = gio.FirstOrDefault(x => x.MaThuoc == t.MaThuoc);
                if (exist != null)
                {
                    exist.SoLuong += sl;
                    if (!string.IsNullOrEmpty(txtLieuDung.Text.Trim()))
                        exist.LieuDung = txtLieuDung.Text.Trim();
                    gio.ResetBindings();
                }
                else
                {
                    gio.Add(new ChiTietGio
                    {
                        MaThuoc = t.MaThuoc,
                        TenThuoc = t.TenThuoc,
                        DonViTinh = t.DonViTinh,
                        DonGia = t.DonGia,
                        SoLuong = sl,
                        LieuDung = txtLieuDung.Text.Trim()
                    });
                }
                txtSoLuong.Clear();
                txtLieuDung.Clear();
                CapNhatTongTien();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm thuốc: " + ex.Message);
            }
        }

        private void BtnXoaDong_Click(object? s, EventArgs e)
        {
            if (dgvGio.CurrentRow?.DataBoundItem is ChiTietGio ct)
            {
                gio.Remove(ct);
                CapNhatTongTien();
            }
        }

        private void CapNhatTongTien()
        {
            decimal tongThuoc = gio.Sum(x => x.ThanhTien);
            decimal tienKham = decimal.TryParse(txtTienKham.Text.Trim(), out var tk) ? tk : 0;
            lblTongTien.Text = $"TỔNG TIỀN: {(tienKham + tongThuoc):N0} VNĐ";
        }

        private void BtnLuu_Click(object? s, EventArgs e)
        {
            try
            {
                if (maHoSoHienTai == null)
                {
                    MessageBox.Show("Chưa tiếp nhận bệnh nhân.");
                    return;
                }
                if (gio.Count == 0)
                {
                    MessageBox.Show("Chưa có thuốc nào trong đơn.");
                    return;
                }
                if (!decimal.TryParse(txtTienKham.Text.Trim(), out decimal tienKham) || tienKham < 0)
                {
                    MessageBox.Show("Tiền khám không hợp lệ.");
                    return;
                }

                var don = new DonThuoc
                {
                    MaHoSo = maHoSoHienTai.Value,
                    TienKham = tienKham,
                    TongTien = tienKham + gio.Sum(x => x.ThanhTien),
                    TrangThai = "Chưa thanh toán"
                };
                var chiTiet = gio.Select(x => new ChiTietDonThuoc
                {
                    MaThuoc = x.MaThuoc,
                    SoLuong = x.SoLuong,
                    LieuDung = x.LieuDung
                }).ToList();

                // Gọi BUS -> DAL (SqlTransaction)
                int maDon = donBUS.LuuDonThuoc(don, chiTiet);
                maDonHienTai = maDon;

                MessageBox.Show(
                    $"Đã lưu & xuất kho thành công!\nMã đơn: {maDon}\n" +
                    $"Tổng tiền: {don.TongTien:N0} VNĐ\nVui lòng bấm 'Thanh toán'.");

                btnLuu.Enabled = false;
                btnThanhToan.Enabled = true;

                // Reload thuốc để thấy tồn kho mới
                LoadThuoc();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu đơn (đã rollback): " + ex.Message);
            }
        }

        private void BtnThanhToan_Click(object? s, EventArgs e)
        {
            try
            {
                if (maDonHienTai == null)
                {
                    MessageBox.Show("Không có đơn để thanh toán.");
                    return;
                }
                donBUS.ThanhToan(maDonHienTai.Value);
                MessageBox.Show($"Đơn {maDonHienTai.Value} đã thanh toán thành công!");
                btnThanhToan.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thanh toán: " + ex.Message);
            }
        }
    }
}