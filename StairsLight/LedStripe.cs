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
            //RedController = PinController.GetGpioController(redIndex);
            //BlueController = PinController.GetGpioController(blueIndex);
            GreenController = PinController.GetGpioController(greenIndex);
        }

        public void SetColor(Color color)
        {
            Console.WriteLine(color.G / 255f);
            GreenController.SetBrightness(color.G / 255f);
            //RedController.SetBrightness(color.R / 255f);
            //BlueController.SetBrightness(color.B / 255f);
        }
    }
}
