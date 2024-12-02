using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPos.Stocks_Classes
{
    using System;
    using System.Data.SqlClient;
    using MyPos.Connections;

    public class PaginationHelper
    {
        // Method to get the total count of records
        public int GetTotalCount()
        {
            int totalCount = 0;
            string query = "SELECT COUNT(*) FROM Products";  // Replace 'Stocks' with your actual table name

            using (SqlConnection connection = new SqlConnection(Connection.ConnectionString()))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                totalCount = (int)command.ExecuteScalar();  // Executes query and returns the first column of the first row
            }

            return totalCount;
        }
    }
}
