using SnmpSharpNet;
using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;

namespace WM.Common.Utils
{
    public class CreateSNMPTrapsHelper
    {
        public static string GetLocalIPAddress()
        {
            foreach (IPAddress address in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                    return address.ToString();
            }
            throw new Exception("Local IP Address Not Found!");
        }

        public void CreateIncident(string description, string summary, string incidentType)
        {
            int specificTrap = !(incidentType == "High") ? (!(incidentType == "Medium") ? 1 : 2) : 3;
            IPAddress address = Dns.GetHostEntry(ConfigurationManager.AppSettings["SNMPServer"]).AddressList[0];
            string localIpAddress = CreateSNMPTrapsHelper.GetLocalIPAddress();
            if (address == null || localIpAddress == null)
                Console.WriteLine("invalid host or wrong IP address found");
            else
                new TrapAgent().SendV1Trap(new IpAddress(address), 162, "public", new Oid(".1.3.6.1.4.1.4767.1.1.1"), new IpAddress(localIpAddress), 6, specificTrap, 0U, new VbCollection()
        {
          {
            new Oid(".1.3.6.1.4.1.4767.1.1.1"),
            (AsnType) new OctetString("")
          },
          {
            new Oid(".1.3.6.1.4.1.4767.1.1.1"),
            (AsnType) new OctetString(description)
          },
          {
            new Oid(".1.3.6.1.4.1.4767.1.1.1"),
            (AsnType) new TimeTicks()
          },
          {
            new Oid(".1.3.6.1.4.1.4767.1.1.1"),
            (AsnType) new OctetString(summary)
          }
        });
        }
    }
}

