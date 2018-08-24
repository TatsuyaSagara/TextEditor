using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CETextBoxControl;
using System.Windows.Forms;

namespace TextEditor
{
    public class ViewInfoMng
    {
        public CEEditView customTextBox;
        public TabButton button;
        public Boolean active;
        public string fileName;
#if false // 検索パネル未使用
        public FindPanel findPanel;
#endif
        public ViewInfoMng()
        {
            customTextBox = null;
            button = null;
            active = false;
            fileName = "";
#if false // 検索パネル未使用
            findPanel = null;
#endif
        }
    }
}
