using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyPos.Connections;

namespace MyPos.Employee_List_Classes
{
    public class EmployeeDataService
    {
        public DataTable GetPaginatedEmployees(int currentPage, int pageSize)
        {
            int offset = (currentPage - 1) * pageSize;

            string query = $@"
            SELECT * 
            FROM Cashiers
            ORDER BY UserID
            OFFSET {offset} ROWS
            FETCH NEXT {pageSize} ROWS ONLY;
        ";

            using (SqlConnection conn = new SqlConnection(Connection.ConnectionString()))
            {
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        public int GetTotalEmployeeCount()
        {
            string countQuery = "SELECT COUNT(*) FROM Cashiers";

            using (SqlConnection conn = new SqlConnection(Connection.ConnectionString()))
            {
                conn.Open();
                SqlCommand countCommand = new SqlCommand(countQuery, conn);
                return (int)countCommand.ExecuteScalar();
            }
        }
    }
}
