using StairsLight.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StairsLight.NetworkingHandlers
{
    class ClientHandler : NetworkClient
    {
        public ClientHandler(TcpClient client) : base(client)
        {
            MessageRecieved += ProcessMessage;
        }

        private void ProcessMessage(object sender, NetworkMessage e)
        {
            try
            {
                ServerManager.Server.ProcessMessage(new MessageInfo(new FluentReader(e.Content), this, e.Header));
            }
            catch (ParsingException)
            {
                Console.WriteLine($"Dropped client {Client.Client.RemoteEndPoint} due to ParsingException with message header {e.Header} and body {BitConverter.ToString(e.Content)}");
                Kill();
            }
        }
    }
}
