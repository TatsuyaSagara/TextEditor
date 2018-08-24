using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CETextBoxControl;
using System.Windows.Forms;

namespace TextEditor
{
    public class ViewInfo
    {
        public CEEditView customTextBox;
        public Button btns;
        public Boolean active;
        public string fileName;

        public ViewInfo()
        {
            customTextBox = null;
            btns = null;
            active = false;
            fileName = "";
        }
    }
}
