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
            Stripes.Add(new LedStripe(13, 26, 19));
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
                    var colorNameSplit = colorName.Split(',');
                    stripe.SetColor(new Color(Convert.ToByte(colorNameSplit[0])
                        , Convert.ToByte(colorNameSplit[1])
                        , Convert.ToByte(colorNameSplit[2])));
                }
            }
        }
    }
}
