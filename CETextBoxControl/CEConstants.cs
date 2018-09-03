using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CETextBoxControl
{
    public class CEConstants
    {
        public const string LineFeed = "\r\n";

        public const int FontDefaultHeight = 12;
        public const int FontDefaultWidth = 6;

        public const int DefaultTabSize = 4;        // タブ数

        public const int DefaultWrapSize = 560000;    // 折り返しサイズ（ピクセル）

        public const int CursorMode = 1;    // カーソルモード（0:通常、1:フリー）

        public const int MAMouseScroll = -3;    // マウスホイール時の移動量（-3を指定すると3行ごとに移動する）

        public const int TabWidth = 100;     // タブボタンの幅

        public const string EmptyViewName = "[無題]";   // 空のビューの名称
#if false // 標準

        // 背景色
        public static int BackColor = CECommon.GetColor(0x00, 0x00, 0x00);
        public static int kBackColor = CECommon.GetColor(0x00, 0x00, 0x00);     // 非アクティブになった時の色

        //public static int BackColor = CECommon.GetColor(0x3C, 0x64, 0x50);
        //public static int BackColor = CECommon.GetColor(0x00, 0x32, 0x00);
        //public static int kBackColor = CECommon.GetColor(0x3C, 0x64, 0x50);     // 非アクティブになった時の色

        // フォント色
        public static int FontColor = CECommon.GetColor(0xff, 0xff, 0xff);
        public static int SymbolColor = CECommon.GetColor(0x88, 0x88, 0x88);
        public static int currentLineColor = CECommon.GetColor(0xff, 0xff, 0x00);
        public static int lrColor = CECommon.GetColor(0xff, 0xff, 0x00);
        public static int eofColor = CECommon.GetColor(0x00, 0xff, 0x00);
        public static int wrapColor = CECommon.GetColor(0xff, 0x00, 0xff);

        // コメント色
        public static int CommentColor = CECommon.GetColor(0x0, 0xff, 0x0);

        // 選択
        public static int SelectBackColor = CECommon.GetColor(0xc1, 0xcf, 0xe3);
        public static int SelectFontColor = CECommon.GetColor(0x11, 0x11, 0xaa);
        public static int SelectRctFontColor = CECommon.GetColor(0x99, 0x99, 0x00); // 矩形選択用

        // 行番号
        public static int currenLineNumBkColor = CECommon.GetColor(0xff, 0x99, 0x99);   // カレント行の背景色
        public static int lineNumBkColor = CECommon.GetColor(0xff, 0xfb, 0xf0);         // 通常行の背景色
        public static int editLineNumBkColor = CECommon.GetColor(0xff, 0xfb, 0xf0);     // 編集行の背景色

        public static int lineNumFtColor = CECommon.GetColor(0x00, 0x00, 0x00);         // 通常行の文字色
        public static int editLineNumFtColor = CECommon.GetColor(0xff, 0x33, 0x33);     // 編集行の文字色

        public static int NumberLineColor = CECommon.GetColor(0x66, 0x44, 0xff);        // ライン色
#endif

#if false // 黒ベース

        // 背景色
        public static int BackColor = CECommon.GetColor(0x00, 0x00, 0x00);              // 背景

        // フォント色
        public static int FontColor = CECommon.GetColor(0xff, 0xff, 0xff);              // フォント
        public static int SymbolFontColor = CECommon.GetColor(0x88, 0x88, 0x88);        // タブ、全角空白等
        public static int CommentFontColor = CECommon.GetColor(0x00, 0xff, 0x00);       // コメント

        public static int lrColor = CECommon.GetColor(0x88, 0x88, 0x88);                // 改行
        public static int eofColor = CECommon.GetColor(0x00, 0xff, 0x00);               // EOF
        public static int wrapColor = CECommon.GetColor(0xff, 0x00, 0xff);              // 折返し

        public static int currentLineColor = CECommon.GetColor(0xff, 0xff, 0x00);       // カレント位置

        // 行番号
        public static int LineFontColor = CECommon.GetColor(0xff, 0xff, 0xff);          // フォント
        public static int EditLineFontColor = CECommon.GetColor(0x55, 0xff, 0xff);      // フォント(編集)

        // 範囲選択
        public static int SelectBackColor = CECommon.GetColor(0xff, 0xff, 0xff);        // 背景
        public static int SelectFontColor = CECommon.GetColor(0x00, 0x00, 0x00);        // フォント
        public static int SelectRctFontColor = CECommon.GetColor(0x00, 0x00, 0x00);     // フォント（矩形選択）

        // ルーラー
        public static int CaretColPosColor = CECommon.GetColor(0xff, 0x00, 0x00);       // キャレット位置

        // タブ
        public static int TabBackColor = CECommon.GetColor(0x00, 0x7a, 0xcc);           // 背景
        public static int TabFontColor = CECommon.GetColor(0xff, 0xff, 0xff);           // フォント
        //public static int TabHoverColor = CECommon.GetColor(0xff, 0xe5, 0xaf);           // 背景（マウスを重ねる）
        //public static int TabBackColor = CECommon.GetColor(0x00, 0x7a, 0xcc);           // 背景
        //public static int TabFontColor = CECommon.GetColor(0xff, 0xff, 0xff);           // フォント
#endif

#if true // 白ベース

        // 背景色
        public static int BackColor = CECommon.GetColor(0xff, 0xff, 0xff);              // 背景

        // フォント色
        public static int FontColor = CECommon.GetColor(0x00, 0x00, 0x00);              // フォント
//        public static int SymbolFontColor = CECommon.GetColor(0xdd, 0xdd, 0xdd);        // タブ、全角空白等
        public static int SymbolFontColor = CECommon.GetColor(0x99, 0x99, 0x99);        // タブ、全角空白等
        public static int CommentFontColor = CECommon.GetColor(0x00, 0xaa, 0x00);       // コメント

        public static int lrColor = CECommon.GetColor(0x88, 0x88, 0x88);                // 改行
        public static int eofColor = CECommon.GetColor(0x00, 0xaa, 0x00);               // EOF
        public static int wrapColor = CECommon.GetColor(0xff, 0x00, 0xaa);              // 折返し

        public static int currentLineColor = CECommon.GetColor(0xaa, 0xaa, 0xaa);       // カレント位置

        // 行番号
        public static int LineFontColor = CECommon.GetColor(0x00, 0x00, 0x00);          // フォント
        public static int EditLineFontColor = CECommon.GetColor(0xdd, 0x00, 0x00);      // フォント(編集)

        // 範囲選択
        public static int SelectBackColor = CECommon.GetColor(0x55, 0x55, 0x00);        // 背景
        public static int SelectFontColor = CECommon.GetColor(0xff, 0xff, 0xff);        // フォント
        public static int SelectRctFontColor = CECommon.GetColor(0xff, 0xff, 0xff);     // フォント（矩形選択）
        public static int SelectLfBackColor = CECommon.GetColor(0x00, 0xdd, 0x00);      // 背景（改行）

        // 検索
        public static int SearchBackColor = CECommon.GetColor(0xff, 0xff, 0x00);        // 背景
        public static int SearchFontColor = CECommon.GetColor(0x00, 0x00, 0x00);        // フォント

        public static int SearchMarkBackColor = CECommon.GetColor(0xdd, 0xdd, 0x00);        // 背景
        public static int SearchMarkFontColor = CECommon.GetColor(0x00, 0x00, 0x00);        // フォント

        // ルーラー
        public static int CaretColPosColor = CECommon.GetColor(0xff, 0x00, 0x00);       // キャレット位置

        // タブ
        public static int TabBackColor = CECommon.GetColor(0xff, 0xff, 0xff);           // 背景
        public static int TabFontColor = CECommon.GetColor(0x00, 0x00, 0x00);           // フォント

        //public static int TabBackColor = CECommon.GetColor(0xff, 0x8c, 0x00);           // 背景
        //public static int TabFontColor = CECommon.GetColor(0x00, 0x00, 0x00);           // フォント

        //public static int TabHoverColor = CECommon.GetColor(0xff, 0xe5, 0xaf);           // 背景（マウスを重ねる）
        //public static int TabBackColor = CECommon.GetColor(0x00, 0x7a, 0xcc);           // 背景
        //public static int TabFontColor = CECommon.GetColor(0xff, 0xff, 0xff);           // フォント
#endif

#if false // 青ベース
        // 背景色
        public static int BackColor = CECommon.GetColor(0x00, 0x00, 0xff);              // 背景

        // フォント色
        public static int FontColor = CECommon.GetColor(0xff, 0xff, 0xff);              // フォント
        public static int SymbolFontColor = CECommon.GetColor(0xaa, 0xaa, 0xaa);        // タブ、全角空白等
        public static int CommentFontColor = CECommon.GetColor(0x00, 0xaa, 0x00);       // コメント

        public static int lrColor = CECommon.GetColor(0xaa, 0xaa, 0xaa);                // 改行
        public static int eofColor = CECommon.GetColor(0x00, 0xaa, 0x00);               // EOF
        public static int wrapColor = CECommon.GetColor(0xff, 0x00, 0xaa);              // 折返し

        public static int currentLineColor = CECommon.GetColor(0xaa, 0xaa, 0xaa);       // カレント位置

        // 行番号
        public static int LineFontColor = CECommon.GetColor(0xff, 0xff, 0xff);          // フォント
        public static int EditLineFontColor = CECommon.GetColor(0xdd, 0x00, 0x00);      // フォント(編集)

        // 範囲選択
        public static int SelectBackColor = CECommon.GetColor(0xff, 0xff, 0xff);        // 背景
        public static int SelectFontColor = CECommon.GetColor(0x00, 0x00, 0xff);        // フォント
        public static int SelectRctFontColor = CECommon.GetColor(0x00, 0x00, 0xff);     // フォント（矩形選択）

        // ルーラー
        public static int CaretColPosColor = CECommon.GetColor(0xff, 0x00, 0xff);       // キャレット位置

        // タブ
        public static int TabBackColor = CECommon.GetColor(0xff, 0x8c, 0x00);           // 背景
        public static int TabFontColor = CECommon.GetColor(0x00, 0x00, 0x00);           // フォント
        //public static int TabHoverColor = CECommon.GetColor(0xff, 0xe5, 0xaf);           // 背景（マウスを重ねる）
        //public static int TabBackColor = CECommon.GetColor(0x00, 0x7a, 0xcc);           // 背景
        //public static int TabFontColor = CECommon.GetColor(0xff, 0xff, 0xff);           // フォント
#endif
    }
}
