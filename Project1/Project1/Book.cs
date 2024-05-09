using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1;

public class Book
{
    public string Url { get; set; }
    public string PublicationDate { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Summary { get; set; }

    public Book(string url, string publicationDate, string title, string author, string summary)
    {
        Url = url;
        PublicationDate = publicationDate;
        Title = title;
        Author = author;
        Summary = summary;
    }

    public override string ToString()
    {
        return $"Naziv knjige: {Title}, Autor: {Author}, Url: {Url}, Datum publikacije: {PublicationDate}, Sazetak: {Summary}";
    }
}
