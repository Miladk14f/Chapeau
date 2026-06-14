using Chapeau.Models;
using Chapeau.Models.Enums;
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

        public List<Bill> GetAllBills()
        {
            List<Bill> bills = new List<Bill>();

            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = "SELECT Id, OrderId, Tip, splited_method, amount, status FROM BILL";

            using SqlCommand cmd = new SqlCommand(query, connection);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                bills.Add(MapReader(reader));
            }

            return bills;
        }

        public Bill GetBillById(int id)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = "SELECT Id, OrderId, Tip, splited_method, amount, status FROM BILL WHERE Id = @Id";

            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return MapReader(reader);
            }

            return null;
        }

        public Bill GetBillByOrderId(int orderId)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = "SELECT Id, OrderId, Tip, splited_method, amount, status FROM BILL WHERE OrderId = @OrderId";

            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@OrderId", orderId);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return MapReader(reader);
            }

            return null;
        }

        public void AddBill(Bill bill)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = @"INSERT INTO BILL
                            (OrderId, Tip, splited_method, amount, status)
                            VALUES
                            (@OrderId, @Tip, @SplitMethod, @Amount, @Status)";

            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@OrderId", bill.Order?.OrderId ?? 0);
            cmd.Parameters.AddWithValue("@Tip", bill.Tip);
            cmd.Parameters.AddWithValue("@SplitMethod", bill.SplitedMethod.ToString().ToLower());
            cmd.Parameters.AddWithValue("@Amount", bill.Amount);
            cmd.Parameters.AddWithValue("@Status", bill.Status.ToString().ToLower());

            cmd.ExecuteNonQuery();
        }

        public void UpdateBill(Bill bill)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = @"UPDATE BILL
                            SET OrderId = @OrderId,
                                Tip = @Tip,
                                splited_method = @SplitMethod,
                                amount = @Amount,
                                status = @Status
                            WHERE Id = @Id";

            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@OrderId", bill.Order?.OrderId ?? 0);
            cmd.Parameters.AddWithValue("@Tip", bill.Tip);
            cmd.Parameters.AddWithValue("@SplitMethod", bill.SplitedMethod.ToString().ToLower());
            cmd.Parameters.AddWithValue("@Amount", bill.Amount);
            cmd.Parameters.AddWithValue("@Status", bill.Status.ToString().ToLower());
            cmd.Parameters.AddWithValue("@Id", bill.BillId);

            cmd.ExecuteNonQuery();
        }

        public void DeleteBill(int id)
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
            return new Bill(
                billId: (int)reader["Id"],
                tip: (decimal)reader["Tip"],
                splitedMethod: Enum.Parse<ESplitMethod>(reader["splited_method"] == DBNull.Value ? "None" : reader["splited_method"].ToString(), ignoreCase: true),
                amount: (decimal)reader["amount"],
                status: Enum.Parse<EBillStatus>(reader["status"] == DBNull.Value ? "Unpaid" : reader["status"].ToString(), ignoreCase: true)
            )
            {
                Order = new Order { OrderId = (int)reader["OrderId"] }
            };
        }
    }
}
