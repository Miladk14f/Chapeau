using Chapeau.Models;
using Microsoft.Data.SqlClient;

namespace Chapeau.Repositories
{
    public class StaffRepository : IStaffRepository
    {
        private readonly string _connectionString;

        public StaffRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("ChapeauDb");
        }

        public List<Staff> GetAll()
        {
            List<Staff> staffList = new List<Staff>();

            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "SELECT Id, Name, Role, Pin FROM STAFF";

            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
                staffList.Add(MapReader(reader));

            return staffList;
        }

        public Staff GetById(int id)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "SELECT Id, Name, Role, Pin FROM STAFF WHERE Id = @Id";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
                return MapReader(reader);

            return null;
        }

        public Staff GetByCredentials(string name, string pin)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "SELECT Id, Name, Role, Pin FROM STAFF WHERE Name = @Name AND Pin = @Pin";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.Parameters.AddWithValue("@Pin",  pin);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
                return MapReader(reader);

            return null;
        }

        public void Add(Staff staff)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "INSERT INTO STAFF (Name, Role, Pin) VALUES (@Name, @Role, @Pin)";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Name", staff.Name);
            cmd.Parameters.AddWithValue("@Role", staff.Role);
            cmd.Parameters.AddWithValue("@Pin",  staff.Pin);

            cmd.ExecuteNonQuery();
        }

        public void Update(Staff staff)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "UPDATE STAFF SET Name = @Name, Role = @Role, Pin = @Pin WHERE Id = @Id";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Name", staff.Name);
            cmd.Parameters.AddWithValue("@Role", staff.Role);
            cmd.Parameters.AddWithValue("@Pin",  staff.Pin);
            cmd.Parameters.AddWithValue("@Id",   staff.Id);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "DELETE FROM STAFF WHERE Id = @Id";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            cmd.ExecuteNonQuery();
        }

        private Staff MapReader(SqlDataReader reader)
        {
            return new Staff(
                id:   (int)reader["Id"],
                name: reader["Name"].ToString(),
                role: reader["Role"].ToString(),
                pin:  reader["Pin"].ToString()
            );
        }
    }
}
