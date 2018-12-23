using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StairsLight
{
    class ConstantColorProvider : IColorProvider
    {
        public ConstantColorProvider(Color color)
        {
            NextTickColor = color;
        }

        public Color NextTickColor { get; }
    }
}
