using StairsLight.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StairsLight.NetworkingHandlers.OpModes
{
    interface IOpMode
    {
        bool Active { get; set; }
        void ProcessModeSpecificMessage(MessageInfo message);
    }
}
