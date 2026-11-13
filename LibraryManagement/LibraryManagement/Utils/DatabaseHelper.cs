using Microsoft.Data.SqlClient;

namespace LibraryManagement.Utils
{
    public static class DatabaseHelper
    {
        // 🔥 ĐÂY LÀ NƠI CẤU HÌNH CHUỖI KẾT NỐI
        private static readonly string _connectionString =
            @"Server=(local)\SQLEXPRESS;Database=LibraryDB;" +
            @"Integrated Security=True;TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}