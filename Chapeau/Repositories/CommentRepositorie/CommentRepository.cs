using Chapeau.Models;
using Microsoft.Data.SqlClient;

namespace Chapeau.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly string _connectionString;

        public CommentRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("ChapeauDb");
        }

        public List<Comment> GetByOrderId(int orderId)
        {
            List<Comment> comments = new List<Comment>();

            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "SELECT Id, OrderId, Type, Text, CreatedAt FROM COMMENT WHERE OrderId = @OrderId";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@OrderId", orderId);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
                comments.Add(MapReader(reader));

            return comments;
        }

        public void Add(Comment comment)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "INSERT INTO COMMENT (OrderId, Type, Text, CreatedAt) VALUES (@OrderId, @Type, @Text, @CreatedAt)";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@OrderId",   comment.OrderId);
            cmd.Parameters.AddWithValue("@Type",      (object?)comment.Type ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Text",      (object?)comment.Text ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedAt", comment.CreatedAt);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "DELETE FROM COMMENT WHERE Id = @Id";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            cmd.ExecuteNonQuery();
        }

        private Comment MapReader(SqlDataReader reader)
        {
            return new Comment(
                id:        (int)reader["Id"],
                orderId:   (int)reader["OrderId"],
                type:      reader["Type"]      == DBNull.Value ? null : reader["Type"].ToString(),
                text:      reader["Text"]      == DBNull.Value ? null : reader["Text"].ToString(),
                createdAt: (DateTime)reader["CreatedAt"]
            );
        }
    }
}
