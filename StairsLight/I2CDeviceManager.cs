using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio; 

namespace StairsLight
{
    static class I2CDeviceManager
    {
        //c#-port from https://github.com/mbazzo/PCA9685
        const ushort PCA9685_MODE1 = 0x00;
        const ushort PCA9685_MODE2 = 0x01;
        const ushort PCA9685_LED0 = 0x06;
        const ushort PCA9685_PRESC = 0xFE;
        const ushort PCA9685_I2C_ADR = 0x40;

        private static I2CDevice[] I2CDevices = new I2CDevice[50];

        public static I2CDevice GetDevice(int index)
        {
            lock (I2CDevices)
            {
                if (I2CDevices[index] == null)
                {
                    var device = Pi.I2C.AddDevice(index);
                    device.WriteAddressByte(PCA9685_MODE1, 17);
                    SetPrescaler(device, 50);
                    device.WriteAddressByte(PCA9685_MODE1, 1);
                    I2CDevices[index] = device;
                }
                return I2CDevices[index];
            }
        }

        private static void SetPrescaler(this I2CDevice device, int frequency)
        {
            device.WriteAddressByte(PCA9685_PRESC, (byte)((25000000 / (4096 * frequency)) - 1));
        }

        private static void SetPwm(this I2CDevice device, int on, int off, int channel)
        {
            device.WriteAddressByte(PCA9685_LED0 + 4 * channel, (byte)(on & 0xFF));
            device.WriteAddressByte(PCA9685_LED0 + 1 + 4 * channel, (byte)(on >> 8));
            device.WriteAddressByte(PCA9685_LED0 + 2 + 4 * channel, (byte)(off & 0xFF));
            device.WriteAddressByte(PCA9685_LED0 + 3 + 4 * channel, (byte)(off >> 8));
        }
    }
}
