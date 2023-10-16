using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.SaveSystem
{
    interface IVersionedSerializable : ISerializable
    {
        int LatestVersion { get; }
        int Version { get; }
    }
}
