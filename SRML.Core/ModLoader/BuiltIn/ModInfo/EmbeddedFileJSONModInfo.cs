using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SRML.Core.ModLoader.BuiltIn.ModInfo
{
    public class EmbeddedFileJSONModInfo : FileJSONModInfo
    {
        private readonly Type entryType;

        public override string GetJSON(Assembly modAssembly)
        {
            string json = null;

            if (modAssembly.GetManifestResourceStream(entryType, "modinfo.json") is Stream s)
            {
                using (StreamReader reader = new StreamReader(s))
                    json = reader.ReadToEnd();
            }
            else if (modAssembly.GetManifestResourceNames().FirstOrDefault((x) => x.EndsWith("modinfo.json")) is string fileName)
            {
                using (StreamReader reader = new StreamReader(modAssembly.GetManifestResourceStream(fileName)))
                    json = reader.ReadToEnd();
            }
            else throw new MissingModInfoException("Could not find an embedded modinfo.json (is it not marked as embedded resource?)");

            return json;
        }

        public EmbeddedFileJSONModInfo(Type entryType) => this.entryType = entryType;
    }
}
