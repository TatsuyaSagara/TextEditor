using System;
using System.Drawing;
using System.Windows.Forms;

namespace TextEditor
{
    public partial class FindDlg : Form
    {
        private CETextBoxControl.CEEditView m_editView;

        public string findString;

        public Boolean ulToggle;
        public Boolean wordToggle;
        public Boolean regToggle;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FindDlg()
        {
            InitializeComponent();

            ulToggle = false;
            wordToggle = false;
            regToggle = false;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FindDlg(CETextBoxControl.CEEditView form)
        {
            InitializeComponent();

            m_editView = form;
        }

        /// <summary>
        /// 前へ検索ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_UpFind_Click(object sender, EventArgs e)
        {
            m_editView.PrevFind(comboBox_Conditions.Text, ulToggle, wordToggle, regToggle);
        }

        /// <summary>
        /// 次へ検索ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_DownFind_Click(object sender, EventArgs e)
        {
            m_editView.NextFind(comboBox_Conditions.Text, ulToggle, wordToggle, regToggle);
        }

        /// <summary>
        /// 検索ダイアログ表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindForm_Shown(object sender, EventArgs e)
        {
            comboBox_Conditions.Focus();
        }

        /// <summary>
        /// キープレス
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                // エスケープ押したら検索ダイアログ消去
                this.Close();
            }
            else if (e.KeyChar == (char)Keys.Return)
            {
                // 検索文字列を入力してエンター押したとき
                m_editView.NextFind(comboBox_Conditions.Text, ulToggle, wordToggle, regToggle);
                this.Close();
            }
        }

        private void FindForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            findString = comboBox_Conditions.Text;
        }

        private void ul_ClickEvent(object sender, EventArgs e)
        {
            if (regToggle) return;

            if (ulToggle)
            {
                // on -> off
                ulToggle = false;
                ULLabel.BackColor = SystemColors.Control;
            }
            else
            {
                // off -> on
                ulToggle = true;
                ULLabel.BackColor = Color.DarkGray;
            }
        }

        private void word_ClickEvent(object sender, EventArgs e)
        {
            if (regToggle) return;

            if (wordToggle)
            {
                // on -> off
                wordToggle = false;
                wordLabel.BackColor = SystemColors.Control;
            }
            else
            {
                // off -> on
                wordToggle = true;
                wordLabel.BackColor = Color.DarkGray;
            }
        }

        private void reg_ClickEvent(object sender, EventArgs e)
        {
            if (regToggle)
            {
                // on -> off
                regToggle = false;
                regLabel.BackColor = SystemColors.Control;
                ULLabel.ForeColor = Color.Black;
                wordLabel.ForeColor = Color.Black;
            }
            else
            {
                // off -> on
                regToggle = true;
                regLabel.BackColor = Color.DarkGray;
                ULLabel.ForeColor = Color.Gray;
                wordLabel.ForeColor = Color.Gray;
            }
        }

        private void close_ClickEvent(object sender, EventArgs e)
        {
            // キャンセルボタン押したら検索ダイアログ消去
            this.Close();
        }
    }
}
