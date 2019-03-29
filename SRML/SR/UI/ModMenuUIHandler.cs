using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SRML.SR.UI.Utils;
using UnityEngine;

namespace SRML.SR.UI
{
    internal static class ModMenuUIHandler
    {

        internal static void OnMainMenuLoaded(MainMenuUI ui)
        {
            MainMenuUtils.AddMainMenuButton(ui, "Mods", () => MainMenuUtils.DisplayBlankPanel<BaseUI>(ui,"Mods")).transform.SetSiblingIndex(5);
        }
    }
}
