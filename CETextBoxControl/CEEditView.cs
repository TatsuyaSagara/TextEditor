﻿using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Forms.Layout;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using System.Timers;
using System.Resources;

namespace CETextBoxControl
{
    /// <summary>
    /// CaretPosイベントで返されるデータ
    /// ここではstring型のひとつのデータのみ返すものとする
    /// </summary>
    public class CaretPositionEventArgs : EventArgs
    {
        public int x;   // 列
        public int y;   // 行
        public string encode; // エンコード
    }

    public class CEEditView : Control
    {
        /// <summary>
        /// キャレットポジション用デリゲートの宣言
        /// CaretPositionEventArgs型のオブジェクトを返すようにする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void CaretPositionEventHandler(object sender, CaretPositionEventArgs e);

        /// <summary>
        /// キャレットポジション用イベントデリゲートの宣言
        /// </summary>
        public event CaretPositionEventHandler CaretPos;

        /// <summary>
        /// 検索状態
        /// </summary>
        private int m_searchType = NOT_SEARCH_TYPE;

        /// <summary>
        /// 未検索：検索状態
        /// </summary>
        private const int NOT_SEARCH_TYPE = -1;

        /// <summary>
        /// 検索：検索状態
        /// </summary>
        private const int SEARCH_TYPE = 1;

        /// <summary>
        /// 選択範囲状態
        /// </summary>
        private int m_selectType = NONE_RANGE_SELECT;

        /// <summary>
        /// 未選択：選択範囲状態
        /// </summary>
        private const int NONE_RANGE_SELECT = -1;

        /// <summary>
        /// 通常選択：選択範囲状態
        /// </summary>
        private const int NORMAL_RANGE_SELECT = 0;

        /// <summary>
        /// 矩形選択：選択範囲状態
        /// </summary>
        private const int RACTANGLE_RANGE_SELECT = 1;

        /// <summary>
        /// 内部エンコード（UTF-16LE）
        /// </summary>
        private Encoding m_internalEncode = Encoding.GetEncoding(Ude.Charsets.UTF16_LE);

        /// <summary>
        /// 読込エンコード
        /// </summary>
        private Encoding m_readEncode = Encoding.GetEncoding(Ude.Charsets.SHIFT_JIS);

        /// <summary>
        /// 画面表示幅（ピクセル）
        /// </summary>
        private int m_screenWidth;

        /// <summary>
        /// 画面に表示される行数
        /// </summary>
        private int m_viewDispRow;

        /// <summary>
        /// 画面に表示される列数
        /// </summary>
        private int m_viewDispColumn;

        /// <summary>
        /// 画面に表示される一番上の論理行(0インデックス)
        /// </summary>
        private int m_viewTopRow;

        /// <summary>
        /// 現在一番上に表示されている位置（論理位置）
        /// </summary>
        private int m_viewTopLineL;

        /// <summary>
        /// 現在一番上に表示されている位置（論理行内の物理位置）
        /// </summary>
        private int m_viewTopLineLP;

        /// <summary>
        /// 画面に表示される一番左の列(0インデックス)
        /// （半角単位）
        /// </summary>
        private int m_viewLeftColumn;

        /// <summary>
        /// キャレット位置(0インデックス）
        /// (物理位置)
        /// </summary>
        private Point m_caretStrBuf;

        /// <summary>
        /// キャレットの配列位置(0インデックス)
        /// レイアウト（改行した場合）位置
        /// </summary>
        private Point m_caretStrBufLayout;

        /// <summary>
        /// キャレットのピクセル座標（0インデックス）（文字列の左端が0）
        /// </summary>
        private Point m_caretPixel;

        /// <summary>
        /// 垂直スクロールバーの縮尺
        /// </summary>
        private int m_nVScrollRate;
#if false
        /// <summary>
        /// 水平スクロールバーの縮尺
        /// </summary>
        public int m_nHScrollRate;
#endif
#if false // スクロールバーの設定（LayoutScrollBar）で値を設定しているが使用していないのでコメントアウト
        /// <summary>
        /// 画面に表示される桁数（ピクセル）
        /// </summary>
        private int m_viewDispColPixel;
#endif

        /// <summary>
        /// タブサイズ（ピクセル）
        /// </summary>
        private int m_tabWidthPixel
        {
            get
            {
                return (m_ShareData.m_charWidthPixel + m_ShareData.m_nColumnSpace) * m_ShareData.m_tabLength;
            }
        }

        /// <summary>
        /// 垂直(縦)スクロールバー
        /// </summary>
#if false
        private CEWin32Api.SCROLLINFO m_scrollInfoV;
#else
        private HScrollBar m_hScrollBar;
#endif

        /// <summary>
        /// 水平(横)スクロールバー
        /// </summary>
#if false
        private CEWin32Api.SCROLLINFO m_scrollInfoH;
#else
        private VScrollBar m_vScrollBar;
#endif

        /// <summary>
        /// 範囲選択開始位置(物理)
        /// </summary>
        private Point m_sRng;

        /// <summary>
        /// 範囲選択終了位置(物理)
        /// </summary>
        private Point m_eRng;

        /// <summary>
        /// 範囲選択起点位置(物理)
        /// </summary>
        private Point m_cRng;

        /// <summary>
        /// ドラッグ中フラグ
        /// </summary>
        private Boolean m_dragFlg;

        /// <summary>
        /// UndoRedo 操作用
        /// </summary>
        private CEUndoRedoOpe UndoRedoOpe;

        /// <summary>
        /// 基準キャレット位置(ピクセル)(桁)(文字列の左端が0)
        /// </summary>
        private int m_curCaretColPosPixel;

        /// <summary>
        /// 矩形選択の開始基準キャレット位置(ピクセル)(桁)(文字列の左端が0)
        /// </summary>
        private int m_rectCaretColPosPixel;

        /// <summary>
        /// 行番号
        /// </summary>
        private List<string> m_lLineNum;

        /// <summary>
        /// メモリデバイスコンテキスト
        /// (文字列や文字の幅を算出するのに必要)
        /// </summary>
        private IntPtr m_hDrawDC;

#if false
        /// <summary>
        /// 編集文字列をタイトルバーに表示しているか
        /// true:表示している / false:表示していない
        /// </summary>
        private Boolean editDispFlag;
#endif

        /// <summary>
        /// 矩形選択用位置（開始）
        /// </summary>
        private int m_sRectIdx;

        /// <summary>
        /// 矩形選択用位置（終了）
        /// </summary>
        private int m_eRectIdx;

        /// <summary>
        /// 共通データ
        /// </summary>
        CEShareData m_ShareData;

        /// <summary>
        /// 元のウィンドウプロシージャ
        /// </summary>
        private IntPtr m_originalWndProcObj = IntPtr.Zero;

        /// <summary>
        /// ウィンドウプロシージャ
        /// </summary>
        private CEWin32Api.WNDPROC m_customWndProcObj = null;

        /// <summary>
        /// IME情報
        /// </summary>
        private CEInputMethod ime;

        /// <summary>
        /// ドキュメント
        /// ★アンドゥ・リドゥクラスが使用しているのでpublic宣言...(-.-)
        /// </summary>
        public CEDocument m_doc;

        /// <summary>
        /// タブ幅（ピクセル）
        /// </summary>
        private int m_tabPixel
        {
            get
            {
                return m_ShareData.m_tabLength * m_ShareData.m_charWidthPixel;
            }
        }

        /// <summary>
        /// 表示開始位置（列）（ピクセル）
        /// </summary>
        private int m_startColPos
        {
            get
            {
                return m_ShareData.m_charWidthPixel * 7;
            }
        }

        /// <summary>
        /// 表示開始位置（行）（ピクセル）
        /// </summary>
        private int m_startRowPos
        {
            get
            {
                return (int)(double)(m_ShareData.m_charHeightPixel * 1.1);
            }
        }

        /// <summary>
        /// トリプルクリック判定用カウンタ
        /// </summary>
        int m_clickFlag = 0;

        /// <summary>
        /// トリプルクリック判定用タイマー
        /// </summary>
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CEEditView()
        {
            // オフスクリーン描画用デバイスコンテキスト作成
            // ★解放のタイミングわからず。アプリ終了するまで解放しないからほっといていいのかな？
            m_hDrawDC = CEWin32Api.CreateCompatibleDC((IntPtr)0);

            // 初期化
            Init();

            // Undo/Redo 情報バッファ
            UndoRedoOpe = new CEUndoRedoOpe();

            // IME制御
            ime = new CEInputMethod();

            // 共有データ
            m_ShareData = CEShareData.GetInstance();

            m_hScrollBar = new HScrollBar();
            m_vScrollBar = new VScrollBar();

            Controls.Add(m_hScrollBar);
            Controls.Add(m_vScrollBar);

            // タイマー生成
            timer.Tick += new EventHandler(this.OnTick_FormsTimer);
            timer.Interval = 350;
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~CEEditView()
        {
            // オフスクリーン描画用オブジェクト開放
            if (m_hDrawDC != (IntPtr)0)
            {
                CEWin32Api.DeleteObject(m_hDrawDC);
                m_hDrawDC = (IntPtr)0;
            }
        }

        /// <summary>
        /// 初期化
        /// </summary>
        private void Init()
        {
            // 表示位置
            m_viewTopRow = 0;
            m_viewTopLineL = 0;
            m_viewTopLineLP = 0;
            m_viewLeftColumn = 0;

            // キャレットの読込文字列の配列位置
            m_caretStrBuf.X = 0;
            m_caretStrBuf.Y = 0;
            m_curCaretColPosPixel = 0;
            m_caretStrBufLayout = m_caretStrBuf;

            // キャレットのピクセル座標
            m_caretPixel.X = 0;
            m_caretPixel.Y = 0;

            // ドラッグ中フラグ
            m_dragFlg = false;

            // 範囲選択情報（通常選択）
            m_sRng.X = -1;
            m_sRng.Y = -1;
            m_eRng.X = -1;
            m_eRng.Y = -1;
            m_cRng.X = -1;
            m_cRng.Y = -1;

            // 範囲選択情報（矩形選択）
            m_sRectIdx = -1;
            m_eRectIdx = -1;

            // 範囲選択状態
            m_selectType = NONE_RANGE_SELECT;
        }

        /// <summary>
        /// フォントの設定
        /// </summary>
        public void SetFont(Font f)
        {
            // 共有データが無い場合は何もしない。
            if (m_ShareData == null)
            {
                MessageBox.Show("Share data is NULL. ");
                return;
            }

            // フォントサイズ設定
            Size sz;
            IntPtr m_fontHandle = f.ToHfont();
            IntPtr m_oldFontHandle = CEWin32Api.SelectObject(m_hDrawDC, m_fontHandle);
            string StandardString = "大";
            CEWin32Api.GetTextExtentPoint32(m_hDrawDC, StandardString, StandardString.Length, out sz);
            m_ShareData.m_charHeightPixel = sz.Height;      // フォントの縦サイズ
            m_ShareData.m_charWidthPixel = sz.Width / 2;    // フォントの横サイズ（半角）

            // 共有データにフォント情報保存
            m_ShareData.m_font = f;
        }

        /// <summary>
        /// フォント取得
        /// </summary>
        public Font GetFont()
        {
            return m_ShareData.m_font;
        }

        /// <summary>
        /// 縦スクロールバーサイズ取得
        /// </summary>
        /// <returns></returns>
        public Size GetVScrollbarSize()
        {
            if (m_vScrollBar == null)
            {
                return new Size(0, 0);
            }
            return new Size(m_vScrollBar.Width, m_vScrollBar.Height);
        }

        /// <summary>
        /// 横スクロールバーサイズ取得
        /// </summary>
        /// <returns></returns>
        public Size GetHScrollbarSize()
        {
            if (m_hScrollBar == null)
            {
                return new Size(0, 0);
            }
            return new Size(m_hScrollBar.Width, m_hScrollBar.Height);
        }

        /// <summary>
        /// ウィンドウプロシージャ
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case CEWin32Api.WM_CREATE:
                    if (m_originalWndProcObj == IntPtr.Zero)
                    {
                        m_originalWndProcObj = CEWin32Api.GetWindowLong(this.Handle, CEWin32Api.GWLP_WNDPROC);
                    }
                    if (m_customWndProcObj == null)
                    {
                        m_customWndProcObj = new CEWin32Api.WNDPROC(this.CustomWndProc);
                    }
                    CEWin32Api.SetWindowLong(this.Handle, CEWin32Api.GWLP_WNDPROC, m_customWndProcObj);

                    // 初期値設定
                    SetFont(m_ShareData.m_font);
                    m_ShareData.m_wrapPositionPixel = CEConstants.DefaultWrapSize;

                    // ビュー設定
                    Cursor = Cursors.IBeam;

                    // ドキュメント設定
                    m_doc = new CEDocument(m_hDrawDC);

                    this.Select();
                    this.Focus();

                    break;
                case CEWin32Api.WM_SHOWWINDOW:
                    this.Select();
                    this.Focus();
                    break;
                case CEWin32Api.WM_DESTROY:
                    // WM_QUITメッセージを送り出しWinMain関数のループを終了する。
                    //CEWin32Api.PostQuitMessage(0);
                    break;
                case CEWin32Api.WM_CLOSE:
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        /// <summary>
        /// カスタムウィンドウプロシージャ
        /// すべてのイベントを受け付けるため
        /// </summary>
        /// <param name="window"></param>
        /// <param name="message"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private IntPtr CustomWndProc(IntPtr window, Int32 message, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                //Debug.WriteLine(this.Name + " / " + message.ToString("x4"));
                switch (message)
                {
                    // フォーカス取得
                    case CEWin32Api.WM_SETFOCUS:
                        // キャレット表示
                        ShowCaret();
                        break;
                    // フォーカス喪失
                    case CEWin32Api.WM_KILLFOCUS:
                        // キャレット非表示
                        HideCaret();
                        break;
                    // ウィンドウサイズ
                    case CEWin32Api.WM_SIZE:
                        // 起動時、何回か呼ばれる。スクロールバーを付ける前と付ける後などで。
                        // スクロールバーの設定
                        LayoutScrollBar();
                        // 再描画（★本来は不要だと思われるが、ここで再描画しないとルーラーが正常に表示されない）
                        CEWin32Api.InvalidateRect(this.Handle, IntPtr.Zero, false);
                        //this.Refresh();
                        break;
                    // 縦スクロール
                    case CEWin32Api.WM_VSCROLL:
                        MoveVScroll(wParam);
                        break;
                    // 横スクロール
                    case CEWin32Api.WM_HSCROLL:
                        MoveHScroll(wParam);
                        break;
                    // 描画
                    case CEWin32Api.WM_PAINT:
                        DrawTextList();
                        break;
                    // 文字入力
                    case CEWin32Api.WM_CHAR:
                        CharKey((int)wParam);
                        break;
                    // キー押下
                    case CEWin32Api.WM_KEYDOWN:
                        DownKey(wParam);
                        break;
                    // マウスホイール
                    case CEWin32Api.WM_MOUSEWHEEL:
                        MoveScrollV((int)(CEConstants.MAMouseScroll * CEWin32Api.GET_WHEEL_DELTA_WPARAM(wParam) / CEWin32Api.WHEEL_DELTA));
                        break;
                    // マウス左ボタン押下
                    case CEWin32Api.WM_LBUTTONDOWN:
                        MouseLeftClick(lParam);
                        break;
                    // マウス左ボタン離す
                    case CEWin32Api.WM_LBUTTONUP:
                        MouseLeftUp(lParam);
                        break;
                    // マウス移動
                    case CEWin32Api.WM_MOUSEMOVE:
                        MousePointerMove(lParam);
                        break;
                    // マウス左ダブルクリック
                    case CEWin32Api.WM_LBUTTONDBLCLK:
                        MouseLeftDoubleClick(lParam);
                        break;
                    // 何のためにある？ここを消すとタブが入力できなくなるのだが。
                    case CEWin32Api.WM_GETDLGCODE:
                        return new IntPtr(CEWin32Api.DLGC_WANTALLKEYS);
                    // IME関連
                    case CEWin32Api.WM_IME_NOTIFY:
                        if ((int)wParam == CEWin32Api.IMN_OPENCANDIDATE)
                        {
                            ime.SetCandidateWindow(this.Handle);
                        }
                        break;
                    // IME関連
                    case CEWin32Api.WM_IME_STARTCOMPOSITION:
                        ime.SetCompositionWindow(this.Handle, m_ShareData.m_font);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                MessageBox.Show(ex.ToString(), "!!! Text Editor Fail. !!!");
            }

            return CEWin32Api.CallWindowProc(m_originalWndProcObj, window, message, wParam, lParam);
        }

        /// <summary>
        /// 縦スクロール移動処理
        /// </summary>
        /// <param name="wParam"></param>
        private void MoveVScroll(IntPtr wParam)
        {
            int nPos = 0;
            int low = CEWin32Api.LOWORD(wParam);
            switch (low)
            {
                // 1 行上へスクロール（スクロールの上矢印選択）
                case CEWin32Api.SB_LINEUP:
                    nPos = -1;
                    break;
                // 1 行下へスクロール（スクロールの下矢印選択）
                case CEWin32Api.SB_LINEDOWN:
                    nPos = 1;
                    break;
                // 1 ページ上へスクロール（スクロールの上矢印とつまみの間を選択）
                case CEWin32Api.SB_PAGEUP:
                    nPos = (int)(-1 * m_vScrollBar.LargeChange/*m_scrollInfoV.nPage*/);
                    break;
                // 1 ページ下へスクロール（スクロールの下矢印とつまみの間を選択）
                case CEWin32Api.SB_PAGEDOWN:
                    nPos = (int)m_vScrollBar.LargeChange/*m_scrollInfoV.nPage*/;
                    break;
                // 絶対位置へスクロール
                case CEWin32Api.SB_THUMBPOSITION:
                    nPos = (CEWin32Api.HIWORD(wParam)/* * m_nHScrollRate*/) - m_vScrollBar.Value/*m_scrollInfoV.nPos*/;
                    break;
                // 指定位置へスクロール ボックスをドラッグ
                case CEWin32Api.SB_THUMBTRACK:
                    nPos = (CEWin32Api.HIWORD(wParam) * m_nVScrollRate) - m_viewTopRow;
                    break;
                // スクロール終了
                case CEWin32Api.SB_ENDSCROLL:
                    nPos = 0;
                    break;
                // 一番上までスクロール
                case CEWin32Api.SB_TOP:
                    nPos = 0;
                    break;
                // 一番下までスクロール
                case CEWin32Api.SB_BOTTOM:
                    nPos = 0;
                    break;
                default:
                    nPos = 0;
                    break;
            }

            // カーソルをデフォルトに戻す
            Cursor = Cursors.Default;

            // 縦スクロール
            MoveScrollV(nPos);
        }

        /// <summary>
        /// 横スクロール移動処理
        /// </summary>
        /// <param name="wParam"></param>
        private void MoveHScroll(IntPtr wParam)
        {
            int nPos = 0;
            int low = CEWin32Api.LOWORD(wParam);
            switch (low)
            {
                // 左へスクロール（スクロールの左矢印選択）
                case CEWin32Api.SB_LINELEFT:
                    nPos = -1;
                    break;
                // 右へスクロール（スクロールの右矢印選択）
                case CEWin32Api.SB_LINERIGHT:
                    nPos = 1;
                    break;
                // 1 ページ左へスクロール（スクロールの左矢印とつまみの間を選択）
                case CEWin32Api.SB_PAGELEFT:
                    nPos = (int)(-1 * m_hScrollBar.LargeChange/*m_scrollInfoH.nPage*/);
                    break;
                // 1 ページ右へスクロール（スクロールの右矢印とつまみの間を選択）
                case CEWin32Api.SB_PAGERIGHT:
                    nPos = (int)m_hScrollBar.LargeChange/*m_scrollInfoH.nPage*/;
                    break;
                // 絶対位置へスクロール
                case CEWin32Api.SB_THUMBPOSITION:
                    nPos = CEWin32Api.HIWORD(wParam) - m_hScrollBar.Value/*m_scrollInfoH.nPos*/;
                    break;
                // 指定位置へスクロール ボックスをドラッグ
                case CEWin32Api.SB_THUMBTRACK:
                    nPos = (CEWin32Api.HIWORD(wParam)) - m_viewLeftColumn;
                    break;
                // スクロール終了
                case CEWin32Api.SB_ENDSCROLL:
                    nPos = 0;
                    break;
                // 左端へスクロール
                case CEWin32Api.SB_LEFT:
                    nPos = 0;
                    break;
                // 右端へスクロール
                case CEWin32Api.SB_RIGHT:
                    nPos = 0;
                    break;
                default:
                    nPos = 0;
                    break;
            }

            // カーソルをデフォルトに戻す
            Cursor = Cursors.Default;

            // 横スクロール
            MoveScrollH(nPos);
        }

        /// <summary>
        /// キー押下処理
        /// </summary>
        /// <param name="wParam"></param>
        private void DownKey(IntPtr wParam)
        {
            int nVKey = (int)wParam;
            if (nVKey == CEWin32Api.VK_CTRL)
            {
                ;
            }
            // 下キー
            if (nVKey == CEWin32Api.VK_DOWN)
            {
                CursorKeyV(1);
            }
            // 上キー
            else if (nVKey == CEWin32Api.VK_UP)
            {
                CursorKeyV(-1);
            }
            // 右キー
            else if (nVKey == CEWin32Api.VK_RIGHT)
            {
                CursorKeyH(1);
            }
            // 左キー
            else if (nVKey == CEWin32Api.VK_LEFT)
            {
                CursorKeyH(-1);
            }
            // ページダウン
            else if (nVKey == CEWin32Api.VK_NEXT)
            {
                MoveScrollV((int)m_vScrollBar.LargeChange/*m_scrollInfoV.nPage*/);
            }
            // ページアップ
            else if (nVKey == CEWin32Api.VK_PRIOR)
            {
                MoveScrollV((int)(-1 * m_vScrollBar.LargeChange/*m_scrollInfoV.nPage*/));
            }
            // ホームキー
            else if (nVKey == CEWin32Api.VK_HOME)
            {
                HomeKey();
            }
            // エンドキー
            else if (nVKey == CEWin32Api.VK_END)
            {
                EndKey();
            }
            // シフトキー
            else if (nVKey == CEWin32Api.VK_SHIFT)
            {
                ;
            }
            // リターンキー
            else if (nVKey == CEWin32Api.VK_RETURN)
            {
                ReturnKey();
            }
            // ●バックスペースー
            else if (nVKey == CEWin32Api.VK_BACK)
            {
                BackSpaceKey();
            }
            // ●デリートキー
            else if (nVKey == CEWin32Api.VK_DELETE)
            {
                DeleteKey();
            }
            // ●エスケープキー
            else if (nVKey == CEWin32Api.VK_ESCAPE)
            {
                EscapeKey();
            }
        }

        /// <summary>
        /// ファイル読込
        /// </summary>
        /// <param name="file">読込ファイル名</param>
        /// <param name="enc">読込んだファイルのエンコード</param>
        /// <returns>true:読込成功／false:読込失敗</returns>
        public Boolean ReadFile(string file, out Encoding enc)
        {
            // （デバッグ）読込速度計測
            //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //sw.Start();

            // ファイルが空かチェック
            Refresh();
            if ((new FileInfo(file)).Length == 0)
            {
                // ファイルが空の場合はShift-JISとして開く
                enc = Encoding.GetEncoding(932);
                return true;
            }

            enc = null;
#if false
            // 読み込むファイルの文字コード取得（再チャレンジ）
            Encoding[] enc2 = CharCode.Detect(File.ReadAllBytes(file));
            if (enc2 == null)
            {
                // 2回チェックしてダメなら仕方ない...。
                return false;
            }
#else
            // 読み込むファイルの文字コード取得
            Ude.CharsetDetector cdet = new Ude.CharsetDetector();
            using (FileStream fs = File.OpenRead(file))
            {
                cdet.Feed(fs);
                cdet.DataEnd();
                if (cdet.Charset == null)
                {
                    // 読み込むファイルの文字コード取得（再チャレンジ）
                    enc = CECommon.GetCode(File.ReadAllBytes(file));
                    if (enc == null)
                    {
                        // 2回チェックしてダメなら仕方ない...。
                        return false;
                    }
                }
                else
                {
                    enc = Encoding.GetEncoding(cdet.Charset);
                }
            }
#endif
            // CEEditViewの初期化
            Init();

            m_doc.ClearAllLogicalLineData();

            m_readEncode = enc;

            // ファイルからテキストデータを読み込む
            // 内部文字コードに変換しながら読み込んでいく
            using (StreamReader sr = new StreamReader(file, enc))
            {
                // 読み込んだテキストデータ
                StringBuilder lineString = new StringBuilder();
                Stopwatch sw = new Stopwatch();
                sw.Start();
#if true // 高速化対応
                // 全ファイル一括読み込み
                string allString = string.Empty;
                allString = sr.ReadToEnd();

                SLogicalLine sld = null;

                // 読み込んだテキストデータ(1文字)
                for (int idx = 0; idx < allString.Length; idx++)
                {
                    lineString.Append(allString[idx]);
                    // 改行コードが"CRLF"の場合
                    if (allString[idx] == '\r' && allString[idx + 1] == '\n')
                    {
                        idx++;
                        lineString.Append(allString[idx]);
                        // UTF16変換し文字列追加
                        sld = m_doc.MakeLineData(m_doc.GetLineCountL(), ConvertSring(lineString.ToString(), enc, m_internalEncode));
                        m_doc.AddLogicalLineData(sld);
                        //edit.updateAllLogicalLine();
                        //edit.LayoutScrollBar();
                        lineString.Clear();
                    }
                    // 改行コードが"CR"または"LF"の場合
                    else if ((allString[idx] == '\r' && allString[idx + 1] != '\n') || allString[idx] == '\n')
                    {
                        // UTF16変換し文字列追加
                        sld = m_doc.MakeLineData(m_doc.GetLineCountL(), ConvertSring(lineString.ToString(), enc, m_internalEncode));
                        m_doc.AddLogicalLineData(sld);
                        //edit.updateAllLogicalLine();
                        //edit.LayoutScrollBar();
                        lineString.Clear();
                    }
                }
                // UTF16変換し文字列追加
                sld = m_doc.MakeLineData(m_doc.GetLineCountL(), ConvertSring(lineString.ToString(), enc, m_internalEncode));
                m_doc.AddLogicalLineData(sld);
                updateAllLogicalLine();
                LayoutScrollBar();
#else
                string aaa = string.Empty;
                Stopwatch sw = new Stopwatch();
                sw.Start();
#if false
                while (sr.Peek() > -1)
                {
                    //一行読み込んで表示する
                    lineString = sr.ReadLine() + "\r\n";
                    lineString = ConvertSring(lineString, enc, base.m_encode);
                    base.AddLine(lineString);
                }
#else
                // 読み込んだテキストデータ(1文字)
                var ch = new char[1];

                while (sr.Peek() > -1)
                {
                    sr.Read(ch, 0, 1);
                    lineString += ch[0];
                    // 改行コードが"CRLF"の場合
                    if (ch[0] == '\r' && sr.Peek() == '\n')
                    {
                        sr.Read(ch, 0, 1);
                        lineString += ch[0];
                        lineString = ConvertSring(lineString, enc, base.m_encode);
                        base.AddLine(lineString);
                        lineString = string.Empty;
                    }
                    // 改行コードが"CR"または"LF"の場合
                    else if ((ch[0] == '\r' && sr.Peek() != '\n') || ch[0] == '\n')
                    {
                        lineString = ConvertSring(lineString, enc, base.m_encode);
                        base.AddLine(lineString);
                        lineString = string.Empty;
                    }
                }
                lineString = ConvertSring(lineString, enc, base.m_encode);
                base.AddLine(lineString);
#endif
#endif
                // 編集中フラグクリア
                m_doc.ClearEditFlg();

                sw.Stop();
                Console.WriteLine("time:" + sw.Elapsed);
            }

            // 正常終了
            return true;
        }

        /// <summary>
        /// ファイル書込
        /// </summary>
        /// <param name="file">書込ファイル名</param>
        /// <param name="enc">書込エンコード</param>
        public void WriteFile(string file, Encoding enc)
        {
            // ファイル書込
            using (FileStream fs = new FileStream(file, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs, enc))
                {
                    string line = string.Empty;
                    int lineNum = m_doc.GetLineCountP();
                    for (int idx = 0; idx < lineNum; idx++)
                    {
                        line = m_doc.GetLineStringP(idx);
                        line = ConvertSring(line, m_internalEncode, enc);
                        sw.Write(line);
                    }
                    // 編集中フラグクリア
                    m_doc.ClearEditFlg();
                    Refresh();
                }
            }
        }

        /// <summary>
        /// 文字コード変換
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string ConvertSring(string str, Encoding src, Encoding dst)
        {
            return dst.GetString(Encoding.Convert(src, dst, src.GetBytes(str)));
        }
#if false
        /// <summary>
        /// 編集状態取得
        /// </summary>
        /// <returns>true:編集中 / false:未編集</returns>
        public Boolean GetEditStatus()
        {
            if (GetEditFlg() || m_doc.GetLineStringP(0) != "")
            {
                return true;
            }
            return false;
        }
#endif
        /// <summary>
        /// カーソルタイプ設定
        /// </summary>
        /// <param name="b">true:フリーカーソル / false:通常カーソル</param>
        public void SetCursorType(Boolean b)
        {
            if (b)
            {
                // フリーカーソルモード
                m_ShareData.m_cursorMode = 1;
            }
            else
            {
                // 通常モード
                m_ShareData.m_cursorMode = 0;
            }
        }

        /// <summary>
        /// カーソルタイプ取得
        /// </summary>
        /// <return>true:フリーカーソル / false:通常カーソル</return>
        public Boolean GetCursorType()
        {
            if (m_ShareData.m_cursorMode == 1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 折返し設定
        /// </summary>
        /// <param name="b">true:折り返す / false:折り返さない</param>
        public void SetWrap(Boolean b)
        {
            int width;
            if (b)
            {
                // 折り返す
                // 折り返し幅 = カスタムテキストボックスの幅 - 縦スクロールバーの幅
                width = this.Width - SystemInformation.VerticalScrollBarWidth - 10;
            }
            else
            {
                // 折り返さない
                width = CEConstants.DefaultWrapSize;
            }

            // -------------------------------------------------------------------------
            // 現在の論理行を保存して一度先頭行に移動し、折り返し情報を更新してから、
            // 保存しておいた論理行に移動する。
            // -------------------------------------------------------------------------

            // 現在位置を論理行として保存
            int colL, rowL;
            m_doc.PtoLPos(m_caretStrBuf.Y, m_caretStrBuf.X, out rowL, out colL);

            // 先頭行に移動
            CursorKeyH(-m_caretStrBuf.X);
            CursorKeyV(-m_caretStrBuf.Y);

            // 折返し情報更新
            m_ShareData.m_wrapPositionFlag = b; // 折り返し有
            m_ShareData.m_wrapPositionPixel = width - m_startColPos;    // 折り返し幅(ピクセル)
            updateAllLogicalLine();

            // スクロールバーの位置設定
            this.LayoutScrollBar();

            // 保存しておいた現在位置(論理)に移動
            int colP, rowP;
            m_doc.LtoPPos(rowL, colL, out rowP, out colP);
            MoveScrollV(rowP);

            // 現在のキャレット位置設定
            m_caretStrBuf.Y = rowP;
            m_caretStrBuf.X = 0;

            // ビュー更新
            //this.LayoutScrollBar();
            this.Refresh();
        }

        /// <summary>
        /// 折返し状態取得
        /// </summary>
        /// <return>true:折り返す / false:折り返さない</return>
        public Boolean GetWrap()
        {
            if (m_ShareData.m_wrapPositionFlag)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 矩形選択設定
        /// </summary>
        /// <param name="b">true:矩形選択オン / false:矩形選択オフ</param>
        public void SetRectangle(Boolean b)
        {
            if (b)
            {
                // 矩形選択オン
                m_selectType = RACTANGLE_RANGE_SELECT;
                m_ShareData.m_cursorMode = 1; // フリーカーソルモードON
            }
            else
            {
                // 矩形選択オフ
                m_selectType = NONE_RANGE_SELECT;
                m_ShareData.m_cursorMode = 0; // フリーカーソルモードON
            }
            m_rectCaretColPosPixel = m_curCaretColPosPixel;
        }

        /// <summary>
        /// 矩形選択状態取得
        /// </summary>
        /// <return>true:矩形選択オン / false:矩形選択オフ</return>
        public Boolean GetRectangle()
        {
            if (m_selectType == RACTANGLE_RANGE_SELECT)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// フォントのサイズ取得
        /// 固定フォントのみ対応
        /// </summary>
        public void GetFontSize(/*IntPtr hdl, Font f,*/ ref int h, ref int w)
        {
            h = m_ShareData.m_charHeightPixel;
            w = m_ShareData.m_charWidthPixel;

            return;
        }
#if false
        /// <summary>
        /// 編集中可否フラグ取得
        /// </summary>
        /// <returns>true:編集中／false:未編集</returns>
        public Boolean GetEditFlg()
        {
            return m_doc.GetEdit(); // 編集中可否フラグとりあえずコメント
            //return false;
        }
#endif

        /// <summary>
        /// スクロールバー間の色を塗る
        /// </summary>
        private void LayoutScrollBarBetweenColor()
        {
            CEWin32Api.RECT rc;
            CEWin32Api.GetClientRect(this.Handle, out rc);
            Graphics g = Graphics.FromHdc(m_hDrawDC);
            SolidBrush backColor = new SolidBrush(Color.FromArgb(255, SystemColors.Control));
            g.FillRectangle(backColor, rc.right - m_vScrollBar.Width, rc.bottom - m_hScrollBar.Height, m_vScrollBar.Width, m_hScrollBar.Height);
            g.Dispose();
        }

        private void LayoutScrollBar()
        {
            LayoutScrollBar(-1);
        }

        /// <summary>
        /// スクロールバーの設定
        /// </summary>
        private void LayoutScrollBar(int allLine)
        {
#if false
            CEWin32Api.RECT rc;

            int nVScrollRate;
            int nVAllLines;
#if false
            int nHScrollRate;
            int nHAllLines;
#endif

            // スクロールバーの精度は最大65535なので
            // スクロール尺度を算出

            // 垂直スクロールバー
            nVAllLines = m_doc.GetLineCountP();
            nVScrollRate = 1;
            while (nVAllLines / nVScrollRate > 65535)
            {
                ++nVScrollRate;
            }
#if false
            // 水平スクロールバー
            nHAllLines = m_ShareData.m_wrapPositionPixel / m_ShareData.m_charWidthPixel;
            nHScrollRate = 1;
            while (nHAllLines / nHScrollRate > 65535)
            {
                ++nHScrollRate;
            }
#endif
            CEWin32Api.GetClientRect(this.Handle, out rc);

            ////////////////////////////////////////////////
            // 垂直スクロールバー
            ////////////////////////////////////////////////
            m_viewDispRow = (rc.bottom - m_startRowPos) / m_ShareData.m_charHeightPixel;
            m_scrollInfoV.cbSize = (uint)Marshal.SizeOf(typeof(CEWin32Api.SCROLLINFO));
            m_scrollInfoV.fMask = CEWin32Api.SIF_ALL;
            m_scrollInfoV.nMin = 0;
            m_scrollInfoV.nMax = (nVAllLines / nVScrollRate) - 1; // 0インデックスなので -1 する
            m_scrollInfoV.nPage = m_viewDispRow / nVScrollRate;
            m_scrollInfoV.nPos = m_viewTopRow / nVScrollRate;
            m_scrollInfoV.nTrackPos = nVScrollRate;
            m_nVScrollRate = nVScrollRate;

            ////////////////////////////////////////////////
            // 水平スクロールバー
            ////////////////////////////////////////////////
            //m_viewDispColPixel = rc.right;
            m_viewDispColumn = (rc.right - m_startColPos) / (m_ShareData.m_charWidthPixel + m_ShareData.m_nColumnSpace);
            m_scrollInfoH.cbSize = (uint)Marshal.SizeOf(typeof(CEWin32Api.SCROLLINFO));
            m_scrollInfoH.fMask = CEWin32Api.SIF_ALL;
            m_scrollInfoH.nMin = 0;
            // 半角１、全角２として最長行の文字数を設定
            // 0インデックスなので -1 するが、改行分もあるので +1 となる
            m_scrollInfoH.nMax = /*(nHAllLines / nHScrollRate) - 1*/m_ShareData.m_wrapPositionPixel / m_ShareData.m_charWidthPixel/*m_ShareData.m_wrapPositionPixel * m_ShareData.m_charWidthPixel*/;
            m_scrollInfoH.nPage = m_viewDispColumn/* / nHScrollRate*/;
            m_scrollInfoH.nPos = m_viewLeftColumn/* / nHScrollRate*/;
#if false
            m_scrollInfoH.nTrackPos = nHScrollRate;
            m_nHScrollRate = nHScrollRate;
#endif

            // スクロール情報設定
            CEWin32Api.SetScrollInfo(this.Handle, CEWin32Api.SB_VERT, ref m_scrollInfoV, true);
            CEWin32Api.SetScrollInfo(this.Handle, CEWin32Api.SB_HORZ, ref m_scrollInfoH, true);

            // 垂直・水平スクロールバー位置設定
            MoveScrollV(0);
            MoveScrollH(0);
#else
            CEWin32Api.RECT rc;
            CEWin32Api.GetClientRect(this.Handle, out rc);

            // スクロールバーの精度は最大65535なのでスクロール尺度を算出
            // 垂直スクロールバー
            int nVScrollRate;
            int nVAllLines;
            if (allLine == -1)
            {
                allLine = m_doc.GetLineCountP();
            }
            nVAllLines = allLine;
            nVScrollRate = 1;
            while (nVAllLines / nVScrollRate > 65535)
            {
                ++nVScrollRate;
            }

            // 垂直スクロールバー
            m_vScrollBar.Location = new Point(rc.right - m_vScrollBar.Width, rc.top);
            m_vScrollBar.Size = new Size(m_vScrollBar.Width, rc.bottom - m_hScrollBar.Height);
            //m_viewDispRow = ((rc.bottom - m_hScrollBar.Height) - m_startRowPos) / m_ShareData.m_charHeightPixel;
            m_viewDispRow = ((rc.bottom - m_hScrollBar.Height) - m_startRowPos) / m_ShareData.m_charHeightPixel;
            m_vScrollBar.Minimum = 0;
            m_vScrollBar.Maximum = (nVAllLines / nVScrollRate);
#if false // 折返した場合、値が画面に表示されている行数の1/4になってしまう。特にレートで引く必要もないので修正
            //m_vScrollBar.LargeChange = (m_viewDispRow / nVScrollRate) < 0 ? 0 : (m_viewDispRow / nVScrollRate); // 値がマイナスになる場合は0とする。
#else
            m_vScrollBar.LargeChange = m_viewDispRow < 0 ? 0 : m_viewDispRow; // 値がマイナスになる場合は0とする。
#endif
            m_vScrollBar.Value = m_viewTopRow / nVScrollRate;
#if false
            if (m_vScrollBar.Maximum < (m_viewTopRow / nVScrollRate))
            {
                m_vScrollBar.Value = m_vScrollBar.Maximum;
            }
            else
            {
                m_vScrollBar.Value = m_viewTopRow / nVScrollRate;
            }
#endif
            m_nVScrollRate = nVScrollRate;
            if (m_viewDispRow > nVAllLines)
            {
                m_vScrollBar.Enabled = false;
            }
            else
            {
                m_vScrollBar.Enabled = true;
            }

            // 水平スクロールバー
            m_hScrollBar.Location = new Point(rc.left, rc.bottom - m_hScrollBar.Height);
            m_hScrollBar.Size = new Size(rc.right - m_vScrollBar.Width, m_hScrollBar.Height);
            m_viewDispColumn = ((rc.right - m_vScrollBar.Width) - m_startColPos) / (m_ShareData.m_charWidthPixel + m_ShareData.m_nColumnSpace);
            m_hScrollBar.Minimum = 0;
            // 半角１、全角２として最長行の文字数を設定
            // 0インデックスなので -1 するが、改行分もあるので +1 となる
            m_hScrollBar.Maximum = m_ShareData.m_wrapPositionPixel / m_ShareData.m_charWidthPixel;
            m_hScrollBar.LargeChange = m_viewDispColumn < 0 ? 0 : m_viewDispColumn;
            m_hScrollBar.Value = m_viewLeftColumn;
            if (m_viewDispColumn > m_ShareData.m_wrapPositionPixel / m_ShareData.m_charWidthPixel)
            {
                m_hScrollBar.Enabled = false;
            }
            else
            {
                m_hScrollBar.Enabled = true;
            }

            // 垂直・水平スクロールバー位置設定
            MoveScrollV(0, nVAllLines);
            MoveScrollH(0);
#endif
        }

        /// <summary>
        /// 現在の文字が選択範囲内かチェック
        /// </summary>
        /// <param name="row">物理行</param>
        /// <param name="col">物理列</param>
        /// <returns>true:範囲内 / false:範囲内でない</returns>
        private Boolean IsRngStr(int row, int col)
        {
            Boolean ret = false;

            // -----------------------------------------------
            // 矩形選択
            // -----------------------------------------------
            if (m_selectType == RACTANGLE_RANGE_SELECT)
            {
                // 指定した行(物理)の列(ピクセル)から列(物理・ピクセル)を取得する
                //int hiddenPixelSize = m_viewLeftColumn * m_charWidthPixel;
                if ( ((m_sRng.Y <= row && row <= m_eRng.Y) || (m_eRng.Y <= row && row <= m_sRng.Y)) &&
                     ((m_sRectIdx    <= col && col < m_eRectIdx)     || (m_eRectIdx    <= col && col < m_sRectIdx    )) )
                {
                    ret = true;
                }
            }
            // -----------------------------------------------
            // 範囲選択
            // -----------------------------------------------
            else if (m_selectType == NORMAL_RANGE_SELECT)
            {
                if ((m_selectType == NORMAL_RANGE_SELECT) && (m_sRng.Y <= row) && (row <= m_eRng.Y))
                {
                    if (m_sRng.Y != m_eRng.Y)
                    {
                        // 複数行に跨る
                        if (m_sRng.Y == row)
                        {
                            // 開始行
                            if (m_sRng.X <= col)
                            {
                                ret = true;
                            }
                        }
                        else if (m_eRng.Y == row)
                        {
                            // 終了行
                            if (m_eRng.X > col)
                            {
                                ret = true;
                            }
                        }
                        else
                        {
                            // 間の行
                            ret = true;
                        }
                    }
                    else
                    {
                        // 単一行
                        if (m_sRng.X <= col && col < m_eRng.X)
                        {
                            ret = true;
                        }
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// 文字列表示(一行分)
        /// （指定された文字列を一文字ずつExtTextOutを使って表示する）
        /// </summary>
        /// <param name="s">表示文字列</param>
        /// <param name="x">表示開始位置（X座標）（ピクセル）</param>
        /// <param name="y">表示開始位置（Y座標）（ピクセル）</param>
        /// <param name="c">表示行</param>
        private bool blockComment = false;
        private int TextOutLine(string s, int x, int y, int c, int row)
        {
            int ret = 0;
            int nowX = x;
            //int py = y;
            int textPixelWidth = 0;
            string text = string.Empty;

            bool lineComment = false;
            bool foundLineComment = false;

            // 表示しようとする文字列から改行文字を削除
            s = m_doc.GetNotLineFeedString(s);

            int column = 0;

            // 表示画面外左は飛ばす
            while (column < s.Length)
            {
                if (s[column].ToString() == "\t")
                {
                    textPixelWidth = m_tabWidthPixel - ((nowX - x) % m_tabWidthPixel);
                }
                else
                {
                    textPixelWidth = this.GetOneWordPixel(s[column].ToString()) + m_ShareData.m_nColumnSpace;
                    if(column + 1 < s.Length && s[column] == '/' && s[column + 1] == '/')
                    {
                        lineComment = true;
                    }
                }

                if (nowX + textPixelWidth >= 0)
                {
                    break;
                }

                nowX += textPixelWidth;
                column++;
            }

            bool selectFlg = false;
            bool findFlg = false;

            while (column < s.Length)
            {
                // タブの場合（タブは１つずつ表示する）
                if (s[column].ToString() == "\t")
                {
                    // タブの幅取得（ピクセル単位）
                    textPixelWidth = m_tabWidthPixel - ((nowX - x) % m_tabWidthPixel);

                    if (nowX > m_screenWidth)
                    {
                        // 画面領域外（右）
                        return nowX;
                    }
                    else
                    {
                        // 表示領域内
                        CEWin32Api.SetTextColor(m_hDrawDC, CEConstants.SymbolFontColor);
                        CEWin32Api.SetBkColor(m_hDrawDC, CEConstants.BackColor);
                        TextOutput(nowX, y, "^", textPixelWidth);
                        nowX += textPixelWidth;
                        column++;
                    }
                }
                // 通常文字（タブやシンタックスハイライト文字がくるまで読み込み一気に表示する）
                else
                {
                    text = string.Empty;
                    textPixelWidth = 0;
                    bool rightArea = false;
                    while (column < s.Length && s[column].ToString() != "\t")
                    {
                        textPixelWidth += this.GetOneWordPixel(s[column].ToString()) + m_ShareData.m_nColumnSpace;

                        if (nowX + textPixelWidth > m_screenWidth)
                        {
                            // 画面領域外（右）
                            rightArea = true;
                            break;
                        }
                        else
                        {
                            // 描画領域内
                            if (!lineComment && isLineComment(s.Substring(column), s.Substring(column).Length))
                            {
                                textPixelWidth -= this.GetOneWordPixel("/") + m_ShareData.m_nColumnSpace; // 暫定
                                foundLineComment = true;
                                break;
                            }
                            text += s[column].ToString();
                        }
#if false
                        if (nowX > m_screenWidth)
                        {
                            rightArea = true;
                            break;
                        }
                        if (!lineComment && isLineComment(s.Substring(column), s.Substring(column).Length))
                        {
                            textPixelWidth -= this.GetOneWordPixel("/") + m_ShareData.m_nColumnSpace; // 暫定
                            foundLineComment = true;
                            break;
                        }
                        text += s[column].ToString();
#endif
                        column++;
                    }

                    // 描画するテキスト情報がある場合、描画する
                    if (textPixelWidth != 0)
                    {
                        if(lineComment)
                        {
                            CEWin32Api.SetTextColor(m_hDrawDC, CEConstants.CommentFontColor);
                            CEWin32Api.SetBkColor(m_hDrawDC, CEConstants.BackColor);
                        }
                        else
                        {
                            CEWin32Api.SetTextColor(m_hDrawDC, c);
                            CEWin32Api.SetBkColor(m_hDrawDC, CEConstants.BackColor);
                        }
                        TextOutput(nowX, y, text, textPixelWidth);
                        nowX += textPixelWidth;
                    }
                    if (rightArea) break;
                    if (foundLineComment) lineComment = true;
                }
            }

            return nowX;
        }

        private bool isLineComment(string str, int length)
        {
            return length >= 2 && str[0] == '/' && str[1] == '/';
        }

        // テキスト出力
        private void TextOutput( int pixelX, int pixelY, string text, int pixelLen )
        {
            CEWin32Api.RECT rcClip;

            rcClip.left = pixelX + m_startColPos;
            rcClip.top = pixelY + m_startRowPos;
            rcClip.right = pixelX + pixelLen + m_startColPos;
            rcClip.bottom = pixelY + m_ShareData.m_charHeightPixel + m_startRowPos;
            uint flag = (uint)CEWin32Api.ETOOptions.ETO_CLIPPED | (uint)CEWin32Api.ETOOptions.ETO_OPAQUE;
            CEWin32Api.ExtTextOut(m_hDrawDC, pixelX + m_startColPos, pixelY + m_startRowPos, (uint)flag, ref rcClip, text, (uint)text.Length, null);
        }

        /// <summary>
        /// 指定位置(ピクセル指定)改行コードを表示する。
        /// </summary>
        /// <param name="g"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void TextOutLineBreak(int x, int y, int row, string str)
        {
            x += m_startColPos;
            y += m_startRowPos;

            // Graphicsオブジェクト生成
            Graphics g = Graphics.FromHdc(m_hDrawDC);
            SolidBrush backColor = null;

            // 背景色表示取得
            if (IsRngStr(row, m_doc.GetNotLineFeedString(str).Length))
            {
                // 範囲選択あり
                backColor = new SolidBrush(Color.FromArgb(255, ColorTranslator.FromHtml(CECommon.ChgRGB(CEConstants.SelectLfBackColor).ToString())));
            }
            else
            {
                // 範囲選択なし
                backColor = new SolidBrush(Color.FromArgb(255, ColorTranslator.FromHtml(CECommon.ChgRGB(CEConstants.BackColor).ToString())));
            }
            g.FillRectangle(backColor, x, y, m_ShareData.m_charWidthPixel * 2, m_ShareData.m_charHeightPixel);

            // 改行マーク表示
            Pen fontColor = new Pen(Color.FromArgb(255, ColorTranslator.FromHtml(CEConstants.lrColor.ToString())), 1);
            int t = m_ShareData.m_charWidthPixel / 3;
            int dy = m_ShareData.m_charHeightPixel / 3;
            Font font = m_ShareData.m_font;
            g.DrawLine(fontColor, x + m_ShareData.m_charWidthPixel, y + dy, x + m_ShareData.m_charWidthPixel, y + font.Height - dy);
            g.DrawLine(fontColor, x + m_ShareData.m_charWidthPixel, y + m_ShareData.m_font.Height - dy, x, y + font.Height - dy);
            g.DrawLine(fontColor, x, y + font.Height - dy, x + t, y + font.Height - dy - t);
            g.DrawLine(fontColor, x, y + font.Height - dy, x + t, y + font.Height - dy + t);

            // Graphicsオブジェクト解放
            g.Dispose();
        }

        /// <summary>
        /// 行番号出力
        /// </summary>
        /// <param name="s">行番号</param>
        /// <param name="py">表示位置</param>
        /// <param name="lLine">論理行</param>
        private void TextOutLineNumber(string s, int py, int lLine)
        {
            if (m_doc.GetEditStatus(lLine))
            {
                // 編集中

                // 背景色とフォント色を設定
                CEWin32Api.SetTextColor(m_hDrawDC, CEConstants.EditLineFontColor);
                CEWin32Api.SetBkColor(m_hDrawDC, CEConstants.BackColor);
            }
            else
            {
                // 未編集中

                // 背景色とフォント色を設定
                CEWin32Api.SetTextColor(m_hDrawDC, CEConstants.LineFontColor);
                CEWin32Api.SetBkColor(m_hDrawDC, CEConstants.BackColor);
            }

            // 表示文字列作成
            s = string.Format("{0, 6}", s);
            s += "|";

            // 表示
            CEWin32Api.RECT rcClip;
            rcClip.left = 0;
            rcClip.top = py + m_startRowPos;
            rcClip.right = m_startColPos;
            rcClip.bottom = py + m_ShareData.m_charHeightPixel + m_startRowPos;
            CEWin32Api.ExtTextOut(m_hDrawDC, 0, py + m_startRowPos, (uint)CEWin32Api.ETOOptions.ETO_CLIPPED | (uint)CEWin32Api.ETOOptions.ETO_OPAQUE, ref rcClip, s, (uint)s.Length, null);
        }

        private void MoveScrollV(int nPos)
        {
            MoveScrollV(nPos, -1);
        }

        /// <summary>
        /// 縦スクロールバーの位置設定
        /// </summary>
        /// <param name="nPos">移動する行数(物理)</param>
        private void MoveScrollV(int nPos, int aLine)
        {
            // EOFも含めた行数取得
            int allLine;
            if (aLine == -1)
            {
                aLine = m_doc.GetLineCountP();
            }
            allLine = aLine;
            if (nPos > allLine - m_viewDispRow - m_viewTopRow)
            {
                // 一番下の行から更に下にスクロールしようとした場合
                nPos = allLine - m_viewDispRow - m_viewTopRow;
            }
            if (nPos < (-1 * m_viewTopRow))
            {
                // 一番上の行から更に上にスクロールしようとした場合
                nPos = -1 * m_viewTopRow;
            }

            if (nPos != 0)
            {
                m_vScrollBar.Value/*m_scrollInfoV.nPos*/ += nPos / m_nVScrollRate; // スクロールバーのつまみの位置指定
                m_viewTopRow += nPos; // 画面に表示される先頭行を指定
                m_doc.getPosition(m_viewTopRow, out m_viewTopLineL, out m_viewTopLineLP);

                //CEWin32Api.SetScrollInfo(this.Handle, CEWin32Api.SB_VERT, ref m_scrollInfoV, true);
#if false
                CEWin32Api.ScrollWindowEx(this.Handle, 0, -nPos * m_ShareData.m_charHeightPixel, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, CEWin32Api.SW_ERASE | CEWin32Api.SW_INVALIDATE);
#else
                // 再描画
                CEWin32Api.InvalidateRect(this.Handle, IntPtr.Zero, true);
                //this.Refresh();
#endif
            }

            return;
        }

        /// <summary>
        /// 横スクロールバーの位置設定
        /// </summary>
        /// <param name="nPos">移動する行数（半角単位）</param>
        private void MoveScrollH(int nPos)
        {
            // 改行コード分 +1 している
            int len = m_ShareData.m_wrapPositionPixel * m_ShareData.m_charWidthPixel;
            if (nPos > len - m_viewLeftColumn)
            {
                // 一番右の行から更に右にスクロールしようとした場合
                return;
            }
            if (nPos < (-1 * m_viewLeftColumn))
            {
                // 一番上の行から更に上にスクロールしようとした場合
                nPos = -1 * m_viewLeftColumn;
            }

            if (nPos != 0)
            {
                m_hScrollBar.Value/*m_scrollInfoH.nPos*/ += nPos; // スクロールバーのつまみの位置指定
                m_viewLeftColumn += nPos;
                //CEWin32Api.SetScrollInfo(this.Handle, CEWin32Api.SB_HORZ, ref m_scrollInfoH, true);
#if false
                CEWin32Api.ScrollWindowEx(m_Handle, -nPos * (m_ShareData.m_charWidthPixel + m_ShareData.m_nColumnSpace), 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, CEWin32Api.SW_ERASE | CEWin32Api.SW_INVALIDATE);
#else
                // 再描画
                CEWin32Api.InvalidateRect(this.Handle, IntPtr.Zero, true);
                //this.Refresh();
#endif
            }

            return;
        }

        /// <summary>
        /// キャレット表示 
        /// </summary>
        private void ShowCaret()
        {
            CEWin32Api.DestroyCaret();

            // 物理位置からバッファ位置を取得
            int lLine;
            int pLine;
            m_doc.getPosition(m_caretStrBuf.Y, out lLine, out pLine);

            // 現在位置(ピクセル)更新
            int m_nowPosX = -1;
            int m_nowPosY = -1;
            if (m_ShareData.m_cursorMode == 0)
            {
                // 通常カーソルモード
                string nowString = m_doc.GetLeftStringP(m_caretStrBuf.Y, m_caretStrBuf.X, lLine, pLine);
                m_nowPosX = m_doc.GetTextPixel(m_ShareData.m_nColumnSpace, nowString);
            }
            else
            {
                // フリーカーソルモード
                // ★うんこソース★

                // 現在行のテキストの長さを取得
                string nowStr = m_doc.GetNotLineFeedString(m_doc.GetLineStringP(m_caretStrBuf.Y, lLine, pLine));
                int len = nowStr.Length;

                // 改行以降にキャレットがあるか？
                // 現在位置がテキストの長さより大きい場合は改行以降にキャレットがあるということ
                if (m_caretStrBuf.X > len)
                {
                    // 改行以降にキャレットがある
                    // 現在行の先頭からキャレット位置までの長さ（ピクセル）を取得
                    m_nowPosX = ((m_caretStrBuf.X - len) * m_ShareData.m_charWidthPixel) +                  // 改行位置から現在のキャレットまでのピクセル長 +
                                m_doc.GetTextPixel(m_ShareData.m_nColumnSpace, nowStr);   // 現在行のピクセル長
                }
                else
                {
                    // 改行以前にキャレットがある（通常カーソルモードと同様の処理）
                    nowStr = m_doc.GetLeftStringP(m_caretStrBuf.Y, m_caretStrBuf.X, lLine, pLine);
                    m_nowPosX = m_doc.GetTextPixel(m_ShareData.m_nColumnSpace, nowStr);
                }
            }
            m_nowPosY = m_caretStrBuf.Y * m_ShareData.m_charHeightPixel;

            // 画面左上の位置(ピクセル)更新
            int screenPosX = m_viewLeftColumn * (m_ShareData.m_nColumnSpace + m_ShareData.m_charWidthPixel);
            int screenPosY = m_viewTopRow * m_ShareData.m_charHeightPixel;

            // 画面の高さ幅(ピクセル)更新
            int screenWidth = m_viewDispColumn * (m_ShareData.m_nColumnSpace + m_ShareData.m_charWidthPixel);
            int screenHeight = m_viewDispRow * m_ShareData.m_charHeightPixel;

            // 表示されている画面外（左または右にキャレットがあるか）チェック
            if ((screenPosX + screenWidth < m_nowPosX) || (screenPosX > m_nowPosX))
            {
                // 画面外の場合はキャレットを表示しない
                return;
            }

            // 表示されている画面外（上または下にキャレットがあるか）チェック
            if ((screenPosY + screenHeight < m_nowPosY) || (screenPosY > m_nowPosY))
            {
                // 画面外の場合はキャレットを表示しない
                return;
            }

            // キャレットを表示する座標を取得
            int cx = m_nowPosX - screenPosX;
            int cy = m_nowPosY - screenPosY;

            int hiddenPixelSize = m_viewLeftColumn * m_ShareData.m_charWidthPixel;
            m_caretPixel.X = cx + hiddenPixelSize;
            m_caretPixel.Y = cy;

            CEWin32Api.CreateCaret(this.Handle, IntPtr.Zero, 2, m_ShareData.m_charHeightPixel);
            CEWin32Api.SetCaretPos(cx + m_startColPos, cy + m_startRowPos);
            CEWin32Api.ShowCaret(this.Handle);

            // アンダーバー表示
            this.ShowCaretUnderbar(cy + m_startRowPos);

            // アンダーバー表示
            this.ShowCaretVerticalLine(cx + m_startColPos);

            // キャレット位置表示
            this.ShowCaretPos(cx + m_startColPos);
        }

        /// <summary>
        /// キャレット位置表示
        /// </summary>
        /// <param name="pos"></param>
        private void ShowCaretPos(int pos)
        {
            // Graphicsオブジェクト生成
            Graphics g = Graphics.FromHdc(m_hDrawDC);

            // カーソル位置
            SolidBrush backColor = new SolidBrush(Color.FromArgb(255, ColorTranslator.FromHtml(CECommon.ChgRGB(CEConstants.CaretColPosColor).ToString()))); ;
            //int p = m_doc.GetTextPixel(m_ShareData.m_nColumnSpace, m_doc.GetLeftStringP(m_caretStrBuf.Y, m_caretStrBuf.X)) + m_startColPos;
            g.FillRectangle(backColor, pos, 0, m_ShareData.m_charWidthPixel, (int)(m_startRowPos * 0.2));

            // Graphicsオブジェクト解放
            g.Dispose();
        }

        /// <summary>
        /// キャレット行（下線）表示
        /// </summary>
        /// <param name="row">アンダーバー出力位置</param>
        private void ShowCaretUnderbar(int pos)
        {
            // Graphicsオブジェクト生成
            Graphics g = Graphics.FromHdc(m_hDrawDC);

            // クライアント領域のサイズ取得
            CEWin32Api.RECT rc;
            CEWin32Api.GetClientRect(this.Handle, out rc);

            // 線描画
            Pen fontColor = new Pen(Color.FromArgb(255, ColorTranslator.FromHtml(CECommon.ChgRGB(CEConstants.currentLineColor).ToString())), 1);
            int y = pos + m_ShareData.m_charHeightPixel;
            g.DrawLine(fontColor, 0, y, rc.right, y);

            // Graphicsオブジェクト解放
            g.Dispose();
        }

        /// <summary>
        /// キャレット行の縦線表示
        /// </summary>
        /// <param name="pos"></param>
        private void ShowCaretVerticalLine(int pos)
        {
            // Graphicsオブジェクト生成
            Graphics g = Graphics.FromHdc(m_hDrawDC);

            // クライアント領域のサイズ取得
            CEWin32Api.RECT rc;
            CEWin32Api.GetClientRect(this.Handle, out rc);

            // 線描画
            Pen fontColor = new Pen(Color.FromArgb(255, ColorTranslator.FromHtml(CECommon.ChgRGB(CEConstants.currentLineColor).ToString())), 1);
            int x = pos;
            g.DrawLine(fontColor, x, 0, x, rc.bottom);

            // Graphicsオブジェクト解放
            g.Dispose();
        }

        /// <summary>
        /// キャレット表示 
        /// </summary>
        public void HideCaret()
        {
            CEWin32Api.HideCaret(this.Handle);
        }

        /// <summary>
        /// ルーラー出力
        /// </summary>
        private void DispRuler()
        {
            // Graphicsオブジェクト生成
            Graphics g = Graphics.FromHdc(m_hDrawDC);

            // 背景色とフォント色を設定
            CEWin32Api.SetTextColor(m_hDrawDC, CEConstants.FontColor);
            CEWin32Api.SetBkColor(m_hDrawDC, CEConstants.BackColor);

            Pen fontColor = new Pen(Color.FromArgb(255, ColorTranslator.FromHtml(CECommon.ChgRGB(CEConstants.FontColor).ToString())), 1);

            int x, y;
            int baseLinePos = (int)(double)(m_ShareData.m_charHeightPixel * 0.9);
            for (int lineIdx = m_viewLeftColumn; lineIdx < m_viewLeftColumn + m_viewDispColumn; lineIdx++)
            {
                x = (lineIdx - m_viewLeftColumn) * m_ShareData.m_charWidthPixel + m_startColPos;
                if (0 == (lineIdx % 10))
                {
                    y = 0;
                    // カラム番号表示
                    string dispColNum = (lineIdx / 10).ToString();
                    CEWin32Api.RECT rcClip;
                    int xPos = x + (int)(m_ShareData.m_charWidthPixel * 0.3); // 文字を少し（文字幅の1/3ほど）右にずらす
                    rcClip.left = xPos;
                    rcClip.top = y;
                    rcClip.right = xPos + (m_ShareData.m_charWidthPixel * lineIdx.ToString().Length);
                    rcClip.bottom = y + m_ShareData.m_charHeightPixel;
                    CEWin32Api.ExtTextOut(m_hDrawDC, xPos, y, (uint)CEWin32Api.ETOOptions.ETO_CLIPPED | (uint)CEWin32Api.ETOOptions.ETO_OPAQUE, ref rcClip, dispColNum, (uint)dispColNum.Length, null);
                }
                else if (0 == (lineIdx % 5))
                {
                    y = (int)((double)m_ShareData.m_charHeightPixel * 0.4);
                }
                else
                {
                    y = (int)((double)m_ShareData.m_charHeightPixel * 0.65);
                }
                g.DrawLine(fontColor, x, y, x, baseLinePos);
            }

            // 下線
            CEWin32Api.RECT rc;
            CEWin32Api.GetClientRect(this.Handle, out rc);
            g.DrawLine(fontColor, m_startColPos, baseLinePos, rc.right, baseLinePos);

#if false // ShowCaretPos()に移行
            // カーソル位置
            SolidBrush backColor = new SolidBrush(Color.FromArgb(255, ColorTranslator.FromHtml(CECommon.ChgRGB(CEConstants.CaretColPosColor).ToString()))); ;
            int p = m_doc.GetTextPixel(m_ShareData.m_nColumnSpace, m_doc.GetLeftStringP(m_caretStrBuf.Y, m_caretStrBuf.X)) + m_startColPos;
            g.FillRectangle(backColor, p, 0, m_ShareData.m_charWidthPixel, (int)(baseLinePos * 0.3));
#endif

            // Graphicsオブジェクト解放
            g.Dispose();
        }

        /// <summary>
        /// 範囲選択開始・終了位置設定
        /// （選択範囲の起点と終点の位置を元に設定）
        /// </summary>
        private void SetRangePos()
        {
            // 開始・終了行設定
            if (m_eRng.Y == m_cRng.Y)
            {
                if (m_eRng.X < m_cRng.X)
                {
                    m_sRng.X = m_eRng.X;
                    m_eRng.X = m_cRng.X;
                    m_sRng.Y = m_cRng.Y;
                }
                else
                {
                    m_sRng.X = m_cRng.X;
                    m_sRng.Y = m_cRng.Y;
                }
            }
            else if (m_eRng.Y < m_cRng.Y)
            {
                m_sRng.Y = m_eRng.Y;
                m_eRng.Y = m_cRng.Y;
                m_sRng.X = m_eRng.X;
                m_eRng.X = m_cRng.X;
            }
            else
            {
                m_sRng.Y = m_cRng.Y;
                m_sRng.X = m_cRng.X;
            }
        }

        /// <summary>
        /// 縦移動
        /// </summary>
        /// <param name="nPos">移動量</param>
        private void GetCaretPositionV(/* int nVKey,*/ int nPos)
        {
            // 1行目で上を押されたときは何もしない。
            if (m_caretStrBuf.Y + nPos < 0)
            {
                return;
            }

            // 移動後の位置が最終行を超える場合は最終行まで移動
            int line;
            if (m_ShareData.m_wrapPositionFlag)
            {
                // 折返しなし
                line = m_doc.GetLineCountP();
            }
            else
            {
                // 折返しあり
                line = m_doc.GetLineCountL();
            }
            if (m_caretStrBuf.Y + nPos >= line)
            {
                nPos = line - m_caretStrBuf.Y - 1;
            }

            // 選択範囲の開始位置取得
            StarRange();

            // キャレットの行を設定
            m_caretStrBuf.Y += nPos;

            // 画面の一番上と下で上下ボタンが押されたとき
            if ((m_caretStrBuf.Y < m_viewTopRow) ||
                (m_caretStrBuf.Y >= m_viewTopRow + m_viewDispRow))
            {
                // 上または下移動
                // （キャレットの縦座標は変わらないので、m_caretPixel.Yは変更なし）
                MoveScrollV(nPos, line);
            }
            // 画面の一番上・下以外で上下ボタンが押されたとき
            else
            {
                m_caretPixel.Y = m_caretPixel.Y + (m_ShareData.m_charHeightPixel * nPos);
            }

            int posX;
            int idx;
            // 指定した行(物理)の列(ピクセル)から列(物理・ピクセル)を取得する
            GetMovePosV(m_caretStrBuf.Y, m_curCaretColPosPixel, out posX, out idx);
            m_caretStrBuf.X = idx;    // キャレットの配列位置(列)
            m_caretPixel.X = posX;      // キャレットの座標(X)

            // 選択範囲の終了位置取得
            EndRange();

            // スクロールバー再配置
            LayoutScrollBar(line);

            // 画面外表示位置移動
            DispPosMove();

            // 再描画
            CEWin32Api.InvalidateRect(this.Handle, IntPtr.Zero, true);
            //this.Refresh();
        }

        /// <summary>
        /// 指定した物理行の指定した位置（ピクセル）の列情報（ピクセル・物理配列位置）を取得
        /// （フリーカーソルモード対応済）
        /// </summary>
        /// <param name="row">[入力]物理行</param>
        /// <param name="colPixel">[入力]指定列(ピクセル)(文字列の一番左が0)</param>
        /// <param name="posX">[出力]移動先の横位置(ピクセル)(文字列の一番左が0)</param>
        /// <param name="idx">[出力]移動先の横位置(配列)(物理位置)</param>
        private Boolean GetMovePosV(int row, int colPixel, out int posX, out int idx)
        {
            Size sz = Size.Empty;
            int len = 0;

            posX = 0;
            idx = 0;

            // 指定した行(物理)の文字列を取得
            string tmpStr = m_doc.GetLineStringP(row);
            if (tmpStr == "")
            {
                // 空と言うことは不正な位置かEOF行と言うことになる。
                return false;
            }

            // 文字列から改行を削除
            string objStr = m_doc.GetNotLineFeedString(tmpStr);

            if (m_ShareData.m_cursorMode == 0)
            {
                // 通常カーソルモードの場合

                // 文字数を取得
                len = objStr.Length;
            }
            else if (m_ShareData.m_cursorMode == 1)
            {
                // フリーカーソルモードの場合

                // 文字列の長さ(ピクセル)を取得
                // 折り返しのみの場合は長さ(ピクセル)は0
                int strPixel = 0;
                if (objStr != "")
                {
                    strPixel = m_doc.GetTextPixel(m_ShareData.m_nColumnSpace, objStr);
                }

                // 改行以降は半角文字とし、折り返し位置までの文字数を取得
                len = objStr.Length + (m_ShareData.m_wrapPositionPixel - strPixel) / m_ShareData.m_charWidthPixel;
            }
            else
            {
                return false;
            }

            // 画面に表示されていないサイズ（左側）
            int hiddenPixelSize = m_viewLeftColumn * m_ShareData.m_charWidthPixel;
            //posX -= hiddenPixelSize;
            //colPixel += hiddenPixelSize;

            int tabWidthPixel = m_ShareData.m_tabLength * m_ShareData.m_charWidthPixel; // タブ幅（ピクセル）

            int prePosX = 0; // ひとつ前の情報
            for (idx = 0; idx <= len; idx++)
            {
                if ((colPixel/* - hiddenPixelSize*/) < (posX/* - hiddenPixelSize*/))
                {
                    // マウスカーソルの位置を超えたらひとつ前の情報を返す
                    posX = prePosX;
                    break;
                }

                prePosX = posX;

                string str = "";
                if (objStr.Length <= idx)
                {
                    str = " ";
                }
                else
                {
                    str = objStr[idx].ToString();
                }

                // タブの場合
                if (str == "\t")
                {
                    posX += tabWidthPixel - ((posX + tabWidthPixel) % tabWidthPixel);
                }
                // 通常文字（タブ以外）
                else
                {
                    CEWin32Api.GetTextExtentPoint32(m_hDrawDC, str, str.Length, out sz);
                    posX += sz.Width + m_ShareData.m_nColumnSpace;
                }
            }

            // 文字列の長さ＋１分チェックし、マウスポインタの位置を超えたらひとつ前の情報を返すので－１する
            if (idx != 0)
            {
                idx--;
            }

            return true;
        }

        private int GetAmountMove(int nPos)
        {
            int amountMove = 0;
            string patterSymbol = "!\"#$%&'()=~|`{+*}<>?@[;:],./\\";
            string patterAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_";
            string patterNumber = "1234567890";

            // 現在位置の文字を取得
            string s = m_doc.GetLineStringP(m_caretStrBuf.Y);

            if ( nPos > 0)
            {
                // 右探す
                int curIdx = m_caretStrBuf.X;
                for (amountMove = 0; curIdx < s.Length; amountMove++, curIdx++)
                {
                    if (((s[curIdx].ToString() == "\t")                            && (s[curIdx + 1].ToString() == "\t")) ||                            // タブ
                        ((s[curIdx].ToString() == " ")                             && (s[curIdx + 1].ToString() == " ")) ||                             // 空白
                        ((checkZen(s[curIdx].ToString()))                          && (checkZen(s[curIdx + 1].ToString()))) ||                          // 全角文字
                        ((IsSpecifiedPattern(s[curIdx].ToString(), patterSymbol)   && IsSpecifiedPattern(s[curIdx + 1].ToString(), patterSymbol))) ||   // 記号
                        ((IsSpecifiedPattern(s[curIdx].ToString(), patterAlphabet) && IsSpecifiedPattern(s[curIdx + 1].ToString(), patterAlphabet))) || // 英字
                        ((IsSpecifiedPattern(s[curIdx].ToString(), patterNumber)   && IsSpecifiedPattern(s[curIdx + 1].ToString(), patterNumber))) ||   // 数字
                        ((IsSpecifiedPattern(s[curIdx].ToString(), patterAlphabet) && (s[curIdx + 1].ToString() == " "))) ||                            // 英字の隣が空白
                        ((IsSpecifiedPattern(s[curIdx].ToString(), patterAlphabet) && (s[curIdx + 1].ToString() == "\t"))))                             // 英字の隣がタブ
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                amountMove++;
            }
            else if (nPos < 0)
            {
                // 左探す
                int curIdx = m_caretStrBuf.X - 1; // １つ左から検索しだす。現在地から検索しだすと常に０になる場合があるため。
                for (amountMove = 0; curIdx > 0; amountMove--, curIdx--)
                {
                    if (((s[curIdx].ToString() == "\t")                            && (s[curIdx - 1].ToString() == "\t")) ||                            // タブ
                        (((s[curIdx].ToString() == " ")                            && (s[curIdx - 1].ToString() == " "))) ||                            // 空白
                        ((checkZen(s[curIdx].ToString())                           && checkZen(s[curIdx - 1].ToString()))) ||                           // 全角文字
                        ((IsSpecifiedPattern(s[curIdx].ToString(), patterSymbol)   && IsSpecifiedPattern(s[curIdx - 1].ToString(), patterSymbol))) ||   // 記号
                        ((IsSpecifiedPattern(s[curIdx].ToString(), patterAlphabet) && IsSpecifiedPattern(s[curIdx - 1].ToString(), patterAlphabet))) || // 英字
                        ((IsSpecifiedPattern(s[curIdx].ToString(), patterNumber)   && IsSpecifiedPattern(s[curIdx - 1].ToString(), patterNumber))) ||   // 数字
                        ((s[curIdx].ToString() == " ")) ||                                                                                              // 英字の隣が空白
                        ((s[curIdx].ToString() == "\t")))                                                                                               // 英字の隣がタブ
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                amountMove--;
            }

            return amountMove;
        }

        /// <summary>
        /// 横移動
        /// </summary>
        /// <param name="nPos">移動量（文字指定）</param>
        private void GetCaretPositionH(int nPos)
        {
            // 移動量が0の場合は何もしない
            if (nPos == 0)
            {
                return;
            }

            // 矩形選択かつ、先頭から左に移動しようとした場合は何もしない
            if (m_selectType == RACTANGLE_RANGE_SELECT && m_caretStrBuf.X == 0 && nPos < 0)
            {
                return;
            }

            // EOF位置で右（プラス方向）に移動しようとした場合は何もしない
            if (isEofPos(m_caretStrBuf.Y, m_caretStrBuf.X) && nPos > 0)
            {
                return;
            }

            // 選択範囲の開始位置取得
            StarRange();

            // 移動先の物理位置を取得
            int aRow = -1;
            int aCol = -1;
            GetPositionH(m_caretStrBuf.Y, m_caretStrBuf.X, nPos, out aRow, out aCol);

            // 移動量が0の場合は何もしない
            if (m_caretStrBuf.Y == aRow && m_caretStrBuf.X == aCol)
            {
                return;
            }

            // 移動先を保存
            m_caretStrBuf.Y = aRow;
            m_caretStrBuf.X = aCol;
            m_caretPixel.Y = aRow * m_ShareData.m_charHeightPixel;
            
            int lLine;
            int pLine;
            m_doc.getPosition(aRow, out lLine, out pLine);

            int hiddenPixelSize = m_viewLeftColumn * m_ShareData.m_charWidthPixel;

#if false
            if (m_cursorMode == 0)
            {
                // 通常カーソルモード

                // 指定した物理行のキャレット位置までの文字列を取得
                string str = m_doc.GetLineStringP(aRow, lLine, pLine).Substring(0, aCol);

                // 指定した文字列のピクセル数を取得
                m_caretPixel.X = m_doc.GetTextPixel(m_nColumnSpace, str);
            }
            else
            {
            // フリーカーソルモード
#endif

            // 指定した物理行の文字列から改行を除いた文字列を取得
            string fStr = m_doc.GetNotLineFeedStringP(aRow, lLine, pLine);
            int len = fStr.Length;
            if (len < aCol)
            {
                // 改行より後ろにキャレットがある場合
#if false
                string str1 = fStr.Substring(0, len);
                m_caretPixel.X = m_doc.GetTextPixel(m_nColumnSpace, str1);
#endif
                m_caretPixel.X = m_doc.GetTextPixel(m_ShareData.m_nColumnSpace, fStr);
                m_caretPixel.X += (aCol - len) * m_ShareData.m_charWidthPixel;
            }
            else
            {
                // 指定した物理行のキャレット位置までの文字列を取得
                string str = m_doc.GetLineStringP(aRow, lLine, pLine).Substring(0, aCol);

                // 指定した文字列のピクセル数を取得
                m_caretPixel.X = m_doc.GetTextPixel(m_ShareData.m_nColumnSpace, str);
            }
#if false
            }
#endif

            // 選択範囲の終了位置取得
            EndRange();

            // 現在の桁（ピクセル）を基準値として保存
            m_curCaretColPosPixel = m_caretPixel.X;

            // スクロールバー再配置
            LayoutScrollBar();

            // 画面外表示位置移動
            DispPosMove();

            // 再描画
            CEWin32Api.InvalidateRect(this.Handle, IntPtr.Zero, true);
            //this.Refresh();
        }

        /// <summary>
        /// 範囲外表示位置移動
        /// </summary>
        /// <remarks>
        /// 表示位置が【画面外】の場合、表示位置を【最寄り】の位置まで移動
        /// </remarks>
        private void DispPosMove()
        {
            //////////////////////////////////
            // 画面外（上下）
            //////////////////////////////////

            if (m_caretStrBuf.Y < m_viewTopRow)
            {
                // キャレットが画面より上にある
                MoveScrollV(m_caretStrBuf.Y - m_viewTopRow);
            }
            else if (m_caretStrBuf.Y > m_viewTopRow + m_viewDispRow - 1)
            {
                // キャレットが画面より下にある
                MoveScrollV(m_caretStrBuf.Y - (m_viewTopRow + m_viewDispRow - 1));
            }

            //////////////////////////////////
            // キャレット位置までの文字列取得
            //////////////////////////////////

            int lLine;
            int pLine;
            m_doc.getPosition(m_caretStrBuf.Y, out lLine, out pLine);

            int columnStringLen = m_doc.GetColumnLength(m_doc.GetLeftStringP(m_caretStrBuf.Y, m_caretStrBuf.X, lLine, pLine));

            if (m_ShareData.m_cursorMode == 1)
            {
                // フリーカーソルモード

                // 現在行のテキストの長さを取得（全角：２、半角：１）
                string nowStr = m_doc.GetNotLineFeedString(m_doc.GetLineStringP(m_caretStrBuf.Y, lLine, pLine));
                int len = nowStr.Length;    // 文字数取得

                // 改行以降にキャレットがあるか？
                // 現在位置がテキストの長さより大きい場合は改行以降にキャレットがあるということ
                if (m_caretStrBuf.X > len)
                {
                    // 改行以降にキャレットがある

                    // キャレット位置までの長さ（全角：２、半角：１）
                    columnStringLen += m_caretStrBuf.X - len;
                }
            }

            //////////////////////////////////
            // 画面外（左右）
            //////////////////////////////////

            // 画面より左にあるか
            if (columnStringLen - m_ShareData.m_scrollColSpage < m_viewLeftColumn)
            {
                MoveScrollH((columnStringLen - m_ShareData.m_scrollColSpage) - m_viewLeftColumn);
            }
            // 画面より右にあるか
            else if (columnStringLen + m_ShareData.m_scrollColSpage > m_viewLeftColumn + m_viewDispColumn)
            {
                MoveScrollH((columnStringLen + m_ShareData.m_scrollColSpage) - (m_viewLeftColumn + m_viewDispColumn));
            }
        }

        /// <summary>
        /// 範囲外キャレット移動
        /// </summary>
        /// <remarks>
        /// キャレットが【画面外】の場合、キャレットを【最寄り】の位置まで移動
        /// </remarks>
        private void CaretPosMove()
        {
            //////////////////////////////////
            // 画面外（上下）
            //////////////////////////////////

            if (m_caretStrBuf.Y < m_viewTopRow)
            {
                // キャレットが画面より上にある
                MoveScrollV(m_caretStrBuf.Y - m_viewTopRow);
                m_caretPixel.Y = 0;
            }
            else if (m_caretStrBuf.Y > m_viewTopRow + m_viewDispRow - 1)
            {
                // キャレットが画面より下にある
                MoveScrollV(m_caretStrBuf.Y - (m_viewTopRow + m_viewDispRow - 1));
                m_caretPixel.Y = (m_viewDispRow - 1) * m_ShareData.m_charHeightPixel;
                m_caretStrBuf.Y = m_doc.GetLineCountP() - 1;
            }

            //////////////////////////////////
            // キャレット位置までの文字列取得
            //////////////////////////////////

            int lLine;
            int pLine;
            m_doc.getPosition(m_caretStrBuf.Y, out lLine, out pLine);

            int columnStringLen = m_doc.GetColumnLength(m_doc.GetLeftStringP(m_caretStrBuf.Y, m_caretStrBuf.X, lLine, pLine));

            if (m_ShareData.m_cursorMode == 1)
            {
                // フリーカーソルモード

                // 現在行のテキストの長さを取得
                string nowStr = m_doc.GetNotLineFeedString(m_doc.GetLineStringP(m_caretStrBuf.Y, lLine, pLine));
                int len = nowStr.Length;    // 文字数取得

                // 改行以降にキャレットがあるか？
                // 現在位置がテキストの長さより大きい場合は改行以降にキャレットがあるということ
                if (m_caretStrBuf.X > len)
                {
                    // 改行以降にキャレットがある
                    //int bbb = this.GetColumnLength(nowStr);
                    //aaa = bbb + (m_caretStrBuf.X - len);
                    columnStringLen = m_caretStrBuf.X;
                }
            }

            //////////////////////////////////
            // 画面外（左右）
            //////////////////////////////////

            if (columnStringLen < m_viewLeftColumn)
            {
                // 画面より左にある
                MoveScrollH(columnStringLen - m_viewLeftColumn);
                m_caretPixel.X = 0;
            }
            else if (columnStringLen > m_viewLeftColumn + m_viewDispColumn)
            {
                // 画面より右にある
                MoveScrollH(columnStringLen - (m_viewLeftColumn + m_viewDispColumn));
                m_caretPixel.X = columnStringLen * (m_ShareData.m_charWidthPixel + m_ShareData.m_nColumnSpace);
            }
        }

#if false
        /// <summary>
        /// 現在の位置（ピクセル）更新
        /// </summary>
        public void UpdateNowPos()
        {
            // 先頭(0,0)からの現在位置(ピクセル)
            //string nowString = EditTextList[caretStrBufRow].ToString().Substring(0, caretStrBufColumn);
            //m_nowPosX = com.GetTextPixel(this.Handle, m_Font/*this.Font*/, m_nColumnSpace, nowString);
            string nowString = doc.GetLeftString(caretStrBufRow, caretStrBufColumn);
            m_nowPosX = doc.GetTextPixel(this.Handle, m_Font/*this.Font*/, m_nColumnSpace, nowString);
            m_nowPosY = caretStrBufRow * m_charHeight;
        }

        /// <summary>
        /// 画面左上の位置（ピクセル）更新
        /// </summary>
        private void UpdateScreenPos()
        {
            // 画面左上の位置(ピクセル)
            m_screenPosX = m_viewLeftColumn * (m_nColumnSpace + m_charWidth);
            m_screenPosY = m_viewTopRow * m_charHeight;

        }

        /// <summary>
        /// 画面の幅と高さ（ピクセル）更新
        /// </summary>
        private void UpdateScreenWH()
        {
            // 画面の幅
            m_screenWidth = m_viewDispColumn * (m_nColumnSpace + m_charWidth);
            m_screenHeight = m_viewDispRow * m_charHeight;
        }
#endif

        /// <summary>
        /// カーソルの上下移動
        /// </summary>
        /// <param name="nPos"></param>
        private void CursorKeyV(int nPos)
        {
            GetCaretPositionV(nPos);
        }

        /// <summary>
        /// カーソルの左右移動
        /// </summary>
        /// <param name="nPos"></param>
        private void CursorKeyH(int nPos)
        {
            byte[] KeyState = new byte[256];
            CEWin32Api.GetKeyboardState(KeyState);
            if (((KeyState[CEWin32Api.VK_CTRL] & 0x80) != 0) && // Ctrlキーかつ
                (((KeyState[CEWin32Api.VK_LEFT] & 0x80) != 0) || ((KeyState[CEWin32Api.VK_RIGHT] & 0x80) != 0))) // 左キーまたは右キー
            {
                // Ctrl+左右
                nPos = GetAmountMove(nPos);
            }

            GetCaretPositionH(nPos);
        }

        /// <summary>
        /// ホームキー
        /// </summary>
        private void HomeKey()
        {
            byte[] KeyState = new byte[256];
            CEWin32Api.GetKeyboardState(KeyState);
            if ((KeyState[CEWin32Api.VK_CTRL] & 0x80) != 0)
            {
                // Ctrl+Home

                CursorKeyH(-m_caretStrBuf.X);
                CursorKeyV(-m_caretStrBuf.Y);
            }
            else
            {
                // Home

                if (m_ShareData.m_cursorMode == 0)
                {
                    // 通常カーソルモード
                    int mPos = m_doc.GetLeftStringP(m_caretStrBuf.Y, m_caretStrBuf.X).Length;
                    CursorKeyH(-mPos);
                }
                else
                {
                    // フリーカーソルモード
                    m_caretPixel.X = 0;
                    m_caretStrBuf.X = 0;
                    // 現在の桁（ピクセル）を基準値として保存
                    m_curCaretColPosPixel = m_caretPixel.X;
                }
                // 現在の桁（ピクセル）を基準値として保存
                // 上記どのような結果であろうとホームが押されたので先頭へ
                m_curCaretColPosPixel = 0;
            }

            // スクロールバー再配置
            LayoutScrollBar();

            // 画面外表示位置移動
            DispPosMove();

            // 再描画
            CEWin32Api.InvalidateRect(this.Handle, IntPtr.Zero, true);
            //this.Refresh();
        }

        /// <summary>
        /// エンドキー
        /// </summary>
        private void EndKey()
        {
            byte[] KeyState = new byte[256];
            CEWin32Api.GetKeyboardState(KeyState);

            int lLine;
            int pLine;
            m_doc.getPosition(m_caretStrBuf.Y, out lLine, out pLine);

            if ((KeyState[CEWin32Api.VK_CTRL] & 0x80) != 0)
            {
                // Ctrl+End

                CursorKeyH(-m_caretStrBuf.X);
                CursorKeyV(m_doc.GetLineCountP() - (m_caretStrBuf.Y + 1));
                CursorKeyH(m_doc.GetLineStringP(m_caretStrBuf.Y, lLine, pLine).Length);
            }
            else
            {
                // End

                if (m_ShareData.m_cursorMode == 1)
                {
                    // フリーカーソルの場合、カーソルを先頭に移動してから通常カーソルのEnd処理を実行
                    m_caretPixel.X = 0;
                    m_caretStrBuf.X = 0;
                }

                int len = m_doc.GetNotLineFeedStringP(m_caretStrBuf.Y, lLine, pLine).Length;
                // 折り返しの場合は1文字手前
                if (isWrapPos(m_caretStrBuf.Y, lLine, pLine))
                {
                    len--;
                }
                int cLen = m_doc.GetLeftStringP(m_caretStrBuf.Y, m_caretStrBuf.X, lLine, pLine).Length;
                CursorKeyH(len - cLen);
            }

            // スクロールバー再配置
            LayoutScrollBar();

            // 画面外表示位置移動
            DispPosMove();

            // 再描画
            CEWin32Api.InvalidateRect(this.Handle, IntPtr.Zero, true);
            //this.Refresh();
        }

        /// <summary>
        /// リターンキー
        /// </summary>
        private void ReturnKey()
        {
            // リターンキーの場合はカーソルモードを通常カーソルモードとして処理する
            int bkCursorMode = m_ShareData.m_cursorMode;
            m_ShareData.m_cursorMode = 0;

            if (m_selectType != NONE_RANGE_SELECT)
            {
                // 選択範囲の文字列を削除
                DeleteRange();
            }

            /*****************************************************/
            /* Undo / Redo アンドゥ リドゥ                       */
            /*****************************************************/
            UndoRedoOpe.PreUndoRedo(m_caretStrBuf, m_selectType, this);

            // キャレット位置に改行マークを挿入する
            int mx = 0; // 横移動量
            MultiLineInsertText(m_caretStrBuf.Y, m_caretStrBuf.X, CEConstants.LineFeed, ref mx);

            // 横移動
            GetCaretPositionH(mx);

            /*****************************************************/
            /* Undo / Redo アンドゥ リドゥ                       */
            /*****************************************************/
            UndoRedoOpe.ProUndoRedo(m_caretStrBuf, UndoRedoCode.UR_INSERT, CEConstants.LineFeed, this);

            // スクロールバー再配置
            LayoutScrollBar();

            // 画面外表示位置移動
            DispPosMove();

            // 再描画
            CEWin32Api.InvalidateRect(this.Handle, IntPtr.Zero, true);
            //this.Refresh();

            // カーソルモードを元に戻す
            m_ShareData.m_cursorMode = bkCursorMode;
        }

        /// <summary>
        /// バックスペースキー
        /// </summary>
        private void BackSpaceKey()
        {
            // 範囲選択状態の場合、選択されている文字列を削除する
            if (m_selectType != NONE_RANGE_SELECT)
            {
                DeleteRange();
            }
            else
            {
                // 行列が(0,0)の場合は何もしない。
                if (m_caretStrBuf.Y == 0 && m_caretStrBuf.X == 0)
                {
                    return;
                }

                // キャレット位置確認
                if (!IsCaretRangeP(m_caretStrBuf.Y, m_caretStrBuf.X))
                {
                    // キャレット範囲外

                    // キャレットを左に移動
                    GetCaretPositionH(-1);
                }
                else
                {
                    // キャレット範囲内

                    // 削除位置に文字列がある場合、カーソルモードを通常カーソルモードとして処理する
                    int bkCursorMode = m_ShareData.m_cursorMode;
                    m_ShareData.m_cursorMode = 0;

                    // キャレットを左に移動
                    GetCaretPositionH(-1);

                    /*****************************************************/
                    /* Undo / Redo アンドゥ リドゥ                       */
                    /*****************************************************/
                    UndoRedoOpe.PreUndoRedo(m_caretStrBuf, m_selectType, this);

                    // キャレット位置の文字を削除
                    string delStr = m_doc.DeleteCharP(m_caretStrBuf.Y, m_caretStrBuf.X);

                    /*****************************************************/
                    /* Undo / Redo アンドゥ リドゥ                       */
                    /*****************************************************/
                    UndoRedoOpe.ProUndoRedo(m_caretStrBuf, UndoRedoCode.UR_DELETE, delStr, this);

                    // カーソルモードを元に戻す
                    m_ShareData.m_cursorMode = bkCursorMode;
                }
            }

            // スクロールバー再配置
            LayoutScrollBar();

            // 画面外表示位置移動
            DispPosMove();

            // 再描画
            CEWin32Api.InvalidateRect(this.Handle, IntPtr.Zero, true);
            //this.Refresh();
        }

        /// <summary>
        /// デリートキー
        /// </summary>
        private void DeleteKey()
        {
            // 範囲選択状態の場合、選択されている文字列を削除する
            if (m_selectType != NONE_RANGE_SELECT)
            {
                DeleteRange();
            }
            else
            {
                // キャレット位置が範囲外またはEOFの場合、何もしない
                if ((!IsCaretRangeP(m_caretStrBuf.Y, m_caretStrBuf.X)) || (isEofPos(m_caretStrBuf.Y, m_caretStrBuf.X)))
                {
                    return;
                }

                /*****************************************************/
                /* Undo / Redo アンドゥ リドゥ                       */
                /*****************************************************/
                UndoRedoOpe.PreUndoRedo(m_caretStrBuf, m_selectType, this);

                // 現在位置の文字削除
                string delStr = m_doc.DeleteCharP(m_caretStrBuf.Y, m_caretStrBuf.X);

                /*****************************************************/
                /* Undo / Redo アンドゥ リドゥ                       */
                /*****************************************************/
                UndoRedoOpe.ProUndoRedo(m_caretStrBuf, UndoRedoCode.UR_DELETE, delStr, this);
            }

            // スクロールバー再配置
            LayoutScrollBar();

            // 画面外表示位置移動
            DispPosMove();

            // 再描画
            CEWin32Api.InvalidateRect(this.Handle, IntPtr.Zero, true);
            //this.Refresh();
        }

        /// <summary>
        /// エスケープキー
        /// </summary>
        public void EscapeKey()
        {
            // 範囲選択の解除
            RangeCancel();

            // 検索終了
            m_searchType = NOT_SEARCH_TYPE;

            // 再描画
            CEWin32Api.InvalidateRect(this.Handle, IntPtr.Zero, true);
            //this.Refresh();
            //DrawTextList();
        }

        /// <summary>
        /// キャラクターキー
        /// </summary>
        private void CharKey(int nVKey)
        {
            // Ctrlとのコンビネーションキーは何もしない
            // 暫定対応
            if ((0 <= nVKey && nVKey <= 31) &&
                nVKey != 9)
            {
                return;
            }

            if (m_selectType != NONE_RANGE_SELECT)
            {
                // 選択範囲の文字列を削除
                DeleteRange();
            }

            /*****************************************************/
            /* Undo / Redo アンドゥ リドゥ                       */
            /*****************************************************/
            UndoRedoOpe.PreUndoRedo(m_caretStrBuf, m_selectType, this);

            //int nVKey = (int)wParam;
            char c = Convert.ToChar(nVKey);
            string newChar = c.ToString();

#if false // 改行が来ることはないのでコメント
            // 改行の場合何もしない
            if (newChar != "\r" && newChar != "\n" && newChar != "\b")
            {
#endif
                m_doc.InsertLineTextP(m_caretStrBuf.Y/*m_caretStrBufRow*/, m_caretStrBuf.X/*m_caretStrBufColumn*/, newChar);
                CEWin32Api.InvalidateRect(this.Handle, IntPtr.Zero, false); // WM_PAINTが実行される。
            //this.Refresh();
                //this.SetTextData();
#if false
                if (m_editLine.IndexOf(m_caretStrBuf.Y + 1) == -1) {
                    m_editLine.Add(m_caretStrBuf.Y + 1);
                }
#endif
                LayoutScrollBar(); // スクロールバー再配置
                GetCaretPositionH(newChar.Length); // 入力文字数分カーソルキー移動

                /*****************************************************/
                /* Undo / Redo アンドゥ リドゥ                       */
                /*****************************************************/
                UndoRedoOpe.ProUndoRedo(m_caretStrBuf, UndoRedoCode.UR_INSERT, newChar, this);

                // スクロールバー再配置
                LayoutScrollBar();

                // 画面外表示位置移動
                DispPosMove();

                // 再描画
                CEWin32Api.InvalidateRect(this.Handle, IntPtr.Zero, true);
            //this.Refresh();
#if false
            }
#endif
        }

        /// <summary>
        /// 描画
        /// </summary>
        private void DrawTextList()
        {
            try
            {
                CEWin32Api.RECT rec;
                CEWin32Api.PAINTSTRUCT ps;

                // 描画領域設定
                CEWin32Api.GetClientRect(this.Handle, out rec);
                IntPtr hdc = CEWin32Api.GetDC(this.Handle);
                IntPtr hDrawBmp = CEWin32Api.CreateCompatibleBitmap(hdc, rec.right, rec.bottom);
                CEWin32Api.SelectObject(m_hDrawDC, hDrawBmp);
                CEWin32Api.ReleaseDC(this.Handle, hdc);
                CEWin32Api.DeleteObject(hDrawBmp);

                // 背景色を白に設定
                IntPtr hDefBrush = CEWin32Api.SelectObject(m_hDrawDC, CEWin32Api.CreateSolidBrush(CEConstants.BackColor));
                CEWin32Api.PatBlt(m_hDrawDC, 0, 0, rec.right, rec.bottom, CEWin32Api.TernaryRasterOperations.PATCOPY);
                CEWin32Api.SelectObject(m_hDrawDC, hDefBrush);

                Stopwatch sw = new Stopwatch();
                sw.Start();

                // 描画する画面を設定
                this.SetTextData();

                sw.Stop();
                Console.WriteLine("描画する画面を設定:" + sw.Elapsed);
                //sw.Restart();

                // ルーラー表示
                this.DispRuler();

                //sw.Stop();
                //Console.WriteLine("ルーラー表示:" + sw.Elapsed);
                //sw.Restart();

                // スクロールバー間の色を塗る
                this.LayoutScrollBarBetweenColor();

                //sw.Stop();
                //Console.WriteLine("スクロールバー間の色を塗る:" + sw.Elapsed);
                //sw.Restart();

                // キャレット描画
                this.ShowCaret();

                //sw.Stop();
                //Console.WriteLine("キャレット描画:" + sw.Elapsed);
                //sw.Restart();

                // 描画（ビットマップ転送）
                IntPtr hDC = CEWin32Api.BeginPaint(this.Handle, out ps);
                CEWin32Api.BitBlt(hDC, 0, 0, rec.right, rec.bottom, m_hDrawDC, 0, 0, CEWin32Api.TernaryRasterOperations.SRCCOPY);
                CEWin32Api.EndPaint(this.Handle, ref ps);

                // キャレットの現在位置を通知
                if (CaretPos != null)
                {
                    CaretPositionEventArgs e = new CaretPositionEventArgs();
                    e.x = m_doc.GetColumnLength(m_doc.GetLeftStringP(m_caretStrBuf.Y, m_caretStrBuf.X));
                    e.y = m_caretStrBuf.Y;
                    e.encode = m_readEncode.EncodingName;
                    CaretPos(this, e);
                }
            }
            catch (Exception e) {
                Debug.WriteLine("An exception occurred in the drawing process." + "\n\n>>> Exception message\n" + e.Message);
                //MessageBox.Show("An exception occurred in the drawing process." + "\n\n>>> Exception message\n" + e.Message);
            }
        }

        /// <summary>
        /// 画面にテキストを表示
        /// </summary>
        private void SetTextData()
        {
            try
            {
                m_lLineNum = new List<string>();
                int py = -1;
                int px = -1;
                int length = 0;
                Boolean wrap = false;
                //m_maxStringPixelLength = 0;
                string str = "";
                // m_viewDispRowは完全に表示されている行の数
                // 中途半端な表示の行も表示するので +1 している
                int lineCntP = m_doc.GetLineCountP();
                int lLine = -1;
                int pLine = -1;
                //getPosition(m_viewTopRow, out lLine, out pLine);
                lLine = m_viewTopLineL;
                pLine = m_viewTopLineLP;
                string lineNumStr = "";
                // 画面表示幅をピクセル単位で取得
                m_screenWidth = m_viewDispColumn * (m_ShareData.m_nColumnSpace + m_ShareData.m_charWidthPixel);
                for (int index = 0; index < m_viewDispRow + 1; index++)
                {
                    int row = index + m_viewTopRow;
                    if (row < lineCntP)
                    {
                        str = getNextPositionString(ref lLine, ref pLine, out wrap);
                        if (pLine == 0)
                        {
                            m_lLineNum.Add((lLine + 1).ToString());
                            lineNumStr = (lLine + 1).ToString();
                        }
                        else
                        {
                            m_lLineNum.Add("");
                            lineNumStr = "";
                        }
                        px = m_viewLeftColumn * (m_ShareData.m_charWidthPixel + m_ShareData.m_nColumnSpace);
                        py = index * m_ShareData.m_charHeightPixel;

                        // 文字列表示
                        // テキストデータ表示
                        length = this.TextOutLine(str, -px, py, CEConstants.FontColor, row);

                        // 改行表示
                        if (!string.IsNullOrEmpty(m_doc.GetLineFeed(str)))
                        {
                            this.TextOutLineBreak(length, py, row, str);
                        }
                        // 折り返しマーク表示
                        else if (wrap)
                        {
                            this.TextOutLine("<", length, py, CEConstants.wrapColor, row);
                        }
                        // EOF表示
                        else if (isEofLine(lLine, pLine))
                        {
                            // 空行（改行もないEOFのみの行）の場合、行数は表示しない
                            if (m_doc.GetLineStringP(row) == "")
                            {
                                lineNumStr = "";
                            }
                            this.TextOutLine("[EOF]", length, py, CEConstants.eofColor, row);
                        }

                        // 行番号出力
                        TextOutLineNumber(lineNumStr, py, lLine);
                    }
                    else
                    {
                        m_lLineNum.Add("");
                        continue;
                    }
                    pLine++;
                } // for
            } // try
            catch (Exception e)
            {
                Console.Write("描画（DrawTextList()）で例外発生：" + e);
            }
        }

        /// <summary>
        /// 表示データ有無
        /// </summary>
        /// <returns>true:表示データあり / false:表示データなし</returns>
        public Boolean IsTextData()
        {
            if (m_doc.GetLineStringP(0) != "")
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 編集状態有無
        /// </summary>
        /// <returns>true:編集データあり / false:編集データなし</returns>
        public Boolean GetEditStatus()
        {
            // 文字が入力されている、かつ、編集されている場合
            if (m_doc.GetEditStatus())
            {
                return true;
            }
            return false;
        }

        public void OnTick_FormsTimer(object sender, EventArgs e)
        {
            m_clickFlag = 0;
            // タイマーを停止
            timer.Stop();
        }

        /// <summary>
        /// マウス左クリック
        /// </summary>
        /// <param name="lParam"></param>
        private void MouseLeftClick(IntPtr lParam)
        {
            // 検索終了
            m_searchType = NOT_SEARCH_TYPE;

            if (!m_dragFlg)
            {
                if (m_clickFlag == 0)
                {
                    // タイマーを開始
                    timer.Start();
                    m_clickFlag++;
                }
                else if (m_clickFlag == 1)
                {
                    m_clickFlag++;
                    timer.Stop();
                }

                // タイマインターバール以内に再度MouseLeftClickイベントが発生したのでトリプルクリックと認定
                if (m_clickFlag == 2)
                {
                    // トリプルクリック
                    m_clickFlag = 0;
                    MouseLeftTripleClick();
                    return;
                }
            }

            // 現コンポーネントにフォーカスをあてる
            this.Focus();

            // 選択範囲の開始位置取得
            StarRange();

            // マウスカーソルの位置から座標を取得
            PointToCaretPos();
#if false
            // クリック座標を取得
            Point cp = this.PointToClient(Cursor.Position);

            // ---------------------------------------------
            // 座標取得
            // ---------------------------------------------
            //Point sp = Cursor.Position;
            //Point cp = this.PointToClient(sp);
            int xPos = cp.X - m_startColPos;
            int yPos = cp.Y - m_startRowPos;
            int hiddenPixelSize = m_viewLeftColumn * m_ShareData.m_charWidthPixel;
            xPos += hiddenPixelSize;

            // ---------------------------------------------
            // 行計算
            // ---------------------------------------------
            int row = (yPos / m_ShareData.m_charHeightPixel) + m_viewTopRow;
            if (m_doc.GetLineCountP() > row)
            {
                m_caretStrBuf.Y = row;
            }
            else {
                m_caretStrBuf.Y = (m_doc.GetLineCountP() - 1) + m_viewTopRow;
            }

            // ---------------------------------------------
            // 列計算
            // ---------------------------------------------
#if false // 共通処理呼び出しによる高速化
            int afterStringPixel = 0;
            string str = GetNotLineFeedStringP(m_caretStrBuf.Y);

            int idx;
            int hidePixcel = (m_viewLeftColumn * (m_charWidthPixel + m_nColumnSpace));
            string chkStr = string.Empty;
            for (idx = 0; idx < str.Length; idx++)
            {
                // 現在の改行を含む文字列を取得
                chkStr = GetLeftString(m_caretStrBuf.Y, idx + 1);
                afterStringPixel = GetTextPixel(this.Handle, m_Font/*this.Font*/, m_nColumnSpace, chkStr);
                if ((afterStringPixel - hidePixcel) > xPos)
                {
                    break;
                }
            }
            m_caretStrBuf.X = idx;
#else
            int posX;
            int idx;
            // 指定した行(物理)の列(ピクセル)から列(物理・ピクセル)を取得する
            GetMovePosV(m_caretStrBuf.Y, xPos, out posX, out idx);
            m_caretStrBuf.X = idx;    // キャレットの配列位置(列)
            m_caretPixel.X = posX;      // キャレットの座標(X)
#endif
#endif
            // 再描画
            CEWin32Api.InvalidateRect(this.Handle, IntPtr.Zero, true);
            //this.Refresh();

            // 選択範囲の終了位置取得
            EndRange();

            // ドラッグ中フラグをON
            m_dragFlg = true;

            // 範囲外キャレット移動
            CaretPosMove();

            // 現在の桁（ピクセル）を基準値として保存
            m_curCaretColPosPixel = m_caretPixel.X;
        }

        /// <summary>
        /// マウス左クリックを離す
        /// </summary>
        /// <param name="lParam"></param>
        private void MouseLeftUp(IntPtr lParam)
        {
            // ドラッグ中フラグをOFF
            m_dragFlg = false;
        }

        /// <summary>
        /// マウス左ダブルクリック
        /// </summary>
        /// <param name="lParam"></param>
        private void MouseLeftDoubleClick(IntPtr lParam)
        {
            string patternSymbol = "!\"#$%&'()=~|`{+*}<>?@[;:],./\\";
            string patternAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_";
            string patternNumber = "1234567890";

            // マウスカーソルの位置から座標を取得
            PointToCaretPos();

            // 現在位置の文字を取得
            string s = m_doc.GetLineStringP(m_caretStrBuf.Y);

            // 行
            m_sRng.Y = m_caretStrBuf.Y;
            m_eRng.Y = m_caretStrBuf.Y;

            // 前検索
            int curIdx = m_caretStrBuf.X;
            for (; curIdx > 0; curIdx--)
            {
                if (((s[curIdx].ToString() == "\t")                             && (s[curIdx - 1].ToString() == "\t")) ||                             // タブ
                    (((s[curIdx].ToString() == " ")                             && (s[curIdx - 1].ToString() == " "))) ||                             // スペース
                    ((checkZen(s[curIdx].ToString())                            && checkZen(s[curIdx - 1].ToString()))) ||                            // 全角文字
                    ((IsSpecifiedPattern(s[curIdx].ToString(), patternSymbol)   && IsSpecifiedPattern(s[curIdx - 1].ToString(), patternSymbol))) ||   // 記号
                    ((IsSpecifiedPattern(s[curIdx].ToString(), patternAlphabet) && IsSpecifiedPattern(s[curIdx - 1].ToString(), patternAlphabet))) || // 英字
                    ((IsSpecifiedPattern(s[curIdx].ToString(), patternNumber)   && IsSpecifiedPattern(s[curIdx - 1].ToString(), patternNumber))))     // 数字
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
            m_sRng.X = curIdx;

            // 後ろ検索
            curIdx = m_caretStrBuf.X;
            int len = s.Length;
            for (; curIdx < len - 1; curIdx++)
            {
                
                if (((s[curIdx].ToString() == "\t")                             && (s[curIdx + 1].ToString() == "\t")) ||                             // タブ
                    (((s[curIdx].ToString() == " ")                             && (s[curIdx + 1].ToString() == " "))) ||                             // 空白
                    ((checkZen(s[curIdx].ToString())                            && checkZen(s[curIdx + 1].ToString()))) ||                            // 全角文字
                    ((IsSpecifiedPattern(s[curIdx].ToString(), patternSymbol)   && IsSpecifiedPattern(s[curIdx + 1].ToString(), patternSymbol))) ||   // 記号
                    ((IsSpecifiedPattern(s[curIdx].ToString(), patternAlphabet) && IsSpecifiedPattern(s[curIdx + 1].ToString(), patternAlphabet))) || // 英字
                    ((IsSpecifiedPattern(s[curIdx].ToString(), patternNumber)   && IsSpecifiedPattern(s[curIdx + 1].ToString(), patternNumber))))     // 数字
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
            m_eRng.X = curIdx + 1;
            m_selectType = NORMAL_RANGE_SELECT;

            // 再描画
            CEWin32Api.InvalidateRect(this.Handle, IntPtr.Zero, true);
        }

        private Boolean checkZen(string s)
        {
            if (m_doc.GetColumnLength(s) == 2)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 指定されたパターンの文字がチェック
        /// </summary>
        /// <param name="s"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private Boolean IsSpecifiedPattern(string s, string pattern)
        {
            for (int i = 0; i < pattern.Length; i++)
            {
                if (s == pattern[i].ToString())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// マウス左ボタントリプルクリック
        /// </summary>
        private void MouseLeftTripleClick()
        {
            // マウスカーソルの位置から座標を取得
            PointToCaretPos();

            // 現在位置の文字を取得
            string s = m_doc.GetLineStringP(m_caretStrBuf.Y);

            // 行
            m_sRng.Y = m_caretStrBuf.Y;
            m_eRng.Y = m_caretStrBuf.Y;

            // 列
            m_sRng.X = 0;
            m_eRng.X = s.Length;

            m_selectType = NORMAL_RANGE_SELECT;

            // 再描画
            CEWin32Api.InvalidateRect(this.Handle, IntPtr.Zero, true);
        }

        /// <summary>
        /// マウスカーソルの位置から座標を取得
        /// </summary>
        private void PointToCaretPos()
        {
            // クリック座標を取得
            Point cp = this.PointToClient(Cursor.Position);

            // ---------------------------------------------
            // 座標取得
            // ---------------------------------------------
            //Point sp = Cursor.Position;
            //Point cp = this.PointToClient(sp);
            int xPos = cp.X - m_startColPos;
            int yPos = cp.Y - m_startRowPos;
            int hiddenPixelSize = m_viewLeftColumn * m_ShareData.m_charWidthPixel;
            xPos += hiddenPixelSize;

            // ---------------------------------------------
            // 行計算
            // ---------------------------------------------
            int row = (yPos / m_ShareData.m_charHeightPixel) + m_viewTopRow;
            if (m_doc.GetLineCountP() > row)
            {
                m_caretStrBuf.Y = row;
            }
            else
            {
                m_caretStrBuf.Y = (m_doc.GetLineCountP() - 1) + m_viewTopRow;
            }

            // ---------------------------------------------
            // 列計算
            // ---------------------------------------------
            int posX;
            int idx;
            // 指定した行(物理)の列(ピクセル)から列(物理・ピクセル)を取得する
            GetMovePosV(m_caretStrBuf.Y, xPos, out posX, out idx);
            m_caretStrBuf.X = idx;    // キャレットの配列位置(列)
            m_caretPixel.X = posX;      // キャレットの座標(X)
        }

        /// <summary>
        /// マウス移動
        /// </summary>
        /// <param name="lParam"></param>
        private void MousePointerMove(IntPtr lParam)
        {
            int x = (int)lParam & 0xffff;
            int y = (int)lParam >> 16;

            if ((m_startColPos < x) && (m_startRowPos < y))
            {
                Cursor = Cursors.IBeam;
            }
#if false // ビットマップのマウスカーソルが綺麗に作れたら有効にする
            else if (x < m_startColPos)
            {
                Bitmap b = new Bitmap("LeftArrow.bmp");
                b.MakeTransparent();
                this.Cursor = CreateCursor(b, 32,0);
            }
#else
            else if (x < m_startColPos)
            {
                Cursor = Cursors.IBeam;
            }
#endif
            else
            {
                Cursor = Cursors.Default;
            }

            if (m_dragFlg)
            {
                MouseLeftClick(lParam);
            }
        }

        public static Cursor CreateCursor(Bitmap bmp, int xHotspot, int yHotspot)
        {
            // HIconへのポインタ
            IntPtr ptr = bmp.GetHicon();
            // アイコン情報
            CEWin32Api.IconInfo tmp = new CEWin32Api.IconInfo();
            // アイコン情報に画像を与える
            CEWin32Api.GetIconInfo(ptr, ref tmp);
            // アイコンのポイント位置を指定
            tmp.xHotspot = xHotspot;
            tmp.yHotspot = yHotspot;
            // アイコンはカーソルである事を示す
            tmp.fIcon = false;
            // アイコンを作成
            ptr = CEWin32Api.CreateIconIndirect(ref tmp);
            return new Cursor(ptr);
        }

        /// <summary>
        /// 一文字の幅（ピクセル）を取得する
        /// </summary>
        /// <param name="hdl"></param>
        /// <param name="f"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        private int GetOneWordPixel(string s)
        {
            Size sz;
            CEWin32Api.GetTextExtentPoint32(m_hDrawDC, s, s.Length, out sz);

            return sz.Width;
        }

        /// <summary>
        /// 選択範囲用のキーが押されたか
        /// （シフト＋上下左右キーやシフト＋ホーム、シフト＋エンドキーなど）
        /// </summary>
        /// <returns>true:押された、false:押されなかった</returns>
        private Boolean IsRangeKey()
        {
            // 矩形選択中の場合は選択範囲用のキーが押されたことにする
            if (m_selectType == RACTANGLE_RANGE_SELECT)
            {
                return true;
            }

            Boolean ret = false;
            byte[] KeyState = new byte[256];
            CEWin32Api.GetKeyboardState(KeyState);

            if ((KeyState[CEWin32Api.VK_SHIFT] & 0x80) != 0) {                          // シフトキーが押され、
                if (((KeyState[CEWin32Api.VK_LEFT] & 0x80) != 0) ||                     //   左キーまたは
                    ((KeyState[CEWin32Api.VK_RIGHT] & 0x80) != 0) ||                    //   右キーまたは
                    ((KeyState[CEWin32Api.VK_UP] & 0x80) != 0) ||                       //   上キーまたは
                    ((KeyState[CEWin32Api.VK_DOWN] & 0x80) != 0) ||                     //   下キーまたは
                    ((KeyState[CEWin32Api.VK_HOME] & 0x80) != 0) ||                     //   ホームキーまたは
                    ((KeyState[CEWin32Api.VK_END] & 0x80) != 0) ||                      //   エンドキーまたは
                    ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left))  //   マウス左クリック
                {                                                                       // が押されたら
                    ret = true;                                                         // 範囲選択中と判断
                }
            }
            else if (m_dragFlg)                                                         // ドラッグ中であれば、
            {
                ret = true;                                                             // 範囲選択中と判断
            }

            return ret;
        }

        /// <summary>
        /// 選択範囲の開始位置取得
        /// シフトキーが押されていれば開始として処理
        /// </summary>
        private void StarRange()
        {
            // 矩形選択
            if (m_selectType == RACTANGLE_RANGE_SELECT)
            {
                if (m_cRng.X == -1 && m_cRng.Y == -1)
                {
                    m_cRng.X = m_caretStrBuf.X;
                    m_cRng.Y = m_caretStrBuf.Y;
                }
                return;
            }

            // Shiftキーが押されているか？
            // 移動元の位置を取得
            if (IsRangeKey())
            {
                if (m_cRng.X == -1 && m_cRng.Y == -1)
                {
                    m_cRng.X = m_caretStrBuf.X;
                    m_cRng.Y = m_caretStrBuf.Y;
                }
                m_selectType = NORMAL_RANGE_SELECT;
            }
            else
            {
                m_cRng.X = -1;
                m_cRng.Y = -1;
                m_selectType = NONE_RANGE_SELECT;
            }
        }

        /// <summary>
        /// 選択範囲の終了位置取得
        /// シフトキーが押されていれば開始として処理
        /// </summary>
        private void EndRange() {
            // Shiftキーが押されているか？
            // 移動元の位置を取得
            if (m_selectType != NONE_RANGE_SELECT)
            {
                m_eRng.X/*m_eRngCol*/ = m_caretStrBuf.X/*m_caretStrBufColumn*/; // 現在のキャレットは選択されていないので-1する
                m_eRng.Y/*m_eRngRow*/ = m_caretStrBuf.Y/*m_caretStrBufRow*/;
                SetRangePos();
            }
        }

        /// <summary>
        /// 選択範囲の文字を削除
        /// (削除の結果キャレットが画面範囲外にくる場合、画面内にくるよう移動する)
        /// </summary>
        private void DeleteRange()
        {
#if false // ★フリーカーソルの場合、範囲外が指定される場合があるためチェック不要
            if (m_selectType != 1)
            {
                // 矩形選択でない場合、選択範囲がテキストの範囲外かチェック
                if ((m_sRng.Y < 0 || GetLineCountP() < m_sRng.Y) ||
                    (m_sRng.X < 0 || GetLineStringP(m_sRng.Y).Length < m_sRng.X))
                {
                    MessageBox.Show("!!!選択範囲削除!!!\r\n選択範囲外(m_sRng.X:" + m_sRng.X + "/m_sRng.Y:" + m_sRng.Y + ")です。");
                    return;
                }
            }
#endif
            if (m_selectType == NORMAL_RANGE_SELECT)
            {
                // 通常選択

                /*****************************************************/
                /* Undo / Redo アンドゥ リドゥ                       */
                /*****************************************************/
                UndoRedoOpe.PreUndoRedo(m_sRng, m_selectType, this);

                // 削除する前に削除文字列を取得（アンドゥ・リドゥバッファ格納のため）★範囲文字列削除と一つに出来ないか？
                string delStr = m_doc.GetRangeString(m_sRng, m_eRng);

                /*****************************************************/
                /* Undo / Redo アンドゥ リドゥ                       */
                /*****************************************************/
                UndoRedoOpe.ProUndoRedo(m_eRng, UndoRedoCode.UR_DELETE, delStr, this);

                // 指定範囲の文字列削除
                m_doc.DelRangeStringP(m_sRng.Y, m_sRng.X, m_eRng.Y, m_eRng.X);
            }
            else if (m_selectType == RACTANGLE_RANGE_SELECT)
            {
                // 矩形選択

                // 指定範囲の文字列削除
                DelRectangleStringP(m_sRng.Y, m_sRng.X, m_eRng.Y, m_eRng.X);
            }

            // キャレット位置変更
            if (m_doc.GetLineCountP() > m_sRng.Y)
            {
                // テキスト範囲内
                m_caretStrBuf.Y = m_sRng.Y;
                m_caretStrBuf.X = m_sRng.X;
            }
            else
            {
                // テキスト範囲外の場合、EOFの場所にキャレットを移動する
                m_caretStrBuf.Y = (m_doc.GetLineCountP() - 1) + m_viewTopRow;
                GetCaretPositionH(m_doc.GetLineStringP(m_caretStrBuf.Y).Length);
            }

            // 範囲選択解除
            RangeCancel();

            // 範囲外キャレット移動
            //DispPosMove();

            // キャレットが横移動している可能性があるので、キャレットの基準位置を更新
            m_curCaretColPosPixel = m_caretPixel.X;
        }

        /// <summary>
        /// 範囲選択の解除
        /// </summary>
        private void RangeCancel()
        {
            // 矩形選択モードの場合、フリーカーソルモードをオフにする
            if (m_selectType == RACTANGLE_RANGE_SELECT)
            {
                // 通常カーソルモード
                m_ShareData.m_cursorMode = 0;

                // 矩形選択の開始位置(ピクセル)
                m_rectCaretColPosPixel = -1;
            }

            // 範囲選択解除
            m_sRng.Y/*m_sRngRow*/ = -1;
            m_sRng.X/*m_sRngCol*/ = -1;
            m_eRng.Y/*m_eRngRow*/ = -1;
            m_eRng.X/*m_eRngCol*/ = -1;
            m_cRng.Y = -1;
            m_cRng.X = -1;
            m_selectType = NONE_RANGE_SELECT;
        }

        /// <summary>
        /// カット
        /// </summary>
        /// <returns></returns>
        public void CutData()
        {
            string copyString = "";

            // 通常選択
            if (m_selectType == NORMAL_RANGE_SELECT)
            {
                // 通常選択範囲を文字列として取得
                copyString = m_doc.GetRangeString(m_sRng, m_eRng);
            }
            // 矩形選択
            else if (m_selectType == RACTANGLE_RANGE_SELECT)
            {
                // 矩形選択範囲を文字列として取得
                copyString = m_doc.GetRectangleString(m_sRng, m_eRng, m_curCaretColPosPixel, m_rectCaretColPosPixel, m_viewLeftColumn, ref m_sRectIdx, ref m_eRectIdx);
            }

            if (copyString != "")
            {
                DataObject data = new DataObject();
                data.SetData(DataFormats.Text, copyString);
                if (m_selectType == RACTANGLE_RANGE_SELECT)
                {
                    // 矩形選択の場合、以下のデータ形式を追加
                    data.SetData("MSDEVColumnSelect", "");
                    data.SetData("CustomTextEditorClipBoard", "");
                }
                Clipboard.SetDataObject(data, true);
            }

            // 選択範囲の文字列を削除
            DeleteRange();

            // スクロールバー再配置
            LayoutScrollBar();

            // 描画
            CEWin32Api.InvalidateRect(this.Handle, IntPtr.Zero, false);
            //this.Refresh();

            return;
        }

        /// <summary>
        /// コピー
        /// </summary>
        public void CopyData()
        {
            string copyString = "";

            // 通常選択
            if (m_selectType == NORMAL_RANGE_SELECT)
            {
                // 選択範囲を文字列として取得
                copyString = m_doc.GetRangeString(m_sRng, m_eRng);
            }
            // 矩形選択
            else if (m_selectType == RACTANGLE_RANGE_SELECT)
            {
                // 選択範囲を文字列として取得
                copyString = m_doc.GetRectangleString(m_sRng, m_eRng, m_curCaretColPosPixel, m_rectCaretColPosPixel, m_viewLeftColumn, ref m_sRectIdx, ref m_eRectIdx);
            }

            if (copyString != "")
            {
                DataObject data = new DataObject();
                data.SetData(DataFormats.Text, copyString);
                if (m_selectType == RACTANGLE_RANGE_SELECT)
                {
                    // 矩形選択の場合、以下のデータ形式を追加
                    data.SetData("MSDEVColumnSelect", "");
                    data.SetData("CustomTextEditorClipBoard", "");
                }
                Clipboard.SetDataObject(data, true);
            }

            // 範囲選択の解除
            RangeCancel();

            // スクロールバー再配置
            LayoutScrollBar();

            // 描画
            CEWin32Api.InvalidateRect(this.Handle, IntPtr.Zero, false);
            //this.Refresh();

            return;
        }

        /// <summary>
        /// 通常範囲選択のペースト
        /// </summary>
        /// <param name="str"></param>
        private void RangePasteData(string str)
        {
            if (m_selectType == NORMAL_RANGE_SELECT) // 通常範囲選択のペースト処理なので通常選択しかチェックしない
            {
                // 選択範囲の文字列を削除
                DeleteRange();
            }

            /*****************************************************/
            /* Undo / Redo アンドゥ リドゥ                       */
            /*****************************************************/
            UndoRedoOpe.PreUndoRedo(m_caretStrBuf, NORMAL_RANGE_SELECT, this);

            // テキスト挿入
            int mx = 0;
            //int my = 0;
            this.MultiLineInsertText(m_caretStrBuf.Y, m_caretStrBuf.X, str, ref mx);
            int bkCursorMod = m_ShareData.m_cursorMode;
            m_ShareData.m_cursorMode = 0;
            GetCaretPositionH(mx);
            m_ShareData.m_cursorMode = bkCursorMod;
            //GetCaretPositionV(my);

            // スクロールバー再配置
            LayoutScrollBar();

            // 再描画
            CEWin32Api.InvalidateRect(this.Handle, IntPtr.Zero, true);
            //this.Refresh();

            /*****************************************************/
            /* Undo / Redo アンドゥ リドゥ                       */
            /*****************************************************/
            UndoRedoOpe.ProUndoRedo(m_caretStrBuf, UndoRedoCode.UR_INSERT, str, this);
        }

        /// <summary>
        /// 矩形選択のペースト
        /// </summary>
        /// <param name="str"></param>
        private void RectanglePasteData(string str)
        {
            if (m_selectType == RACTANGLE_RANGE_SELECT)
            {
                // 矩形選択範囲の文字列を削除
                DeleteRange();
            }

            /*****************************************************/
            /* Undo / Redo アンドゥ リドゥ                       */
            /*****************************************************/
            //UndoRedoOpe.PreUndoRedo(m_caretStrBuf, RACTANGLE_RANGE_SELECT, this);

            // テキスト挿入
            // 通常貼り付けの場合
#if false
            this.MultiLineInsertText(m_caretStrBuf.X, m_caretStrBuf.Y, str);
#else
            // 矩形貼り付け
            int lCol = -1;
            int lRow = -1;
            this.MultiLineRectangleInsertText(m_caretStrBuf.Y, m_caretStrBuf.X, str, out lRow, out lCol);

            // キャレット移動
            CursorKeyV(lRow - m_caretStrBuf.Y);
            CursorKeyH(lCol - m_caretStrBuf.X);
#endif

            // スクロールバー再配置
            LayoutScrollBar();

            // 再描画
            CEWin32Api.InvalidateRect(this.Handle, IntPtr.Zero, true);
            //this.Refresh();

            /*****************************************************/
            /* Undo / Redo アンドゥ リドゥ                       */
            /*****************************************************/
            Point p = new Point(lCol, lRow);
            //UndoRedoOpe.ProUndoRedo(p/*m_caretStrBuf*/, UndoRedoCode.UR_INSERT, str, this);

            // 範囲選択の解除
            //RangeCancel();
        }

        /// <summary>
        /// ペースト
        /// </summary>
        /// <param name="str"></param>
        public void PasteData()
        {
            IDataObject data = Clipboard.GetDataObject();

            string str = (string)data.GetData(DataFormats.StringFormat);
            if (str == null || str == "") return;  // データが無い場合は何もしない

            if (Array.IndexOf(data.GetFormats(), "MSDEVColumnSelect") > 0)
            {
                // 矩形範囲選択
                RectanglePasteData(str);
            }
            else
            {
                // 通常範囲選択
                RangePasteData(str);
            }
        }
#if true
        /// <summary>
        /// ★作成中★
        /// ★折りけし指定での矩形情報貼り付けをどのような仕様にするか要検討★
        /// ★サクラエディター、秀丸、Visual Studioで貼り付け後の表示が全て違うため
        /// (矩形選択用)複数行に跨るテキストデータを挿入
        /// </summary>
        /// <param name="sy">挿入する物理位置（行）</param>
        /// <param name="sx">挿入する物理位置（桁）</param>
        /// <param name="str">挿入するテキスト</param>
        /// <param name="ex">挿入したテキストの右下の物理位置（桁）</param>
        /// <param name="ey">挿入したテキストの右下の物理位置（行）</param>
        private void MultiLineRectangleInsertText(int pRow, int pCol, string str, out int ey, out int ex)
        {
            // 矩形の場合はフリーカーソルモードとして動作させる
            int bkCursorMode = m_ShareData.m_cursorMode;
            m_ShareData.m_cursorMode = 1;

            ex = -1;
            ey = -1;

            /* ------------------------------------------------------------------- */
            /* 貼り付けるテキストを改行（CELF,CR,LF）で分割                        */
            /* （分割文字列に改行コードを付加するため正規表現を使用）              */
            /* ------------------------------------------------------------------- */
            string[] parts = Regex.Split(str, "([^\r\n]*\r\n|[^\r]*\r|[^\n]*\n)");

            /* ------------------------------------------------------------------- */
            /* 分割データの挿入                                                    */
            /* ------------------------------------------------------------------- */
            foreach (string s in parts)
            {
                if (s == "")
                {
                    continue;
                }

                if (isEofLine(pRow))
                {
                    // EOFがある行の場合

                    int lRow, lCol;
                    m_doc.PtoLPos(pRow, pCol, out lRow, out lCol);
                    if (m_doc.GetLineStringP(pRow) != "")
                    {
                        // EOFがある
                        m_doc.AddText(lRow, m_doc.GetLineFeed(s));
                        m_doc.UpdateLogicalLineDataL(lRow);
                    }
                    else
                    {
                        // EOFのみ
                        m_doc.InsertLineL(lRow, m_doc.GetLineFeed(s));
                    }
                }

                // キャレット位置が改行より後ろの場合、キャレット位置までを空白で埋める
                // （フリーカーソル対応）
                // abc↓     |     ⇒ abc_____|↓
                if (m_doc.GetNotLineFeedStringP(pRow).Length < pCol)
                {
                    // 改行コードを除いた文字列数を取得
                    int len = m_doc.GetNotLineFeedString(m_doc.GetLineStringP(pRow)).Length;

                    // 空白を埋める
                    m_doc.InsertLineTextP(pRow, len, String.Format("{" + 0 + ", " + (-pCol + len) + "}", " "));
                }

                // キャレット位置に挿入文字列を追加
                // abc|123↓       ⇒ def↓
                // ------------------------------
                // abc_____|123↓  ⇒ ↓
                m_doc.InsertLineTextP(pRow, pCol, m_doc.GetNotLineFeedString(s));

                // 折返し行更新（改行があるので２行分更新）
                m_doc.UpdateLogicalLineDataP(pRow);

                // 移動先保持
                ey = pRow;
                ex = pCol + m_doc.GetNotLineFeedString(s).Length;

                // 行更新
                pRow++;

                // 列更新
                if (!isEofLine(pRow))
                {
                    // EOFの場合

                    pCol = m_caretStrBuf.X;
                }
                else
                {
                    // EOFでない場合

                    int posX, idx;
                    GetMovePosV(pRow, m_curCaretColPosPixel, out posX, out idx);
                    pCol = idx;
                }
            }

            m_ShareData.m_cursorMode = bkCursorMode;
        }

        /// <summary>
        /// (通常選択用)複数行に跨るテキストデータを挿入
        /// ※物理位置を指定し、論理位置で処理を行い折返し情報更新後、描画を行う
        /// </summary>
        /// <param name="x">挿入する物理位置（桁）</param>
        /// <param name="y">挿入する物理位置（行）</param>
        /// <param name="str">挿入するテキスト</param>
        /// <param name="aMove">挿入後のカーソル移動量</param>
        private void MultiLineInsertText(int pRow, int pCol, string str, ref int aMove)
        {
            /* ------------------------------------------------------------------- */
            /* 貼り付けるテキストを改行（CELF,CR,LF）で分割                        */
            /* （分割文字列に改行コードを付加するため正規表現を使用）              */
            /* ------------------------------------------------------------------- */
            //string[] parts = Regex.Split(str, "([^\r\n|\r|\n]*\r\n|\r|\n)");
            string[] parts = Regex.Split(str, "([^\r\n]*\r\n|[^\r]*\r|[^\n]*\n)");

            /* ------------------------------------------------------------------- */
            /* 物理位置→論理位置                                                  */
            /* ------------------------------------------------------------------- */
            int lRow, lCol;
            m_doc.PtoLPos(pRow, pCol, out lRow, out lCol);

            /* ------------------------------------------------------------------- */
            /* 分割データの挿入                                                    */
            /* ------------------------------------------------------------------- */
            foreach (string s in parts)
            {
                if (s == "")
                {
                    continue;
                }

                /* ------------------------------------------------------------------- */
                /* 改行確認                                                            */
                /* ------------------------------------------------------------------- */
                if (m_doc.GetLineFeed(s) != "")
                {
                    /* ------------------------------------------------------------------- */
                    /* 改行あり                                                            */
                    /* ------------------------------------------------------------------- */

                    // キャレット位置が改行より後ろの場合、キャレット位置までを空白で埋める
                    // （フリーカーソル対応）
                    // abc↓     |     ⇒ abc_____|↓
                    if (m_doc.GetNotLineFeedStringP(pRow).Length < pCol)
                    {
                        // 改行コードを除いた文字列数を取得
                        int len = m_doc.GetNotLineFeedString(m_doc.GetLineStringL(lRow)).Length;

                        // 空白を埋める
                        m_doc.InsertLineTextL(lRow, len, String.Format("{" + 0 + ", " + (-lCol + len) + "}", " "));
#if false // ★必要か不明　特に問題が無ければ削除
                        // 折返し情報更新
                        m_doc.UpdateLogicalLineDataL(lRow);
#endif
                    }

                    // キャレット位置より右の文字列を取得
                    // abc|def↓       ⇒ def↓
                    // ------------------------------
                    // abc_____|↓     ⇒ ↓
                    string rightStr = m_doc.GetRightStringL(lRow, lCol);
                    // キャレット位置より右の文字列を削除
                    // abc|            ⇒ def↓
                    // ------------------------------
                    // abc_____|       ⇒ ↓
                    m_doc.DelRightStringL(lRow, lCol);
                    // キャレット位置に挿入文字列を追加
                    // abc|123↓       ⇒ def↓
                    // ------------------------------
                    // abc_____|123↓  ⇒ ↓
                    m_doc.InsertLineTextL(lRow, lCol, s);
                    // キャレット位置の下に取得し文字列を追加
                    // abc|123↓
                    // def↓
                    // ------------------------------
                    // abc_____|123↓
                    // ↓
                    m_doc.InsertLineL(lRow + 1, rightStr);

                    // 折返し行更新（改行があるので２行分更新）
                    m_doc.UpdateRengeLogicalLineData(lRow, 2);

                    // 挿入後の移動量取得
                    aMove += m_doc.GetNotLineFeedString(s).Length + 1; // 改行を1文字として＋１しておく

                    // 行の更新（次行へ）
                    lRow++;

                    // 列の更新（次行は文字列の先頭へ）
                    lCol = 0;
                }
                else
                {
                    /* ------------------------------------------------------------------- */
                    /* 改行なし（改行が無いということは最終行と言うこと）                  */
                    /* ------------------------------------------------------------------- */

                    // 文字列を挿入
                    m_doc.InsertLineTextL(lRow, lCol, s);

                    // 挿入後の移動量取得
                    aMove += s.Length;

                    // 折返し行更新
                    m_doc.UpdateLogicalLineDataL(lRow);

                    break;
                }
            }
        }
#endif

        /// <summary>
        /// 全て選択
        /// </summary>
        public void AllSelect()
        {
            // 範囲選択解除
            m_sRng.Y = 0;
            m_sRng.X = 0;
            m_eRng.Y = m_doc.GetLineCountP() - 1;
            m_eRng.X = m_doc.GetLineStringP(m_doc.GetLineCountP()-1).Length;
            m_selectType = NORMAL_RANGE_SELECT;

            m_caretPixel.X = 0;
            m_caretPixel.Y = 0;

            m_caretStrBuf.X = 0;
            m_caretStrBuf.Y = 0;

            // スクロールバー再配置
            LayoutScrollBar();

            // 描画
            CEWin32Api.InvalidateRect(this.Handle, IntPtr.Zero, false);
            //this.Refresh();
        }

        /// <summary>
        /// アンドゥ Undo
        /// </summary>
        public void Undo()
        {
            CEUndoRedoData OpeData = UndoRedoOpe.GetUndoOpe();
            if (OpeData == null)
            {
                return;
            }
            if(OpeData.m_ope == UndoRedoCode.UR_INSERT)
            {
                // 操作前位置(論理位置→物理位置)
                int pPreRow, pPreCol;
                m_doc.LtoPPos(OpeData.m_preCaret.Y, OpeData.m_preCaret.X, out pPreRow, out pPreCol);
                // 操作後位置(論理位置→物理位置)
                int pAftRow, pAftCol;
                m_doc.LtoPPos(OpeData.m_aftCaret.Y, OpeData.m_aftCaret.X, out pAftRow, out pAftCol);
                m_caretStrBuf.X = pPreCol;
                m_caretStrBuf.Y = pPreRow;

                if (OpeData.m_selectType == RACTANGLE_RANGE_SELECT)
                {
                    // 矩形選択部分の削除
                    //DelRectangleStringP(OpeData.m_preCaret.Y, OpeData.m_preCaret.X, OpeData.m_aftCaret.Y, OpeData.m_aftCaret.X);
                    int bkRectCaretColPosPixel = m_rectCaretColPosPixel;
                    int bkCurCaretColPosPixel = m_curCaretColPosPixel;
                    m_rectCaretColPosPixel = pPreCol * (m_ShareData.m_charWidthPixel + m_ShareData.m_nColumnSpace);
                    m_curCaretColPosPixel = pAftCol * (m_ShareData.m_charWidthPixel + m_ShareData.m_nColumnSpace);
                    DelRectangleStringP(pPreRow, pPreCol, pAftRow, pAftCol);
                    m_rectCaretColPosPixel = bkRectCaretColPosPixel;
                    m_curCaretColPosPixel = bkCurCaretColPosPixel;
                }
                else
                {
                    // その他
                    m_doc.DelRangeStringL(OpeData.m_preCaret.Y, OpeData.m_preCaret.X, OpeData.m_aftCaret.Y, OpeData.m_aftCaret.X);
                }
                //m_caretStrBuf.X = OpeData.m_preCaret.X;
                //m_caretStrBuf.Y = OpeData.m_preCaret.Y;
                // 範囲外キャレット移動
                DispPosMove();
                CEWin32Api.InvalidateRect(this.Handle, IntPtr.Zero, true);
                //this.Refresh();
                ShowCaret();
            }
            else if (OpeData.m_ope == UndoRedoCode.UR_DELETE)
            {
                // 保持データが空の場合は挿入処理は行わない
                if (OpeData.m_pcmemData != "")
                {
                    int pRow, pCol;
                    m_doc.LtoPPos(OpeData.m_preCaret.Y, OpeData.m_preCaret.X, out pRow, out pCol);   // 論理位置→物理位置
                    m_caretStrBuf.X = pCol;
                    m_caretStrBuf.Y = pRow;
                    int mx = 0;
                    //int my = 0;
                    this.MultiLineInsertText(m_caretStrBuf.Y, m_caretStrBuf.X, OpeData.m_pcmemData, ref mx);
                    GetCaretPositionH(mx);
                    //GetCaretPositionV(my);
                }
                // 範囲外キャレット移動
                DispPosMove();
                CEWin32Api.InvalidateRect(this.Handle, IntPtr.Zero, true);
                //this.Refresh();
                ShowCaret();
            }
        }

        /// <summary>
        /// リドゥ Redo
        /// </summary>
        public void Redo()
        {
            CEUndoRedoData OpeData = UndoRedoOpe.GetRedoOpe();
            if (OpeData == null)
            {
                return;
            }
            if (OpeData.m_ope == UndoRedoCode.UR_INSERT)
            {
#if false // 論理位置で保存されているので物理位置に変換してから使用
                m_caretStrBuf.X = OpeData.m_preCaret.X;
                m_caretStrBuf.Y = OpeData.m_preCaret.Y;
#else
                int pRow, pCol;
                m_doc.LtoPPos(OpeData.m_preCaret.Y, OpeData.m_preCaret.X, out pRow, out pCol);   // 論理位置→物理位置
                m_caretStrBuf.X = pCol;
                m_caretStrBuf.Y = pRow;
#endif
                if (OpeData.m_selectType == RACTANGLE_RANGE_SELECT)
                {
                    // 矩形選択部分の削除
                    int eX, eY;
                    this.MultiLineRectangleInsertText(m_caretStrBuf.Y, m_caretStrBuf.X, OpeData.m_pcmemData, out eY, out eX);
                }
                else
                {
                    // その他
                    int mx = 0;
                    //int my = 0;
                    MultiLineInsertText(m_caretStrBuf.Y, m_caretStrBuf.X, OpeData.m_pcmemData, ref mx);
                    GetCaretPositionH(mx);
                    //GetCaretPositionV(my);

                }
                // 範囲外キャレット移動
                DispPosMove();
                CEWin32Api.InvalidateRect(this.Handle, IntPtr.Zero, true);
                //this.Refresh();
            }
            else if (OpeData.m_ope == UndoRedoCode.UR_DELETE)
            {
                m_doc.DelRangeStringL(OpeData.m_preCaret.Y, OpeData.m_preCaret.X, OpeData.m_aftCaret.Y, OpeData.m_aftCaret.X);
                m_caretStrBuf.X = OpeData.m_preCaret.X;
                m_caretStrBuf.Y = OpeData.m_preCaret.Y;
                // 範囲外キャレット移動
                DispPosMove();
                CEWin32Api.InvalidateRect(this.Handle, IntPtr.Zero, true);
                //this.Refresh();
            }
        }

        string m_findString = "";
        Boolean m_ul = false;
        Boolean m_word = false;
        Boolean m_reg = false;

        /// <summary>
        /// 次検索
        /// </summary>
        /// <param name="fStr">検索文字列</param>
        /// <param name="ul">大文字小文字区分(true:する/false:しない)</param>
        public void NextFind(string fStr, Boolean ul, Boolean word, Boolean reg)
        {
            // 検索情報を保存
            m_findString = fStr;
            m_ul = ul;
            m_word = word;
            m_reg = reg;

            // 検索開始位置
            int findStartCol = -1;
            int findStartRow = -1;

            // 検索対象のテキスト行数
            int textLine = m_doc.GetLineCountP();

            string str = "";
            int findPoint = -1;

            if (m_selectType == NONE_RANGE_SELECT)
            {
                // 範囲選択中でない場合
                findStartRow = m_caretStrBuf.Y;
                findStartCol = m_caretStrBuf.X;
            }
            else
            {
                // 通常または矩形範囲選択中の場合
                findStartRow = m_eRng.Y;
                findStartCol = m_eRng.X;
            }
#if false
            CEFind cf = new CEFind(m_doc);
            cf.NextFind(fStr, findY, findX, ul, word, reg, ref m_sRng, ref m_eRng);
#endif
            for (int loopIdx = 0; loopIdx < textLine; loopIdx++)
            {
                // 一番下まで検索したら上から再検索
                int row = loopIdx + findStartRow;
                if (row >= textLine)
                {
                    row -= textLine;
                    //break; // 上から再検索したくない場合は有効にする
                }

                // 検索対象文字列取得
                str = m_doc.GetLineStringP(row);

                //改行のみまたは検索文字が無い場合は次の行へ
                if (str == CEConstants.LineFeed || str == "")
                {
                    continue;
                }

                // 正規表現検索の場合
                if (ul || word || reg)
                {
                    string pattern = fStr;

                    // 大文字・小文字区別する
                    if (ul && !reg)
                    {
                        pattern = "(?-i)" + pattern;
                        //findPoint = str.IndexOf(fStr, findX);
                    }
                    // 大文字・小文字区別しない
                    else if (!ul && !reg)
                    {
                        pattern = "(?i)" + pattern;
                        //findPoint = str.IndexOf(fStr, findX, StringComparison.OrdinalIgnoreCase);
                    }

                    // 単語単位
                    if (word && !reg)
                    {
                        pattern = "\\b" + pattern + "\\b";
                    }

                    // 検索
                    try
                    {
                        //Debug.WriteLine("pattern : " + pattern);
                        var r = new Regex(pattern, RegexOptions.IgnoreCase);
                        string sss = str.Substring(findStartCol);
                        if (r.Match(sss).Success)
                        {
                            findPoint = r.Match(sss).Index + findStartCol;
                        }
                    }
                    catch(ArgumentException a)
                    {
                        findPoint = -1;
                    }
                }
                // 正規表現検索でない場合
                else
                {
                    findPoint = str.IndexOf(fStr, findStartCol, StringComparison.OrdinalIgnoreCase);
                }

                // 見つからない場合は次の行へ
                if (findPoint == -1)
                {
                    findStartCol = 0;
                    continue;
                }
                // 見つかった場合はキャレット移動し再描画
                else
                {
                    m_sRng.Y = row;
                    m_sRng.X = findPoint;
                    m_eRng.Y = row;
                    m_eRng.X = findPoint + fStr.Length;

                    m_caretStrBuf.Y = m_sRng.Y;
                    m_caretStrBuf.X = m_sRng.X;

                    m_selectType = NORMAL_RANGE_SELECT;
                    m_searchType = SEARCH_TYPE;
                    break;
                }
                
            } //for

            CaretPosMove();
            CEWin32Api.InvalidateRect(this.Handle, IntPtr.Zero, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="findStringLength"></param>
        /// <returns></returns>
        private int NextLineFind(int row, out int findStringLength)
        {
            int findX = 0;
            int findPoint = -1;

            findStringLength = -1;

            // 検索文字列が無い場合は検索結果なしとする
            if (m_findString == "") return findPoint;

            // 検索対象文字列取得
            string str = m_doc.GetLineStringP(row);

            //改行のみまたは検索文字が無い場合は次の行へ
            if (str == CEConstants.LineFeed || str == "")
            {
                return findPoint;
            }

            // 正規表現検索の場合
            if (m_ul || m_word || m_reg)
            {
                string pattern = m_findString;

                // 大文字・小文字区別する
                if (m_ul && !m_reg)
                {
                    pattern = "(?-i)" + pattern;
                    //findPoint = str.IndexOf(fStr, findX);
                }
                // 大文字・小文字区別しない
                else if (!m_ul && !m_reg)
                {
                    pattern = "(?i)" + pattern;
                    //findPoint = str.IndexOf(fStr, findX, StringComparison.OrdinalIgnoreCase);
                }

                // 単語単位
                if (m_word && !m_reg)
                {
                    pattern = "\\b" + pattern + "\\b";
                }

                // 検索
                try
                {
                    var r = new Regex(pattern, RegexOptions.IgnoreCase);
                    string sss = str.Substring(findX);
                    if (r.Match(sss).Success)
                    {
                        findPoint = r.Match(sss).Index + findX;
                    }
                }
                catch (ArgumentException a)
                {
                    findPoint = -1;
                }
            }
            // 正規表現検索でない場合
            else
            {
                findPoint = str.IndexOf(m_findString, findX, StringComparison.OrdinalIgnoreCase);
            }

            if (findPoint != -1)
            {
                //findStringLength = findPoint + m_findString.Length;
                findStringLength = m_findString.Length;
            }

            return findPoint;
        }

        /// <summary>
        /// 前検索
        /// </summary>
        /// <param name="fStr">検索文字列</param>
        /// <param name="ul">大文字小文字区分(true:する/false:しない)</param>
        public void PrevFind(string fStr, Boolean ul, Boolean word, Boolean reg)
        {
            m_findString = fStr;
            m_ul = ul;
            m_word = word;
            m_reg = reg;

            // 検索開始位置
            int findX = 0;
            int findY = 0;

            // 検索対象のテキスト行数
            int textLine = m_doc.GetLineCountP();

            string str = "";
            int findPoint = -1;

            if (m_selectType == NONE_RANGE_SELECT)
            {
                // 範囲選択中でない場合
                findY = m_caretStrBuf.Y;
                findX = m_caretStrBuf.X;
            }
            else
            {
                // 通常または矩形範囲選択中の場合
                findY = m_sRng.Y;
                findX = m_sRng.X;
            }

            for (int loopIdx = 0; loopIdx < textLine; loopIdx++)
            {
                // 一番上まで検索したら下から再検索
                int row = findY - loopIdx;
                if (row < 0)
                {
                    row += textLine;
                    //break; // 下からから再検索したくない場合は有効にする
                }

                str = m_doc.GetLineStringP(row);

                //改行のみまたは検索文字が無い場合は次の行へ
                if (str == CEConstants.LineFeed || str == "")
                {
                    if (row > 0)
                    {
                        findX = m_doc.GetLineStringP(row - 1).Length;
                    }
                    continue;
                }

                if (ul || word || reg)
                {
                    string pattern = fStr;

                    // 大文字・小文字区別する
                    if (ul && !reg)
                    {
                        pattern = "(?-i)" + pattern;
                        //findPoint = str.LastIndexOf(fStr, findX);
                    }
                    // 大文字・小文字区別しない
                    else if (!ul && !reg)
                    {
                        pattern = "(?i)" + pattern;
                        //findPoint = str.LastIndexOf(fStr, findX, StringComparison.OrdinalIgnoreCase);
                    }

                    // 単語単位
                    if (word && !reg)
                    {
                        pattern = "\\b" + pattern + "\\b";
                    }

                    // 検索
                    try
                    {
                        string sss = str.Substring(0, findX);
                        //var r = new Regex(pattern, RegexOptions.IgnoreCase);
                        foreach (Match match in Regex.Matches(sss, pattern, RegexOptions.RightToLeft | RegexOptions.IgnoreCase))
                        {
                            if (match.Success)
                            {
                                findPoint = match.Index;
                                break;
                            }
                        }
                    }
                    catch(ArgumentException a)
                    {
                        findPoint = -1;
                    }
                }
                else
                {
                    findPoint = str.LastIndexOf(fStr, findX, StringComparison.OrdinalIgnoreCase);
                }

                // 見つからない場合は前の行へ
                if (findPoint == -1)
                {
                    if(row > 0 ) {
                        findX = m_doc.GetLineStringP(row-1).Length;
                    }
                    continue;
                }
                // 見つかった場合はキャレット移動し再描画
                else
                {
                    m_sRng.Y = row;
                    m_sRng.X = findPoint;
                    m_eRng.Y = row;
                    m_eRng.X = findPoint + fStr.Length;

                    m_caretStrBuf.Y = m_sRng.Y;
                    m_caretStrBuf.X = m_eRng.X;

                    m_selectType = NORMAL_RANGE_SELECT;
                    m_searchType = SEARCH_TYPE;
                    break;
                }

            } // for

            CaretPosMove();
            CEWin32Api.InvalidateRect(this.Handle, IntPtr.Zero, true);
        }

        /// <summary>
        /// 指定した現在の物理位置から指定した物理列移動したらときの物理位置を返す
        /// </summary>
        /// <remarks>
        /// この"うんこ"みたいな処理は精査する必要がある
        /// もっと単純にできるはず
        /// </remarks>
        /// <param name="row">[入力]現在位置(行)</param>
        /// <param name="col">[入力]現在位置(列)</param>
        /// <param name="nPos">[入力]移動量</param>
        /// <param name="oRow">[出力]移動位置(行)</param>
        /// <param name="oCol">[出力]移動位置(列)</param>
        private void GetPositionH(int row, int col, int nPos, out int oRow, out int oCol)
        {
            // 移動量が0の場合なにもしない
            if (nPos == 0)
            {
                oRow = row;
                oCol = col;
                return;
            }

            oRow = -1;
            oCol = -1;
            int tmpCol = -1;

            // 現在行の情報取得
            int clLine;
            int cpLine;
            Boolean cRet = m_doc.getPosition(row, out clLine, out cpLine);

            // 範囲外が指定された場合、移動量0とする
            if (clLine == -1 || cpLine == -1)
            {
                oRow = row;
                oCol = col;
                return;
            }

            if (nPos > 0)
            {
                ////////////////////////////////////////////////
                // プラス側に移動
                ////////////////////////////////////////////////

                tmpCol = col;

                int mLine = 0; // 現在行から移動した行(この値を現在行に加算することで移動後の行がわかる)
                for (int ii = clLine; ii < m_doc.GetLineCountL(); ii++)
                {
                    for (int jj = cpLine; jj < m_doc.GetPhysicalLineCount(ii); jj++)
                    {
                        int ofs = m_doc.GetPhysicalLineData(ii, jj).m_offset;
                        int lfSz = m_doc.GetPhysicalLineData(ii, jj).m_lfStr.Length;
                        int tlfSz = 0;
                        if (lfSz != 0)
                        {
                            tlfSz = 1; // 改行は1文字として計算
                        }
                        int len = m_doc.GetPhysicalLineData(ii, jj).m_length - lfSz + tlfSz;
                        Boolean wrap = m_doc.GetPhysicalLineData(ii, jj).m_wrap;

                        // lenが0と言うことはEOFのみの行
                        if (len <= 0)
                        {
                            oCol = 0;
                            oRow = row + mLine;
                            return;
                        }
                        // ちょうどEOFの位置
                        /*else*/ if (!wrap && lfSz == 0 && (tmpCol + nPos == len))
                        {
                            oCol = tmpCol + nPos;
                            oRow = row + mLine;
                            return;
                        }
                        // EOFの位置を超える
                        /*else*/ if (!wrap && lfSz == 0 && (tmpCol + nPos > len))
                        {
                            oCol = col;
                            oRow = row;
                            return;
                        }
#if true
                        // >>> フリーカーソル対応 2017/02/01 >>>
                        // 折り返しでないくフリーカーソルの場合、改行以降にカーソルを進めるようにする
                        if (!wrap && m_ShareData.m_cursorMode == 1)
                        {
                            string nowStr = m_doc.GetNotLineFeedString(m_doc.GetLineStringP(row + mLine));
                            int width = m_doc.GetTextPixel(m_ShareData.m_nColumnSpace, nowStr);
                            len += (m_ShareData.m_wrapPositionPixel - width) / m_ShareData.m_charWidthPixel - 1;
                        }
                        // <<< フリーカーソル対応 2017/02/01 <<<
#endif
                        // 現在行で移動できるので移動して終わり
                        /*else*/ if (tmpCol + nPos < len)
                        {
                            oCol = tmpCol + nPos;
                            oRow = row + mLine;
                            return;
                        }
                        tmpCol = tmpCol - len;
                        mLine++;
                    } // for
                    cpLine = 0;
                } // for

                oCol = tmpCol;
                oRow = row + mLine;
            }
            else if (nPos < 0)
            {
                ////////////////////////////////////////////////
                // マイナス側に移動
                ////////////////////////////////////////////////

                // 一番左上の場合は何もしない
                if (row == 0 && col == 0)
                {
                    oRow = row;
                    oCol = col;
                    return;
                }

#if false
                //nPos = System.Math.Abs(nPos); // 移動量(絶対値)取得
#else
                nPos = -nPos; // 移動量に変換
#endif
                tmpCol = col;

                int mLine = 0; // 行移動量(この値を現在行に加算することで移動後の行がわかる)
                for (int lRowIdx = clLine; lRowIdx >= 0; lRowIdx--)
                {
                    for (int lpColIdx = cpLine; lpColIdx >= 0; lpColIdx--)
                    {
                        // 改行を1文字とした文字列の長さを取得
                        // 12345\r\n ←7文字だが6文字とする
                        int lfSz = m_doc.GetPhysicalLineData(lRowIdx, lpColIdx).m_lfStr.Length;
                        int tlfSz = 0;
                        if (lfSz != 0)
                        {
                            tlfSz = 1;
                        }
                        int len = m_doc.GetPhysicalLineData(lRowIdx, lpColIdx).m_length - lfSz + tlfSz;
#if false
                        // >>> フリーカーソル対応 2017/02/01 >>>
                        // 現在の物理行の折り返し状態取得
                        Boolean wrap = m_doc.GetPhysicalLineData(lRowIdx, lpColIdx).m_wrap;

                        // 折り返しでなくフリーカーソルの場合、改行以降にカーソルを進めるようにする
                        if (!wrap && m_cursorMode == 1)
                        {
                            string nowStr = m_doc.GetNotLineFeedString(m_doc.GetLineStringP(row + mLine));
                            int width = m_doc.GetTextPixel(m_nColumnSpace, nowStr);
                            len += (m_wrapPositionPixel - width) / m_charWidthPixel - 1;
                        }
                        // <<< フリーカーソル対応 2017/02/01 <<<
#endif
                        tmpCol = tmpCol < 0 ? len : tmpCol;

                        if (nPos <= tmpCol)
                        {
                            oCol = tmpCol - nPos;
                            oRow = row - mLine;
                            return;
                        }
                        nPos = nPos - tmpCol;
                        tmpCol = -1;
                        mLine++;
                    }
                    // 一行上の一番下の物理行からチェック開始
                    cpLine = m_doc.GetPhysicalLineCount(lRowIdx - 1) - 1;
                }
                oRow = row - mLine;
            } // if - else
            else
            {
                Debug.Write("移動量ゼロ");
            }

            return;
        }

        /// <summary>
        /// 今いる位置がEOFか？
        /// </summary>
        /// <param name="p">物理位置</param>
        /// <returns>true:EOFである / false:EOFでない</returns>
        private Boolean isEofPos(int pRow, int pCol)
        {
            int oRow = 0;
            int oCol = 0;
            GetPositionH(pRow, pCol, 1, out oRow, out oCol);
            if (pRow == oRow && pCol == oCol)
            {
                // EOFである
                // 右に移動できないのであればEOF位置ということ
                return true;
            }

            // EOFではない
            return false;
        }

        /// <summary>
        /// 指定した物理行がEOFかチェック
        /// </summary>
        /// <param name="pRow"></param>
        /// <returns></returns>
        private Boolean isEofLine(int pRow)
        {
            int lRow, lCol;
            m_doc.PtoLPos(pRow, 0, out lRow, out lCol);

            return isEofLine(lRow, 0);
        }

        /// <summary>
        /// 指定した行(物理)にEOFがあるかチェック
        /// </summary>
        /// <param name="lLine">論理行</param>
        /// <param name="pLine">論理行内の物理行</param>
        /// <returns>true:EOFあり / false:EOFなし</returns>
        private Boolean isEofLine(int lLine, int pLine)
        {
            string lfstr = m_doc.GetPhysicalLineData(lLine, pLine)/*m_textList[lLine].m_physicalLine[pLine]*/.m_lfStr;
            bool wrap = m_doc.GetPhysicalLineData(lLine, pLine)/*m_textList[lLine].m_physicalLine[pLine]*/.m_wrap;
            if (lfstr == "" && !wrap)
            {
                return true;
            }
            return false;
        }
#if false // ★未使用　しばらくしてから削除予定（3/3）
        /// <summary>
        /// 現在位置の補正
        /// </summary>
        /// <param name="p"></param>
        private void GetAdjustCursorPos(ref Point p)
        {
            p.Y = -1;
            p.X = -1;

            // 現在行の情報取得
            int clLine;
            int cpLine;
            //Boolean cRet = getPosition(p.Y, out clLine, out cpLine);

            for (int ii = 0; ii < m_doc.GetLineCountL()/*m_textList.Count*/; ii++)
            {
                p.Y += m_doc.GetPhysicalLineCount(ii)/*m_textList[ii].m_sldLine*/;
            }
            m_doc.getPosition(p.Y, out clLine, out cpLine);
            int len = m_doc.GetPhysicalLineData(clLine, cpLine)/*m_textList[clLine].m_physicalLine[cpLine]*/.m_length;
            int lf = m_doc.GetPhysicalLineData(clLine, cpLine)/*m_textList[clLine].m_physicalLine[cpLine]*/.m_lfStr.Length;
            p.X = len - lf;
        }
#endif
        /// <summary>
        /// 指定した現在の物理位置から指定した物理行移動したらときの物理位置を返す
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private Boolean GetPositionV(int row, int col, int nPos, out int oRow, out int oCol)
        {
            oRow = -1;
            oCol = -1;

            return true;
        }

        /// <summary>
        /// 指定された物理位置のテキストを返却し、次行へ値を進める
        /// </summary>
        /// <param name="lLine">論理位置</param>
        /// <param name="pLine">物理位置</param>
        /// <returns></returns>
        private string getNextPositionString(ref int lLine, ref int pLine, out Boolean wrap)
        {
            string str = "";

            if (lLine < 0 || pLine < 0)
            {
                wrap = false;
                return str;
            }

            if (m_doc.GetPhysicalLineCount(lLine)/*m_textList[lLine].m_physicalLine.Count*/ <= pLine)
            {
                lLine++;
                pLine = 0;
            }
            //str = m_textList[lLine].m_text.Substring(m_textList[lLine].m_physicalLine[pLine].m_offset, m_textList[lLine].m_physicalLine[pLine].m_length);
            //wrap = m_textList[lLine].m_physicalLine[pLine].m_wrap;
            SPhysicalLine spd = m_doc.GetPhysicalLineData(lLine, pLine);
            str = m_doc.GetLineStringL(lLine).Substring(spd.m_offset, spd.m_length);
            wrap = spd.m_wrap;

            return str;
        }

        /// <summary>
        /// [矩形範囲]指定(物理位置)した範囲の文字列を削除する
        /// </summary>
        /// <param name="sRow">削除開始行(物理)</param>
        /// <param name="sCol">削除開始列(物理)</param>
        /// <param name="eRow">削除終了行(物理)</param>
        /// <param name="eCol">削除終了列(物理)</param>
        private void DelRectangleStringP(int sRowP, int sColP, int eRowP, int eColP)
        {
            // フリーカーソルモードに設定
            m_ShareData.m_cursorMode = 1;

            // 削除行数取得
            int procNum = eRowP - sRowP + 1;

            // 上から消すと削除行以降のインデックスが狂うので下から消していく。
            // 折返しが無かったら普通に上からで良いのだが。
            string objStr = "";
            for (int row = eRowP; row > sRowP - 1; row--)
            {
                int lLine;
                int pLine;
                m_doc.getPosition(row, out lLine, out pLine);

                // 削除対象行の一行分の文字列を取得
                objStr = m_doc.GetNotLineFeedString(m_doc.GetLineStringP(row, lLine, pLine));

                // 削除範囲の列インデックスを取得し、gsIdxとgeIdxに格納
                m_doc.GetRectMovePosV(row, m_rectCaretColPosPixel, m_curCaretColPosPixel, objStr, m_viewLeftColumn, out m_sRectIdx, out m_eRectIdx);

                // 削除開始・終了位置をPointクラスに設定
                Point sRng = new Point(m_sRectIdx, row);
                Point eRng = new Point(m_eRectIdx, row);

                /*****************************************************/
                /* Undo / Redo アンドゥ リドゥ                       */
                /*****************************************************/
                // 削除する前に削除文字列を取得
                string delStr = m_doc.GetRangeString(sRng, eRng);
                if (delStr == "")
                {
                    // もし削除する文字列が無い場合は削除処理を行わない
                    continue;
                }
                UndoRedoOpe.PreUndoRedo(sRng, m_selectType, this);

                /*****************************************************/
                /* Undo / Redo アンドゥ リドゥ                       */
                /*****************************************************/
                UndoRedoOpe.ProUndoRedo(eRng, UndoRedoCode.UR_DELETE, delStr, this);

                // 指定範囲の文字列を削除
                m_doc.DelRangeStringP(sRng.Y, sRng.X, eRng.Y, eRng.X);
            }

            // 通常カーソルモードに戻す
            m_ShareData.m_cursorMode = 0;
        }

        /// <summary>
        /// 全行の折り返し情報更新
        /// </summary>
        private void updateAllLogicalLine()
        {
            for (int idx = 0; idx < m_doc.GetLineCountL(); idx++)
            {
                // 折り返し情報更新
                m_doc.UpdateLogicalLineDataL(idx);
            }
        }

        /// <summary>
        /// 指定した行(物理)に折返しがあるかチェック
        /// </summary>
        /// <param name="row">折返しチェック行(物理)</param>
        /// <returns>true:折返しあり / false:折返しなし</returns>
        private Boolean isWrapPos(int row)
        {
            int lLine;
            int pLine;
            m_doc.getPosition(row, out lLine, out pLine);

            return isWrapPos(row, lLine, pLine);
        }
        private Boolean isWrapPos(int row, int lLine, int pLine)
        {
            string lfstr = m_doc.GetPhysicalLineData(lLine, pLine)/*m_textList[lLine].m_physicalLine[pLine]*/.m_lfStr;
            bool wrap = m_doc.GetPhysicalLineData(lLine, pLine)/*m_textList[lLine].m_physicalLine[pLine]*/.m_wrap;
            if (lfstr == "" && wrap)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// (物理)キャレット範囲チェック
        /// </summary>
        /// <param name="pRow">物理行</param>
        /// <param name="pCol">物理列</param>
        /// <returns>true:範囲内／false:範囲外</returns>
        private Boolean IsCaretRangeP(int pRow, int pCol)
        {
            if (m_doc.GetNotLineFeedStringP(pRow).Length < pCol)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 物理行の先頭かチェックし論理行を返す
        /// </summary>
        /// <param name="row"></param>
        /// <returns>物理行の先頭であれば論理行を返す</returns>
        private int CheckLeanLine(int row)
        {
            int lLine;
            int pLine;
            Boolean f = m_doc.getPosition(row, out lLine, out pLine);

            if (pLine == 0 && f)
            {
                return lLine;
            }
            return -1;
        }
    }
}