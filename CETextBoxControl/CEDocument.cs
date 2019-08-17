using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CETextBoxControl
{
    public class CEDocument
    {
        ////////////////////////////////////////////////////////
        // [テキスト情報格納領域]
        // m_textList
        //  [論理行情報]       [物理行情報]
        // +-------------+      +-------------+
        // |SLogicalLine |---+->|SPhysicalLine|
        // +-------------+   |  +-------------+
        // |             |-+ +->|             |
        // +-------------+ | |  +-------------+
        // |             | | +->|             |
        // +-------------+ |    +-------------+
        // |             | |
        // +-------------+ |    +-------------+
        // |             | +-+->|SPhysicalLine|
        // +-------------+   |  +-------------+
        // |             |   +->|             |
        // +-------------+      +-------------+
        ////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////
        // 本クラスはドキュメント（テキスト情報）の操作を行う
        ////////////////////////////////////////////////////////

        /// <summary>
        /// テキスト情報(折り返し情報込)
        /// </summary>
        private List<SLogicalLine> m_textList;
        //private CEList<SLogicalLine> m_textList;

        /// <summary>
        /// タブ幅(ピクセル)
        /// </summary>
        private int m_tabPixel
        {
            get
            {
                return m_ShareData.m_tabLength * m_ShareData.m_charWidthPixel;
            }
        }

        /// <summary>
        /// 共有データ
        /// </summary>
        private CEShareData m_ShareData;

        /// <summary>
        /// メモリデバイスコンテキスト
        /// </summary>
        private IntPtr m_hDrawDC;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CEDocument(IntPtr hdc)
        {
            // 空行（[EOF]）情報を作成
            m_textList = new List<SLogicalLine>();
            //m_textList = new CEList<SLogicalLine>();
            SLogicalLine sld = new SLogicalLine();
            sld.m_physicalLine = new List<SPhysicalLine>();
            sld.m_text = "";
            sld.m_physicalLine.Add(new SPhysicalLine());
            m_textList.Add(sld);

            m_ShareData = CEShareData.GetInstance();

            m_hDrawDC = hdc;

            // 編集フラグクリア
            ClearEditFlg();
        }

        #region 物理行情報操作

        /* -----------------------------------------------------------------------*/
        /* 物理行情報操作                                                         */
        /* -----------------------------------------------------------------------*/

        /// <summary>
        /// 指定した論理行内の物理行数を取得
        /// </summary>
        /// <param name="lRow">論理行</param>
        /// <returns>論理行内の物理行数</returns>
        public int GetPhysicalLineCount(int lRow)
        {
            return m_textList[lRow].m_physicalLine.Count;
        }

        /// <summary>
        /// 指定した論理行内の指定した物理位置の情報を取得
        /// </summary>
        /// <param name="lRow">論理行</param>
        /// <param name="lpRow">論理行内の物理位置</param>
        /// <returns>論理行内の物理行情報</returns>
        public SPhysicalLine GetPhysicalLineData(int lRow, int lpRow)
        {
            return m_textList[lRow].m_physicalLine[lpRow];
        }

        /// <summary>
        /// 指定した論理行内の物理位置情報を取得
        /// </summary>
        /// <param name="lRow">論理行</param>
        /// <returns>論理行内の物理行情報</returns>
        public List<SPhysicalLine> GetPhysicalLineData(int lRow)
        {
            return m_textList[lRow].m_physicalLine;
        }

        /// <summary>
        /// 指定論理行に物理行情報が存在するか
        /// </summary>
        /// <param name="lRow">論理行</param>
        /// <returns>true:存在する / false:存在しない</returns>
        public Boolean IsPhysicalLineData(int lRow)
        {
            return m_textList[lRow].m_physicalLine == null ? false : true;
        }

        /// <summary>
        /// 指定論理行の物理行情報をクリア
        /// </summary>
        /// <param name="lRow"></param>
        public void ClearPhysicalLineData(int lRow)
        {
            m_textList[lRow].m_physicalLine.Clear();
        }

        /// <summary>
        /// 物理行数取得
        /// （物理行：画面に表示されている行）
        /// ※折返しoffであっても4000桁あたりで折り返すので必ず折り返しを考慮しておく必要がある
        /// ★遅い★
        /// </summary>
        /// <returns>物理行数</returns>
        public int GetLineCountP()
        {
            int pNum = 0;

            // ForEach / foreach / for の中で for が一番早いと言われているのでとりあえず for を使っておく
            // しかも、for の条件にm_textList.Countを指定するのではんく、一度int型の num に入れたほうが早いらしい
            // →ループ毎に毎回、Countプロパティを呼び出す分遅くなるらしい

            //m_textList.ForEach(x => pNum += x.m_physicalLine.Count);
            for (int i = 0, num = m_textList.Count; i < num; i++) pNum += m_textList[i].m_physicalLine.Count;

            return pNum;
        }

#endregion

#region 論理行情報操作

        /* -----------------------------------------------------------------------*/
        /* 論理行情報操作                                                         */
        /* -----------------------------------------------------------------------*/

        /// <summary>
        /// 指定した論理行に論理行情報を追加
        /// </summary>
        /// <param name="obj">論理行情報</param>
        public void AddLogicalLineData(SLogicalLine obj)
        {
            m_textList.Add(obj);
        }

        /// <summary>
        /// 指定した論理行情報を削除
        /// </summary>
        /// <param name="lRow">論理行</param>
        public void RemoveLogicalLineData(int lRow)
        {
            m_textList.RemoveAt(lRow);
        }

        /// <summary>
        /// 指定した論理行に論理行情報を挿入
        /// </summary>
        /// <param name="lRow"></param>
        /// <param name="obj"></param>
        public void InsertLogicalLineData(int lRow, SLogicalLine obj)
        {
            m_textList.Insert(lRow, obj);
        }

        /// <summary>
        /// 指定した論理行に論理行情報を設定
        /// </summary>
        /// <param name="lRow">論理行</param>
        public void SetLogicalLineData(int lRow)
        {
            m_textList[lRow] = new SLogicalLine();
        }

        /// <summary>
        /// 論理行数取得
        /// </summary>
        /// <returns>論理行数</returns>
        public int GetLineCountL()
        {
            return m_textList.Count;
        }

        /// <summary>
        /// 指定した論理行に論理行データを設定
        /// </summary>
        /// <param name="row"></param>
        /// <param name="obj"></param>
        public void SetLogicalLine(int row, SLogicalLine obj)
        {
            m_textList[row] = obj;
        }

        /// <summary>
        /// 論理行情報を全てクリア
        /// </summary>
        public void ClearAllLogicalLineData()
        {
            m_textList.Clear();
        }

        /// <summary>
        /// 折返情報更新
        /// </summary>
        /// <param name="row">更新する論理行</param>
        public void UpdateLogicalLineDataL(int lRow)
        {
            // 指定された論理行番号の情報がない、または論理行データがない場合は何もしない
            if (lRow > m_textList.Count - 1) return;

            // 指定した論理行の物理行情報を作り直す
            if (m_textList[lRow].m_physicalLine != null)
            {
                m_textList[lRow].m_physicalLine.Clear(); // 物理行情報があれば削除
            }

            m_textList[lRow] = MakeLineData(lRow, m_textList[lRow].m_text);
        }

        /// <summary>
        /// 折返情報更新
        /// </summary>
        /// <param name="row">更新する論理行</param>
        public void UpdateLogicalLineDataP(int pRow)
        {
            // 物理位置 → 論理位置
            int lRow, lCol;
            PtoLPos(pRow, 0, out lRow, out lCol);

            // 指定された論理行番号の情報がない、または論理行データがない場合は何もしない
            if (lRow > m_textList.Count - 1) return;

            // 指定した論理行の物理行情報を作り直す
            if (m_textList[lRow].m_physicalLine != null)
            {
                m_textList[lRow].m_physicalLine.Clear(); // 物理行情報があれば削除
            }

            m_textList[lRow] = MakeLineData(lRow, m_textList[lRow].m_text);
        }

        /// <summary>
        /// 折返情報更新（範囲行指定）
        /// </summary>
        /// <param name="lRow">開始行</param>
        /// <param name="lLine">更新行数</param>
        public void UpdateRengeLogicalLineData(int lRow, int lLine)
        {
            for (int row = lRow; row < lRow + lLine; row++)
            {
                UpdateLogicalLineDataL(row);
            }
        }

        /// <summary>
        /// 指定した【論理行】に一行追加し、指定した文字列を設定する
        /// </summary>
        /// <param name="lRow">挿入する論理行位置</param>
        public void InsertLineL(int lRow, string text)
        {
            InsertLogicalLineData(lRow, MakeLineData(lRow, text));
        }

        /// <summary>
        /// 指定した論理行の指定した論理列に、指定した文字列を挿入
        /// </summary>
        /// <param name="lRow">論理行</param>
        /// <param name="lCol">論理列</param>
        /// <param name="text">挿入文字列</param>
        public void InsertLineTextL(int lRow, int lCol, string text)
        {
            string nowStr = GetNotLineFeedString(m_textList[lRow].m_text);
            string nowlf = GetLineFeed(m_textList[lRow].m_text);
            int len = nowStr.Length;
            if (lCol > len)
            {
#if false
                // 改行よりも後ろにキャレットがある場合（フリーカーソルや矩形選択）
                if (GetLineFeed(text) != string.Empty)
                {
                    nowlf = string.Empty;
                }
#endif
                // 文字長以上の場所で入力された場合（フリーカーソル）空白で埋める
                string fmt = "{" + 0 + ", " + (-lCol) + "}";
                //m_textList[lLine].m_text = String.Format(fmt, nowStr) + text + nowlf;
                //SetText(lRow, (String.Format(fmt, nowStr) + text + CEConstants.LineFeed/*nowlf*/), false);
                m_textList[lRow].m_text = String.Format(fmt, nowStr) + text + nowlf;
            }
            else
            {
                m_textList[lRow].m_text = m_textList[lRow].m_text.Insert(lCol, text);
            }
        }

        /// <summary>
        /// 指定した物理行の指定した物理列に、指定した文字列を挿入
        /// </summary>
        /// <param name="pRow">物理行</param>
        /// <param name="pCol">物理列</param>
        /// <param name="text">挿入文字列</param>
        /// <returns></returns>
        public void InsertLineTextP(int pRow, int pCol, string text)
        {
#if false // ★本来は必要だが遅いので外しておく
            // 指定行チェック
            if (row > GetLineCountP())
            {
                return;
            }
#endif
            int lLine;
            int pLine;
            getPosition(pRow, out lLine, out pLine);

            // 割り出されたバッファの位置に指定された文字列を挿入する
            if (m_textList[lLine].m_physicalLine.Count != 0)
            {
                int lCol = m_textList[lLine].m_physicalLine[pLine].m_offset + pCol;

                string nowStr = GetNotLineFeedString(m_textList[lLine].m_text);
                string nowlf = GetLineFeed(m_textList[lLine].m_text);
                int len = nowStr.Length;
                if (lCol > len)
                {
                    // 文字長以上の場所で入力された場合（フリーカーソル）空白で埋める
                    string fmt = "{" + 0 + ", " + (-lCol) + "}";
                    //m_textList[lLine].m_text = String.Format(fmt, nowStr) + text + nowlf;
                    SetText(lLine, (String.Format(fmt, nowStr) + text + nowlf), false);
                }
                else
                {
                    //m_textList[lLine].m_text = m_textList[lLine].m_text.Insert(lCol, text);
                    SetText(lLine, m_textList[lLine].m_text.Insert(lCol, text), false);
                }
            }
            else
            {
                //m_textList[lLine].m_text = text;
                SetText(lLine, text, false);
            }

            // 指定した論理行の折り返し情報を更新する
            UpdateLogicalLineDataL(lLine);
        }

#endregion

#region 基本テキスト情報操作

        /* -----------------------------------------------------------------------*/
        /* 基本テキスト情報操作                                                   */
        /* -----------------------------------------------------------------------*/

        /// <summary>
        /// 指定した論理行の最後尾に指定した文字列を追加
        /// </summary>
        /// <param name="lRow">物理行</param>
        /// <param name="text">文字列</param>
        public void AddText(int lRow, string text)
        {
            m_textList[lRow].m_text += text;
            //UpdateLogicalLineData(lRow);    // 折返情報更新
        }

        /// <summary>
        /// 指定した論理行に指定した文字列を設定
        /// </summary>
        /// <param name="lRow">論理行</param>
        /// <param name="text">論理列</param>
        /// <param name="b">折返情報更新有無</param>
        public void SetText(int lRow, string text, Boolean b)
        {
            m_textList[lRow].m_text = text;
            if (b) UpdateLogicalLineDataL(lRow);    // 折返情報更新
        }

        /// <summary>
        /// 指定した物理行の文字列を指定した文字列で置換
        /// </summary>
        /// <param name="lRow">論理行</param>
        /// <param name="text1">文字列</param>
        /// <param name="text2">置換文字列</param>
        /// <returns></returns>
        public string ReplaceText(int lRow, string text1, string text2)
        {
            return m_textList[lRow].m_text.Replace(text1, text2);
        }

        /// <summary>
        /// 指定した論理行の指定した論理列に指定した文字列を挿入
        /// </summary>
        /// <param name="lRow">論理行</param>
        /// <param name="lCol">論理列</param>
        /// <param name="text">文字列</param>
        /// <returns></returns>
        public string InsertText(int lRow, int lCol, string text)
        {
            return m_textList[lRow].m_text.Insert(lCol, text);
        }

        /// <summary>
        /// 指定した論理行の指定した位置から指定した長さ分のテキストを削除
        /// </summary>
        /// <param name="lRow">論理行</param>
        /// <param name="pCol">位置</param>
        /// <param name="len">長さ</param>
        /// <returns>文字列</returns>
        public string RemoveText(int lRow, int lCol, int len)
        {
            return m_textList[lRow].m_text.Remove(lCol, len);
        }

#endregion

#region テキスト情報操作

        /* -----------------------------------------------------------------------*/
        /* テキスト情報操作                                                       */
        /* -----------------------------------------------------------------------*/

        /// <summary>
        /// 指定した論理行の文字列を取得（改行含む）
        /// </summary>
        /// <param name="lRow">論理行</param>
        /// <returns>文字列</returns>
        public string GetLineStringL(int lRow)
        {
            return m_textList[lRow].m_text;
        }

        /// <summary>
        /// 指定した行(物理)の文字列を取得
        /// （改行含む）
        /// 表示行数範囲外を指定された場合、Emptyを返す
        /// </summary>
        /// <param name="row">行</param>
        /// <returns></returns>
        public string GetLineStringP(int row)
        {
            int lLine;
            int pLine;
            getPosition(row, out lLine, out pLine);

            return GetLineStringP(row, lLine, pLine);
        }
        public string GetLineStringP(int row, int lLine, int pLine)
        {
            string str = "";

            if (0 <= row && row < this.GetLineCountP())
            {
                str = m_textList[lLine].m_text.Substring(m_textList[lLine].m_physicalLine[pLine].m_offset, m_textList[lLine].m_physicalLine[pLine].m_length);
                SPhysicalLine spd = m_textList[lLine].m_physicalLine[pLine];
                str = m_textList[lLine].m_text.Substring(spd.m_offset, spd.m_length);
            }

            return str;
        }

        /// <summary>
        /// 指定した行(物理)の文字列を取得
        /// （改行含む）
        /// 表示行数範囲外を指定された場合、Emptyを返す
        /// </summary>
        /// <param name="row">行</param>
        /// <returns></returns>
        public string GetLineStringPEx(int row, int count)
        {
            int lLine;
            int pLine;
            getPosition(row, out lLine, out pLine);

            return GetLineStringPEx(row, count, lLine, pLine);
        }
        public string GetLineStringPEx(int row, int count, int lLine, int pLine)
        {
            string str = "";

            if (0 <= row && row < count)
            {
                //str = m_textList[lLine].m_text.Substring(m_textList[lLine].m_physicalLine[pLine].m_offset, m_textList[lLine].m_physicalLine[pLine].m_length);
                SPhysicalLine spd = m_textList[lLine].m_physicalLine[pLine];
                str = m_textList[lLine].m_text.Substring(spd.m_offset, spd.m_length);
            }

            return str;
        }

        /// <summary>
        /// 指定した行の指定した列までの先頭からの文字列を取得
        /// </summary>
        /// <param name="row">指定行数(物理)</param>
        /// <param name="col">取得位置</param>
        /// <returns>取得文字列(失敗した場合は空文字列)</returns>
        public string GetLeftStringP(int row, int col)
        {
            int lLine;
            int pLine;
            getPosition(row, out lLine, out pLine);

            return GetLeftStringP(row, col, lLine, pLine);
        }
        public string GetLeftStringP(int row, int col, int lLine, int pLine)
        {
            if (lLine > m_textList.Count - 1)
            {
                return "";
            }

            // 文字列の長さを取得
            int offset = m_textList[lLine].m_physicalLine[pLine].m_offset;
            int len = m_textList[lLine].m_physicalLine[pLine].m_length;
            int length = m_textList[lLine].m_text.Substring(offset, len/*m_textList[lLine].m_physicalLine[pLine].m_length*/).Length;
            if (length < col)
            {
                // 取得位置が行末以降の場合は行末までとする（フリーカーソル対応）
                col = length;
            }

            // 文字列を返却
            return m_textList[lLine].m_text.Substring(offset, col);
        }

        /// <summary>
        /// 指定された論理行の指定された論理列より最後までの情報を取得
        /// </summary>
        /// <param name="row">論理行</param>
        /// <param name="col">論理列</param>
        /// <returns></returns>
        public string GetRightStringL(int lRow, int lCol)
        {
            string str = "";

            int len = m_textList[lRow].m_text.Length;

            if ((0 <= lCol) && (lCol < len))
            {
                str = m_textList[lRow].m_text.Substring(lCol, len - lCol);
            }

            return str;
        }

        /// <summary>
        /// 指定した物理行の指定した物理列から終端までの文字列を取得
        /// （改行含む）
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns>取得文字列（指定列が範囲外の場合は空文字）</returns>
        public string GetRightStringP(int row, int col)
        {
            int lLine;
            int pLine;
            getPosition(row, out lLine, out pLine);

            return GetRightStringP(row, col, lLine, pLine);
        }
        public string GetRightStringP(int row, int col, int lLine, int pLine)
        {
#if true
            string str = GetLineStringP(row); // 物理行(改行含)取得
            int len = str.Length;
            if (len > col)
            {
                return str.Substring(col, len - col);
            }
            else
            {
                return "";
            }
#else
            string str = string.Empty;
            int len = GetLineStringP(row).Length;
            if (len > 0)
            {
                int pCol = m_textList[lLine].m_physicalLine[pLine].m_offset + col;
                int pLen = m_textList[lLine].m_physicalLine[pLine].m_length;
                if (pLen > pCol) // フリーカーソル等でカーソルの位置が改行以降の場合は空を返却
                {
                    str = m_textList[lLine].m_text.Substring(pCol, pLen - col);
                }
            }

            // 折り返し情報更新
            //updateLogicalLine(lLine);

            return str;
#endif
        }

        /// <summary>
        /// カラム数を取得
        /// （全角：２、半角：１として計算）
        /// </summary>
        /// <param name="text">カラム数を取得する文字列</param>
        /// <returns>カラム数</returns>
        public int GetColumnLength(string text)
        {
            return GetTextPixel(0, text) / m_ShareData.m_charWidthPixel;
        }

        /// <summary>
        /// 先頭から指定した物理行の指定した物理列までの文字を削除
        /// (物理列の文字は含まない)
        /// </summary>
        /// <param name="row">行</param>
        /// <param name="col">列</param>
        public void DelLeftString(int row, int col)
        {
            int lLine;
            int pLine;
            getPosition(row, out lLine, out pLine);

            DelLeftString(row, col, lLine, pLine);
        }
        public void DelLeftString(int row, int col, int lLine, int pLine)
        {
            int pCol = m_textList[lLine].m_physicalLine[pLine].m_offset;
            //m_textList[lLine].m_text = m_textList[lLine].m_text.Remove(pCol, col);
            SetText(lLine, m_textList[lLine].m_text.Remove(pCol, col), true);

            //m_doc.UpdateLogicalLineData(lLine);
        }

        /// <summary>
        /// 指定した論理行の指定した論理列から終端までの文字を削除
        /// (物理列の文字も含む)
        /// </summary>
        /// <param name="row">行(論理)</param>
        /// <param name="col">列(論理)</param>
        public void DelRightStringL(int row, int col)
        {
            string text = "";

            int len = m_textList[row].m_text.Length;
            if ((0 <= col) && (col < len))
            {
                text = m_textList[row].m_text.Remove(col, len - col);
                m_textList[row] = new SLogicalLine();
                SetText(row, text, true);
            }
        }

        /// <summary>
        /// 指定した物理行の指定した物理列から終端までの文字を削除
        /// (物理列の文字も含む)
        /// </summary>
        /// <param name="row">行</param>
        /// <param name="col">列</param>
        public void DelRightStringP(int row, int col)
        {
            int lLine;
            int pLine;
            getPosition(row, out lLine, out pLine);

            DelRightStringP(row, col, lLine, pLine);
        }
        public void DelRightStringP(int row, int col, int lLine, int pLine)
        {
            int pCol = m_textList[lLine].m_physicalLine[pLine].m_offset + col;
            int pLen = m_textList[lLine].m_physicalLine[pLine].m_length;
            //m_textList[lLine].m_text = m_textList[lLine].m_text.Remove(pCol, pLen - col);
            SetText(lLine,m_textList[lLine].m_text.Remove(pCol, pLen - col), true);

            //m_doc.UpdateLogicalLineData(lLine);
        }

        /// <summary>
        /// 指定行(論理)の指定した列(論理)から指定した長さを削除
        /// </summary>
        /// <param name="row">行(論理)</param>
        /// <param name="col">列(論理)</param>
        /// <param name="len">長さ</param>
        public void DelMidStringL(int lRow, int lCol, int len)
        {
            // 文字列削除
            //m_textList[lRow].m_text = m_textList[lRow].m_text.Remove(lCol, len);
            SetText(lRow, m_textList[lRow].m_text.Remove(lCol, len), true);

            UpdateLogicalLineDataL(lRow);
        }

        /// <summary>
        /// 指定した位置（論理行／論理列）の文字を削除
        /// </summary>
        /// <param name="lRow">論理行</param>
        /// <param name="lCol">論理列</param>
        /// <returns>削除文字</returns>
        public string DeleteCharL(int lRow, int lCol)
        {
            // 削除対象文字取得
            string delStr = m_textList[lRow].m_text.Substring(lCol);

            // 改行確認
            if (IsLineFeedOnly(delStr))
            {
                // 改行の場合

                // 改行削除し、一行下の文字列を結合
                m_textList[lRow].m_text = GetNotLineFeedString(m_textList[lRow].m_text) + m_textList[lRow + 1].m_text;

                // 一行下の行を削除
                m_textList.RemoveAt(lRow + 1);
            }
            else
            {
                // 改行でない場合

                // 現在位置の文字を取得
                delStr = m_textList[lRow].m_text[lCol].ToString();

                // 現在位置の文字を削除
                m_textList[lRow].m_text = m_textList[lRow].m_text.Remove(lCol, 1);
            }

            // 折り返し情報更新
            UpdateLogicalLineDataL(lRow);

            return delStr;
        }

        /// <summary>
        /// 指定した位置（物理行／物理列）の文字を削除
        /// </summary>
        /// <param name="pRow">物理行</param>
        /// <param name="pCol">物理列</param>
        /// <returns>削除文字</returns>
        public string DeleteCharP(int pRow, int pCol)
        {
            int lRow = -1; // 論理行
            int lCol = -1; // 論理列
            PtoLPos(pRow, pCol, out lRow, out lCol);
            string retStr = DeleteCharL(lRow, lCol);

            return retStr;
#if false // 論理行削除の処理(DelStringL)を使用するのでコメント
            int lLine;
            int pLine;
            getPosition(row, out lLine, out pLine);

            return DelStringP(row, col, lLine, pLine);
#endif
        }
#if false
        public string DelStringP(int row, int col, int lLine, int pLine)
        {
            string delStr = string.Empty;

            int lCol = m_textList[lLine].m_physicalLine[pLine].m_offset + col;
            int length = m_textList[lLine].m_physicalLine[pLine].m_length - col;
            string lfText = m_textList[lLine].m_text.Substring(lCol);
            if ((length == 2 && lfText[length - 2] == '\r' && lfText[length - 1] == '\n') ||
                (length == 1 && (lfText[length - 1] == '\n' || lfText[length - 1] == '\r')))
            {
                // 改行の場合
                SetText(lLine, m_textList[row].m_text.Replace(lfText, ""), true);
                string str2 = m_textList[lLine + 1].m_text;;
                m_textList.RemoveAt(lLine + 1);
                int r, c;
                PtoLPos(row, col, out r, out c); // 物理位置→論理位置
                m_textList[r].m_text += str2;
                delStr = lfText;
            }
            else
            {
                // 改行でない場合
                if (m_textList[lLine].m_text.Length != lCol)    // EOFの場合は何もしない
                {
                    delStr = m_textList[lLine].m_text.Substring(lCol, 1);
                    //m_textList[lLine].m_text = m_textList[lLine].m_text.Remove(lCol, 1);
                    SetText(lLine, m_textList[lLine].m_text.Remove(lCol, 1), true);
                }
            }

            UpdateLogicalLineData(lLine);

            return delStr;
        }
#endif











#endregion

#region 選択範囲操作

        /* -----------------------------------------------------------------------*/
        /* 選択範囲操作                                                           */
        /* -----------------------------------------------------------------------*/

        /// <summary>
        /// 通常選択範囲を文字列として返却
        /// </summary>
        /// <param name="sRow"></param>
        /// <param name="sCol"></param>
        /// <param name="eRow"></param>
        /// <param name="eCol"></param>
        /// <returns></returns>
        public string GetRangeString(Point sRng, Point eRng)
        {
            int cCol = /*m_*/sRng.X;
            int cRow = /*m_*/sRng.Y;
            StringBuilder sss = new StringBuilder();

            // 複数行にまたぐか？
            if (/*m_*/sRng.Y != /*m_*/eRng.Y)
            {
                // 複数行の場合
                int procNum = /*m_*/eRng.Y - /*m_*/sRng.Y + 1; // 処理する行数

                for (int idx = 0; idx < procNum; idx++)
                {
                    if (cRow == /*m_*/sRng.Y)
                    {
                        // 先頭行
                        sss.Append(GetRightStringP(cRow, cCol));
                        cRow++;
                    }
                    else if (cRow == /*m_*/eRng.Y)
                    {
                        // 最終行
                        sss.Append(GetLeftStringP(cRow, /*m_*/eRng.X));
                    }
                    else
                    {
                        // 先頭行でも最終行でもない
                        sss.Append(GetLineStringP(cRow));
                        cRow++;
                    }
                }
            }
            else
            {
                // 単一行の場合
                int lLine;
                int pLine;
                getPosition(sRng.Y, out lLine, out pLine);

                int offset = m_textList[lLine].m_physicalLine[pLine].m_offset;
                int length = m_textList[lLine].m_physicalLine[pLine].m_length;
                string s1 = GetNotLineFeedString(m_textList[lLine].m_text);
                if (s1.Length < sRng.X)
                {
                    // 取得開始位置が行末以降の場合は空文字とする（フリーカーソル対応）
                    return "";
                }
                if (s1.Length < eRng.X)
                {
                    // 取得終了位置が行末以降の場合は行末までとする（フリーカーソル対応）
                    eRng.X = s1.Length;
                }
                sss.Append(s1.Substring(offset + sRng.X, eRng.X - sRng.X));
            }
            return sss.ToString();
        }

        /// <summary>
        /// 矩形選択範囲を文字列として返却
        /// </summary>
        /// <param name="sRow"></param>
        /// <param name="sCol"></param>
        /// <param name="eRow"></param>
        /// <param name="eCol"></param>
        /// <returns></returns>
        public string GetRectangleString(Point sRng, Point eRng, int curCaretPos, int rectCaretPos, int viewLeftCol, ref int gsIdx, ref int geIdx)
        {
            // 選択範囲の文字列格納用ストリングバッファ
            StringBuilder rctStrBuff = new StringBuilder();

            // 処理する行数
            int procNum = eRng.Y - sRng.Y + 1;

#if false
            // 行位置取得(sRngとeRngがひっくり返ってる場合があるため)
            int sPosY = -1;
            int ePosY = -1;
            if (sRng.Y < eRng.Y)
            {
                sPosY = sRng.Y;
                ePosY = eRng.Y;
            }
            else
            {
                sPosY = eRng.Y;
                ePosY = sRng.Y;
            }

            // 列位置取得(sRngとeRngがひっくり返ってる場合があるため)
            int sPosX = -1;
            int ePosX = -1;
            if (sRng.X < eRng.X)
            {
                sPosX = sRng.X;
                ePosX = eRng.X;
            }
            else
            {
                sPosX = eRng.X;
                ePosX = sRng.X;
            }
#endif
#if false
            int lLine;
            int pLine;
#endif
            string lineStr = "";
            for (int idx = sRng.Y; idx <= eRng.Y; idx++)
            {
#if false
                // 指定したピクセルの位置(物理)を取得
                // ※クラス変数の gsIdx, geIdx に設定
                GetRectMovePosV(idx, m_rectangleCaretColumnPosPixel, m_currentCaretColumnPosPixel);
#endif
#if false
                // 指定した行(物理)から折返しバッファの位置を取得
                getPosition(idx, out lLine, out pLine);
#endif
                //lineStr = GetLineStringP(idx);
                // 指定した行(物理)から改行を除いた文字列を取得
                lineStr = GetNotLineFeedStringP(idx);
#if true
                // 指定したピクセルの位置(物理)を取得
                // ※クラス変数の gsIdx, geIdx に設定
                GetRectMovePosV(idx, rectCaretPos, curCaretPos, lineStr, viewLeftCol, out gsIdx, out geIdx);
#endif

                if (lineStr.Length < geIdx)
                {
                    geIdx = lineStr.Length;
                }
                if (lineStr.Length > gsIdx)
                {
                    rctStrBuff.Append(lineStr.Substring(gsIdx, geIdx - gsIdx));
                }
                rctStrBuff.Append("\r\n");
            }

            return rctStrBuff.ToString();
        }

        /// <summary>
        /// [矩形選択用]指定した位置(ピクセル)の列位置(物理)を取得
        /// </summary>
        /// <param name="row">[入力]行(物理)</param>
        /// <param name="sColPixel">[入力]選択位置１(ピクセル)(文字列の一番左が0)</param>
        /// <param name="eColPixel">[入力]選択位置２(ピクセル)(文字列の一番左が0)</param>
        public void GetRectMovePosV(int row, int sColPixel, int eColPixel, string objStr, int viewLeftCol, out int gsIdx, out int geIdx)
        {
            Size sz = Size.Empty;
            int len = 0;

            gsIdx = -1;
            geIdx = -1;

            // 文字列が空の場合は位置は0,0にして終了
            if (objStr == "")
            {
                gsIdx = 0;
                geIdx = 0;
                return;
            }

            // 選択開始と終了の位置入替
            if (sColPixel > eColPixel)
            {
                int tmp = sColPixel;
                sColPixel = eColPixel;
                eColPixel = tmp;
            }

            // 矩形モード専用なのため、フリーカーソルモードでない場合は何もしない。
            if (m_ShareData.m_cursorMode != 1)
            {
                return;
            }

            // 画面に表示されていないサイズ（左側）
            int hiddenPixelSize = viewLeftCol * m_ShareData.m_charWidthPixel;

            // タブ幅（ピクセル）
            int tabWidthPixel = m_ShareData.m_tabLength * m_ShareData.m_charWidthPixel;

            // 折り返し位置までの文字数を取得(改行以降は半角文字として計算)
            int strPixel = GetTextPixel(m_ShareData.m_nColumnSpace, objStr);
            len = objStr.Length + (m_ShareData.m_wrapPositionPixel - strPixel) / m_ShareData.m_charWidthPixel;

            //Size proposedSize = new Size(int.MaxValue, int.MaxValue);
            //Graphics g = Graphics.FromHdc(m_hDrawDC);

            StringBuilder str = new StringBuilder();

            int posX = 0/*-hiddenPixelSize*/;
            //sColPixel += hiddenPixelSize;
            //eColPixel += hiddenPixelSize;
#if false
            int pixelIdx = 0;
            if (sColPixel >= 0)
            {
                pixelIdx = sColPixel / m_charWidthPixel;
            }
#endif
            for (int idx = 0/*pixelIdx*/; idx < len; idx++)
            {
                if (gsIdx != -1 && geIdx != -1)
                {
                    break;
                }
                if ((gsIdx == -1) && (/*(*/sColPixel/* - hiddenPixelSize)*/ <= /*(*/posX/* - hiddenPixelSize)*/))
                {
                    gsIdx = idx;
                }
                if ((geIdx == -1) && (/*(*/eColPixel/* - hiddenPixelSize)*/ <= /*(*/posX/* - hiddenPixelSize)*/))
                {
                    geIdx = idx;
                }

                str.Clear();
                //string str = string.Empty;
                if (objStr.Length <= idx)
                {
                    str.Append(" ");
                    //str = " ";
                }
                else
                {
                    str.Append(objStr[idx]);
                    //str = objStr[idx].ToString();
                }

                // タブの場合
                if (str.ToString() == "\t")
                {
                    posX += tabWidthPixel - ((posX + tabWidthPixel) % tabWidthPixel);
                }
                // 通常文字（タブ以外）
                else
                {
                    //sz = TextRenderer.MeasureText(g, str, m_Font/*this.Font*/, proposedSize, TextFormatFlags.NoPadding);
                    CEWin32Api.GetTextExtentPoint32(m_hDrawDC, str.ToString(), str.Length, out sz);
                    posX += sz.Width + m_ShareData.m_nColumnSpace;
                }
            }
            //posX = posX - hiddenPixelSize;
        }

        /// <summary>
        /// 指定(論理位置)した範囲の文字列を削除する
        /// </summary>
        /// <param name="sRow">削除開始行(論理)</param>
        /// <param name="sCol">削除開始列(論理)</param>
        /// <param name="eRow">削除終了行(論理)</param>
        /// <param name="eCol">削除終了列(論理)</param>
        public void DelRangeStringL(int sRowL, int sColL, int eRowL, int eColL)
        {
            // 削除位置が "改行以降" かつ "最終行でない" 場合、
            // 削除位置を次行の先頭に再設定（フリーカーソル対応）
            // 削除開始位置
            int len = -1;
            len = m_textList[sRowL].m_text.Length;
            if ((sColL > len - 1) && (sRowL != m_textList.Count - 1))
            {
                sRowL++;
                sColL = 0;
            }
            // 削除終了位置
            len = m_textList[eRowL].m_text.Length;
            if ((eColL > len - 1) && (eRowL != m_textList.Count - 1))
            {
                eRowL++;
                eColL = 0;
            }

            // 処理中の行
            int cRowL = sRowL;

            // 複数行 or 単一行？
            if (sRowL != eRowL)
            {
                // 複数行

                // 削除行数取得
                int procNum = eRowL - sRowL + 1;

                // 最終行の削除後文字列
                string lastString = "";

                for (int idx = 0; idx < procNum; idx++)
                {
                    if (cRowL == sRowL)
                    {
                        // 先頭行
                        DelRightStringL(cRowL, sColL);
                        cRowL++;
                    }
                    else if (cRowL == eRowL)
                    {
                        // 最終行
                        lastString = GetRightStringL(cRowL, eColL);
                        //m_textList.RemoveAt(cRowL);
                        m_textList.RemoveAt(cRowL);
                    }
                    else
                    {
                        // 途中行
                        //m_textList.RemoveAt(cRowL);
                        m_textList.RemoveAt(cRowL);
                        eRowL--;
                    }
                    UpdateLogicalLineDataL(cRowL);
                }
                // 先頭行と最終行の文字を結合
                m_textList[sRowL].m_text += lastString;

                UpdateLogicalLineDataL(sRowL);
            }
            else
            {
                // 単一行
#if false
                // 取得開始位置が行末以降の場合は削除処理はしない
                if ((m_textList[cRowL].m_text.Length - 1) < sColL )
                {
                    return;
                }

                // 取得終了位置が行末以降の場合は行末を削除位置とする
                if ((m_textList[cRowL].m_text.Length - 1) < eColL)
                {
                    eColL = GetNotLineFeedString(m_textList[cRowL].m_text).Length;
                }
#endif
                // 文字列削除
                DelMidStringL(cRowL, sColL, eColL - sColL);
                //m_textList[cRowL].m_text = m_textList[cRowL].m_text.Remove(sColL, eColL - sColL);

                // 折返し情報更新
                UpdateLogicalLineDataL(sRowL);
            }
        }

        /// <summary>
        /// 指定(物理位置)した範囲の文字列を削除する
        /// </summary>
        /// <param name="sRow">削除開始行(物理)</param>
        /// <param name="sCol">削除開始列(物理)</param>
        /// <param name="eRow">削除終了行(物理)</param>
        /// <param name="eCol">削除終了列(物理)</param>
        public void DelRangeStringP(int sRowP, int sColP, int eRowP, int eColP)
        {
            // 通常選択範囲の 物理位置 → 論理位置 変換
            int sRowL, sColL, eRowL, eColL;
            PtoLPos(sRowP, sColP, out sRowL, out sColL);
            PtoLPos(eRowP, eColP, out eRowL, out eColL);

            // 指定(論理位置)した範囲の文字列を削除する
            DelRangeStringL(sRowL, sColL, eRowL, eColL);
        }

#endregion

#region その他操作

        /* -----------------------------------------------------------------------*/
        /* その他操作                                                             */
        /* -----------------------------------------------------------------------*/

        /// <summary>
        /// 物理位置から論理位置に変換
        /// </summary>
        public void PtoLPos(int pRow, int pCol, out int lRow, out int lCol)
        {
            int lLine;
            int pLine;
            getPosition(pRow, out lLine, out pLine);

            PtoLPos(pRow, pCol, out lRow, out lCol, lLine, pLine);
        }
        public void PtoLPos(int pRow, int pCol, out int lRow, out int lCol, int lLine, int pLine)
        {
            lRow = -1;
            lCol = -1;

            lRow = lLine;
            lCol = m_textList[lLine].m_physicalLine[pLine].m_offset + pCol;
        }

        /// <summary>
        /// 論理位置から物理位置に変換
        /// （ここも★うんこ★みたいなんで修正したい）
        /// </summary>
        public void LtoPPos(int lRow, int lCol, out int pRow, out int pCol)
        {
            pRow = -1;
            pCol = -1;

            int lpLine = 0;     // 処理中論理行
            int ppLine = 0;     // 処理中物理行
            int offset = 0;

            for (int i = 0; i < m_textList.Count; i++)
            {
                string text = m_textList[i].m_text;

                while (true)
                {
                    // 折り返し位置までの文字数と文字幅(ピクセル)取得
                    int pixel;
                    int length = getWrapPosLen(text, out pixel);
                    if (lpLine >= lRow && offset + length >= lCol)
                    {
                        pRow = ppLine;
                        pCol = lCol - offset;
                        return;
                    }
                    ppLine++;
                    // これ以上文字が続かない場合は終わり
                    if (text.Substring(length) == "")
                    {
                        break;
                    }
                    text = text.Substring(length);
                    offset += length;
                }

                lpLine++;
                offset = 0;
                //pCol = lCol - offset;
            }
        }

        /// <summary>
        /// 物理行情報作成
        /// </summary>
        /// <param name="text">物理行を作成するテキスト</param>
        /// <returns>物理行情報</returns>
        public SLogicalLine MakeLineData(int line, string text)
        {
            SLogicalLine sld = new SLogicalLine();
            sld.m_physicalLine = new List<SPhysicalLine>();

            // 論理テキストデータ設定
            sld.m_text = text;

            // 空文字の場合、空の物理行情報を設定し終了
            if (text == "")
            {
                sld.m_physicalLine.Add(new SPhysicalLine());
                //sld.m_lineNum = sld.m_physicalLine.Count;
                return sld;
            }

            // 折り返しなし
            if (!m_ShareData.m_wrapPositionFlag)
            {
                sld.m_physicalLine.Add(new SPhysicalLine(line, 0, text.Length, GetLineFeed(text), false));
            }
            // 折り返しあり
            else
            {
                // オフセット初期化
                int offset = 0;

                // 物理行設定
                while (true)
                {
                    // 折り返し位置までの文字数と文字列幅(ピクセル)取得
                    int pixel;
                    int length = getWrapPosLen(text, out pixel);
                    // 物理行の追加
                    sld.m_physicalLine.Add(new SPhysicalLine(line, offset, length, "", true));
                    // これ以上文字が続かない場合は終わり
                    if (text.Substring(length) == "")
                    {
                        // 折返しギリギリ、かつ改行コード終了でない場合は以下のようになる
                        //   --------------
                        //   12345678901234<      ←1行目(物理行)
                        //   56789012345678[EOF]  ←2行目(物理行)
                        //   --------------
                        // 改行文字と折り返しなしを設定
                        sld.m_physicalLine[sld.m_physicalLine.Count - 1].m_lfStr = GetLineFeed(text);
                        sld.m_physicalLine[sld.m_physicalLine.Count - 1].m_wrap = false;
                        break;
                    }
                    text = text.Substring(length);
                    offset += length;
                }
            }
            // 物理行情報返却
            return sld;
        }

        /// <summary>
        /// 折り返し位置までの文字数取得
        /// </summary>
        /// <param name="text">折返し文字数</param>
        /// <param name="pixel">折返し文字サイズ(ピクセル)</param>
        /// <returns>折り返し位置までの文字数</returns>
        public int getWrapPosLen(string text, out int pixel)
        {
            Size sz;
            int posX = 0;
            int cPos = 0;
            int idx = 0;
            string ch = "";
            for (; idx < text.Length; idx++)
            {
                ch = text[idx].ToString();

                cPos = posX;

                // CRLF
                if (text.Length >= 2 && ch == "\r" && text[idx + 1].ToString() == "\n")
                {
                    posX += m_ShareData.m_charWidthPixel/* * 2*/;
                    if (m_ShareData.m_wrapPositionPixel >= posX)
                    {
                        idx++;
                    }
                }
                // CR or LF
                else if (text.Length >= 1 && (ch == "\n" || ch == "\r"))
                {
                    posX += m_ShareData.m_charWidthPixel;
                }
                // TAB
                else if (ch == "\t")
                {
                    posX += m_tabPixel - ((posX + m_tabPixel) % m_tabPixel);
                }
                // misc
                else
                {
                    CEWin32Api.GetTextExtentPoint32(m_hDrawDC, ch, ch.Length, out sz);
                    posX += sz.Width;
                }
                if (m_ShareData.m_wrapPositionPixel < posX) break;
            }

            // 文字サイズ(ピクセル)
            if (m_ShareData.m_wrapPositionPixel < posX)
            {
                pixel = cPos;
            }
            else
            {
                pixel = posX;
            }

            return idx;
        }

        /// <summary>
        /// 指定された物理行からバッファの位置を特定
        /// この方法も結構★うんこ★なので変えたい
        /// </summary>
        /// <param name="row">物理行</param>
        /// <param name="lLine">論理行</param>
        /// <param name="pLine">折り返し位置</param>
        /// <returns>true:行あり／false:行なし</returns>
        static int svRow = -1;
        static int svlLine = -1;
        static int svpLine = -1;
        public Boolean getPosition(int row, out int lLine, out int pLine)
        {
            lLine = 0;
            pLine = 0;

            //chLine = 0;

            if (0 > row)
            {
                return false;
            }

            // 折返し表示でない場合
            if (!m_ShareData.m_wrapPositionFlag)
            {
                lLine = row; // 折り返しなしなので、物理行＝論理行
                pLine = 0;
                return true;
            }

            // 保存中の物理行と同一の場合は保存している値を返却して終了
            // 注）getPositionをコールするタイミングによって挙動がおかしくなる可能性あり
            if (row == svRow)
            {
                lLine = svlLine;
                pLine = svpLine;
                return true;
            }

            svRow = -1;
            svlLine = -1;
            svpLine = -1;

            // 全物理行数取得
            int maxLine = GetLineCountP();      // この処理に時間が掛かる。あらかじめ持っていられたら良いが。

            // 検索
            if (row < (maxLine / 2))
            {
                // 上から検索
                int len = m_textList.Count;
                while (lLine < len)
                {
                    pLine += m_textList[lLine].m_physicalLine.Count;
                    if (row < pLine)
                    {
                        pLine = m_textList[lLine].m_physicalLine.Count - (pLine - row);
                        svRow = row;
                        svlLine = lLine;
                        svpLine = pLine;
                        return true;
                    }
                    lLine++;
                }
            }
            else
            {
                // 下から検索
                pLine = maxLine/*GetLineCountP()*/ - 1;
                lLine = m_textList.Count - 1;
                while (lLine >= 0)
                {
                    pLine -= m_textList[lLine].m_physicalLine.Count;
                    if (row > pLine)
                    {
                        pLine = (row - pLine) - 1;
                        svRow = row;
                        svlLine = lLine;
                        svpLine = pLine;
                        return true;
                    }
                    lLine--;
                }
            }

            // ヒットしなかった（通常はありえないはず）
            return false;
        }

        /// <summary>
        /// 指定した文字列のピクセル数を取得
        /// 文字間隔も考慮済み
        /// </summary>
        /// <param name="handle">ハンドル</param>
        /// <param name="f">フォント</param>
        /// <param name="cs">文字間(ピクセル単位)</param>
        /// <param name="s">ピクセル数を取得する文字列</param>
        /// <returns>指定された文字列のピクセル数</returns>
        public int GetTextPixel(int cs, string s)
        {
            int bLen = 0;
            Size sz = Size.Empty;
            string wStr = "";
            int tabWidthPixel = m_ShareData.m_tabLength * m_ShareData.m_charWidthPixel;

            //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //sw.Start();

#if false   // 高速化失敗(纏めてGetTextExtentPoint32()実行しようとしたら、こっちの方が遅かった...)
            int idx = 0;
            int bLen2 = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i].ToString() == "\t")
                {
                    // タブを空白に置き換え
                    int tabNum = m_tabLength - (idx % m_tabLength);
                    for (int iii = 0; iii < tabNum; iii++)
                    {
                        wStr += " ";
                        idx++;
                    }
                }
                else
                {
                    wStr += s[i].ToString();
                    idx++;
                }
            }
            CEWin32Api.GetTextExtentPoint32(m_hDrawDC, wStr, wStr.Length, out sz);
            bLen2 = sz.Width;
            Console.Write("bLen2:" + bLen2 + " / ");
#else
            wStr = "";

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i].ToString() == "\t")
                {
                    CEWin32Api.GetTextExtentPoint32(m_hDrawDC, wStr, wStr.Length, out sz);
                    bLen += sz.Width;
                    bLen += tabWidthPixel - ((bLen + tabWidthPixel) % tabWidthPixel);
                    wStr = "";
                }
                else
                {
                    wStr += s[i].ToString();
                }
            }
            if (wStr != "")
            {
                CEWin32Api.GetTextExtentPoint32(m_hDrawDC, wStr, wStr.Length, out sz);
                bLen += sz.Width;
            }

            //Console.Write("bLen:" + bLen + " / ");
#endif
            //sw.Stop();
            //Console.WriteLine("time:" + sw.Elapsed);

            // 行間分足す
            // タブの間隔を足す
            return bLen/*(bLen * (cs + m_charWidthPixel)) + tabPixelWidth*/;
        }
#if false
        /// <summary>
        /// ★作成中★
        /// ★折りけし指定での矩形情報貼り付けをどのような仕様にするか要検討★
        /// ★サクラエディター、秀丸、Visual Studioで貼り付け後の表示が全て違うため
        /// (矩形選択用)複数行に跨るテキストデータを挿入
        /// </summary>
        /// <param name="sx">挿入する物理位置（桁）</param>
        /// <param name="sy">挿入する物理位置（行）</param>
        /// <param name="str">挿入するテキスト</param>
        /// <param name="ex">挿入したテキストの右下の物理位置（桁）</param>
        /// <param name="ey">挿入したテキストの右下の物理位置（行）</param>
        private void MultiLineRectangleInsertText(int sx, int sy, string str, out int ex, out int ey)
        {
            // 改行を区切り文字として分割
            string[] parts;
            parts = Strings.Split(str, CEConstants.LineFeed, -1, CompareMethod.Binary);

#if true
            // 分割した配列に改行を付ける
            for (int idx = 0; idx < parts.Count() - 1; idx++)
            {
                parts[idx] = parts[idx] + CEConstants.LineFeed;
            }
#endif
            ex = -1;
            ey = -1;

            // 列情報より開始・終了位置取得(ピクセル)
            int sPixel = GetTextPixel(m_nColumnSpace, GetLeftStringP(sy, sx));

            // データを確認する
            int rRow = m_caretStrBuf.Y;
            int rCol = -1;
            foreach (string s in parts)
            {
#if false
                if (s == "")
                {
                    // 処理する文字列データが空の場合は終了
                    break;
                }
#endif
                int posX;
                int idx;
                // 指定した行(物理)の列(ピクセル)から列(物理・ピクセル)を取得する
                //GetMovePosV(rRow, m_curCaretColPosPixel, out posX, out idx);
                //int hiddenPixelSize = m_viewLeftColumn * m_charWidthPixel;
                if (GetMovePosV(rRow, sPixel, out posX, out idx))
                {
                    // 途中(EOFのみの行以外)に挿入の場合
                    string ss = m_doc.GetNotLineFeedString(s);
                    InsertLineText(rRow, idx, ss);
                }
                else
                {
                    // EOFのみの行に挿入の場合
                    MultiLineInsertText(sx, rRow/*sy*/, s/* + CEConstants.LineFeed*/);
                    Console.WriteLine("EOFのみの行に挿入 sx:" + sx + " / sy:" + sy + " s:" + s + " / rRow:" + rRow);
                }

                rRow++;
                rCol = idx + s.Length;
            }

            ey = rRow - 1;
            ex = rCol;

        }

        /// <summary>
        /// (通常選択用)複数行に跨るテキストデータを挿入
        /// </summary>
        /// <param name="x">挿入する物理位置（桁）</param>
        /// <param name="y">挿入する物理位置（行）</param>
        /// <param name="str">挿入するテキスト</param>
        private void MultiLineInsertText(int x, int y, string str)
        {
            // 分割
            string[] parts;
            parts = Strings.Split(str, CEConstants.LineFeed, -1, CompareMethod.Binary);

            // 分割した配列に改行を付ける
            for (int idx = 0; idx < parts.Count() - 1; idx++)
            {
                parts[idx] = parts[idx] + CEConstants.LineFeed;
            }

            // データを確認する
            foreach (string s in parts)
            {
                if (s == "")
                {
                    // 処理する文字列データが空の場合は終了
                    break;
                }
                string lfs = GetLineFeed(s);
                if (lfs == "")
                {
                    // 改行なし
                    InsertLineText(y, x, s);
                    int mlen = s.Length;
                    CursorKeyH(mlen);
                }
                else
                {
                    // 改行あり
                    int lRow, lCol;
                    PtoLPos(y, x, out lRow, out lCol); // 物理位置→論理位置
                    string sss = GetRightStringL(lRow, lCol);

                    // 高速化 updateLogicalLineをInsertLineL()、DelRightStringL()、AddText()の中でいちいちやってたので処理を外だしにして最後にupdateLogicalLine()を実行するようにした。
                    // 微妙にしか速くなってない。
#if false
          
                    InsertLineL(lRow + 1, sss);     // 一行下に追加
                    DelRightStringL(lRow, lCol);    // キャレットの位置より右側を削除
                    AddText(lRow, s);               // キャレットの位置に追加

#else
                    //m_textList.Insert(lRow + 1, MakeLineData(lRow + 1, sss));
                    InsertLineL(lRow + 1, sss);

                    //int len2 = GetLineStringL(lRow).Length;
                    //string text = m_textList[lRow].m_text.Remove(lCol, len2 - lCol);
                    //m_textList[lRow] = new SLogicalLine();
                    //m_textList[lRow].m_text = text;
                    DelRightStringL(lRow, lCol);

                    //m_textList[lRow].m_text += s;
                    AddText(lRow, s);

                    //m_doc.UpdateLogicalLineData(lRow);
#endif
#if false
                    string sss = m_ee/*doc*/.GetRightStringL(y, x); // 今いる行(物理)のキャレットから右終端(論理)までのテキストデータを取得
                    m_ee/*doc*/.InsertLineL(y, sss); // 一行下に挿入(論理)
                    m_ee/*doc*/.DelRightStringL(y, x);   // 今いる行(物理)のキャレットから右終端(論理)までのテキストデータを削除
                    m_ee/*doc*/.AddText(y, s);  // 挿入するテキストデータを今いる行(物理)の後ろに追加する
#endif
                    // 挿入した文字列の最後まで移動する。
                    // （GetCaretPositionH()で移動数を指定してもうまく移動してくれないので1文字ずつ移動）
                    int len = GetNotLineFeedString(s).Length;
                    if (GetLineFeed(s).Length != 0)
                    {
                        len++; // 改行は1文字と計算
                    }
                    // 改行はカーソルモードを通常モードとして処理
                    int bkCursoMode = m_cursorMode;
                    m_cursorMode = 0;
                    GetCaretPositionH(len);
                    m_cursorMode = bkCursoMode;
#if false
                    for (int iii = 0; iii < len; iii++)
                    {
                        GetCaretPositionH(1);
                    }

                    GetCaretPositionH(-x);  // キャレットを先頭の、
                    int down = m_ee.m_textList[lRow].m_lgLine.Count;
                    GetCaretPositionV(down);   // 一行下に移動
                    y += down;
#endif
                    y = m_caretStrBuf.Y;
                    x = m_caretStrBuf.X;
                }
            }
        }
#endif
#endregion

#region 改行関連操作

        /* -----------------------------------------------------------------------*/
        /* 改行関連操作                                                           */
        /* -----------------------------------------------------------------------*/

        /// <summary>
        /// 改行文字のみかチェック
        /// </summary>
        /// <param name="str"></param>
        /// <returns>true:改行のみ / false:改行のでない</returns>
        public Boolean IsLineFeedOnly(string str)
        {
            int len = str.Length;
            if (len == 2 && str[len - 2] == '\r' && str[len - 1] == '\n')
                return true;
            if (len == 1 && (str[len - 1] == '\n' || str[len - 1] == '\r'))
                return true;
            return false;
        }

        /// <summary>
        /// 表示文字列から改行コードを取得
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public string GetLineFeed(string str)
        {
            int len = str.Length;
            if (len >= 2 && str[len - 2] == '\r' && str[len - 1] == '\n')
                return str.Substring(len - 2);
            if (len >= 1 && (str[len - 1] == '\n' || str[len - 1] == '\r'))
                return str.Substring(len - 1);
            return "";
        }

        /// <summary>
        /// 指定した文字列から改行を除いた文字列を返す
        /// </summary>
        /// <param name="ptr">改行を除く文字列</param>
        /// <returns></returns>
        public string GetNotLineFeedString(string ptr)
        {
            int length = ptr.Length;

            if (length >= 2 && ptr[length - 2] == '\r' && ptr[length - 1] == '\n')
                return ptr.Remove(length - 2);
            if (length >= 1 && (ptr[length - 1] == '\n' || ptr[length - 1] == '\r'))
                return ptr.Remove(length - 1);
            return ptr;
        }

        /// <summary>
        /// 指定した行(物理)の文字列から改行を除いた文字列を返す
        /// </summary>
        /// <param name="row">改行を除く文字列の行</param>
        /// <returns></returns>
        public string GetNotLineFeedStringP(int row)
        {
            string ptr = GetLineStringP(row);

            int length = ptr.Length;

            if (length >= 2 && ptr[length - 2] == '\r' && ptr[length - 1] == '\n')
                return ptr.Remove(length - 2);
            if (length >= 1 && (ptr[length - 1] == '\n' || ptr[length - 1] == '\r'))
                return ptr.Remove(length - 1);
            return ptr;
        }
        public string GetNotLineFeedStringP(int row, int lLine, int pLine)
        {
            string ptr = GetLineStringP(row, lLine, pLine);

            int length = ptr.Length;

            if (length >= 2 && ptr[length - 2] == '\r' && ptr[length - 1] == '\n')
                return ptr.Remove(length - 2);
            if (length >= 1 && (ptr[length - 1] == '\n' || ptr[length - 1] == '\r'))
                return ptr.Remove(length - 1);
            return ptr;
        }

#endregion
#region 編集

        /* -----------------------------------------------------------------------*/
        /* 編集関連                                                               */
        /* -----------------------------------------------------------------------*/

        /// <summary>
        /// 編集中フラグクリア
        /// </summary>
        public void ClearEditFlg()
        {
            foreach (SLogicalLine cm in m_textList)
            {
                cm.editFlg = false;
            }
        }

        /// <summary>
        /// 編集状態取得
        /// 指定した行が編集されているか
        /// </summary>
        /// <param name="line">指定論理行</param>
        /// <returns></returns>
        public Boolean GetEditStatus(int line)
        {
            return m_textList[line].editFlg;
        }

        /// <summary>
        /// 編集状態取得
        /// </summary>
        /// <returns></returns>
        public Boolean GetEditStatus()
        {
            // foreach より for の方が早いらしい(僅かな高速化)
            for (int i = 0, num = m_textList.Count; i < num; i++)
            {
                if (m_textList[i].editFlg) return true;
            }
            //foreach (SLogicalLine cm in m_textList)
            //{
            //    if (cm.editFlg == true)
            //    {
            //        return true;
            //    }
            //}

            return false;
        }

#endregion
    }
}
