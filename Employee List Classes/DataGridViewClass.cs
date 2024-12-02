using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyPos.Connections;

namespace MyPos.Employee_List_Classes
{
    public class DataGridViewClass
    {
        public async Task<DataTable> PopulateDataGridView(DataGridView dataGridView)
        {
            DataTable dataTable = new DataTable();
            dataGridView.AutoGenerateColumns = false;
            try
            {
                string query = @"
            SELECT 
                UserID,
                CONCAT(FirstName, ' ', MiddleName, ' ', LastName) AS FullName,
                Age,
                Gender,
                Address,
                BirthDate,
                DateCreated,
                DateUpdated
            FROM Cashiers";

                using (SqlConnection conn = new SqlConnection(Connection.ConnectionString()))
                {
                    await conn.OpenAsync();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.Fill(dataTable);
                        // Bind to DataGridView
                        dataGridView.DataSource = dataTable;
                        
                        dataGridView.Columns["UserID"].DataPropertyName = "UserID";
                        dataGridView.Columns["FullName"].DataPropertyName = "FullName";
                        dataGridView.Columns["Age"].DataPropertyName = "Age";
                        dataGridView.Columns["Gender"].DataPropertyName = "Gender";
                        dataGridView.Columns["Address"].DataPropertyName = "Address";
                        dataGridView.Columns["BirthDate"].DataPropertyName = "BirthDate";
                        dataGridView.Columns["DateCreated"].DataPropertyName = "DateCreated";
                        dataGridView.Columns["DateUpdated"].DataPropertyName = "DateUpdated";

                        // Ensure columns are correctly set
                        dataGridView.Columns["UserID"].HeaderText = "Cashier ID";
                        dataGridView.Columns["FullName"].HeaderText = "Full Name";
                        dataGridView.Columns["Age"].HeaderText = "Age";
                        dataGridView.Columns["Gender"].HeaderText = "Gender";
                        dataGridView.Columns["Address"].HeaderText = "Address";
                        dataGridView.Columns["BirthDate"].HeaderText = "Birthday";
                        dataGridView.Columns["DateCreated"].HeaderText = "Date Created";
                        dataGridView.Columns["DateUpdated"].HeaderText = "Date Updated";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dataTable; // Return DataTable
        }
    }
}
