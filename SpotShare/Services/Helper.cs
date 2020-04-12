using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpotShare.Services
{
    public class Helper
    {
        public Dictionary<string,object> Map (object obj)
        {
            var json = JsonSerializer.Serialize(obj);
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return dict;
        }
    }
}
