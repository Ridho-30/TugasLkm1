using Npgsql;
using TugasLkm1.Helper;
using TugasLkm1.Models;

namespace TugasLkm1.Repositories
{
    public class ProductRepository
    {
        private readonly DBHelper _db;
        public ProductRepository(DBHelper db) { _db = db; }

        public async Task<List<Product>> GetAllAsync()
        {
            var list = new List<Product>();
            using var conn = _db.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand(
                "SELECT * FROM products WHERE is_deleted = 0 ORDER BY id", conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(MapProduct(reader));
            return list;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            using var conn = _db.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand(
                "SELECT * FROM products WHERE id = @id AND is_deleted = 0", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapProduct(reader) : null;
        }

        public async Task<Product> CreateAsync(ProductRequest req)
        {
            using var conn = _db.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand(@"
                INSERT INTO products (name, description, price, stock, category)
                VALUES (@name, @desc, @price, @stock, @category)
                RETURNING *", conn);
            cmd.Parameters.AddWithValue("@name", req.Name);
            cmd.Parameters.AddWithValue("@desc", (object?)req.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@price", req.Price);
            cmd.Parameters.AddWithValue("@stock", req.Stock);
            cmd.Parameters.AddWithValue("@category", (object?)req.Category ?? DBNull.Value);
            using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();
            return MapProduct(reader);
        }

        public async Task<Product?> UpdateAsync(int id, ProductRequest req)
        {
            using var conn = _db.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand(@"
                UPDATE products
                SET name = @name, description = @desc, price = @price,
                    stock = @stock, category = @category, updated_at = NOW()
                WHERE id = @id AND is_deleted = 0
                RETURNING *", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", req.Name);
            cmd.Parameters.AddWithValue("@desc", (object?)req.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@price", req.Price);
            cmd.Parameters.AddWithValue("@stock", req.Stock);
            cmd.Parameters.AddWithValue("@category", (object?)req.Category ?? DBNull.Value);
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapProduct(reader) : null;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            using var conn = _db.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand(@"
                UPDATE products SET is_deleted = 1, updated_at = NOW()
                WHERE id = @id AND is_deleted = 0", conn);
            cmd.Parameters.AddWithValue("@id", id);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        private static Product MapProduct(NpgsqlDataReader r) => new()
        {
            Id = r.GetInt32(r.GetOrdinal("id")),
            Name = r.GetString(r.GetOrdinal("name")),
            Description = r.IsDBNull(r.GetOrdinal("description")) ? null : r.GetString(r.GetOrdinal("description")),
            Price = r.GetDecimal(r.GetOrdinal("price")),
            Stock = r.GetInt32(r.GetOrdinal("stock")),
            Category = r.IsDBNull(r.GetOrdinal("category")) ? null : r.GetString(r.GetOrdinal("category")),
            IsDeleted = r.GetInt32(r.GetOrdinal("is_deleted")),
            CreatedAt = r.GetDateTime(r.GetOrdinal("created_at")),
            UpdatedAt = r.GetDateTime(r.GetOrdinal("updated_at")),
        };
    }
}