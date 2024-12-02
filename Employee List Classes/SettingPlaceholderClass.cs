using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyPos.Employee_List_Classes
{
    public class SettingPlaceholderClass
    {
        private string placeholder = "Name or Cashier ID";

        public void SetPlaceholder(TextBox textBox, string placeholderText)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = placeholderText;
                textBox.ForeColor = System.Drawing.Color.Gray;  // Placeholder text color
            }
        }

        // Remove the placeholder when the textbox gains focus
        public void RemovePlaceholder(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == placeholder)
            {
                textBox.Text = string.Empty;
                textBox.ForeColor = System.Drawing.Color.Black; // Normal text color
            }
        }

        // Add the placeholder back if the textbox loses focus and is empty
        public void AddPlaceholder(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                SetPlaceholder(textBox, placeholder);
            }
        }
    }
}
