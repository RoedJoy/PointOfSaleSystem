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
    public class SearchEmployee
    {
        public async Task<DataTable> GetDataAsync(int pageNumber = 1, int pageSize = 40)
        {
            DataTable dataTable = new DataTable();

            try
            {
                int offset = (pageNumber - 1) * pageSize;
                string query = $@"
                SELECT 
                    UserID,
                    CONCAT(FirstName, 
                        CASE WHEN MiddleName IS NOT NULL AND MiddleName != '' THEN CONCAT(' ', MiddleName) ELSE '' END, 
                        ' ', LastName) AS FullName,
                    Age,
                    Gender,
                    Address,
                    BirthDate,
                    DateCreated,
                    DateUpdated
                FROM Cashiers
                ORDER BY UserID
                OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY;";

                using (SqlConnection conn = new SqlConnection(Connection.ConnectionString()))
                {
                    await conn.OpenAsync();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dataTable;
        }

        public void FilterData(DataGridView dgvEmployeeInfo, DataTable dataTable, string searchText)
        {
            dgvEmployeeInfo.AutoGenerateColumns = false;
            if (dataTable == null)
            {
                MessageBox.Show("DataTable is null. Cannot filter data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(searchText))
            {
                dgvEmployeeInfo.DataSource = dataTable; // Show all data
            }
            else
            {
                searchText = searchText.Trim().ToLower();
                var filteredData = dataTable.AsEnumerable()
                    .Where(row =>
                        (row.Field<string>("FullName")?.ToLower().Contains(searchText) == true) || // Check FullName
                        row.Field<int>("UserID").ToString().Contains(searchText) == true        // Check UserID
                    )
                    .ToList();

                if (filteredData.Any())
                {
                    DataTable filteredTable = dataTable.Clone(); // Create an empty table with the same schema

                    foreach (var row in filteredData)
                    {
                        filteredTable.ImportRow(row); // Add matching rows
                    }

                    dgvEmployeeInfo.DataSource = filteredTable; // Show the filtered data
                }
                else
                {
                    dgvEmployeeInfo.DataSource = null; // No matches: Clear the DataGridView
                }
            }

            // Set DataPropertyNames and Headers
            dgvEmployeeInfo.Columns["UserID"].DataPropertyName = "UserID";
            dgvEmployeeInfo.Columns["FullName"].DataPropertyName = "FullName";
            dgvEmployeeInfo.Columns["Age"].DataPropertyName = "Age";
            dgvEmployeeInfo.Columns["Gender"].DataPropertyName = "Gender";
            dgvEmployeeInfo.Columns["Address"].DataPropertyName = "Address";
            dgvEmployeeInfo.Columns["BirthDate"].DataPropertyName = "BirthDate";
            dgvEmployeeInfo.Columns["DateCreated"].DataPropertyName = "DateCreated";
            dgvEmployeeInfo.Columns["DateUpdated"].DataPropertyName = "DateUpdated";

            dgvEmployeeInfo.Columns["UserID"].HeaderText = "Cashier ID";
            dgvEmployeeInfo.Columns["FullName"].HeaderText = "Full Name";
            dgvEmployeeInfo.Columns["Age"].HeaderText = "Age";
            dgvEmployeeInfo.Columns["Gender"].HeaderText = "Gender";
            dgvEmployeeInfo.Columns["Address"].HeaderText = "Address";
            dgvEmployeeInfo.Columns["BirthDate"].HeaderText = "Birthday";
            dgvEmployeeInfo.Columns["DateCreated"].HeaderText = "Date Created";
            dgvEmployeeInfo.Columns["DateUpdated"].HeaderText = "Date Updated";
        }
    }
}
