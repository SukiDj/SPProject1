using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1;
public class BookCache
{
    private static Dictionary<string, List<Book>> cache = new Dictionary<string, List<Book>>();//hash mapa koja povezuje odredjenog autora sa njegovim knjigama
    private static LinkedList<string> accessOrder = new LinkedList<string>(); // Redosled pristupa
    private static readonly ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();

    private static int maxCacheSize = 2;

    public static bool ContainsKey(string key)
    {
        return cache.ContainsKey(key);
    }

    public static List<Book> GetValue(string key)
    {
        Lock.EnterWriteLock();
        try
        {
            accessOrder.Remove(key);// izbacujemo ga sa liste najskorije pristupanih gde god da se nalazio na njoj
            accessOrder.AddLast(key);// ubacujemo ga na kraj liste tako da se zna da je on najskorije iskoriscen podatak,
                                     // ovako ce prvi podatak u accessListi uvek biti najodavnije koriscen i spreman za promenu
            return cache[key];
        }
        catch (Exception e)
        {
            Console.WriteLine($"Greska u funkciji GetValue: {e.Message}");
            throw;
        }
        finally
        {
            Lock.ExitWriteLock();
        }
    }

    public static void Add(string key, List<Book> value)
    {
        Lock.EnterWriteLock();
        try
        {
            if (cache.Count >= maxCacheSize)
            {
                RemoveLRU();
                // ako je cache pun kada treba da se kesira novi podatak izbacujemo najodavnije korisceni element iz liste pristupa i iz kesa,
                // i dodajemo novi element u kes i na kraj liste pristupa
            }

            cache[key] = value;
            accessOrder.AddLast(key);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Greska u funkciji Add: {e.Message}");
            throw;
        }
        finally
        {
            Lock.ExitWriteLock();
        }

    }

    private static void RemoveLRU()
    {
        try
        {
            string lruKey = accessOrder.First.Value;// iz liste pristupa pronalazimo kljuc najodavnije koriscenog podatka iz kesa
            accessOrder.RemoveFirst();// uklanjamo ga iz liste pristupa
            cache.Remove(lruKey);// uklanjamo ga i iz kesa
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
}