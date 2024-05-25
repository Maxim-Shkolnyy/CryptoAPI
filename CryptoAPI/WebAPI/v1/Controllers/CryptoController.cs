using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
//using System.Web.Mvc;
using System.Web.UI.WebControls;
using CryptoAPI.Models;
using CryptoAPI.Presistent;
using RestSharp;
using Newtonsoft.Json.Linq;



namespace CryptoAPI.WebAPI.v1.Controllers
{
    public class CryptoController : ApiController
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        private readonly CryptoDbContext _context;
        private readonly string _coinApiKey;

        public CryptoController()
        {
            _context = new CryptoDbContext();
            _coinApiKey = System.Configuration.ConfigurationManager.AppSettings["CoinApiKey"];
        }

        [HttpGet]
        [Route("api/crypto/supported")]
        public IHttpActionResult GetSupportedCryptos()
        {
            var cryptos = _context.CryptoCurrencies.ToList();
            return Ok(cryptos);
        }

        [HttpGet]
        [Route("api/crypto/price/{symbol}")]
        public async Task<IHttpActionResult> GetCryptoPrice(string symbol)
        {
            var client = new RestClient($"https://rest.coinapi.io/v1/exchangerate/{symbol}/USD");
            //var request = new RestRequest(Method.Get);
            var request = new RestRequest($"https://rest.coinapi.io/v1/exchangerate/{symbol}/USD", Method.Get);

            request.AddHeader("X-CoinAPI-Key", _coinApiKey);

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var content = JObject.Parse(response.Content);
                var price = content["rate"].Value<decimal>();
                var lastUpdated = DateTime.Now;

                var crypto = _context.CryptoCurrencies.FirstOrDefault(c => c.Symbol == symbol);
                if (crypto == null)
                {
                    crypto = new CryptoCurrency
                    {
                        Symbol = symbol,
                        Name = symbol, 
                        Price = price,
                        LastUpdated = lastUpdated
                    };
                    _context.CryptoCurrencies.Add(crypto);
                }
                else
                {
                    crypto.Price = price;
                    crypto.LastUpdated = lastUpdated;
                    _context.Entry(crypto).State = System.Data.Entity.EntityState.Modified;
                }

                await _context.SaveChangesAsync();

                return Ok(new { Symbol = symbol, Price = price, LastUpdated = lastUpdated });
            }

            return StatusCode(response.StatusCode);
        }

    }
}
