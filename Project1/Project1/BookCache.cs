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
        private static Dictionary<string, List<Book>> cache = new Dictionary<string, List<Book>>();
        public static bool ContainsKey(string key)
        {
            return cache.ContainsKey(key);
        }

        public static List<Book> GetValue(string key)
        {
            return cache[key];
        }

        public static void Add(string key, List<Book> value)
        {
            cache[key] = value;
        }
    }
}
