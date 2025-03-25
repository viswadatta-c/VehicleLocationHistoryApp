using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace LocationHistory
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable ExecuteQuery(string query)
        {
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // Handle SQL exceptions (e.g., log, throw, etc.)
                    Console.WriteLine($"SQL Error: {ex.Message}");
                    throw; // Rethrow to propagate the exception
                }
                catch (Exception ex)
                {
                    // Handle other exceptions (e.g., connection issues)
                    Console.WriteLine($"General Error: {ex.Message}");
                    throw;
                }
            }

            return dataTable;
        }

        // Add more methods for other database operations (e.g., ExecuteNonQuery, ExecuteScalar) as needed.
        public int ExecuteNonQuery(string query)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public object ExecuteScalar(string query)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    return command.ExecuteScalar();
                }
            }
        }
    }
}
