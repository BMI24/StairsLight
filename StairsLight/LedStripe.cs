using System;
using System.Collections.Generic;
using System.Text;

namespace StairsLight
{
    class LedStripe
    {
        PinController RedController, BlueController, GreenController;

        public LedStripe(int redIndex, int blueIndex, int greenIndex)
        {
            RedController = PinController.GetGpioController(redIndex);
            BlueController = PinController.GetGpioController(blueIndex);
            GreenController = PinController.GetGpioController(greenIndex);
        }

        public void SetColor(Color color)
        {
            RedController.SetBrightness(255f / color.R);
            BlueController.SetBrightness(255f / color.B);
            GreenController.SetBrightness(255f / color.G);
        }
    }
}
