using InControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static HarmonyLib.AccessTools;

namespace SRML.SR
{
    public static class BindingRegistry
    {
        internal static Dictionary<PlayerAction, SRMod> moddedActions = new Dictionary<PlayerAction, SRMod>();

        internal delegate void PlayerActionCreateDelegate(PlayerActionSet set);
        internal static List<PlayerActionCreateDelegate> precreators = new List<PlayerActionCreateDelegate>();

        internal static List<PlayerAction> ephemeralActions = new List<PlayerAction>();

        internal static void RegisterAction(PlayerAction action, SRMod mod)
        {
            moddedActions.Add(action, mod);
        }
        public static bool IsModdedAction(PlayerAction action)
        {
            return moddedActions.ContainsKey(action) || ephemeralActions.Contains(action);
        }

        public static void RegisterActions(Type type)
        {
            var mod = SRMod.GetCurrentMod();
            foreach (var field in type.GetFields().Where(x => x.IsStatic&&typeof(PlayerAction).IsAssignableFrom(x.FieldType)))
            {
                PlayerActionCreateDelegate del = (set) =>
                {
                    var action = new PlayerAction(field.Name, set);
                    RegisterAction(action, mod);
                    field.SetValue(null, action);
                };

                switch (SRModLoader.CurrentLoadingStep)
                {
                    case SRModLoader.LoadingStep.PRELOAD:
                        precreators.Add(del);
                        break;
                    default:
                        del(SRInput.Actions);
                        break;
                }
            }
        }
    }
}
