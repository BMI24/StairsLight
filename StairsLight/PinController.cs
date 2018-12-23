using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace StairsLight
{
    class PinController
    {
        private static Dictionary<int, PinController> ExistingControllers = new Dictionary<int, PinController>();
        public static PinController GetGpioController(int bcmIndex)
        {
            if (bcmIndex > 27 || bcmIndex < 0)
                throw new NotSupportedException("BCM Pins are numbered 0-27");

            lock (ExistingControllers)
            {
                PinController controller;
                if (!ExistingControllers.TryGetValue(bcmIndex, out controller))
                {
                    controller = new PinController(bcmIndex);
                    ExistingControllers[bcmIndex] = controller;
                }
                return controller;
            }
        }

        public void SetBrightness(float percentage) => PwmDriver.SetDutyCylce(percentage);

        public readonly int BcmIndex;
        public readonly GpioPin Pin;
        private IPwmDriver PwmDriver;

        public PinController(int bcmIndex, int dutyCycle = 0)
        {
            BcmIndex = bcmIndex;
            Pin = Pi.Gpio.GetGpioPinByBcmPinNumber(bcmIndex);
            PwmDriver = Pin.Capabilities.Contains(PinCapability.PWM) ? (IPwmDriver) new HardwarePwmDriver(Pin) : new SoftwarePwmDriver(Pin);
            PwmDriver.SetDutyCylce(dutyCycle);
        }
    }
}
