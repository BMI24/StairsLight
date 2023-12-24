using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace StairsLight
{
    class OnOffButtonController
    {
        public static OnOffButtonController Instance;

        public event EventHandler<bool> StateChanged;
        public bool State;

        public static void Initialize(params int[] buttonPcms)
        {
            Instance = new OnOffButtonController(buttonPcms);
        }

        public OnOffButtonController(params int[] buttonBcms)
        {
            foreach (int buttonBcm in buttonBcms)
            {
                var gpioPin = Pi.Gpio.GetGpioPinByBcmPinNumber(buttonBcm);
                gpioPin.PinMode = GpioPinDriveMode.Input;
                gpioPin.InputPullMode = GpioPinResistorPullMode.PullUp;
                gpioPin.RegisterInterruptCallback(EdgeDetection.RisingEdge, RisingEdgeDetected);
            }
        }

        private void RisingEdgeDetected()
        {
            State = !State;
            StateChanged?.Invoke(this, State);
        }
    }
}
