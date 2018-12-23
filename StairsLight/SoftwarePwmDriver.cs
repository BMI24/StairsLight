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
            if (percentage > 1f || percentage < 0f)
                throw new NotSupportedException($"The percentage must be between 0.0f and 1.0f, but it was {percentage}");
            Pin.SoftPwmValue = Convert.ToInt32(PwmRange * percentage);
        }
    }
}
