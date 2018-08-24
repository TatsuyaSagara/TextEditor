namespace TextEditor
{
    partial class FindDlg
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.comboBox_Conditions = new System.Windows.Forms.ComboBox();
            this.ULLabel = new System.Windows.Forms.Label();
            this.wordLabel = new System.Windows.Forms.Label();
            this.regLabel = new System.Windows.Forms.Label();
            this.closeLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboBox_Conditions
            // 
            this.comboBox_Conditions.FormattingEnabled = true;
            this.comboBox_Conditions.Location = new System.Drawing.Point(6, 6);
            this.comboBox_Conditions.Name = "comboBox_Conditions";
            this.comboBox_Conditions.Size = new System.Drawing.Size(214, 20);
            this.comboBox_Conditions.TabIndex = 1;
            // 
            // ULLabel
            // 
            this.ULLabel.Location = new System.Drawing.Point(224, 5);
            this.ULLabel.Name = "ULLabel";
            this.ULLabel.Size = new System.Drawing.Size(20, 20);
            this.ULLabel.TabIndex = 8;
            this.ULLabel.Text = "Aa";
            this.ULLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ULLabel.Click += new System.EventHandler(this.ul_ClickEvent);
            // 
            // wordLabel
            // 
            this.wordLabel.Location = new System.Drawing.Point(250, 5);
            this.wordLabel.Name = "wordLabel";
            this.wordLabel.Size = new System.Drawing.Size(20, 20);
            this.wordLabel.TabIndex = 9;
            this.wordLabel.Text = "|A|";
            this.wordLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.wordLabel.Click += new System.EventHandler(this.word_ClickEvent);
            // 
            // regLabel
            // 
            this.regLabel.BackColor = System.Drawing.SystemColors.Control;
            this.regLabel.Location = new System.Drawing.Point(276, 5);
            this.regLabel.Name = "regLabel";
            this.regLabel.Size = new System.Drawing.Size(20, 20);
            this.regLabel.TabIndex = 10;
            this.regLabel.Text = "${";
            this.regLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.regLabel.Click += new System.EventHandler(this.reg_ClickEvent);
            // 
            // closeLabel
            // 
            this.closeLabel.Location = new System.Drawing.Point(300, 5);
            this.closeLabel.Name = "closeLabel";
            this.closeLabel.Size = new System.Drawing.Size(20, 20);
            this.closeLabel.TabIndex = 11;
            this.closeLabel.Text = "×";
            this.closeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.closeLabel.Click += new System.EventHandler(this.close_ClickEvent);
            // 
            // FindDlg
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(326, 31);
            this.Controls.Add(this.closeLabel);
            this.Controls.Add(this.regLabel);
            this.Controls.Add(this.wordLabel);
            this.Controls.Add(this.ULLabel);
            this.Controls.Add(this.comboBox_Conditions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(1, 1);
            this.Name = "FindDlg";
            this.Text = "検索";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FindForm_FormClosing);
            this.Shown += new System.EventHandler(this.FindForm_Shown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FindForm_KeyPress);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ComboBox comboBox_Conditions;
        private System.Windows.Forms.Label ULLabel;
        private System.Windows.Forms.Label wordLabel;
        private System.Windows.Forms.Label regLabel;
        private System.Windows.Forms.Label closeLabel;
    }
}