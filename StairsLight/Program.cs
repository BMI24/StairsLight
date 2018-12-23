using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StairsLight
{
    class Program
    {
        static List<LedStripe> Stripes = new List<LedStripe>();
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Stripes.Add(new LedStripe(16, 21, 20));
            Stripes.Add(new LedStripe(13, 19, 26));
            Stripes.Add(new LedStripe(6, 12, 5));

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
                    string colorName = Console.ReadLine();
                    if (colorName == "exit")
                        return;
                    stripe.SetColor(Color.GetColor(colorName));
                }
            }
        }
    }
}
