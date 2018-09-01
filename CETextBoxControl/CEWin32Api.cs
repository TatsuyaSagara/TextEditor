//
// Win32APIのDLLインポート
// （こんなことするならC#じゃなくてC++でいいんじゃね？）
//

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Debug = System.Diagnostics.Debug;

namespace CETextBoxControl
{
    public static class CEWin32Api
    {
        public const int SIF_RANGE = 0x0001;
        public const int SIF_PAGE = 0x0002;
        public const int SIF_POS = 0x0004;
        public const int SIF_DISABLENOSCROLL = 0x0008;
        public const int SIF_TRACKPOS = 0x0010;
        public const int SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS);

        public const int SB_HORZ = 0;
        public const int SB_VERT = 1;
        public const int SB_CTL = 2;
        public const int SB_BOTH = 3;

        public const int ESB_ENABLE_BOTH = 0;
        public const int ESB_DISABLE_BOTH = 3;
        public const int ESB_DISABLE_LEFT = 1;
        public const int ESB_DISABLE_RIGHT = 2;
        public const int ESB_DISABLE_UP = 1;
        public const int ESB_DISABLE_DOWN = 2;
        public const int ESB_DISABLE_LTUP = 1;
        public const int ESB_DISABLE_RTDN = 2;

        public const int WS_HSCROLL = 0x00100000;

        public const int GWL_STYLE = -16;

        public const int WM_CREATE = 0x0001;
        public const int WM_DESTROY = 0x0002;
        public const int WM_MOVE = 0x0003;
        public const int WM_SIZE = 0x0005;
        public const int WM_ACTIVATE = 0x0006;
        public const int WM_SETFOCUS = 0x0007;
        public const int WM_KILLFOCUS = 0x0008;

        public const int WM_PAINT = 0x000F;
        public const int WM_CLOSE = 0x0010;
        public const int WM_CHAR = 0x0102;
        public const int WM_ERASEBKGND = 0x0014;

        public const int WM_SHOWWINDOW = 0x0018;

        public const int WM_SETCURSOR = 0x0020;

        public const int WM_GETFONT = 0x0031;

        public const int WM_NCCALCSIZE = 0x0083;

        public const int WM_GETDLGCODE = 0x0087;

        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;
        public const int WM_SYSKEYDOWN = 0x0104;
        public const int WM_SYSKEYUP = 0x0105;

        //public const int WM_IME_STARTCOMPOSITION = 0x010D;
        //public const int WM_IME_ENDCOMPOSITION = 0x010E;
        //public const int WM_IME_COMPOSITION = 0x010F;
        //public const int WM_IME_KEYLAST = 0x010F;

        public const int WM_INITDIALOG = 0x0110;
        public const int WM_COMMAND = 0x0111;
        public const int WM_SYSCOMMAND = 0x0112;
        public const int WM_TIMER = 0x0113;
        public const int WM_HSCROLL = 0x0114;
        public const int WM_VSCROLL = 0x0115;
        public const int WM_INITMENU = 0x0116;
        public const int WM_INITMENUPOPUP = 0x0117;

        public const int WM_MOUSEWHEEL = 0x020A;
        public const int WM_MOUSEHOVER = 0x02A1;
        public const int WM_MOUSELEAVE = 0x02A3;

        public const int WM_IME_CHAR = 0x286;    // IMEが取得した文字を受信
        public const int WM_IME_COMPOSITION = 0x10F;    // 変換が実行されたことを受信する(フラグはGCS_xxxxx)
        public const int WM_IME_COMPOSITIONFULL = 0x284;    // コンポジションウィンドウの領域が以上広がらないときに受信する
        public const int WM_IME_CONTROL = 0x283;    // アプリケーションが生成したIMEウィンドウに処理要求を行う (コマンドはIMC_xxxxx)
        public const int WM_IME_ENDCOMPOSITION = 0x10E;    // 変換が終了したことを受信する
        public const int WM_IME_KEYDOWN = 0x290;    // 変換が終了したことを受信する
        public const int WM_IME_KEYUP = 0x291;    // 変換が終了したことを受信する
        public const int WM_IME_NOTIFY = 0x282;    // アプリケーションが生成したIMEウィンドウに処理要求を行う (コマンドはIMN_xxxxx)
        public const int WM_IME_SELECT = 0x285;    // カレントのIMEが変更されたことを受信する
        public const int WM_IME_SETCONTEXT = 0x281;    // アプリケーションが生成したIMEウィンドウに処理要求を行う (コマンドはISC_xxxxx)
        public const int WM_IME_STARTCOMPOSITION = 0x10D;    // 変換が開始されたことを受信する

        public const int WM_MOUSEMOVE = 0x0200;     // マウスドラッグ
        public const int WM_LBUTTONDOWN = 0x0201;   // マウス左ボタンを押し下げ
        public const int WM_LBUTTONUP = 0x0202;   // マウス左ボタンを離した
        public const int WM_LBUTTONDBLCLK = 0x0203;   // マウス左ボタンをダブルクリック
        public const int WM_MBUTTONDBLCLK = 0x0209;   // マウス中央ボタンをダブルクリック
        public const int WM_MBUTTONDOWN = 0x0207;   // マウス中央ボタンを押し下げ
        public const int WM_MBUTTONUP = 0x0208;   // マウス中央ボタンを離した
        public const int WM_RBUTTONDBLCLK = 0x0206;   // マウス右ボタンをダブルクリック
        public const int WM_RBUTTONDOWN = 0x0204;   // マウス右ボタンを押し下げ
        public const int WM_RBUTTONUP = 0x0205;   // マウス右ボタンを離した

        public const int MK_LBUTTON = 0x0001;
        public const int MK_RBUTTON = 0x0002;
        public const int MK_SHIFT = 0x0004;
        public const int MK_CONTROL = 0x0008;
        public const int MK_MBUTTON = 0x0010;

        public const int SB_LINEUP = 0;
        public const int SB_LINELEFT = 0;
        public const int SB_LINEDOWN = 1;
        public const int SB_LINERIGHT = 1;
        public const int SB_PAGEUP = 2;
        public const int SB_PAGELEFT = 2;
        public const int SB_PAGEDOWN = 3;
        public const int SB_PAGERIGHT = 3;
        public const int SB_THUMBPOSITION = 4;
        public const int SB_THUMBTRACK = 5;
        public const int SB_TOP = 6;
        public const int SB_LEFT = 6;
        public const int SB_BOTTOM = 7;
        public const int SB_RIGHT = 7;
        public const int SB_ENDSCROLL = 8;

        public const uint SW_SCROLLCHILDREN = 0x0001;
        public const uint SW_INVALIDATE = 0x0002;
        public const uint SW_ERASE = 0x0004;
        public const uint SW_SMOOTHSCROLL = 0x0010;

        [Flags]
        public enum ETOOptions : uint
        {
            ETO_CLIPPED = 0x4,
            ETO_GLYPH_INDEX = 0x10,
            ETO_IGNORELANGUAGE = 0x1000,
            ETO_NUMERICSLATIN = 0x800,
            ETO_NUMERICSLOCAL = 0x400,
            ETO_OPAQUE = 0x2,
            ETO_PDY = 0x2000,
            ETO_RTLREADING = 0x800,
        }

        public const int DLGC_WANTARROWS = 0x0001;
        public const int DLGC_WANTTAB = 0x0002;
        public const int DLGC_WANTALLKEYS = 0x0004;
        public const int DLGC_WANTMESSAGE = 0x0004;

        public const int GWLP_WNDPROC = -4;

        public const int VK_BACK = 0x08;        // バックスペース
        public const int VK_RETURN = 0x0D;      // リターン
        public const int VK_SHIFT = 0x10;       // シフト
        public const int VK_CTRL = 0x11;        // コントロール
        public const int VK_ESCAPE = 0x1B;      // エスケープ

        public const int VK_SPACE = 0x20;       // スペース
        public const int VK_PRIOR = 0x21;       // ページアップ
        public const int VK_NEXT = 0x22;        // ページダウン
        public const int VK_END = 0x23;         // エンド
        public const int VK_HOME = 0x24;        // ホーム
        public const int VK_LEFT = 0x25;        // 左
        public const int VK_UP = 0x26;          // 上
        public const int VK_RIGHT = 0x27;       // 右
        public const int VK_DOWN = 0x28;        // 下
        //public const int VK_SELECT = 0x29;
        //public const int VK_PRINT = 0x2A;
        //public const int VK_EXECUTE = 0x2B;
        //public const int VK_SNAPSHOT = 0x2C;
        //public const int VK_INSERT = 0x2D;      // インサート
        public const int VK_DELETE = 0x2E;      // デリート
        //public const int VK_HELP = 0x2F;        // ヘルプ

        public const int VK_PROCESSKEY = 0xE5;

        public enum SystemMetric
        {
            SM_CXSCREEN = 0,  // 0x00
            SM_CYSCREEN = 1,  // 0x01
            SM_CXVSCROLL = 2,  // 0x02
            SM_CYHSCROLL = 3,  // 0x03
            SM_CYCAPTION = 4,  // 0x04
            SM_CXBORDER = 5,  // 0x05
            SM_CYBORDER = 6,  // 0x06
            SM_CXDLGFRAME = 7,  // 0x07
            SM_CXFIXEDFRAME = 7,  // 0x07
            SM_CYDLGFRAME = 8,  // 0x08
            SM_CYFIXEDFRAME = 8,  // 0x08
            SM_CYVTHUMB = 9,  // 0x09
            SM_CXHTHUMB = 10, // 0x0A
            SM_CXICON = 11, // 0x0B
            SM_CYICON = 12, // 0x0C
            SM_CXCURSOR = 13, // 0x0D
            SM_CYCURSOR = 14, // 0x0E
            SM_CYMENU = 15, // 0x0F
            SM_CXFULLSCREEN = 16, // 0x10
            SM_CYFULLSCREEN = 17, // 0x11
            SM_CYKANJIWINDOW = 18, // 0x12
            SM_MOUSEPRESENT = 19, // 0x13
            SM_CYVSCROLL = 20, // 0x14
            SM_CXHSCROLL = 21, // 0x15
            SM_DEBUG = 22, // 0x16
            SM_SWAPBUTTON = 23, // 0x17
            SM_CXMIN = 28, // 0x1C
            SM_CYMIN = 29, // 0x1D
            SM_CXSIZE = 30, // 0x1E
            SM_CYSIZE = 31, // 0x1F
            SM_CXSIZEFRAME = 32, // 0x20
            SM_CXFRAME = 32, // 0x20
            SM_CYSIZEFRAME = 33, // 0x21
            SM_CYFRAME = 33, // 0x21
            SM_CXMINTRACK = 34, // 0x22
            SM_CYMINTRACK = 35, // 0x23
            SM_CXDOUBLECLK = 36, // 0x24
            SM_CYDOUBLECLK = 37, // 0x25
            SM_CXICONSPACING = 38, // 0x26
            SM_CYICONSPACING = 39, // 0x27
            SM_MENUDROPALIGNMENT = 40, // 0x28
            SM_PENWINDOWS = 41, // 0x29
            SM_DBCSENABLED = 42, // 0x2A
            SM_CMOUSEBUTTONS = 43, // 0x2B
            SM_SECURE = 44, // 0x2C
            SM_CXEDGE = 45, // 0x2D
            SM_CYEDGE = 46, // 0x2E
            SM_CXMINSPACING = 47, // 0x2F
            SM_CYMINSPACING = 48, // 0x30
            SM_CXSMICON = 49, // 0x31
            SM_CYSMICON = 50, // 0x32
            SM_CYSMCAPTION = 51, // 0x33
            SM_CXSMSIZE = 52, // 0x34
            SM_CYSMSIZE = 53, // 0x35
            SM_CXMENUSIZE = 54, // 0x36
            SM_CYMENUSIZE = 55, // 0x37
            SM_ARRANGE = 56, // 0x38
            SM_CXMINIMIZED = 57, // 0x39
            SM_CYMINIMIZED = 58, // 0x3A
            SM_CXMAXTRACK = 59, // 0x3B
            SM_CYMAXTRACK = 60, // 0x3C
            SM_CXMAXIMIZED = 61, // 0x3D
            SM_CYMAXIMIZED = 62, // 0x3E
            SM_NETWORK = 63, // 0x3F
            SM_CLEANBOOT = 67, // 0x43
            SM_CXDRAG = 68, // 0x44
            SM_CYDRAG = 69, // 0x45
            SM_SHOWSOUNDS = 70, // 0x46
            SM_CXMENUCHECK = 71, // 0x47
            SM_CYMENUCHECK = 72, // 0x48
            SM_SLOWMACHINE = 73, // 0x49
            SM_MIDEASTENABLED = 74, // 0x4A
            SM_MOUSEWHEELPRESENT = 75, // 0x4B
            SM_XVIRTUALSCREEN = 76, // 0x4C
            SM_YVIRTUALSCREEN = 77, // 0x4D
            SM_CXVIRTUALSCREEN = 78, // 0x4E
            SM_CYVIRTUALSCREEN = 79, // 0x4F
            SM_CMONITORS = 80, // 0x50
            SM_SAMEDISPLAYFORMAT = 81, // 0x51
            SM_IMMENABLED = 82, // 0x52
            SM_CXFOCUSBORDER = 83, // 0x53
            SM_CYFOCUSBORDER = 84, // 0x54
            SM_TABLETPC = 86, // 0x56
            SM_MEDIACENTER = 87, // 0x57
            SM_STARTER = 88, // 0x58
            SM_SERVERR2 = 89, // 0x59
            SM_MOUSEHORIZONTALWHEELPRESENT = 91, // 0x5B
            SM_CXPADDEDBORDER = 92, // 0x5C
            SM_DIGITIZER = 94, // 0x5E
            SM_MAXIMUMTOUCHES = 95, // 0x5F

            SM_REMOTESESSION = 0x1000, // 0x1000
            SM_SHUTTINGDOWN = 0x2000, // 0x2000
            SM_REMOTECONTROL = 0x2001, // 0x2001

            SM_CONVERTABLESLATEMODE = 0x2003,
            SM_SYSTEMDOCKED = 0x2004,
        }

        public const int WHEEL_DELTA = 120;

        public delegate IntPtr WNDPROC(IntPtr window, Int32 message, IntPtr wParam, IntPtr lParam);

        public static short GET_WHEEL_DELTA_WPARAM(IntPtr p)
        {
            if(IntPtr.Size == 8) {
                // おそらく64bit
                return (short)HIWORD64(p);
            }
            else
            {
                // おそらく32bit
                return (short)HIWORD(p);
            }
        }

        public static int LOWORD(IntPtr p)
        {
            return p.ToInt32() & 0xffff;
        }

        public static int HIWORD(IntPtr p)
        {
            return (p.ToInt32() >> 16) & 0xffff;
        }

        public static long HIWORD64(IntPtr p)
        {
            return (p.ToInt64() >> 16) & 0xffff;
        }

        public static int min(int a, int b)
        {
            return (a > b ? b : a);
        }

        public static int max(int a, int b)
        {
            return (a < b ? b : a);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SCROLLINFO
        {
            public UInt32 cbSize;
            public UInt32 fMask;
            public Int32 nMin;
            public Int32 nMax;
            public Int32 nPage;
            public Int32 nPos;
            public Int32 nTrackPos;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public RECT(int left_, int top_, int right_, int bottom_)
            {
                left = left_;
                top = top_;
                right = right_;
                bottom = bottom_;
            }
            public RECT(Rectangle rect)
            {
                left = rect.Left;
                top = rect.Top;
                right = rect.Right;
                bottom = rect.Bottom;
            }
            public static RECT FromRectangle(Rectangle rectangle)
            {
                return new RECT(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
            }
            public Int32 left, top, right, bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PAINTSTRUCT
        {
            public IntPtr hdc;
            public bool fErase;
            public RECT rcPaint;
            public bool fRestore;
            public bool fIncUpdate;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] rgbReserved;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        //[DllImport("user32.dll")]
        //public static extern int GetSystemMetrics(SystemMetric smIndex);

        [DllImport("user32.dll")]
        public static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

        //[DllImport("coredll.dll")]
        //public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        // キャレット
        [DllImport("user32.dll")]
        public static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);
        [DllImport("user32.dll")]
        public static extern bool ShowCaret(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool SetCaretPos(int X, int Y);
        [DllImport("user32.dll")]
        public static extern bool DestroyCaret();

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
        public static extern IntPtr CreateCompatibleBitmap([In] IntPtr hdc, int nWidth, int nHeight);

        //[DllImport("user32.dll")]
        //public static extern int SetScrollInfo(IntPtr hwnd, int fnBar, [In] ref SCROLLINFO lpsi, bool fRedraw);

        //[DllImport("user32.dll")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool ShowScrollBar(IntPtr hWnd, int wBar, [MarshalAs(UnmanagedType.Bool)] bool bShow);

        //[DllImport("user32.dll")]
        //public static extern bool EnableScrollBar(IntPtr hWnd, uint wSBflags, uint wArrows);

        //[DllImport("gdi32.dll")]
        //public extern static int TextOut(IntPtr hdc, int nXStart, int nYStart, string lpString, int cbString);
        [DllImport("gdi32.dll", EntryPoint = "ExtTextOutW")]
        //public static extern bool ExtTextOut(IntPtr hdc, int X, int Y, uint fuOptions, [In] ref RECT lprc, [MarshalAs(UnmanagedType.LPWStr)] string lpString, uint cbCount, [In] IntPtr lpDx);
        public static extern bool ExtTextOut(IntPtr hdc, int X, int Y, uint fuOptions, [In] ref RECT lprc, [MarshalAs(UnmanagedType.LPWStr)] string lpString, uint cbCount, [In] int[] lpDx);
        //[DllImport("user32.dll")]
        //public static extern int TabbedTextOut(IntPtr hDC, int X, int Y, string lpString, int nCount, int nTabPositions, int[] lpnTabStopPositions, int nTabOrigin);

        [DllImport("gdi32", CharSet = CharSet.Unicode)]
        unsafe static extern bool ExtTextOutW(IntPtr hdc, Int32 x, Int32 y, UInt32 formatOptions, RECT* bounds, string text, UInt32 textLength, IntPtr zero);
        public static bool ExtTextOut2(IntPtr hdc, int x, int y, int formatOptions, string text)
        {
            unsafe
            {
                return ExtTextOutW(hdc, x, y, (uint)formatOptions, null, text, (UInt32)text.Length, IntPtr.Zero);
            }
        }
        public static bool ExtTextOut2(IntPtr hdc, int x, int y, Rectangle bounds, int formatOptions, string text)
        {
            unsafe
            {
                RECT rect = new RECT(bounds);
                return ExtTextOutW(hdc, x, y, (uint)formatOptions, &rect, text, (UInt32)text.Length, IntPtr.Zero);
            }
        }

        [DllImport("gdi32.dll")]
        public extern static IntPtr SelectObject(IntPtr hObject, IntPtr hFont);
        [DllImport("gdi32.dll")]
        public extern static bool DeleteObject(System.IntPtr hObject);

        [DllImport("gdi32.dll")]
        public extern static int SetROP2(IntPtr hdc, int fnDrawMode);
        [DllImport("gdi32.dll")]
        public extern static IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);
        [DllImport("gdi32.dll")]
        public extern static bool PaintRgn(IntPtr hdc, IntPtr hrgn);

        public enum BinaryRasterOperations
        {
            R2_BLACK = 1,
            R2_NOTMERGEPEN = 2,
            R2_MASKNOTPEN = 3,
            R2_NOTCOPYPEN = 4,
            R2_MASKPENNOT = 5,
            R2_NOT = 6,
            R2_XORPEN = 7,
            R2_NOTMASKPEN = 8,
            R2_MASKPEN = 9,
            R2_NOTXORPEN = 10,
            R2_NOP = 11,
            R2_MERGENOTPEN = 12,
            R2_COPYPEN = 13,
            R2_MERGEPENNOT = 14,
            R2_MERGEPEN = 15,
            R2_WHITE = 16
        }

        //[DllImport("gdi32.dll")]
        //public static extern bool GetTextExtentPoint32(IntPtr hdc, string lpString, int cbString, out Size lpSize);
        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        public static extern bool GetTextExtentPoint32(IntPtr hdc, string lpString, int cbString, out Size lpSize);

        [DllImport("gdi32", CharSet = CharSet.Unicode)]
        unsafe static extern Int32 GetTextExtentExPointW(IntPtr hdc, string text, int textLen, int maxWidth, int* out_fitLength, int* out_x, Size* out_size);
        public static Size GetTextExtent(IntPtr hdc, string text, int textLen, int maxWidth, out int fitLength, out int[] extents)
        {
            Int32 bOk;
            Size size;
            extents = new int[text.Length];

            unsafe
            {
                fixed (int* pExtents = extents)
                fixed (int* pFitLength = &fitLength)
                    bOk = GetTextExtentExPointW(hdc, text, textLen, maxWidth, pFitLength, pExtents, &size);
                Debug.Assert(bOk != 0, "failed to calculate text width");
                return new Size(size.Width, size.Height);
            }
        }

        //*****************************
        //[DllImport("coredll.dll")]
        //public static extern bool GetTextExtentExPointW(IntPtr hDc, string lpString, int nLength, int nMaxExtent, int[] lpnFit, int[] alpDx, out Size size);

        //[DllImport("gdi32.dll", EntryPoint = "GetTextExtentExPointW")]
        //public static extern bool GetTextExtentExPoint(IntPtr hdc, [MarshalAs(UnmanagedType.LPWStr)] string lpszStr, int cchString, int nMaxExtent, out int lpnFit, int[] alpDx, out Size lpSize);

        //*****************************

        [DllImport("gdi32.dll")]
        public static extern uint SetTextColor(IntPtr hdc, int crColor);

        // SetTextAlignを指定しない場合は既定値で、TA_LEFT、TA_TOP、および TA_NOUPDATECPがセットされているの
        // とりあえず使わない。
        //const uint TA_LEFT = 0;
        //const uint TA_RIGHT = 2;
        //const uint TA_CENTER = 6;
        //const uint TA_NOUPDATECP = 0;
        //const uint TA_TOP = 0;
        //[DllImport("gdi32")]
        //static extern UInt32 SetTextAlign(IntPtr hdc, UInt32 mode);
        //public static void SetTextAlign(IntPtr hdc, bool alignRight)
        //{
        //    uint flag = TA_TOP | TA_NOUPDATECP;
        //    flag |= alignRight ? TA_RIGHT : TA_LEFT;
        //    uint rc = SetTextAlign(hdc, flag);
        //    Debug.Assert(rc != UInt32.MaxValue, "failed to set text alignment by SetTextAlign.");
        //}
        [DllImport("gdi32")]
        public static extern Int32 SetBkMode(IntPtr hdc, Int32 mode);

        //[DllImport("user32.dll")]
        //public static extern bool SetScrollRange(IntPtr hWnd, int nBar, int nMinPos, int nMaxPos, bool bRedraw);
        //[DllImport("user32.dll")]
        //public static extern int SetScrollPos(IntPtr hWnd, System.Windows.Forms.Orientation nBar, int nPos, bool bRedraw);
        //public static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);
        //[DllImport("user32.dll")]
        //public static extern bool ScrollWindow(IntPtr hWnd, int XAmount, int YAmount, IntPtr lpRect, [In] ref RECT lpClipRect);

        //[DllImport("user32.dll")]
        //public static extern int ScrollWindowEx(IntPtr hWnd, int dx, int dy, IntPtr prcScroll, IntPtr prcClip, IntPtr hrgnUpdate, IntPtr prcUpdate, uint flags);
        [DllImport("user32")]
        unsafe public static extern Int32 ScrollWindowEx(IntPtr window, int x, int y, RECT* scroll, RECT* clip, IntPtr updateRegion, RECT* update, UInt32 flags);
        public static void ScrollWindow(IntPtr window, int x, int y)
        {
            unsafe
            {
                ScrollWindowEx(window, x, y, null, null, IntPtr.Zero, null, SW_INVALIDATE);
                //ScrollWindowEx( window, x, y, null, null, IntPtr.Zero, null, 0 );
            }
        }
        public static void ScrollWindow(IntPtr window, int x, int y, Rectangle clipRect)
        {
            unsafe
            {
                RECT clip = new RECT(clipRect);
                ScrollWindowEx(window, x, y, &clip, &clip, IntPtr.Zero, null, SW_INVALIDATE);
                //ScrollWindowEx( window, x, y, &clip, &clip, IntPtr.Zero, null, 0 );
            }
        }

        [DllImport("user32")]
        static unsafe extern int SetScrollInfo(IntPtr window, int barType, SCROLLINFO* si, Int32 bRedraw);
        public static void SetScrollPos(IntPtr window, bool isHScroll, int pos)
        {
            unsafe
            {
                SCROLLINFO si;
                si.cbSize = (uint)sizeof(SCROLLINFO);
                si.fMask = SIF_POS;
                si.nPos = pos;
                SetScrollInfo(window, isHScroll ? 0 : 1, &si, 1);
            }
        }

        [DllImport("user32.dll")]
        public static unsafe extern IntPtr BeginPaint(IntPtr hwnd, out PAINTSTRUCT lpPaint);
        [DllImport("user32.dll")]
        public static extern bool EndPaint(IntPtr hWnd, [In] ref PAINTSTRUCT lpPaint);
        //public static unsafe extern IntPtr BeginPaint(IntPtr hWnd, PAINTSTRUCT* ps);
        [DllImport("user32.dll")]
        public static extern bool UpdateWindow(IntPtr hWnd);

        //[DllImport("user32.dll", SetLastError = true)]
        //public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);
        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("gdi32.dll")]
        public static extern uint SetBkColor(IntPtr hdc, int crColor);

        [DllImport("user32")]
        static extern Int32 SetWindowLongPtrW(IntPtr hWnd, Int32 code, WNDPROC newLong);
        [DllImport("user32")]
        static extern Int32 SetWindowLongW(IntPtr hWnd, Int32 code, WNDPROC newLong);

        //[DllImport("USER32.dll")]
        //public static extern short GetKeyState(int nVirtKey);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC", SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC([In] IntPtr hdc);

        //[DllImport("coredll.dll")]
        //public static extern IntPtr GetDesktopWindow();

        //[DllImport("gdi32.dll")]
        //public static extern IntPtr GetStockObject(StockObjects fnObject);

        [DllImport("gdi32.dll")]
        public static extern bool PatBlt(IntPtr hdc, int nXLeft, int nYLeft, int nWidth, int nHeight, TernaryRasterOperations dwRop);

        [DllImport("gdi32.dll", EntryPoint = "BitBlt", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

        //[DllImport("gdi32.dll")]
        //public static extern bool Rectangle(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        //[DllImport("gdi32.dll", ExactSpelling = true, PreserveSig = true, SetLastError = true)]
        //public static extern bool Ellipse(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("gdi32.dll", EntryPoint = "CreateSolidBrush", SetLastError = true)]
        public static extern IntPtr CreateSolidBrush(int crColor);

        //[DllImport("imm32.dll")]
        //public static extern IntPtr ImmCreateContext();

        //[DllImport("Imm32.dll")]
        //public static extern bool ImmAssociateContextEx(IntPtr hWnd, IntPtr hIMC, ImmAssociateContextExFlags dwFlags);

        //[DllImport("Imm32.dll", CharSet = CharSet.Auto)]
        //public static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);

        [Flags]
        public enum ImmAssociateContextExFlags : uint
        {
            IACE_CHILDREN = 0x0001,
            IACE_DEFAULT = 0x0010,
            IACE_IGNORENOCONTEXT = 0x0020
        }
#if false
        public class LOGFONT
        {
            public int lfHeight = 0;
            public int lfWidth = 0;
            public int lfEscapement = 0;
            public int lfOrientation = 0;
            public int lfWeight = 0;
            public byte lfItalic = 0;
            public byte lfUnderline = 0;
            public byte lfStrikeOut = 0;
            public byte lfCharSet = 0;
            public byte lfOutPrecision = 0;
            public byte lfClipPrecision = 0;
            public byte lfQuality = 0;
            public byte lfPitchAndFamily = 0;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string lfFaceName = string.Empty;
        }
#endif
        public enum StockObjects
        {
            WHITE_BRUSH = 0,
            LTGRAY_BRUSH = 1,
            GRAY_BRUSH = 2,
            DKGRAY_BRUSH = 3,
            BLACK_BRUSH = 4,
            NULL_BRUSH = 5,
            HOLLOW_BRUSH = NULL_BRUSH,
            WHITE_PEN = 6,
            BLACK_PEN = 7,
            NULL_PEN = 8,
            OEM_FIXED_FONT = 10,
            ANSI_FIXED_FONT = 11,
            ANSI_VAR_FONT = 12,
            SYSTEM_FONT = 13,
            DEVICE_DEFAULT_FONT = 14,
            DEFAULT_PALETTE = 15,
            SYSTEM_FIXED_FONT = 16,
            DEFAULT_GUI_FONT = 17,
            DC_BRUSH = 18,
            DC_PEN = 19,
        }

        public enum TernaryRasterOperations : uint
        {
            /// <summary>dest = source</summary>
            SRCCOPY = 0x00CC0020,
            /// <summary>dest = source OR dest</summary>
            SRCPAINT = 0x00EE0086,
            /// <summary>dest = source AND dest</summary>
            SRCAND = 0x008800C6,
            /// <summary>dest = source XOR dest</summary>
            SRCINVERT = 0x00660046,
            /// <summary>dest = source AND (NOT dest)</summary>
            SRCERASE = 0x00440328,
            /// <summary>dest = (NOT source)</summary>
            NOTSRCCOPY = 0x00330008,
            /// <summary>dest = (NOT src) AND (NOT dest)</summary>
            NOTSRCERASE = 0x001100A6,
            /// <summary>dest = (source AND pattern)</summary>
            MERGECOPY = 0x00C000CA,
            /// <summary>dest = (NOT source) OR dest</summary>
            MERGEPAINT = 0x00BB0226,
            /// <summary>dest = pattern</summary>
            PATCOPY = 0x00F00021,
            /// <summary>dest = DPSnoo</summary>
            PATPAINT = 0x00FB0A09,
            /// <summary>dest = pattern XOR dest</summary>
            PATINVERT = 0x005A0049,
            /// <summary>dest = (NOT dest)</summary>
            DSTINVERT = 0x00550009,
            /// <summary>dest = BLACK</summary>
            BLACKNESS = 0x00000042,
            /// <summary>dest = WHITE</summary>
            WHITENESS = 0x00FF0062,
            /// <summary>
            /// Capture window as seen on screen.  This includes layered windows 
            /// such as WPF windows with AllowsTransparency="true"
            /// </summary>
            CAPTUREBLT = 0x40000000
        }

        public static Int32 SetWindowLong(IntPtr hWnd, Int32 code, WNDPROC newLong)
        {
            if (Marshal.SizeOf(IntPtr.Zero) == 4)
                return SetWindowLongW(hWnd, code, newLong);
            else
                return SetWindowLongPtrW(hWnd, code, newLong);
        }

        [DllImport("user32", EntryPoint = "GetWindowLongPtrW")]
        static extern IntPtr GetWindowLongPtrW(IntPtr hWnd, Int32 code);
        [DllImport("user32", EntryPoint = "GetWindowLongW")]
        static extern IntPtr GetWindowLongW(IntPtr hWnd, Int32 code);

        public static IntPtr GetWindowLong(IntPtr hWnd, Int32 code)
        {
            if (Marshal.SizeOf(IntPtr.Zero) == 4)
                return GetWindowLongW(hWnd, code);
            else
                return GetWindowLongPtrW(hWnd, code);
        }

        [DllImport("user32")]
        public static extern IntPtr CallWindowProc(IntPtr wndProc, IntPtr window, Int32 message, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool GetCaretPos(out POINT lpPoint);

        //[DllImport("gdi32.dll")]
        //public static extern uint SetDCPenColor(IntPtr hdc, uint crColor);

        //[DllImport("gdi32.dll")]
        //public static extern uint SetDCBrushColor(IntPtr hdc, uint crColor);

        //[DllImport("gdi32.dll")]
        //public static extern IntPtr CreateSolidBrush(uint crColor);

        //[System.Runtime.InteropServices.DllImport("shell32.dll")]
        //private static extern int FindExecutable( string lpFile, string lpDirectory, System.Text.StringBuilder lpResult);

        // IME関係

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("imm32.dll")]
        public static extern int ImmSetCompositionFont(IntPtr hIMC, [In, Out] IntPtr lplf);

        [DllImport("Imm32.dll", SetLastError = true)]
        public static extern IntPtr ImmGetContext(IntPtr hWnd);
        //[DllImport("imm32.dll", CharSet = CharSet.Unicode)]
        //public static extern int ImmGetCompositionString(IntPtr hIMC, uint dwIndex, char[] lpBuf, uint dwBufLen);
        [DllImport("Imm32.dll")]
        public static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);
        [DllImport("Imm32.dll")]
        public static extern bool ImmSetCandidateWindow(IntPtr hIMC, ref CANDIDATEFORM lpCandidate);
        //[DllImport("imm32.dll")]
        //public static extern bool ImmGetConversionStatus(IntPtr himc, ref int lpdw, ref int lpdw2);
        [DllImport("imm32.dll")]
        public static extern bool ImmSetCompositionWindow(IntPtr hIMC, ref COMPOSITIONFORM lpCompForm);
        //[DllImport("imm32.dll")]
        //public static extern int ImmSetCompositionStringW(IntPtr himc, int dwIndex, IntPtr lpComp, int dw, int lpRead, int dw2);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalLock(IntPtr hMem);

        public const int LF_FACESIZE = 32;

        public struct COMPOSITIONFORM
        {
            public uint dwStyle;
            public POINT ptCurrentPos;
            public RECT rcArea;
        }

#if true
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public POINT(System.Drawing.Point pt) : this(pt.X, pt.Y) { }

            public static implicit operator System.Drawing.Point(POINT p)
            {
                return new System.Drawing.Point(p.X, p.Y);
            }

            public static implicit operator POINT(System.Drawing.Point p)
            {
                return new POINT(p.X, p.Y);
            }
        }
#else
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }
#endif

        // CANDIDATEFORM structures
        [StructLayout(LayoutKind.Sequential)]
        public struct CANDIDATEFORM
        {
            public uint dwIndex;
            public uint dwStyle;
            public POINT ptCurrentPos;
            public RECT rcArea;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class LOGFONT
        {
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public FontWeight lfWeight;
            [MarshalAs(UnmanagedType.U1)]
            public bool lfItalic;
            [MarshalAs(UnmanagedType.U1)]
            public bool lfUnderline;
            [MarshalAs(UnmanagedType.U1)]
            public bool lfStrikeOut;
            public FontCharSet lfCharSet;
            public FontPrecision lfOutPrecision;
            public FontClipPrecision lfClipPrecision;
            public FontQuality lfQuality;
            public FontPitchAndFamily lfPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LF_FACESIZE * 2)]
            public string lfFaceName;
        }
        //LOGFONT
        public enum FontWeight : int
        {
            FW_DONTCARE = 0,
            FW_THIN = 100,
            FW_EXTRALIGHT = 200,
            FW_LIGHT = 300,
            FW_NORMAL = 400,
            FW_MEDIUM = 500,
            FW_SEMIBOLD = 600,
            FW_BOLD = 700,
            FW_EXTRABOLD = 800,
            FW_HEAVY = 900,
        }
        public enum FontCharSet : byte
        {
            ANSI_CHARSET = 0,
            DEFAULT_CHARSET = 1,
            SYMBOL_CHARSET = 2,
            SHIFTJIS_CHARSET = 128,
            HANGEUL_CHARSET = 129,
            HANGUL_CHARSET = 129,
            GB2312_CHARSET = 134,
            CHINESEBIG5_CHARSET = 136,
            OEM_CHARSET = 255,
            JOHAB_CHARSET = 130,
            HEBREW_CHARSET = 177,
            ARABIC_CHARSET = 178,
            GREEK_CHARSET = 161,
            TURKISH_CHARSET = 162,
            VIETNAMESE_CHARSET = 163,
            THAI_CHARSET = 222,
            EASTEUROPE_CHARSET = 238,
            RUSSIAN_CHARSET = 204,
            MAC_CHARSET = 77,
            BALTIC_CHARSET = 186,
        }
        public enum FontPrecision : byte
        {
            OUT_DEFAULT_PRECIS = 0,
            OUT_STRING_PRECIS = 1,
            OUT_CHARACTER_PRECIS = 2,
            OUT_STROKE_PRECIS = 3,
            OUT_TT_PRECIS = 4,
            OUT_DEVICE_PRECIS = 5,
            OUT_RASTER_PRECIS = 6,
            OUT_TT_ONLY_PRECIS = 7,
            OUT_OUTLINE_PRECIS = 8,
            OUT_SCREEN_OUTLINE_PRECIS = 9,
            OUT_PS_ONLY_PRECIS = 10,
        }
        public enum FontClipPrecision : byte
        {
            CLIP_DEFAULT_PRECIS = 0,
            CLIP_CHARACTER_PRECIS = 1,
            CLIP_STROKE_PRECIS = 2,
            CLIP_MASK = 0xf,
            CLIP_LH_ANGLES = (1 << 4),
            CLIP_TT_ALWAYS = (2 << 4),
            CLIP_DFA_DISABLE = (4 << 4),
            CLIP_EMBEDDED = (8 << 4),
        }
        public enum FontQuality : byte
        {
            DEFAULT_QUALITY = 0,
            DRAFT_QUALITY = 1,
            PROOF_QUALITY = 2,
            NONANTIALIASED_QUALITY = 3,
            ANTIALIASED_QUALITY = 4,
            CLEARTYPE_QUALITY = 5,
            CLEARTYPE_NATURAL_QUALITY = 6,
        }
        [Flags]
        public enum FontPitchAndFamily : byte
        {
            DEFAULT_PITCH = 0,
            FIXED_PITCH = 1,
            VARIABLE_PITCH = 2,
            FF_DONTCARE = (0 << 4),
            FF_ROMAN = (1 << 4),
            FF_SWISS = (2 << 4),
            FF_MODERN = (3 << 4),
            FF_SCRIPT = (4 << 4),
            FF_DECORATIVE = (5 << 4),
        }

        public const int SCS_SETSTR = 0x00000009;

        public const int GCS_RESULTSTR = 0x0800;
        public const int GCS_COMPSTR = 0x0008;
        public const int CFS_POINT = 0x0002;
        public const int CFS_FORCE_POSITION = 0x0020;
        public const int CFS_CANDIDATEPOS = 0x0040;

        public const int IMN_CLOSESTATUSWINDOW = 0x0001;
        public const int IMN_OPENSTATUSWINDOW = 0x0002;
        public const int IMN_CHANGECANDIDATE = 0x0003;
        public const int IMN_CLOSECANDIDATE = 0x0004;
        public const int IMN_OPENCANDIDATE = 0x0005;
        public const int IMN_SETCONVERSIONMODE = 0x0006;
        public const int IMN_SETSENTENCEMODE = 0x0007;
        public const int IMN_SETOPENSTATUS = 0x0008;
        public const int IMN_SETCANDIDATEPOS = 0x0009;
        public const int IMN_SETCOMPOSITIONFONT = 0x000A;
        public const int IMN_SETCOMPOSITIONWINDOW = 0x000B;
        public const int IMN_SETSTATUSWINDOWPOS = 0x000C;
        public const int IMN_GUIDELINE = 0x000D;
        public const int IMN_PRIVATE = 0x000E;

        //[DllImport("user32.dll", SetLastError = true)]
        //public static extern uint RegisterClipboardFormat(string lpszFormat);

        //[DllImport("coredll.dll", SetLastError = true)]
        //public static extern int SetClipboardData(uint uFormat, IntPtr hMem);

        [DllImport("user32.dll")]
        public static extern bool HideCaret(IntPtr hWnd);

#if false
        [Flags]
        public enum WindowStylesEx : uint
        {
            /// <summary>Specifies a window that accepts drag-drop files.</summary>
            WS_EX_ACCEPTFILES = 0x00000010,

            /// <summary>Forces a top-level window onto the taskbar when the window is visible.</summary>
            WS_EX_APPWINDOW = 0x00040000,

            /// <summary>Specifies a window that has a border with a sunken edge.</summary>
            WS_EX_CLIENTEDGE = 0x00000200,

            /// <summary>
            /// Specifies a window that paints all descendants in bottom-to-top painting order using double-buffering.
            /// This cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC. This style is not supported in Windows 2000.
            /// </summary>
            /// <remarks>
            /// With WS_EX_COMPOSITED set, all descendants of a window get bottom-to-top painting order using double-buffering.
            /// Bottom-to-top painting order allows a descendent window to have translucency (alpha) and transparency (color-key) effects,
            /// but only if the descendent window also has the WS_EX_TRANSPARENT bit set.
            /// Double-buffering allows the window and its descendents to be painted without flicker.
            /// </remarks>
            WS_EX_COMPOSITED = 0x02000000,

            /// <summary>
            /// Specifies a window that includes a question mark in the title bar. When the user clicks the question mark,
            /// the cursor changes to a question mark with a pointer. If the user then clicks a child window, the child receives a WM_HELP message.
            /// The child window should pass the message to the parent window procedure, which should call the WinHelp function using the HELP_WM_HELP command.
            /// The Help application displays a pop-up window that typically contains help for the child window.
            /// WS_EX_CONTEXTHELP cannot be used with the WS_MAXIMIZEBOX or WS_MINIMIZEBOX styles.
            /// </summary>
            WS_EX_CONTEXTHELP = 0x00000400,

            /// <summary>
            /// Specifies a window which contains child windows that should take part in dialog box navigation.
            /// If this style is specified, the dialog manager recurses into children of this window when performing navigation operations
            /// such as handling the TAB key, an arrow key, or a keyboard mnemonic.
            /// </summary>
            WS_EX_CONTROLPARENT = 0x00010000,

            /// <summary>Specifies a window that has a double border.</summary>
            WS_EX_DLGMODALFRAME = 0x00000001,

            /// <summary>
            /// Specifies a window that is a layered window.
            /// This cannot be used for child windows or if the window has a class style of either CS_OWNDC or CS_CLASSDC.
            /// </summary>
            WS_EX_LAYERED = 0x00080000,

            /// <summary>
            /// Specifies a window with the horizontal origin on the right edge. Increasing horizontal values advance to the left.
            /// The shell language must support reading-order alignment for this to take effect.
            /// </summary>
            WS_EX_LAYOUTRTL = 0x00400000,

            /// <summary>Specifies a window that has generic left-aligned properties. This is the default.</summary>
            WS_EX_LEFT = 0x00000000,

            /// <summary>
            /// Specifies a window with the vertical scroll bar (if present) to the left of the client area.
            /// The shell language must support reading-order alignment for this to take effect.
            /// </summary>
            WS_EX_LEFTSCROLLBAR = 0x00004000,

            /// <summary>
            /// Specifies a window that displays text using left-to-right reading-order properties. This is the default.
            /// </summary>
            WS_EX_LTRREADING = 0x00000000,

            /// <summary>
            /// Specifies a multiple-document interface (MDI) child window.
            /// </summary>
            WS_EX_MDICHILD = 0x00000040,

            /// <summary>
            /// Specifies a top-level window created with this style does not become the foreground window when the user clicks it.
            /// The system does not bring this window to the foreground when the user minimizes or closes the foreground window.
            /// The window does not appear on the taskbar by default. To force the window to appear on the taskbar, use the WS_EX_APPWINDOW style.
            /// To activate the window, use the SetActiveWindow or SetForegroundWindow function.
            /// </summary>
            WS_EX_NOACTIVATE = 0x08000000,

            /// <summary>
            /// Specifies a window which does not pass its window layout to its child windows.
            /// </summary>
            WS_EX_NOINHERITLAYOUT = 0x00100000,

            /// <summary>
            /// Specifies that a child window created with this style does not send the WM_PARENTNOTIFY message to its parent window when it is created or destroyed.
            /// </summary>
            WS_EX_NOPARENTNOTIFY = 0x00000004,

            /// <summary>Specifies an overlapped window.</summary>
            WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,

            /// <summary>Specifies a palette window, which is a modeless dialog box that presents an array of commands.</summary>
            WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,

            /// <summary>
            /// Specifies a window that has generic "right-aligned" properties. This depends on the window class.
            /// The shell language must support reading-order alignment for this to take effect.
            /// Using the WS_EX_RIGHT style has the same effect as using the SS_RIGHT (static), ES_RIGHT (edit), and BS_RIGHT/BS_RIGHTBUTTON (button) control styles.
            /// </summary>
            WS_EX_RIGHT = 0x00001000,

            /// <summary>Specifies a window with the vertical scroll bar (if present) to the right of the client area. This is the default.</summary>
            WS_EX_RIGHTSCROLLBAR = 0x00000000,

            /// <summary>
            /// Specifies a window that displays text using right-to-left reading-order properties.
            /// The shell language must support reading-order alignment for this to take effect.
            /// </summary>
            WS_EX_RTLREADING = 0x00002000,

            /// <summary>Specifies a window with a three-dimensional border style intended to be used for items that do not accept user input.</summary>
            WS_EX_STATICEDGE = 0x00020000,

            /// <summary>
            /// Specifies a window that is intended to be used as a floating toolbar.
            /// A tool window has a title bar that is shorter than a normal title bar, and the window title is drawn using a smaller font.
            /// A tool window does not appear in the taskbar or in the dialog that appears when the user presses ALT+TAB.
            /// If a tool window has a system menu, its icon is not displayed on the title bar.
            /// However, you can display the system menu by right-clicking or by typing ALT+SPACE. 
            /// </summary>
            WS_EX_TOOLWINDOW = 0x00000080,

            /// <summary>
            /// Specifies a window that should be placed above all non-topmost windows and should stay above them, even when the window is deactivated.
            /// To add or remove this style, use the SetWindowPos function.
            /// </summary>
            WS_EX_TOPMOST = 0x00000008,

            /// <summary>
            /// Specifies a window that should not be painted until siblings beneath the window (that were created by the same thread) have been painted.
            /// The window appears transparent because the bits of underlying sibling windows have already been painted.
            /// To achieve transparency without these restrictions, use the SetWindowRgn function.
            /// </summary>
            WS_EX_TRANSPARENT = 0x00000020,

            /// <summary>Specifies a window that has a border with a raised edge.</summary>
            WS_EX_WINDOWEDGE = 0x00000100
        }
        /// <summary>
        /// Window Styles.
        /// The following styles can be specified wherever a window style is required. After the control has been created, these styles cannot be modified, except as noted.
        /// </summary>
        [Flags()]
        public enum WindowStyles : uint
        {
            /// <summary>The window has a thin-line border.</summary>
            WS_BORDER = 0x800000,

            /// <summary>The window has a title bar (includes the WS_BORDER style).</summary>
            WS_CAPTION = 0xc00000,

            /// <summary>The window is a child window. A window with this style cannot have a menu bar. This style cannot be used with the WS_POPUP style.</summary>
            WS_CHILD = 0x40000000,

            /// <summary>Excludes the area occupied by child windows when drawing occurs within the parent window. This style is used when creating the parent window.</summary>
            WS_CLIPCHILDREN = 0x2000000,

            /// <summary>
            /// Clips child windows relative to each other; that is, when a particular child window receives a WM_PAINT message, the WS_CLIPSIBLINGS style clips all other overlapping child windows out of the region of the child window to be updated.
            /// If WS_CLIPSIBLINGS is not specified and child windows overlap, it is possible, when drawing within the client area of a child window, to draw within the client area of a neighboring child window.
            /// </summary>
            WS_CLIPSIBLINGS = 0x4000000,

            /// <summary>The window is initially disabled. A disabled window cannot receive input from the user. To change this after a window has been created, use the EnableWindow function.</summary>
            WS_DISABLED = 0x8000000,

            /// <summary>The window has a border of a style typically used with dialog boxes. A window with this style cannot have a title bar.</summary>
            WS_DLGFRAME = 0x400000,

            /// <summary>
            /// The window is the first control of a group of controls. The group consists of this first control and all controls defined after it, up to the next control with the WS_GROUP style.
            /// The first control in each group usually has the WS_TABSTOP style so that the user can move from group to group. The user can subsequently change the keyboard focus from one control in the group to the next control in the group by using the direction keys.
            /// You can turn this style on and off to change dialog box navigation. To change this style after a window has been created, use the SetWindowLong function.
            /// </summary>
            WS_GROUP = 0x20000,

            /// <summary>The window has a horizontal scroll bar.</summary>
            WS_HSCROLL = 0x100000,

            /// <summary>The window is initially maximized.</summary> 
            WS_MAXIMIZE = 0x1000000,

            /// <summary>The window has a maximize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.</summary> 
            WS_MAXIMIZEBOX = 0x10000,

            /// <summary>The window is initially minimized.</summary>
            WS_MINIMIZE = 0x20000000,

            /// <summary>The window has a minimize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.</summary>
            WS_MINIMIZEBOX = 0x20000,

            /// <summary>The window is an overlapped window. An overlapped window has a title bar and a border.</summary>
            WS_OVERLAPPED = 0x0,

            /// <summary>The window is an overlapped window.</summary>
            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_SIZEFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,

            /// <summary>The window is a pop-up window. This style cannot be used with the WS_CHILD style.</summary>
            WS_POPUP = 0x80000000u,

            /// <summary>The window is a pop-up window. The WS_CAPTION and WS_POPUPWINDOW styles must be combined to make the window menu visible.</summary>
            WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,

            /// <summary>The window has a sizing border.</summary>
            WS_SIZEFRAME = 0x40000,

            /// <summary>The window has a sizing border. Same as the WS_SIZEBOX style.</summary>
            WS_THICKFRAME = 0x00040000,

            /// <summary>The window has a window menu on its title bar. The WS_CAPTION style must also be specified.</summary>
            WS_SYSMENU = 0x80000,

            /// <summary>
            /// The window is a control that can receive the keyboard focus when the user presses the TAB key.
            /// Pressing the TAB key changes the keyboard focus to the next control with the WS_TABSTOP style.  
            /// You can turn this style on and off to change dialog box navigation. To change this style after a window has been created, use the SetWindowLong function.
            /// For user-created windows and modeless dialogs to work with tab stops, alter the message loop to call the IsDialogMessage function.
            /// </summary>
            WS_TABSTOP = 0x10000,

            /// <summary>The window is initially visible. This style can be turned on and off by using the ShowWindow or SetWindowPos function.</summary>
            WS_VISIBLE = 0x10000000,

            /// <summary>The window has a vertical scroll bar.</summary>
            WS_VSCROLL = 0x200000
            
            , SBS_VERT = 0x1
        }
        /// <summary>
        /// The CreateWindowEx function creates an overlapped, pop-up, or child window with an extended window style; otherwise, this function is identical to the CreateWindow function. 
        /// </summary>
        /// <param name="dwExStyle">Specifies the extended window style of the window being created.</param>
        /// <param name="lpClassName">Pointer to a null-terminated string or a class atom created by a previous call to the RegisterClass or RegisterClassEx function. The atom must be in the low-order word of lpClassName; the high-order word must be zero. If lpClassName is a string, it specifies the window class name. The class name can be any name registered with RegisterClass or RegisterClassEx, provided that the module that registers the class is also the module that creates the window. The class name can also be any of the predefined system class names.</param>
        /// <param name="lpWindowName">Pointer to a null-terminated string that specifies the window name. If the window style specifies a title bar, the window title pointed to by lpWindowName is displayed in the title bar. When using CreateWindow to create controls, such as buttons, check boxes, and static controls, use lpWindowName to specify the text of the control. When creating a static control with the SS_ICON style, use lpWindowName to specify the icon name or identifier. To specify an identifier, use the syntax "#num". </param>
        /// <param name="dwStyle">Specifies the style of the window being created. This parameter can be a combination of window styles, plus the control styles indicated in the Remarks section.</param>
        /// <param name="x">Specifies the initial horizontal position of the window. For an overlapped or pop-up window, the x parameter is the initial x-coordinate of the window's upper-left corner, in screen coordinates. For a child window, x is the x-coordinate of the upper-left corner of the window relative to the upper-left corner of the parent window's client area. If x is set to CW_USEDEFAULT, the system selects the default position for the window's upper-left corner and ignores the y parameter. CW_USEDEFAULT is valid only for overlapped windows; if it is specified for a pop-up or child window, the x and y parameters are set to zero.</param>
        /// <param name="y">Specifies the initial vertical position of the window. For an overlapped or pop-up window, the y parameter is the initial y-coordinate of the window's upper-left corner, in screen coordinates. For a child window, y is the initial y-coordinate of the upper-left corner of the child window relative to the upper-left corner of the parent window's client area. For a list box y is the initial y-coordinate of the upper-left corner of the list box's client area relative to the upper-left corner of the parent window's client area.
        /// <para>If an overlapped window is created with the WS_VISIBLE style bit set and the x parameter is set to CW_USEDEFAULT, then the y parameter determines how the window is shown. If the y parameter is CW_USEDEFAULT, then the window manager calls ShowWindow with the SW_SHOW flag after the window has been created. If the y parameter is some other value, then the window manager calls ShowWindow with that value as the nCmdShow parameter.</para></param>
        /// <param name="nWidth">Specifies the width, in device units, of the window. For overlapped windows, nWidth is the window's width, in screen coordinates, or CW_USEDEFAULT. If nWidth is CW_USEDEFAULT, the system selects a default width and height for the window; the default width extends from the initial x-coordinates to the right edge of the screen; the default height extends from the initial y-coordinate to the top of the icon area. CW_USEDEFAULT is valid only for overlapped windows; if CW_USEDEFAULT is specified for a pop-up or child window, the nWidth and nHeight parameter are set to zero.</param>
        /// <param name="nHeight">Specifies the height, in device units, of the window. For overlapped windows, nHeight is the window's height, in screen coordinates. If the nWidth parameter is set to CW_USEDEFAULT, the system ignores nHeight.</param> <param name="hWndParent">Handle to the parent or owner window of the window being created. To create a child window or an owned window, supply a valid window handle. This parameter is optional for pop-up windows.
        /// <para>Windows 2000/XP: To create a message-only window, supply HWND_MESSAGE or a handle to an existing message-only window.</para></param>
        /// <param name="hMenu">Handle to a menu, or specifies a child-window identifier, depending on the window style. For an overlapped or pop-up window, hMenu identifies the menu to be used with the window; it can be NULL if the class menu is to be used. For a child window, hMenu specifies the child-window identifier, an integer value used by a dialog box control to notify its parent about events. The application determines the child-window identifier; it must be unique for all child windows with the same parent window.</param>
        /// <param name="hInstance">Handle to the instance of the module to be associated with the window.</param> <param name="lpParam">Pointer to a value to be passed to the window through the CREATESTRUCT structure (lpCreateParams member) pointed to by the lParam param of the WM_CREATE message. This message is sent to the created window by this function before it returns.
        /// <para>If an application calls CreateWindow to create a MDI client window, lpParam should point to a CLIENTCREATESTRUCT structure. If an MDI client window calls CreateWindow to create an MDI child window, lpParam should point to a MDICREATESTRUCT structure. lpParam may be NULL if no additional data is needed.</para></param>
        /// <returns>If the function succeeds, the return value is a handle to the new window.
        /// <para>If the function fails, the return value is NULL. To get extended error information, call GetLastError.</para>
        /// <para>This function typically fails for one of the following reasons:</para>
        /// <list type="">
        /// <item>an invalid parameter value</item>
        /// <item>the system class was registered by a different module</item>
        /// <item>The WH_CBT hook is installed and returns a failure code</item>
        /// <item>if one of the controls in the dialog template is not registered, or its window window procedure fails WM_CREATE or WM_NCCREATE</item>
        /// </list></returns>

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CreateWindowEx(
           WindowStylesEx dwExStyle,
           [MarshalAs(UnmanagedType.LPStr)] string lpClassName,
           [MarshalAs(UnmanagedType.LPStr)] string lpWindowName,
           WindowStyles dwStyle,
           int x,
           int y,
           int nWidth,
           int nHeight,
           IntPtr hWndParent,
           IntPtr hMenu,
           IntPtr hInstance,
           IntPtr lpParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowEnabled(IntPtr hWnd);
#endif
        [DllImport("user32.dll")]
        public static extern IntPtr LoadCursor(IntPtr hInstance, string lpCursorName);

        [DllImport("user32.dll")]
        public static extern IntPtr SetCursor(IntPtr hCursor);

        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconIndirect([In] ref IconInfo piconinfo);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

        public struct IconInfo
        {

            public bool fIcon;      // iconならtrue,curならfalse
            public int xHotspot;    // x座標のHotspot
            public int yHotspot;    // y座標のHotspot
            public IntPtr hbmMask;  // アイコン画像のマスク( 上半分がANDマスク、下半分がXORマスク )
            public IntPtr hbmColor; // カラービットマップ、白黒のアイコンならなしでもよい
        }
    }
}