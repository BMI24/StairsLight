using StairsLight.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StairsLight.NetworkingHandlers.OpModes
{
    class CascadeOpMode : OpModeBase<CascadeOperationModeProtocol>
    {
        class CascadePart
        {
            public int Width { get; set; }
            public Color Color { get; set; }

            public CascadePart(int width, Color color)
            {
                Width = width;
                Color = color;
            }
        }
        public CascadeOpMode() : base(OperationModeIdentifier.Cascade)
        {
            RegisterReaction(CascadeOperationModeProtocol.GetCascade, ProcessGetCascade);
            RegisterReaction(CascadeOperationModeProtocol.GetSpeed, ProcesGetSpeed);
            RegisterReaction(CascadeOperationModeProtocol.SetCascade, ProcessSetCascade);
            RegisterReaction(CascadeOperationModeProtocol.SetSpeed, ProcessSetSpeed);
           
            ActiveCascade = new List<CascadePart>();
            UpdateCascadeTimer = new Timer(UpdateCascade, null, Timeout.Infinite, Timeout.Infinite);
        }

        float Speed = 1;
        private void ProcessSetSpeed(MessageInfo message)
        {
            message.Reader.ReadSingle(out Speed);
        }

        private void ProcesGetSpeed(MessageInfo message)
        {
            SendData(message, w => w.WriteSingle(Speed), CascadeOperationModeProtocol.GetSpeed);
        }

        List<CascadePart> _activeCascade;
        private List<CascadePart> ActiveCascade 
        { 
            get => _activeCascade;
            set
            {
                CurrentOffset = 0;
                _activeCascade = value;
            }
        }
        private void ProcessSetCascade(MessageInfo message)
        {
            message.Reader.ReadInt(out var cascadeLength);
            var newCascade = new List<CascadePart>(cascadeLength);
            for (int i = 0; i < cascadeLength; i++)
            {
                message.Reader.ReadByte(out var colorR)
                    .ReadByte(out var colorG)
                    .ReadByte(out var colorB)
                    .ReadInt(out var width);
                newCascade.Add(new CascadePart(width, new Color(colorR, colorG, colorB)));
            }
            ActiveCascade = newCascade;
            if (Active)
                OnActivate();
        }

        private void ProcessGetCascade(MessageInfo message)
        {
            SendData(message, writer =>
            {
                writer.WriteInt(ActiveCascade.Count);
                for (int i = 0; i < ActiveCascade.Count; i++)
                {
                    writer.WriteByte(ActiveCascade[i].Color.R)
                        .WriteByte(ActiveCascade[i].Color.G)
                        .WriteByte(ActiveCascade[i].Color.B)
                        .WriteInt(ActiveCascade[i].Width);
                }
                return writer;
            }, CascadeOperationModeProtocol.GetCascade);
        }
        protected override void OnActivate()
        {
            UpdateCascadeTimer.Change(UpdateCascadeTimerFrequency, UpdateCascadeTimerFrequency);
            foreach (var stripe in LedStripe.ActiveStripesReadOnly)
            {
                stripe.SetColor(Color.Black);
            }
            LastTickSetStripesIndices = null;
            StepChangeSteps = StepsCount - 1;
            CurrentOffset = 0;
        }
        protected override void OnDeactivate()
        {
            UpdateCascadeTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        const int TicksPerSecond = 20;
        const int UpdateCascadeTimerFrequency = 1000 / TicksPerSecond;
        const float SpeedToIncrementMultiplier = 0.5f / TicksPerSecond;
        int StepChangeSteps;

        List<int> LastTickSetStripesIndices;
        private void ApplyCascadeWithOffset(int currentOffset)
        {
            var currentTickSetIndices = new List<int>();
            foreach (var cascadePart in ActiveCascade)
            {
                for (int i = 0; i < cascadePart.Width; i++)
                {
                    if (currentOffset == LedStripe.ActiveStripesReadOnly.Count)
                        currentOffset = 0;
                    LedStripe.ActiveStripesReadOnly[currentOffset].SetColor(cascadePart.Color);
                    currentTickSetIndices.Add(currentOffset);
                    currentOffset++;
                }
            }
            if (LastTickSetStripesIndices != null)
            {
                foreach (var index in LastTickSetStripesIndices.Except(currentTickSetIndices))
                {
                    LedStripe.ActiveStripesReadOnly[index].SetColor(Color.Black);
                }
            }
            LastTickSetStripesIndices = currentTickSetIndices;
        }

        Timer UpdateCascadeTimer;
        int StepsCount = LedStripe.ActiveStripesReadOnly.Count;

        float CurrentOffset;
        void UpdateCascade(object state)
        {
            if (!Active)
                return;

            CurrentOffset += SpeedToIncrementMultiplier * Speed;
            var offsetInt = (int)Math.Floor(CurrentOffset);
            if (offsetInt > StepChangeSteps + 1)
            {
                CurrentOffset = 0;
                offsetInt = 0;
            }
            if (offsetInt < 0)
            {
                CurrentOffset = StepChangeSteps + 1;
                offsetInt = StepChangeSteps + 1;
            }

            ApplyCascadeWithOffset(offsetInt);
        }
    }
}
