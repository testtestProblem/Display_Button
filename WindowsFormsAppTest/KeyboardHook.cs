using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WindowsFormsAppTest;

namespace ScreenBrightness
{
    class KeyboardHook
    {
        public enum HookType : int
        {
            WH_KEYBOARD = 2,
            WH_KEYBOARD_LL = 13,
        }

        // Brightness mBrightness2 = new Brightness();

        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static int m_HookHandle = 0;    // Hook handle
        private HookProc m_KbdHookProc;     // 鍵盤掛鉤函式指標

        //是否只由這個Global Hook抓取鍵盤事件
        public static bool globalControlOnly = true;

        // 設置掛鉤.
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn,
        IntPtr hInstance, int threadId);

        // 將之前設置的掛鉤移除。記得在應用程式結束前呼叫此函式.
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        // 呼叫下一個掛鉤處理常式（若不這麼做，會令其他掛鉤處理常式失效）.
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode,
        IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        static extern int GetCurrentThreadId();

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public void SetWindowsHook(bool IsHookEnabled)
        {
            if (IsHookEnabled && m_HookHandle == 0)
            {
                using (Process curProcess = Process.GetCurrentProcess())
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    m_KbdHookProc = new HookProc(KeyboardHookProc);
                    // 如果必須設置全域掛鉤（global hook），其對應的 hook 常數是 WH_KEYBOARD_LL（數值為 13，最後的 LL 是 Low Level 的縮寫）。
                    m_HookHandle = SetWindowsHookEx((int)HookType.WH_KEYBOARD_LL, m_KbdHookProc, GetModuleHandle(curModule.ModuleName), 0);
                }

                if (m_HookHandle == 0)
                {
                    Trace.WriteLine("Install Global Keyboard Hook Fail!");
                    return;
                }
            }
            else
            {
                bool ret = UnhookWindowsHookEx(m_HookHandle);
                if (ret == false)
                {
                    Trace.WriteLine("Uninstall Global Keyboard Hook Fail!");
                    return;
                }
                m_HookHandle = 0;
            }
        }

        const int WM_KEYUP = 0x0101;
        const int WM_SYSKEYUP = 0x105;
        public int KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            bool isUp = (wParam.ToInt32().Equals(WM_KEYUP) || wParam.ToInt32().Equals(WM_SYSKEYUP));

            if (nCode < 0 || !isUp) return CallNextHookEx(m_HookHandle, nCode, wParam, lParam);

            // 取得欲攔截之按鍵狀態
            KeyStateInfo ctrlKey = KeyboardInfo.GetKeyState(Keys.ControlKey);
            KeyStateInfo UpKey = KeyboardInfo.GetKeyState(Keys.Up);
            KeyStateInfo DownKey = KeyboardInfo.GetKeyState(Keys.Down);

            if (ctrlKey.IsPressed && UpKey.IsPressed)
            {
                Form1.IsBrightnessUp = true;
            }
            if (ctrlKey.IsPressed && DownKey.IsPressed)
            {
                Form1.IsBrightnessDown = true;
            }
            return CallNextHookEx(m_HookHandle, nCode, wParam, lParam);
        }
    }
}
