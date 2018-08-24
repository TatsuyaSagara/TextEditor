using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace CustomTextBoxControl
{
    public partial class CustomTextBox : ScrollableControl
    {
        // Public
        public List<string> TextList { set; get; }      // 表示する文字列

        // Private
        private Pen caretPen;

        private int caretColumn;                        // キャレットの配列位置（列）（0インデックス）
        private int caretRow;                           // キャレットの配列位置（行）（0インデックス）
        private int afterCaretColumn;                   // キャレットの移動先の配列位置（列）（0インデックス）
        private int afterCaretRow;                      // キャレットの移動先の配列位置（行）（0インデックス）

        private int viewTopLine;                        // 画面の一番上に表示されている配列の位置
        private int viewLine;                           // 一画面に表示されている行数

        private int viewCaretX, viewCaretY;             // キャレットが表示されている画面上の位置（ピクセル単位）

        private Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");

        private IntPtr hDC;
        private IntPtr hFont;
        private IntPtr hOldFont;

        private const int WM_IME_COMPOSITION = 0x10F;
        private const int WM_IME_ENDCOMPOSITION = 0x10E;
        private const int GCS_RESULTSTR = 0x0800;
        private const int EM_LINESCROLL = 0xB6;

        [DllImport("gdi32.dll")]
        private extern static int TextOut(IntPtr hdc, int nXStart, int nYStart, string lpString, int cbString);
        [DllImport("gdi32.dll")]
        private extern static IntPtr SelectObject(IntPtr hObject, IntPtr hFont);
        [DllImport("gdi32.dll")]
        private extern static bool DeleteObject(System.IntPtr hObject);
        [DllImport("gdi32.dll")]
        static extern bool GetTextExtentPoint32(IntPtr hdc, string lpString, int cbString, out Size lpSize);
        [DllImport("gdi32.dll")]
        static extern uint SetTextColor(IntPtr hdc, int crColor);

        [DllImport("Imm32.dll")]
        private static extern int ImmGetContext(int hWnd);
        [DllImport("Imm32.dll")]
        private static extern int ImmReleaseContext(int hWnd, int hIMC);
        [DllImport("Imm32.dll")]
        private static extern int ImmGetCompositionString(int hIMC, int dwIndex, StringBuilder lpBuf, int dwBufLen);
        [DllImport("Imm32.dll")]
        private static extern int ImmGetOpenStatus(int hIMC);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam);
        private const int WM_SETREDRAW = 0x000B;

        private const int WM_VSCROLL = 0x0115;

        

        // イベント
        // カーソル移動イベント
        public event EventHandler CursorPositionChanged;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CustomTextBox()
        {
            InitializeComponent();

            // 初期化
            InitializeCustomTextBox();

            //MsgListener msgListener = new MsgListener(this);
            //msgListener.Enabled = true;
        }

        private const int SB_LINEUP = 0x00;
        private const int SB_LINEDOWN = 0x1;
        private const int SB_THUMBPOSITION = 0x04;
        private const int SB_PAGEDOWN = 0x03;
        private const int SB_PAGEUP = 0x02;
        private const int SB_THUMBTRACK = 0x05;

        [DllImport("user32.dll")]
        static extern bool UpdateWindow(IntPtr hWnd);

        //'----------------------------------------------------------------
        //' 	読み取得対象コントロールのウィンドウプロシージャ
        //'----------------------------------------------------------------
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            int hIMC;
            int intLength;

            switch (m.Msg)
            { // Select Case m.Msg
                case WM_IME_COMPOSITION:
                    if (((uint)m.LParam & (uint)GCS_RESULTSTR) > 0)
                    {
                        hIMC = ImmGetContext(this.Handle.ToInt32());
                        // 読みの文字列
                        intLength = ImmGetCompositionString(hIMC, GCS_RESULTSTR, null, 0);
                        StringBuilder henkanMoji;
                        if (intLength > 0)
                        {
                            henkanMoji = new StringBuilder(intLength);
                            henkanMoji.Length = intLength;
                            ImmGetCompositionString(hIMC, GCS_RESULTSTR, henkanMoji, intLength);

                            // 入力した文字を現在のキャレット位置に挿入する。
                            string insString = TextList[caretRow].Insert(caretColumn, henkanMoji.ToString());
                            TextList[caretRow] = insString;
                            caretColumn++;

                            string chkLenString = TextList[caretRow].Substring(0, caretColumn).ToString();
                            chkLenString = getNotEolPtr(chkLenString, chkLenString.Length);
                            viewCaretX = this.GetTextPoint(chkLenString).Width;

                            this.Refresh();
                        }
                        ImmReleaseContext(this.Handle.ToInt32(), hIMC);
                    }
                    break;
                case WM_VSCROLL:
                    //Point scpos = this.AutoScrollPosition;
                    //int scposX = -scpos.X;
                    //int scposY = -scpos.Y;

                    int low = m.WParam.ToInt32() & 0xffff;
                    int hiw = (m.WParam.ToInt32() >> 16) & 0xffff;

                    switch (low)
                    {
                        case SB_LINEUP:
                            Console.Write("SB_LINEUP low:[" + low + "] / hiw:[" + hiw + "]\r\n");
                            break;
                        case SB_LINEDOWN:
                            Console.Write("SB_LINEDOWN low:[" + low + "] / hiw:[" + hiw + "]\r\n");
                            break;
                        case SB_THUMBPOSITION:
                            Console.Write(" SB_THUMBPOSITIONlow:[" + low + "] / hiw:[" + hiw + "]\r\n");
                            this.AutoScrollPosition = new Point(0, hiw);
                            break;
                        case SB_PAGEDOWN:
                            Console.Write("SB_PAGEDOWN low:[" + low + "] / hiw:[" + hiw + "]\r\n");
                            break;
                        case SB_PAGEUP:
                            Console.Write("SB_PAGEUP low:[" + low + "] / hiw:[" + hiw + "]\r\n");
                            break;
                        case SB_THUMBTRACK:
                            Console.Write("SB_THUMBTRACK low:[" + low + "] / hiw:[" + hiw + "]\r\n");
                            int aaa = hiw / this.FontHeight;
                            aaa = aaa * this.FontHeight;
                            Console.Write("SB_THUMBTRACK aaa:[" + aaa + "]\r\n");
                            this.AutoScrollPosition = new Point(0, aaa);
                            break;
                        default:
                            Console.Write("default low:[" + low + "] / hiw:[" + hiw + "]\r\n");
                            break;
                    }

                    //    UpdateWindow(this.Handle);

                    //int Scroll = OnVScroll(
                    //    (int)LOWORD(wParam), ((int)HIWORD(wParam)) * m_nVScrollRate);
                    break;
            }
            base.WndProc(ref m);
        }

        #region public

        /// <summary>
        /// キャレットが表示されている列を取得
        /// </summary>
        /// <returns></returns>
        public int GetCaretColumn()
        {
            return caretColumn;
        }

        /// <summary>
        /// キャレットが表示されている行を取得
        /// </summary>
        /// <returns></returns>
        public int GetCaretRow()
        {
            return caretRow;
        }

        /// <summary>
        /// カーソルカラー設定
        /// </summary>
        /// <param name="b">色</param>
        public void SetCaretColor(Brush b)
        {
            caretPen.Brush = b;
        }

        #endregion

        /// <summary>
        /// CustomTextBoxの初期化
        /// </summary>
        private void InitializeCustomTextBox()
        {
            // ダブル・バッファリングの設定
            //this.SetStyle(ControlStyles.OptimizedDoubleBuffer, false);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            // カーソルの座標
            viewCaretX = 0;
            viewCaretY = 0;

            // 移動先のカーソルの位置
            afterCaretColumn = 0;
            afterCaretRow = 0;

            // 現在のカーソルの位置
            caretColumn = 0;
            caretRow = 0;

            // 表示されている行数
            viewLine = 0;

            // 画面に表示されている一番上の配列の位置
            viewTopLine = -1;

            // ペンの初期化
            caretPen = new Pen(Color.Black);

            // 表示テキスト格納エリア確保
            TextList = new List<string>();
        }

        /// <summary>
        /// RGB形式で指定した色を数値に変換
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private int GetColor(int r, int g, int b)
        {
            return r + (g << 8) + (b << 16);
        }

        /// <summary>
        /// 指定した文字列を表示
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="s">表示文字列</param>
        /// <param name="x">表示するX座標</param>
        /// <param name="y">表示するY座標</param>
        /// <param name="c">表示色</param>
        private void ViewTextString(Graphics g, string s, int x, int y ,int c)
        {
            hDC = g.GetHdc();
            hFont = this.Font.ToHfont();
            hOldFont = SelectObject(hDC, hFont);

            SetTextColor(hDC, c);
            TextOut(hDC, x, y, s, sjisEnc.GetByteCount(s));

            DeleteObject(SelectObject(hDC, hOldFont));
            g.ReleaseHdc(hDC);
        }

        /// <summary>
        /// 指定した文字列のピクセル数を取得
        /// </summary>
        /// <param name="g"></param>
        /// <param name="s"></param>
        /// <param name="size"></param>
        private Size GetTextPoint(string s)
        {
            Graphics g = this.CreateGraphics();
            hDC = g.GetHdc();
            hFont = this.Font.ToHfont();
            hOldFont = SelectObject(hDC, hFont);

            Size size;
            int bLen = sjisEnc.GetByteCount(s);
            //Console.Write(bLen + "\r\n");
            GetTextExtentPoint32(hDC, s, bLen, out size);

            DeleteObject(SelectObject(hDC, hOldFont));
            g.ReleaseHdc(hDC);

            return size;
        }

        /// <summary>
        /// 表示文字列から改行コード取得
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private string getEolPtr(string ptr, int length)
        {
            if (length >= 2 && ptr[length - 2] == '\r' && ptr[length - 1] == '\n')
                return ptr.Substring(length - 2);
            if (length >= 1 && (ptr[length - 1] == '\n' || ptr[length - 1] == '\r'))
                return ptr.Substring(length - 1);
            return ptr;
        }

        /// <summary>
        /// 改行を抜いた文字列を返す
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private string getNotEolPtr(string ptr, int length)
        {
            if (length >= 2 && ptr[length - 2] == '\r' && ptr[length - 1] == '\n')
                return ptr.Remove(length - 2);
            if (length >= 1 && (ptr[length - 1] == '\n' || ptr[length - 1] == '\r'))
                return ptr.Remove(length - 1);
            return ptr;
        }

        /// <summary>
        /// 指定位置(ピクセル指定)改行コードを表示する。
        /// </summary>
        /// <param name="g"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void ViewLineBreak(Graphics g, int x, int y)
        {
            int fontWidth = (int)(FontHeight * 0.5);
            int t = fontWidth / 4 + 1;
            int dy = fontWidth / 2;
            g.DrawLine(Pens.CadetBlue, x + fontWidth, y + dy, x + fontWidth, y + FontHeight - dy);
            g.DrawLine(Pens.CadetBlue, x + fontWidth, y + FontHeight - dy, x, y + FontHeight - dy);
            g.DrawLine(Pens.CadetBlue, x, y + FontHeight - dy, x + t, y + FontHeight - dy - t);
            g.DrawLine(Pens.CadetBlue, x, y + FontHeight - dy, x + t, y + FontHeight - dy + t);
        }

        /// <summary>
        /// 全角チェック
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool isZenkaku(string str)
        {
            int num = sjisEnc.GetByteCount(str);
            return num == str.Length * 2;
        }

        /// <summary>
        /// 半角チェック
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool isHankaku(string str)
        {
            int num = sjisEnc.GetByteCount(str);
            return num == str.Length;
        }

        /// <summary>
        /// テキストエリアを描画する。
        /// </summary>
        /// <param name="pe"></param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;

            //////////////////////////////////////////////////////////////////
            // 画面にテキストを出力する。
            // 無駄を省くため見える範囲だけ表示する。
            //////////////////////////////////////////////////////////////////
            Point scpos = this.AutoScrollPosition;
            int scposX = -scpos.X;
            int scposY = -scpos.Y;

            //int ccc = scposY / this.FontHeight;
            //if (viewTopLine == ccc)
            //{
            //    Console.Write("OnPaintキャンセル\r\n");
            //    //base.OnPaint(pe);
            //    return;
            //}

            int line = scposY / this.FontHeight;
            int py = 0;
            int bottom = this.Height;
            for (; line < TextList.Count && py < bottom; line++, py += this.FontHeight)
            {
                string text = TextList[line].ToString();
                string eolptr = getEolPtr(text, text.Length);
                //Console.Write("view\r\n");
                this.ViewTextString(g, text, 0, py, this.GetColor(0x00, 0x00, 0x00));
                if (!string.IsNullOrEmpty(eolptr))
                {    //  改行コードが存在
                    Size tSize = this.GetTextPoint(text.Replace(eolptr,""));
                    int px = tSize.Width;
                    ViewLineBreak(g, px, py);
                }
            }

            // 画面に表示されている行数を取得
            viewLine = this.Height / FontHeight;

            // 画面の一番上の配列の位置
            viewTopLine = scposY / this.FontHeight;

            //////////////////////////////////////////////////////////////////
            // 現在のカーソル位置更新
            //////////////////////////////////////////////////////////////////
            caretRow = afterCaretRow;
            caretColumn = afterCaretColumn;
            afterCaretRow = 0;
            afterCaretColumn = 0;

            //////////////////////////////////////////////////////////////////
            // カーソル表示
            //////////////////////////////////////////////////////////////////
            // "|"カーソル
            g.DrawLine(caretPen, viewCaretX, viewCaretY, viewCaretX, viewCaretY + FontHeight);

            // "□"カーソル
            //g.DrawRectangle(Pens.Blue, cx1, cy1, fontWidth, FontHeight);

            //Console.Write("表示行数：" + viewLine + " / 先頭配列：" + viewTopLine + " / scposY：" + scposY + "\r\n");
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            return;

            Point scpos = this.AutoScrollPosition;
            int scposX = -scpos.X;
            int scposY = -scpos.Y;

            switch (se.Type)
            {
                // ドラッグしたマウスを話したとき
                case ScrollEventType.ThumbPosition:
                    this.AutoScrollPosition = new Point(0, se.OldValue);
                    break;
                // スライドバーがドラッグされた場合
                case ScrollEventType.ThumbTrack:
                    //Console.Write(">---------------\r\n");
                    //Console.Write("scposY:" + scposY + "\r\n");
                    //Console.Write("NewValue:" + se.NewValue + "\r\n");
                    //Console.Write("OldValue:" + se.OldValue + "\r\n");

                    //int bbb = se.NewValue / this.FontHeight;
                    //if(bbb == viewTopLine)
                    //{
                    //    Console.Write("描画停止 NewValue:" + se.NewValue + " / bbb:" + bbb +  " / viewTopLine:" + viewTopLine + "\r\n");
                    //    SendMessage(new HandleRef(this, this.Handle), WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero); this.Refresh();
                    //    return;
                    //}
                    //else
                    //{
                    //    Console.Write("描画開始 NewValue:" + se.NewValue + " / bbb:" + bbb + " / viewTopLine:" + viewTopLine + "\r\n");

                    int pY = se.NewValue / this.FontHeight;
                    pY = pY * this.FontHeight;

                    //int nPos = se.NewValue * this.FontHeight;
                    //int oPos = se.OldValue * this.FontHeight;
                    //int pY = nPos - oPos;

                    this.AutoScrollPosition = new Point(0, pY);

                    //SendMessage(new HandleRef(this, this.Handle), WM_SETREDRAW, new IntPtr(1), IntPtr.Zero); this.Refresh();
                    //}

                    //Console.Write("nPos:" + nPos + " / oPos:" + oPos + " / pY:" + pY + "\r\n");
                    //Console.Write("---------------<\r\n");
                    break;
                default:
                    base.OnScroll(se);
                    break;
            }
        }

        // カーソル移動の検出イベントを発生させるメソッド
        protected virtual void OnCursorPositionChanged(EventArgs e)
        {
            // イベントが未発生であれば、発生させる
            if (CursorPositionChanged != null)
            {
                CursorPositionChanged(this, e);
            }
        }

        /// <summary>
        /// キーイベント
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            //Console.Write("OnPreviewKeyDown() : " + e.KeyCode + "\r\n");

            base.OnPreviewKeyDown(e);

            // スクロールポジション取得
            Point scpos = this.AutoScrollPosition;
            int scposX = -scpos.X;
            int scposY = -scpos.Y;
            
            // 下キー
            if (e.KeyCode == Keys.Down)
            {
                // 配列の一番下の行でなければ移動可能
                if (caretRow < TextList.Count - 1)
                {
                    afterCaretRow = caretRow + 1;
                }
                else
                {
                    afterCaretRow = caretRow;
                }

                afterCaretColumn = caretColumn;

                // 画面に表示されている範囲外か1行分スクロール
                if (afterCaretRow >= viewTopLine + viewLine)
                {
                    if( afterCaretRow < TextList.Count() - 1)
                    {
                        // 画面に表示されている範囲外の場合、
                        this.AutoScrollPosition = new Point(0, scposY + this.FontHeight);
                    }
                    else
                    {
                        // 画面に表示されている範囲外の場合、
                        int ss = this.Height - (this.Height / this.FontHeight * this.FontHeight);
                        this.AutoScrollPosition = new Point(0, scposY + ss);
                    }
                }

                KeyUpDown();

                // 再描画
                this.Refresh();
            }
            // 上キー
            else if (e.KeyCode == Keys.Up)
            {
                // 配列の一番上の行でなければ移動可能
                if (caretRow > 0)
                {
                    afterCaretRow = caretRow - 1;
                }
                else
                {
                    afterCaretRow = caretRow;
                }

                afterCaretColumn = caretColumn;

                // 画面外のとき
                if (afterCaretRow < viewTopLine)
                {
                    this.AutoScrollPosition = new Point(0, scposY - FontHeight);
                }

                KeyUpDown();

                // 再描画
                this.Refresh();
            }
            // 右キー
            else if (e.KeyCode == Keys.Right)
            {
                // 一番右でなければ処理する。
                string text = TextList[caretRow];
                if (caretColumn < getNotEolPtr(text.ToString(), text.Length).Length)
                {
                    afterCaretColumn = caretColumn + 1;
                    afterCaretRow = caretRow;
                }
                else
                {
                    afterCaretColumn = 0;
                    afterCaretRow = caretRow + 1;
                    viewCaretX = 0;
                }

                KeyRightLeft();

                // 再描画
                this.Refresh();
            }
            // 左キー
            else if (e.KeyCode == Keys.Left)
            {
                // 一番左でなければ処理する。
                if (caretColumn > 0)
                {
                    afterCaretColumn = caretColumn - 1;
                    afterCaretRow = caretRow;
                }
                else
                {

                    string text = TextList[caretRow - 1].ToString();
                    text = getNotEolPtr(text, text.Length);
                    afterCaretColumn = text.Count() - 1;
                    afterCaretRow = caretRow - 1; ;
                    viewCaretY = this.GetTextPoint(text).Width;
                }

                KeyRightLeft();

                // 再描画
                this.Refresh();
            }
            // カーソルが移動した場合にイベントを発生させる。
            if( e.KeyCode == Keys.Down ||
                e.KeyCode == Keys.Up ||
                e.KeyCode == Keys.Right ||
                e.KeyCode == Keys.Left )
            {
                OnCursorPositionChanged(e);
            }

            if (e.KeyCode == Keys.ProcessKey)
            {
                // 入力した文字を現在のキャレット位置に挿入する。
                //string insString = TextList[caretRow].Insert(caretColumn, e.KeyChar.ToString());
                //TextList[caretRow] = insString;
                //caretColumn++;

                //string chkLenString = TextList[caretRow].Substring(0, caretColumn).ToString();
                //chkLenString = getNotEolPtr(chkLenString, chkLenString.Length);
                //viewCaretX = this.GetTextPoint(chkLenString).Width;

                //this.Refresh();
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            //Console.Write("OnKeyPress() : " + e.KeyChar + "\r\n");

            var hIMC = ImmGetContext(this.Handle.ToInt32());
            var status = ImmGetOpenStatus(hIMC);
            ImmReleaseContext(this.Handle.ToInt32(), hIMC);

            // 日本IMEがOFFの場合
            if( status == 0)
            {
                // 入力した文字を現在のキャレット位置に挿入する。
                string insString = TextList[caretRow].Insert(caretColumn, e.KeyChar.ToString());
                TextList[caretRow] = insString;
                caretColumn++;

                string chkLenString = TextList[caretRow].Substring(0, caretColumn).ToString();
                chkLenString = getNotEolPtr(chkLenString, chkLenString.Length);
                viewCaretX = this.GetTextPoint(chkLenString).Width;

                afterCaretRow = caretRow;
                afterCaretColumn = caretColumn;

                this.Refresh();
            }
        }

        /// <summary>
        /// カーソルの上下移動
        /// </summary>
        private void KeyUpDown()
        {
            // 行設定
            string chkLenString = string.Empty;
            int diffSize = 0;

            // 現在の行よりも移動先の行が長い場合、移動先の位置（列）を取得する。
            string t = getNotEolPtr(TextList[afterCaretRow].ToString(), TextList[afterCaretRow].Length);
            int w = this.GetTextPoint(t).Width;
            if (viewCaretX <= w )
            {
                // 移動先の行の文字数分ループ
                int textIndex = 0;
                int afterLenRow = TextList[afterCaretRow].Count();
                for (; textIndex < afterLenRow; textIndex++)
                {
                    chkLenString += TextList[afterCaretRow][textIndex].ToString();
                    chkLenString = getNotEolPtr(chkLenString, chkLenString.Length);
                    int chkStrWidth = this.GetTextPoint(chkLenString).Width;
                    if (chkStrWidth > viewCaretX)
                    {
                        int leftSpace = viewCaretX - diffSize;
                        int rightSpace = chkStrWidth - viewCaretX;
                        if (leftSpace > rightSpace)
                        {
                            afterCaretColumn = textIndex + 1;
                        }
                        else
                        {
                            afterCaretColumn = textIndex;
                        }
                        break;
                    }
                    else
                    {
                        diffSize = chkStrWidth;
                    }
                }
            }
            // 現在の行よりも移動先の行が短い場合、移動先の行の一番最後にキャレットを移動する。
            else
            {
                afterCaretColumn = getNotEolPtr(TextList[afterCaretRow].ToString(), TextList[afterCaretRow].Length).Length;
            }

            // 列設定
            Size size = this.GetTextPoint(TextList[afterCaretRow].Substring(0, afterCaretColumn));

            // 画面の一番上の配列の位置
            Point scpos = this.AutoScrollPosition;
            int scposX = -scpos.X;
            int scposY = -scpos.Y;
            viewTopLine = scposY / this.FontHeight;

            // カーソル位置を設定
            viewCaretX = size.Width;
            viewCaretY = (afterCaretRow - viewTopLine/*移動量*/) * FontHeight;
        }

        /// <summary>
        /// カーソルの左右移動
        /// </summary>
        private void KeyRightLeft()
        {
            // 列設定
            Size size = this.GetTextPoint(TextList[afterCaretRow].Substring(0, afterCaretColumn));

            // カーソル位置を設定
            viewCaretX = size.Width;
            viewCaretY = (afterCaretRow - viewTopLine) * FontHeight;
        }
    }
}
