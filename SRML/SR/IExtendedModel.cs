using System.IO;

namespace SRML.SR
{
    public interface IExtendedModel
    {
        void WriteData(BinaryWriter writer);
        void LoadData(BinaryReader reader);
    }
}
