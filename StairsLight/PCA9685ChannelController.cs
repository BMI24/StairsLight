using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unosquare.RaspberryIO.Gpio;

namespace StairsLight
{
    class PCA9685ChannelController : IColorController
    {
        private int LedIndex;
        private PCA9685Device PCADevice;
        public void SetBrightness(float percentage)
        {
            PCADevice.SetBrightness(LedIndex, percentage);
        }

        public PCA9685ChannelController(PCA9685Device pcaDevice, int ledIndex)
        {
            PCADevice = pcaDevice;
            LedIndex = ledIndex;
        }
    }
}
