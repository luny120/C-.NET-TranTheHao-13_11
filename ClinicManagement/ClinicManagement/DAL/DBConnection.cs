using Microsoft.Data.SqlClient;

namespace ClinicManagement.DAL
{
    public static class DBConnection
    {
        // Sửa "Server=." thành "Server=.\SQLEXPRESS"
        public const string ConnectionString =
            "Server=.\\SQLEXPRESS;Database=ClinicDB;Integrated Security=True;TrustServerCertificate=True;";

        public static SqlConnection GetConnection() => new SqlConnection(ConnectionString);
    }
}