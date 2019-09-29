using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StairsLight
{
    class GradientColorProvider : IColorProvider
    {
        const float Step = 0.0025f;

        float CurrentStep = 0f;
        int Direction = 1;

        Color Color1, Color2;

        public GradientColorProvider(Color color1, Color color2)
        {
            Color1 = color1;
            Color2 = color2;
        }

        public Color NextTickColor
        {
            get
            {
                CurrentStep += Step * Direction;
                if (CurrentStep > 1 || CurrentStep < 0)
                {
                    Direction *= -1;
                    CurrentStep += Step * Direction;
                }
                return Color.Lerp(Color1, Color2, CurrentStep);
            }
        }
    }
}
