using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CETextBoxControl
{
    /// <summary>
    /// 物理行管理データ(画面)
    /// </summary>
    public class SPhysicalLine
    {
        /// <summary>
        /// 論理行数
        /// </summary>
        public int m_lineNum;

        /// <summary>
        /// 表示開始位置
        /// </summary>
        public int m_offset;

        /// <summary>
        /// 文字列長
        /// </summary>
        public int m_length;

        /// <summary>
        /// 改行文字列
        /// </summary>
        public string m_lfStr;

        /// <summary>
        /// 折り返し有無
        /// </summary>
        public Boolean m_wrap;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SPhysicalLine()
        {
            m_lineNum = 0;
            m_offset = 0;
            m_length = 0;
            m_lfStr = "";
            m_wrap = false;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="offset">表示開始位置</param>
        /// <param name="length">文字列長</param>
        /// <param name="lfStr">改行文字列</param>
        /// <param name="wrap">折り返し有無</param>
        public SPhysicalLine(int line, int offset, int length, string lfStr, Boolean wrap)
        {
            m_lineNum = line;
            m_offset = offset;
            m_length = length;
            m_lfStr = lfStr;
            m_wrap = wrap;
        }
    }
}
