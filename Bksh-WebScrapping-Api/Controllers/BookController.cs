using Bksh_WebScrapping_Api.Models;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Bksh_WebScrapping_Api.Controllers
{
    [RoutePrefix("api/[controller]")]
    public class BookController : ApiController
    {
        private readonly Helper _helper;

        public BookController()
        {
            _helper = new Helper();
        }
        

        [HttpPost]
        public async Task<IHttpActionResult> GetSearchResults([FromBody] RequestModel request)
        {
            if (request == null)
                return Content(HttpStatusCode.BadRequest, "Request shouldn't be null" );

            var requestForm = new RequestFormModel();

            if (!string.IsNullOrEmpty(request.Title))
            {
                requestForm.TitleSearchValue = request.Title;
                requestForm.IsSearchingByTitle = "on";
            }

            if(!string.IsNullOrEmpty(request.Author))
            {
                requestForm.AuthorSearchValue = request.Author;
                requestForm.IsSearchingByAuthor = "on";
            }

            if( ! string.IsNullOrEmpty( request.Text ) )
            {
                requestForm.TextSearchValue = request.Text;
                requestForm.IsSearchingByText = "on";
            }

            if( ! string.IsNullOrEmpty( request.Keyword ) )
            {
                requestForm.KeyWordSearchValue = request.Keyword;
                requestForm.IsSearchingByKeyWord = "on";
            }

            if(request.Year > 900 )
            {
                requestForm.YearSearchValue = request.Year.ToString();
                requestForm.IsSearchingByYear = "on";
            }

            requestForm.Database = request.Database;

            try
            {
                var modelToQueryString = requestForm.ToQueryString();

                int numberOfRecordsFound = await _helper.GetNumberOfRecordsAsync(modelToQueryString);

                if (numberOfRecordsFound == 0)
                    return Content(HttpStatusCode.NotFound, "No records were found matching the terms you entered.");

                requestForm.Limit = numberOfRecordsFound;
                var newModelToQueryString = requestForm.ToQueryString();

                var result = await _helper.GetListOfBooksAsync(newModelToQueryString);

                return Ok(result);
            }
            catch(Exception exc)
            {
                return Content(HttpStatusCode.InternalServerError, exc.Message);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetSearchDetails([FromBody] string url)
        {
            if(string.IsNullOrEmpty(url))
                return Content(HttpStatusCode.BadRequest, "Url shouldn't be null");

            var result = await _helper.GetBookDataAsync(url);

            return Ok(result);
        }
    }
}