using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.SR
{
    public static class SRCallbacks
    {
        public delegate void OnSaveGameLoadedDelegate(SceneContext t);

        public static event OnSaveGameLoadedDelegate OnSaveGameLoaded;
        public static event OnSaveGameLoadedDelegate PreSaveGameLoad;


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
