using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.SR
{
    public static class IdentifiableRegistry
    {
        internal static Dictionary<Identifiable.Id,SRMod> moddedIdentifiables = new Dictionary<Identifiable.Id, SRMod>();
        
        public static Identifiable.Id CreateIdentifiableId(object value, string name, bool shouldCategorize = true)
        {
            if (SRModLoader.CurrentLoadingStep > SRModLoader.LoadingStep.PRELOAD)
                throw new Exception("Can't register identifiables outside of the PreLoad step");
            var id = (Identifiable.Id) value;
            if (moddedIdentifiables.ContainsKey(id))
                throw new Exception(
                    $"Identifiable {value} is already registered to {moddedIdentifiables[id].ModInfo.Id}");
            var sr = SRMod.GetCurrentMod();
            if (sr != null) moddedIdentifiables[id] = sr;
            EnumPatcher.AddEnumValue(typeof(Identifiable.Id), id, name);
            
            if (shouldCategorize) CategorizeId(id);
            return id;
        }

        public static bool IsModdedIdentifiable(Identifiable.Id id)
        {
            return moddedIdentifiables.ContainsKey(id);
        }

        public static void CategorizeId(Identifiable.Id id)
        {
            string name = Enum.GetName(typeof(Identifiable.Id), id);
            if (name.EndsWith("_VEGGIE"))
            {
                Identifiable.VEGGIE_CLASS.Add(id);
                Identifiable.FOOD_CLASS.Add(id);
                Identifiable.NON_SLIMES_CLASS.Add(id);
            }
            else if (name.EndsWith("_FRUIT"))
            {
                Identifiable.FRUIT_CLASS.Add(id);
                Identifiable.FOOD_CLASS.Add(id);
                Identifiable.NON_SLIMES_CLASS.Add(id);
            }
            else if (name.EndsWith("_TOFU"))
            {
                Identifiable.TOFU_CLASS.Add(id);
                Identifiable.FOOD_CLASS.Add(id);
                Identifiable.NON_SLIMES_CLASS.Add(id);
            }
            else if (name.EndsWith("_SLIME"))
            {
                Identifiable.SLIME_CLASS.Add(id);
            }
            else if (name.EndsWith("_LARGO"))
            {
                Identifiable.LARGO_CLASS.Add(id);
            }
            else if (name.EndsWith("_GORDO"))
            {
                Identifiable.GORDO_CLASS.Add(id);
            }
            else if (name.EndsWith("_PLORT"))
            {
                Identifiable.PLORT_CLASS.Add(id);
                Identifiable.NON_SLIMES_CLASS.Add(id);
            }
            else if (name.EndsWith("HEN") || name.EndsWith("ROOSTER"))
            {
                Identifiable.MEAT_CLASS.Add(id);
                Identifiable.FOOD_CLASS.Add(id);
                Identifiable.NON_SLIMES_CLASS.Add(id);
            }
            else if (id == Identifiable.Id.CHICK || name.EndsWith("_CHICK"))
            {
                Identifiable.NON_SLIMES_CLASS.Add(id);
                Identifiable.CHICK_CLASS.Add(id);
            }
            else if (name.EndsWith("_LIQUID"))
            {
                Identifiable.LIQUID_CLASS.Add(id);
            }
            else if (name.EndsWith("_CRAFT"))
            {
                Identifiable.CRAFT_CLASS.Add(id);
                Identifiable.NON_SLIMES_CLASS.Add(id);
            }
            else if (name.EndsWith("_FASHION"))
            {
                Identifiable.FASHION_CLASS.Add(id);
            }
            else if (name.EndsWith("_ECHO"))
            {
                Identifiable.ECHO_CLASS.Add(id);
            }
            else if (name.StartsWith("ECHO_NOTE_"))
            {
                Identifiable.ECHO_NOTE_CLASS.Add(id);
            }
            else if (name.EndsWith("_ORNAMENT"))
            {
                Identifiable.ORNAMENT_CLASS.Add(id);
            }
            else if (name.EndsWith("_TOY") || id == Identifiable.Id.KOOKADOBA_BALL)
            {
                Identifiable.TOY_CLASS.Add(id);
            }
            if (name.Contains("TANGLE"))
            {
                Identifiable.ALLERGY_FREE_CLASS.Add(id);
            }
            Identifiable.EATERS_CLASS.UnionWith(Identifiable.SLIME_CLASS);
            Identifiable.EATERS_CLASS.UnionWith(Identifiable.LARGO_CLASS);
        }
    }
}
