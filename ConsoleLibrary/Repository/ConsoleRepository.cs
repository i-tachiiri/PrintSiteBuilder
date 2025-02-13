namespace ConsoleLibrary.Repository
{
    public class ConsoleRepository
    {
        private static readonly object _lock = new object();
        private int _count;
        private string _lastId;

        private void WriteLog(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            string logMessage = $"[{timestamp}] {message}";

            // コンソール出力のみ
            lock (_lock)
            {
                Console.WriteLine(logMessage);
            }
        }

        public async Task TaskLog(Func<Task> task)
        {
            string taskName = task.Method.Name;
            WriteLog($"[Task] {taskName} 開始");
            try
            {
                await task();
                WriteLog($"[Task] {taskName} 完了");
            }
            catch (Exception ex)
            {
                WriteLog($"[Task] {taskName} 失敗: {ex.Message}");
            }
        }

        public void ExceptionLog(string message, [System.Runtime.CompilerServices.CallerMemberName] string methodName = "")
        {
            WriteLog($"[Error] {methodName}: {message}");
        }

        public void LoopLog(string loopId)
        {
            lock (_lock)
            {
                if (loopId == _lastId)
                {
                    _count++;
                }
                else
                {
                    _lastId = loopId;
                    _count = 1;
                }
                WriteLog($"[Loop] {_count}/X {loopId}");
            }
        }

        public void LoopLog(string loopId,int loopCount )
        {
            lock (_lock)
            {
                if (loopId == _lastId)
                {
                    _count++;
                }
                else
                {
                    _lastId = loopId;
                    _count = 1;
                }
                WriteLog($"[Loop] {_count}/{loopCount} {loopId}");
            }
        }

        public void LoopLog(string loopId, int loopCount,  string additionalInfo)
        {
            lock (_lock)
            {
                if (loopId == _lastId)
                {
                    _count++;
                }
                else
                {
                    _lastId = loopId;
                    _count = 1;
                }
                WriteLog($"[Loop] {_count}/{loopCount} {loopId}: {additionalInfo}");
            }
        }
    }
}
