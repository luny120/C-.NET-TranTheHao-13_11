namespace DAL
{
    /// <summary>
    /// Chuỗi kết nối dùng chung cho toàn bộ DAL.
    /// Đổi Server nếu máy bạn dùng instance SQL Server khác.
    /// </summary>
    public static class DBConnection
    {
        // Windows Authentication (phổ biến với SQL Express cài mặc định)
        public const string ConnectionString =
            @"Server=.\SQLEXPRESS;Database=BilliardsDB;Integrated Security=True;TrustServerCertificate=True;";

        // Nếu dùng SQL Authentication, thay bằng:
        // @"Server=.\SQLEXPRESS;Database=BilliardsDB;User Id=sa;Password=YourPass;TrustServerCertificate=True;";
    }
}