using System;

namespace ManagedPluginSample.Plugin
{
    public class DummyClient
    {
        public delegate void ConnectEventHandler(object sender, EventArgs e, bool connected);
        public delegate void DisconnectEventHandler(object sender, EventArgs e);

        public event ConnectEventHandler OnConnected;
        public event DisconnectEventHandler OnDisconnected;

        public bool IsConnected { get; private set; }

        public void Connect(string key)
        {
            Console.WriteLine($"[{nameof(DummyClient)}] Connect");
            IsConnected = key == "1234567890";
            OnConnected?.Invoke((object) this, new EventArgs(), IsConnected);
        }

        public void Disconnect()
        {
            Console.WriteLine($"[{nameof(DummyClient)}] Disconnect");
            IsConnected = false;
            OnDisconnected?.Invoke((object) this, new EventArgs());
        }
    }
}
