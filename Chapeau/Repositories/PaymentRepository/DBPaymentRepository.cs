using Chapeau.Models;
using Chapeau.Models.Enums;
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

        public List<Payment> GetAllPayments()
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

        public List<Payment> GetPaymentsByBillId(int billId)
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

        public Payment GetPaymentById(int id)
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

        public void AddPayment(Payment payment)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = @"INSERT INTO PAYMENT
                            (BillId, payment_method, amount, status, PaidAt)
                            VALUES
                            (@BillId, @PaymentMethod, @Amount, @Status, @PaidAt)";

            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BillId", payment.BillId);
            cmd.Parameters.AddWithValue("@PaymentMethod", payment.PaymentMethod.ToString().ToLower());
            cmd.Parameters.AddWithValue("@Amount", payment.Amount);
            cmd.Parameters.AddWithValue("@Status", payment.Status.ToString().ToLower());
            cmd.Parameters.AddWithValue("@PaidAt", payment.PaidAt);

            cmd.ExecuteNonQuery();
        }

        public void UpdatePayment(Payment payment)
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
            cmd.Parameters.AddWithValue("@PaymentMethod", payment.PaymentMethod.ToString().ToLower());
            cmd.Parameters.AddWithValue("@Amount", payment.Amount);
            cmd.Parameters.AddWithValue("@Status", payment.Status.ToString().ToLower());
            cmd.Parameters.AddWithValue("@PaidAt", payment.PaidAt);
            cmd.Parameters.AddWithValue("@Id", payment.Id);

            cmd.ExecuteNonQuery();
        }

        public void DeletePayment(int id)
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
                PaymentMethod = Enum.Parse<EPaymentMethod>(reader["payment_method"] == DBNull.Value ? "Cash" : reader["payment_method"].ToString(), ignoreCase: true),
                Amount = (decimal)reader["amount"],
                Status = Enum.Parse<EBillStatus>(reader["status"] == DBNull.Value ? "Unpaid" : reader["status"].ToString(), ignoreCase: true),
                PaidAt = reader["PaidAt"] == DBNull.Value ? null : (DateTime)reader["PaidAt"]
            };
        }
    }
}
