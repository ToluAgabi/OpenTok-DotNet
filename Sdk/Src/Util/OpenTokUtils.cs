using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Net;

namespace OpenTokSDK.Util
{
    public class OpenTokUtils
    {
        public static string[] SplitString(string toBeSplitted, char separator, int numberOfFields)
        {
            char[] sep = { separator };
            string[] fields = toBeSplitted.Split(sep);

            if (numberOfFields > 0 && fields.Length != numberOfFields)
            {
                return null;
            }
            return fields;
        }

        public static string Decode64(string encodedString)
        {
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    byte[] data = Convert.FromBase64String(encodedString);
                    return Encoding.UTF8.GetString(data);
                }
                catch (FormatException)
                {
                    // We don't do anything here because we need to try to 
                    // decode the string again
                }
                encodedString = encodedString + "=";
            }
            throw new FormatException("String cannot be decoded");
        }

        public static string EncodeHMAC(string input, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            HMACSHA1 hmac = new HMACSHA1(keyBytes);
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashedValue = hmac.ComputeHash(inputBytes);
            return String.Join("", hashedValue.Select(a => a.ToString("x2")));

        }

        public static string Convert64(string input)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(inputBytes);
        }
        public static double GetUnixTimeStampForDate(DateTime date)
        {
            return (date - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        } 

        public static double GetCurrentUnixTimeStamp() 
        {
            return GetUnixTimeStampForDate(DateTime.UtcNow);
        }   

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }

        public static int GetRandomNumber()
        {
            Random random = new Random();
            return random.Next(0, 999999);
        }

        public static bool TestIpAddress(string location)
        {
            IPAddress ipAddress;
            if (location == "" || location == "localhost")
            {
                return true;
            }
            else if(IPAddress.TryParse(location, out ipAddress))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
