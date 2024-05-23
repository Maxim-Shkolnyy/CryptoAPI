using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CryptoAPI.Models
{
    public class CryptoCurrency
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public DateTime LastUpdated { get; set; }
    }

}