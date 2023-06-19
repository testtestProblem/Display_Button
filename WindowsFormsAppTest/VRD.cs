//Include from VRDControl V1.0.0.9_code
using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace VRDSDK
{
    public enum MB_RET
    {
        RET_SUCCESS = 0,
        RET_NO_RESPONSE = -1,
        RET_DATA_ERROR = -2,
        RET_DATA_LENGTH_ERROR = -3,
        RET_COMMAND_ERROR = -4,
        RET_INVALID_OPEN = -5,
        RET_ERROR_CODE = -6,
        RET_NOT_SUCCESS = -7,
        RET_UNKNOWN = -8
    };

    public class VRD
    {
        private System.IO.Ports.SerialPort m_SerialPort = new System.IO.Ports.SerialPort();
        Byte[] sendcmdbuf = new Byte[100];
        private int iRetryCount = 4;
        private int iRetryDelay = 20;
        private int iWriteDelay = 20;
        private int iReadDelay = 20;

        public bool Init(string comport, int baudrate)
        {
            m_SerialPort.Close();
            m_SerialPort.BaudRate = baudrate;
            string port = comport;
            m_SerialPort.PortName = port;
            m_SerialPort.ReadTimeout = 100;
            m_SerialPort.Parity = System.IO.Ports.Parity.None;
            m_SerialPort.StopBits = System.IO.Ports.StopBits.One;
            m_SerialPort.Handshake = System.IO.Ports.Handshake.None;
            m_SerialPort.DataBits = 8;
            m_SerialPort.ErrorReceived += new SerialErrorReceivedEventHandler(m_SerialPort_ErrorReceived);

            try
            {
                m_SerialPort.Open();
            }
            catch (Exception cc)
            {
                MessageBox.Show(string.Format("Unable to open the {0}", port), "Warning", MessageBoxButtons.OK);
                return false;
            }
            return true;

        }
        private bool IsOpened
        {
            get
            {
                return m_SerialPort.IsOpen;
            }
        }

        public bool Close()
        {
            try
            {
                if (!IsOpened)
                    return false;

                m_SerialPort.Close();
            }
            catch (Exception cc)
            {
                return false;
            }

            return true;
        }

        void m_SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            // MessageBox.Show(e.ToString(), "Warning", MessageBoxButtons.OK);
        }

        private int GetReply(byte[] recbuf)
        {
            int index = 0;
            int count = 0;

            Thread.Sleep(150);
            do
            {
                count = m_SerialPort.BytesToRead;
                if (count > 0)
                {
                    m_SerialPort.Read(recbuf, index, count);
                    index += count;
                }
                Thread.Sleep(50);
            }
            while (m_SerialPort.BytesToRead > 0);

            //Thread.Sleep(50);
            if (count == 0)
                return (int)MB_RET.RET_NO_RESPONSE;

            if (count < 3)
                return (int)MB_RET.RET_DATA_LENGTH_ERROR;

            byte BBC = WE_BCC_Encode(recbuf, (count - 1));

            if (BBC != recbuf[count - 1])
                return (int)MB_RET.RET_DATA_ERROR;

            return count;
        }

        private int GetReply(byte[] recbuf, int time)
        {
            int index = 0;
            int count = 0;

            Thread.Sleep(time);
            do
            {
                count = m_SerialPort.BytesToRead;
                if (count > 0)
                {
                    m_SerialPort.Read(recbuf, index, count);
                    index += count;
                }
                Thread.Sleep(50);
            }
            while (m_SerialPort.BytesToRead > 0);

            //Thread.Sleep(50);
            if (count == 0)
                return (int)MB_RET.RET_NO_RESPONSE;

            if (count < 3)
                return (int)MB_RET.RET_DATA_LENGTH_ERROR;

            byte BBC = WE_BCC_Encode(recbuf, (count - 1));

            if (BBC != recbuf[count - 1])
                return (int)MB_RET.RET_DATA_ERROR;

            return count;
        }

        private int GetReply_None_Check(byte[] recbuf)
        {
            int index = 0;
            int count = 0;

            Thread.Sleep(500);
            do
            {
                count = m_SerialPort.BytesToRead;
                if (count > 0)
                {
                    m_SerialPort.Read(recbuf, index, count);
                    index += count;
                }
                Thread.Sleep(50);
            }
            while (m_SerialPort.BytesToRead > 0);

            //Thread.Sleep(50);
            if (count == 0)
                return (int)MB_RET.RET_NO_RESPONSE;

            if ((count > 11) && (recbuf[0] > 0x30) && (recbuf[0] < 0x7F))
                return count;
            else
                return (int)MB_RET.RET_DATA_LENGTH_ERROR;
        }

        byte WE_BCC_Encode(byte[] puchMsg, int usDataLen)
        {
            byte uchBCC = 0x00;

            for (int i = 0; i < usDataLen; i++)
            {
                uchBCC = (byte)(uchBCC + puchMsg[i]);
            }

            uchBCC = (byte)((byte)((byte)(~uchBCC) + 1) & 0xff);

            return uchBCC;
        }

        public int Get_BIOS_BOM(out string version)
        {
            int iResult = (int)MB_RET.RET_NOT_SUCCESS;
            int iCount = 0;

            do
            {
                if (iCount != 0)
                    Thread.Sleep(iRetryDelay);

                iResult = CMD_Get_BIOS_BOM(out version);
                ++iCount;

            } while (iResult != (int)MB_RET.RET_SUCCESS && iCount < iRetryCount);

            return iResult;
        }

        public int CMD_Get_BIOS_BOM(out string version)
        {
            version = "";

            if (!IsOpened)
                return (int)MB_RET.RET_INVALID_OPEN;

            byte[] reccmdbuf = new byte[128];
            sendcmdbuf[0] = 0x04;
            sendcmdbuf[1] = 0xA0;
            sendcmdbuf[2] = 0x00;
            byte BBC = WE_BCC_Encode(sendcmdbuf, 3);
            sendcmdbuf[3] = BBC;

            m_SerialPort.Write(sendcmdbuf, 0, 4);
            int count = GetReply_None_Check(reccmdbuf);
            if (count < 0)
            {
                return count;
            }

            for (int i = 0; i < count; i++)
            {
                version += Convert.ToChar(reccmdbuf[i]);
            }

            return (int)MB_RET.RET_SUCCESS;
        }

        public int Get_BIOS_Name(out string version)
        {
            int iResult = (int)MB_RET.RET_NOT_SUCCESS;
            int iCount = 0;

            do
            {
                if (iCount != 0)
                    Thread.Sleep(iRetryDelay);

                iResult = CMD_Get_BIOS_Name(out version);
                ++iCount;

            } while (iResult != (int)MB_RET.RET_SUCCESS && iCount < iRetryCount);

            return iResult;
        }

        public int CMD_Get_BIOS_Name(out string version)
        {
            version = "";

            if (!IsOpened)
                return (int)MB_RET.RET_INVALID_OPEN;

            byte[] reccmdbuf = new byte[128];
            sendcmdbuf[0] = 0x04;
            sendcmdbuf[1] = 0xA0;
            sendcmdbuf[2] = 0x01;
            byte BBC = WE_BCC_Encode(sendcmdbuf, 3);
            sendcmdbuf[3] = BBC;

            m_SerialPort.Write(sendcmdbuf, 0, 4);
            int count = GetReply_None_Check(reccmdbuf);
            if (count < 0)
            {
                return count;
            }

            for (int i = 0; i < count; i++)
            {
                version += Convert.ToChar(reccmdbuf[i]);
            }

            return (int)MB_RET.RET_SUCCESS;
        }

        public int Set_LoadDefault()
        {
            int iResult = (int)MB_RET.RET_NOT_SUCCESS;
            int iCount = 0;

            do
            {
                if (iCount != 0)
                    Thread.Sleep(iRetryDelay);

                iResult = CMD_Set_LoadDefault();
                ++iCount;

            } while (iResult != (int)MB_RET.RET_SUCCESS && iCount < iRetryCount);

            return iResult;
        }

        public int CMD_Set_LoadDefault()
        {
            if (!IsOpened)
                return (int)MB_RET.RET_INVALID_OPEN;

            byte[] reccmdbuf = new byte[32];
            sendcmdbuf[0] = 0x05;
            sendcmdbuf[1] = 0x40;
            sendcmdbuf[2] = 0x21;
            sendcmdbuf[3] = 0x00;
            byte BBC = WE_BCC_Encode(sendcmdbuf, 4);
            sendcmdbuf[4] = BBC;

            m_SerialPort.Write(sendcmdbuf, 0, 5);
            int count = GetReply(reccmdbuf, 1000);
            if (count < 0)
            {
                return count;
            }

            if (reccmdbuf[1] == 0x0C)
                return (int)MB_RET.RET_SUCCESS;
            else if (reccmdbuf[1] == 0x0B)
                return (int)MB_RET.RET_NOT_SUCCESS;

            return (int)MB_RET.RET_ERROR_CODE;
        }

        public int Set_VRD_BL_Index(byte value)
        {
            int iResult = (int)MB_RET.RET_NOT_SUCCESS;
            int iCount = 0;

            do
            {
                if (iCount != 0)
                    Thread.Sleep(iRetryDelay);

                iResult = CMD_Set_VRD_BL_Index(value);
                ++iCount;

            } while (iResult != (int)MB_RET.RET_SUCCESS && iCount < iRetryCount);

            return iResult;
        }

        public int CMD_Set_VRD_BL_Index(byte value)
        {
            byte cmd = 0x00;
            cmd = 0x10;

            if (!IsOpened)
                return (int)MB_RET.RET_INVALID_OPEN;

            byte[] reccmdbuf = new byte[32];
            sendcmdbuf[0] = 0x05;
            sendcmdbuf[1] = 0x40;
            sendcmdbuf[2] = cmd;
            sendcmdbuf[3] = value;
            byte BBC = WE_BCC_Encode(sendcmdbuf, 4);
            sendcmdbuf[4] = BBC;

            m_SerialPort.Write(sendcmdbuf, 0, 5);
            int count = GetReply(reccmdbuf, iWriteDelay);
            if (count < 0)
            {
                return count;
            }

            if (reccmdbuf[1] == 0x0C)
                return (int)MB_RET.RET_SUCCESS;
            else if (reccmdbuf[1] == 0x0B)
                return (int)MB_RET.RET_NOT_SUCCESS;

            return (int)MB_RET.RET_ERROR_CODE;
        }

        public int Get_VRD_BL_Index(out byte value)
        {
            int iResult = (int)MB_RET.RET_NOT_SUCCESS;
            int iCount = 0;

            do
            {
                if (iCount != 0)
                    Thread.Sleep(iRetryDelay);

                iResult = CMD_Get_VRD_BL_Index(out value);
                ++iCount;

            } while (iResult != (int)MB_RET.RET_SUCCESS && iCount < iRetryCount);

            return iResult;
        }

        public int CMD_Get_VRD_BL_Index(out byte value)
        {
            byte cmd = 0x00;
            value = 0;
            cmd = 0x10;

            if (!IsOpened)
                return (int)MB_RET.RET_INVALID_OPEN;

            byte[] reccmdbuf = new byte[32];
            sendcmdbuf[0] = 0x04;
            sendcmdbuf[1] = 0x30;
            sendcmdbuf[2] = cmd;
            byte BBC = WE_BCC_Encode(sendcmdbuf, 3);
            sendcmdbuf[3] = BBC;

            m_SerialPort.Write(sendcmdbuf, 0, 4);
            int count = GetReply(reccmdbuf, iReadDelay);
            if (count < 0)
            {
                return count;
            }

            if (reccmdbuf[1] != cmd)
            {
                return (int)MB_RET.RET_COMMAND_ERROR;
            }

            value = reccmdbuf[2];
            return (int)MB_RET.RET_SUCCESS;
        }

        public int Set_VRD_BL_IndexPWM(byte index, byte value)
        {
            int iResult = (int)MB_RET.RET_NOT_SUCCESS;
            int iCount = 0;

            do
            {
                if (iCount != 0)
                    Thread.Sleep(iRetryDelay);

                iResult = CMD_Set_VRD_BL_IndexPWM(index, value);
                ++iCount;

            } while (iResult != (int)MB_RET.RET_SUCCESS && iCount < iRetryCount);

            return iResult;
        }

        public int CMD_Set_VRD_BL_IndexPWM(byte index, byte value)
        {
            byte cmd = 0x00;
            cmd = (byte)(index + 0x30);

            if (!IsOpened)
                return (int)MB_RET.RET_INVALID_OPEN;

            byte[] reccmdbuf = new byte[32];
            sendcmdbuf[0] = 0x05;
            sendcmdbuf[1] = 0x40;
            sendcmdbuf[2] = cmd;
            sendcmdbuf[3] = value;
            byte BBC = WE_BCC_Encode(sendcmdbuf, 4);
            sendcmdbuf[4] = BBC;

            m_SerialPort.Write(sendcmdbuf, 0, 5);
            int count = GetReply(reccmdbuf, iWriteDelay);
            if (count < 0)
            {
                return count;
            }

            if (reccmdbuf[1] == 0x0C)
                return (int)MB_RET.RET_SUCCESS;
            else if (reccmdbuf[1] == 0x0B)
                return (int)MB_RET.RET_NOT_SUCCESS;

            return (int)MB_RET.RET_ERROR_CODE;
        }

        public int Get_VRD_BL_IndexPWM(byte index, out byte value)
        {
            int iResult = (int)MB_RET.RET_NOT_SUCCESS;
            int iCount = 0;

            do
            {
                if (iCount != 0)
                    Thread.Sleep(iRetryDelay);

                iResult = CMD_Get_VRD_BL_IndexPWM(index, out value);
                ++iCount;

            } while (iResult != (int)MB_RET.RET_SUCCESS && iCount < iRetryCount);

            return iResult;
        }

        public int CMD_Get_VRD_BL_IndexPWM(byte index, out byte value)
        {
            byte cmd = 0x00;
            value = 0;
            cmd = (byte)(index + 0x30);

            if (!IsOpened)
                return (int)MB_RET.RET_INVALID_OPEN;

            byte[] reccmdbuf = new byte[32];
            sendcmdbuf[0] = 0x04;
            sendcmdbuf[1] = 0x30;
            sendcmdbuf[2] = cmd;
            byte BBC = WE_BCC_Encode(sendcmdbuf, 3);
            sendcmdbuf[3] = BBC;

            m_SerialPort.Write(sendcmdbuf, 0, 4);
            int count = GetReply(reccmdbuf, iReadDelay);
            if (count < 0)
            {
                return count;
            }

            if (reccmdbuf[1] != cmd)
            {
                return (int)MB_RET.RET_COMMAND_ERROR;
            }

            value = reccmdbuf[2];
            return (int)MB_RET.RET_SUCCESS;
        }

        public int CMD_Set_VRD_BL_PWM_LowByte(byte value)
        {
            byte cmd = 0x00;
            cmd = 0x3C;

            if (!IsOpened)
                return (int)MB_RET.RET_INVALID_OPEN;

            byte[] reccmdbuf = new byte[32];
            sendcmdbuf[0] = 0x05;
            sendcmdbuf[1] = 0x40;
            sendcmdbuf[2] = cmd;
            sendcmdbuf[3] = value;
            byte BBC = WE_BCC_Encode(sendcmdbuf, 4);
            sendcmdbuf[4] = BBC;

            m_SerialPort.Write(sendcmdbuf, 0, 5);
            int count = GetReply(reccmdbuf, iWriteDelay);
            if (count < 0)
            {
                return count;
            }

            if (reccmdbuf[1] == 0x0C)
                return (int)MB_RET.RET_SUCCESS;
            else if (reccmdbuf[1] == 0x0B)
                return (int)MB_RET.RET_NOT_SUCCESS;

            return (int)MB_RET.RET_ERROR_CODE;
        }

        public int Set_ECDIS_Backlignt(byte index, byte value)
        {
            int iResult = (int)MB_RET.RET_NOT_SUCCESS;
            int iCount = 0;

            do
            {
                if (iCount != 0)
                    Thread.Sleep(iRetryDelay);

                iResult = CMD_Set_ECDIS_Backlignt(index, value);
                ++iCount;

            } while (iResult != (int)MB_RET.RET_SUCCESS && iCount < iRetryCount);

            return iResult;
        }

        public int CMD_Set_ECDIS_Backlignt(byte index, byte value)
        {
            byte cmd = 0x00;
            cmd = index;

            if (!IsOpened)
                return (int)MB_RET.RET_INVALID_OPEN;

            byte[] reccmdbuf = new byte[32];
            sendcmdbuf[0] = 0x05;
            sendcmdbuf[1] = 0x02;
            sendcmdbuf[2] = cmd;
            sendcmdbuf[3] = value;
            byte BBC = WE_BCC_Encode(sendcmdbuf, 4);
            sendcmdbuf[4] = BBC;

            m_SerialPort.Write(sendcmdbuf, 0, 5);
            int count = GetReply(reccmdbuf, iWriteDelay);
            if (count < 0)
            {
                return count;
            }

            if ((reccmdbuf[0] != 0x05) || (reccmdbuf[1] != 0xFF) || (reccmdbuf[2] != cmd) || (reccmdbuf[3] != value))
            {
                return (int)MB_RET.RET_NOT_SUCCESS;
            }

            return (int)MB_RET.RET_SUCCESS;
        }

        public int Get_ECDIS_Backlignt(byte index, out byte value)
        {
            int iResult = (int)MB_RET.RET_NOT_SUCCESS;
            int iCount = 0;

            do
            {
                if (iCount != 0)
                    Thread.Sleep(iRetryDelay);

                iResult = CMD_Get_ECDIS_Backlignt(index, out value);
                ++iCount;

            } while (iResult != (int)MB_RET.RET_SUCCESS && iCount < iRetryCount);

            return iResult;
        }

        public int CMD_Get_ECDIS_Backlignt(byte index, out byte value)
        {
            byte cmd = 0x00;
            value = 0;
            cmd = index;

            if (!IsOpened)
                return (int)MB_RET.RET_INVALID_OPEN;

            byte[] reccmdbuf = new byte[32];
            sendcmdbuf[0] = 0x04;
            sendcmdbuf[1] = 0x03;
            sendcmdbuf[2] = cmd;
            byte BBC = WE_BCC_Encode(sendcmdbuf, 3);
            sendcmdbuf[3] = BBC;

            m_SerialPort.Write(sendcmdbuf, 0, 4);
            int count = GetReply(reccmdbuf, iReadDelay);
            if (count < 0)
            {
                return count;
            }

            if ((reccmdbuf[0] != 0x05) || (reccmdbuf[1] != 0xFF) || (reccmdbuf[2] != cmd))
            {
                return (int)MB_RET.RET_NOT_SUCCESS;
            }

            value = reccmdbuf[3];
            return (int)MB_RET.RET_SUCCESS;
        }

        public int Get_ECDIS_Color_Table(byte mode, byte table, out byte red, out byte green, out byte blue)
        {
            int iResult = (int)MB_RET.RET_NOT_SUCCESS;
            int iCount = 0;

            do
            {
                if (iCount != 0)
                    Thread.Sleep(iRetryDelay);

                iResult = CMD_Get_ECDIS_Color_Table(mode, table, out red, out green, out blue);
                ++iCount;

            } while (iResult != (int)MB_RET.RET_SUCCESS && iCount < iRetryCount);

            return iResult;
        }

        public int CMD_Get_ECDIS_Color_Table(byte mode, byte table, out byte red, out byte green, out byte blue)
        {
            red = 0;
            green = 0;
            blue = 0;

            if (!IsOpened)
                return (int)MB_RET.RET_INVALID_OPEN;

            byte[] reccmdbuf = new byte[32];
            sendcmdbuf[0] = 0x05;
            sendcmdbuf[1] = 0x01;
            sendcmdbuf[2] = mode;
            sendcmdbuf[3] = table;
            byte BBC = WE_BCC_Encode(sendcmdbuf, 4);
            sendcmdbuf[4] = BBC;

            m_SerialPort.Write(sendcmdbuf, 0, 5);
            int count = GetReply(reccmdbuf, iReadDelay);
            if (count < 0)
            {
                return count;
            }

            if ((reccmdbuf[0] != 0x08) || (reccmdbuf[1] != 0xFF) || (reccmdbuf[2] != mode))
            {
                return (int)MB_RET.RET_NOT_SUCCESS;
            }

            red = reccmdbuf[4];
            green = reccmdbuf[5];
            blue = reccmdbuf[6];

            return (int)MB_RET.RET_SUCCESS;
        }

        public int Set_ECDIS_Color_Table(byte mode, byte table, byte red, byte green, byte blue)
        {
            int iResult = (int)MB_RET.RET_NOT_SUCCESS;
            int iCount = 0;

            do
            {
                if (iCount != 0)
                    Thread.Sleep(iRetryDelay);

                iResult = CMD_Set_ECDIS_Color_Table(mode, table, red, green, blue);
                ++iCount;

            } while (iResult != (int)MB_RET.RET_SUCCESS && iCount < iRetryCount);

            return iResult;
        }

        public int CMD_Set_ECDIS_Color_Table(byte mode, byte table, byte red, byte green, byte blue)
        {
            if (!IsOpened)
                return (int)MB_RET.RET_INVALID_OPEN;

            byte[] reccmdbuf = new byte[32];
            sendcmdbuf[0] = 0x08;
            sendcmdbuf[1] = 0x00;
            sendcmdbuf[2] = mode;
            sendcmdbuf[3] = table;
            sendcmdbuf[4] = red;
            sendcmdbuf[5] = green;
            sendcmdbuf[6] = blue;
            byte BBC = WE_BCC_Encode(sendcmdbuf, 7);
            sendcmdbuf[7] = BBC;

            m_SerialPort.Write(sendcmdbuf, 0, 8);
            int count = GetReply(reccmdbuf, iWriteDelay);
            if (count < 0)
            {
                return count;
            }

            if ((reccmdbuf[0] != 0x08) || (reccmdbuf[1] != 0xFF) || (reccmdbuf[2] != mode))
            {
                return (int)MB_RET.RET_NOT_SUCCESS;
            }

            return (int)MB_RET.RET_SUCCESS;
        }

        public int Get_ECDIS_Package(byte num, out byte[] buffer)
        {
            int iResult = (int)MB_RET.RET_NOT_SUCCESS;
            int iCount = 0;

            do
            {
                if (iCount != 0)
                    Thread.Sleep(iRetryDelay);

                iResult = CMD_Get_ECDIS_Package(num, out buffer);
                ++iCount;

            } while (iResult != (int)MB_RET.RET_SUCCESS && iCount < iRetryCount);

            return iResult;
        }

        public int CMD_Get_ECDIS_Package(byte num, out byte[] buffer)
        {
            byte red = 255;
            byte green = 255;
            byte blue = 255;

            int iRet = 0;
            byte iMode = 0;
            byte iDataIdx = 0;
            byte iLen = 10;

            if (num == 18)
                iLen = 9;
            else
                iLen = 10;

            buffer = new byte[iLen * 3];

            if (num > 18)
                return (int)MB_RET.RET_DATA_LENGTH_ERROR;

            for (int i = 0; i < iLen; i++)
            {
                int idx = ((num * 10) + i + 1);
                if (idx >= 127)
                {
                    idx = idx - 126;
                    iMode = 2;

                }
                else if (idx >= 64)
                {
                    idx = idx - 63;
                    iMode = 1;
                }

                //Trace.WriteLine("Mode=" + iMode + ", idx=" + idx.ToString());
                iRet += CMD_Get_ECDIS_Color_Table(iMode, (byte)idx, out red, out green, out blue);
                if (iRet != (int)MB_RET.RET_SUCCESS)
                    return (int)MB_RET.RET_NOT_SUCCESS;
                else
                {
                    //Trace.WriteLine("Mode=" + iMode + ", idx=" + idx.ToString() + "R=" + red.ToString() + ",G=" + green.ToString() + ",B=" + blue.ToString());
                    buffer[iDataIdx++] = red;
                    buffer[iDataIdx++] = green;
                    buffer[iDataIdx++] = blue;
                }
            }

            return (int)MB_RET.RET_SUCCESS;
        }

        public int CMD_Get_VRD_Light_Sensor_Value(out byte[] value)
        {
            byte cmd = 0x00;
            value = new byte[2];
            value[0] = 0;
            value[1] = 0;
            cmd = 0x40;

            if (!IsOpened)
                return (int)MB_RET.RET_INVALID_OPEN;

            byte[] reccmdbuf = new byte[32];
            sendcmdbuf[0] = 0x04;
            sendcmdbuf[1] = 0x30;
            sendcmdbuf[2] = 0x40;
            byte BBC = WE_BCC_Encode(sendcmdbuf, 3);
            sendcmdbuf[3] = BBC;

            m_SerialPort.Write(sendcmdbuf, 0, 4);
            int count = GetReply(reccmdbuf, iReadDelay);
            if (count < 0)
            {
                return count;
            }

            if (reccmdbuf[1] != cmd)
            {
                return (int)MB_RET.RET_COMMAND_ERROR;
            }

            value[0] = reccmdbuf[2];
            value[1] = reccmdbuf[3];
            return (int)MB_RET.RET_SUCCESS;
        }

        public string PackageDataToString(byte[] buffer, int count)
        {
            string s = "";
            for (int i = 0; i < count; i++)
            {
                s = s + String.Format("{0:X2}", buffer[i]) + " ";
            }

            //Trace.WriteLine("dbg = " + s);
            return s;
        }

        public void OutputRS232DebugMessgae(byte[] buffer, int count)
        {
            string s = "";
            for (int i = 0; i < count; i++)
            {
                s = s + String.Format("{0:X2}", buffer[i]) + " ";
            }

            Trace.WriteLine("dbg = " + s);
        }
    }
}
