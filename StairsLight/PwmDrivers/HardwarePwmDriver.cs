﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unosquare.RaspberryIO.Gpio;

namespace StairsLight.PwmDrivers
{
    class HardwarePwmDriver : IPwmDriver
    {
        const int PwmRange = 1024;
        GpioPin Pin;
        public HardwarePwmDriver(GpioPin pin)
        {
            Pin = pin;
            if (!Pin.Capabilities.Contains(PinCapability.PWM))
                throw new NotImplementedException("You cant create a HardwarePwmDriver on a pin that does not support it!");
            Pin.PinMode = GpioPinDriveMode.PwmOutput;
            Pin.PwmMode = PwmMode.Balanced;
            Pin.PwmRegister = 0;
        }

        public void SetDutyCylce(float percentage)
        {
            if (percentage > 1f || percentage < 0f)
                throw new NotSupportedException($"The percentage must be between 0.0f and 1.0f, but it was {percentage}");
            Pin.PwmRegister = Convert.ToInt32(PwmRange * percentage);
        }
    }
}
