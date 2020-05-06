using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bksh_WebScrapping_Api.Models
{
    /// <summary>
    ///     Modeli me te dhenat qe do vije kerkesa
    /// </summary>
    public class RequestModel
    {
        public string Title { get; set; }

        public string Author { get; set; }

        public string Text { get; set; }

        public string Keyword { get; set; }

        public int Year { get; set; }

        public string Database { get; set; }
    }
}