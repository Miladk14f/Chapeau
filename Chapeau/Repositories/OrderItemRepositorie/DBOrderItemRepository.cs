using Chapeau.Models;
using Chapeau.Models.Enums;
using Microsoft.Data.SqlClient;

namespace Chapeau.Repositories
{
    public class DBOrderItemRepository : IOrderItemRepository
    {
        private readonly string _connectionString;

        public DBOrderItemRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("ChapeauDb");
        }

        public List<OrderItem> GetByOrderId(int orderId)
        {
            var list = new List<OrderItem>();
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand(
                "SELECT Id, OrderId, MenuId, Name, Qty, Price, Vat, ItemType FROM ORDER_ITEM WHERE OrderId = @OrderId", conn);
            cmd.Parameters.AddWithValue("@OrderId", orderId);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapReader(reader));
            return list;
        }

        public OrderItem GetById(int id)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand(
                "SELECT Id, OrderId, MenuId, Name, Qty, Price, Vat, ItemType FROM ORDER_ITEM WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using SqlDataReader reader = cmd.ExecuteReader();
            return reader.Read() ? MapReader(reader) : null;
        }

        public void Add(OrderItem item)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand(
                "INSERT INTO ORDER_ITEM (OrderId, MenuId, Name, Qty, Price, Vat, ItemType) VALUES (@OrderId, @MenuId, @Name, @Qty, @Price, @Vat, @ItemType)", conn);
            cmd.Parameters.AddWithValue("@OrderId", item.OrderId);
            cmd.Parameters.AddWithValue("@MenuId", item.MenuId);
            cmd.Parameters.AddWithValue("@Name", item.Name);
            cmd.Parameters.AddWithValue("@Qty", item.Qty);
            cmd.Parameters.AddWithValue("@Price", item.Price);
            cmd.Parameters.AddWithValue("@Vat", item.Vat);
            cmd.Parameters.AddWithValue("@ItemType", item.ItemType.ToString().ToLower());
            cmd.ExecuteNonQuery();
        }

        public void Update(OrderItem item)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand(
                "UPDATE ORDER_ITEM SET OrderId=@OrderId, MenuId=@MenuId, Name=@Name, Qty=@Qty, Price=@Price, Vat=@Vat, ItemType=@ItemType WHERE Id=@Id", conn);
            cmd.Parameters.AddWithValue("@OrderId", item.OrderId);
            cmd.Parameters.AddWithValue("@MenuId", item.MenuId);
            cmd.Parameters.AddWithValue("@Name", item.Name);
            cmd.Parameters.AddWithValue("@Qty", item.Qty);
            cmd.Parameters.AddWithValue("@Price", item.Price);
            cmd.Parameters.AddWithValue("@Vat", item.Vat);
            cmd.Parameters.AddWithValue("@ItemType", item.ItemType.ToString().ToLower());
            cmd.Parameters.AddWithValue("@Id", item.Id);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand("DELETE FROM ORDER_ITEM WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        public void DeleteByOrderId(int orderId)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand("DELETE FROM ORDER_ITEM WHERE OrderId = @OrderId", conn);
            cmd.Parameters.AddWithValue("@OrderId", orderId);
            cmd.ExecuteNonQuery();
        }

        private OrderItem MapReader(SqlDataReader reader) =>
            new OrderItem(
                (int)reader["Id"],
                (int)reader["OrderId"],
                (int)reader["MenuId"],
                reader["Name"].ToString(),
                (int)reader["Qty"],
                (decimal)reader["Price"],
                (int)reader["Vat"],
                Enum.Parse<EItemType>(reader["ItemType"] == DBNull.Value ? "Food" : reader["ItemType"].ToString(), ignoreCase: true)
            );
    }
}
