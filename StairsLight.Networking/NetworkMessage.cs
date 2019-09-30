using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StairsLight.Networking
{
    public class NetworkMessage
    {
        public Protocol Header;
        public byte[] Content;

        public NetworkMessage(Protocol header, byte[] content)
        {
            Header = header;
            Content = content;
        }
    }
}
