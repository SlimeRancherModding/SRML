using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Data.Partial;
using VanillaLandPlotData = MonomiPark.SlimeRancher.Persist.LandPlotV08;
namespace SRML.SR.SaveSystem.Data.LandPlot
{
    internal class PartialLandPlotData : VersionedPartialData<VanillaLandPlotData>
    {
        public override int LatestVersion => 0;

        public override void Pull(VanillaLandPlotData data)
        {

        }

        public override void Push(VanillaLandPlotData data)
        {
            throw new NotImplementedException();
        }

        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
        }
    }
}
