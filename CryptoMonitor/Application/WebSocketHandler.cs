using CoincheckDomain.Entity;
using CoincheckLibrary.External;
using Newtonsoft.Json;

namespace CryptoMonitor.Application
{
    public class WebSocketHandler
    {
        private WebSocketClient webSocketClient;
        private Queue<SocketTradeEntity> recentTrades;
        private Queue<long> ExportNumbers;

        private long SocketCounter  = 0;
        private int MaxQueueSize = 100; 

        public WebSocketHandler()
        {
            webSocketClient = new WebSocketClient();
            recentTrades = new Queue<SocketTradeEntity>(MaxQueueSize);
            ExportNumbers = new Queue<long>(MaxQueueSize);
        }

        public async Task StartAsync()
        {
            string uri = "wss://ws-api.coincheck.com/";
            try
            {
                await webSocketClient.ConnectAsync(uri);
                await webSocketClient.SendMessageAsync();
                _ = StartWebSocketListening();
                while (true)
                {
                    if (IsBuyPattern())
                    {
                        Console.WriteLine($"Buy pattern detected!");
                        //AddToExportNumber();
                    }
                    await Task.Delay(100); 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                await webSocketClient.DisconnectAsync();
            }
        }

        private async Task StartWebSocketListening()
        {
            await webSocketClient.ReceiveMessagesAsync(async message =>
            {
                var trades = ParseMessage(message);
                lock (recentTrades)
                {
                    foreach (var trade in trades)
                    {
                        AddToRecentTrades(trade);                        
                        SocketCounter++;
                        if (ExportNumbers.Contains(SocketCounter))
                        {
                            var TradesSnapshot = recentTrades;
                            _ = Task.Run(() => ExportTradeLogAsync(TradesSnapshot));
                        }
                    }
                }
            });
        }
        private void AddToRecentTrades(SocketTradeEntity trade)
        {
            if (recentTrades.Count >= MaxQueueSize)
            {
                recentTrades.Dequeue(); // 古いデータを削除
            }
            recentTrades.Enqueue(trade);
        }
        private void AddToExportNumber()
        {
            if (ExportNumbers.Count >= MaxQueueSize)
            {
                ExportNumbers.Dequeue(); // 古いデータを削除
            }
            ExportNumbers.Enqueue(SocketCounter + MaxQueueSize);// - 5);
        }
        private async Task ExportTradeLogAsync(Queue<SocketTradeEntity> recentTrades)
        {
            var CsvFolder = Path.Combine(Directory.GetCurrentDirectory(), "csv");
            Directory.CreateDirectory(CsvFolder);
            var lines = new List<string>();
            foreach(var trade in recentTrades)
            {
                lines.Add($"{trade.Timestamp},{trade.TradeId},{trade.Pair},{trade.Rate},{trade.Amount},{trade.OrderType},{trade.TakerOrderId},{trade.MakerOrderId}");
            }
            var CsvPath = Path.Combine(CsvFolder, $"{DateTime.Now.ToString("yyyyMMdd_hhmmss_fff")}.csv");
            await File.WriteAllLinesAsync(CsvPath, lines.ToArray());
            Console.WriteLine("csv exported.");
        }
        private bool IsBuyPattern()
        {
            if (recentTrades.Count < 4) return false;

            var trades = recentTrades.ToArray();

            if (trades[^1].OrderType != "buy") return false;
            if (trades[^2].OrderType != "sell" || trades[^3].OrderType != "sell" || trades[^4].OrderType != "sell")
                return false;
            var recentSellTrades = trades.TakeLast(4).Take(3).Where(t => t.OrderType == "sell").ToArray();
            if (recentSellTrades.Length < 3) return false;
            if (recentSellTrades.Max(t => t.Rate) - recentSellTrades.Min(t => t.Rate) > 5) return false;
            if (trades[^1].Rate >= recentSellTrades.Min(t => t.Rate)) return false;

            return true;
        }

        private List<SocketTradeEntity> ParseMessage(string message)
        {
            try
            {
                var trades = JsonConvert.DeserializeObject<List<List<string>>>(message);
                return trades.Select(trade => new SocketTradeEntity
                {
                    Timestamp = long.Parse(trade[0]),
                    TradeId = trade[1],
                    Pair = trade[2],
                    Rate = decimal.Parse(trade[3]),
                    Amount = decimal.Parse(trade[4]),
                    OrderType = trade[5],
                    TakerOrderId = trade[6],
                    MakerOrderId = trade[7]
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing message: {ex.Message}");
                return new List<SocketTradeEntity>();
            }
        }
    }
}
