using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Chapeau.Models;

namespace Chapeau.Repositories
{
    public class StockItemRepository : IStockItemRepository
    {
        private readonly string _connectionString = "Server=tcp:sweeden2.database.windows.net,1433;Initial Catalog=Chapeau;Persist Security Info=False;User ID=sweed2;Password=Inholland@14;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public List<StockItem> GetAllStockItems()
        {
            List<StockItem> items = new List<StockItem>();
            string query = "SELECT Id, Name, Quantity, MaxQuantity FROM STOCK_ITEM";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            StockItem item = new StockItem
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString(),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                MaxQuantity = Convert.ToInt32(reader["MaxQuantity"])
                            };
                            items.Add(item);
                        }
                    }
                }
            }
            return items;
        }

        public void UpdateStockQuantity(int itemId, int newQuantity)
        {
            string query = "UPDATE STOCK_ITEM SET Quantity = @Quantity WHERE Id = @Id";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Quantity", newQuantity);
                    command.Parameters.AddWithValue("@Id", itemId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}