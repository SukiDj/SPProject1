using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    public class HttpServer
    {
        private readonly HttpListener listener;

        public HttpServer(string[] prefixes)
        {
            listener = new HttpListener();
            foreach (string prefix in prefixes)
            {
                listener.Prefixes.Add(prefix);
            }
        }

        public void Start()
        {
            listener.Start();
            Console.WriteLine("Web server running...");

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                ThreadPool.QueueUserWorkItem(ProcessRequest, context);
            }
        }

        private void ProcessRequest(object state)
        {
            HttpListenerContext context = (HttpListenerContext)state;
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            Console.WriteLine($"Request received: {request.Url}");

            try
            {
                // Procesiranje zahteva
                string author = request.QueryString["author"];
                if (string.IsNullOrEmpty(author))
                {
                    SendBadRequestResponse(response);
                    return;
                }

                if (BookCache.ContainsKey(author))
                {
                    Console.WriteLine($"\nIz cache: {author}");
                    SendCachedResponse(author, response);
                }
                else
                {
                    Console.WriteLine($"\nNije iz cache: {author}");
                    SearchAndSendResponse(author, response);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error in HttpServer: {e.Message}");
                throw;
            }
        }

        private void SendBadRequestResponse(HttpListenerResponse response)
        {
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            response.StatusDescription = "Bad Request";
            response.Close();
        }

        private void SendCachedResponse(string author, HttpListenerResponse response)
        {
            List<Book> cachedResponse = BookCache.GetValue(author);
            string responseBody = Newtonsoft.Json.JsonConvert.SerializeObject(cachedResponse);
            SendResponse(response, responseBody);
        }

        private void SearchAndSendResponse(string author, HttpListenerResponse response)
        {
            BookSearchService bookSearchService = new BookSearchService();
            List<Book> books = bookSearchService.SearchBooksByAuthor(author);

            BookCache.Add(author, books);
            Console.WriteLine($"\nUbacen u cache: {author}");

            string responseBody = Newtonsoft.Json.JsonConvert.SerializeObject(books);
            SendResponse(response, responseBody);
        }

        private void SendResponse(HttpListenerResponse response, string responseBody)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseBody);
            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }
    }
}
