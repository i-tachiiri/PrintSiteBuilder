using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace CoincheckLibrary.External
{
    public class WebSocketClient
    {
        private ClientWebSocket _client;
        private string message = "{\"type\": \"subscribe\", \"channel\": \"btc_jpy-trades\"}";


        public WebSocketClient()
        {
            _client = new ClientWebSocket();
        }

        public async Task ConnectAsync(string uri)
        {
            Console.WriteLine("Connecting to WebSocket...");
            await _client.ConnectAsync(new Uri(uri), CancellationToken.None);
            Console.WriteLine("Connected.");
        }

        public async Task SendMessageAsync()
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            await _client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task ReceiveMessagesAsync(Func<string, Task> messageHandler)
        {
            var buffer = new byte[1024 * 4]; // 4 KB buffer
            while (_client.State == WebSocketState.Open)
            {
                var result = await _client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine("WebSocket closed by server.");
                    await DisconnectAsync();
                }
                else
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    await messageHandler(message);
                }
            }
        }

        public async Task DisconnectAsync()
        {
            if (_client.State == WebSocketState.Open || _client.State == WebSocketState.CloseReceived)
            {
                await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }
            _client.Dispose();
            Console.WriteLine("Disconnected.");
        }
    }
}
