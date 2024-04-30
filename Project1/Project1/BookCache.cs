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
        private static LinkedList<string> accessOrder = new LinkedList<string>(); // Redosled pristupa
        private static readonly ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();

        private static int maxCacheSize = 2; // Maksimalna veličina cache-a

        public static bool ContainsKey(string key)
        {
            return cache.ContainsKey(key);
        }

        public static List<Book> GetValue(string key)
        {
            Lock.EnterReadLock();
            try
            {
                // Ažuriranje redosleda pristupa
                accessOrder.Remove(key);
                accessOrder.AddLast(key);

                return cache[key];
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error in function GetValue: {e.Message}");
                throw;
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }

        public static void Add(string key, List<Book> value)
        {
            Lock.EnterWriteLock();
            try
            {
                // Ažuriranje redosleda pristupa
                if (cache.Count >= maxCacheSize)
                {
                    RemoveLRU();
                }

                cache[key] = value;
                accessOrder.AddLast(key);
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error in function Add: {e.Message}");
                throw;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
            
        }

        // Uklanja najmanje korišćen podatak iz cache-a
        private static void RemoveLRU()
        {
            try
            {
                string lruKey = accessOrder.First.Value;
                accessOrder.RemoveFirst();
                cache.Remove(lruKey);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}
