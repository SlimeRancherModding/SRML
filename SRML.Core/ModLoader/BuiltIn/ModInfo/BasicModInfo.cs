using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Semver;
using SRML.Core.ModLoader.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRML.Core.ModLoader.BuiltIn.ModInfo
{
    public class BasicModInfo : IModInfo
    {
        public string Id { get => parsedInfo.id; }
        public string Name { get => parsedInfo.name; }
        public string Description { get => parsedInfo.description; }
        public string Author { get => parsedInfo.author; }
        public SemVersion Version { get => parsedInfo.version; }
        private ProtoMod parsedInfo;

        public void Parse(string json) => parsedInfo = JsonConvert.DeserializeObject<ProtoMod>(json, new ProtoModConverter());

        public class ProtoMod
        {
            public string id;
            public string name;
            public string author;
            public string description;
            public string path;

            public SemVersion version;
            public DependencyMetadata dependencies;
        }

        public class ProtoModConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType) => objectType == typeof(ProtoMod);

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                ProtoMod pm = new ProtoMod();
                JObject token = (JObject)JToken.ReadFrom(reader);

                try
                {
                    pm.id = token["id"].ToObject<string>();
                    pm.version = SemVersion.Parse(token["version"].ToObject<string>(), SemVersionStyles.OptionalPatch);
                    pm.name = token["name"].ToObject<string>();

                    pm.author = token["author"]?.ToObject<string>() ?? string.Empty;
                    pm.description = token["description"]?.ToObject<string>() ?? string.Empty;
                }
                catch (Exception e)
                {
                    if (string.IsNullOrEmpty(pm.id))
                        throw new Exception($"Error parsing unknown basic mod information! {e}");
                    else
                        throw new Exception($"Error parsing basic mod information for {pm.id}! {e}");
                }

                string[] loadBefore = token["load_before"]?.ToObject<string[]>() ?? new string[0];
                string[] loadAfter = token["load_after"]?.ToObject<string[]>() ?? new string[0];
                Dictionary<string, SemVersion> dependencies = new Dictionary<string, SemVersion>();

                try
                {
                    if (token.ContainsKey("dependencies"))
                    {
                        if (token["dependencies"].Type == JTokenType.Array)
                        {
                            dependencies = token["dependencies"].ToObject<string[]>().ToDictionary(x => x.Split(' ')[0],
                                y => SemVersion.Parse(y.Split(' ')[1], SemVersionStyles.OptionalPatch));
                        }
                        else if (token["dependencies"].Type == JTokenType.Object)
                        {
                            dependencies = ((JObject)token["dependencies"]).Properties().ToDictionary(x => x.Name,
                                y => SemVersion.Parse(y.Value.ToObject<string>(), SemVersionStyles.OptionalPatch));
                        }
                        else
                        {
                            throw new InvalidOperationException($"Malformed dependencies in {pm.id}");
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new Exception($"Error parsing dependencies in {pm.id}! {e}");
                }

                if (string.IsNullOrEmpty(pm.id) || Main.loader.FORBIDDEN_IDS.Contains(pm.id) || pm.id.Contains(" ") || pm.id.Contains("."))
                    throw new Exception($"Invalid or missing mod id: {pm.id}");
                pm.id = pm.id.ToLower();

                return pm;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
            }
        }
    }
}
