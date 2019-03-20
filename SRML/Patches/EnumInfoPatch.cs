using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using UnityEngine;

namespace SRML.Patches
{
    [HarmonyPatch]
    internal static class EnumInfoPatch
    {
        private static FieldInfo values;
        private static FieldInfo names;
        static EnumInfoPatch()
        {
            var t = AccessTools.TypeByName("System.MonoEnumInfo");
            names = AccessTools.Field(t, "names");
            values = AccessTools.Field(t,"values");
        }
        static MethodBase TargetMethod(HarmonyInstance instance)
        {
            return AccessTools.Method(Type.GetType("System.MonoEnumInfo"),"GetInfo");

        }
        static object FixMono(Type enumType, object mono)
        {
            EnumPatcher.EnumPatch patch;
            if (EnumPatcher.TryGetRawPatch(enumType, out patch))
            {

                var oldValues = (int[])values.GetValue(mono);
                var oldNames = (string[])names.GetValue(mono);
                patch.GetArrays(out string[] toBePatchedNames,out int[] toBePatchedValues);
                Array.Resize(ref toBePatchedNames, toBePatchedNames.Length + oldNames.Length);
                Array.Resize(ref toBePatchedValues, toBePatchedValues.Length + oldValues.Length);
                Array.Copy(oldNames, 0, toBePatchedNames, toBePatchedNames.Length - oldNames.Length, oldNames.Length);
                Array.Copy(oldValues, 0, toBePatchedValues, toBePatchedValues.Length - oldValues.Length, oldValues.Length);
                names.SetValue(mono,toBePatchedNames);
                values.SetValue(mono,toBePatchedValues);
            }
            return mono;
        }
        
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            int curindex = -1;
            using (var enumerator = instructions.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var v = enumerator.Current;
                    curindex++;
                    if (v.operand is MethodInfo me&&me.Name=="get_enum_info")
                    {
                        yield return v;
                        enumerator.MoveNext();
                        enumerator.MoveNext();
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        var operand = Type.GetType("System.MonoEnumInfo");
                        yield return new CodeInstruction(OpCodes.Ldobj, operand);
                        yield return new CodeInstruction(OpCodes.Box, operand);
                        yield return new CodeInstruction(OpCodes.Call, typeof(EnumInfoPatch).GetMethod("FixMono", BindingFlags.Static | BindingFlags.NonPublic));
                        yield return new CodeInstruction(OpCodes.Unbox_Any, operand);
                        yield return new CodeInstruction(OpCodes.Stobj, operand);
                        yield return new CodeInstruction(OpCodes.Ldnull);
                        yield return new CodeInstruction(OpCodes.Stloc_0);
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