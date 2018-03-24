using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace sendUDP
{
    class Program
    {
        private static void Main(string[] args)
        {
            string macAddress = args[0];  
            string ip = args[1];
            int port = Convert.ToInt32(args[2]);

            macAddress = Regex.Replace(macAddress, "[-|:]", "");       // Remove any semicolons or minus characters present in our MAC address

            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
            {
                EnableBroadcast = true
            };

            int payloadIndex = 0;

            /* The magic packet is a broadcast frame containing anywhere within its payload 6 bytes of all 255 (FF FF FF FF FF FF in hexadecimal), followed by sixteen repetitions of the target computer's 48-bit MAC address, for a total of 102 bytes. */
            byte[] payload = new byte[1024];    // Our packet that we will be broadcasting

            // Add 6 bytes with value 255 (FF) in our payload
            for (int i = 0; i < 6; i++)
            {
                payload[payloadIndex] = 255;
                payloadIndex++;
            }

            // Repeat the device MAC address sixteen times
            for (int i = 0; i < 16; i++)
                for (int k = 0; k < macAddress.Length; k += 2)
                {
                    var s = macAddress.Substring(k, 2);
                    payload[payloadIndex] = byte.Parse(s, NumberStyles.HexNumber);
                    payloadIndex++;
                }
            int a = 0;
            try
            {
                a = sock.SendTo(payload, new IPEndPoint(IPAddress.Parse(ip), port));  // Broadcast our packet
            }
            catch
            {
                Console.WriteLine("Some trouble. Send bytes = " + a);
                Console.ReadKey();
            }
            if (a != 1024)
            {
                Console.WriteLine("Some trouble. Send bytes = "+ a);
                Console.ReadKey();
            }
            sock.Close();
        }

      
    }
}
