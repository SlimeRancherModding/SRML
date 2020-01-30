using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Data.Partial;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRML.SR.SaveSystem.Data.World
{
    public class PartialDecorizerSettingsData : VersionedPartialData<DecorizerSettingsV01>
    {
        public override int LatestVersion => throw new NotImplementedException();

        public override void Pull(DecorizerSettingsV01 data)
        {
            throw new NotImplementedException();
        }

        public override void Push(DecorizerSettingsV01 data)
        {
            throw new NotImplementedException();
        }

        public override void ReadData(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public override void WriteData(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
