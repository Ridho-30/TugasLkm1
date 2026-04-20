using Npgsql;


namespace TugasLkm1.Helper
{
    public class DBHelper
    {
        private readonly string _connectionString;

        public DBHelper(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        public NpgsqlConnection CreateConnection()
            => new NpgsqlConnection(_connectionString);
    }
}
