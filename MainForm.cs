using MyPos.AdminForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyPos
{
    public partial class MainForm : Form
    {
        //Fields
        private Button currentButton;
        private Random random;
        private int tempIndex;
        private Form activeForm;
        LoginForm login;

        //Constructor
        public MainForm()
        {
            InitializeComponent();
            random = new Random();
            BtnCloseChildForm.Visible = false;
            this.Text = string.Empty;
            this.ControlBox = false;
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;

        }
        /*
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
        */
        //Methods
        private Color SelectThemeColor()
        {
            int index = random.Next(ColorClass.colorList.Count);
            while(tempIndex == index)
            {
                index = random.Next(ColorClass.colorList.Count);
            }
            tempIndex = index;
            string color  = ColorClass.colorList[index];
            return ColorTranslator.FromHtml(color);
        }
        private void ActivateButton(object btnSender)
        {
            if(btnSender != null)
            {
                if (currentButton != (Button)btnSender)
                {
                    DisableButton();
                    Color color = SelectThemeColor();
                    currentButton = (Button)btnSender;
                    currentButton.BackColor = color;
                    currentButton.ForeColor = Color.White;
                    currentButton.Font = new System.Drawing.Font("Calibri", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    panelTitleBar.BackColor = color;
                    panelPicture.BackColor = ColorClass.ChangeColorBrightness(color, -0.3);
                    ColorClass.primaryColor = color;
                    ColorClass.secondaryColor = ColorClass.ChangeColorBrightness(color, -0.3);
                    BtnCloseChildForm.Visible = true;
                }
            }
        }
        private void DisableButton()
        {
            foreach(Control previousBtn in panelMenu.Controls)
            {
                if(previousBtn.GetType() == typeof(Button))
                {
                    previousBtn.BackColor = Color.FromArgb(51, 51, 76);
                    previousBtn.ForeColor = Color.Gainsboro;
                    previousBtn.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                }
            }
        }
        private void OpenChildForm(Form childForm, object btnSender)
        {
            if(activeForm != null)
            {
                activeForm.Close();
            }
            ActivateButton(btnSender);
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            this.panelForChildfForms.Controls.Add(childForm);
            this.panelForChildfForms.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
            lblTitle.Text = childForm.Text;
        }

        private void BtnHome_Click(object sender, EventArgs e)
        {
            OpenChildForm(new AdminForms.FormHome(), sender);
        }

        private void BtnProfile_Click(object sender, EventArgs e)
        {
            OpenChildForm(new AdminForms.FormProfile(), sender);
        }

        private void BtnStocks_Click(object sender, EventArgs e)
        {
            OpenChildForm(new AdminForms.FormStocks(), sender);
        }

        private void BtnSales_Click(object sender, EventArgs e)
        {
            OpenChildForm(new AdminForms.FormSales(), sender);
        }

        private void BtnEmployeeList_Click(object sender, EventArgs e)
        {
            OpenChildForm(new AdminForms.FormEmployeeList(), sender);
        }

        private void BtnAddEmployee_Click(object sender, EventArgs e)
        {
            OpenChildForm(new AdminForms.FormAddEmployee(), sender);
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            login = new LoginForm();
            login.Show();
            this.Hide();
        }

        private void BtnCloseChildForm_Click(object sender, EventArgs e)
        {
            if(activeForm != null)
            {
                activeForm.Close();
            }
            Reset();
        }
        private void Reset()
        {
            DisableButton();
            lblTitle.Text = "Dashboard";
            panelTitleBar.BackColor = Color.FromArgb(0, 150, 136);
            panelPicture.BackColor = Color.FromArgb(39, 39, 58);
            currentButton = null;
            BtnCloseChildForm.Visible = false;
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void panelTitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            //ReleaseCapture();
            //SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            FormStocks stocks = new FormStocks();
            stocks.StopRealTimeUpdates();
            Application.Exit();
        }


        private void BtnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
