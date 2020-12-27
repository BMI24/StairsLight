using StairsLight.Networking;
using StairsLight.NetworkingHandlers.OpModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StairsLight.NetworkingHandlers
{
    class OperationModeManager
    {
        readonly Dictionary<Protocol, Action<MessageInfo>> RecieveReaction;
        public OperationModeManager()
        {
            RecieveReaction = new Dictionary<Protocol, Action<MessageInfo>>
            {
                { Protocol.GetBrightness, ProcessGetBrightness },
                { Protocol.SetBrightness, ProcessSetBrightness },
                { Protocol.GetOperationMode, ProcessGetOperationMode },
                { Protocol.SetOperationMode, ProcessSetOperationMode },
                { Protocol.OperationModeSpecific, ProcessOperationModeSpecific }
            };
            IdentifierToOpMode = new Dictionary<OperationModeIdentifier, IOpMode>()
            {
                { OperationModeIdentifier.Individual, new IndividualOpMode() },
                { OperationModeIdentifier.Cascade, new CascadeOpMode() }
            };
            CurrentOpMode = IdentifierToOpMode[OperationModeIdentifier.Individual];
        }

        Dictionary<OperationModeIdentifier, IOpMode> IdentifierToOpMode;

        static IOpMode _currentOpMode;
        static object CurrentOpModeLock = new object();
        static IOpMode CurrentOpMode
        {
            get
            {
                return _currentOpMode;
            }
            set
            {
                lock (CurrentOpModeLock)
                {
                    if (_currentOpMode != null)
                        _currentOpMode.Active = false;
                    (_currentOpMode = value).Active = true;
                }
            }
        }

        private void ProcessOperationModeSpecific(MessageInfo message)
        {
            CurrentOpMode.ProcessModeSpecificMessage(message);
        }


        private void ProcessSetOperationMode(MessageInfo message)
        {
            message.Reader.ReadEnum<OperationModeIdentifier>(out var newOpModeIdentifier);
            CurrentOpMode = IdentifierToOpMode[newOpModeIdentifier];
        }

        private void ProcessGetOperationMode(MessageInfo message)
        {
            var currentOpModeIdentifier = IdentifierToOpMode.First(kv => kv.Value == CurrentOpMode).Key;
            message.RespondWith(new FluentWriter()
                .WriteEnum(Protocol.GetOperationMode)
                .WriteEnum(currentOpModeIdentifier));
        }

        private void ProcessSetBrightness(MessageInfo message)
        {
            message.Reader.ReadSingle(out float brightness);
            LedStripe.Brightness = brightness;
        }

        private void ProcessGetBrightness(MessageInfo message)
        {
            message.RespondWith(new FluentWriter()
                .WriteEnum(Protocol.GetBrightness)
                .WriteSingle(LedStripe.Brightness));
        }
        
        public void ProcessMessage(MessageInfo message)
        {
            if (RecieveReaction.TryGetValue(message.Header, out var reaction))
            {
                reaction(message);
            }
        }
    }
}
