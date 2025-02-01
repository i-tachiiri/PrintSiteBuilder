using CryptoMonitor.Application;

int retryCount = 0;
int maxRetries = 10;
bool shouldReconnect = true;

WebSocketHandler handler = new WebSocketHandler();
while (shouldReconnect)
{
    try
    {
        await handler.StartAsync();
        shouldReconnect = false; // 正常終了時は再接続しない
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unhandled error: {ex.Message}");

        retryCount++;
        if (retryCount > maxRetries)
        {
            Console.WriteLine("Max retry attempts reached. Exiting...");
            break;
        }

        int waitTime = Math.Min(5000 * retryCount, 60000); // 最大1分まで待機
        Console.WriteLine($"Retrying in {waitTime / 1000} seconds...");
        await Task.Delay(waitTime);
    }
}