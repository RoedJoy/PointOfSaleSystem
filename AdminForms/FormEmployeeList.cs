using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyPos.Connections;
using MyPos.Employee_List_Classes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MyPos.AdminForms
{
    public partial class FormEmployeeList : Form
    {
        private DataGridViewClass dgv;
        DataGridViewRow selectedRow;
        private Textboxes textboxes;
        private DataTable allData;
        private SettingPlaceholderClass placeholder;
        private SearchEmployee search;
        private EmployeeDataService dataService;
        private string placeholderText = "Name or Cashier ID";
        private int currentPage = 1; private int pageSize = 40; private int totalRecords = 0;

        public FormEmployeeList()
        {
            InitializeComponent();
            dgv = new DataGridViewClass();
            placeholder = new SettingPlaceholderClass();
            search = new SearchEmployee();
            dataService = new EmployeeDataService();
            textboxes = new Textboxes(
                txtFirstName, txtMiddleName, txtLastName,
                txtAge, txtGender, txtAddress, txtBirthday,
                txtUsername, txtPassword
            );
            txtboxSearch.GotFocus += placeholder.RemovePlaceholder;
            txtboxSearch.LostFocus += placeholder.AddPlaceholder;
        }
        private async void FormEmployeeList_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'myPosDataSet1.Cashiers' table. You can move, or remove it, as needed.
            //this.cashiersTableAdapter.Fill(this.myPosDataSet1.Cashiers);
            LoadTheme();
            LoadData();
            LoadDataNumbers();
            await dgv.PopulateDataGridView(dgvEmployees);
            textboxes.SetTextboxesReadOnly(true);
            panelFullDetails.Visible = false;
            placeholder.SetPlaceholder(txtboxSearch, placeholderText);
            btnDelete.FlatAppearance.BorderColor = ColorClass.primaryColor;
            bttnView.FlatAppearance.BorderColor = ColorClass.primaryColor;
            btnSave.FlatAppearance.BorderColor = ColorClass.primaryColor;
            btnEdit.FlatAppearance.BorderColor = ColorClass.primaryColor;
            txtDateCreated.ReadOnly = true;
            txtDateUpdated.ReadOnly = true;
            txtCashierID.ReadOnly = true;
        }
        private void LoadTheme()
        {
            foreach (Control btns in this.Controls)
            {
                if (btns.GetType() == typeof(System.Windows.Forms.Button))
                {
                    System.Windows.Forms.Button btn = (System.Windows.Forms.Button)btns;
                    btn.BackColor = ColorClass.primaryColor;
                    btn.ForeColor = Color.White;
                    btn.FlatAppearance.BorderColor = ColorClass.secondaryColor;
                }
            }
        }
        private async void txtboxSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtboxSearch.Text;

            // Run filtering logic in a background thread
            await Task.Run(() =>
            {
                // Ensure thread-safe call to the UI
                Invoke(new Action(() =>
                {
                    search.FilterData(dgvEmployees, allData, searchText);
                }));
            });
        }
        private async void bttnView_Click(object sender, EventArgs e)
        {
            textboxes.SetTextboxesReadOnly(true);
            GetSelectedEmployeeDetails select = new GetSelectedEmployeeDetails();
            if (dgvEmployees.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvEmployees.SelectedRows[0];
                int userID = Convert.ToInt32(selectedRow.Cells["UserID"].Value);

                // Fetch data asynchronously
                DataTable employeeDetails = await select.GetEmployeeDetailsAsync(userID);

                if (employeeDetails != null && employeeDetails.Rows.Count > 0)
                {
                    DataRow row = employeeDetails.Rows[0];

                    txtCashierID.Text = row["UserID"].ToString();
                    txtFirstName.Text = row["FirstName"].ToString();
                    txtMiddleName.Text = row["MiddleName"].ToString();
                    txtLastName.Text = row["LastName"].ToString();
                    txtAge.Text = row["Age"].ToString();
                    txtGender.Text = row["Gender"].ToString();
                    txtAddress.Text = row["Address"].ToString();
                    txtBirthday.Text = row["BirthDate"].ToString();
                    txtUsername.Text = row["Username"].ToString();
                    txtPassword.Text = row["Password"].ToString();
                    txtDateCreated.Text = row["DateCreated"].ToString();
                    txtDateUpdated.Text = row["DateUpdated"].ToString();

                    panelFullDetails.Visible = true;
                }
                else
                {
                    MessageBox.Show("No details found for the selected cashier.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void txtDateCreated_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Handle cell click event if needed
        }

        private void dgvEmployees_SelectionChanged(object sender, EventArgs e)
        {
            // If no row is selected, hide the panel
            if (dgvEmployees.SelectedRows.Count == 0)
            {
                panelFullDetails.Visible = false;
            }
            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    panelFullDetails.Visible = dgvEmployees.SelectedRows.Count > 0;
                }));
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            // Enable editing mode for the textboxes
            textboxes.SetTextboxesReadOnly(false);
        }
        private async void LoadData() {
            allData = await search.GetDataAsync();
            search.FilterData(dgvEmployees, allData, null);
            UpdatePaginationLabel();
        }
        private void LoadDataNumbers()
        {
            // Fetch data using the service class
            DataTable employeeData = dataService.GetPaginatedEmployees(currentPage, pageSize);
            int totalRecords = dataService.GetTotalEmployeeCount();

            // Bind to DataGridView
            //dgvEmployees.DataSource = employeeData;

            // Update the label
            int startRecord = ((currentPage - 1) * pageSize) + 1;
            int endRecord = Math.Min(currentPage * pageSize, totalRecords);
            lblPagination.Text = $"Displaying {startRecord} to {endRecord} out of {totalRecords}";

            // Update button states
            UpdateButtonStates(totalRecords);
        }
        private void UpdatePaginationLabel() {
            int startRecord = (currentPage - 1) * pageSize + 1;
            int endRecord = startRecord + dgvEmployees.Rows.Count - 1
                ; lblPagination.Text = $"Displaying {startRecord} to {endRecord} of {totalRecords}";
        }


        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (currentPage > 1) 
            { 
                currentPage--; 
                LoadDataNumbers();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            currentPage++;
            LoadDataNumbers();
        }
        private void UpdateButtonStates(int totalRecords)
        {
            // Calculate if there are more pages
            bool hasPreviousPage = currentPage > 1;
            bool hasNextPage = currentPage * pageSize < totalRecords;

            // Enable or disable buttons based on the conditions
            btnPrevious.Enabled = hasPreviousPage;
            btnNext.Enabled = hasNextPage;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Ensure a row is selected
            if (dgvEmployees.SelectedRows.Count > 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = dgvEmployees.SelectedRows[0];

                // Assume the primary key is in a column named "UserID"
                int userId = Convert.ToInt32(selectedRow.Cells["UserID"].Value);

                // Confirm deletion
                DialogResult result = MessageBox.Show(
                    "Are you sure you want to delete this record?",
                    "Confirm Deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    // Call the delete method
                    DeleteEmployee(userId);

                    // Refresh the data
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void DeleteEmployee(int userId)
        {
            string query = "DELETE FROM Cashiers WHERE UserID = @UserID"; // Ensure the column name matches your database

            using (SqlConnection conn = new SqlConnection(Connection.ConnectionString()))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Record deleted successfully.", "Deletion Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Get the selected row
                selectedRow = dgvEmployees.SelectedRows[0];
            }
            catch(Exception ex)
            {
                MessageBox.Show("Please select an account first to avoid this error: " + ex.Message);
                return;
            }

            // Assume the primary key is in a column named "UserID"
            int Id = Convert.ToInt32(selectedRow.Cells["UserID"].Value);

            using (SqlConnection conn = new SqlConnection(Connection.ConnectionString()))
            {
                string query = "UPDATE Cashiers SET FirstName = @FirstName, MiddleName = @MiddleName, LastName = @LastName, Age = @Age, Gender = @Gender, Address = @Address, BirthDate = @BirthDate, Username = @Username, Password = @Password, DateUpdated = GETDATE() WHERE UserID = @ID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (!int.TryParse(txtAge.Text, out int age))
                    {
                        MessageBox.Show("Please enter a valid input for age.");
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtMiddleName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text) || string.IsNullOrWhiteSpace(txtAge.Text) || string.IsNullOrWhiteSpace(txtGender.Text) || string.IsNullOrWhiteSpace(txtAddress.Text) || string.IsNullOrWhiteSpace(txtBirthday.Text) || string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        MessageBox.Show("Please fill in all the details.");
                        return;
                    }

                    cmd.Parameters.AddWithValue("@ID", Id);
                    cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                    cmd.Parameters.AddWithValue("@MiddleName", txtMiddleName.Text);
                    cmd.Parameters.AddWithValue("@LastName", txtLastName.Text);
                    cmd.Parameters.AddWithValue("@Age", txtAge.Text);
                    cmd.Parameters.AddWithValue("@Gender", txtGender.Text);
                    cmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                    cmd.Parameters.AddWithValue("@BirthDate", Convert.ToDateTime(txtBirthday.Text));
                    cmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@Password", txtPassword.Text);
                    //cmd.Parameters.AddWithValue("@DateUpdated", DateTime.Now);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Data updated successfully.");
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("There was a problem updating the data: " + ex.Message);
                    }
                }
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
