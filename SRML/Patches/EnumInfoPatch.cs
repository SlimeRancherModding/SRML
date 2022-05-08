using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace SRML.Patches
{
    [HarmonyPatch]
    internal static class EnumInfoPatch
    {
        static EnumInfoPatch()
        {
        }
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(Type.GetType("System.Enum"), "GetCachedValuesAndNames");

        }
        static void FixEnum(object type, ref ulong[] oldValues, ref string[] oldNames)
        {
            var enumType = type as Type;
            if (EnumPatcher.TryGetRawPatch(enumType, out var patch))
            {
                var pairs = patch.GetPairs();

                List<ulong> newValues = new List<ulong>(oldValues);
                List<string> newNames = new List<string>(oldNames);

                foreach (var pair in pairs)
                {
                    newValues.Add(pair.Key);
                    newNames.Add(pair.Value);
                }

                oldValues = newValues.ToArray();
                oldNames = newNames.ToArray();

                Array.Sort(oldValues, oldNames, Comparer<ulong>.Default);
            }
        }
        
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            using (var enumerator = instructions.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var v = enumerator.Current;
                    if (v.operand is MethodInfo me&&me.Name=="Sort")
                    {
                        yield return v;
                        enumerator.MoveNext();
                        v = enumerator.Current;
                        var labels = v.labels;
                        v.labels = new List<Label>();
                        yield return new CodeInstruction(OpCodes.Ldarg_0) { labels = labels};
                        yield return new CodeInstruction(OpCodes.Ldloca, 1);
                        yield return new CodeInstruction(OpCodes.Ldloca, 2);
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EnumInfoPatch), "FixEnum"));
                        yield return v;
                    }
                    else
                    {
                        yield return v;
                    }
                }
            }
        }
    }
}