﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StairsLight.Networking
{
    public enum Protocol : byte
    {
        KeepAlive = 1,
        GetStripeCount = 2,
        GetColor = 3,
        SetColor = 4,
        GetBrightness = 5,
        SetBrightness = 6,
        GetOperationMode = 7,
        SetOperationMode = 8
    }
}
