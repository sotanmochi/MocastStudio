using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SignalStreamingSamples.ConsoleAppClient
{
    class Program
    {
        static readonly double TimestampsToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;
        static readonly int TargetFrameTimeMilliseconds = 1000 / 60;

        static async Task Main(string[] args)
        {
            string serverAddress = "localhost";
            ushort port = 3333;
            string connectionKey = "SignalStreaming";

            Log($"[{nameof(ConsoleAppClient)}] Main @Thread: {Thread.CurrentThread.ManagedThreadId}");

            var client = new SampleClient(serverAddress, port, connectionKey);
            Task.Run(async() => client.StartAsync());

            // Main loop
            var sw = new SpinWait();
            while (true)
            {
                var begin = Stopwatch.GetTimestamp();

                sw.SpinOnce(); // No Operation

                var end = Stopwatch.GetTimestamp();
                var elapsedTicks = (end - begin) * TimestampsToTicks;
                var elapsedMilliseconds = (long)elapsedTicks / TimeSpan.TicksPerMillisecond;

                var waitForNextFrameMilliseconds = (int)(TargetFrameTimeMilliseconds - elapsedMilliseconds);
                if (waitForNextFrameMilliseconds > 0)
                {
                    Thread.Sleep(waitForNextFrameMilliseconds);
                }
            }
        }

        static void Log(object message)
        {
            Console.WriteLine(message);
        }
    }
}
