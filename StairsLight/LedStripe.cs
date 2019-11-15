using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace StairsLight
{
    class LedStripe
    {
        private static List<LedStripe> ActiveStripes = new List<LedStripe>();

        public static IReadOnlyList<LedStripe> ActiveStripesReadOnly => ActiveStripes.AsReadOnly();

        IColorController RedController, BlueController, GreenController;

        public Color Color { get; private set; }

        public LedStripe(IColorController redController, IColorController greenController, IColorController blueController, Color color)
        {
            RedController = redController;
            BlueController = blueController;
            GreenController = greenController;
            lock(ActiveStripes)
            {
                ActiveStripes.Add(this);
            }
            SetColor(color);
        }

        public void SetColor(Color color)
        {
            Color = color;
            GreenController.SetBrightness(color.G / 255f);
            RedController.SetBrightness(color.R / 255f);
            BlueController.SetBrightness(color.B / 255f);
        }
    }
}
