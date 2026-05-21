using Chapeau.Models;
using Chapeau.Models.Enums;
using Microsoft.Data.SqlClient;

namespace Chapeau.Repositories
{
    public class DBRestaurantTableRepository : IRestaurantTableRepository
    {
        private readonly string _connectionString;

        public DBRestaurantTableRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("ChapeauDb");
        }

        public List<RestaurantTable> GetAllTables()
        {
            List<RestaurantTable> tables = new List<RestaurantTable>();

            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "SELECT Id, Seats, Guests, Status, SeatedAt, WaiterId, ReservationName FROM [TABLE]";

            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                tables.Add(MapReader(reader));
            }

            return tables;
        }

        public RestaurantTable GetTableById(int id)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "SELECT Id, Seats, Guests, Status, SeatedAt, WaiterId, ReservationName FROM [TABLE] WHERE Id = @Id";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
                return MapReader(reader);

            return null;
        }

        public void SeatGuestsAtTable(int tableId, int guests, int waiterId)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = @"UPDATE [TABLE]
                             SET Guests = @Guests, WaiterId = @WaiterId,
                                 Status = 'occupied', SeatedAt = GETDATE()
                             WHERE Id = @Id";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Guests", guests);
            cmd.Parameters.AddWithValue("@WaiterId", waiterId);
            cmd.Parameters.AddWithValue("@Id", tableId);

            cmd.ExecuteNonQuery();
        }

        public void ClearTable(int tableId)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = @"UPDATE [TABLE]
                             SET Guests = NULL, WaiterId = NULL,
                                 Status = 'free', SeatedAt = NULL, ReservationName = NULL
                             WHERE Id = @Id";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", tableId);

            cmd.ExecuteNonQuery();
        }

        public void UpdateTableStatus(int tableId, ETableStatus status)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "UPDATE [TABLE] SET Status = @Status WHERE Id = @Id";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Status", status.ToString().ToLower());
            cmd.Parameters.AddWithValue("@Id", tableId);

            cmd.ExecuteNonQuery();
        }

        private RestaurantTable MapReader(SqlDataReader reader)
        {
            return new RestaurantTable(
                id: (int)reader["Id"],
                seats: (int)reader["Seats"],
                guests: reader["Guests"] == DBNull.Value ? null : (int?)reader["Guests"],
                status: Enum.Parse<ETableStatus>(reader["Status"] == DBNull.Value ? "Free" : reader["Status"].ToString(), ignoreCase: true),
                seatedAt: reader["SeatedAt"] == DBNull.Value ? null : (DateTime?)reader["SeatedAt"],
                waiterId: reader["WaiterId"] == DBNull.Value ? null : (int?)reader["WaiterId"],
                reservationName: reader["ReservationName"] == DBNull.Value ? null : reader["ReservationName"].ToString()
            );
        }
    }
}
