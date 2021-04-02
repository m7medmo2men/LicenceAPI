using System;
using System.Globalization;
using System.Runtime;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using System.Text;
using System.Security.Cryptography;

class Program
{
 
    public static string Encryptt(string encryptString)
    {
        string EncryptionKey = "RE358P71305KMCHA8721DFA684ZXCZNCXD0QMVJD4220L";
        byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }
                encryptString = Convert.ToBase64String(ms.ToArray());
            }
        }
        return encryptString;
    }

    public static string Decryptt(string cipherText)
    {
        string EncryptionKey = "RE358P71305KMCHA8721DFA684ZXCZNCXD0QMVJD4220L";
        cipherText = cipherText.Replace(" ", "+");
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }
                cipherText = Encoding.Unicode.GetString(ms.ToArray());
            }
        }
        return cipherText;
    }

    public static string GetMacAddress()
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
            return finalResponse["message"].ToString();

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return "fail";
        }

    }

    public static void UpdateLicence(string xml)
    {
        /**
         * Take a string that represents the xml file
         * Encrypt it and save it
         * 
         * Replace The Old encrypted xml file
         * */
        string encryptedNewXml = Encryptt(xml);
        File.WriteAllText("D:\\work\\licence.xml", encryptedNewXml);
    }

    public static void GetDataFromServer() {
            
        var data = CheckLicence(); // Get Latest Data From Server

        if (data == "fail") {
            Console.WriteLine("Not Authorized ..");
        } else { 
            XNode node = JsonConvert.DeserializeXNode(data, "Root");
            UpdateLicence(node.ToString());
        }
    }

    public static void ValidateDate(ref XmlDocument xdoc) // This like end of day proc.
    {
        string Date = xdoc.GetElementsByTagName("Date")[0].InnerText;
        DateTime lastDate = Convert.ToDateTime(Date);
        DateTime ClientDate = DateTime.UtcNow.Date;
        int res = DateTime.Compare(lastDate, ClientDate);
        if (res > 0)
        {
            // systemDateNow < lastDate
            Console.WriteLine("Date Was Changed ");
            
            xdoc.GetElementsByTagName("Active")[0].InnerText = "N";
            xdoc.GetElementsByTagName("Date")[0].InnerText = lastDate.AddDays(1.0).ToShortDateString();
            UpdateLicence(xdoc.InnerXml);
        }
        else
        {
            xdoc.GetElementsByTagName("Date")[0].InnerText = lastDate.AddDays(1.0).ToShortDateString();
            UpdateLicence(xdoc.InnerXml);
        }
    }
    
    public static string readLicence()
    {
        string encryptedXml = File.ReadAllText("D:\\work\\licence.xml");
        return encryptedXml;
    }

    
    static void Main(string[] args)
    {

        if (!File.Exists("D:\\work\\licence.xml")) {
            GetDataFromServer();
        }
        
        try
        {
            string encryptedXml = readLicence();
            string decryptedXml = Decryptt(encryptedXml);
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(decryptedXml);
            ValidateDate(ref xdoc);
            string licenceStatus = xdoc.GetElementsByTagName("Active")[0].InnerText;
            if (licenceStatus == "Y")
            {
                Console.WriteLine("Authorized ...");
                Console.WriteLine("Processing Stores ....");
            }
            else
            {
                Console.WriteLine("Your Licence Runs in Issue ...");
                Console.WriteLine("Please Contact StarCom ....");
                GetDataFromServer();
            }
        } 
        catch (Exception e)
        {
            Console.WriteLine("Licence Was Manipulated ...");
            /* Terminate The Service */
        }

        

    }
}

