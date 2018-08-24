using System.Text;

namespace TextEditor
{
    partial class TextEditor
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextEditor));
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.StatusBar = new System.Windows.Forms.StatusStrip();
            this.StatusLabel_CaretX = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabel_CaretY = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripDropDownBtn = new System.Windows.Forms.ToolStripDropDownButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ファイルFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.CloseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Undo = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Redo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.CopyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PasteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AllSelectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.RectSelectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.検索SToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StripMenuItem_Find = new System.Windows.Forms.ToolStripMenuItem();
            this.StripMenuItem_NextFind = new System.Windows.Forms.ToolStripMenuItem();
            this.StripMenuItem_PrevFind = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.LineJumpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.設定OToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.FreeCursorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RightWrapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ウィンドウWToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NextWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PrevWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ヘルプHToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.VersionInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.tabPanel = new System.Windows.Forms.Panel();
            this.StatusBar.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // StatusBar
            // 
            this.StatusBar.BackColor = System.Drawing.SystemColors.Control;
            this.StatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel_CaretX,
            this.StatusLabel_CaretY,
            this.toolStripDropDownBtn});
            this.StatusBar.Location = new System.Drawing.Point(0, 656);
            this.StatusBar.Name = "StatusBar";
            this.StatusBar.Size = new System.Drawing.Size(658, 24);
            this.StatusBar.TabIndex = 0;
            // 
            // StatusLabel_CaretX
            // 
            this.StatusLabel_CaretX.AutoSize = false;
            this.StatusLabel_CaretX.Name = "StatusLabel_CaretX";
            this.StatusLabel_CaretX.Size = new System.Drawing.Size(90, 19);
            this.StatusLabel_CaretX.Text = "-:桁";
            this.StatusLabel_CaretX.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // StatusLabel_CaretY
            // 
            this.StatusLabel_CaretY.AutoSize = false;
            this.StatusLabel_CaretY.Name = "StatusLabel_CaretY";
            this.StatusLabel_CaretY.Size = new System.Drawing.Size(90, 19);
            this.StatusLabel_CaretY.Text = "-:行";
            this.StatusLabel_CaretY.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // toolStripDropDownBtn
            // 
            this.toolStripDropDownBtn.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripDropDownBtn.AutoSize = false;
            this.toolStripDropDownBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownBtn.Name = "toolStripDropDownBtn";
            this.toolStripDropDownBtn.Size = new System.Drawing.Size(150, 22);
            this.toolStripDropDownBtn.Text = "-";
            this.toolStripDropDownBtn.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.EncodeSelect);
            // 
            // menuStrip1
            // 
            this.menuStrip1.AutoSize = false;
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.menuStrip1.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ファイルFToolStripMenuItem,
            this.EditToolStripMenuItem,
            this.検索SToolStripMenuItem,
            this.設定OToolStripMenuItem,
            this.ウィンドウWToolStripMenuItem,
            this.ヘルプHToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(0);
            this.menuStrip1.Size = new System.Drawing.Size(658, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // ファイルFToolStripMenuItem
            // 
            this.ファイルFToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewMenuItem,
            this.OpenMenuItem,
            this.SaveAsMenuItem,
            this.SaveMenuItem,
            this.toolStripMenuItem2,
            this.CloseMenuItem});
            this.ファイルFToolStripMenuItem.Name = "ファイルFToolStripMenuItem";
            this.ファイルFToolStripMenuItem.Size = new System.Drawing.Size(73, 24);
            this.ファイルFToolStripMenuItem.Text = "ファイル(&F)";
            // 
            // NewMenuItem
            // 
            this.NewMenuItem.Name = "NewMenuItem";
            this.NewMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.NewMenuItem.Size = new System.Drawing.Size(183, 22);
            this.NewMenuItem.Text = "新規作成(&N)";
            this.NewMenuItem.Click += new System.EventHandler(this.NewMenuItem_Click);
            // 
            // OpenMenuItem
            // 
            this.OpenMenuItem.Name = "OpenMenuItem";
            this.OpenMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.OpenMenuItem.Size = new System.Drawing.Size(183, 22);
            this.OpenMenuItem.Text = "開く(&O)...";
            this.OpenMenuItem.Click += new System.EventHandler(this.OpenMenuItem_Click);
            // 
            // SaveAsMenuItem
            // 
            this.SaveAsMenuItem.Name = "SaveAsMenuItem";
            this.SaveAsMenuItem.Size = new System.Drawing.Size(183, 22);
            this.SaveAsMenuItem.Text = "名前をつけて保存(&A)...";
            this.SaveAsMenuItem.Click += new System.EventHandler(this.SaveAsMenuItem_Click);
            // 
            // SaveMenuItem
            // 
            this.SaveMenuItem.Name = "SaveMenuItem";
            this.SaveMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.SaveMenuItem.Size = new System.Drawing.Size(183, 22);
            this.SaveMenuItem.Text = "保存(&S)";
            this.SaveMenuItem.Click += new System.EventHandler(this.SaveMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(180, 6);
            // 
            // CloseMenuItem
            // 
            this.CloseMenuItem.Name = "CloseMenuItem";
            this.CloseMenuItem.Size = new System.Drawing.Size(183, 22);
            this.CloseMenuItem.Text = "閉じる(&X)";
            this.CloseMenuItem.Click += new System.EventHandler(this.CloseMenuItem_Click);
            // 
            // EditToolStripMenuItem
            // 
            this.EditToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Undo,
            this.ToolStripMenuItem_Redo,
            this.toolStripMenuItem1,
            this.CopyMenuItem,
            this.PasteMenuItem,
            this.CutMenuItem,
            this.AllSelectMenuItem,
            this.toolStripMenuItem5,
            this.RectSelectToolStripMenuItem});
            this.EditToolStripMenuItem.Name = "EditToolStripMenuItem";
            this.EditToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.R)));
            this.EditToolStripMenuItem.Size = new System.Drawing.Size(56, 24);
            this.EditToolStripMenuItem.Text = "編集(&E)";
            // 
            // ToolStripMenuItem_Undo
            // 
            this.ToolStripMenuItem_Undo.Name = "ToolStripMenuItem_Undo";
            this.ToolStripMenuItem_Undo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.ToolStripMenuItem_Undo.Size = new System.Drawing.Size(192, 22);
            this.ToolStripMenuItem_Undo.Text = "元に戻す(&U)";
            this.ToolStripMenuItem_Undo.Click += new System.EventHandler(this.ToolStripMenuItem_Undo_Click);
            // 
            // ToolStripMenuItem_Redo
            // 
            this.ToolStripMenuItem_Redo.Name = "ToolStripMenuItem_Redo";
            this.ToolStripMenuItem_Redo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.ToolStripMenuItem_Redo.Size = new System.Drawing.Size(192, 22);
            this.ToolStripMenuItem_Redo.Text = "やり直し(&R)";
            this.ToolStripMenuItem_Redo.Click += new System.EventHandler(this.ToolStripMenuItem_Redo_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(189, 6);
            // 
            // CopyMenuItem
            // 
            this.CopyMenuItem.Name = "CopyMenuItem";
            this.CopyMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.CopyMenuItem.Size = new System.Drawing.Size(192, 22);
            this.CopyMenuItem.Text = "コピー(&C)";
            this.CopyMenuItem.Click += new System.EventHandler(this.CopyMenuItem_Click);
            // 
            // PasteMenuItem
            // 
            this.PasteMenuItem.Name = "PasteMenuItem";
            this.PasteMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.PasteMenuItem.Size = new System.Drawing.Size(192, 22);
            this.PasteMenuItem.Text = "貼り付け(&P)";
            this.PasteMenuItem.Click += new System.EventHandler(this.PasteMenuItem_Click);
            // 
            // CutMenuItem
            // 
            this.CutMenuItem.Name = "CutMenuItem";
            this.CutMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.CutMenuItem.Size = new System.Drawing.Size(192, 22);
            this.CutMenuItem.Text = "切り取り(&T)";
            this.CutMenuItem.Click += new System.EventHandler(this.CutMenuItem_Click);
            // 
            // AllSelectMenuItem
            // 
            this.AllSelectMenuItem.Name = "AllSelectMenuItem";
            this.AllSelectMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.AllSelectMenuItem.Size = new System.Drawing.Size(192, 22);
            this.AllSelectMenuItem.Text = "すべてを選択(&S)";
            this.AllSelectMenuItem.Click += new System.EventHandler(this.AllSelectMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(189, 6);
            // 
            // RectSelectToolStripMenuItem
            // 
            this.RectSelectToolStripMenuItem.Name = "RectSelectToolStripMenuItem";
            this.RectSelectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.R)));
            this.RectSelectToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.RectSelectToolStripMenuItem.Text = "矩形選択";
            this.RectSelectToolStripMenuItem.Click += new System.EventHandler(this.RectSelectToolStripMenuItem_Click);
            // 
            // 検索SToolStripMenuItem
            // 
            this.検索SToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StripMenuItem_Find,
            this.StripMenuItem_NextFind,
            this.StripMenuItem_PrevFind,
            this.toolStripMenuItem6,
            this.LineJumpToolStripMenuItem});
            this.検索SToolStripMenuItem.Name = "検索SToolStripMenuItem";
            this.検索SToolStripMenuItem.Size = new System.Drawing.Size(56, 24);
            this.検索SToolStripMenuItem.Text = "検索(&S)";
            // 
            // StripMenuItem_Find
            // 
            this.StripMenuItem_Find.Name = "StripMenuItem_Find";
            this.StripMenuItem_Find.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.StripMenuItem_Find.Size = new System.Drawing.Size(212, 22);
            this.StripMenuItem_Find.Text = "検索(&F)...";
            this.StripMenuItem_Find.Click += new System.EventHandler(this.StripMenuItem_Find_Click);
            // 
            // StripMenuItem_NextFind
            // 
            this.StripMenuItem_NextFind.Name = "StripMenuItem_NextFind";
            this.StripMenuItem_NextFind.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.StripMenuItem_NextFind.Size = new System.Drawing.Size(212, 22);
            this.StripMenuItem_NextFind.Text = "次を検索(&N)";
            this.StripMenuItem_NextFind.Click += new System.EventHandler(this.StripMenuItem_NextFind_Click);
            // 
            // StripMenuItem_PrevFind
            // 
            this.StripMenuItem_PrevFind.Name = "StripMenuItem_PrevFind";
            this.StripMenuItem_PrevFind.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
            this.StripMenuItem_PrevFind.Size = new System.Drawing.Size(212, 22);
            this.StripMenuItem_PrevFind.Text = "前を検索(&P)";
            this.StripMenuItem_PrevFind.Click += new System.EventHandler(this.StripMenuItem_PrevFind_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(209, 6);
            // 
            // LineJumpToolStripMenuItem
            // 
            this.LineJumpToolStripMenuItem.Name = "LineJumpToolStripMenuItem";
            this.LineJumpToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.J)));
            this.LineJumpToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.LineJumpToolStripMenuItem.Text = "指定行へジャンプ(&J)";
            this.LineJumpToolStripMenuItem.Click += new System.EventHandler(this.LineJumpToolStripMenuItem_Click);
            // 
            // 設定OToolStripMenuItem
            // 
            this.設定OToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FontToolStripMenuItem,
            this.toolStripMenuItem3,
            this.FreeCursorToolStripMenuItem,
            this.RightWrapToolStripMenuItem});
            this.設定OToolStripMenuItem.Name = "設定OToolStripMenuItem";
            this.設定OToolStripMenuItem.Size = new System.Drawing.Size(61, 24);
            this.設定OToolStripMenuItem.Text = "設定（&O）";
            // 
            // FontToolStripMenuItem
            // 
            this.FontToolStripMenuItem.Name = "FontToolStripMenuItem";
            this.FontToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D7)));
            this.FontToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.FontToolStripMenuItem.Text = "フォント(&F)";
            this.FontToolStripMenuItem.Click += new System.EventHandler(this.FontToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(175, 6);
            // 
            // FreeCursorToolStripMenuItem
            // 
            this.FreeCursorToolStripMenuItem.Name = "FreeCursorToolStripMenuItem";
            this.FreeCursorToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F)));
            this.FreeCursorToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.FreeCursorToolStripMenuItem.Text = "フリーカーソル";
            this.FreeCursorToolStripMenuItem.Click += new System.EventHandler(this.FreeCursorToolStripMenuItem_Click);
            // 
            // RightWrapToolStripMenuItem
            // 
            this.RightWrapToolStripMenuItem.Name = "RightWrapToolStripMenuItem";
            this.RightWrapToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.L)));
            this.RightWrapToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.RightWrapToolStripMenuItem.Text = "右端で折返し";
            this.RightWrapToolStripMenuItem.Click += new System.EventHandler(this.RightWrapToolStripMenuItem_Click);
            // 
            // ウィンドウWToolStripMenuItem
            // 
            this.ウィンドウWToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NextWindowToolStripMenuItem,
            this.PrevWindowToolStripMenuItem});
            this.ウィンドウWToolStripMenuItem.Name = "ウィンドウWToolStripMenuItem";
            this.ウィンドウWToolStripMenuItem.Size = new System.Drawing.Size(82, 24);
            this.ウィンドウWToolStripMenuItem.Text = "ウィンドウ(&W)";
            // 
            // NextWindowToolStripMenuItem
            // 
            this.NextWindowToolStripMenuItem.Name = "NextWindowToolStripMenuItem";
            this.NextWindowToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Tab)));
            this.NextWindowToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.NextWindowToolStripMenuItem.Text = "次のウィンドウ(&N)";
            this.NextWindowToolStripMenuItem.Click += new System.EventHandler(this.NextWindowToolStripMenuItem_Click);
            // 
            // PrevWindowToolStripMenuItem
            // 
            this.PrevWindowToolStripMenuItem.Name = "PrevWindowToolStripMenuItem";
            this.PrevWindowToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Tab)));
            this.PrevWindowToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.PrevWindowToolStripMenuItem.Text = "前のウィンドウ(&P)";
            this.PrevWindowToolStripMenuItem.Click += new System.EventHandler(this.PrevWindowToolStripMenuItem_Click);
            // 
            // ヘルプHToolStripMenuItem
            // 
            this.ヘルプHToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem4,
            this.VersionInfoToolStripMenuItem});
            this.ヘルプHToolStripMenuItem.Name = "ヘルプHToolStripMenuItem";
            this.ヘルプHToolStripMenuItem.Size = new System.Drawing.Size(67, 24);
            this.ヘルプHToolStripMenuItem.Text = "ヘルプ(&H)";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(144, 6);
            // 
            // VersionInfoToolStripMenuItem
            // 
            this.VersionInfoToolStripMenuItem.Name = "VersionInfoToolStripMenuItem";
            this.VersionInfoToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.VersionInfoToolStripMenuItem.Text = "バージョン情報";
            this.VersionInfoToolStripMenuItem.Click += new System.EventHandler(this.VersionInfoToolStripMenuItem_Click);
            // 
            // tabPanel
            // 
            this.tabPanel.AllowDrop = true;
            this.tabPanel.BackColor = System.Drawing.Color.DarkGray;
            this.tabPanel.Location = new System.Drawing.Point(0, 24);
            this.tabPanel.Name = "tabPanel";
            this.tabPanel.Size = new System.Drawing.Size(658, 22);
            this.tabPanel.TabIndex = 6;
            this.tabPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.TabPanelPaintEvent);
            // 
            // TextEditor
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(658, 680);
            this.Controls.Add(this.tabPanel);
            this.Controls.Add(this.StatusBar);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "TextEditor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormClosingEvent);
            this.Shown += new System.EventHandler(this.ShownEvent);
            this.SizeChanged += new System.EventHandler(this.customTextBoxSizeChanged);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.DragDropEvent);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.DragEnterEvent);
            this.Move += new System.EventHandler(this.customTextBoxMove);
            this.StatusBar.ResumeLayout(false);
            this.StatusBar.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

#endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.StatusStrip StatusBar;
        private System.Windows.Forms.MenuStrip menuStrip1;
        //private CETextBoxControl.CEEditView customTextBox;
        private System.Windows.Forms.ToolStripMenuItem ファイルFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpenMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveAsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CopyMenuItem;
        private System.Windows.Forms.ToolStripMenuItem PasteMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CloseMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem NewMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AllSelectMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel_CaretX;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel_CaretY;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownBtn;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Undo;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Redo;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 検索SToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem StripMenuItem_Find;
        private System.Windows.Forms.ToolStripMenuItem StripMenuItem_NextFind;
        private System.Windows.Forms.ToolStripMenuItem StripMenuItem_PrevFind;
        private System.Windows.Forms.ToolStripMenuItem ヘルプHToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem VersionInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 設定OToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FontToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FreeCursorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RightWrapToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem RectSelectToolStripMenuItem;
        private System.Windows.Forms.Panel tabPanel;
        private System.Windows.Forms.ToolStripMenuItem ウィンドウWToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem NextWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem PrevWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem LineJumpToolStripMenuItem;
    }
}

