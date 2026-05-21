using Chapeau.Models;
using Microsoft.Data.SqlClient;

namespace Chapeau.Repositories.BillRepository
{
    public class DBBillRepository : IBillRepository
    {
        private readonly string _connectionString;

        public DBBillRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("ChapeauDb");
        }

        public List<Bill> GetAll()
        {
            List<Bill> bills = new List<Bill>();

            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = "SELECT Id, OrderId, Tip, split_method, amount, status FROM BILL";

            using SqlCommand cmd = new SqlCommand(query, connection);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                bills.Add(MapReader(reader));
            }

            return bills;
        }

        public Bill GetById(int id)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = "SELECT Id, OrderId, Tip, split_method, amount, status FROM BILL WHERE Id = @Id";

            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return MapReader(reader);
            }

            return null;
        }

        public Bill GetByOrderId(int orderId)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = "SELECT Id, OrderId, Tip, split_method, amount, status FROM BILL WHERE OrderId = @OrderId";

            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@OrderId", orderId);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return MapReader(reader);
            }

            return null;
        }

        public void Add(Bill bill)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = @"INSERT INTO BILL 
                            (OrderId, Tip, split_method, amount, status)
                            VALUES
                            (@OrderId, @Tip, @SplitMethod, @Amount, @Status)";

            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@OrderId", bill.OrderId);
            cmd.Parameters.AddWithValue("@Tip", bill.Tip);
            cmd.Parameters.AddWithValue("@SplitMethod", bill.SplitedMethod);
            cmd.Parameters.AddWithValue("@Amount", bill.Amount);
            cmd.Parameters.AddWithValue("@Status", bill.Status);

            cmd.ExecuteNonQuery();
        }

        public void Update(Bill bill)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = @"UPDATE BILL
                            SET OrderId = @OrderId,
                                Tip = @Tip,
                                split_method = @SplitMethod,
                                amount = @Amount,
                                status = @Status
                            WHERE Id = @Id";

            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@OrderId", bill.OrderId);
            cmd.Parameters.AddWithValue("@Tip", bill.Tip);
            cmd.Parameters.AddWithValue("@SplitMethod", bill.SplitedMethod);
            cmd.Parameters.AddWithValue("@Amount", bill.Amount);
            cmd.Parameters.AddWithValue("@Status", bill.Status);
            cmd.Parameters.AddWithValue("@Id", bill.Id);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = "DELETE FROM BILL WHERE Id = @Id";

            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            cmd.ExecuteNonQuery();
        }

        private Bill MapReader(SqlDataReader reader)
        {
            return new Bill
            {
                Id = (int)reader["Id"],
                OrderId = (int)reader["OrderId"],
                Tip = (decimal)reader["Tip"],
                SplitedMethod = reader["split_method"].ToString(),
                Amount = (decimal)reader["amount"],
                Status = reader["status"].ToString()
            };
        }
    }
}