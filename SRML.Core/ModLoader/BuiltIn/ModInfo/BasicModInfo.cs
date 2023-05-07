using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRML.Core.ModLoader.BuiltIn.ModInfo
{
    public class BasicModInfo : IModInfo
    {
        public string Id { get => parsedInfo.id; }
        public string Name { get => parsedInfo.name; }
        public string Description { get => parsedInfo.description; }
        public string Author { get => parsedInfo.author; }

        private ProtoMod parsedInfo;

        public void Parse(string json)
        {
            throw new NotImplementedException();
        }

        public class ProtoMod
        {
            public string id;
            public string name;
            public string author;
            public string version;
            public string description;
            public string path;
            public string[] load_after;
            public string[] load_before;

            [JsonExtensionData]
            public IDictionary<string, JToken> dependencies;
        }

        public class ProtoModConverter : JsonConverter
        {
            private readonly string path;

            public override bool CanConvert(Type objectType) => objectType == typeof(ProtoMod);

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                /*ProtoMod pm = new ProtoMod();
                JObject token = (JObject)JToken.ReadFrom(reader);

                try
                {
                    pm.id = token["id"].ToObject<string>();
                    pm.version = token["version"].ToObject<string>();
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

                try
                {
                    pm.load_after = token["load_after"]?.ToObject<string[]>() ?? new string[0];
                    pm.load_before = token["load_before"]?.ToObject<string[]>() ?? new string[0];
                }
                catch (Exception e)
                {
                    throw new Exception($"Error parsing mod loading order for {pm.id}! {e}");
                }

                try
                {
                    if (token.ContainsKey("dependencies"))
                    {
                        if (token["dependencies"].Type == JTokenType.Array)
                        {
                            pm.parsedDependencies = token["dependencies"].ToObject<string[]>().Select(x =>
                                new DependencyChecker.Dependency(x.Split(' ')[0], x.Split(' ')[1])).ToArray();
                        }
                        else if (token["dependencies"].Type == JTokenType.Object)
                        {
                            pm.parsedDependencies = ((JObject)token["dependencies"]).Properties().Select(x =>
                                new DependencyChecker.Dependency(x.Name, x.Value.ToObject<string>())).ToArray();
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

                if (pm.id == null) throw new Exception($"{path} is missing an id field!");
                pm.id = pm.id.ToLower();
                if (pm.id.IsNullOrEmpty() || forbiddenIds.Contains(pm.id) || pm.id.Contains(" ") || pm.id.Contains("."))
                    throw new Exception($"Invalid mod id: {pm.id}");

                pm.path = Path.GetDirectoryName(path);
                pm.entryFile = path;

                return pm;*/
                throw new NotImplementedException();
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
            }

            public ProtoModConverter(string path) : base() => this.path = path;
        }
    }
}
