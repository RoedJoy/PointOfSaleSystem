using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyPos.Employee_List_Classes
{
    public class Textboxes
    {
        private List<TextBox> textBoxes;

        public Textboxes(params TextBox[] textBoxes)
        {
            // Store all the textboxes passed to the constructor
            this.textBoxes = new List<TextBox>(textBoxes);
        }

        // Method to set all textboxes to read-only or editable
        public void SetTextboxesReadOnly(bool isReadOnly)
        {
            foreach (var textbox in textBoxes)
            {
                textbox.ReadOnly = isReadOnly;
            }
        }
    }
}
