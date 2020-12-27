using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StairsLight.NetworkingHandlers
{
    class Listener
    {
        private TcpListener TcpListener { get; set; }
        private bool AcceptConnections { get; set; } = false;
        private IPAddress IPAddress { get; }
        private int Port { get; }

        public Listener(int port, IPAddress ip)
        {
            IPAddress = ip;
            Port = port;
            TcpListener = new TcpListener(IPAddress, Port);
        }

        async void Listen()
        {
            while (AcceptConnections)
            {
                var client = await TcpListener.AcceptTcpClientAsync();
                if (client == null) continue;
                client.NoDelay = true;
                Console.WriteLine($"Client mit IP {client.Client.RemoteEndPoint} verbunden");
                new ClientHandler(client);
            }
        }

        public void StartListening()
        {
            AcceptConnections = true;
            Console.WriteLine($"Listener started. Listening to TCP clients at {IPAddress}:{Port}");
            TcpListener.Start();
            Listen();
        }

        public void StopListening()
        {
            TcpListener.Stop();
            AcceptConnections = true;
            Console.WriteLine($"Listener stopped. No more listening to TCP clients at {IPAddress}:{Port}");
        }
    }
}
