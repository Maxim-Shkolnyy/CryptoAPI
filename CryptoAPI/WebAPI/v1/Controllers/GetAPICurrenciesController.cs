using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using RestSharp;


namespace CryptoAPI.WebAPI.v1.Controllers
{
    public class GetAPICurrenciesController : ApiController
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<ActionResult> GetApiData()
        {
            // Fetch API key from configuration
            string apiKey = ConfigurationManager.AppSettings["21d3abe8"];
            string apiUrl = $"https://rest.coinapi.io/v1/exchangerate/:asset_id_base";

            // Create RestClient and RestRequest
            var client = new RestClient(apiUrl);
            var request = new RestRequest(Method.Get);

            // Add headers
            request.AddHeader("Accept", "application/json");
            request.AddHeader("X-CoinAPI-Key", apiKey);

            // Execute request asynchronously
            RestResponse response = await client.ExecuteAsync(request);

            // Check if request was successful
            if (response.IsSuccessful)
            {
                // Process the response here
                // For example, you can return the JSON content to the view
                return Content(response.Content, "application/json");
            }
            else
            {
                // Handle error if request was not successful
                // For example, you can return an error view
                return View("Error");
            }
        }

    }
}
