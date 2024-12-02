using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyPos.Connections;
using MyPos.Stocks_Classes;

namespace MyPos.AdminForms
{
    public partial class FormStocks : Form

    {
        private int currentPage = 1;
        private int pageSize = 38;  // Number of items to display per page
        private PaginationHelper paginationHelper;
        private Timer timer;
        SqlConnection conn = new SqlConnection(Connection.ConnectionString());

        public FormStocks()
        {
            InitializeComponent();
            paginationHelper = new PaginationHelper();
            timer = new Timer();
            cbCategory.DropDown += cbCategory_DropDown;
            cbCategory.DropDownClosed += cbCategory_DropDownClosed;
        }

        private void FormStocks_Load(object sender, EventArgs e)
        {
            LoadTheme();
            LoadStocks();
            LoadCategories();
            LoadProducts();
            InitializeRealTimeUpdates();
        }
        private void LoadTheme()
        {
            foreach (Control btns in this.Controls)
            {
                if (btns.GetType() == typeof(Button))
                {
                    Button btn = (Button)btns;
                    btn.BackColor = ColorClass.primaryColor;
                    btn.ForeColor = Color.White;
                    btn.FlatAppearance.BorderColor = ColorClass.secondaryColor;
                }
            }
        }
        private void InitializeRealTimeUpdates()
        {
            // Initialize the timer if not already created
            if (timer == null)
            {
                timer = new Timer();
                timer.Interval = 200; // Set interval to 200 milliseconds
                timer.Tick += DisplayStocksTimer_Tick; // Attach the Tick event
            }

            // Start the timer
            if (!timer.Enabled)
            {
                timer.Start();
            }
        }
        public void StopRealTimeUpdates()
        {
            if (timer != null)
            {
                timer.Stop(); // Stop the timer
                //timer.Dispose(); // Release resources
                //timer = null;
            }
        }
        public void RestartRealTimeUpdates()
        {
            if (timer == null)
            {
                InitializeRealTimeUpdates();  // Initialize the timer if it's disposed
            }
            else
            {
                if (!timer.Enabled)
                {
                    timer.Start();  // Restart the timer if it's not running
                }
            }
        }
        private void LoadCategories()
        {
            ArrayList categories = new ArrayList();
            categories.Add("All");
            categories.Add("Baby Care");
            categories.Add("Beverages");
            categories.Add("Biscuits");
            categories.Add("Candies");
            categories.Add("Canned Goods");
            categories.Add("Condiments");
            categories.Add("Cooking Essentials");
            categories.Add("Dairy Products");
            categories.Add("Frozen Goods");
            categories.Add("Household Items");
            categories.Add("Noodles");
            categories.Add("Packaged Foods");
            categories.Add("Personal Care");
            categories.Add("Rice");
            categories.Add("Snacks");
            foreach(string category  in categories)
            {
                cbCategory.Items.Add(category);
                cbCategoryAdd.Items.Add(category);
            }
        }
        private void LoadProducts(string category = null)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.ConnectionString()))
                {
                    // SQL query to fetch product details including DateAdded and LastStockUpdate
                    string query = @"
                        SELECT 
                            p.ProductName,
                            p.Category,
                            p.Price,
                            p.Quantity,
                            p.DateAdded,
                            p.LastStockUpdate
                        FROM Products p";

                    // If a category is provided, filter by that category
                    if (!string.IsNullOrEmpty(category) && category != "All")
                    {
                        query += " WHERE p.Category = @Category";
                    }

                    SqlCommand cmd = new SqlCommand(query, conn);

                    // Add category as a parameter if filtering by category
                    if (!string.IsNullOrEmpty(category) && category != "All")
                    {
                        cmd.Parameters.AddWithValue("@Category", category);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvStocks.DataSource = dt;

                    // Customize column headers
                    dgvStocks.Columns["ProductName"].HeaderText = "Product Name";
                    dgvStocks.Columns["Category"].HeaderText = "Category";
                    dgvStocks.Columns["Price"].HeaderText = "Price";
                    dgvStocks.Columns["Quantity"].HeaderText = "Quantity";
                    dgvStocks.Columns["DateAdded"].HeaderText = "Date Added";
                    dgvStocks.Columns["LastStockUpdate"].HeaderText = "Last Stock Update";

                    // Hide the Barcode column if it exists
                    if (dgvStocks.Columns.Contains("Barcode"))
                    {
                        dgvStocks.Columns["Barcode"].Visible = false;
                    }

                    // Adjust column formatting (optional)
                    dgvStocks.Columns["Price"].DefaultCellStyle.Format = "C2"; // Format price as currency
                    dgvStocks.Columns["DateAdded"].DefaultCellStyle.Format = "yyyy-MM-dd";
                    dgvStocks.Columns["LastStockUpdate"].DefaultCellStyle.Format = "yyyy-MM-dd";

                    // Show message if no data found
                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("No product data found for this category.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading products: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadStocks()
        {
            try
            {
                int totalCount = paginationHelper.GetTotalCount(); // Total number of items in the database
                int skip = (currentPage - 1) * pageSize;           // Calculate the number of items to skip

                string query = $@"
                        SELECT ProductName, Category, Price, Quantity, DateAdded, LastStockUpdate
                        FROM Products
                        ORDER BY ProductName
                        OFFSET @Skip ROWS FETCH NEXT @PageSize ROWS ONLY";

                using (SqlConnection connection = new SqlConnection(Connection.ConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        // Add parameters to prevent SQL injection
                        cmd.Parameters.AddWithValue("@Skip", skip);
                        cmd.Parameters.AddWithValue("@PageSize", pageSize);

                        SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);

                        // Bind the data to the DataGridView
                        dgvStocks.DataSource = dataTable;
                    }
                }

                // Calculate the displayed range
                int startItem = Math.Min(skip + 1, totalCount);
                int endItem = Math.Min(skip + dgvStocks.RowCount, totalCount);

                // Update the label to show the range and total count
                lblItemCount.Text = $"Displaying {startItem} to {endItem} items out of {totalCount}";

                // Update pagination button states
                UpdatePaginationButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading stocks: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void UpdatePaginationButtons()
        {
            int totalCount = paginationHelper.GetTotalCount(); // Total items in the database

            // Disable the "Previous" button if on the first page
            btnPrevious.Enabled = currentPage > 1;

            // Disable the "Next" button if no more items are available after the current page
            btnNext.Enabled = (currentPage * pageSize) < totalCount;
        }

        private void btnNext_Click_1(object sender, EventArgs e)
        {
            if ((currentPage * pageSize) < paginationHelper.GetTotalCount())
            {
                currentPage++;
                LoadStocks();
            }
        }

        private void btnPrevious_Click_1(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadStocks();
            }
        }

        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected category
            string selectedCategory = cbCategory.SelectedItem?.ToString();

            // If "All" is selected or no selection, load all products
            if (string.IsNullOrEmpty(cbCategory.Text) || cbCategory.Text == "All")
            {
                LoadProducts();  // Load all products
            }
            else
            {
                LoadProducts(selectedCategory);  // Load products for the selected category
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }
        private void AddNewBarcode(string productName, string barcode, string category)
        {
            try
            {
                // Check if the barcode already exists
                string queryCheckBarcode = "SELECT COUNT(*) FROM Barcodes WHERE Barcode = @Barcode";
                using (SqlCommand cmd = new SqlCommand(queryCheckBarcode, conn))
                {
                    cmd.Parameters.AddWithValue("@Barcode", barcode);
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    conn.Close();

                    if (count > 0)
                    {
                        MessageBox.Show("The barcode already exists in the database.", "Duplicate Barcode", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Check if the product exists
                string queryGetProductID = "SELECT ProductID FROM Products WHERE ProductName = @ProductName AND Category = @Category";
                int productId;
                using (SqlCommand cmd = new SqlCommand(queryGetProductID, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductName", productName);
                    cmd.Parameters.AddWithValue("@Category", category);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    conn.Close();

                    if (result == null)
                    {
                        MessageBox.Show("The specified product does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    productId = (int)result;
                }

                // Insert the new barcode
                string queryInsertBarcode = "INSERT INTO Barcodes (ProductID, Barcode) VALUES (@ProductID, @Barcode)";
                using (SqlCommand cmd = new SqlCommand(queryInsertBarcode, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    cmd.Parameters.AddWithValue("@Barcode", barcode);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                MessageBox.Show("New barcode added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtProductName.Clear();
                txtBarcode.Clear();
                cbCategoryAdd.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void AddNewProduct(string productName, string barcode, string category, string priceText, string quantityText)
        {
            try
            {
                decimal price = decimal.Parse(priceText);
                int quantity = int.Parse(quantityText);

                // Check if the barcode already exists
                string queryCheckBarcode = "SELECT COUNT(*) FROM Barcodes WHERE Barcode = @Barcode";
                using (SqlCommand cmd = new SqlCommand(queryCheckBarcode, conn))
                {
                    cmd.Parameters.AddWithValue("@Barcode", barcode);
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    conn.Close();

                    if (count > 0)
                    {
                        MessageBox.Show("The barcode already exists in the database.", "Duplicate Barcode", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Insert the product into the Products table
                string queryInsertProduct = "INSERT INTO Products (ProductName, Category, Price, Quantity, DateAdded) OUTPUT INSERTED.ProductID VALUES (@ProductName, @Category, @Price, @Quantity, GETDATE())";
                int productId;
                using (SqlCommand cmd = new SqlCommand(queryInsertProduct, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductName", productName);
                    cmd.Parameters.AddWithValue("@Category", category);
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    conn.Open();
                    productId = (int)cmd.ExecuteScalar(); // Get the inserted ProductID
                    conn.Close();
                }

                // Insert the barcode into the Barcodes table
                string queryInsertBarcode = "INSERT INTO Barcodes (ProductID, Barcode) VALUES (@ProductID, @Barcode)";
                using (SqlCommand cmd = new SqlCommand(queryInsertBarcode, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    cmd.Parameters.AddWithValue("@Barcode", barcode);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                MessageBox.Show("New product added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtProductName.Clear();
                txtBarcode.Clear();
                txtPrice.Clear();
                txtQuantity.Clear();
                cbCategoryAdd.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string productName = txtProductName.Text.Trim();
            string barcode = txtBarcode.Text.Trim();
            string category = cbCategoryAdd.SelectedItem?.ToString();
            string priceText = txtPrice.Text.Trim();
            string quantityText = txtQuantity.Text.Trim();

            // Check if all fields are filled (Adding a New Product)
            if (!string.IsNullOrEmpty(productName) &&
                !string.IsNullOrEmpty(barcode) &&
                !string.IsNullOrEmpty(category) &&
                !string.IsNullOrEmpty(priceText) &&
                !string.IsNullOrEmpty(quantityText))
            {
                AddNewProduct(productName, barcode, category, priceText, quantityText);
            }
            // Check if only Product Name, Barcode, and Category are filled (Adding a New Barcode)
            else if (!string.IsNullOrEmpty(productName) &&
                     !string.IsNullOrEmpty(barcode) &&
                     !string.IsNullOrEmpty(category))
            {
                AddNewBarcode(productName, barcode, category);
            }
            else
            {
                MessageBox.Show("Please fill in the required fields.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DisplayStocksTimer_Tick(object sender, EventArgs e)
        {
            if (cbCategory.DroppedDown) return; // Skip updates if dropdown is open
            SearchProducts(); // Call the search method
        }
        private void cbCategory_DropDown(object sender, EventArgs e)
        {
            if (timer.Enabled)
            {
                timer.Stop(); // Pause the timer while the user is selecting a category
            }
        }

        private void cbCategory_DropDownClosed(object sender, EventArgs e)
        {
            timer.Start(); // Resume the timer after the user closes the dropdown
        }
        private void SearchProducts()
        {
            string selectedCategory = cbCategory.SelectedItem?.ToString().Trim();
            if (string.IsNullOrEmpty(selectedCategory))
            {
                selectedCategory = "All"; // Set default to "All"
            }

            string searchTerm = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = "%"; // Use "%" for empty search term
            }

            string query = @"
                SELECT p.ProductName, p.Category, p.Price, p.Quantity, p.DateAdded, p.LastStockUpdate
                FROM Products p
                LEFT JOIN Barcodes b ON p.ProductID = b.ProductID
                WHERE (LOWER(p.ProductName) LIKE LOWER(@SearchTerm) OR LOWER(b.Barcode) LIKE LOWER(@SearchTerm))
                AND (@Category = 'All' OR LOWER(p.Category) = LOWER(@Category))
                GROUP BY p.ProductName, p.Category, p.Price, p.Quantity, p.DateAdded, p.LastStockUpdate";

            try
            {
                // Ensure the connection is open
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open(); // Open the connection if it's closed
                }

                // Setup the SQL command with parameters
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Add parameters for the query
                    cmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                    cmd.Parameters.AddWithValue("@Category", selectedCategory);

                    // Execute the query
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Prepare a DataTable to store the results
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader); // Load results from reader into DataTable

                        // Update the DataGridView with the retrieved data
                        dgvStocks.DataSource = dataTable;

                        // Hide the Barcode column if it exists
                        /*if (dgvStocks.Columns.Contains("Barcode"))
                        {
                            dgvStocks.Columns["Barcode"].Visible = false;
                        }*/
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during the search: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Always close the connection after the query
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        private void dgvStocks_SelectionChanged(object sender, EventArgs e)
        {
            StopRealTimeUpdates();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            // Ensure a row is selected
            if (dgvStocks.SelectedRows.Count > 0)
            {
                // Get the selected product's ID (assuming it's in the first column, adjust based on your DataGridView)
                int selectedProductId = Convert.ToInt32(dgvStocks.SelectedRows[0].Cells["ProductID"].Value);

                // Retrieve the product data from the database
                RetrieveProductDataFromDatabase(selectedProductId);
            }
            else
            {
                MessageBox.Show("Please select a product to edit.");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            RestartRealTimeUpdates();
        }
        private void RetrieveProductDataFromDatabase(int productId)
        {
            // SQL query to retrieve product details and the first barcode from the database
            string query = @"
        SELECT p.ProductName, p.Price, p.Quantity, p.Category, 
               (SELECT TOP 1 b.Barcode FROM Barcodes b WHERE b.ProductID = p.ProductID) AS Barcode
        FROM Products p
        WHERE p.ProductID = @ProductId";

            // Create a connection object using your connection string
            using (SqlConnection conn = new SqlConnection(Connection.ConnectionString()))
            {
                try
                {
                    // Open the connection
                    conn.Open();

                    // Create a SqlCommand object to execute the query
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Add parameters to the SQL query
                        cmd.Parameters.AddWithValue("@ProductId", productId);

                        // Execute the query and get the result
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Check if there is a row of data
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    // Retrieve product data and barcode from the query result
                                    txtProductName.Text = reader["ProductName"].ToString();
                                    txtPrice.Text = reader["Price"].ToString();
                                    txtQuantity.Text = reader["Quantity"].ToString();
                                    cbCategory.SelectedItem = reader["Category"].ToString();

                                    // Retrieve and display only the first barcode
                                    string barcode = reader["Barcode"].ToString();
                                    txtBarcode.Text = barcode;

                                    // Optionally, disable the edit button to avoid editing while the form is in editing mode
                                    btnEdit.Enabled = false;
                                    btnSave.Enabled = true;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Product not found.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
    }
}
