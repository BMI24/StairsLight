using StairsLight.Networking;
using StairsLight.NetworkingHandlers;
using StairsLight.PCA9685;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
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

            OnOffButtonController.Initialize(26);

            var redController = PCA9685Manager.GetDevice(0x42);
            var blueController = PCA9685Manager.GetDevice(0x43);
            var greenController = PCA9685Manager.GetDevice(0x41);

            const int StairsStepCount = 15;
            for (int i = 0; i < StairsStepCount; i++)
            {
                //blue indices are inverted because of weird physical installation
                Stripes.Add(new LedStripe(redController.GetChannelController(i), greenController.GetChannelController(i)
                    , blueController.GetChannelController(StairsStepCount - i), Color.Black));
            }

            void stateChanged(object sender, bool e)
            {
                foreach (var stripe in Stripes)
                {
                    stripe.RefreshShownColor(recalculateColors: false);
                }
            }

            OnOffButtonController.Instance.StateChanged += stateChanged;


            ServerManager.Server = new OperationModeManager();
            Listener listener = new Listener(NetworkClient.ApplicationPort, IPAddress.Any);
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

        static void MainAction()
        {
            while (true)
            {
                string input = Console.ReadLine();
                if (input == "exit")
                    return;
            }
        }
    }
}
