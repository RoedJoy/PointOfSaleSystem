using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Windows.Forms;
using MyPos.Connections;

namespace MyPos.LoginClasses
{
    public class AdminLoginClass : AdminClass
    {
        public void AdminLogin(Form login)
        {
            MainForm admin = new MainForm();
            SqlConnection conn = new SqlConnection(Connection.ConnectionString());
            conn.Open();
            string query = "SELECT COUNT(*) \r\nFROM dbo.Admin \r\nWHERE Username = @username AND Password = @password;";
            SqlCommand command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("@username", Username);
            command.Parameters.AddWithValue("@password", Password);
            int count = (int)command.ExecuteScalar();
            if (count > 0)
            {
                MessageBox.Show("Login Success!", "info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                admin.Show();
                login.Hide();
            }
            else
            {
                MessageBox.Show("Incorrect Admin Username or Admin Password. ", "Input error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
