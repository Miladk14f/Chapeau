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



        public Order GetActiveOrderByTableId(int tableId)
        {

            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "SELECT Id, TableId, StaffId, Status, Note, CreatedAt, total_price FROM [ORDER] WHERE TableId = @TableId AND (Status = 'pending' OR Status = 'inprogress')";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@TableId", tableId);

            using SqlDataReader reader = cmd.ExecuteReader();


            if (reader.Read())
            {
                return MapReader(reader);
            }

            return null;
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

        public void DeleteCommentsByOrderId(int orderId)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand("DELETE FROM COMMENT WHERE OrderId = @OrderId", conn);
            cmd.Parameters.AddWithValue("@OrderId", orderId);
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

        public List<OrderItem> GetAllOrderItems()
        {
            var list = new List<OrderItem>();
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand(
                "SELECT Id, OrderId, MenuId, Name, Qty, Price, Vat, ItemType, CreatedAt, Status, Note FROM ORDER_ITEM", conn);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapOrderItem(reader));
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
            while (reader.Read()) list.Add(MapOrderItem(reader));
            return list;
        }

        public List<OrderItem> GetOrderItemsByTableId(int tableId)
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
            while (reader.Read()) list.Add(MapOrderItem(reader));

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
            return reader.Read() ? MapOrderItem(reader) : null;
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

        public void UpdateOrderItemsStatusByType(int orderId, SubCategory[] types, OrderItemStatus fromStatus, OrderItemStatus toStatus)
        {
            string inList = string.Join(",", types.Select(t => $"'{t.ToString().ToLower()}'"));
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand(
                $"UPDATE ORDER_ITEM SET Status = @ToStatus WHERE OrderId = @OrderId AND ItemType IN ({inList}) AND Status = @FromStatus", conn);
            cmd.Parameters.AddWithValue("@ToStatus", toStatus.ToString().ToLower());
            cmd.Parameters.AddWithValue("@OrderId", orderId);
            cmd.Parameters.AddWithValue("@FromStatus", fromStatus.ToString().ToLower());
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

        private static SubCategory ParseItemType(object val)
        {
            string s = (val == DBNull.Value ? "" : val.ToString()).Trim().ToLower();
            return s switch
            {
                "food" => SubCategory.Starters,
                "drink" => SubCategory.Beer,
                _ => Enum.TryParse<SubCategory>(s, ignoreCase: true, out var t) ? t : SubCategory.Starters
            };
        }

        private OrderItem MapOrderItem(SqlDataReader reader) =>
            new OrderItem(
                (int)reader["Id"],
                reader["Name"].ToString(),
                (int)reader["Qty"],
                (decimal)reader["Price"],
                (int)reader["Vat"],
                ParseItemType(reader["ItemType"])
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
