using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace StairsLight
{
    class OnOffButtonController
    {
        public static OnOffButtonController Instance;

        public event EventHandler<bool> StateChanged;
        public bool State = true;
        DateTime LastChange = DateTime.MinValue;
        TimeSpan MinTimeSpanBetweenChanges = TimeSpan.FromSeconds(1);
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
                gpioPin.InputPullMode = GpioPinResistorPullMode.PullDown;
                gpioPin.RegisterInterruptCallback(EdgeDetection.RisingEdge, RisingEdgeDetected);
            }
        }

        private void RisingEdgeDetected()
        {
            if (DateTime.UtcNow - LastChange < MinTimeSpanBetweenChanges)
                return;

            LastChange = DateTime.UtcNow;
            State = !State;
            StateChanged?.Invoke(this, State);
        }
    }
}
