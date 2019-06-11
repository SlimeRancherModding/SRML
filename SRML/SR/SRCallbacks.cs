using MonomiPark.SlimeRancher.DataModel;
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
        public delegate void OnActorSpawnDelegate(Identifiable.Id id, GameObject obj, ActorModel model);
            
        public static event OnSaveGameLoadedDelegate OnSaveGameLoaded;
        public static event OnSaveGameLoadedDelegate PreSaveGameLoad;
        public static event OnMainMenuLoadedDelegate OnMainMenuLoaded;
        public static event OnActorSpawnDelegate OnActorSpawn;


        internal static void OnMainMenuLoad(MainMenuUI mainmenu)
        {
            OnMainMenuLoaded?.Invoke(mainmenu);
        }

        internal static void OnSceneLoaded(SceneContext t)
        {
            if (Levels.isMainMenu()) return;
            OnSaveGameLoaded?.Invoke(t);
        }

        internal static void OnActorSpawnCallback(Identifiable.Id id,GameObject obj, ActorModel model)
        {
            OnActorSpawn?.Invoke(id, obj, model);
        }

        internal static void PreSceneLoad(SceneContext t)
        {
            if (Levels.isMainMenu()) return;
            PreSaveGameLoad?.Invoke(t);
        }

    }
}
