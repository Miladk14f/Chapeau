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

        public List<OrderItem> GetAllOrderItems()
        {
            var list = new List<OrderItem>();
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand(
                "SELECT Id, OrderId, MenuId, Name, Qty, Price, Vat, ItemType, CreatedAt, Status, Note FROM ORDER_ITEM", conn);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapReader(reader));
            return list;
        }

        public List<OrderItem> GetOrderItemsByOrderId(int orderId)
        {
            var list = new List<OrderItem>();
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand(
                "SELECT Id, OrderId, MenuId, Name, Qty, Price, Vat, ItemType, CreatedAt, Status, Note FROM ORDER_ITEM WHERE OrderId = @OrderId", conn);
            cmd.Parameters.AddWithValue("@OrderId", orderId);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapReader(reader));
            return list;
        }

        List<OrderItem> IOrderItemRepository.GetOrderItemsByTableId(int tableId)
        {
            List<OrderItem> list = new List<OrderItem>();
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand(
                @"SELECT oi.Id, oi.OrderId, oi.MenuId, oi.Name, oi.Qty, oi.Price, oi.Vat, oi.ItemType, oi.CreatedAt, oi.Status, oi.Note
          FROM ORDER_ITEM oi
          INNER JOIN [ORDER] o ON oi.OrderId = o.Id
          WHERE o.TableId = @TableId AND o.Status != 'paid'", conn);
            cmd.Parameters.AddWithValue("@TableId", tableId);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapReader(reader));

            return list;
        }

        public OrderItem GetOrderItemById(int id)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand(
                "SELECT Id, OrderId, MenuId, Name, Qty, Price, Vat, ItemType, CreatedAt, Status, Note FROM ORDER_ITEM WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using SqlDataReader reader = cmd.ExecuteReader();
            return reader.Read() ? MapReader(reader) : null;
        }

        public void AddOrderItem(OrderItem item)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand(
                "INSERT INTO ORDER_ITEM (OrderId, MenuId, Name, Qty, Price, Vat, ItemType, CreatedAt, Status, Note) VALUES (@OrderId, @MenuId, @Name, @Qty, @Price, @Vat, @ItemType, @CreatedAt, @Status, @Note)", conn);
            cmd.Parameters.AddWithValue("@OrderId", item.Order?.OrderId ?? 0);
            cmd.Parameters.AddWithValue("@MenuId", item.MenuItem?.MenuItemId ?? 0);
            cmd.Parameters.AddWithValue("@Name", item.Name);
            cmd.Parameters.AddWithValue("@Qty", item.Qty);
            cmd.Parameters.AddWithValue("@Price", item.Price);
            cmd.Parameters.AddWithValue("@Vat", item.Vat);
            cmd.Parameters.AddWithValue("@ItemType", item.ItemType.ToString().ToLower());
            cmd.Parameters.AddWithValue("@CreatedAt", item.CreatedAt);
            cmd.Parameters.AddWithValue("@Status", item.Status.ToString().ToLower());
            cmd.Parameters.AddWithValue("@Note", (object)item.Note ?? DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        public void UpdateOrderItem(OrderItem item)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand(
                "UPDATE ORDER_ITEM SET OrderId=@OrderId, MenuId=@MenuId, Name=@Name, Qty=@Qty, Price=@Price, Vat=@Vat, ItemType=@ItemType WHERE Id=@Id", conn);
            cmd.Parameters.AddWithValue("@OrderId", item.Order?.OrderId ?? 0);
            cmd.Parameters.AddWithValue("@MenuId", item.MenuItem?.MenuItemId ?? 0);
            cmd.Parameters.AddWithValue("@Name", item.Name);
            cmd.Parameters.AddWithValue("@Qty", item.Qty);
            cmd.Parameters.AddWithValue("@Price", item.Price);
            cmd.Parameters.AddWithValue("@Vat", item.Vat);
            cmd.Parameters.AddWithValue("@ItemType", item.ItemType.ToString().ToLower());
            cmd.Parameters.AddWithValue("@Id", item.OrderItemId);
            cmd.ExecuteNonQuery();
        }

        public void UpdateOrderItemStatus(int id, OrderItemStatus status)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand("UPDATE ORDER_ITEM SET Status = @Status WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Status", status.ToString().ToLower());
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        public void UpdateOrderItemNote(int id, string note)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand("UPDATE ORDER_ITEM SET Note = @Note WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Note", (object)note ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        public void MarkOrderItemsReady(int orderId, ItemType type)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand(
                "UPDATE ORDER_ITEM SET Status = 'ready' WHERE OrderId = @OrderId AND ItemType = @Type AND Status = 'ordered'", conn);
            cmd.Parameters.AddWithValue("@OrderId", orderId);
            cmd.Parameters.AddWithValue("@Type", type.ToString().ToLower());
            cmd.ExecuteNonQuery();
        }

        public void DeleteOrderItem(int id)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand("DELETE FROM ORDER_ITEM WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        public void DeleteOrderItemsByOrderId(int orderId)
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
                reader["Name"].ToString(),
                (int)reader["Qty"],
                (decimal)reader["Price"],
                (int)reader["Vat"],
                Enum.Parse<ItemType>(reader["ItemType"] == DBNull.Value ? "Food" : reader["ItemType"].ToString(), ignoreCase: true)
            )
            {
                Order = new Order { OrderId = (int)reader["OrderId"] },
                MenuItem = new MenuItem { MenuItemId = (int)reader["MenuId"] },
                CreatedAt = (DateTime)reader["CreatedAt"],
                Status = Enum.Parse<OrderItemStatus>(reader["Status"] == DBNull.Value ? "Ordered" : reader["Status"].ToString(), ignoreCase: true),
                Note = reader["Note"] == DBNull.Value ? null : reader["Note"].ToString()
            };
    }
}
