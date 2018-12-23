using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace StairsLight
{
    class LedStripe
    {
        private static List<LedStripe> ActiveStripes = new List<LedStripe>();
        static LedStripe()
        {
            Timer colorUpdateTimer = new Timer(UpdateColor, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1f / 5));
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
        }

        PinController RedController, BlueController, GreenController;

        public IColorProvider ColorProvider;

        public LedStripe(int redIndex, int blueIndex, int greenIndex, IColorProvider colorProvider)
        {
            RedController = PinController.GetGpioController(redIndex);
            BlueController = PinController.GetGpioController(blueIndex);
            GreenController = PinController.GetGpioController(greenIndex);
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
