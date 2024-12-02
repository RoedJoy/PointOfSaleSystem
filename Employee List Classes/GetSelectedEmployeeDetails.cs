using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyPos.Connections;

namespace MyPos.Employee_List_Classes
{
    public class GetSelectedEmployeeDetails
    {
        public async Task<DataTable> GetEmployeeDetailsAsync(int userID)
        {
            try
            {
                string query = @"
            SELECT UserID, FirstName, MiddleName, LastName, Age, Gender, 
                   Address, BirthDate, Username, Password, DateCreated, DateUpdated
            FROM Cashiers
            WHERE UserID = @UserID";

                using (SqlConnection conn = new SqlConnection(Connection.ConnectionString()))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        await Task.Run(() => adapter.Fill(dt));  // Fill DataTable on a background thread
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }
}
