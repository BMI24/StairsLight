using StairsLight.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StairsLight.NetworkingHandlers.OpModes
{
    class CircadiusOpMode : OpModeBase<CircadiusOperationModeProtocol>
    {
        class CircadiusPart
        {
            public CircadiusPart(TimeSpan time, Color color)
            {
                Time = time;
                Color = color;
            }

            public TimeSpan Time { get; }
            public Color Color { get; }
        }
        public CircadiusOpMode() : base (OperationModeIdentifier.Circadius)
        {
            RegisterReaction(CircadiusOperationModeProtocol.GetCircadialParts, ProcessGetCircadialParts);
            RegisterReaction(CircadiusOperationModeProtocol.SetCircadialParts, ProcessSetCircadialParts);
            ActiveCircadiusParts = new List<CircadiusPart>();
            UpdateColorTimer = new Timer(UpdateColor, null, Timeout.Infinite, Timeout.Infinite);
        }

        private void UpdateColor(object state)
        {
            if (!ActiveCircadiusParts.Any())
            {
                SetColor(Color.Black);
                return;
            }
            return;
            var currentTimeOfDay = DateTime.Now.TimeOfDay;
            var prevCircPart = ActiveCircadiusParts.LastOrDefault(p => p.Time < currentTimeOfDay) ?? ActiveCircadiusParts.Last();
            var nextCircPart = ActiveCircadiusParts.FirstOrDefault(p => p.Time >= currentTimeOfDay) ?? ActiveCircadiusParts.First();
            var ticksDiff = (nextCircPart.Time - prevCircPart.Time).Ticks;
            var ticksSinceLastPart = (currentTimeOfDay - prevCircPart.Time).Ticks;
            var completedFraction = (float)(ticksSinceLastPart / (double)ticksDiff);
            var newR = (byte)MathHelper.Lerp(prevCircPart.Color.R, nextCircPart.Color.R, completedFraction);
            var newG = (byte)MathHelper.Lerp(prevCircPart.Color.G, nextCircPart.Color.G, completedFraction);
            var newB = (byte)MathHelper.Lerp(prevCircPart.Color.B, nextCircPart.Color.B, completedFraction);
            var newColor = new Color(newR, newG, newB);
            SetColor(newColor);
        }

        private void SetColor(Color color)
        {
            foreach (var ledStripe in LedStripe.ActiveStripesReadOnly)
            {
                ledStripe.SetColor(color);
            }
        }

        Timer UpdateColorTimer;
        List<CircadiusPart> ActiveCircadiusParts;
        protected override void OnActivate()
        {
            //Keep in mind: setting TimeSpan.Zero as first argument will make the callback being called instantly
            UpdateColorTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(10));
        }
        protected override void OnDeactivate()
        {
            UpdateColorTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }
        private void ProcessSetCircadialParts(MessageInfo message)
        {
            List<CircadiusPart> parts = new List<CircadiusPart>();
            var reader = message.Reader;
            reader.ReadInt(out var count);
            for (int i = 0; i < count; i++)
            {
                reader.ReadTimeSpan(out var time)
                    .ReadColor(out var r, out var g, out var b);
                parts.Add(new CircadiusPart(time, new Color(r, g, b)));
            }
            ActiveCircadiusParts = parts;
            return;
            if (Active)
                UpdateColor(null);
        }

        private void ProcessGetCircadialParts(MessageInfo message)
        {
            SendData(message, w =>
            {
                w.WriteInt(ActiveCircadiusParts.Count);
                for (int i = 0; i < ActiveCircadiusParts.Count; i++)
                {
                    var color = ActiveCircadiusParts[i].Color;
                    w.WriteTimeSpan(ActiveCircadiusParts[i].Time)
                        .WriteColor(color.R, color.G, color.B);
                }
                return w;
            }, CircadiusOperationModeProtocol.GetCircadialParts);
        }
    }
}
