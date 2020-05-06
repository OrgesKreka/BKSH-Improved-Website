using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bksh_WebScrapping_Api.Models
{
    public class BookResponse
    {
        public string Title { get; set; }

        public List<string> Authors { get; set; }

        public string BookInfoUrl { get; set; }
    }
}