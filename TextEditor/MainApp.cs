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
    public class MainApp : Form
    {
        private NotifyIcon notifyIcon;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem QuitToolStripMenuItem;
        private System.ComponentModel.IContainer components;

        // ウィンドウ管理情報
        public List<TextEditor> m_viewTextEditor;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainApp()
        {
            ;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainApp(string[] openFilename)
        {
            // フォーム初期化
            InitializeComponent();

            // フォーム非表示対応
            WindowState = FormWindowState.Minimized;    // 最小化
            ShowInTaskbar = false;                      // タスクバーに表示しない

            // ウィンドウ管理情報インスタンス作成
            m_viewTextEditor = new List<TextEditor>();

            if (openFilename.Length == 0)
            {
                // 新規ビュー作成
                NewTabView();
            }
            else
            {
                // ファイル読込
                for (int idx = 0; idx < openFilename.Length; idx++)
                {
                    OpenTabView(openFilename[idx]);
                }
            }
        }

        /// <summary>
        /// 新規タブビュー作成
        /// </summary>
        /// <remarks>
        /// フォームがない場合はフォームを作成しタブビューを追加し、
        /// フォームがある場合は管理上最後のフォームにタブビューを追加する。
        /// </remarks>
        public void NewTabView()
        {
            if (m_viewTextEditor.Count == 0)
            {
                TextEditor te = new TextEditor(this);
                te.FormClosing += new FormClosingEventHandler(this.FormClosingEvent);
                m_viewTextEditor.Add(te);
            }

            m_viewTextEditor[m_viewTextEditor.Count - 1].NewTabView();
            m_viewTextEditor[m_viewTextEditor.Count - 1].Show();

            // 追加タブボタン追加
            //if (m_viewTextEditor.Count == 1)
            //if (m_viewTextEditor[m_viewTextEditor.Count - 1].m_viewInfoMng.Count == 1)
            //{
            m_viewTextEditor[m_viewTextEditor.Count - 1].AddTabButton();
            //}
        }

        /// <summary>
        /// 新規ビュー作成しファイル読込
        /// </summary>
        /// <remarks>
        /// フォームがない場合はフォームを作成しタブビューを追加しファイルを読込み、
        /// フォームがある場合は管理上最後のフォームにタブビューを追加しファイルを読込む。
        /// タブビュー表示完了後（フォームのShownイベント）にファイルを読込むためファイル名をクラスメソッドに一時保存
        /// </remarks>
        /// <param name="fileName">開くファイル名</param>
        public void OpenTabView(string fileName)
        {
            if (m_viewTextEditor.Count == 0)
            {
                TextEditor te = new TextEditor(this);
                te.FormClosing += new FormClosingEventHandler(this.FormClosingEvent);
                //te.Shown += new EventHandler(this.FormShownEvent);
                te.openFilePath = fileName;
                m_viewTextEditor.Add(te);
            }
            else
            {
                m_viewTextEditor[m_viewTextEditor.Count - 1].OpenTabView(fileName);
            }
            
            m_viewTextEditor[m_viewTextEditor.Count - 1].Show();
        }

        /// <summary>
        /// 新規ウィンドウにタブビュー分離
        /// </summary>
        /// <param name="editView">分離するビュー</param>
        /// <param name="fileName">分離するファイル名</param>
        public void TabViewSeparation(CEEditView editView, string fileName)
        {
            TextEditor te = new TextEditor(this);
            te.FormClosing += new FormClosingEventHandler(this.FormClosingEvent);
            m_viewTextEditor.Add(te);

            m_viewTextEditor[m_viewTextEditor.Count - 1].AddTabView(editView, fileName);
            m_viewTextEditor[m_viewTextEditor.Count - 1].Show();
        }

        /// <summary>
        /// 別ウィンドウにタブビューを移動
        /// </summary>
        /// <param name="te"></param>
        /// <param name="ev"></param>
        /// <param name="fn"></param>
        public void TabViewAnotherWinMove(TextEditor te, CEEditView ev, string fn)
        {
            for (int idx = 0; idx < m_viewTextEditor.Count; idx++)
            {
                if (m_viewTextEditor[idx] == te)
                {
                    m_viewTextEditor[idx].AddTabView(ev, fn);
                    m_viewTextEditor[idx].Show();
                    break;
                }
            }
        }

        /// <summary>
        /// 初期化
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainApp));
            this.notifyIcon = new NotifyIcon(this.components);
            this.contextMenuStrip = new ContextMenuStrip(this.components);
            this.QuitToolStripMenuItem = new ToolStripMenuItem();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipText = "TextEditor";
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
            //this.notifyIcon.Icon = (Icon)resources.GetObject("notifyIcon.Icon");
            //this.notifyIcon.Icon = new Icon("TextEditor.ico");
            this.notifyIcon.Text = "notifyIcon";
            this.notifyIcon.Visible = true;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.QuitToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(218, 48);
            // 
            // QuitToolStripMenuItem
            // 
            this.QuitToolStripMenuItem.Name = "QuitToolStripMenuItem";
            this.QuitToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.QuitToolStripMenuItem.Text = "TextEditorの全終了（&Q）";
            this.QuitToolStripMenuItem.Click += new System.EventHandler(this.QuitToolStripMenuItem_Click);
            // 
            // MainApp
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "MainApp";
            this.Opacity = 0D;
            this.Shown += new System.EventHandler(this.ShownEvent);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        /// <summary>
        /// ポインタがクライアント領域内かチェック
        /// </summary>
        /// <param name="sp">画面上のポインタの位置</param>
        /// <returns>1:アクティブフォーム／2:多フォーム／3:その他</returns>
        public int isPointClientArea(Point sp, out TextEditor form)
        {
            TextEditor te = (TextEditor)ActiveForm;
            if (ActiveForm == null)
            {
                form = null;
                return -1;
            }

            if (sp.X < te.Left || sp.Y < te.Top || sp.X > (te.Left + te.Width) || sp.Y > (te.Top + te.Height))
            {
                foreach (TextEditor t in m_viewTextEditor)
                {
                    if (ActiveForm == t)
                    {
                        continue;
                    }

                    if (sp.X > t.Left && sp.Y > t.Top && sp.X < (t.Left + t.Width) && sp.Y < (t.Top + t.Height))
                    {
                        // 他フォーム

                        form = t;
                        return 2;
                    }
                }

                // その他

                form = null;
                return 3;
            }
            else
            {
                // アクティブフォーム

                form = null;
                return 1;
            }
        }

        /// <summary>
        /// フォームクローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormClosingEvent(object sender, FormClosingEventArgs e)
        {
            if (e.Cancel)
            {
                return;
            }

            TextEditor te = ((TextEditor)sender);
            te.Dispose();
            m_viewTextEditor.Remove(te);
            this.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormShownEvent(object sender, EventArgs e)
        {
//            m_viewTextEditor[m_viewTextEditor.Count - 1].OpenTabView(m_svOpenFilePath);
//            m_svOpenFilePath = "";
        }

        /// <summary>
        /// 終了イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QuitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;

            foreach(TextEditor te in m_viewTextEditor)
            {
                te.Dispose();
            }
            m_viewTextEditor.Clear();

            Application.Exit();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShownEvent(object sender, EventArgs e)
        {
            // フォーム非表示対応
            this.Visible = false;
        }
    }
}
