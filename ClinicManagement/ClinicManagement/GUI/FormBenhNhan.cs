using ClinicManagement.BUS;
using ClinicManagement.DTO;

namespace ClinicManagement.GUI
{
    public class FormBenhNhan : Form
    {
        private readonly BenhNhanBUS bus = new BenhNhanBUS();

        private TextBox txtSDT = null!, txtHoTen = null!, txtNamSinh = null!,
                         txtTrieuChung = null!, txtChanDoan = null!;
        private DataGridView dgv = null!;
        private Label lblKetQua = null!;

        public FormBenhNhan()
        {
            Text = "Tiếp nhận bệnh nhân";
            Width = 900;
            Height = 620;
            StartPosition = FormStartPosition.CenterScreen;

            BuildUI();
            LoadDanhSach();
        }

        private void BuildUI()
        {
            // ---- Hàng 1 ----
            Controls.Add(new Label { Text = "SĐT:", Left = 20, Top = 23, Width = 50 });
            txtSDT = new TextBox { Left = 70, Top = 20, Width = 180 };
            Controls.Add(txtSDT);

            Controls.Add(new Label { Text = "Họ tên:", Left = 270, Top = 23, Width = 55 });
            txtHoTen = new TextBox { Left = 330, Top = 20, Width = 200 };
            Controls.Add(txtHoTen);

            Controls.Add(new Label { Text = "Năm sinh:", Left = 550, Top = 23, Width = 70 });
            txtNamSinh = new TextBox { Left = 625, Top = 20, Width = 80 };
            Controls.Add(txtNamSinh);

            var btnTim = new Button { Text = "Tra cứu", Left = 720, Top = 18, Width = 100, Height = 28 };
            btnTim.Click += BtnTim_Click;
            Controls.Add(btnTim);

            // ---- Hàng 2 ----
            Controls.Add(new Label { Text = "Triệu chứng:", Left = 20, Top = 58, Width = 80 });
            txtTrieuChung = new TextBox { Left = 105, Top = 55, Width = 300 };
            Controls.Add(txtTrieuChung);

            Controls.Add(new Label { Text = "Chẩn đoán:", Left = 420, Top = 58, Width = 75 });
            txtChanDoan = new TextBox { Left = 500, Top = 55, Width = 320 };
            Controls.Add(txtChanDoan);

            // ---- Nút Lưu / Làm mới ----
            var btnLuu = new Button { Text = "Lưu / Tiếp nhận", Left = 20, Top = 95, Width = 140, Height = 32 };
            btnLuu.Click += BtnLuu_Click;
            Controls.Add(btnLuu);

            var btnLamMoi = new Button { Text = "Làm mới", Left = 170, Top = 95, Width = 100, Height = 32 };
            btnLamMoi.Click += (s, e) => ResetForm();
            Controls.Add(btnLamMoi);

            lblKetQua = new Label { Left = 290, Top = 102, Width = 580, ForeColor = Color.DarkGreen };
            Controls.Add(lblKetQua);

            // ---- DataGridView ----
            dgv = new DataGridView
            {
                Left = 20,
                Top = 145,
                Width = 840,
                Height = 420,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            Controls.Add(dgv);
        }

        private void LoadDanhSach()
        {
            try
            {
                dgv.DataSource = null;
                dgv.DataSource = bus.LayTatCa();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách: " + ex.Message);
            }
        }

        private void BtnTim_Click(object? s, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSDT.Text))
                {
                    MessageBox.Show("Nhập SĐT để tra cứu.");
                    return;
                }
                var bn = bus.TimTheoSDT(txtSDT.Text.Trim());
                if (bn == null)
                {
                    MessageBox.Show("Không tìm thấy. Nhập họ tên và năm sinh để tạo mới.");
                    txtHoTen.Clear();
                    txtNamSinh.Clear();
                    txtHoTen.ReadOnly = false;
                    txtNamSinh.ReadOnly = false;
                    lblKetQua.Text = "";
                    return;
                }
                txtHoTen.Text = bn.HoTen;
                txtNamSinh.Text = bn.NamSinh.ToString();
                txtHoTen.ReadOnly = true;
                txtNamSinh.ReadOnly = true;
                lblKetQua.Text = $"Tìm thấy bệnh nhân: {bn.HoTen} (Mã BN: {bn.MaBN})";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void BtnLuu_Click(object? s, EventArgs e)
        {
            try
            {
                string sdt = txtSDT.Text.Trim();
                if (string.IsNullOrEmpty(sdt))
                {
                    MessageBox.Show("Chưa nhập SĐT.");
                    return;
                }
                int namSinh = 0;
                int.TryParse(txtNamSinh.Text.Trim(), out namSinh);

                var (bn, maHoSo) = bus.TiepNhan(
                    sdt,
                    txtHoTen.Text.Trim(),
                    namSinh,
                    txtTrieuChung.Text.Trim(),
                    txtChanDoan.Text.Trim());

                lblKetQua.Text = $"Tiếp nhận thành công: {bn.HoTen} (Mã BN: {bn.MaBN}) - Hồ sơ: {maHoSo}";
                LoadDanhSach();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu: " + ex.Message);
            }
        }

        private void ResetForm()
        {
            txtSDT.Clear();
            txtHoTen.Clear();
            txtNamSinh.Clear();
            txtTrieuChung.Clear();
            txtChanDoan.Clear();
            txtHoTen.ReadOnly = false;
            txtNamSinh.ReadOnly = false;
            lblKetQua.Text = "";
        }
    }
}