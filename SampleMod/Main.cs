using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Text;
using SRML;
using Harmony;
using SRML.Utils;
using SRML.SR;

namespace SampleMod
{
    public class Main : IModEntryPoint
    {
        // Called before GameContext.Awake
        // this is where you want to register stuff (like custom enum values or identifiable id's)
        // and patch anything you want to patch with harmony
        public void PreLoad()
        {
            Debug.Log("We did it!");
            HarmonyPatcher.GetInstance().PatchAll(Assembly.GetExecutingAssembly());


            // this code registers a callback that's run every time a saved game is loaded
            // in this case it spawns a mosaic boom largo
            SRCallbacks.OnSaveGameLoaded += (scenecontext) =>
            {
                
                var playerModel = SceneContext.Instance.GameModel.GetPlayerModel();
                SRBehaviour.InstantiateActor(
                    GameContext.Instance.LookupDirector.GetPrefab(Identifiable.Id.MOSAIC_BOOM_LARGO), playerModel.position,
                    playerModel.rotation);
            };
        }


        // Called after GameContext.Start
        // stuff like gamecontext.lookupdirector are available in this step, generally for when you want to access
        // ingame prefabs and the such
        public void PostLoad()
        {
            Debug.Log("We did it! Again! ");
        }
    }

}
