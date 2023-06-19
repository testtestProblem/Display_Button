using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WindowsFormsAppTest
{
    static class Program
    {

        [DllImport("user32.dll")]
        public static extern Int32 FindWindow(String lpClassName, String lpWindowName);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern Int32 PostMessage(Int32 hWnd, Int32 wMsg, Int32 wParam, Int32 lParam);


        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {

            bool AppRunningFlag = false;
            System.Threading.Mutex mutex = new System.Threading.Mutex(true, Application.ProductName, out AppRunningFlag);
            if (AppRunningFlag)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
                mutex.ReleaseMutex();
            }
            else
            {
                int _exoUI = FindWindow(null, "TouchEventTrace");
                PostMessage(_exoUI, 32773, 0, 0);
                Environment.Exit(1);
            }
        }
    }
}
