using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StairsLight
{
    class GradientColorProvider : IColorProvider
    {
        const float Step = 0.05f;

        float CurrentStep = 0f;

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
                CurrentStep += Step;
                if (CurrentStep > 1)
                    CurrentStep = 0;
                return Color.Lerp(Color1, Color2, CurrentStep);
            }
        }
    }
}
