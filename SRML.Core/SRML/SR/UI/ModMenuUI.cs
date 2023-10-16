using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace SRML.SR.UI
{
    internal class ModMenuUI : BaseUI
    {
        public GameObject infoButtonPrefab;
        private GameObject modScrollbarContent;
        private SRToggleGroup modScrollbarGroup;

        private Text modNamePanelText;
        private Text authorNameText;
        private Text descriptionText;
        private Text versionText;
        private Text dependenciesText;

        public override void Awake()
        {
            modScrollbarContent = transform.Find("ModScroll/Viewport/Content").gameObject;
            modScrollbarGroup = modScrollbarContent.GetComponent<SRToggleGroup>();
            transform.Find("ModsFolderButton").GetComponent<Button>().onClick.AddListener(() => Process.Start(Path.GetFullPath(FileSystem.ModPath)));
            modNamePanelText = transform.Find("ModNamePanel/ModNameText").GetComponent<Text>();
            var modinfo = transform.Find("ModInfoScroll/Viewport/Content");
            authorNameText = modinfo.Find("AuthorText").GetComponent<Text>();
            versionText = modinfo.Find("VersionText").GetComponent<Text>();
            dependenciesText = modinfo.Find("DependenciesText").GetComponent<Text>();
            descriptionText = modinfo.Find("DescriptionText").GetComponent<Text>();
        }

        public void Start()
        {
            foreach (var mod in SRModLoader.GetMods())
                AddModInfo(mod.ModInfo);
        }

        public void AddModInfo(SRModInfo info)
        {
            var newobj = Instantiate(infoButtonPrefab);
            ModButton button = new ModButton(info);
            newobj.GetComponent<SRToggle>().onValueChanged.AddListener((x) =>
            {
                if (x) OnModSelect(button);
            });
            newobj.transform.GetChild(0).GetComponent<Text>().text = info.Name;
            newobj.transform.GetChild(1).GetComponent<Text>().text = $"Version: {info.Version}";
            newobj.GetComponent<SRToggle>().group = modScrollbarGroup;
            newobj.transform.SetParent(modScrollbarContent.transform, false);
        }

        public void OnModSelect(ModButton button)
        {
            modNamePanelText.text = button.name;
            authorNameText.text = button.author;
            versionText.text = button.version;
            dependenciesText.text = button.dependencies;
            descriptionText.text = button.description;
        }

        internal class ModButton
        {
            public string name;
            public string description;
            public string dependencies;
            public string author;
            public string version;

            public ModButton(SRModInfo info)
            {
                name = info.Name;
                description = $"Description: {(info.Description == null || info.Description == string.Empty ? "No info provided" : info.Description)}";
                version = $"Version: {info.Version}";
                author = $"Author: {info.Author}";
                dependencies = $"Dependencies: ";
                if (info.Dependencies.Count > 0)
                {
                    int i = 0;
                    foreach (KeyValuePair<string, SRModInfo.ModVersion> dep in info.Dependencies)
                    {
                        dependencies += $"{dep.Key} {dep.Value}";
                        if ((i + 1) != info.Dependencies.Count) dependencies += ", ";
                        i++;
                    }
                }
                else dependencies += "None";
            }
        }
    }
}
