using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRML.SR.SaveSystem
{
    interface ISerializable
    {
        void Write(BinaryWriter writer);
        void Read(BinaryReader reader);
    }
}
