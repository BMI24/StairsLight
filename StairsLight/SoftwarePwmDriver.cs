using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unosquare.RaspberryIO.Gpio;

namespace StairsLight
{
    class SoftwarePwmDriver : IPwmDriver
    {
        const int PwmRange = 1024;
        GpioPin Pin;
        public SoftwarePwmDriver(GpioPin pin)
        {
            Pin = pin;
            if (Pin.Capabilities.Contains(PinCapability.PWM))
                Console.WriteLine($"GPIO Pin {Pin.BcmPinNumber} supports hardware PWM mode but runs software mode");
            Pin.PinMode = GpioPinDriveMode.Output;
            pin.StartSoftPwm(0, PwmRange);
        }

        public void SetDutyCylce(float percentage)
        {
            Pin.SoftPwmValue = Convert.ToInt32(PwmRange * percentage);
        }
    }
}
