using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var redController = PCA9685Manager.GetDevice(0x42);
            //var blueController = PCA9685Manager.GetDevice(0x42);
            //var greenController = PCA9685Manager.GetDevice(0x43);

            for (int i = 0; i < 3; i++)
            {
                Stripes.Add(new LedStripe(redController.GetChannelController(i), redController.GetChannelController(i)
                    , redController.GetChannelController(i), new ConstantColorProvider(Color.Black)));
            }


            Listener listener = new Listener(NetworkServer.ApplicationPort, IPAddress.Any);
            listener.StartListening();

            try
            {
                MainAction();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                throw;
            }

            Console.WriteLine("Closing demo..");
            Console.ReadLine();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(((Exception)e.ExceptionObject).StackTrace);
        }

        static Color GenerateRandomColor() => new Color((byte)RandomProvider.Next(256), (byte)RandomProvider.Next(256), (byte)RandomProvider.Next(256));

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
