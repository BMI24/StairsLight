using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StairsLight.Networking
{
    public class NetworkClient
    {
        public const int ApplicationPort = 8012;

        public event EventHandler Disconnected = delegate { };

        Timer LifeClock;
        protected TcpClient Client;
        NetworkStream NetworkStream => Client?.Connected == true ? Client.GetStream() : null;
        public bool Connected => Client?.Connected ?? false;
        readonly Object NetworkStreamLock = new object();
        List<byte> RecievedBytes = new List<byte>();
        public event EventHandler<NetworkMessage> MessageRecieved = delegate { };
        const int SendKeepaliveOnTick = 100;
        private int CurrentKeepaliveTick = 0;

        public NetworkClient(TcpClient client)
        {
            Client = client;
            Client.ReceiveTimeout = Client.SendTimeout = 4000;
            LifeClock = new Timer(a => LifeTick());
            StartListening();
        }

        void LifeTick()
        {
            CurrentKeepaliveTick++;
            if (CurrentKeepaliveTick >= SendKeepaliveOnTick)
            {
                CurrentKeepaliveTick = 0;
                SendData(new byte[] { (byte)Protocol.KeepAlive });
            }
            if (!Client.Connected)
            {
                LifeClock.Change(Timeout.Infinite, Timeout.Infinite);
                return;
            }
            ReadFromStream();
        }

        void ReadFromStream()
        {
            while (NetworkStream?.DataAvailable == true)
            {
                #region Interactive test
                //using System.Net.Sockets;
                //
                //TcpListener listener = new TcpListener(System.Net.IPAddress.Parse("127.0.0.1"), 9917);
                //listener.Start();
                //TcpClient client = new TcpClient("127.0.0.1", 9917);
                //WriteLine(client.GetStream().CanRead);
                //TcpClient remoteClient = listener.AcceptTcpClient();
                //remoteClient.GetStream().WriteByte(12);
                //byte[] buffer = new byte[1024];
                //if (client.GetStream().DataAvailable)
                //{
                //    client.GetStream().Read(buffer, 0, buffer.Length);
                //
                //    foreach (byte b in buffer.Where(b => b != 0))
                //        Write(b + " ");
                //}
                //else
                //    WriteLine("No Data");
                #endregion
                int readBytes = 0;
                byte[] buffer = new byte[128];
                lock (NetworkStreamLock)
                {
                    //All data could allready be read while we waited for the lock to be released
                    if (!NetworkStream.DataAvailable) return;

                    readBytes = NetworkStream.Read(buffer, 0, buffer.Length);
                }
                lock (RecievedBytes)
                {
                    RecievedBytes.AddRange(buffer.Take(readBytes));
                }
            }
            ProcessByteMassage();
        }

        void ProcessByteMassage()
        {
            List<byte[]> subMessages = new List<byte[]>();
            lock (RecievedBytes)
            {
                int subMessageStartIndex = 0;
                while (true)
                {
                    if (subMessageStartIndex + 2 >= RecievedBytes.Count)
                        break;
                    short expectedLenght = BitConverter.ToInt16(new byte[] { RecievedBytes[subMessageStartIndex], RecievedBytes[subMessageStartIndex + 1] }, 0);
                    if (subMessageStartIndex + expectedLenght + 2 > RecievedBytes.Count) break;

                    var currentSubMessage = new byte[expectedLenght];
                    RecievedBytes.CopyTo(subMessageStartIndex + 2, currentSubMessage, 0, expectedLenght);
                    subMessages.Add(currentSubMessage);
                    subMessageStartIndex += 2 + expectedLenght;
                }
                List<byte> newRecievedBytes = new List<byte>();
                for (int i = subMessageStartIndex; i < RecievedBytes.Count; i++)
                {
                    newRecievedBytes.Add(RecievedBytes[i]);
                }
                RecievedBytes = newRecievedBytes;
            }
            foreach (var message in subMessages)
            {
                try
                {
                    Protocol protocol = (Protocol)message[0];
                    MessageRecieved(this, new NetworkMessage(protocol, message.Skip(1).ToArray()));
                }
                catch (InvalidCastException)
                {
                    Kill();
                }
            }
        }

        public void SendData(byte[] messageContent)
        {
            if (messageContent.Length >= 65563 /*=short.Max*/)
                throw new NotImplementedException("Sending Messages this big is not yet supported");

            try
            {
                byte[] messageWithWrapper = new byte[messageContent.Length + 2];
                Array.Copy(messageContent, 0, messageWithWrapper, 2, messageContent.Length);
                byte[] lengthInBytes = BitConverter.GetBytes(messageContent.Length);
                messageWithWrapper[0] = lengthInBytes[0];
                messageWithWrapper[1] = lengthInBytes[1];
                lock (NetworkStream)
                    NetworkStream.BeginWrite(messageWithWrapper, 0, messageWithWrapper.Length, new AsyncCallback(DataWritten), null);
            }
            catch (Exception e) when (e is ArgumentNullException || e is IOException || e is SocketException)
            {
                Kill();
            }
        }

        private void DataWritten(IAsyncResult result)
        {
            try
            {
                lock (NetworkStream)
                    NetworkStream.EndWrite(result);
            }
            catch (Exception e) when (e is ArgumentNullException || e is IOException || e is SocketException)
            {
                Kill();
            }
        }

        void StartListening()
        {
            LifeClock.Change(50, 20);
        }
        void StopListening()
        {
            LifeClock.Change(Timeout.Infinite, Timeout.Infinite);
        }
        public virtual void Kill()
        {
            Client.GetStream().Close();
            Client.Close();
            Disconnected(this, null);
            StopListening();
        }
    }
}
