# BÁO CÁO BÀI TẬP THỰC HÀNH LẬP TRÌNH C# WINFORMS

## I. THÔNG TIN SINH VIÊN

* **Họ và tên:** Trần Thế Hào
* **Mã số sinh viên:** 24810320120
* **Lớp học phần:** Lập trình .NET – Lớp: D19QTANM1
* **Link Repository:** https://github.com/datdct24810320215-cloud/LTNET-BaiTap_66_67_68_-_24810320215_DangCaoThanhDat.git

---

## II. DANH SÁCH BÀI TẬP HOÀN THÀNH & TỰ ĐÁNH GIÁ

| Bài tập | Tiêu đề bài tập | Trạng thái | Mức độ hoàn thành (%) |
| :---: | :--- | :---: | :---: |
| **Bài 6.6** | Hệ thống Quản lý Mượn trả Sách Thư viện (3-Tier, lọc trạng thái, cảnh báo quá hạn) | Hoàn thành | 100% |
| **Bài 6.7** | Hệ thống Quản lý Phòng khám & Kê đơn thuốc (SqlTransaction, trừ tồn kho) | Hoàn thành | 100% |
| **Bài 6.8** | Hệ thống Quản lý Bàn Bida & Dịch vụ Giải trí (Visual Grid, DataTable, SqlTransaction) | Hoàn thành | 100% |

---

## III. CẤU TRÚC SOLUTION 3-TIER

Tất cả các bài đều tuân thủ cấu trúc Solution 3-Tier chuẩn:

---

## IV. MINH HỌA GIAO DIỆN & KỸ THUẬT NỔI BẬT

### 1. Bài 6.6: Hệ thống Quản lý Mượn trả Sách Thư viện

* **Kỹ thuật áp dụng:**
  * Kiến trúc 3-Tier đầy đủ (DTO / DAL / BUS / GUI).
  * `SqlParameter` chống SQL Injection khi lọc phiếu mượn.
  * `SqlDataReader` để đọc danh sách phiếu quá hạn.
  * `ExecuteScalar` + `GROUP BY` để thống kê lượt mượn theo `TheLoai`.
  * Kiểm tra hạn thẻ `NgayHetHanThe >= GETDATE()` trước khi lập phiếu mượn.
  * `UPDATE` có tham số khi trả sách, đồng thời cộng lại `SoLuongCon` cho `DauSach`.

* **Hình ảnh minh họa Giao diện:**

![Giao diện chính Bài 6.6](docs/bai66_main.png)

*Mô tả: Form chính hiển thị danh sách Độc giả và Đầu sách sau khi tải dữ liệu.*

![Thêm sách mới Bài 6.6](docs/bai66_add_book.png)

*Mô tả: Thêm đầu sách mới thành công, dòng sách xuất hiện cuối lưới.*

![Cảnh báo thẻ hết hạn Bài 6.6](docs/bai66_card_expired.png)

*Mô tả: Chặn lập phiếu mượn khi thẻ độc giả đã hết hạn.*

![Lọc phiếu theo trạng thái Bài 6.6](docs/bai66_loan_list.png)

*Mô tả: Lọc danh sách phiếu mượn theo Độc giả và Trạng thái "Đang mượn".*

![Trả sách Bài 6.6](docs/bai66_return.png)

*Mô tả: Trả sách chuyển trạng thái sang "Đã trả" và tự động cộng lại SoLuongCon.*

![Danh sách quá hạn Bài 6.6](docs/bai66_overdue.png)

*Mô tả: Liệt kê các phiếu có HanTra < GETDATE() và TrangThai <> N'Đã trả' bằng SqlDataReader.*

![Thống kê theo thể loại Bài 6.6](docs/bai66_statistics.png)

*Mô tả: Thống kê tổng số lượt mượn theo từng TheLoai bằng GROUP BY.*

---

### 2. Bài 6.7: Hệ thống Quản lý Phòng khám & Kê đơn thuốc

* **Kỹ thuật áp dụng:**
  * Tra cứu bệnh nhân theo `SoDienThoai` bằng `SqlParameter`.
  * Quản lý danh mục thuốc CRUD đầy đủ qua 3-Tier.
  * Lưu đơn thuốc bằng **`SqlTransaction`**: tạo `DonThuoc` → tạo `ChiTietDonThuoc` → trừ `SoLuongTon`; nếu thiếu tồn kho → `Rollback()` toàn bộ.
  * Tính tổng tiền = `TienKham + Σ(SoLuong × DonGia)`.
  * Cập nhật trạng thái đơn sang "Đã thanh toán".

* **Hình ảnh minh họa Giao diện:**

![Giao diện chính Bài 6.7](docs/bai67_main.png)

*Mô tả: Giao diện chính với menu Tiếp nhận – Danh mục thuốc – Kê đơn – Thanh toán.*

![Tra cứu bệnh nhân Bài 6.7](docs/bai67_find_patient.png)

*Mô tả: Tra cứu bệnh nhân theo số điện thoại bằng SqlParameter.*

![Tiếp nhận bệnh nhân mới Bài 6.7](docs/bai67_new_patient.png)

*Mô tả: Tạo mới bệnh nhân và hồ sơ khám khi số điện thoại chưa tồn tại.*

![Quản lý danh mục thuốc Bài 6.7](docs/bai67_medicine.png)

*Mô tả: Thêm thuốc mới vào danh mục, cập nhật đơn giá và tồn kho.*

![Kê đơn & xuất kho thành công Bài 6.7](docs/bai67_prescription.png)

*Mô tả: Lưu đơn thuốc thành công, SoLuongTon của các thuốc giảm tương ứng.*

![Rollback khi thiếu tồn kho Bài 6.7](docs/bai67_rollback.png)

*Mô tả: Cảnh báo không đủ tồn kho, trans.Rollback() — không có bản ghi DonThuoc nào được tạo.*

![Thanh toán hóa đơn Bài 6.7](docs/bai67_payment.png)

*Mô tả: Tính tổng chi phí và chuyển trạng thái đơn sang "Đã thanh toán".*

---

### 3. Bài 6.8: Hệ thống Quản lý Bàn Bida & Dịch vụ Giải trí

* **Kỹ thuật áp dụng:**
  * Sinh Button động từ CSDL để tạo **sơ đồ bàn trực quan** (xanh = trống, cam/đỏ = đang chơi).
  * Mô hình **ngắt kết nối** (`SqlDataAdapter` + `DataTable`) hiển thị danh sách dịch vụ đã gọi.
  * Tính giờ chơi tự động: `GioKetThuc - GioBatDau`, làm tròn theo block 15 phút.
  * **`SqlTransaction`** khi thanh toán: cập nhật `HoaDon` → trừ `TonKho` `MenuDichVu` → đổi `BanBida.TrangThai = "Trống"` → `Commit()`.
  * Bỏ qua trừ kho nếu dịch vụ là "Thuê gậy".

* **Hình ảnh minh họa Giao diện:**

![Sơ đồ bàn Bida Bài 6.8](docs/bai68_main.png)

*Mô tả: Sơ đồ bàn dạng Button động — xanh lá = Trống, cam/đỏ = Đang chơi.*

![Mở bàn Bài 6.8](docs/bai68_open_table.png)

*Mô tả: Mở bàn thành công, tạo HoaDon mới với GioBatDau = GETDATE().*

![Gọi dịch vụ (DataTable) Bài 6.8](docs/bai68_order.png)

*Mô tả: Thêm món vào hóa đơn theo mô hình ngắt kết nối DataTable.*

![Tính tiền & thanh toán Bài 6.8](docs/bai68_payment_calc.png)

*Mô tả: Form thanh toán hiển thị tiền giờ chơi, tiền dịch vụ và tổng cộng.*

![Sau khi thanh toán Bài 6.8](docs/bai68_paid.png)

*Mô tả: Bàn trở về trạng thái "Trống"; TonKho dịch vụ giảm; HoaDon.TrangThai = 1.*

---

## V. HƯỚNG DẪN CÀI ĐẶT & CHẠY DỰ ÁN

1. Clone Repository về máy:

```bash
git clone https://github.com/datdct24810320215-cloud/LTNET-BaiTap_66_67_68_-_24810320215_DangCaoThanhDat.git