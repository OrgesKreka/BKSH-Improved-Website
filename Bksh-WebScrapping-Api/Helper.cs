using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Bksh_WebScrapping_Api.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Bksh_WebScrapping_Api
{
    public class Helper
    {
        private readonly string baseUrl = "http://www.bksh.al/";
        private readonly string searchUrl = "adlib/scripts/wwwopac.exe";
        private readonly RestRequest _request;
        private readonly string _pagesNumberpattern = @"\d+ f."; // regex per te kontrolluar nese eshte numri i faqeve apo jo tek te dhenat fizike

        /// <summary>
        ///     Te dhenat e marra nga https://capitalizemytitle.com/reading-time/100-pages/
        ///     Ku ka kohe mesatare ne minuta per leximin e 1 faqeje ngadale dhe shpejte
        /// </summary>
        private const float SlowestTimeToReadAPage = 4;     // min, 125 fjale per minute
        private const float FastestTimeToReadAPage = 1.1f;  // min, 450 fjale per minute

        /// <summary>
        ///		Inicializon requestin me header-at qe pranon website
        /// </summary>
        public Helper()
        {
            _request = new RestRequest(searchUrl);
            _request.AddHeader("Host", "www.bksh.al");
            _request.AddHeader("Connection", "keep-alive");
            _request.AddHeader("Cache-Control", "max-age=0");
            _request.AddHeader("Origin", "http://www.bksh.al");
            _request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            _request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.113 Safari/537.36");
            _request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            _request.AddHeader("Referer", "http://www.bksh.al/Katalogu/library/wwwopac/wwwroot/beginner/form_al.html");
            _request.AddHeader("Accept-Encoding", "gzip, deflate");
            _request.AddHeader("Accept-Language", "en-US,en;q=0.9");
        }

        /// <summary>
        ///		Ben nje kerkese me te dhenat e formes dhe limit 25
        ///		Nga pergjigja kontrollon nese ka rezultat apo jo
        ///		nese ka merr vetem numrin total  te rezultateve te kerkimit dhe e kthen
        /// </summary>
        /// <param name="bodyQueryString">Objekti me te dhenat e kerkimit</param>
        /// <returns>0 nese nuk ka rezultate, totalin e rezultateve ne te kundert</returns>
        public async Task<int> GetNumberOfRecordsAsync(string bodyQueryString)
        {
            var client = new RestClient(baseUrl);

            _request.Method = Method.POST;
            _request.Parameters.Clear();
            _request.AddParameter("application/x-www-form-urlencoded", bodyQueryString, ParameterType.RequestBody);

            var response = await client.ExecuteAsync(_request);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new HttpException($"BKSH returned {response.StatusCode} response code.");

            var content = response.Content;

            var parser = new HtmlParser();
            var document = await parser.ParseDocumentAsync(content);

            var numberOfRecordsTextTag = document.All.FirstOrDefault(m => m.LocalName == "font" && m.InnerHtml.Contains("hr"));

            if (numberOfRecordsTextTag == null)
                throw new KeyNotFoundException("No records were found matching the terms you entered.");

            var numberOfRecords = Regex.Match(numberOfRecordsTextTag.InnerHtml, @"\d+").Value;

            return int.Parse(numberOfRecords);
        }

        /// <summary>
        ///		Kthen nje objekt me te rezultatet e kerkimit
        /// </summary>
        /// <param name="bodyQueryString">Objekti me te dhenat e kerkimit</param>
        /// <returns>Rezultatet e kerkimit nese ka, null perndryshe</returns>
        public async Task<IEnumerable<BookResponse>> GetListOfBooksAsync(string bodyQueryString)
        {
            var client = new RestClient(baseUrl);

            _request.Method = Method.POST;
            _request.Parameters.Clear();
            _request.AddParameter("application/x-www-form-urlencoded", bodyQueryString, ParameterType.RequestBody);

            var response = await client.ExecuteAsync(_request);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new HttpException($"BKSH returned {response.StatusCode} resposnse code.");

            var content = response.Content;

            var parser = new HtmlParser();
            var document = await parser.ParseDocumentAsync(content);

            var tableOfResults = document.All.FirstOrDefault(m => m.LocalName.Equals("table", StringComparison.OrdinalIgnoreCase));
            if (tableOfResults == null)
                return new List<BookResponse>();

            var dataALinks = document.QuerySelectorAll("table tr td:last-child a").OfType<IHtmlAnchorElement>(); ;

            var data = new List<BookResponse>();

            foreach (var item in dataALinks)
            {
                var linkContent = item.InnerHtml.Replace("\n", "").Split(new string[] { " / " }, StringSplitOptions.None);

                var bookTitle = linkContent[0];
                var listOfAuthors = new List<string>();

                if (linkContent.Length > 1)
                    listOfAuthors = linkContent[1].Split(';').ToList();

                data.Add(new BookResponse
                {
                    Title = bookTitle,
                    Authors = listOfAuthors,
                    BookInfoUrl = item.Search
                });

            }

            return data;
        }

        /// <summary>
        ///		Merr rezultatet specifike te nje kerkimi
        /// </summary>
        /// <param name="urlToSearch">Url nga ku do te nxirren te dhenat</param>
        /// <returns>Objektin me te dhena, aq sa ka</returns>
        public async Task<BookData> GetBookDataAsync(string urlToSearch)
        {
            var client = new RestClient(baseUrl);

            _request.Method = Method.POST;
            _request.AddParameter("application/x-www-form-urlencoded", baseUrl + searchUrl + urlToSearch, ParameterType.RequestBody);

            var response = await client.ExecuteAsync(_request);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new HttpException($"BKSH returned {response.StatusCode} resposnse code.");

            var content = response.Content;

            var parser = new HtmlParser();
            var document = await parser.ParseDocumentAsync(content);

            var tableResult = document.QuerySelectorAll("table table ");

            var bookData = new BookData();
            var listOfCopies = new List<ExemplarData>();

            if (tableResult.Length >= 1)
            {
                var infoTable = tableResult[0];

                foreach (var row in infoTable.QuerySelectorAll("tr").OfType<IHtmlTableRowElement>())
                {

                    var rowContent = row.InnerHtml.ToLower();

                    if (rowContent.Contains("<td valign=\"top\"><b>titulli</b><br></td>"))
                    {
                        bookData.Title = row.LastChild.TextContent.Replace("\n","");
                        continue;
                    }

                    if (rowContent.Contains("<td valign=\"top\"><b>autorët</b><br></td>"))
                    {
                        var authorsTd = row.QuerySelectorAll("a").OfType<IHtmlAnchorElement>().Select(x => x.TextContent.Replace("\n", ""));
                        bookData.Authors = authorsTd;
                        continue;
                    }

                    if (rowContent.Contains("<td valign=\"top\"><b>botuar</b><br></td>"))
                    {
                        bookData.PublishingData = row.LastChild.TextContent.Replace("\n", "");
                        continue;
                    }

                    if (rowContent.Contains("<td valign=\"top\"><b>të dhëna fizike</b><br></td>"))
                    {
                        bookData.PhysicalData = row.LastChild.TextContent.Replace("\n", "");

                        Regex regex = new Regex(_pagesNumberpattern);
                        var match =  regex.Match(bookData.PhysicalData);

                        if(match.Success)
                        {
                            var numberOfPages = 0;

                            if (int.TryParse(match.Value.Replace(" f.", ""), out numberOfPages))
                            {
                                bookData.NumberOfPages = numberOfPages;
                                bookData.MaxEstimatedReadingTime = numberOfPages * SlowestTimeToReadAPage;
                                bookData.MinEstimatedReadingTime = numberOfPages * FastestTimeToReadAPage;
                            }
                        }


                        continue;
                    }

                    if (rowContent.Contains("<td valign=\"top\"><b>titulli i serisë</b><br></td>"))
                    {
                        bookData.SerieTitle = row.LastChild.TextContent.Replace("\n", "");
                        continue;
                    }

                    // Ka raste qe ç kodohet gabim dhe vjen si �
                    // keshtu qe kontrolloj vetem me nje pjese te fjales
                    if (rowContent.Contains("<td valign=\"top\"><b>fjalët ky"))
                    {
                        var keyWords = row.QuerySelectorAll("a").OfType<IHtmlAnchorElement>().Select(x => x.TextContent.Replace("\n", ""));
                        bookData.Keywords = keyWords;
                        continue;
                    }

                    if (rowContent.Contains("<td valign=\"top\"><b>klasifikimi dhjetor</b><br></td>"))
                    {
                        bookData.Clasification = row.LastChild.TextContent.Replace("\n", "");
                        continue;
                    }

                    if (rowContent.Contains("<td valign=\"top\"><b>shënime</b><br></td>"))
                    {
                        bookData.Notes = row.LastChild.TextContent.Replace("\n", "");
                        continue;
                    }

                    if (rowContent.Contains("<td valign=\"top\"><b>isbn</b><br></td>"))
                    {
                        bookData.Isbn = row.LastChild.TextContent.Replace("\n", "");
                    }
                }
            }

            if (tableResult.Length >= 2)
            {
                var exemplarsTable = tableResult[1];

                foreach (var row in exemplarsTable.QuerySelectorAll("tr").OfType<IHtmlTableRowElement>().Skip(1))
                {
                    var tds = row.QuerySelectorAll("td").ToArray();

                    var exemplarData = new ExemplarData
                    {
                        InventoryNumber = tds[0].TextContent.Replace("\n", ""),
                        PlaceNumber = tds[3].TextContent.Replace("\n", ""),
                        State = tds[1].TextContent.Replace("\n", ""),
                        TypeOfBorrowing = tds[2].TextContent.Replace("�", "ë").Replace("\n", "")
                    };
                    listOfCopies.Add(exemplarData);
                }

                bookData.Exemplars = listOfCopies;
            }

            return bookData;
        }
    }

    /// <summary>
    ///     Klase ndihmese, qe kodon nje objekt me te dhena ne string url
    /// </summary>
    public static class UrlHelpers
    {
        public static string ToQueryString(this object request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // Merr gjithe property-t e objektit
            // dhe i konverton ne nje liste Key - Value
            // ku key eshte Descriptioni i properties dhe value eshte vlera
            var properties = request.GetType().GetProperties()
                .Where(x => x.CanRead)
                .Where(x => x.GetValue(request, null) != null) // Injoron Fushat qe kane vlera null, nga ndertimi i klases jane vetem fushat e checBoxeve
                .Select(x => new { Key = (x.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute).Description, Value = x.GetValue(request, null) });


            // Concat all key/value pairs into a string separated by ampersand
            return string.Join("&", properties
                .Select(x => string.Concat(
                    Uri.EscapeDataString(x.Key), "=",
                    Uri.EscapeDataString(x.Value.ToString()))));
        }
    }
}
