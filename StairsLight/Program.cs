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
        static Random RandomProvider = new Random();
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

        static Color GenerateRandomColor()
        {
            var random1 = RandomProvider.Next(256);
            Console.WriteLine(random1);
            var random2 = RandomProvider.Next(256);
            Console.WriteLine(random2);
            var random3 = RandomProvider.Next(256);
            Console.WriteLine(random3);
            return new Color(random1, random2, random3);
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
                    var color1 = (color1Name == "random" ? GenerateRandomColor() : Color.GetColor(color1Name)) * brightness;
                    var color2 = (color2Name == "random" ? GenerateRandomColor() : Color.GetColor(color2Name)) * brightness;
                    stripe.ColorProvider = new GradientColorProvider(color1, color2);
                }
            }
        }
    }
}
