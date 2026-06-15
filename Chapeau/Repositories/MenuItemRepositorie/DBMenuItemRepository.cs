using Chapeau.Models;
using Microsoft.Data.SqlClient;

namespace Chapeau.Repositories
{
    public class DBMenuItemRepository : IMenuItemRepository
    {
        private readonly string _connectionString;

        public DBMenuItemRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("ChapeauDb");
        }

        public List<MenuItem> GetAllMenuItems()
        {
            var list = new List<MenuItem>();
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand(
                "SELECT Id, Name, Description, Category, Price, Vat, Allergens, InStock FROM MENU_ITEM", conn);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapReader(reader));
            return list;
        }

        public List<MenuItem> GetMenuItemsByCategory(string category)
        {
            var list = new List<MenuItem>();
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand(
                "SELECT Id, Name, Description, Category, Price, Vat, Allergens, InStock FROM MENU_ITEM WHERE Category = @Category", conn);
            cmd.Parameters.AddWithValue("@Category", category);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapReader(reader));
            return list;
        }

        public MenuItem GetMenuItemById(int id)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand(
                "SELECT Id, Name, Description, Category, Price, Vat, Allergens, InStock FROM MENU_ITEM WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using SqlDataReader reader = cmd.ExecuteReader();
            return reader.Read() ? MapReader(reader) : null;
        }

        public void AddMenuItem(MenuItem item)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand(
                "INSERT INTO MENU_ITEM (Name, Description, Category, Price, Vat, Allergens, InStock) VALUES (@Name, @Description, @Category, @Price, @Vat, @Allergens, @InStock)", conn);
            cmd.Parameters.AddWithValue("@Name", item.Name);
            cmd.Parameters.AddWithValue("@Description", (object?)item.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Category", (object?)item.Category ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Price", item.Price);
            cmd.Parameters.AddWithValue("@Vat", item.Vat);
            cmd.Parameters.AddWithValue("@Allergens", (object?)item.Allergens ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@InStock", item.InStock);
            cmd.ExecuteNonQuery();
        }

        public void UpdateMenuItem(MenuItem item)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand(
                "UPDATE MENU_ITEM SET Name=@Name, Description=@Description, Category=@Category, Price=@Price, Vat=@Vat, Allergens=@Allergens, InStock=@InStock WHERE Id=@Id", conn);
            cmd.Parameters.AddWithValue("@Name", item.Name);
            cmd.Parameters.AddWithValue("@Description", (object?)item.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Category", (object?)item.Category ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Price", item.Price);
            cmd.Parameters.AddWithValue("@Vat", item.Vat);
            cmd.Parameters.AddWithValue("@Allergens", (object?)item.Allergens ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@InStock", item.InStock);
            cmd.Parameters.AddWithValue("@Id", item.MenuItemId);
            cmd.ExecuteNonQuery();
        }

        public void DeleteMenuItem(int id)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand("DELETE FROM MENU_ITEM WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        public void UpdateMenuItemStock(int id, bool inStock)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand("UPDATE MENU_ITEM SET InStock = @InStock WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@InStock", inStock);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        private MenuItem MapReader(SqlDataReader reader) =>
            new MenuItem(
                (int)reader["Id"],
                reader["Name"].ToString(),
                reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                reader["Category"] == DBNull.Value ? null : reader["Category"].ToString(),
                (decimal)reader["Price"],
                (int)reader["Vat"],
                reader["Allergens"] == DBNull.Value ? null : reader["Allergens"].ToString(),
                (bool)reader["InStock"]
            );
    }
}
