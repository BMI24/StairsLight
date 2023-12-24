using StairsLight.PCA9685;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unosquare.RaspberryIO.Gpio;

namespace StairsLight.ColorControllers
{
    class PCA9685ChannelController : IColorController
    {
        private int LedIndex;
        private PCA9685Device PCADevice;
        public float Brightness { get; set; }

        public void Refresh()
        {
            PCADevice.SetBrightness(LedIndex, OnOffButtonController.Instance.State ? Brightness : 0);
        }

        public PCA9685ChannelController(PCA9685Device pcaDevice, int ledIndex)
        {
            PCADevice = pcaDevice;
            LedIndex = ledIndex;
        }
    }
}
