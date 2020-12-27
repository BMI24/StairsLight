using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StairsLight.Networking
{
    public enum OperationModeIdentifier : byte
    {
        Invalid = 0,
        Individual = 1,
        Cascade = 2
    }
}
