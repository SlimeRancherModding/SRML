using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

namespace SRML.SR.UI
{
    internal class ModMenuUI : BaseUI
    {
        public GameObject infoButtonPrefab;
        private GameObject modScrollbarContent;
        private Text modNamePanelText;


        public override void Awake()
        {
            this.modScrollbarContent = transform.Find("ModScroll/Viewport/Content").gameObject;
            transform.Find("ModsFolderButton").GetComponent<Button>().onClick.AddListener(()=>Process.Start(Path.GetFullPath(FileSystem.ModPath)));
            modNamePanelText = transform.Find("ModNamePanel/ModNameText").GetComponent<Text>();
        }

        public void Start()
        {
            foreach (var mod in SRModLoader.GetMods())
            {
                AddModInfo(mod.ModInfo);
            }
        }

        public void AddModInfo(SRModInfo info)
        {
            var newobj = Instantiate(infoButtonPrefab);
            newobj.GetComponent<Button>().onClick.AddListener( () => OnModSelect(info));
            newobj.transform.GetChild(0).GetComponent<Text>().text = info.Name;
            newobj.transform.GetChild(1).GetComponent<Text>().text = $"Version: {info.Version}";
            newobj.transform.SetParent(modScrollbarContent.transform,false);
        }

        public void OnModSelect(SRModInfo info)
        {
            modNamePanelText.text = info.Name;
        }
    }
}
