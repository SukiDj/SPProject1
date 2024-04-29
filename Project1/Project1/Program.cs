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
        using (HttpListener listener = new HttpListener())
        {
            listener.Prefixes.Add("http://localhost:8080/");
            listener.Start();
            Console.WriteLine("Web server running...");

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                ThreadPool.QueueUserWorkItem((state) =>
                {
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;

                    Console.WriteLine($"Request received: {request.Url}");

                    // Procesiranje zahteva
                    string author = request.QueryString["author"];
                    Console.WriteLine(author);
                    Console.WriteLine(BookCache.ContainsKey(author));
                    if (BookCache.ContainsKey(author))
                    {
                        // Slanje keširanog odgovora
                        List<Book> cachedResponse = BookCache.GetValue(author);
                        string responseBody = Newtonsoft.Json.JsonConvert.SerializeObject(cachedResponse);
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseBody);
                        response.ContentLength64 = buffer.Length;
                        Stream output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                        output.Close();
                    }
                    else if (!string.IsNullOrEmpty(author))
                    {
                        BookSearchService bookSearchService = new BookSearchService();
                        List<Book> books = bookSearchService.SearchBooksByAuthor(author);

                        BookCache.Add(author, books);

                        //if(BookCache.GetValue(author).Count > 0)
                        //    Console.WriteLine(BookCache.GetValue(author).First().Title);

                        // Slanje odgovora klijentu
                        string responseBody = Newtonsoft.Json.JsonConvert.SerializeObject(books);
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseBody);
                        response.ContentLength64 = buffer.Length;
                        Stream output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                        output.Close();
                    }
                    else
                    {
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        response.StatusDescription = "Bad Request";
                        response.Close();
                    }
                }, null);
            }
        }
    }
}
