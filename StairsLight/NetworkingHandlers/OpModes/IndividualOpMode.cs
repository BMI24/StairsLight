using StairsLight.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StairsLight.NetworkingHandlers.OpModes
{
    class IndividualOpMode : IOpMode
    {
        public IndividualOpMode()
        {
            RecieveReaction = new Dictionary<IndividualOperationModeProtocol, Action<MessageInfo>>
            {
                { IndividualOperationModeProtocol.GetColor, ProcessGetColor },
                { IndividualOperationModeProtocol.GetStripeCount, ProcessGetStripeCount },
                { IndividualOperationModeProtocol.SetColor, ProcessSetColor }
            };
        }

        private void ProcessSetColor(MessageInfo message)
        {
            message.Reader.ReadInt(out int ledStripeIndex)
                .ReadByte(out byte r)
                .ReadByte(out byte g)
                .ReadByte(out byte b);
            LedStripe.ActiveStripesReadOnly[ledStripeIndex].SetColor(new Color(r, g, b));
        }

        Color[] StripeColors = new Color[LedStripe.ActiveStripesReadOnly.Count];

        private void ProcessGetStripeCount(MessageInfo message)
        {
            SendData(message, w => w.WriteEnum(IndividualOperationModeProtocol.GetStripeCount)
                .WriteInt(StripeColors.Length));
        }

        private void ProcessGetColor(MessageInfo message)
        {
            message.Reader.ReadInt(out int ledStripeIndex);
            var color = StripeColors[ledStripeIndex];
            SendData(message, w => w.WriteEnum(IndividualOperationModeProtocol.GetColor)
                .WriteInt(ledStripeIndex)
                .WriteByte(color.R)
                .WriteByte(color.G)
                .WriteByte(color.B));
        }

        readonly Dictionary<IndividualOperationModeProtocol, Action<MessageInfo>> RecieveReaction;

        bool _active;
        bool IOpMode.Active
        {
            get
            {
                return _active;
            }
            set
            {
                if (!_active && value)
                    Activate();
                _active = value;
            }
        }

        public void Activate()
        {
            for (int i = 0; i < StripeColors.Length; i++)
            {
                LedStripe.ActiveStripesReadOnly[i].SetColor(StripeColors[i]);
            }
        }

        private void SendData(MessageInfo message, Func<FluentWriter, FluentWriter> writerFunc)
        {
            message.RespondWith(
                writerFunc(new FluentWriter()
                .WriteEnum(Protocol.OperationModeSpecific)
                .WriteEnum(OperationModeIdentifier.Individual)));
        }

        public void ProcessModeSpecificMessage(MessageInfo message)
        {
            message.Reader.ReadEnum<OperationModeIdentifier>(out var opMode);
            if (opMode != OperationModeIdentifier.Individual)
                throw new ParsingException(null);

            message.Reader.ReadEnum<IndividualOperationModeProtocol>(out var protocol);
            if (!RecieveReaction.TryGetValue(protocol, out var reaction))
                throw new ParsingException(null);
            
            reaction(message);
        }
    }
}
