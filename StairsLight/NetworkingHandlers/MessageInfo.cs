using StairsLight.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StairsLight.NetworkingHandlers
{
    class MessageInfo
    {
        public FluentReader Reader { get; }
        ClientHandler Source;
        public Protocol Header { get; }

        public MessageInfo(FluentReader reader, ClientHandler source, Protocol header)
        {
            Reader = reader;
            Source = source;
            Header = header;
        }

        public void RespondWith(byte[] data)
        {
            Source.SendData(data);
        }
    }
}
