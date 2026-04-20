using Npgsql;
using TugasLkm1.Helper;
using TugasLkm1.Models;

namespace TugasLkm1.Repositories
{
    public class CustomerRepository
    {
        private readonly DBHelper _db;
        public CustomerRepository(DBHelper db) { _db = db; }

        public async Task<List<Customer>> GetAllAsync()
        {
            var list = new List<Customer>();
            using var conn = _db.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand("SELECT * FROM customers ORDER BY id", conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(MapCustomer(reader));
            return list;
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            using var conn = _db.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand("SELECT * FROM customers WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapCustomer(reader) : null;
        }

        public async Task<Customer> CreateAsync(CustomerRequest req)
        {
            using var conn = _db.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand(@"
                INSERT INTO customers (name, email, phone, address)
                VALUES (@name, @email, @phone, @address)
                RETURNING *", conn);
            cmd.Parameters.AddWithValue("@name", req.Name);
            cmd.Parameters.AddWithValue("@email", req.Email);
            cmd.Parameters.AddWithValue("@phone", (object?)req.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@address", (object?)req.Address ?? DBNull.Value);
            using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();
            return MapCustomer(reader);
        }

        public async Task<Customer?> UpdateAsync(int id, CustomerRequest req)
        {
            using var conn = _db.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand(@"
                UPDATE customers
                SET name = @name, email = @email, phone = @phone,
                    address = @address, updated_at = NOW()
                WHERE id = @id
                RETURNING *", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", req.Name);
            cmd.Parameters.AddWithValue("@email", req.Email);
            cmd.Parameters.AddWithValue("@phone", (object?)req.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@address", (object?)req.Address ?? DBNull.Value);
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapCustomer(reader) : null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var conn = _db.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand("DELETE FROM customers WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        private static Customer MapCustomer(NpgsqlDataReader r) => new()
        {
            Id = r.GetInt32(r.GetOrdinal("id")),
            Name = r.GetString(r.GetOrdinal("name")),
            Email = r.GetString(r.GetOrdinal("email")),
            Phone = r.IsDBNull(r.GetOrdinal("phone")) ? null : r.GetString(r.GetOrdinal("phone")),
            Address = r.IsDBNull(r.GetOrdinal("address")) ? null : r.GetString(r.GetOrdinal("address")),
            CreatedAt = r.GetDateTime(r.GetOrdinal("created_at")),
            UpdatedAt = r.GetDateTime(r.GetOrdinal("updated_at")),
        };
    }
}