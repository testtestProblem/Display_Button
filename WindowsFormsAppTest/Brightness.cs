using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using VRDSDK;

namespace ScreenBrightness
{
    class Brightness
    {
        public static VRD m_MCUSerial = new VRD();

        private int TempBrightnessUpValue = 0;
        private int TempBrightnessDownValue = 0;

        //get the actual percentage of brightness
        public int GetBrightness()
        { 
            { 
                //define scope (namespace)
                System.Management.ManagementScope s = new System.Management.ManagementScope("root\\WMI");

                //define query
                System.Management.SelectQuery q = new System.Management.SelectQuery("WmiMonitorBrightness");

                //output current brightness
                System.Management.ManagementObjectSearcher mos = new System.Management.ManagementObjectSearcher(s, q);

                System.Management.ManagementObjectCollection moc = mos.Get();

                //store result
                byte curBrightness = 0;
                foreach (System.Management.ManagementObject o in moc)
                {
                    curBrightness = (byte)o.GetPropertyValue("CurrentBrightness");
                    break; //only work on the first object
                }

                moc.Dispose();
                mos.Dispose();

                return (int)curBrightness;
            }
        }

        //array of valid brightness values in percent
        public byte[] GetBrightnessLevels()
        { 
            {
                //define scope (namespace)
                System.Management.ManagementScope s = new System.Management.ManagementScope("root\\WMI");

                //define query
                System.Management.SelectQuery q = new System.Management.SelectQuery("WmiMonitorBrightness");

                //output current brightness
                System.Management.ManagementObjectSearcher mos = new System.Management.ManagementObjectSearcher(s, q);
                byte[] BrightnessLevels = new byte[0];

                try
                {
                    System.Management.ManagementObjectCollection moc = mos.Get();

                    //store result
                    foreach (System.Management.ManagementObject o in moc)
                    {
                        BrightnessLevels = (byte[])o.GetPropertyValue("Level");
                        break; //only work on the first object
                    }
                    moc.Dispose();
                    mos.Dispose();
                }
                catch (Exception)
                {
                    MessageBox.Show("Sorry, Your System does not support this brightness control...");
                }
                return BrightnessLevels;
            }
        }

        public void SetBrightness(byte targetBrightness)
        { 
            {
                //define scope (namespace)
                System.Management.ManagementScope s = new System.Management.ManagementScope("root\\WMI");

                //define query
                System.Management.SelectQuery q = new System.Management.SelectQuery("WmiMonitorBrightnessMethods");

                //output current brightness
                System.Management.ManagementObjectSearcher mos = new System.Management.ManagementObjectSearcher(s, q);

                System.Management.ManagementObjectCollection moc = mos.Get();

                foreach (System.Management.ManagementObject o in moc)
                {
                    o.InvokeMethod("WmiSetBrightness", new Object[] { UInt32.MaxValue, targetBrightness }); //note the reversed order - won't work otherwise!
                    break; //only work on the first object
                }

                moc.Dispose();
                mos.Dispose();
            }
        } 
    }
}
