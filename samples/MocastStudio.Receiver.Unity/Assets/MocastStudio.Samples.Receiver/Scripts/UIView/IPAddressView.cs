using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MocastStudio.Samples.Receiver.UIView
{
    public sealed class IPAddressView : MonoBehaviour
    {
        [SerializeField] Text _ipAddressText;

        void Awake()
        {
            var ipAddresses = GetIpAddresses();
            _ipAddressText.text = (ipAddresses.Count > 0)
                ? $"Local IP Address: {ipAddresses[0]}"
                : $"Local IP Address:";
        }

        List<string> GetIpAddresses()
        {
            var ipAddresses = new List<string>();

            foreach (var ipAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
               if (ipAddress.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ipAddress))
                {
                    ipAddresses.Add(ipAddress.ToString());
                }
            }

            return ipAddresses;
        }
    }
}
