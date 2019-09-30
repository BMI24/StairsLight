using StairsLight.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StairsLight
{
    class NetworkServer : Networking.NetworkClient
    {
        public NetworkServer(TcpClient client) : base(client)
        {
            RecieveReaction = new Dictionary<Protocol, Action<byte[]>>
            {
                { Protocol.GetColor, ProcessGetColor },
                { Protocol.GetStripeCount, ProcessGetStripeCount },
                { Protocol.SetColor, ProcessSetColor }
            };
            MessageRecieved += ProcessMessage;
        }

        private void ProcessSetColor(byte[] content)
        {
            new FluentReader(content)
                .ReadInt(out int ledStripeIndex)
                .ReadByte(out byte r)
                .ReadByte(out byte g)
                .ReadByte(out byte b);
            LedStripe.ActiveStripesReadOnly[ledStripeIndex].ColorProvider = new ConstantColorProvider(new Color(r, g, b));
        }

        private void ProcessGetStripeCount(byte[] b)
        {
            SendData(new FluentWriter()
                .WriteEnum(Protocol.GetStripeCount)
                .WriteInt(LedStripe.ActiveStripesReadOnly.Count));
        }

        private void ProcessGetColor(byte[] b)
        {
            new FluentReader(b)
                .ReadInt(out int ledStripeIndex);
            var color = LedStripe.ActiveStripesReadOnly[ledStripeIndex].ColorProvider.NextTickColor;
            SendData(new FluentWriter()
                .WriteEnum(Protocol.GetColor)
                .WriteInt(ledStripeIndex)
                .WriteByte(color.R)
                .WriteByte(color.G)
                .WriteByte(color.B)
                .ToByteArray());
        }
        


        protected void ProcessMessage(object sender, NetworkMessage args)
        {
            if (RecieveReaction.TryGetValue(args.Header, out Action<byte[]> reaction))
            {
                try
                {
                    reaction(args.Content);
                }
                catch (ParsingException)
                {
                    Kill();
                }
            }
        }
        readonly Dictionary<Protocol, Action<byte[]>> RecieveReaction;
    }
}
