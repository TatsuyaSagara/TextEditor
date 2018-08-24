using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace CETextBoxControl
{
    class CEInputMethod
    {
        /// <summary>
        /// 現在のキャレット位置にキャンディデートウィンドウを表示する
        /// （キャンディデートウィンドウ：変換候補一覧ウィンドウ）
        /// </summary>
        /// <param name="handle"></param>
        public void SetCandidateWindow( IntPtr handle )
        {
            if(handle == null)
            {
                return;
            }

            IntPtr hImc = CEWin32Api.ImmGetContext(handle);
            CEWin32Api.POINT point = new CEWin32Api.POINT(0, 0);
            CEWin32Api.GetCaretPos(out point);
            CEWin32Api.CANDIDATEFORM cndFrm = new CEWin32Api.CANDIDATEFORM();
            cndFrm.ptCurrentPos.X = point.X;
            cndFrm.ptCurrentPos.Y = point.Y;
            cndFrm.dwStyle = CEWin32Api.CFS_CANDIDATEPOS;
            if (hImc != null)
            {
                CEWin32Api.ImmSetCandidateWindow(hImc, ref cndFrm);
            }
            CEWin32Api.ImmReleaseContext(handle, hImc);
        }

        /// <summary>
        /// 現在のキャレット位置にコンポジションウィンドウを表示する 
        /// （コンポジションウィンドウ：入力中の未確定文字列）
        /// </summary>
        /// <param name="handle"></param>
        public void SetCompositionWindow(IntPtr handle, Font font)
        {
            if (handle == null)
            {
                return;
            }

            IntPtr hImc = CEWin32Api.ImmGetContext(handle);
            CEWin32Api.POINT point = new CEWin32Api.POINT(0, 0);
            CEWin32Api.GetCaretPos(out point);
            CEWin32Api.COMPOSITIONFORM compform = new CEWin32Api.COMPOSITIONFORM();
            compform.ptCurrentPos.X = point.X;
            compform.ptCurrentPos.Y = point.Y;
            compform.dwStyle = CEWin32Api.CFS_POINT;
            if (hImc != null)
            {
                CEWin32Api.ImmSetCompositionWindow(hImc, ref compform);
            }

            IntPtr hHGlobalLOGFONT = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(CEWin32Api.LOGFONT)));
            IntPtr pLogFont = CEWin32Api.GlobalLock(hHGlobalLOGFONT);
            CEWin32Api.LOGFONT logFont = new CEWin32Api.LOGFONT();
            font.ToLogFont(logFont);
            logFont.lfFaceName/*Name*/ = font.Name; //追加
            Marshal.StructureToPtr(logFont, pLogFont, false);
            CEWin32Api.GlobalUnlock(hHGlobalLOGFONT);
            CEWin32Api.ImmSetCompositionFont(hImc, hHGlobalLOGFONT);
            Marshal.FreeHGlobal(hHGlobalLOGFONT);

            CEWin32Api.ImmReleaseContext(handle, hImc);
        }
    }
}
