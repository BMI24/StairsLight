using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StairsLight.Networking
{
    public class ParsingException : Exception
    {
        public ParsingException(Exception inner) : base("", inner)
        {
        }
    }
}
