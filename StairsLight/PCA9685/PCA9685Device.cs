using StairsLight.ColorControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unosquare.RaspberryIO.Gpio;
using Unosquare.RaspberryIO.Native;

namespace StairsLight.PCA9685
{
    class PCA9685Device
    {
        const ushort PCA9685_MODE1_ADDRESS = 0x00;
        const ushort PCA9685_MODE2_ADDRESS = 0x01;
        const ushort PCA9685_LED0_ADDRESS = 0x06;
        const ushort PCA9685_PRESCALE_ADDRESS = 0xFE;
        const ushort PCA9685_I2C_ADDRESS = 0x40;
        const int Frequency = 900;
        const int PwmResolution = 4096;
        const int LedCount = 16;

        private I2CDevice Device;
        private PCA9685ChannelController[] ChannelControllers = new PCA9685ChannelController[LedCount];

        public PCA9685Device(I2CDevice device)
        {
            Device = device;
            WriteAddressByte(PCA9685_MODE1_ADDRESS, 0x30);
            WriteAddressByte(PCA9685_MODE2_ADDRESS, 0x04);
            SetFrequency(Frequency);
            WriteAddressByte(PCA9685_MODE1_ADDRESS, 0x00);
        }

        private void SetFrequency(int frequency)
        {
            WriteAddressByte(PCA9685_PRESCALE_ADDRESS, CalculatePrescale(frequency));
        }

        private static byte CalculatePrescale(int frequency) => (byte)((25000000 / (4096 * frequency)) - 1);

        public void SetBrightness(int ledIndex, float percentage)
        {
            //We will use pulseStart = 0, so that the on-pulse start at the beginning of each pwm cycle
            int pulseEnd = (int)((PwmResolution - 1) * percentage);
            SetPwm(ledIndex, 0, pulseEnd);
        }

        private void SetPwm(int ledIndex, int pulseStart, int pulseEnd)
        {
            if (pulseStart >= PwmResolution)
                throw new ArgumentException($"{nameof(pulseStart)}={pulseStart} must be less than {nameof(PwmResolution)}={PwmResolution})");
            if (pulseEnd >= PwmResolution)
                throw new ArgumentException($"{nameof(pulseEnd)}={pulseEnd} must be less than {nameof(PwmResolution)}={PwmResolution})");
            if (pulseStart > pulseEnd)
                throw new ArgumentException($"{nameof(pulseStart)}={pulseStart} must be less than {nameof(pulseEnd)}={pulseEnd})");
            if (ledIndex >= LedCount)
                throw new ArgumentException($"{nameof(ledIndex)}={ledIndex} must be less than {nameof(LedCount)}={LedCount})");
            lock (Device)
            {
                //Console.WriteLine($"{ledIndex} {pulseStart} {pulseEnd} {PCA9685_LED0_ADDRESS + 4 * ledIndex}");
                WriteAddressByte(PCA9685_LED0_ADDRESS + 4 * ledIndex, (byte)(pulseStart & 0xFF));
                WriteAddressByte(PCA9685_LED0_ADDRESS + 1 + 4 * ledIndex, (byte)(pulseStart >> 8));
                WriteAddressByte(PCA9685_LED0_ADDRESS + 2 + 4 * ledIndex, (byte)(pulseEnd & 0xFF));
                WriteAddressByte(PCA9685_LED0_ADDRESS + 3 + 4 * ledIndex, (byte)(pulseEnd >> 8));
            }
        }

        void WriteAddressByte(int address, byte data, int maxRetryCount = 150)
        {
            bool success = false;
            for (int i = 0; i < maxRetryCount && !success; i++)
            {
                try
                {
                    Device.WriteAddressByte(address, data);
                    success = true;
                }
                catch (HardwareException e) when (e.ErrorCode == 121)
                {
                    if (i == maxRetryCount - 1)
                        throw;
                }
            }
        }

        public PCA9685ChannelController GetChannelController(int channelIndex)
        {
            if (channelIndex >= LedCount)
                throw new ArgumentException($"{nameof(channelIndex)}={channelIndex} must be less than {nameof(LedCount)}={LedCount}");
            lock(ChannelControllers)
            {
                if (ChannelControllers[channelIndex] == null)
                {
                    ChannelControllers[channelIndex] = new PCA9685ChannelController(this, channelIndex);
                }
                return ChannelControllers[channelIndex];
            }
        }
    }
}
