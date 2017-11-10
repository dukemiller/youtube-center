using System;
using System.Runtime.InteropServices;

namespace youtube_center.Classes
{
    public static class NativeMethods
    {
        private const int Restore = 9;

        /// <summary>
        ///     Focus the opened downloader.
        /// </summary>
        public static void FocusOther()
        {
            var hwnd = FindWindow(null, "Youtube Center");
            ShowWindow(hwnd, Restore);
            SetForegroundWindow(hwnd);
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string sClassName, string sAppName);
    }
}
