using ClinicManagement.BUS;
using ClinicManagement.DTO;

namespace ClinicManagement.GUI
{
    public class FormThuoc : Form
    {
        private readonly ThuocBUS bus = new ThuocBUS();

        private DataGridView dgv = null!;
        private TextBox txtTen = null!, txtDVT = null!, txtDonGia = null!,
                         txtTon = null!, txtNhapThem = null!;
        private int? maDangChon = null;

        public FormThuoc()
        {
            Text = "Quản lý thuốc";
            Width = 950;
            Height = 620;
            StartPosition = FormStartPosition.CenterScreen;

            BuildUI();
            LoadData();
        }

        private void BuildUI()
        {
            dgv = new DataGridView
            {
                Left = 20,
                Top = 20,
                Width = 890,
                Height = 320,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgv.SelectionChanged += (s, e) =>
            {
                if (dgv.CurrentRow?.DataBoundItem is Thuoc t)
                {
                    maDangChon = t.MaThuoc;
                    txtTen.Text = t.TenThuoc;
                    txtDVT.Text = t.DonViTinh;
                    txtDonGia.Text = t.DonGia.ToString("0.##");
                    txtTon.Text = t.SoLuongTon.ToString();
                }
            };
            Controls.Add(dgv);

            int y = 360;
            Controls.Add(new Label { Text = "Tên thuốc:", Left = 20, Top = y + 3, Width = 70 });
            txtTen = new TextBox { Left = 95, Top = y, Width = 250 };
            Controls.Add(txtTen);

            Controls.Add(new Label { Text = "ĐVT:", Left = 360, Top = y + 3, Width = 40 });
            txtDVT = new TextBox { Left = 405, Top = y, Width = 100 };
            Controls.Add(txtDVT);

            Controls.Add(new Label { Text = "Đơn giá:", Left = 520, Top = y + 3, Width = 60 });
            txtDonGia = new TextBox { Left = 585, Top = y, Width = 100 };
            Controls.Add(txtDonGia);

            Controls.Add(new Label { Text = "Tồn kho:", Left = 700, Top = y + 3, Width = 60 });
            txtTon = new TextBox { Left = 765, Top = y, Width = 80, ReadOnly = true };
            Controls.Add(txtTon);

            y += 45;
            Controls.Add(MakeButton("Thêm", 20, y, BtnThem_Click));
            Controls.Add(MakeButton("Sửa", 130, y, BtnSua_Click));
            Controls.Add(MakeButton("Xóa", 240, y, BtnXoa_Click));

            Controls.Add(new Label { Text = "Nhập thêm:", Left = 360, Top = y + 7, Width = 75 });
            txtNhapThem = new TextBox { Left = 440, Top = y + 4, Width = 80 };
            Controls.Add(txtNhapThem);
            Controls.Add(MakeButton("Nhập tồn", 530, y, BtnNhapTon_Click));
            Controls.Add(MakeButton("Làm mới", 640, y, (s, e) => LoadData()));
        }

        private Button MakeButton(string text, int left, int top, EventHandler h)
        {
            var b = new Button { Text = text, Left = left, Top = top, Width = 100, Height = 30 };
            b.Click += h;
            return b;
        }

        private void LoadData()
        {
            try
            {
                dgv.DataSource = null;
                dgv.DataSource = bus.LayTatCa();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        private void BtnThem_Click(object? s, EventArgs e)
        {
            try
            {
                if (!decimal.TryParse(txtDonGia.Text.Trim(), out decimal dg) || dg < 0)
                {
                    MessageBox.Show("Đơn giá không hợp lệ.");
                    return;
                }
                var t = new Thuoc
                {
                    TenThuoc = txtTen.Text.Trim(),
                    DonViTinh = txtDVT.Text.Trim(),
                    DonGia = dg,
                    SoLuongTon = 0
                };
                bus.Them(t);
                LoadData();
                MessageBox.Show("Thêm thuốc thành công.");
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void BtnSua_Click(object? s, EventArgs e)
        {
            try
            {
                if (maDangChon == null) { MessageBox.Show("Chọn thuốc cần sửa."); return; }
                if (!decimal.TryParse(txtDonGia.Text.Trim(), out decimal dg) || dg < 0)
                {
                    MessageBox.Show("Đơn giá không hợp lệ."); return;
                }
                int ton = 0; int.TryParse(txtTon.Text, out ton);
                var t = new Thuoc
                {
                    MaThuoc = maDangChon.Value,
                    TenThuoc = txtTen.Text.Trim(),
                    DonViTinh = txtDVT.Text.Trim(),
                    DonGia = dg,
                    SoLuongTon = ton
                };
                bus.Sua(t);
                LoadData();
                MessageBox.Show("Cập nhật thành công.");
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void BtnXoa_Click(object? s, EventArgs e)
        {
            try
            {
                if (maDangChon == null) { MessageBox.Show("Chọn thuốc cần xóa."); return; }
                if (MessageBox.Show("Xác nhận xóa?", "Hỏi", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;
                bus.Xoa(maDangChon.Value);
                maDangChon = null;
                LoadData();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void BtnNhapTon_Click(object? s, EventArgs e)
        {
            try
            {
                if (maDangChon == null) { MessageBox.Show("Chọn thuốc cần nhập tồn."); return; }
                if (!int.TryParse(txtNhapThem.Text.Trim(), out int sl) || sl <= 0)
                {
                    MessageBox.Show("Số lượng nhập phải > 0."); return;
                }
                bus.NhapTon(maDangChon.Value, sl);
                txtNhapThem.Clear();
                LoadData();
                MessageBox.Show("Đã nhập thêm tồn kho.");
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }
    }
}