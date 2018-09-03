using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CETextBoxControl
{
    public class CEShareData
    {
        /// <summary>
        /// 1文字の文字幅（ピクセル）
        /// </summary>
        public int m_charWidthPixel;

        /// <summary>
        /// 1文字の文字高（ピクセル）
        /// </summary>
        public int m_charHeightPixel;

        /// <summary>
        /// 折返し位置（ピクセル）
        /// </summary>
        public int m_wrapPositionPixel;

        /// <summary>
        /// 折返し有無
        /// </summary>
        public Boolean m_wrapPositionFlag;

        /// <summary>
        /// カーソルモード（0:通常カーソルモード、1:フリーカーソルモード）
        /// </summary>
        public int m_cursorMode;

        /// <summary>
        /// タブ幅（文字数）
        /// </summary>
        public int m_tabLength;

        /// <summary>
        /// 文字と文字の隙間
        /// </summary>
        public int m_nColumnSpace;

        /// <summary>
        /// 行間
        /// </summary>
        public int m_nRowSpace;

        /// <summary>
        /// フォント
        /// </summary>
        public Font m_font;

        public int m_scrollColSpage;        // スクロール移動差分（この値をあけてスクロールする）
                                            // もし3の場合、、、
                                            // 1234567890|
                                            //        A ←ここに来たら画面がスクロールする

        private static CEShareData _shareDataInstance = new CEShareData();

        /// <summary>
        /// インスタンス取得
        /// </summary>
        /// <returns></returns>
        public static CEShareData GetInstance()
        {
            return _shareDataInstance;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private CEShareData()
        {
            m_charWidthPixel = 0;           // 1文字の文字幅(ピクセル)
            m_charHeightPixel = 0;          // 1文字の文字高(ピクセル)
            m_wrapPositionPixel = 0;        // 折返し位置(ピクセル)
            m_wrapPositionFlag = false;	    // 折返し有無
            m_cursorMode = 0;		        // カーソルモード
            m_tabLength = 4;			    // タブ幅(文字数)
            m_nColumnSpace = 0;             // 文字と文字の隙間
            m_nRowSpace = 0;                // 行間
            m_font = new Font("ＭＳ ゴシック", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));    // フォント
            m_scrollColSpage = 3;           // スクロール移動差分
        }
    }
}
