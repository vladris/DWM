using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Dwm
{
    /// <summary>
    /// Provides message handling of Dwm Windows messages
    /// </summary>
    public class DwmMessage
    {
        /// <summary>
        /// Colorization color changed
        /// </summary>
        public const int WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320;

        /// <summary>
        /// Window maximized changed
        /// </summary>
        public const int WM_DWMWINDOWMAXIMIZEDCHANGED = 0x0321;

        /// <summary>
        /// Window composition changed
        /// </summary>
        public const int WM_DWMCOMPOSITIONCHANGED = 0x031E;

        /// <summary>
        /// Non client rendering changed
        /// </summary>
        public const int WM_DWMNCRENDERINGCHANGED = 0x031F;

        /// <summary>
        /// Handles a Windows message
        /// </summary>
        /// <param name="hwnd">Handle to the form that received the message</param>
        /// <param name="m">Message</param>
        /// <returns>True if the message was handled, false otherwise</returns>
        public static bool DwmWndProc(IntPtr hwnd, ref Message m)
        {
            IntPtr result;
            bool handled;

            // Native API call
            handled = DwmDefWindowProc(hwnd, m.Msg, m.WParam, m.LParam, out result);

            // Set result
            m.Result = result;

            return handled;
        }

        /// <summary>
        /// Handles a Windows message
        /// </summary>
        /// <param name="form">Form that received the message</param>
        /// <param name="m">Message</param>
        /// <returns>True if the message was handled, false otherwise</returns>
        public static bool DwmWndProc(Form form, ref Message m)
        {
            return DwmWndProc(form.Handle, ref m);
        }

        #region API imports

        [DllImport("dwmapi.dll")]
        private static extern bool DwmDefWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, out IntPtr lResult);

        #endregion
    }
}
