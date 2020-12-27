using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StairsLight.Networking
{
    public enum Protocol : byte
    {
        KeepAlive = 1,
        GetOperationMode = 2,
        SetOperationMode = 3,
        OperationModeSpecific = 4,
        GetBrightness = 5,
        SetBrightness = 6
    }
}
