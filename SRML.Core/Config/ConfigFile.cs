using IniParser.Model;
using SRML.Config.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.Config
{
    public class ConfigFile
    {
        public string FileName { get; internal set; }

        public string DefaultSection { get; internal set; }

        readonly Dictionary<string, ConfigSection> sections = new Dictionary<string, ConfigSection>();

        public IEnumerable<ConfigSection> Sections => sections.Values;

        public void AddSection(ConfigSection section)
        {
            sections[section.Name] = section;
        }

        public ConfigSection AddSection(string name)
        {
            var section = new ConfigSection(name);
            AddSection(section);
            return section;
        }
        public ConfigSection this[string index] => sections[index];

        public void LoadIniData(IniData data)
        {
            
            foreach(var section in data.Sections)
            {
                if(sections.TryGetValue(section.SectionName,out var confsect))
                {
                    confsect.LoadIniData(section.Keys);
                }
                else
                {
                    Debug.LogWarning($"Unknown section {section.SectionName} in data! Ignoring...");
                }
            }
        }

        public void WriteIniData(IniData data)
        {
            foreach(var sectiondata in sections)
            {
                var section = new SectionData(sectiondata.Key);
                sectiondata.Value.WriteIniData(section.Keys);
                if (sectiondata.Value.Comment != null) section.Comments.Add(sectiondata.Value.Comment);
                data.Sections.Add(section);
            }
        }

        public static ConfigFile GenerateConfig(Type type)
        {
            var file = new ConfigFile();
            var attribute = type.GetCustomAttributes(typeof(ConfigFileAttribute), false).FirstOrDefault() as ConfigFileAttribute;
            if (attribute == null) return null;
            file.FileName = attribute.FileName;
            var defaultSection = file.AddSection(attribute.DefaultSection);
            foreach (var field in type.GetFields().Where(x => x.IsStatic))
            {
                defaultSection.AddElement(new FieldBackedConfigElement(field));
            }
            foreach(var v in type.GetNestedTypes())
            {
                var sectionAttribute = v.GetCustomAttributes(typeof(ConfigSectionAttribute), false).FirstOrDefault() as ConfigSectionAttribute;
                if (sectionAttribute == null) continue;
                var newSection = file.AddSection(sectionAttribute.SectionName??v.Name);
                foreach(var field in v.GetFields().Where(x => x.IsStatic))
                {
                    newSection.AddElement(new FieldBackedConfigElement(field));
                }
            }
            return file;
        }

        public bool TryLoadFromFile(bool writeInDefault = true)
        {
            var configpath = FileSystem.GetMyConfigPath();
            var parser = new IniParser.FileIniDataParser();
            var filePath = Path.ChangeExtension(Path.Combine(configpath, FileName), "ini");
            IniData data;
            try
            {
                data = parser.ReadFile(filePath);
            }
            catch
            {
                SaveToFile();
                return false;
            }
            LoadIniData(data);
            SaveToFile();
            return true;
            
        }

        public void SaveToFile()
        {
            var parser = new IniParser.FileIniDataParser();
            var filePath = Path.ChangeExtension(Path.Combine(FileSystem.GetMyConfigPath(), FileName), "ini");
            var data = new IniData();
            WriteIniData(data);
            parser.WriteFile(filePath, data);
        }
    }
}
