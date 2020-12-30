using StairsLight.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StairsLight.NetworkingHandlers.OpModes
{
    abstract class OpModeBase<T> : IOpMode where T : Enum
    {
        private OperationModeIdentifier OpModeIdentifier;
        private Dictionary<T, Action<MessageInfo>> ReceiveReaction;
        protected OpModeBase(OperationModeIdentifier identifier)
        {
            OpModeIdentifier = identifier;
            ReceiveReaction = new Dictionary<T, Action<MessageInfo>>();
        }

        protected void RegisterReaction(T enumValue, Action<MessageInfo> reaction)
        {
            ReceiveReaction.Add(enumValue, reaction);
        }

        private bool _active;
        public bool Active
        {
            get => _active;
            set
            {
                if (value && !_active)
                    OnActivate();
                else if (!value && _active)
                    OnDeactivate();

                _active = value;
            }
        }

        public void ProcessModeSpecificMessage(MessageInfo message)
        {
            message.Reader.ReadEnum<T>(out var protocol);
            if (!ReceiveReaction.TryGetValue(protocol, out var reaction))
                throw new ParsingException(null, $"{nameof(OpModeBase<T>)} received message with {nameof(T)}.{protocol} which is not supported");

            reaction(message);
        }

        protected virtual void OnActivate() { }
        protected virtual void OnDeactivate() { }

        protected void SendData(MessageInfo message, Func<FluentWriter, FluentWriter> writerFunc, T modeSpecificResponseHeader)
        {
            message.RespondWith(
                writerFunc(new FluentWriter()
                .WriteEnum(Protocol.OperationModeSpecific)
                .WriteEnum(OpModeIdentifier)
                .WriteEnum(modeSpecificResponseHeader)));
        }
    }
}
