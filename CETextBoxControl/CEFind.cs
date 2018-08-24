using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CETextBoxControl
{
    public class CEFind
    {
        /// <summary>
        /// 大文字・小文字区別
        /// </summary>
        private Boolean m_ul;

        /// <summary>
        /// 単語単位
        /// </summary>
        private Boolean m_word;

        /// <summary>
        /// 正規表現
        /// </summary>
        private Boolean m_reg;

        /// <summary>
        /// 検索文字列
        /// </summary>
        private string m_findString;
#if false
        private CEDocument m_doc;
#endif
        /// <summary>
        /// コンストラクタ(ドキュメント指定)
        /// </summary>
        /// <param name="doc">ドキュメントクラス</param>
        public CEFind(CEDocument doc)
        {
#if false
            m_doc = doc;
#endif
        }

        public Boolean NextFind(string findstring, int findStartCol, ref int resCol, ref int resLen)
        {
            return NextFind(findstring, findStartCol, false, false, false, ref resCol, ref resLen);
        }

        /// <summary>
        /// 次検索
        /// </summary>
        /// <param name="findstring">[入力]検索文字列</param>
        /// <param name="findStartRow">[入力]検索開始位置(物理行)</param>
        /// <param name="findStartCol">[入力]検索開始位置(物理列)</param>
        /// <param name="ul">[入力]大文字小文字(true:する/false:しない)</param>
        /// <param name="word">[入力]単語単位(true:する/false:しない)</param>
        /// <param name="reg">[入力]正規表現(true:する/false:しない)</param>
        /// <param name="sPos">[出力]検索結果開始位置(物理)</param>
        /// <param name="ePos">[出力]検索結果終了位置(物理)</param>
        /// <returns>true:結果あり / false:結果なし</returns>
        public Boolean NextFind(string findstring, int findStartCol, Boolean ul, Boolean word, Boolean reg, ref int resCol, ref int resLen)
        {
            m_findString = findstring;
            m_ul = ul;
            m_word = word;
            m_reg = reg;

            string str = "";
            int findPoint = -1;

            // 一番下まで検索したら上から再検索
            // 正規表現検索の場合
            if (ul || word || reg)
            {
                string pattern = findstring;

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
                catch (ArgumentException a)
                {
                    findPoint = -1;
                }
            }
            // 正規表現検索でない場合
            else
            {
                findPoint = str.IndexOf(findstring, findStartCol, StringComparison.OrdinalIgnoreCase);
            }

            // 見つからない場合は次の行へ
            if (findPoint == -1)
            {
                findStartCol = 0;
                return false;
            }
            // 見つかった場合はキャレット移動し再描画
            else
            {
                resCol = findPoint;
                resLen = findstring.Length;
                return true;
            }
        }
    }
}
