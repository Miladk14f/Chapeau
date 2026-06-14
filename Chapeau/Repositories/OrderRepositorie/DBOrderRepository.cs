using Chapeau.Models;
using Chapeau.Models.Enums;
using Microsoft.Data.SqlClient;

namespace Chapeau.Repositories
{
    public class DBOrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        public DBOrderRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("ChapeauDb");
        }

        public List<Order> GetAllOrders()
        {
            List<Order> orders = new List<Order>();

            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "SELECT Id, TableId, StaffId, Status, Note, CreatedAt, total_price FROM [ORDER]";

            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
                orders.Add(MapReader(reader));

            return orders;
        }

        public List<Order> GetOrdersByTableId(int tableId)
        {
            List<Order> orders = new List<Order>();

            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "SELECT Id, TableId, StaffId, Status, Note, CreatedAt, total_price FROM [ORDER] WHERE TableId = @TableId";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@TableId", tableId);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
                orders.Add(MapReader(reader));

            return orders;
        }

        public List<Order> GetOrdersByStaffId(int staffId)
        {
            List<Order> orders = new List<Order>();

            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "SELECT Id, TableId, StaffId, Status, Note, CreatedAt, total_price FROM [ORDER] WHERE StaffId = @StaffId";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@StaffId", staffId);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
                orders.Add(MapReader(reader));

            return orders;
        }

        public Order GetOrderById(int orderId)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "SELECT Id, TableId, StaffId, Status, Note, CreatedAt, total_price FROM [ORDER] WHERE Id = @Id";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", orderId);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
                return MapReader(reader);

            return null;
        }

        public int AddOrder(Order order)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = @"INSERT INTO [ORDER] (TableId, StaffId, Status, Note, CreatedAt, total_price)
                             VALUES (@TableId, @StaffId, @Status, @Note, @CreatedAt, @TotalPrice);
                             SELECT SCOPE_IDENTITY();";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@TableId", order.Table?.TableId ?? 0);
            cmd.Parameters.AddWithValue("@StaffId", order.Staff?.StaffId ?? 0);
            cmd.Parameters.AddWithValue("@Status", order.Status.ToString().ToLower());
            cmd.Parameters.AddWithValue("@Note", (object?)order.Note ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedAt", order.CreatedAt);
            cmd.Parameters.AddWithValue("@TotalPrice", order.TotalPrice);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void UpdateOrder(Order order)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = @"UPDATE [ORDER]
                             SET TableId = @TableId, StaffId = @StaffId,
                                 Status = @Status, Note = @Note,
                                 total_price = @TotalPrice
                             WHERE Id = @Id";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@TableId", order.Table?.TableId ?? 0);
            cmd.Parameters.AddWithValue("@StaffId", order.Staff?.StaffId ?? 0);
            cmd.Parameters.AddWithValue("@Status", order.Status.ToString().ToLower());
            cmd.Parameters.AddWithValue("@Note", (object?)order.Note ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TotalPrice", order.TotalPrice);
            cmd.Parameters.AddWithValue("@Id", order.OrderId);

            cmd.ExecuteNonQuery();
        }

        public void UpdateOrderStatus(int orderId, OrderStatus status)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "UPDATE [ORDER] SET Status = @Status WHERE Id = @Id";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Status", status.ToString().ToLower());
            cmd.Parameters.AddWithValue("@Id", orderId);

            cmd.ExecuteNonQuery();
        }

        public void DeleteOrder(int orderId)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "DELETE FROM [ORDER] WHERE Id = @Id";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", orderId);

            cmd.ExecuteNonQuery();
        }

        private Order MapReader(SqlDataReader reader)
        {
            return new Order(
                orderId: (int)reader["Id"],
                status: Enum.Parse<OrderStatus>((reader["Status"] == DBNull.Value ? "Pending" : reader["Status"].ToString()).Replace("_", "").Replace(" ", ""), ignoreCase: true),
                note: reader["Note"] == DBNull.Value ? null : reader["Note"].ToString(),
                createdAt: (DateTime)reader["CreatedAt"],
                totalPrice: (decimal)reader["total_price"]
            )
            {
                Table = reader["TableId"] == DBNull.Value ? null : new RestaurantTable { TableId = (int)reader["TableId"] },
                Staff = reader["StaffId"] == DBNull.Value ? null : new Staff { StaffId = (int)reader["StaffId"] }
            };
        }
    }
}
