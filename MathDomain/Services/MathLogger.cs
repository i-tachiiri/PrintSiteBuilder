


namespace MathDomain.Services
{
    public class MathLogger
    {
        private int count;
        private string LastId;
        public async Task LogMessage(string Message)
        {
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]{Message}");
        }
        public async Task TaskLog(Func<Task> task)
        {
            string taskName = task.Method.Name;
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]{taskName}");
            await task();
        }
        public void ExceptionLog(string message, [System.Runtime.CompilerServices.CallerMemberName] string methodName = "")
        {
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}][Error]{methodName}{message}");
        }
        public void LoopLog(string LoopId)
        {
            if (LoopId == LastId)
            {
                count++;
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}][{count}/X]{LoopId}");
            }
            else
            {
                LastId = LoopId;
                count = 1;
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}][{count}/X]{LoopId}");
            }
        }
        public void LoopLog(string LoopId,int LoopCount)
        {
            if(LoopId == LastId)
            {
                count++;
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}][{count}/{LoopCount}]{LoopId}");
            }
            else
            {
                LastId = LoopId;
                count = 1;
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}][{count}/{LoopCount}]{LoopId}");
            }
        }
        public void LoopLog(string LoopId, int LoopCount,string AdditionalInfo)
        {
            if (LoopId == LastId)
            {
                count++;
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}][{count}/{LoopCount}]{LoopId}:{AdditionalInfo}");
            }
            else
            {
                LastId = LoopId;
                count = 1;
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}][{count}/{LoopCount}]{LoopId}:{AdditionalInfo}");
            }
        }
    }
}
