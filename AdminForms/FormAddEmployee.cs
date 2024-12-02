using MyPos.AddingEmployee;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace MyPos.AdminForms
{
    public partial class FormAddEmployee : Form
    {
        private int id;
        private readonly AddEmployeeClass addEmployeeClass;

        public FormAddEmployee()
        {
            InitializeComponent();
            addEmployeeClass = new AddEmployeeClass();
            this.Resize += FormAddEmployee_Resize;
        }

        private void FormAddEmployee_Load(object sender, EventArgs e)
        {
            ArrayList genders = new ArrayList();
            genders.Add("Male");
            genders.Add("Female");
            foreach(string gender in genders)
            {
                cbGender.Items.Add(gender);
            }
            LoadTheme();
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
            foreach (Control pnls in this.Controls)
            {
                if(pnls.GetType() == typeof(System.Windows.Forms.Panel))
                {
                    System.Windows.Forms.Panel pnl = (System.Windows.Forms.Panel)pnls;
                    pnl.BackColor = ColorClass.primaryColor;
                    pnl.ForeColor = Color.White;
                }
            }
            BtnGenerateIDNum.FlatAppearance.BorderColor = ColorClass.primaryColor;
            BtnRegister.FlatAppearance.BorderColor = ColorClass.primaryColor;
        }
        public void FormAddEmployee_Resize(object sender, EventArgs e)
        {

        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text) || string.IsNullOrWhiteSpace(txtMiddleName.Text) || string.IsNullOrWhiteSpace(txtAge.Text) || string.IsNullOrEmpty(cbGender.Text) || string.IsNullOrEmpty(dtpBirthday.Text) || string.IsNullOrWhiteSpace(txtAddress.Text) || string.IsNullOrWhiteSpace(txtMobileNum.Text) || string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please complete the requirements.", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (string.IsNullOrEmpty(lblGeneratedIDNum.Text))
            {
                MessageBox.Show("Please click the button to generate a ID number.", "Input null", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    addEmployeeClass.FirstName = txtFirstName.Text;
                    addEmployeeClass.MiddleName = txtMiddleName.Text;
                    addEmployeeClass.LastName = txtLastName.Text;
                    addEmployeeClass.Age = Convert.ToInt32(txtAge.Text);
                    addEmployeeClass.Gender = cbGender.SelectedItem.ToString();
                    addEmployeeClass.Address = txtAddress.Text;
                    addEmployeeClass.MobileNum = txtMobileNum.Text;
                    addEmployeeClass.BirthDate = Convert.ToDateTime(dtpBirthday.Text);
                    addEmployeeClass.Username = txtUsername.Text;
                    addEmployeeClass.Password = txtPassword.Text;
                    addEmployeeClass.UserID = Convert.ToInt32(lblGeneratedIDNum.Text);
                }
                catch (FormatException fe)
                {
                    MessageBox.Show("Exception Handled: " + fe.Message);
                }
                finally
                {
                    addEmployeeClass.UploadInfo();
                    txtFirstName.Clear();
                    txtMiddleName.Clear();
                    txtLastName.Clear();
                    txtAge.Clear();
                    txtAddress.Clear();
                    txtMobileNum.Clear();
                    txtUsername.Clear();
                    txtPassword.Clear();
                    lblGeneratedIDNum.ResetText();
                    dtpBirthday.ResetText();
                    cbGender.ResetText();
                }
            }
        }

        private void BtnGenerateIDNum_Click(object sender, EventArgs e)
        {
            try
            {
                lblGeneratedIDNum.Text = Convert.ToString(addEmployeeClass.GenerateCashierID());
                BtnGenerateIDNum.Text = "Clicked";
                BtnGenerateIDNum.Enabled = false;
                id = int.Parse(lblGeneratedIDNum.Text);
            }
            catch (FormatException fe)
            {
                MessageBox.Show("An exception has been caught: " + fe.Message);
            }
        }

        private void checkBoxShowPass_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxShowPass.Checked)
            {
                txtPassword.UseSystemPasswordChar = false;
            }
            else
            {
                txtPassword.UseSystemPasswordChar = true;
            }
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = true;
        }

        private void lblMN_Click(object sender, EventArgs e)
        {

        }

        private void txtAge_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
