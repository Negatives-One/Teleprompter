using System;
using System.IO.Ports;
using System.Management;
using System.Runtime.InteropServices;

namespace Janga
{
    class Program
    {
        [DllImport("user32.dll")] static extern bool SetCursorPos(int X, int Y);
        [DllImport("user32.dll")] static extern bool GetCursorPos(out POINT point);

        public struct POINT
        {
            public int x;
            public int y;
        }

        public static POINT GetMousePosition()
        {
            POINT pos;
            GetCursorPos(out pos);
            return pos;
        }

        public static void SetCursorPosition(int x, int y)
        {
            SetCursorPos(x, y);
        }
        private static SerialPort port;
        public static int height;
        static void Main(string[] args)
        {
            var managementScope = new System.Management.ManagementScope();
            managementScope.Connect();
            var q = new System.Management.ObjectQuery("SELECT CurrentHorizontalResolution, CurrentVerticalResolution FROM Win32_VideoController");
            var searcher = new System.Management.ManagementObjectSearcher(managementScope, q);
            var records = searcher.Get();
            foreach (var record in records)
            {
                if (!int.TryParse(record.GetPropertyValue("CurrentVerticalResolution").ToString(), out height))
                {
                    throw new Exception("Throw some exception");
                }
            }

            port = new SerialPort();
            port.BaudRate = 9600;
            port.PortName = AutodetectArduinoPort();//Set your board COM
            port.ReadTimeout = 9999;
            port.RtsEnable = true;
            port.Open();

            float floatValue = 0f;
            while (true)
            {
                string linha = port.ReadLine();
                try
                {
                    floatValue = float.Parse(linha);
                    if (floatValue >= 88f)
                    {
                        floatValue = Map(floatValue, 88f, 100f, 0.5f, 1f);
                    }
                    else
                    {
                        floatValue = Map(floatValue, 0f, 88f, 0f, 0.5f);
                    }
                    int heightPercentage = (int)(floatValue * height);
                    SetCursorPosition(GetMousePosition().x, heightPercentage);
                }
                catch (Exception)
                {
                    Console.WriteLine(linha);
                }
            }
        }

        public static string AutodetectArduinoPort()
        {
            ManagementScope connectionScope = new ManagementScope();
            SelectQuery serialQuery = new SelectQuery("SELECT * FROM Win32_SerialPort");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(connectionScope, serialQuery);

            try
            {
                foreach (ManagementObject item in searcher.Get())
                {
                    string desc = item["Description"].ToString();
                    string deviceId = item["DeviceID"].ToString();

                    if (desc.Contains("Arduino"))
                    {
                        return deviceId;
                    }
                }
            }
            catch (ManagementException e)
            {
                /* Do Nothing */
            }

            return null;
        }

        public static float Map(float value, float start1, float stop1, float start2, float stop2)
        {
            float outgoing = start2 + (stop2 - start2) * ((value - start1) / (stop1 - start1));
            return outgoing;
        }


    }
}
