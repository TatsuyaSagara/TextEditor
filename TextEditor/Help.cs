using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextEditor
{
    public partial class Help : Form
    {
        public Help()
        {
            InitializeComponent();

            label1.Text += "ようこそ テキストエディタ へ\r\n";
            label1.Text += "\r\n";
            label1.Text += "使用頂きありがとうございます。このテキストエディタはRichTextやTextBoxなどのコンポーネントを\r\n";
            label1.Text += "使用せず、Controlコンポーネントの派生から作成した独自コンポーネントで構成されています。\r\n";
            label1.Text += "まだ常用出来るだけの安定性も機能も満たしていません。\r\n";
            label1.Text += "不具合や機能追加の要望がありましたら連絡いただけると幸いです。\r\n";
            label1.Text += "\r\n";
            label1.Text += "現在実装中の機能は以下となります。（実装順）\r\n";
            label1.Text += "もし、優先して実装した方が良い機能がありましたら連絡下さい。\r\n";
            label1.Text += "\r\n";
            label1.Text += "１．[完了]複数文字コード対応\r\n";
            label1.Text += "２．[完了]アンドゥ／リドゥ\r\n";
            label1.Text += "３．検索\r\n";
            label1.Text += "４．置換\r\n";
            label1.Text += "６．単語単位選択\r\n";
            label1.Text += "５．行指定ジャンプ\r\n";
            label1.Text += "\r\n";
            label1.Text += "実装予定\r\n";
            label1.Text += "\r\n";
            label1.Text += "１．指定行折り返し\r\n";
            label1.Text += "２．ボックス選択\r\n";
            label1.Text += "３．ウィンドウ分割\r\n";
            label1.Text += "４．Grep\r\n";
            label1.Text += "\r\n";
            label1.Text += "以上です。\r\n";
            label1.Text += "---\r\n";
            label1.Text += "株式会社システムクオリティ\r\n";
            label1.Text += "相良達也";
        }
    }
}
