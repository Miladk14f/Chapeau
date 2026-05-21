using Chapeau.Models;
using Microsoft.Data.SqlClient;

namespace Chapeau.Repositories
{
    public class DBPaymentRepository : IPaymentRepository
    {
        private readonly string _connectionString;

        public DBPaymentRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("ChapeauDb");
        }

        public List<Payment> GetAll()
        {
            List<Payment> payments = new List<Payment>();

            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = "SELECT Id, BillId, payment_method, amount, status, PaidAt FROM PAYMENT";

            using SqlCommand cmd = new SqlCommand(query, connection);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                payments.Add(MapReader(reader));
            }

            return payments;
        }

        public List<Payment> GetByBillId(int billId)
        {
            List<Payment> payments = new List<Payment>();

            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = "SELECT Id, BillId, payment_method, amount, status, PaidAt FROM PAYMENT WHERE BillId = @BillId";

            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BillId", billId);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                payments.Add(MapReader(reader));
            }

            return payments;
        }

        public Payment GetById(int id)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = "SELECT Id, BillId, payment_method, amount, status, PaidAt FROM PAYMENT WHERE Id = @Id";

            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return MapReader(reader);
            }

            return null;
        }

        public void Add(Payment payment)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = @"INSERT INTO PAYMENT
                            (BillId, payment_method, amount, status, PaidAt)
                            VALUES
                            (@BillId, @PaymentMethod, @Amount, @Status, @PaidAt)";

            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BillId", payment.BillId);
            cmd.Parameters.AddWithValue("@PaymentMethod", payment.PaymentMethod);
            cmd.Parameters.AddWithValue("@Amount", payment.Amount);
            cmd.Parameters.AddWithValue("@Status", payment.Status);
            cmd.Parameters.AddWithValue("@PaidAt", payment.PaidAt);

            cmd.ExecuteNonQuery();
        }

        public void Update(Payment payment)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = @"UPDATE PAYMENT
                            SET BillId = @BillId,
                                payment_method = @PaymentMethod,
                                amount = @Amount,
                                status = @Status,
                                PaidAt = @PaidAt
                            WHERE Id = @Id";

            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BillId", payment.BillId);
            cmd.Parameters.AddWithValue("@PaymentMethod", payment.PaymentMethod);
            cmd.Parameters.AddWithValue("@Amount", payment.Amount);
            cmd.Parameters.AddWithValue("@Status", payment.Status);
            cmd.Parameters.AddWithValue("@PaidAt", payment.PaidAt);
            cmd.Parameters.AddWithValue("@Id", payment.Id);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = "DELETE FROM PAYMENT WHERE Id = @Id";

            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            cmd.ExecuteNonQuery();
        }

        private Payment MapReader(SqlDataReader reader)
        {
            return new Payment
            {
                Id = (int)reader["Id"],
                BillId = (int)reader["BillId"],
                PaymentMethod = reader["payment_method"].ToString(),
                Amount = (decimal)reader["amount"],
                Status = reader["status"].ToString(),
                PaidAt = reader["PaidAt"] == DBNull.Value ? null : (DateTime)reader["PaidAt"]
            };
        }
    }
}
