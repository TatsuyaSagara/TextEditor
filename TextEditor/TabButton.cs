using CETextBoxControl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextEditor
{
    public class TabButton : Label/*Button*/
    {
        /// <summary>
        /// 表示しているファイル名
        /// </summary>
        public string openFilePath;

        /// <summary>
        /// 状態
        /// </summary>
        public Boolean m_active;

        /// <summary>
        /// フォント
        /// </summary>
        private Font m_font;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TabButton()
        {
            m_font = new Font("ＭＳ ゴシック", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // タブにファイル名を出力
            //int POINTSPERINCH = 96;
            //e.Graphics.ScaleTransform(e.Graphics.DpiX / POINTSPERINCH, e.Graphics.DpiY / POINTSPERINCH);

            string fileName = Path.GetFileName(openFilePath);

            StringFormat sf = new StringFormat();
            SizeF stringSize = e.Graphics.MeasureString(fileName, m_font, 5000, sf); // そんなに長いファイル名ないだろうということで5000...。

            int x, y;
            x = 3;
            y = (this.Height / 2) - (int)(stringSize.Height / 2);
            e.Graphics.SetClip(new Rectangle(0, 0, this.Width - 20, this.Height)); // 20はクローズボタン「ｘ」が表示されるサイズ
            e.Graphics.DrawString(fileName, m_font, new SolidBrush(Color.Black), x, y);
            e.Graphics.SetClip(new Rectangle(0, 0, this.Width, this.Height));

            // タブに枠線を引く
            Pen lineColorPen;
            Pen underLineColorPen;
            if (m_active)
            {
                // アクティブ
                lineColorPen = new Pen(Color.Black);
                underLineColorPen = new Pen(Color.White);
            }
            else
            {
                // 非アクティブ
                lineColorPen = new Pen(Color.Gray);
                underLineColorPen = new Pen(Color.Black);
            }
            e.Graphics.DrawLine(lineColorPen, 0, 0, 0, this.Height - 1); // 左縦線
            e.Graphics.DrawLine(lineColorPen, 0, 0, this.Width - 1, 0); // 上線
            e.Graphics.DrawLine(lineColorPen, this.Width - 1, 0, this.Width - 1, this.Height - 1); // 右縦線
            e.Graphics.DrawLine(underLineColorPen, 0, this.Height - 1, this.Width - 1, this.Height - 1); // 下線
        }
    }
}
