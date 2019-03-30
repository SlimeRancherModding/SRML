using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using SRML.SR.UI.Utils;
using SRML.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace SRML.SR.UI
{
    internal static class ModMenuUIHandler
    {
        private static GameObject mainMenuUIPrefab;
        internal static void OnMainMenuLoaded(MainMenuUI ui)
        {
            MainMenuUtils.AddMainMenuButton(ui, "Mods", () =>
            {
                var g = MainMenuUtils.DisplayBlankPanel<BaseUI>(ui, "Mods");
                GameObject.Instantiate(mainMenuUIPrefab)
                .transform.SetParent(g.transform.GetChild(0),false);
                
            }).transform.SetSiblingIndex(5);

            if (mainMenuUIPrefab) return;

            var bundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(typeof(ModMenuUIHandler), "srml"));
            var h = bundle.LoadAsset<GameObject>("ModPanel");
            h.AddComponent<ModMenuUI>().infoButtonPrefab = bundle.LoadAsset<GameObject>("ModInfoButton");
            FixStyles(h);
            h.GetComponent<Image>().color = Color.green;
            mainMenuUIPrefab = h;
        }

        internal static void FixStyles(GameObject h)
        {
            foreach (var v in h.GetComponentsInChildren<Component>())
            {
                if(v.GetComponent<Scrollbar>())
                {
                    v.gameObject.AddComponent<ScrollbarStyler>().styleName =
                "Default";

                }
                else if (v.GetComponent<Text>())
                {
                    v.gameObject.AddComponent<TextStyler>().styleName = "Default";
                }
                else if(v.GetComponent<Image>()&&!v.GetComponent<Button>())
                {
                    v.gameObject.AddComponent<PanelStyler>().styleName = "Default";
                }


            }
            
        }
    }
}
