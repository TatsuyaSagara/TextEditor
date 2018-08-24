using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CETextBoxControl
{
    public static class UndoRedoCode
    {
        public static int UR_UNKNOWN = 0;   // 不明
        public static int UR_INSERT = 1;    // 挿入
        public static int UR_DELETE = 2;    // 削除
        public static int UR_MOVECARET = 3; // キャレット移動
    }

    public class CEUndoRedoData
    {
        /// <summary>
        /// 操作(挿入・削除・キャレット移動)
        /// </summary>
        public int m_ope;

        /// <summary>
        /// 操作前のキャレット位置(論理)
        /// </summary>
        public Point m_preCaret;

        /// <summary>
        /// 操作後のキャレット位置(論理)
        /// </summary>
        public Point m_aftCaret;

        /// <summary>
        /// 操作に関するデータ
        /// </summary>
        public string m_pcmemData;

        /// <summary>
        /// 範囲選択タイプ
        /// </summary>
        public int m_selectType;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CEUndoRedoData(){ }
    }

    public class CEUndoRedoOpe
    {
        private List<CEUndoRedoData> m_ppCOpeArr;
        private int m_opeIdx; // 操作位置

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CEUndoRedoOpe()
        {
            m_ppCOpeArr = new List<CEUndoRedoData>();
            m_opeIdx = -1;
        }

        /// <summary>
        /// Undo/Redo情報保存
        /// </summary>
        /// <param name="urd"></param>
        private void Append(CEUndoRedoData urd)
        {
            m_ppCOpeArr.Add(urd);
            m_opeIdx++;
            return;
        }

        /// <summary>
        /// Undo/Redo情報削除
        /// </summary>
        private void DelOpe()
        {
            if (m_ppCOpeArr.Count == 0) return;

            for (int idx = m_ppCOpeArr.Count - 1; idx > -1 && idx > m_opeIdx; idx--)
            {
                m_ppCOpeArr.RemoveAt(idx);
            }
        }

        /// <summary>
        /// Undo/Redo情報取得
        /// </summary>
        /// <returns></returns>
        public CEUndoRedoData GetUndoOpe()
        {
            if (m_ppCOpeArr.Count == 0) return null;
            if (m_opeIdx == -1) return null;

            int cnt = m_opeIdx;
            m_opeIdx--;
            return m_ppCOpeArr[cnt];
        }

        /// <summary>
        /// Undo/Redo情報取得
        /// </summary>
        /// <returns></returns>
        public CEUndoRedoData GetRedoOpe()
        {
            if (m_ppCOpeArr.Count == 0) return null;
            if (m_opeIdx == m_ppCOpeArr.Count - 1) return null;

            m_opeIdx++;
            int cnt = m_opeIdx;
            return m_ppCOpeArr[cnt];
        }

        CEUndoRedoData UndoRedoData = null;

        /// <summary>
        /// 操作前情報の保存
        /// </summary>
        /// <param name="p">キャレット位置(物理)</param>
        /// <param name="ee">実行クラス</param>
        public void PreUndoRedo(Point p, int selectType, CEEditView ee)
        {
            // Undo/Redo 不要な操作情報削除
            this.DelOpe();

            // Undo/Redo 操作情報格納エリア確保
            if (UndoRedoData == null)
            {
                UndoRedoData = new CEUndoRedoData();
            }

            // Undo/Redo 操作前のキャレット位置保存（論理位置として保存）
            int lRow, lCol;
            ee.m_doc.PtoLPos(p.Y, p.X, out lRow, out lCol); // 物理位置→論理位置
            UndoRedoData.m_preCaret.X = lCol;
            UndoRedoData.m_preCaret.Y = lRow;
            UndoRedoData.m_selectType = selectType;
        }

        /// <summary>
        /// 操作後情報の保存
        /// </summary>
        /// <param name="p">キャレット位置</param>
        /// <param name="ope">操作(挿入・削除・キャレット移動)</param>
        /// <param name="str">操作文字列</param>
        /// <param name="ee">実行クラス</param>
        public void ProUndoRedo(Point p, int ope, string str, CEEditView ee)
        {
            if (UndoRedoData == null)
            {
                // Undo/Redo 情報がない場合は何もしない
                return;
            }

            // Undo/Redo 操作保存
            UndoRedoData.m_ope = ope;

            // Undo/Redo 操作後のキャレット位置保存（論理位置として保存）
            int lRow, lCol;
            ee.m_doc.PtoLPos(p.Y, p.X, out lRow, out lCol); // 物理位置→論理位置
            UndoRedoData.m_aftCaret.X = lCol;
            UndoRedoData.m_aftCaret.Y = lRow;

            // Undo/Redo 操作データ保存
            UndoRedoData.m_pcmemData = str;

            // Undo/Redo 保存
            this.Append(UndoRedoData);

            // Undo/Redo 情報削除
            UndoRedoData = null;
        }
    }
}
