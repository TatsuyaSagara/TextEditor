using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CETextBoxControl
{
    /// <summary>
    /// 論理行データ(ファイル)
    /// </summary>
    public class SLogicalLine
    {
        /// <summary>
        /// 物理行管理データ
        /// </summary>
        public List<SPhysicalLine> m_physicalLine;

        /// <summary>
        /// 編集有無
        /// （true:編集済／false:未編集）
        /// </summary>
        public Boolean editFlg;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SLogicalLine()
        {
            clearEditFlg();
        }

        /// <summary>
        /// 編集フラグクリア
        /// </summary>
        public void clearEditFlg()
        {
            editFlg = false;
        }

        /// <summary>
        /// 論理行データ内の物理行数
        /// </summary>
        public int m_sldLine
        {
            get
            {
                return m_physicalLine.Count;
            }
        }

        /// <summary>
        /// 文字列
        /// </summary>
        private string _m_text;
        public string m_text
        {
            get
            {
                return _m_text;
            }
            set
            {
                editFlg = true;
                _m_text = value;
            }
        }
    }
}
