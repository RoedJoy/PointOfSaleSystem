using MyPos.LoginClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyPos
{
    public partial class LoginForm : Form
    {
        private AdminLoginClass admin;
        private CashierLoginClass cashier;
        public LoginForm()
        {
            InitializeComponent();
            admin = new AdminLoginClass();
            cashier = new CashierLoginClass();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (cbRoles.SelectedIndex == 0)
            {
                admin.Username = TxtboxUsername.Text;
                admin.Password = TxtboxPassword.Text;
                admin.AdminLogin(this);
            }
            else if (cbRoles.SelectedIndex == 1)
            {
                cashier.Username = TxtboxUsername.Text;
                cashier.Password = TxtboxPassword.Text;
                cashier.CashierLogin(this);
            }
            else
            {
                BtnLogin.Enabled = false;
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            ArrayList roles = new ArrayList();
            roles.Add("Admin");
            roles.Add("Cashier");
            foreach (string role in roles)
            {
                cbRoles.Items.Add(role);
            }
            BtnLogin.Enabled = false;
            TxtboxPassword.UseSystemPasswordChar = true;
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void cbRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbRoles.SelectedIndex == 0)
            {
                BtnLogin.Enabled = true;
                lblTitle.Text = "Admin Login";
            }
            else if (cbRoles.SelectedIndex == 1)
            {
                BtnLogin.Enabled = true;
                lblTitle.Text = "Cashier Login";
            }
            else
            {
                BtnLogin.Enabled = false;
            }
        }

        private void cboxShowPass_CheckedChanged(object sender, EventArgs e)
        {
            if(!cboxShowPass.Checked)
            {
                TxtboxPassword.UseSystemPasswordChar = true;
            }
            else
            {
                TxtboxPassword.UseSystemPasswordChar = false;
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
