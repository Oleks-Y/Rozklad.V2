using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Rozklad.V2.Models;

namespace Rozklad.V2.Services
{
    public  class TelegramValidationService
    {
        private readonly string _botToken;

        public TelegramValidationService(string botToken)
        {
            _botToken = botToken;
        }
        
        public  bool Validate(TelegramUser userResponse)
        {
            var hash = userResponse.hash;
            var json = JsonConvert.SerializeObject(userResponse);
            var requestData = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            requestData.Remove("hash");
            var sortedDict = from entry in requestData orderby entry.Key ascending select entry;
            var dataCheckStrings = new List<string>();
            foreach (var (key, value) in sortedDict)
            {
                dataCheckStrings.Add(key+"="+value);
            }

            var dataCheckString = string.Join("\n", dataCheckStrings);
            var sha256 = SHA256.Create();
            var secretKey = sha256.ComputeHash(Encoding.ASCII.GetBytes(_botToken));

            var computedHash = new HMACSHA256(secretKey).ComputeHash(Encoding.ASCII.GetBytes(dataCheckString));
            var hexValue = ByteArrayToString(computedHash);

            return hexValue == hash;
        }
        private static string ByteArrayToString(byte[] ba)
        {
            var hex = new StringBuilder(ba.Length * 2);
            foreach (var b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}