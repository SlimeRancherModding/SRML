using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Data.Partial;
using SRML.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRML.SR.SaveSystem.Data.Macro
{
    public class PartialWorldData : VersionedPartialData<WorldV22>
    {
        static SerializerPair<string> stringSerializer = new SerializerPair<string>((x, y) => x.Write(y), (x) => x.ReadString());


        public override int LatestVersion => 0;

        public PartialCollection<string> activeGingerPatches = new PartialCollection<string>(ModdedStringRegistry.IsValidModdedString,stringSerializer, ModdedStringRegistry.IsValidModdedString);
        public AmbianceDirector.Weather weather;
        //public PartialDictionary<string, EchoNoteGordoV01> echoNoteGordos = new PartialDictionary<string, EchoNoteGordoV01>(x=>ModdedStringRegistry.IsValidModdedString(x.Key))
        public override void Pull(WorldV22 data)
        {
           
        }

        public override void Push(WorldV22 data)
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
