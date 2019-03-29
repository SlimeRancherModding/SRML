using SRML.SR.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.SR
{
    public static class SRCallbacks
    {

        static SRCallbacks()
        {
            OnMainMenuLoaded += ModMenuUIHandler.OnMainMenuLoaded;
        }

        public delegate void OnSaveGameLoadedDelegate(SceneContext t);

        public delegate void OnMainMenuLoadedDelegate(MainMenuUI mainMenu);

        public static event OnSaveGameLoadedDelegate OnSaveGameLoaded;
        public static event OnSaveGameLoadedDelegate PreSaveGameLoad;
        public static event OnMainMenuLoadedDelegate OnMainMenuLoaded;



        internal static void OnMainMenuLoad(MainMenuUI mainmenu)
        {
            OnMainMenuLoaded?.Invoke(mainmenu);
        }

        internal static void OnSceneLoaded(SceneContext t)
        {
            if (Levels.isMainMenu()) return;
            OnSaveGameLoaded?.Invoke(t);

        }

        internal static void PreSceneLoad(SceneContext t)
        {
            if (Levels.isMainMenu()) return;
            PreSaveGameLoad?.Invoke(t);
        }

    }
}
