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
        TimeSpan DebounceTime = TimeSpan.FromSeconds(0.1);
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
                gpioPin.RegisterInterruptCallback(EdgeDetection.FallingEdge, FallingEdgeDetected);
            }
        }

        private void FallingEdgeDetected()
        {
            if (DateTime.UtcNow - LastChange < DebounceTime)
                return;

            LastChange = DateTime.UtcNow;
            State = !State;
            Console.WriteLine($"{DateTime.Now} Detected button press. Changing state to {State}");
            StateChanged?.Invoke(this, State);
        }
    }
}
