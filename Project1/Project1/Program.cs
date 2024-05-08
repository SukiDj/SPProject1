using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Project1;
class Program
{
    static void Main(string[] args)
    {
        string[] prefixes = { "http://localhost:8080/" };
        HttpServer httpServer = new HttpServer(prefixes);
        httpServer.Start();
    }
}
