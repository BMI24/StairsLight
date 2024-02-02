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
        DateTime LastPress = DateTime.MinValue;
        DateTime LastChange = DateTime.MinValue;
        TimeSpan DebounceTime = TimeSpan.FromSeconds(0.5);
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
                gpioPin.RegisterInterruptCallback(EdgeDetection.RisingEdge, FallingEdgeDetected);
            }
        }

        private void FallingEdgeDetected()
        {
            Console.WriteLine($"{DateTime.Now:O} Detected button press");

            var timeSinceLastPress = DateTime.UtcNow - LastPress;
            LastPress = DateTime.UtcNow;

            if (timeSinceLastPress < TimeSpan.FromSeconds(0.15) 
                || timeSinceLastPress > TimeSpan.FromSeconds(1))
                return;

            var timeSinceLastChange = DateTime.UtcNow - LastChange;
            if (timeSinceLastChange < TimeSpan.FromSeconds(1))
                return;

            LastChange = DateTime.UtcNow;


            State = !State;
            Console.WriteLine($"Changing state to {State}");
            StateChanged?.Invoke(this, State);
        }
    }
}
