using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms;
using System.Xml.Linq;

namespace MyPos.AddingEmployee
{
    public class AddEmployeeClass : EmployeeInfoClass
    {
        private int x;
        public AddEmployeeClass() 
        {
            this.x = 110000;  // Initial value for Cashier ID
        }

        // Method to generate a unique cashier ID
        public int GenerateCashierID()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection("Data Source=LAPTOP-UBGAH14Q\\SQLEXPRESS;Initial Catalog=MyPos;Integrated Security=True;"))
                {
                    connection.Open();

                    string query = "SELECT MAX(UserID) FROM Cashiers";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        object result = command.ExecuteScalar();

                        if (result != DBNull.Value && result != null)
                        {
                            if (int.TryParse(result.ToString(), out int lastID))
                            {
                                UserID = lastID + 1;  // Assign the incremented value to the class-level variable
                                return UserID;
                            }
                            else
                            {
                                throw new FormatException("Unable to parse the last UserID from the database.");
                            }
                        }
                        else
                        {
                            // No users exist; start with a default value, e.g., 110001
                            x++;
                            UserID = x;  // Assign the incremented default value to the class-level variable
                            return UserID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while generating UserID: " + ex.Message);
                return -1;  // Return a negative number to indicate failure
            }
        }

        // Method to upload the user info to the database
        public void UploadInfo()
        {
            try
            {
                string query = @"INSERT INTO Cashiers (UserID, FirstName, MiddleName, LastName, Age, Gender, Address, BirthDate, Username, Password)
                         VALUES (@UserID, @FirstName, @MiddleName, @LastName, @Age, @Gender, @Address, @BirthDate, @Username, @Password)";

                using (SqlConnection connection = new SqlConnection("Data Source=LAPTOP-UBGAH14Q\\SQLEXPRESS;Initial Catalog=MyPos;Integrated Security=True;"))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parameters
                    command.Parameters.AddWithValue("@UserID", UserID);
                    command.Parameters.AddWithValue("@FirstName", FirstName);
                    command.Parameters.AddWithValue("@MiddleName", MiddleName);
                    command.Parameters.AddWithValue("@LastName", LastName);
                    command.Parameters.AddWithValue("@Age", Age);
                    command.Parameters.AddWithValue("@Gender", Gender);
                    command.Parameters.AddWithValue("@Address", Address);
                    command.Parameters.AddWithValue("@BirthDate", BirthDate);
                    command.Parameters.AddWithValue("@Username", Username);
                    command.Parameters.AddWithValue("@Password", Password);

                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Data saved successfully!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
    }
}
