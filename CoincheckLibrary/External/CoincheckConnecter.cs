

using CoincheckDomain.Entity;
using CoincheckLibrary.Config;
using CoincheckDomain.Config;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using System;


namespace CoincheckLibrary.External
{
    public class CoincheckConnecter
    {
        /// <summary>
        /// 指値注文を行うメソッド
        /// </summary>
        /// <param name="pair">取引ペア（例: "btc_jpy"）</param>
        /// <param name="orderType">注文タイプ（"buy" または "sell"）</param>
        /// <param name="rate">注文レート</param>
        /// <param name="amount">注文量</param>
        /// <returns>注文結果</returns>
        public async Task<OrderResponse> PlaceOrder(string pair, string orderType, decimal rate, decimal amount)
        {
            var url = ApiConstants.ExchangeOrderUrl;
            string nonce = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            // リクエストボディを作成
            var requestBody = new
            {
                pair = pair,
                order_type = orderType,
                rate = rate,
                amount = amount
            };
            string jsonBody = JsonConvert.SerializeObject(requestBody);

            // メッセージを作成して署名
            string message = nonce + url + jsonBody;
            string signature = CreateSignature(message, ApiConstants.ApiSecret);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("ACCESS-KEY", ApiConstants.ApiKey);
                client.DefaultRequestHeaders.Add("ACCESS-NONCE", nonce);
                client.DefaultRequestHeaders.Add("ACCESS-SIGNATURE", signature);

                // POSTリクエストを送信
                HttpContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);
                string responseBody = await response.Content.ReadAsStringAsync();

                // レスポンスを OrderResponse クラスにデシリアライズ
                var orderResponse = JsonConvert.DeserializeObject<OrderResponse>(responseBody);
                return orderResponse;
            }
        }

        public async Task<BalanceEntity> GetBalance()
        {
            var url = ApiConstants.BalanceUrl;
            string nonce = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            string message = nonce + ApiConstants.BalanceUrl;
            string signature = CreateSignature(message, ApiConstants.ApiSecret);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("ACCESS-KEY", ApiConstants.ApiKey);
                client.DefaultRequestHeaders.Add("ACCESS-NONCE", nonce);
                client.DefaultRequestHeaders.Add("ACCESS-SIGNATURE", signature);

                HttpResponseMessage response = await client.GetAsync(url);
                string responseBody = await response.Content.ReadAsStringAsync();

                // JSONレスポンスをBalanceクラスにマッピング
                BalanceEntity balance = JsonConvert.DeserializeObject<BalanceEntity>(responseBody);
                balance.Date = DateTime.Now;
                return balance;
            }
        }
        public async Task<List<PublicTradeEntity>> GetTradeHistory()
        {
            var allTrades = new List<PublicTradeEntity>();
            foreach (var pair in CoincheckConstants.TradingPairs)
            { 
                // リクエストURLに取引ペアとlimit、ページネーション用のstarting_afterを指定
                var url = $"{ApiConstants.PublicTradeUrl}?pair={pair}&limit=100";

                string nonce = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                string message = nonce + url;
                string signature = CreateSignature(message, ApiConstants.ApiSecret);

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("ACCESS-KEY", ApiConstants.ApiKey);
                    client.DefaultRequestHeaders.Add("ACCESS-NONCE", nonce);
                    client.DefaultRequestHeaders.Add("ACCESS-SIGNATURE", signature);

                    HttpResponseMessage response = await client.GetAsync(url);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // ResponseWrapperを使用してデシリアライズ
                    var wrapper = JsonConvert.DeserializeObject<ResponseWrapper>(responseBody);

                    if (wrapper != null && wrapper.Success && wrapper.Data != null)
                    {
                        foreach (var trade in wrapper.Data)
                        {
                            trade.Pair = pair;
                            trade.UpdatedAt = DateTime.Now;
                            allTrades.Add(trade);
                        }
                    }
                }
            }
            return allTrades;
        }
        public async Task<List<OrderBookEntity>> GetAllOrderBooks()
        {
            var allOrderBookEntries = new List<OrderBookEntity>();

            foreach (var pair in CoincheckConstants.TradingPairs)
            {
                var url = $"{ApiConstants.OrderBookUrl}?pair={pair}";

                string nonce = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                string message = nonce + url;
                string signature = CreateSignature(message, ApiConstants.ApiSecret);
                var now = DateTime.Now;
                var timestamp = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("ACCESS-KEY", ApiConstants.ApiKey);
                    client.DefaultRequestHeaders.Add("ACCESS-NONCE", nonce);
                    client.DefaultRequestHeaders.Add("ACCESS-SIGNATURE", signature);

                    HttpResponseMessage response = await client.GetAsync(url);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // JSONデシリアライズ
                    var orderBookResponse = JsonConvert.DeserializeObject<OrderBookResponse>(responseBody);

                    if (orderBookResponse != null)
                    {
                        // asksをエンティティにマッピングし、Typeを "ask" に設定
                        allOrderBookEntries.AddRange(orderBookResponse.Asks.Select(a => new OrderBookEntity
                        {
                            Pair = pair,                    // 通貨ペアを設定
                            Timestamp = timestamp,          // 秒単位を丸めた取得日時
                            Type = "ask",
                            Price = decimal.Parse(a[0].ToString()),
                            Quantity = decimal.Parse(a[1].ToString())
                        }));

                        // bidsをエンティティにマッピングし、Typeを "bid" に設定
                        allOrderBookEntries.AddRange(orderBookResponse.Bids.Select(b => new OrderBookEntity
                        {
                            Pair = pair,                    // 通貨ペアを設定
                            Timestamp = timestamp,          // 秒単位を丸めた取得日時
                            Type = "bid",
                            Price = decimal.Parse(b[0].ToString()),
                            Quantity = decimal.Parse(b[1].ToString())
                        }));
                    }
                }
            }

            return allOrderBookEntries;
        }


        private static string CreateSignature(string message, string secret)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
