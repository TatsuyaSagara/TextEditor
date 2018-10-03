using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CETextBoxControl
{
    public class CECommon
    {
        public static IntPtr RectangleToIntPtr(Rectangle rct)
        {
            CEWin32Api.RECT lpRect = CEWin32Api.RECT.FromRectangle(rct);
            IntPtr ptrRect = Marshal.AllocHGlobal(Marshal.SizeOf(lpRect));
            Marshal.StructureToPtr(lpRect, ptrRect, false);

            return ptrRect;
        }

        /// <summary>
        /// RGB形式で指定した色を数値に変換
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int GetColor(int r, int g, int b)
        {
            return r + (g << 8) + (b << 16);
        }

        public static void print(string str)
        {
            Debug.WriteLine(str);
        }

        /// <summary>
        /// RGBをひっくり返す
        /// </summary>
        /// <param name="aaa"></param>
        /// <returns></returns>
        public static int ChgRGB(int aaa)
        {
            int bbb = ((aaa & 0x0000ff) << 16);
            bbb += (aaa & 0x00ff00);
            bbb += (aaa & 0xff0000) >> 16;

            return bbb;
        }

        /// <summary>
        /// エンコードタイプ取得
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Encoding GetEncoding(byte[] bytes)
        {
            const byte bEscape = 0x1B;
            const byte bAt = 0x40;
            const byte bDollar = 0x24;
            const byte bAnd = 0x26;
            const byte bOpen = 0x28;    //'('
            const byte bB = 0x42;
            const byte bD = 0x44;
            const byte bJ = 0x4A;
            const byte bI = 0x49;

            int len = bytes.Length;
            byte b1, b2, b3, b4;

            //Encode::is_utf8 は無視

            bool isBinary = false;
            for (int i = 0; i < len; i++)
            {
                b1 = bytes[i];
                if (b1 <= 0x06 || b1 == 0x7F || b1 == 0xFF)
                {
                    //'binary'
                    isBinary = true;
                    if (b1 == 0x00 && i < len - 1 && bytes[i + 1] <= 0x7F)
                    {
                        //smells like raw unicode
                        return Encoding.Unicode;
                    }
                }
            }
            if (isBinary)
            {
                return null;
            }

            //not Japanese
            bool notJapanese = true;
            for (int i = 0; i < len; i++)
            {
                b1 = bytes[i];
                if (b1 == bEscape || 0x80 <= b1)
                {
                    notJapanese = false;
                    break;
                }
            }
            if (notJapanese)
            {
                return Encoding.ASCII;
            }

            for (int i = 0; i < len - 2; i++)
            {
                b1 = bytes[i];
                b2 = bytes[i + 1];
                b3 = bytes[i + 2];

                if (b1 == bEscape)
                {
                    if (b2 == bDollar && b3 == bAt)
                    {
                        //JIS_0208 1978
                        //JIS
                        return Encoding.GetEncoding(50220);
                    }
                    else if (b2 == bDollar && b3 == bB)
                    {
                        //JIS_0208 1983
                        //JIS
                        return Encoding.GetEncoding(50220);
                    }
                    else if (b2 == bOpen && (b3 == bB || b3 == bJ))
                    {
                        //JIS_ASC
                        //JIS
                        return Encoding.GetEncoding(50220);
                    }
                    else if (b2 == bOpen && b3 == bI)
                    {
                        //JIS_KANA
                        //JIS
                        return Encoding.GetEncoding(50220);
                    }
                    if (i < len - 3)
                    {
                        b4 = bytes[i + 3];
                        if (b2 == bDollar && b3 == bOpen && b4 == bD)
                        {
                            //JIS_0212
                            //JIS
                            return Encoding.GetEncoding(50220);
                        }
                        if (i < len - 5 &&
                            b2 == bAnd && b3 == bAt && b4 == bEscape &&
                            bytes[i + 4] == bDollar && bytes[i + 5] == bB)
                        {
                            //JIS_0208 1990
                            //JIS
                            return Encoding.GetEncoding(50220);
                        }
                    }
                }
            }

            //should be euc|sjis|utf8
            //use of (?:) by Hiroki Ohzaki <ohzaki@iod.ricoh.co.jp>
            int sjis = 0;
            int euc = 0;
            int utf8 = 0;
            for (int i = 0; i < len - 1; i++)
            {
                b1 = bytes[i];
                b2 = bytes[i + 1];
                if (((0x81 <= b1 && b1 <= 0x9F) || (0xE0 <= b1 && b1 <= 0xFC)) &&
                    ((0x40 <= b2 && b2 <= 0x7E) || (0x80 <= b2 && b2 <= 0xFC)))
                {
                    //SJIS_C
                    sjis += 2;
                    i++;
                }
            }
            for (int i = 0; i < len - 1; i++)
            {
                b1 = bytes[i];
                b2 = bytes[i + 1];
                if (((0xA1 <= b1 && b1 <= 0xFE) && (0xA1 <= b2 && b2 <= 0xFE)) ||
                    (b1 == 0x8E && (0xA1 <= b2 && b2 <= 0xDF)))
                {
                    //EUC_C
                    //EUC_KANA
                    euc += 2;
                    i++;
                }
                else if (i < len - 2)
                {
                    b3 = bytes[i + 2];
                    if (b1 == 0x8F && (0xA1 <= b2 && b2 <= 0xFE) &&
                        (0xA1 <= b3 && b3 <= 0xFE))
                    {
                        //EUC_0212
                        euc += 3;
                        i += 2;
                    }
                }
            }
            for (int i = 0; i < len - 1; i++)
            {
                b1 = bytes[i];
                b2 = bytes[i + 1];
                if ((0xC0 <= b1 && b1 <= 0xDF) && (0x80 <= b2 && b2 <= 0xBF))
                {
                    //UTF8
                    utf8 += 2;
                    i++;
                }
                else if (i < len - 2)
                {
                    b3 = bytes[i + 2];
                    if ((0xE0 <= b1 && b1 <= 0xEF) && (0x80 <= b2 && b2 <= 0xBF) &&
                        (0x80 <= b3 && b3 <= 0xBF))
                    {
                        //UTF8
                        utf8 += 3;
                        i += 2;
                    }
                }
            }
            //M. Takahashi's suggestion
            //utf8 += utf8 / 2;

            Debug.WriteLine(string.Format("sjis = {0}, euc = {1}, utf8 = {2}", sjis, euc, utf8));
            if (euc > sjis && euc > utf8)
            {
                //EUC
                return Encoding.GetEncoding(51932);
            }
            else if (sjis > euc && sjis > utf8)
            {
                //SJIS
                return Encoding.GetEncoding(932);
            }
            else if (utf8 > euc && utf8 > sjis)
            {
                //UTF8
                return Encoding.UTF8;
            }

            return null;
        }
    }

    /// <summary>
    /// 文字コードに関するクラス
    /// </summary>
    public static class CharCode
    {
        /// <summary>
        /// ASCII
        /// </summary>
        public static System.Text.Encoding ASCII
        {
            get
            {
                return System.Text.Encoding.ASCII;
            }
        }

        /// <summary>
        /// EUC-JP
        /// </summary>
        public static System.Text.Encoding EUCJP
        {
            get
            {
                return System.Text.Encoding.GetEncoding("EUC-JP");
            }
        }

        /// <summary>
        /// JIS
        /// </summary>
        public static System.Text.Encoding JIS
        {
            get
            {
                return System.Text.Encoding.GetEncoding("iso-2022-jp");
            }
        }

        /// <summary>
        /// Shift_JIS
        /// </summary>
        public static System.Text.Encoding SJIS
        {
            get
            {
                return System.Text.Encoding.GetEncoding("Shift_JIS");
            }
        }

        /// <summary>
        /// UTF-7
        /// </summary>
        public static System.Text.Encoding UTF7
        {
            get
            {
                return System.Text.Encoding.GetEncoding("UTF-7");
            }
        }

        /// <summary>
        /// UTF-8
        /// </summary>
        public static System.Text.Encoding UTF8
        {
            get
            {
                return System.Text.Encoding.GetEncoding("UTF-8");
            }
        }

        /// <summary>
        /// UTF-8 (without BOM)
        /// </summary>
        public static System.Text.Encoding UTF8N
        {
            get
            {
                return new System.Text.UTF8Encoding(false);
            }
        }

        /// <summary>
        /// UTF-16
        /// </summary>
        public static System.Text.Encoding UTF16
        {
            get
            {
                return System.Text.Encoding.GetEncoding("UTF-16");
            }
        }

        /// <summary>
        /// UTF-16 Big-Endian
        /// </summary>
        public static System.Text.Encoding UTF16B
        {
            get
            {
                return System.Text.Encoding.GetEncoding("unicodeFFFE");
            }
        }

        /// <summary>
        /// UTF-16 (without BOM)
        /// </summary>
        public static System.Text.Encoding UTF16LE
        {
            get
            {
                return System.Text.Encoding.GetEncoding("UTF-16LE");
            }
        }

        /// <summary>
        /// UTF-16 Big-Endian (without BOM)
        /// </summary>
        public static System.Text.Encoding UTF16BE
        {
            get
            {
                return System.Text.Encoding.GetEncoding("UTF-16BE");
            }
        }

        /// <summary>
        /// UTF-32
        /// </summary>
        public static System.Text.Encoding UTF32
        {
            get
            {
                return System.Text.Encoding.GetEncoding("UTF-32");
            }
        }

        /// <summary>
        /// UTF-32 Big-Endian
        /// </summary>
        public static System.Text.Encoding UTF32B
        {
            get
            {
                return System.Text.Encoding.GetEncoding("UTF-32BE");
            }
        }

        /// <summary>
        /// バイト配列から文字コードを判別します。
        /// </summary>
        /// <param name="srcBytes">文字コードを判別するバイト配列</param>
        /// <returns><paramref name="srcBytes"/> から予想される文字コード</returns>
        public static System.Text.Encoding[] Detect(byte[] srcBytes)
        {
            if (srcBytes == null || srcBytes.Length <= 0)
            {
                return null;
            }

            System.Collections.Generic.List<System.Text.Encoding> dstEncodings = new System.Collections.Generic.List<System.Text.Encoding>();

            if (DetectEncoding(srcBytes, ASCII))
            {
                dstEncodings.Add(ASCII);
            }

            if (DetectEncoding(srcBytes, EUCJP))
            {
                dstEncodings.Add(EUCJP);
            }

            if (DetectEncoding(srcBytes, JIS))
            {
                dstEncodings.Add(JIS);
            }

            if (DetectEncoding(srcBytes, SJIS))
            {
                dstEncodings.Add(SJIS);
            }

            if (DetectEncoding(srcBytes, UTF7))
            {
                dstEncodings.Add(UTF7);
            }

            if (DetectBOM(srcBytes, 0xef, 0xbb, 0xbf) && DetectEncoding(srcBytes, UTF8))
            {
                dstEncodings.Add(UTF8);
            }
            else if (DetectEncoding(srcBytes, UTF8N))
            {
                dstEncodings.Add(UTF8N);
            }

            if (DetectBOM(srcBytes, 0xff, 0xfe) && DetectEncoding(srcBytes, UTF16))
            {
                dstEncodings.Add(UTF16);
            }
            else if (DetectBOM(srcBytes, 0xfe, 0xff) && DetectEncoding(srcBytes, UTF16B))
            {
                dstEncodings.Add(UTF16B);
            }

            if (DetectBOM(srcBytes, 0xff, 0xfe, 0x00, 0x00) && DetectEncoding(srcBytes, UTF32))
            {
                dstEncodings.Add(UTF32);
            }
            else if (DetectBOM(srcBytes, 0x00, 0x00, 0xfe, 0xff) && DetectEncoding(srcBytes, UTF32B))
            {
                dstEncodings.Add(UTF32B);
            }

            if (srcBytes.Length >= 2 && srcBytes.Length % 2 == 0)
            {
                if (srcBytes[0] == 0x00)
                {
                    dstEncodings.Add(UTF16BE);

                    for (int i = 0; i < srcBytes.Length; i += 2)
                    {
                        if (srcBytes[i] != 0x00 || srcBytes[i + 1] < 0x06 || srcBytes[i + 1] >= 0x7f)
                        {
                            dstEncodings.Remove(UTF16BE);
                            break;
                        }
                    }
                }
                else if (srcBytes[1] == 0x00)
                {
                    dstEncodings.Add(UTF16LE);

                    for (int i = 0; i < srcBytes.Length; i += 2)
                    {
                        if (srcBytes[i] < 0x06 || srcBytes[i] >= 0x7f || srcBytes[i + 1] != 0x00)
                        {
                            dstEncodings.Remove(UTF16LE);
                            break;
                        }
                    }
                }
            }

            return dstEncodings.ToArray();
        }

        /// <summary>
        /// BOM から文字コードを判定します。
        /// </summary>
        /// <param name="srcBytes">文字コードを判別するバイト配列</param>
        /// <param name="bom">バイトオーダーマーク</param>
        /// <returns><paramref name="srcBytes"/> から文字コードが判定できた場合は true</returns>
        private static bool DetectBOM(byte[] srcBytes, params byte[] bom)
        {
            if (srcBytes.Length < bom.Length)
            {
                return false;
            }

            for (int i = 0; i < bom.Length; i++)
            {
                if (srcBytes[i] != bom[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// バイト配列から文字列、文字列からバイト配列に変換し、文字コードを判別します。
        /// </summary>
        /// <param name="srcBytes">文字コードを判別するバイト配列</param>
        /// <param name="encoding">変換する文字コード</param>
        /// <returns><paramref name="srcBytes"/> から文字コードが判定できた場合は true</returns>
        private static bool DetectEncoding(byte[] srcBytes, System.Text.Encoding encoding)
        {
            string encodedStr = encoding.GetString(srcBytes);
            byte[] encodedByte = encoding.GetBytes(encodedStr);

            if (srcBytes.Length != encodedByte.Length)
            {
                return false;
            }

            for (int i = 0; i < srcBytes.Length; i++)
            {
                if (srcBytes[i] != encodedByte[i])
                {
                    return false;
                }
            }

            return true;
        }
    }

}
