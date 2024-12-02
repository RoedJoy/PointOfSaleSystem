using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyPos.AdminForms
{
    public partial class FormProfile : Form
    {
        public FormProfile()
        {
            InitializeComponent();
        }

        private void FormProfile_Load(object sender, EventArgs e)
        {
            LoadTheme();
        }
        private void LoadTheme()
        {
            foreach (Control btns in this.Controls)
            {
                if (btns.GetType() == typeof(Button))
                {
                    Button btn = (Button)btns;
                    //btn.BackColor = ColorClass.primaryColor;
                    btn.ForeColor = Color.White;
                    btn.FlatAppearance.BorderColor = ColorClass.secondaryColor;
                }
            }
            foreach(Control pnls in this.Controls)
            {
                if(pnls.GetType() == typeof(Panel))
                { 
                    if(pnls == panel1)
                    {
                        panel1.BackColor = Color.White;
                    }
                    Panel pnl = (Panel)pnls;
                    pnl.BackColor = ColorClass.primaryColor;
                    pnl.ForeColor = Color.White;

                }
            }
        }
    }
}
