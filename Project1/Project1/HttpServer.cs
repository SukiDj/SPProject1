using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Project1;

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
        Console.WriteLine("Server pokrenut...");

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

        Console.WriteLine($"Primljen zahtev: {request.Url}");
        Stopwatch sw = new Stopwatch();
        sw.Start();

        try
        {
            // Procesiranje zahteva
            string author = request.QueryString["author"];
            if (string.IsNullOrEmpty(author))
            {
                SendBadRequestResponse(response);
                return;
            }

            if (BookCache.ContainsKey(author))// ako se autor (kljuc u kesu) nalazi u kesu, pribavljamo odgovor iz kesa
            {
                SendCachedResponse(author, response);
                sw.Stop();
                Console.WriteLine($"Vreme potrebno za pribavljanje podataka iz kesa za autora {author}: {sw.Elapsed}");
            }
            else// ako ne, pribavljamo odgovor sa apija i kesiramo taj odgovor 
            {
                SearchAndSendResponse(author, response);
                sw.Stop();
                Console.WriteLine($"Vreme potrebno za pribavljanje podataka koji nije u kesu za autora {author}: {sw.Elapsed}");
            }

        }
        catch (Exception e)
        {
            Console.WriteLine($"Greska u HttpServer: {e.Message}");
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
        List<Book> cachedResponse = BookCache.GetValue(author);// u funkciji getValue ce se azurirati i redosled pristupa podacima iz kesa
        string responseBody = Newtonsoft.Json.JsonConvert.SerializeObject(cachedResponse);
        SendResponse(response, responseBody);
    }

    private void SearchAndSendResponse(string author, HttpListenerResponse response)
    {
        BookSearchService bookSearchService = new BookSearchService();
        List<Book> books = bookSearchService.SearchBooksByAuthor(author);

        BookCache.Add(author, books);// u funkciji add ce se takodje azurirati redosled pristupa kesu
        //Console.WriteLine($"\nUbacen u cache: {author}");

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
