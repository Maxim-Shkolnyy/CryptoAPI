using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using CryptoAPI.Models;

namespace CryptoAPI.Presistent
{
    public class CryptoDbContext : DbContext
    {
        public DbSet<CryptoCurrency> CryptoCurrencies { get; set; }

        public CryptoDbContext() : base("CryptoDbContext") { }
    }
}