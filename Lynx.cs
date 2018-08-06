using Canberra.DSA3K.DataTypes.Communications;
using System;
using System.Net;

namespace Rsx.CAM
{
    public class Linx
    {
        private IDevice device = null;

        public static String GetLocalAddress()
        {
            IPHostEntry Entry = Dns.GetHostEntry(System.Net.Dns.GetHostName());
            String LocalAddr = "";
            int AdapterCnt = 0;
            foreach (IPAddress Address in Entry.AddressList)
            {
                //Skip loopbacks
                if (IPAddress.IsLoopback(Address)) continue;

                AdapterCnt++;
                if (0 == LocalAddr.Length) { LocalAddr = Address.ToString(); }
            }

        
            if (AdapterCnt > 1) return "";

            //We found 1 adapter so return it's address
            return LocalAddr;
        }

        public Linx()
        {
            // Canberra.DSA3K.DataTypes.Communications.DeviceFactory.CreateInstance(Canberra.DSA3K.DataTypes.Communications.DeviceFactory.DeviceInterface);

            object o = DeviceFactory.CreateInstance(DeviceFactory.DeviceInterface.IDevice);

            device = (IDevice)o;

            string local = "10.0.3.122";

            string linx = "192.168.114.001";
            try
            {
                device.Open(local, linx);
            }
            catch (Canberra.DSA3K.DataTypes.DeviceException ex)
            {
                string exce = ex.Description;
            }
            bool opened = device.IsOpen;

            object real = device.GetProperty("Real Time");

            string retext = real.ToString();
        }
    }
}