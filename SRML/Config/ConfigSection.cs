using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IniParser;
using IniParser.Model;
using UnityEngine;

namespace SRML.Config
{
    public class ConfigSection
    {
        
        readonly Dictionary<string, ConfigElement> elementDict = new Dictionary<string, ConfigElement>();
        public void AddElement(ConfigElement element)
        {
            elementDict[element.Options.Name] = element;
        }

        public ConfigElement this[string index]
        {
            get
            {
                return elementDict[index];
            }
            set
            {
                elementDict[index] = value;
            }
        }
        public void LoadIniData(KeyDataCollection keys)
        {
            foreach(var data in keys)
            {
                if(elementDict.TryGetValue(data.KeyName, out var element))
                {
                    element.SetValue(element.Options.Parser.ParseObject(data.Value));
                }
                else
                {
                    Debug.LogWarning($"Unknown key {data.KeyName} in section! Ignoring...");
                }
            }
        }

        public void WriteIniData(KeyDataCollection keys)
        {
            foreach(var element in elementDict)
            {
                var keyData = new KeyData(element.Key);
                keyData.Value = element.Value.Options.Parser.EncodeObject(element.Value.GetValue<object>());
                keyData.Comments = element.Value.Options.Comment != null ? new List<string> { element.Value.Options.Comment } : new List<string>();
                if (!keys.AddKey(keyData)) Debug.LogWarning($"Key {element.Key} already present in section!");
            }
        }

        static ConfigSection()
        {
            IniData file = new IniData();
            var section = new SectionData("test");

            var test = new ConfigSection();
            test.AddElement(new ConfigElement<int>("testelement"));
            var element = new ConfigElement<float>("testfloat");
            element.Options.Comment = "Example Comment";
            test.AddElement(element);
            test.WriteIniData(section.Keys);
            file.Sections.Add(section);
            var parser = new FileIniDataParser();
            parser.WriteFile(Path.Combine(Application.persistentDataPath, "test.ini"), file);

        }
    }
}
