using System.Collections.Generic;
using CoincheckDomain.Entity;
using Newtonsoft.Json;
namespace CoincheckLibrary.Config
{
    public class ResponseWrapper
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public List<PublicTradeEntity> Data { get; set; }
    }
    public class OrderBookResponse
    {
        [JsonProperty("asks")]
        public List<List<object>> Asks { get; set; }

        [JsonProperty("bids")]
        public List<List<object>> Bids { get; set; }
    }
    public class OrderResponse
    {
        public bool Success { get; set; }
        public int Id { get; set; }
        public string Rate { get; set; }
        public string Amount { get; set; }
        public string OrderType { get; set; }
        public string TimeInForce { get; set; }
        public string StopLossRate { get; set; }
        public string Pair { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
