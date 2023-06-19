#define _CRT_SECURE_NO_WARNINGS
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Diagnostics;
using VRDSDK;
using ScreenBrightness;
using Win8Hottab;

namespace WindowsFormsAppTest
{
    public partial class Form1 : Form
    {
        [DllImport("iDisplayButtonDLL.dll", EntryPoint = "Add", CallingConvention = CallingConvention.Cdecl)]
        private static extern int add(int a, int b);

        [DllImport("iDisplayButtonDLL.dll", EntryPoint = "initial_test", CallingConvention = CallingConvention.Cdecl)]
        private static extern int initial_test();

        [DllImport("iDisplayButtonDLL.dll", EntryPoint = "Set_Screen_Brightness", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Set_Screen_Brightness(int b);
       
        [DllImport("iDisplayButtonDLL.dll", EntryPoint = "Read_Device_State", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Read_Device_State();

        [DllImport("iDisplayButtonDLL.dll", EntryPoint = "fill_color", CallingConvention = CallingConvention.Cdecl)]
        private static extern int fill_color(byte r, byte g, byte b);
       
        [DllImport("iDisplayButtonDLL.dll", EntryPoint = "show_picture", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int show_picture(uint x, uint y,  byte* img_buf, ulong size);

        [DllImport("iDisplayButtonDLL.dll", EntryPoint = "initial_test", CallingConvention = CallingConvention.Cdecl)]
        private static extern int close_device();

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("winmm.dll")]
        public static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        //[DllImport("winmm.dll", CharSet = CharSet.Ansi)]
        //public static extern int IAudioEndpointVolume(ref float pfLevelDB);

        public event System.Windows.Forms.KeyPressEventHandler KeyPress;

        int pictureIndex = 0;
        bool pictureLock = false;
        int brightnessValue = 50;

        public static bool IsBrightnessUp = false;
        public static bool IsBrightnessDown = false;

        string pathRoot = Environment.CurrentDirectory;
        string[] pathP = new string[12] { 
            @"\\1.bmp",
            @"\\1_1.bmp",
            @"\\2.bmp",
            @"\\2_1.bmp",
            @"\\3.bmp",
            @"\\3_1.bmp",
            @"\\4.bmp",
            @"\\4_1.bmp",
            @"\\5.bmp",
            @"\\5_1.bmp",
            @"\\6.bmp",
            @"\\6_1.bmp"
        };
        
        public void openPicture(string path )
        {
            string absPath= pathRoot + path;
            FileStream fileStream = new FileStream(absPath, FileMode.Open);

            //clear background color
            fill_color(0, 0, 0);

            long size1 = fileStream.Length;

            // Create a new instance of memorystream
            var memoryStream = new MemoryStream();

            // Use the .CopyTo() method and write current filestream to memory stream
            fileStream.CopyTo(memoryStream);

            // Convert Stream To Array
            byte[] byteArray = memoryStream.ToArray();
            //byte* b = byteArray;
            unsafe
            {
                fixed (byte* byteArrayP = &byteArray[0])
                {
                    show_picture(50, 0, byteArrayP, (ulong)size1);
                }
            }
            fileStream.Close();
        }

        public Form1()
        {
            InitializeComponent();
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Opacity = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int ret = initial_test();
            if (ret == 0)
            {
                //MessageBox.Show("Successful connect", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                connectStateT.Text = "Success";
                timer1.Enabled = true;
                timer1.Start();

                openPicture(pathP[0]);
                Set_Screen_Brightness(brightnessValue);
            }
            else
            {
                //MessageBox.Show("Fail connect\nError code -> " + x.ToString(), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                connectStateT.Text = "Fail\nrestart app";
            }
            //mKeyboardHook.SetWindowsHook(true);
            KeyPreview = true; 
        }

        // Detect all numeric characters at the form level and consume 1,4, and 7.
        // Form.KeyPreview must be set to true for this event handler to be called.
        void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= 48 && e.KeyChar <= 57)
            {
                MessageBox.Show($"Form.KeyPress: '{e.KeyChar}' pressed.");

                switch (e.KeyChar)
                {
                    case (char)49:
                    case (char)52:
                    case (char)55:
                        MessageBox.Show($"Form.KeyPress: '{e.KeyChar}' consumed.");
                        e.Handled = true;
                        break;
                }
            }
        }

        [Guid("5CDF2C82-841E-4546-9722-0CF74078229A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAudioEndpointVolume
        {
            int GetMasterVolumeLevel(ref float pfLevelDB);
        }

        int counterRestart = 0;

        int displayFlag = 0;

        int musicStart = 0;
        System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"D:\a.wav");
 
        int CurrVol;
        private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        private const int APPCOMMAND_VOLUME_UP = 0xA0000;
        private const int APPCOMMAND_VOLUME_DOWN = 0x90000;
        private const int WM_APPCOMMAND = 0x319;
        private void timer1_Tick(object sender, EventArgs e)
        {
            int state=Read_Device_State();
            

            brightnessShow.Text = brightnessValue.ToString();

            counterRestart++;
            if (counterRestart == 20)
            {
                int ret = initial_test();

                counterRestart=0;
            }

            if (!pictureLock)
            { 
                if ((state % 10) == 0)
                {
                    dir.Text = "counterclockwise";
                    pictureIndex -= 2;
                    if(pictureIndex == -2) pictureIndex = 10;

                    openPicture(pathP[pictureIndex]);
                }
                else if ((state % 10) == 1)
                {
                    dir.Text = "clockwise";
                    pictureIndex += 2;
                    if(pictureIndex == 12) pictureIndex = 0;

                    openPicture(pathP[pictureIndex]);
                }
            }
            else if (pictureLock && (pictureIndex == 1))  //backlight
            {
                if ((state % 10) == 0)  //counterclockwise
                {
                    if (brightnessValue >= 10) brightnessValue -= 10;
                    else brightnessValue = 0;

                    mBrightness.SetBrightness((byte)(brightnessValue * 255 / 100));
                    Set_Screen_Brightness(brightnessValue);
                }
                else if ((state % 10) == 1)//clockwise
                {
                    if (brightnessValue <= 90) brightnessValue += 10;
                    else brightnessValue = 100;

                    mBrightness.SetBrightness((byte)(brightnessValue * 255 / 100));
                    Set_Screen_Brightness(brightnessValue);
                }
            }
            else if (pictureLock && (pictureIndex == 3))    //sound
            {
                if (musicStart == 0)
                {
                    //  player.Play();
                    //  musicStart = 1;
                }
                if ((state % 10) == 0)//counterclockwise
                {
                    SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle,
                (IntPtr)APPCOMMAND_VOLUME_DOWN);
                }
                else if ((state % 10) == 1)//clockwise
                {
                    SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle,
                (IntPtr)APPCOMMAND_VOLUME_UP);
                }
            }
            else if (pictureLock && (pictureIndex == 5))
            {
                if ((state % 10) == 0)//counterclockwise
                {
                    SendKeys.Send("%{UP}");
                    keyCodeT.Text = "ALT + up";
                }
                else if ((state % 10) == 1)//clockwise
                {
                    SendKeys.Send("%{DOWN}");
                    keyCodeT.Text = "ALT + down";
                }
            }
            else if (pictureLock && (pictureIndex == 7))
            {
                if ((state % 10) == 0)//counterclockwise
                {
                    SendKeys.Send("%{LEFT}");
                    keyCodeT.Text = "ALT + left";
                }
                else if ((state % 10) == 1)//clockwise
                {
                    SendKeys.Send("%{RIGHT}");
                    keyCodeT.Text = "ALT + right";
                }
            }
            else if (pictureLock && (pictureIndex == 9))
            {
                if ((state % 10) == 0)//counterclockwise
                {
                    SendKeys.Send("{LEFT}");
                    keyCodeT.Text = "left";
                }
                else if ((state % 10) == 1)//clockwise
                {
                    SendKeys.Send("{RIGHT}");
                    keyCodeT.Text = "right";
                }
            }
            else if (pictureLock && (pictureIndex == 11))//play demo
            {
                if (displayFlag == 0)
                {
                    SendKeys.Send("{d}");
                    keyCodeT.Text = "d";
                    displayFlag = 1;
                }
                
            }

            if ((state/10)==1)
            {
                button.Text = "break";
            }
            else if ((state / 10) == 2)
            {
                button.Text = "press";
            }
            else if ((state / 10) == 3)
            {
                button.Text = "click";

                if (!pictureLock)
                {
                    pictureLock = true;
                    openPicture(pathP[++pictureIndex]);
                }
                else
                {
                    pictureLock = false;
                    openPicture(pathP[--pictureIndex]);
                    displayFlag = 0;
                }
            }
            else if ((state / 10) == 4)
            {
                button.Text = "long press";
            }

            if (IsBrightnessUp ==true)
            { 
                if(winBackLight!=100) mBrightness.SetBrightness(winBackLight+=10);
                //label6.Text = "UP ok";
                IsBrightnessUp = false;
            }
            else if(IsBrightnessDown == true)
            {
                if (winBackLight != 0) mBrightness.SetBrightness(winBackLight -= 10);
                //label7.Text = "DOWN ok";
                IsBrightnessDown = false;
            }
            VolumeWindows7 volumeTemp = new VolumeWindows7();
            CurrVol = (int)(volumeTemp.MasterVolume * 100);

            volumeT.Text = CurrVol.ToString();
        }
 

        Brightness mBrightness = new Brightness();
        KeyboardHook mKeyboardHook = new KeyboardHook();
        byte winBackLight = 50;

        private void button5_Click(object sender, EventArgs e)
        {
            mBrightness.SetBrightness(winBackLight);
            mKeyboardHook.SetWindowsHook(true);
        }


    }
}
