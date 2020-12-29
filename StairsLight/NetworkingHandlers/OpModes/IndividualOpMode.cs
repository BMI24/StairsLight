using StairsLight.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StairsLight.NetworkingHandlers.OpModes
{
    class IndividualOpMode : OpModeBase<IndividualOperationModeProtocol>
    {
        public IndividualOpMode() : base(OperationModeIdentifier.Individual)
        {
            RegisterReaction(IndividualOperationModeProtocol.GetColor, ProcessGetColor);
            RegisterReaction(IndividualOperationModeProtocol.GetStripeCount, ProcessGetStripeCount);
            RegisterReaction(IndividualOperationModeProtocol.SetColor, ProcessSetColor);
        }

        private void ProcessSetColor(MessageInfo message)
        {
            message.Reader.ReadInt(out int ledStripeIndex)
                .ReadByte(out byte r)
                .ReadByte(out byte g)
                .ReadByte(out byte b);
            var color = new Color(r, g, b);
            StripeColors[ledStripeIndex] = color;
            LedStripe.ActiveStripesReadOnly[ledStripeIndex].SetColor(color);
        }

        Color[] StripeColors = new Color[LedStripe.ActiveStripesReadOnly.Count];

        private void ProcessGetStripeCount(MessageInfo message)
        {
            SendData(message, w => w.WriteInt(StripeColors.Length), IndividualOperationModeProtocol.GetStripeCount);
        }

        private void ProcessGetColor(MessageInfo message)
        {
            message.Reader.ReadInt(out int ledStripeIndex);
            var color = StripeColors[ledStripeIndex];
            SendData(message, w => w.WriteInt(ledStripeIndex)
                .WriteByte(color.R)
                .WriteByte(color.G)
                .WriteByte(color.B), IndividualOperationModeProtocol.GetColor);
        }

        protected override void OnActivate()
        {
            for (int i = 0; i < StripeColors.Length; i++)
            {
                LedStripe.ActiveStripesReadOnly[i].SetColor(StripeColors[i]);
            }
        }
    }
}
