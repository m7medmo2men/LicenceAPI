using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleApp1
{

    class Program
    {
        /*public static string GetIPAddress(){
            string IPAddress = string.Empty;
            IPHostEntry Host = default(IPHostEntry);
            string Hostname = null;
            Hostname = System.Environment.MachineName;
            Host = Dns.GetHostEntry(Hostname);
            foreach (IPAddress IP in Host.AddressList){
                if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork){
                    IPAddress = Convert.ToString(IP);
                }
            }
            return IPAddress;
        }*/

        /*public static string GetMacAddress()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Only consider Ethernet network interfaces
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    nic.OperationalStatus == OperationalStatus.Up)
                {
                    return nic.GetPhysicalAddress().ToString();
                }   
            }
            return null;
        }*/

        public static string GetMacAddress()
        {
            var macAddr = (
                from nic in NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == OperationalStatus.Up
                select nic.GetPhysicalAddress().ToString()
            ).FirstOrDefault();

            return macAddr;
        }   
        
        public static string CheckLicence()
        {
            try
            {
                string macAddress = GetMacAddress();
                var client = new RestClient("http://localhost:3000");
                var request = new RestRequest("licences/checkLicence", Method.POST);
                request.AddHeader("Accept", "application/json");
                request.AddJsonBody(new {mac = macAddress});
                IRestResponse response = client.Execute(request);
                if (response.ResponseStatus == ResponseStatus.Error) {
                    throw response.ErrorException;
                }
                var finalResponse = JObject.Parse(response.Content);
                return finalResponse["status"].ToString();
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "fail";
            }

        }

        static void Main(string[] args)
        {
            if (CheckLicence() == "success")
            {
                Console.WriteLine("Success");
                Console.WriteLine("Accessing The Portal....");
                Console.WriteLine("Processing Stores....");
                
            }
            else
                Console.WriteLine("Failed");
        }
    }
}