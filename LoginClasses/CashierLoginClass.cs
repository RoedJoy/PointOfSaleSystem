using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Windows.Forms;

namespace MyPos.LoginClasses
{
    public class CashierLoginClass : CashierClass
    {
        public void CashierLogin(Form login)
        {
            MainForm1 cashier = new MainForm1();
            SqlConnection conn = new SqlConnection("Data Source=LAPTOP-UBGAH14Q\\SQLEXPRESS;Initial Catalog=MyPos;Integrated Security=True;");
            conn.Open();
            string query = "SELECT COUNT(*) \r\nFROM dbo.Cashiers \r\nWHERE Username = @username AND Password = @password;";
            SqlCommand command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("@username", Username);
            command.Parameters.AddWithValue("@password", Password);
            Count = (int)command.ExecuteScalar();
            if (Count > 0)
            {
                MessageBox.Show("Login Success!", "info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cashier.Show();
                login.Hide();
                
            }
            else
            {
                MessageBox.Show("Incorrect Cashier Username or Cashier Password. ", "Input error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
