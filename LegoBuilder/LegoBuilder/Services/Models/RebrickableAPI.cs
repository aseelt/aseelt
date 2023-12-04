using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace LegoBuilder.Services.Models
{
    public class RebrickableAPI : IRebrickableAPI
    {
        public string RebrickableApiUrl { get; set; }
        public string ApiKey { get; set; }

        public RebrickableAPI(string url, string key)
        {
            RebrickableApiUrl = url;
            ApiKey = key;
        }
        
    }
}
