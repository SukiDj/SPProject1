using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    public class BookCache
    {
        // Koristimo Dictionary kao hash mapu za keširanje odgovora
        private static Dictionary<string, string> cache = new Dictionary<string, string>();
        public static bool ContainsKey(string key)
        {
            return cache.ContainsKey(key);
        }

        public static string GetValue(string key)
        {
            return cache[key];
        }

        public static void Add(string key, string value)
        {
            cache[key] = value;
        }
    }
}
