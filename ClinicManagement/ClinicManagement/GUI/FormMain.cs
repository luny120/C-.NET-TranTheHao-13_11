namespace ClinicManagement.GUI
{
    public class FormMain : Form
    {
        public FormMain()
        {
            Text = "Hệ thống quản lý phòng khám";
            Width = 900;
            Height = 620;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.WhiteSmoke;

            var lblTitle = new Label
            {
                Text = "HỆ THỐNG QUẢN LÝ PHÒNG KHÁM & KÊ ĐƠN THUỐC",
                Dock = DockStyle.Top,
                Height = 70,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White
            };
            Controls.Add(lblTitle);

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(40, 60, 40, 40)
            };
            Controls.Add(panel);
            panel.BringToFront();

            panel.Controls.Add(MakeMenuButton("🩺  Tiếp nhận bệnh nhân",
                (s, e) => new FormBenhNhan().ShowDialog()));
            panel.Controls.Add(MakeMenuButton("💊  Quản lý thuốc",
                (s, e) => new FormThuoc().ShowDialog()));
            panel.Controls.Add(MakeMenuButton("📝  Kê đơn & Thanh toán",
                (s, e) => new FormKeDon().ShowDialog()));
        }

        private Button MakeMenuButton(string text, EventHandler onClick)
        {
            var btn = new Button
            {
                Text = text,
                Width = 400,
                Height = 70,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                Margin = new Padding(0, 15, 0, 15),
                BackColor = Color.LightSteelBlue
            };
            btn.Click += onClick;
            return btn;
        }
    }
}