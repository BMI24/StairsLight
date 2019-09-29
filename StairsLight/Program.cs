using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unosquare.RaspberryIO.Gpio;

namespace StairsLight
{
    class Program
    {
        static List<LedStripe> Stripes = new List<LedStripe>();
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var testDevice = PCA9685Manager.GetDevice(0x43);

            for (int i = 1; i < 16; i+=3)
            {
                Stripes.Add(new LedStripe(testDevice.GetChannelController(i), testDevice.GetChannelController(i + 1)
                    , testDevice.GetChannelController(i + 2), new ConstantColorProvider(Color.Black)));
            }

            MainAction();

            Console.WriteLine("Closing demo..");
            Console.ReadLine();
        }

        static void MainAction()
        {
            while (true)
            {
                foreach (var stripe in Stripes)
                {
                    string colorInput = Console.ReadLine();
                    if (colorInput == "exit")
                        return;
                    var colorInputSplit = colorInput.Split(',');
                    string color1Name = colorInputSplit[0];
                    string color2Name = colorInputSplit[1];
                    float brightness = Convert.ToSingle(colorInputSplit[2]);
                    stripe.ColorProvider = new GradientColorProvider(Color.GetColor(color1Name) * brightness, Color.GetColor(color2Name) * brightness);
                }
            }
        }
    }
}
