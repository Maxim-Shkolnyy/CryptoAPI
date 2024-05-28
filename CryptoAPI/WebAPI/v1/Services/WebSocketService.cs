using System;
using WebSocketSharp;
using Newtonsoft.Json.Linq;
using System.Data.Entity;
using CryptoAPI.Models;
using CryptoAPI.Presistent;
using System.Linq;

public class WebSocketService
{
    private readonly WebSocket _webSocket;
    private readonly CryptoDbContext _context;
    private readonly string _coinApiKey;

    public WebSocketService(string _coinApiKey, CryptoDbContext context)
    {
        _webSocket = new WebSocket($"wss://ws.coinapi.io/v1/?apikey={_coinApiKey}");
        _coinApiKey = System.Configuration.ConfigurationManager.AppSettings["CoinApiKey"];

        _context = context;
        _webSocket.OnMessage += (sender, e) => OnMessageReceived(e.Data);
        _webSocket.OnError += (sender, e) => OnError(e.Message);
        _webSocket.OnClose += (sender, e) => OnClose();
    }

    public void Connect()
    {
        _webSocket.Connect();
    }

    public void Subscribe(string symbol)
    {
        var subscribeMessage = new
        {
            type = "hello",
            apikey = _coinApiKey,
            heartbeat = false,
            subscribe_data_type = new[] { "trade" },
            subscribe_filter_asset_id = new[] { symbol }
        };
        _webSocket.Send(JObject.FromObject(subscribeMessage).ToString());
    }

    private void OnMessageReceived(string message)
    {
        Console.WriteLine($"Message received: {message}");

        var data = JObject.Parse(message);
        var crypto = new CryptoCurrency
        {
            Symbol = data["asset_id_base"].ToString(),
            Name = data["asset_id_base"].ToString(), 
            Price = data["price"].Value<decimal>(),
            LastUpdated = DateTime.UtcNow
        };

        var existingCrypto = _context.CryptoCurrencies.SingleOrDefault(c => c.Symbol == crypto.Symbol);
        if (existingCrypto == null)
        {
            _context.CryptoCurrencies.Add(crypto);
        }
        else
        {
            existingCrypto.Price = crypto.Price;
            existingCrypto.LastUpdated = crypto.LastUpdated;
            _context.Entry(existingCrypto).State = EntityState.Modified;
        }

        _context.SaveChanges();
    }

    private void OnError(string errorMessage)
    {
        Console.WriteLine($"Error: {errorMessage}");
    }

    private void OnClose()
    {
        Console.WriteLine("Connection closed");
    }

    public void Close()
    {
        _webSocket.Close();
    }
}
