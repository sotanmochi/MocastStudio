using System;
using System.Threading;
using System.Threading.Tasks;

namespace MocastStudio.Server.SignalStreaming
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine($"[{nameof(StreamingHubServer)}] Start of Main.");

            var listenPort = 50010;

            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == "--port" || args[i] == "-p")
                {
                    listenPort = int.Parse(args[i + 1]);
                }
            }

            var cancellationTokenSource = new CancellationTokenSource();
            RegisterAppEventHandlers(cancellationTokenSource);

            using var streamingHub = new StreamingHubServer((ushort)listenPort);

            // var mainLoopTask = Task.Run(() => streamingHub.RunMainLoop(cancellationTokenSource.Token));
            // var signalDispatcherTask = Task.Run(() => streamingHub.RunSignalDispatcher(cancellationTokenSource.Token));
            var transportEventLoopTask = Task.Run(() => streamingHub.RunTransportEventLoop(cancellationTokenSource.Token));

            try
            {
                await transportEventLoopTask;
                // await signalDispatcherTask;
                // await mainLoopTask;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"[{nameof(StreamingHubServer)}] Loop tasks have been finished."); // 不要？
            }
            catch (Exception e)
            {
                Console.WriteLine($"[{nameof(StreamingHubServer)}] Error: {e}");
            }

            Console.WriteLine($"[{nameof(StreamingHubServer)}] End of Mains.");
        }

        static void RegisterAppEventHandlers(CancellationTokenSource cancellationTokenSource)
        {
            AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) =>
            {
                cancellationTokenSource?.Cancel();
                cancellationTokenSource?.Dispose();
                cancellationTokenSource = null;

                Console.WriteLine($"[{nameof(StreamingHubServer)}] ProcessExit");
            };

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cancellationTokenSource?.Cancel();
                cancellationTokenSource?.Dispose();
                cancellationTokenSource = null;

                Console.WriteLine($"[{nameof(StreamingHubServer)}] CancelKeyPress");
            };
        }
    }
}
