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

    public static string getLicence() {
        var client = new RestClient("http://localhost:3000");
        var request = new RestRequest("licences/1", Method.GET);
        request.AddHeader("Accept", "application/json");
        IRestResponse response = client.Execute(request);
        if (response.ResponseStatus == ResponseStatus.Error) {
            throw response.ErrorException;
        }
            
        //return response.Content;
        var finalResponse = JObject.Parse(response.Content);
        return finalResponse["data"].ToString();
    }

    public static void updateLicenceFile() {
            
        var data = getLicence(); // Get Latest Data From Server

        XNode node = JsonConvert.DeserializeXNode(data, "Root");
        string EncryptXml = Encryptt(node.ToString());
        File.WriteAllText("licence.xml", EncryptXml);

    }
    
    public static string readLicenceXml()
    {
        XmlDocument xdoc = new XmlDocument();
        xdoc.Load("D:\\Work\\dotnet test\\licence.xml");
        string encryptedXml = Encryptt(xdoc.InnerXml);
        return encryptedXml;
    }

    public static bool validate(XmlDocument xdoc)
    {
        string Active = xdoc.GetElementsByTagName("Active")[0].InnerText;
        string mac = xdoc.GetElementsByTagName("mac")[0].InnerText;
        string LicenceType = xdoc.GetElementsByTagName("LicenceType")[0].InnerText;
        string EndDate = xdoc.GetElementsByTagName("EndDate")[0].InnerText;
        string EndTimestamp = xdoc.GetElementsByTagName("EndTimestamp")[0].InnerText;
        Console.WriteLine(Active + " " + mac + " " + LicenceType + " " + EndDate + " " + EndTimestamp);
        return true;
    }

    static void Main(string[] args)
    {

        if (File.Exists("licence.xml")) {
            string encryptedXml = readLicenceXml();
            string decryptedXml = Decryptt(encryptedXml);

            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(decryptedXml);

            validate(xdoc);


        } else {
            // Call The Servar to update
            Console.WriteLine("ELSE");
            updateLicenceFile();
        }

    }
}

