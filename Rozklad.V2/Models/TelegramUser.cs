
using System.Text.Json.Serialization;

namespace Rozklad.V2.Models
{
    public class TelegramUser
    {
      
        public long auth_date { get; set; }
      
        public string first_name { get; set; }
     
        public string hash { get; set; }

        public long id { get; set; }
     
        public string last_name { get; set; }
     
        public string photo_url { get; set; }
    
        public string username { get; set; }
    }
}