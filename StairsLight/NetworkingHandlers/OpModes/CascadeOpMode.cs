using StairsLight.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StairsLight.NetworkingHandlers.OpModes
{
    class CascadeOpMode : IOpMode
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

        public Dictionary<CascadeOperationModeProtocol, Action<MessageInfo>> RecieveReaction;

        public CascadeOpMode()
        {
            RecieveReaction = new Dictionary<CascadeOperationModeProtocol, Action<MessageInfo>>
            {
                { CascadeOperationModeProtocol.GetCascade, ProcessGetCascade },
                { CascadeOperationModeProtocol.GetSpeed, ProcesGetSpeed },
                { CascadeOperationModeProtocol.SetCascade, ProcessSetCascade },
                { CascadeOperationModeProtocol.SetSpeed, ProcessSetSpeed }
            };
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
            SendData(message, w => w.WriteEnum(CascadeOperationModeProtocol.GetSpeed)
                .WriteSingle(Speed));
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
            Activate();
        }

        private void ProcessGetCascade(MessageInfo message)
        {
            SendData(message, writer =>
            {
                writer.WriteEnum(CascadeOperationModeProtocol.GetCascade)
                    .WriteInt(ActiveCascade.Count);
                for (int i = 0; i < ActiveCascade.Count; i++)
                {
                    writer.WriteByte(ActiveCascade[i].Color.R)
                        .WriteByte(ActiveCascade[i].Color.G)
                        .WriteByte(ActiveCascade[i].Color.B)
                        .WriteInt(ActiveCascade[i].Width);
                }
                return writer;
            });
        }

        private void SendData(MessageInfo message, Func<FluentWriter, FluentWriter> writerFunc)
        {
            message.RespondWith(
                writerFunc(new FluentWriter()
                .WriteEnum(Protocol.OperationModeSpecific)
                .WriteEnum(OperationModeIdentifier.Cascade)));
        }

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
                else if (_active && !value)
                    Deactivate();
                _active = value;
            }
        }
        private void Deactivate()
        {
            UpdateCascadeTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        const int TicksPerSecond = 20;
        const int UpdateCascadeTimerFrequency = 1000 / TicksPerSecond;
        const float SpeedToIncrementMultiplier = 0.5f / TicksPerSecond;

        int StepChangeSteps;
        private void Activate()
        {
            UpdateCascadeTimer.Change(UpdateCascadeTimerFrequency, UpdateCascadeTimerFrequency);
            foreach (var stripe in LedStripe.ActiveStripesReadOnly)
            {
                stripe.SetColor(Color.Black);
            }
            StepChangeSteps = StepsCount + ActiveCascade.Sum(c => c.Width) - 1;
            CurrentOffset = 0;
        }

        private void ApplyCascadeWithOffset(float offset)
        {
            int currentOffset = (int)Math.Floor(offset);
            foreach (var cascadePart in ActiveCascade)
            {
                for (int i = 0; i < cascadePart.Width; i++)
                {
                    currentOffset++;
                    if (currentOffset >= LedStripe.ActiveStripesReadOnly.Count)
                        break;
                    LedStripe.ActiveStripesReadOnly[i].SetColor(cascadePart.Color);
                }
            }
        }

        Timer UpdateCascadeTimer;
        int StepsCount = LedStripe.ActiveStripesReadOnly.Count;

        float CurrentOffset;
        void UpdateCascade(object state)
        {
            if (!_active)
                return;
            Console.WriteLine("updateCascadeCalled");
            CurrentOffset += SpeedToIncrementMultiplier * Speed;
            ApplyCascadeWithOffset(CurrentOffset);
            if (CurrentOffset > StepChangeSteps)
                CurrentOffset = 0;
        }

        public void ProcessModeSpecificMessage(MessageInfo message)
        {
            message.Reader.ReadEnum<CascadeOperationModeProtocol>(out var protocol);
            if (!RecieveReaction.TryGetValue(protocol, out var reaction))
                throw new ParsingException(null, $"{nameof(CascadeOpMode)} received message with {nameof(CascadeOperationModeProtocol)}.{protocol} which is not supported");

            reaction(message);
        }
    }
}
