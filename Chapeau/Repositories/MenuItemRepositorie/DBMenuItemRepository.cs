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
                "SELECT Id, Name, Description, Category, SubCategory, Price, Vat, Allergens, InStock FROM MENU_ITEM", conn);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapReader(reader));
            return list;
        }

        public MenuItem GetMenuItemById(int id)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand(
                "SELECT Id, Name, Description, Category, SubCategory, Price, Vat, Allergens, InStock FROM MENU_ITEM WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using SqlDataReader reader = cmd.ExecuteReader();
            return reader.Read() ? MapReader(reader) : null;
        }

        private MenuItem MapReader(SqlDataReader reader) =>
            new MenuItem(
                (int)reader["Id"],
                reader["Name"].ToString(),
                reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                reader["Category"] == DBNull.Value ? null : reader["Category"].ToString(),
                reader["SubCategory"] == DBNull.Value ? null : reader["SubCategory"].ToString(),
                (decimal)reader["Price"],
                (int)reader["Vat"],
                reader["Allergens"] == DBNull.Value ? null : reader["Allergens"].ToString(),
                (bool)reader["InStock"]
            );
    }
}