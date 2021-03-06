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
    // public static XmlDocument xdoc;
    public static string MALL_NAME;
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
            // return finalResponse["message"].ToString();
            if (finalResponse["status"].ToString() == "success")
            {
                MALL_NAME = finalResponse["message"]["MallName"].ToString();
                return finalResponse["message"].ToString();
            }
            else
            {
                return finalResponse["status"].ToString();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return "fail";
        }

    }

    public static void GetDataFromServer() {
            
        var data = CheckLicence(); // Get Latest Data From Server

        if (data == "fail") {
            Console.WriteLine("Not Authorized ..");
        } else {
            XNode node = JsonConvert.DeserializeXNode(data, "Root"); // Convert from json to xml
            UpdateLicence(node.ToString()); // pass the xml file in string format to be encrypted
        }
    }

    // Boolean or Void ??? Msh 3aref Lesa
    public static bool ValidateMac(ref XmlDocument xdoc)
    {
        if (xdoc.GetElementsByTagName("mac")[0].InnerText == GetMacAddress())
            return true;
        else
        {
            string mallName = xdoc.GetElementsByTagName("MallName")[0].InnerText;
            xdoc.GetElementsByTagName("Active")[0].InnerText = "N";
            NotifyServer($"${mallName} runs the application on different device");
            return false;
        }
    }

    public static void ValidateDate(ref XmlDocument xdoc) // This like end of day proc.
    {
        string mallName = xdoc.GetElementsByTagName("MallName")[0].InnerText;
        string Date = xdoc.GetElementsByTagName("Date")[0].InnerText; // This Should be today date even if the user changes computer's date
        // Date = "4/27/2021";
        string ExpiryDateStr = xdoc.GetElementsByTagName("EndDate")[0].InnerText; // End date of licence

        DateTime lastDate = Convert.ToDateTime(Date);
        DateTime ExpiryDate = Convert.ToDateTime(ExpiryDateStr);
        DateTime ClientDate = DateTime.UtcNow.Date;
        

        // Move To Server
        // Msh Moshkela delw2ty
        if (lastDate.AddDays(5.0) > ExpiryDate)
        {
            string message = $"This {mallName} Licence will end in {ExpiryDate}";
            NotifyServer(message);
        }


        int res = DateTime.Compare(lastDate, ClientDate);
        if (res > 0)
        {
            // clientDate < lastDate -- Mall Manipulated His System Date
            Console.WriteLine("Date Was Changed ");
            NotifyServer("Mall XX Changed The Date");
            xdoc.GetElementsByTagName("Active")[0].InnerText = "N"; // change the active flag
            xdoc.GetElementsByTagName("Date")[0].InnerText = lastDate.AddDays(1.0).ToShortDateString(); // increment the date for next day
            UpdateLicence(xdoc.InnerXml); // This function takes the modified xml string then encrypt it and save it to computer
        }
        else if (res == 0)
        {
            // User Mo7tram
            xdoc.GetElementsByTagName("Date")[0].InnerText = lastDate.AddDays(1.0).ToShortDateString();
            UpdateLicence(xdoc.InnerXml);
        } 
        else
        {
            // clientDate > lastDate -- Mall Forget To Open Middleware
            xdoc.GetElementsByTagName("Date")[0].InnerText = ClientDate.ToShortDateString();
            NotifyServer($"Mall XXX didn't use middelware since {lastDate}");
        }
    }
    
    public static string readLicence()
    {
        // Read The Encrypted XML File From My Computer and Return it
        string encryptedXml = File.ReadAllText("D:\\Work\\licence");
        return encryptedXml;
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
        File.WriteAllText("D:\\Work\\licence", encryptedNewXml);
    }

    public static void DisableLicence()
    {
        // send request to server to change the active flag on server
        // we didn't use it yet
        try
        {
            string macAddress = GetMacAddress();
            var client = new RestClient("http://localhost:3000");
            var request = new RestRequest("licences/disableLicence", Method.POST);
            request.AddHeader("Accept", "application/json");
            request.AddJsonBody(new { mac = macAddress });
            IRestResponse response = client.Execute(request);
            if (response.ResponseStatus == ResponseStatus.Error)
            {
                throw response.ErrorException;
            }
            var finalResponse = JObject.Parse(response.Content);
            // return finalResponse["message"].ToString();
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public static void NotifyServer(string msg)
    {
        // Notify the server with specific message
        try
        {
            string macAddress = GetMacAddress();
            var client = new RestClient("http://localhost:3000");
            var request = new RestRequest("licences/notifyExpirationDate", Method.POST);
            request.AddHeader("Accept", "application/json");
            request.AddJsonBody(new { mac = macAddress , message = msg, mallName = MALL_NAME});
            IRestResponse response = client.Execute(request);
            if (response.ResponseStatus == ResponseStatus.Error)
            {
                throw response.ErrorException;
            }
            var finalResponse = JObject.Parse(response.Content);
            // return finalResponse["message"].ToString();

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
    
    static void Main(string[] args)
    {
        if (!File.Exists("D:\\Work\\licence")) {
            // This call server to get licence and if found we encrypt the licence and write it to xml file on computer
            GetDataFromServer(); 
        }
        
        try
        {
            // if (File.Exists("D:\\Work\\licence.xml")) ;
            string encryptedXml = readLicence(); // Get The Encrypted XML File
            string decryptedXml = Decryptt(encryptedXml); // Decrypt the XML File
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(decryptedXml); // Convert the Xml string to XMl Documanet
            NotifyServer("Test message"); // Just Test 
            ValidateDate(ref xdoc); // ValidateDate --> this should be called in End Of Day Procedure
            string licenceStatus = xdoc.GetElementsByTagName("Active")[0].InnerText; // Get The Active Flag
            string mallName = xdoc.GetElementsByTagName("MallName")[0].InnerText; // Get The Mall Name
            if (licenceStatus == "Y") // if licence is valid
            {
                Console.WriteLine("Authorized ...");
                Console.WriteLine("Processing Stores ....");
            }
            else // if licence is not valid due to some conditions (date/change encrypted file/mac)
            {
                Console.WriteLine("Your Licence Runs in Issue ...");
                Console.WriteLine("Please Contact StarCom ....");
                
                // GetDataFromServer();
                /* Send Request to turn off it's validation */
            }
        } 
        catch (Exception e) // catch an error if something wrong happend during decryption 
        {
            Console.WriteLine("Licence Was Manipulated ...");
            NotifyServer($"Some Mall Tried to manipulate the licence");
            // File.Delete("D:\\Work\\licence");
            /* delete file */
            /* Terminate The Service */
        }

        

    }
}

