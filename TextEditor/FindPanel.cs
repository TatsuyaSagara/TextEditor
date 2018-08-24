using CETextBoxControl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextEditor
{
#if false // 入力補完テスト
    class SampleKeyValuePair
    {
        string _key;
        string _value;

        public SampleKeyValuePair(string key, string aValue)
        {
            _key = key;
            _value = aValue;
        }

        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
    }
#endif
    public class FindPanel : Panel
    {
        public ComboBox findComboBox;    // 検索ワード入力ボックス
        Label ULLabel;      // 大文字小文字
        Label wordLabel;    // 単語単位
        Label regLabel;     // 正規表現
        Label closeLabel;   // 閉じる
        Label findLabel;    // 検索

        private Boolean ULToggle;     // 大文字・小文字
        private Boolean wordToggle;   // 単語単位
        private Boolean regToggle;    // 正規表現

        CEEditView m_editView;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FindPanel(CEEditView ev)
        {
            // 対象となるビュー保存
            m_editView = ev;

            // サイズと色
            this.Size = new Size(300, 47);
            this.BackColor = Color.Gainsboro;

            // 検索ボックス
            findComboBox = new ComboBox();
            findComboBox.Size = new Size(200, 21);
            findComboBox.Location = new Point(4, 2);
            findComboBox.MaxDropDownItems = 10;
            this.Controls.Add(findComboBox);

#if false // 入力補完テスト
            findTextBox.DataSource = GetDataSource();
            findTextBox.DisplayMember = "Key";
            findTextBox.ValueMember = "Value";
            findTextBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            findTextBox.AutoCompleteSource = AutoCompleteSource.ListItems;
#endif

            // 大文字・小文字トグルラベル
            ULLabel = new Label();
            ULLabel.Text = "Aa";
            ULLabel.TextAlign = ContentAlignment.MiddleCenter;
            ULLabel.Size = new Size(30, 19);
            ULLabel.Location = new Point(4, 25);
            //ULLabel.BorderStyle = BorderStyle.None;
            ULLabel.BackColor = Color.Gainsboro;
            ULToggle = false;
            ULLabel.Click += new EventHandler(ULLabel_Click);
            ToolTip ULLabelToolTip = new ToolTip();
            ULLabelToolTip.SetToolTip(ULLabel, "大文字・小文字判別");
            this.Controls.Add(ULLabel);

            // 単語単位トグルラベル
            wordLabel = new Label();
            wordLabel.Text = "|A|";
            wordLabel.TextAlign = ContentAlignment.MiddleCenter;
            wordLabel.Size = new Size(30, 19);
            wordLabel.Location = new Point(36, 25);
            //wordLabel.BorderStyle = BorderStyle.None;
            wordLabel.BackColor = Color.Gainsboro;
            wordToggle = false;
            wordLabel.Click += new EventHandler(wordLabel_Click);
            ToolTip wordLabelToolTip = new ToolTip();
            wordLabelToolTip.SetToolTip(wordLabel, "単語単位");
            this.Controls.Add(wordLabel);

            // 正規表現トグルラベル
            regLabel = new Label();
            regLabel.Text = "${";
            regLabel.TextAlign = ContentAlignment.MiddleCenter;
            regLabel.Size = new Size(30, 19);
            regLabel.Location = new Point(68, 25);
            //regLabel.BorderStyle = BorderStyle.None;
            regLabel.BackColor = Color.Gainsboro;
            regToggle = false;
            regLabel.Click += new EventHandler(regLabel_Click);
            ToolTip regLabelToolTip = new ToolTip();
            regLabelToolTip.SetToolTip(regLabel, "正規表現");
            this.Controls.Add(regLabel);

            // 閉じるボタン
            closeLabel = new Label();
            closeLabel.Font = new Font("ＭＳ ゴシック", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));
            closeLabel.Text = "×";
            closeLabel.TextAlign = ContentAlignment.MiddleCenter;
            closeLabel.Size = new Size(21, 21);
            closeLabel.Location = new Point(this.Width - closeLabel.Width - 2, 2);
            closeLabel.Click += new EventHandler(closeLabel_Click);
            ToolTip closeLabelToolTip = new ToolTip();
            closeLabelToolTip.SetToolTip(closeLabel, "閉じる");
            this.Controls.Add(closeLabel);

            // 検索
            findLabel = new Label();
            findLabel.Text = "⇒";
            findLabel.TextAlign = ContentAlignment.MiddleCenter;
            findLabel.Size = new Size(21, 21);
            findLabel.Location = new Point(findComboBox.Width + 6, 2);
            findLabel.Click += new EventHandler(findLabel_Click);
            ToolTip findLabelToolTip = new ToolTip();
            findLabelToolTip.SetToolTip(findLabel, "検索");
            this.Controls.Add(findLabel);

            // マウスポインタ変更
#if false
            this.MouseEnter += new EventHandler(mouseEnter);
#endif
            this.MouseEnter += (sender, e) =>
            {
                Cursor = Cursors.Default;
            };
        }
#if false // 入力補完テスト
        private BindingSource GetDataSource()
        {
            BindingSource s = new BindingSource();

            s.DataSource = typeof(SampleKeyValuePair);

            s.Add(new SampleKeyValuePair("value1", "gsf_zero1"));
            s.Add(new SampleKeyValuePair("value2", "gsf_zero2"));
            s.Add(new SampleKeyValuePair("value3", "gsf_zero3"));

            return s;
        }
#endif

        /// <summary>
        /// 検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void findLabel_Click(object sender, EventArgs e)
        {
            this.NextFind();
        }

        /// <summary>
        /// 次検索
        /// </summary>
        public void NextFind()
        {
            // 検索
            string findString = this.findComboBox.Text;
            Boolean findUL = this.ULToggle;
            Boolean findWord = this.wordToggle;
            Boolean findReg = this.regToggle;
            m_editView.NextFind(findString, findUL, findWord, findReg);

            // 検索コンボボックスに検索文字列追加
            int idx = findComboBox.Items.IndexOf(findComboBox.Text);
            if (idx == -1)
            {
                findComboBox.Items.Insert(0, findComboBox.Text);
            }
        }

        /// <summary>
        /// 前検索
        /// </summary>
        public void PrevFind()
        {
            // 検索
            string findString = this.findComboBox.Text;
            Boolean findUL = this.ULToggle;
            Boolean findWord = this.wordToggle;
            Boolean findReg = this.regToggle;
            m_editView.PrevFind(findString, findUL, findWord, findReg);

            // 検索コンボボックスに検索文字列追加
            int idx = findComboBox.Items.IndexOf(findComboBox.Text);
            if (idx == -1)
            {
                //アイテム一覧の一番上に登録
                findComboBox.Items.Insert(0, findComboBox.Text);
            }
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeLabel_Click(object sender, EventArgs e)
        {
            // 非表示
            this.Visible = false;
        }
#if false // イベントをラムダ式で書いたので不要
        /// <summary>
        /// マウスポインタ変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mouseEnter(object sender, EventArgs e)
        {
            // デフォルトカーソル
            Cursor = Cursors.Default;
        }
#endif
        /// <summary>
        /// 大文字・小文字トグルラベル
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ULLabel_Click(object sender, EventArgs e)
        {
            if (regToggle) return;

            if (ULToggle)
            {
                // on -> off
                ULToggle = false;
                //ULLabel.BorderStyle = BorderStyle.None;
                ULLabel.BackColor = Color.Gainsboro;
            }
            else
            {
                // off -> on
                ULToggle = true;
                //ULLabel.BorderStyle = BorderStyle.FixedSingle;
                ULLabel.BackColor = Color.DarkGray;
            }
        }

        /// <summary>
        /// 単語単位トグルラベル
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wordLabel_Click(object sender, EventArgs e)
        {
            if (regToggle) return;

            if (wordToggle)
            {
                // on -> off
                wordToggle = false;
                //wordLabel.BorderStyle = BorderStyle.None;
                wordLabel.BackColor = Color.Gainsboro;
            }
            else
            {
                // off -> on
                wordToggle = true;
                //wordLabel.BorderStyle = BorderStyle.FixedSingle;
                wordLabel.BackColor = Color.DarkGray;
            }
        }

        /// <summary>
        /// 正規表現トグルラベル
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void regLabel_Click(object sender, EventArgs e)
        {
            if (regToggle)
            {
                // on -> off
                regToggle = false;
                //regLabel.BorderStyle = BorderStyle.None;
                regLabel.BackColor = Color.Gainsboro;
                ULLabel.ForeColor = Color.Black;
                wordLabel.ForeColor = Color.Black;
            }
            else
            {
                // off -> on
                regToggle = true;
                //regLabel.BorderStyle = BorderStyle.FixedSingle;
                regLabel.BackColor = Color.DarkGray;
                ULLabel.ForeColor = Color.Gray;
                wordLabel.ForeColor = Color.Gray;
            }
        }

        /// <summary>
        /// コピー
        /// </summary>
        public void CopyData()
        {
            DataObject data = new DataObject();
            data.SetData(DataFormats.Text, findComboBox.SelectedText);
            Clipboard.SetDataObject(data, true);
        }

        /// <summary>
        /// ペースト
        /// </summary>
        public void PasetData()
        {
            IDataObject data = Clipboard.GetDataObject();
            string str = (string)data.GetData(DataFormats.StringFormat);
            findComboBox.Text = str;
        }

        public void CutData()
        {
            DataObject data = new DataObject();
            data.SetData(DataFormats.Text, findComboBox.SelectedText);
            Clipboard.SetDataObject(data, true);
        }
    }
}
