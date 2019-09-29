using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace StairsLight
{
    class LedStripe
    {
        private static List<LedStripe> ActiveStripes = new List<LedStripe>();
        private static int Count = 0;
        static LedStripe()
        {
            Timer colorUpdateTimer = new Timer(UpdateColor, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1f / 100));
            Timer colorUpdateFrequencyChecker = new Timer(PrintUpdateColorFrequency, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(10));
        }

        private static void UpdateColor(object state)
        {
            lock(ActiveStripes)
            {
                foreach(var stripe in ActiveStripes)
                {
                    stripe.SetColor(stripe.ColorProvider.NextTickColor);
                }
            }
            Count++;
        }

        private static void PrintUpdateColorFrequency(object state)
        {
            Console.WriteLine(Count);
            Count = 0;
        }

        IColorController RedController, BlueController, GreenController;

        public IColorProvider ColorProvider;

        public LedStripe(IColorController redController, IColorController blueController, IColorController greenController, IColorProvider colorProvider)
        {
            RedController = redController;
            BlueController = blueController;
            GreenController = greenController;
            ColorProvider = colorProvider;
            lock(ActiveStripes)
            {
                ActiveStripes.Add(this);
            }
        }

        public void SetColor(Color color)
        {
            GreenController.SetBrightness(color.G / 255f);
            RedController.SetBrightness(color.R / 255f);
            BlueController.SetBrightness(color.B / 255f);
        }
    }
}
