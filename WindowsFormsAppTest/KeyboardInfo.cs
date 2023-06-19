using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ScreenBrightness
{
    public class KeyboardInfo
    {
        private KeyboardInfo() { }

        [DllImport("user32")]
        private static extern short GetKeyState(int vKey);

        public static KeyStateInfo GetKeyState(Keys key)
        {
            int vkey = (int)key;

            if (key == Keys.Alt)
            {
                vkey = 0x12;    // VK_ALT
            }

            short keyState = GetKeyState(vkey);
            int low = Low(keyState);
            int high = High(keyState);
            bool toggled = (low == 1);
            bool pressed = (high == 1);

            return new KeyStateInfo(key, pressed, toggled);
        }

        private static int High(int keyState)
        {
            if (keyState > 0)
            {
                return keyState >> 0x10;
            }
            else
            {
                return (keyState >> 0x10) & 0x1;
            }
        }

        private static int Low(int keyState)
        {
            return keyState & 0xffff;
        }
    }

    public struct KeyStateInfo
    {
        Keys m_Key;
        bool m_IsPressed;
        bool m_IsToggled;

        public KeyStateInfo(Keys key, bool ispressed, bool istoggled)
        {
            m_Key = key;
            m_IsPressed = ispressed;
            m_IsToggled = istoggled;
        }

        public static KeyStateInfo Default
        {
            get
            {
                return new KeyStateInfo(Keys.None, false, false);
            }
        }

        public Keys Key
        {
            get { return m_Key; }
        }

        public bool IsPressed
        {
            get { return m_IsPressed; }
        }

        public bool IsToggled
        {
            get { return m_IsToggled; }
        }
    }
}
