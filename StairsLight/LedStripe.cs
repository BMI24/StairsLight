using StairsLight.ColorControllers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace StairsLight
{
    class LedStripe
    {
        private static object ActiveStripesLock { get; } = new object();
        private static List<LedStripe> ActiveStripes = new List<LedStripe>();

        public static IReadOnlyList<LedStripe> ActiveStripesReadOnly => ActiveStripes.AsReadOnly();

        private static float _brightness;

        static LedStripe()
        {
            Brightness = 1;
        }

        public static float Brightness
        {
            get
            {
                lock (ActiveStripesLock)
                {
                    return _brightness;
                }
            }
            set
            {
                lock (ActiveStripesLock)
                {
                    _brightness = value;
                    foreach (var stripe in ActiveStripes)
                    {
                        stripe.RefreshShownColor();
                    }
                }
            }
        }

        IColorController RedController, BlueController, GreenController;

        public Color Color { get; private set; }

        public LedStripe(IColorController redController, IColorController greenController, IColorController blueController, Color color)
        {
            RedController = redController;
            BlueController = blueController;
            GreenController = greenController;
            lock(ActiveStripesLock)
            {
                ActiveStripes.Add(this);
            }
            SetColor(color);
        }

        public void SetColor(Color color)
        {
            if (Color != color)
            {
                Color = color;
                RefreshShownColor();
            }
        }

        private void RefreshShownColor()
        {
            GreenController.SetBrightness(Color.G / 255f * Brightness);
            RedController.SetBrightness(Color.R / 255f * Brightness);
            BlueController.SetBrightness(Color.B / 255f * Brightness);
        }
    }
}
