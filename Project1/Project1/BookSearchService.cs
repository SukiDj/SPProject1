using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Project1
{
    public class BookSearchService
    {
        private string apiUrl = "https://api.nytimes.com/svc/books/v3/reviews.json";
        private string apiKey = "7DQNwuHGWsu87ZfjCmO9Pg4sSzdmDPAs";

        public List<Book> SearchBooksByAuthor(string author)
        {
            List<Book> books = new List<Book>();

            string query = $"?author={author}&api-key={apiKey}";
            string fullUrl = apiUrl + query;

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage apiResponse = httpClient.GetAsync(fullUrl).Result;
                    if (apiResponse.IsSuccessStatusCode)
                    {
                        string responseBody = apiResponse.Content.ReadAsStringAsync().Result;
                        dynamic jsonData = JsonConvert.DeserializeObject(responseBody);
                        foreach (var item in jsonData.results)
                        {
                            Book book = new Book
                            {
                                Url = item.url,
                                PublicationDate = item.publication_dt,
                                Title = item.book_title,
                                Author = item.book_author,
                                Summary = item.summary
                            };
                            books.Add(book);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error: {apiResponse.StatusCode}");
                    }
                }

                return books;
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error in function SearchBooksByAuthor: {e.Message}");
                throw;
            }

        }
    }
}
