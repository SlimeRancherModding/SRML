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
            MainMenuUtils.AddMainMenuButtonWithTranslation(ui, "ModsButton", "b.mods", () =>
            {
                var g = MainMenuUtils.DisplayBlankPanel<BaseUI>(ui, "Mods");
                GameObject.Instantiate(mainMenuUIPrefab).transform.SetParent(g.transform.GetChild(0), false);
                
            }).transform.SetSiblingIndex(5);

            if (mainMenuUIPrefab) return;

            var h = Main.uiBundle.LoadAsset<GameObject>("ModPanel");
            h.AddComponent<ModMenuUI>().infoButtonPrefab = Main.uiBundle.LoadAsset<GameObject>("ModInfoButton");
            mainMenuUIPrefab = h;
        }


    }
}
