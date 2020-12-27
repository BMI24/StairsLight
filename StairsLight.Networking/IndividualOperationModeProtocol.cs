using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StairsLight.Networking
{
    public enum IndividualOperationModeProtocol : byte
    {
        GetStripeCount = 1,
        GetColor = 2,
        SetColor = 3
    }
}
