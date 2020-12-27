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
                if (e.Content == null)
                    Console.WriteLine("e is null");
                ServerManager.Server.ProcessMessage(new MessageInfo(new FluentReader(e.Content), this, e.Header));
            }
            catch (ParsingException)
            {
                Kill();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
