using Npgsql;
using TugasLkm1.Helper;
using TugasLkm1.Models;

namespace TugasLkm1.Repositories
{
    public class OrderRepository
    {
        private readonly DBHelper _db;
        public OrderRepository(DBHelper db) { _db = db; }

        public async Task<List<Order>> GetAllAsync()
        {
            var list = new List<Order>();
            using var conn = _db.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand("SELECT * FROM orders ORDER BY id", conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(MapOrder(reader));
            return list;
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            using var conn = _db.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand("SELECT * FROM orders WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapOrder(reader) : null;
        }

        public async Task<Order> CreateAsync(OrderRequest req)
        {
            using var conn = _db.CreateConnection();
            await conn.OpenAsync();
            // Hitung total_price otomatis dari harga produk x quantity
            using var cmdPrice = new NpgsqlCommand(
                "SELECT price FROM products WHERE id = @id AND is_deleted = 0", conn);
            cmdPrice.Parameters.AddWithValue("@id", req.ProductId);
            var priceObj = await cmdPrice.ExecuteScalarAsync();
            if (priceObj is null) throw new Exception("Produk tidak ditemukan");
            var total = (decimal)priceObj * req.Quantity;

            using var cmd = new NpgsqlCommand(@"
                INSERT INTO orders (customer_id, product_id, quantity, total_price, status)
                VALUES (@cid, @pid, @qty, @total, @status)
                RETURNING *", conn);
            cmd.Parameters.AddWithValue("@cid", req.CustomerId);
            cmd.Parameters.AddWithValue("@pid", req.ProductId);
            cmd.Parameters.AddWithValue("@qty", req.Quantity);
            cmd.Parameters.AddWithValue("@total", total);
            cmd.Parameters.AddWithValue("@status", req.Status);
            using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();
            return MapOrder(reader);
        }

        public async Task<Order?> UpdateAsync(int id, OrderRequest req)
        {
            using var conn = _db.CreateConnection();
            await conn.OpenAsync();
            using var cmdPrice = new NpgsqlCommand(
                "SELECT price FROM products WHERE id = @id AND is_deleted = 0", conn);
            cmdPrice.Parameters.AddWithValue("@id", req.ProductId);
            var priceObj = await cmdPrice.ExecuteScalarAsync();
            if (priceObj is null) throw new Exception("Produk tidak ditemukan");
            var total = (decimal)priceObj * req.Quantity;

            using var cmd = new NpgsqlCommand(@"
                UPDATE orders
                SET customer_id = @cid, product_id = @pid, quantity = @qty,
                    total_price = @total, status = @status, updated_at = NOW()
                WHERE id = @id
                RETURNING *", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@cid", req.CustomerId);
            cmd.Parameters.AddWithValue("@pid", req.ProductId);
            cmd.Parameters.AddWithValue("@qty", req.Quantity);
            cmd.Parameters.AddWithValue("@total", total);
            cmd.Parameters.AddWithValue("@status", req.Status);
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapOrder(reader) : null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var conn = _db.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand("DELETE FROM orders WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        private static Order MapOrder(NpgsqlDataReader r) => new()
        {
            Id = r.GetInt32(r.GetOrdinal("id")),
            CustomerId = r.GetInt32(r.GetOrdinal("customer_id")),
            ProductId = r.GetInt32(r.GetOrdinal("product_id")),
            Quantity = r.GetInt32(r.GetOrdinal("quantity")),
            TotalPrice = r.GetDecimal(r.GetOrdinal("total_price")),
            Status = r.GetString(r.GetOrdinal("status")),
            CreatedAt = r.GetDateTime(r.GetOrdinal("created_at")),
            UpdatedAt = r.GetDateTime(r.GetOrdinal("updated_at")),
        };
    }
}