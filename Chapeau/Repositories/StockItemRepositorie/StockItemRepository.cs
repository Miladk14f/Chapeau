using Chapeau.Models;
using Microsoft.Data.SqlClient;

namespace Chapeau.Repositories
{
    public class StockItemRepository : IStockItemRepository
    {
        private readonly string _connectionString;

        public StockItemRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("ChapeauDb");
        }

        public List<StockItem> GetAll()
        {
            List<StockItem> items = new List<StockItem>();

            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "SELECT Id, Name, Quantity, MaxQuantity FROM STOCK_ITEM";

            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
                items.Add(MapReader(reader));

            return items;
        }

        public StockItem GetById(int id)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "SELECT Id, Name, Quantity, MaxQuantity FROM STOCK_ITEM WHERE Id = @Id";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
                return MapReader(reader);

            return null;
        }

        public void Add(StockItem item)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "INSERT INTO STOCK_ITEM (Name, Quantity, MaxQuantity) VALUES (@Name, @Quantity, @MaxQuantity)";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Name",        item.Name);
            cmd.Parameters.AddWithValue("@Quantity",    item.Quantity);
            cmd.Parameters.AddWithValue("@MaxQuantity", item.MaxQuantity);

            cmd.ExecuteNonQuery();
        }

        public void Update(StockItem item)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "UPDATE STOCK_ITEM SET Name = @Name, Quantity = @Quantity, MaxQuantity = @MaxQuantity WHERE Id = @Id";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Name",        item.Name);
            cmd.Parameters.AddWithValue("@Quantity",    item.Quantity);
            cmd.Parameters.AddWithValue("@MaxQuantity", item.MaxQuantity);
            cmd.Parameters.AddWithValue("@Id",          item.Id);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "DELETE FROM STOCK_ITEM WHERE Id = @Id";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            cmd.ExecuteNonQuery();
        }

        public void UpdateQuantity(int id, int quantity)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "UPDATE STOCK_ITEM SET Quantity = @Quantity WHERE Id = @Id";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Quantity", quantity);
            cmd.Parameters.AddWithValue("@Id",       id);

            cmd.ExecuteNonQuery();
        }

        private StockItem MapReader(SqlDataReader reader)
        {
            return new StockItem(
                id:          (int)reader["Id"],
                name:        reader["Name"].ToString(),
                quantity:    (int)reader["Quantity"],
                maxQuantity: (int)reader["MaxQuantity"]
            );
        }
    }
}
