using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio; 

namespace StairsLight
{
    static class PCA9685Manager
    {
        private static PCA9685Device[] PCA9685Devices = new PCA9685Device[50];

        public static PCA9685Device GetDevice(int i2cAddress)
        {
            lock (PCA9685Devices)
            {
                if (PCA9685Devices[i2cAddress] == null)
                {
                    var i2cDevice = Pi.I2C.AddDevice(i2cAddress);
                    PCA9685Devices[i2cAddress] = new PCA9685Device(i2cDevice);
                }
                return PCA9685Devices[i2cAddress];
            }
        }
    }
}
