using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Chapeau.Models;

namespace Chapeau.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly string _connectionString = "Server=tcp:sweeden2.database.windows.net,1433;Initial Catalog=Chapeau;Persist Security Info=False;User ID=sweed2;Password=Inholland@14;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public List<Comment> GetCommentsByOrderIdAsync(int orderId)
        {
            List<Comment> comments = new List<Comment>();
            string query = "SELECT Id, OrderId, Type, Text, CreatedAt FROM COMMENT WHERE OrderId = @OrderId";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OrderId", orderId);
                    
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Comment comment = new Comment
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                OrderId = Convert.ToInt32(reader["OrderId"]),
                                Type = reader["Type"].ToString(),
                                Text = reader["Text"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            };
                            comments.Add(comment);
                        }
                    }
                }
            }
            return comments;
        }

        public void AddCommentAsync(Comment comment)
        {
            string query = "INSERT INTO COMMENT (OrderId, Type, Text, CreatedAt) VALUES (@OrderId, @Type, @Text, @CreatedAt)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OrderId", comment.OrderId);
                    command.Parameters.AddWithValue("@Type", comment.Type);
                    command.Parameters.AddWithValue("@Text", comment.Text);
                    command.Parameters.AddWithValue("@CreatedAt", comment.CreatedAt);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}