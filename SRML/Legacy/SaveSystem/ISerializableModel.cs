using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRML.SR.SaveSystem
{
    public interface ISerializableModel
    {
        void WriteData(BinaryWriter writer);
        void LoadData(BinaryReader reader);
    }
}
