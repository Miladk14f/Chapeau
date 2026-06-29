using Chapeau.Models;
using Chapeau.Models.Enums;
using Microsoft.Data.SqlClient;

namespace Chapeau.Repositories
{
    public class DBCommentRepository : ICommentRepository
    {
        private readonly string _connectionString;

        public DBCommentRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("ChapeauDb");
        }

        public List<Comment> GetAllComments()
        {
            List<Comment> comments = new List<Comment>();

            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "SELECT Id, OrderId, Type, Text, CreatedAt FROM COMMENT ORDER BY CreatedAt DESC";

            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
                comments.Add(MapReader(reader));

            return comments;
        }

        public List<Comment> GetCommentsByOrderId(int orderId)
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

        public void AddComment(Comment comment)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "INSERT INTO COMMENT (OrderId, Type, Text, CreatedAt) VALUES (@OrderId, @Type, @Text, @CreatedAt)";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@OrderId", comment.Order?.OrderId ?? 0);
            cmd.Parameters.AddWithValue("@Type", comment.Type.ToString().ToLower());
            cmd.Parameters.AddWithValue("@Text", (object?)comment.Text ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedAt", comment.CreatedAt);

            cmd.ExecuteNonQuery();
        }

        public void DeleteComment(int id)
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
                commentId: (int)reader["Id"],
                type: Enum.Parse<CommentType>(reader["Type"] == DBNull.Value ? "Comment" : reader["Type"].ToString(), ignoreCase: true),
                text: reader["Text"] == DBNull.Value ? null : reader["Text"].ToString(),
                createdAt: (DateTime)reader["CreatedAt"]
            )
            {
                Order = new Order { OrderId = (int)reader["OrderId"] }
            };
        }
    }
}
