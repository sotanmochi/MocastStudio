using System;
using System.IO;
using System.Reflection;    

namespace ManagedPluginSample
{
    public class SampleClientWrapper
    {
        dynamic _client;

        public bool IsAvailable { get; }

        public SampleClientWrapper()
        {
            var directory = "../Plugin/bin/Debug/net8.0";
            // var directory = "../Plugin/bin/Release/net8.0";

            var dllName = "ManagedPluginSample.Plugin.dll";
            var typeFullName = "ManagedPluginSample.Plugin.DummyClient";

            try
            {
                var pluginLoader = new PluginLoader(Path.Combine(directory, dllName));
                _client = pluginLoader.CreateInstance(typeFullName);

                var connectedEventInfo = _client.GetType().GetEvent("OnConnected");
                Console.WriteLine($"[{nameof(SampleClientWrapper)}] ConnectedEventInfo: {connectedEventInfo.Name}");
                Console.WriteLine($"[{nameof(SampleClientWrapper)}] ConnectedEventInfo.EventHandlerType: {connectedEventInfo.EventHandlerType.Name}");

                var disconnectedEventInfo = _client.GetType().GetEvent("OnDisconnected");
                Console.WriteLine($"[{nameof(SampleClientWrapper)}] DisconnectedEventInfo: {disconnectedEventInfo.Name}");
                Console.WriteLine($"[{nameof(SampleClientWrapper)}] DisconnectedEventInfo.EventHandlerType: {disconnectedEventInfo.EventHandlerType.Name}");

                var connectedHandler = typeof(SampleClientWrapper).GetMethod("OnConnected", BindingFlags.Instance | BindingFlags.NonPublic);
                connectedEventInfo.AddEventHandler(_client,
                    Delegate.CreateDelegate(connectedEventInfo.EventHandlerType, this, connectedHandler));

                var disconnectedHandler = typeof(SampleClientWrapper).GetMethod("OnDisconnected", BindingFlags.Instance | BindingFlags.NonPublic);
                disconnectedEventInfo.AddEventHandler(_client,
                    Delegate.CreateDelegate(disconnectedEventInfo.EventHandlerType, this, disconnectedHandler));

                IsAvailable = true;
            }
            catch (Exception ex)
            {
                IsAvailable = false;
                Console.WriteLine(ex);
            }
        }

        public void Connect(string key)
        {
            if (!IsAvailable)
            {
                Console.WriteLine($"[{nameof(SampleClientWrapper)}] Plugin is not available.");
                return;
            }
            _client.Connect(key);
        }

        public void Disconnect()
        {
            if (!IsAvailable)
            {
                Console.WriteLine($"[{nameof(SampleClientWrapper)}] Plugin is not available.");
                return;
            }
            _client.Disconnect();
        }

        void OnConnected(object sender, EventArgs e, bool connected)
        {
            Console.WriteLine($"[{nameof(SampleClientWrapper)}] Connected: {connected}");
        }

        void OnDisconnected(object sender, EventArgs e)
        {
            Console.WriteLine($"[{nameof(SampleClientWrapper)}] Disconnected");
        }
    }
}