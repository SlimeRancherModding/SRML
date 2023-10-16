using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

namespace SRML.Utils
{
    
    public class HarmonyOverrideAttribute : Attribute
    {

    }

    internal static class HarmonyOverrideHandler
    {
        public static Dictionary<MethodBase, List<MethodBase>> methodsToPatch = new Dictionary<MethodBase, List<MethodBase>>();

        public static bool CheckMethod()
        {
            // eventually put stacktrace checking here
            return true;
        }
        public static IEnumerable<CodeInstruction> Transpiler(MethodBase __originalMethod,IEnumerable<CodeInstruction> instr, ILGenerator gen)
        {
            if (methodsToPatch.TryGetValue(__originalMethod, out var methods)) {
                Label lab1 = gen.DefineLabel();
                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HarmonyOverrideHandler), "CheckMethod"));
                yield return new CodeInstruction(OpCodes.Brfalse, lab1);

                Label labLoop = default;

                foreach(var v in methods)
                {

                    var code = new CodeInstruction(OpCodes.Ldarg_0);
                    if (labLoop != default) code.labels.Add(labLoop);
                    labLoop = gen.DefineLabel();
                    yield return code;
                    yield return new CodeInstruction(OpCodes.Ldind_Ref);
                    yield return new CodeInstruction(OpCodes.Isinst, v.ReflectedType);
                    yield return new CodeInstruction(OpCodes.Brfalse, v==methods.Last()?lab1:labLoop);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    int i = 1;
                    foreach(var p in v.GetParameters())
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg, i++);
                    }
                    yield return new CodeInstruction(v.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, v);
                    yield return new CodeInstruction(OpCodes.Ret);
                }

                var list = instr.ToList();
                list.First().labels.Add(lab1);
                foreach (var v in list)
                {
                    yield return v;
                }
            }
            else
            {
                foreach(var v in instr)
                {
                    yield return v;
                }
            }

        }

        public static void PatchAll()
        {
            foreach(var v in methodsToPatch)
            {
                HarmonyPatcher.Instance.Patch(v.Key, transpiler: new HarmonyMethod(AccessTools.Method(typeof(HarmonyOverrideHandler), "Transpiler")));
            }
        }
        
        public static void LoadOverrides(Module module)
        {
            foreach(var v in module.GetTypes().SelectMany(x => x.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)))
            {
                if(v.GetCustomAttributes(false).Any(x=>x is HarmonyOverrideAttribute))
                {
                    var curType = v.ReflectedType;
                    MethodBase info = null;
                    while (true)
                    {
                        curType = curType.BaseType;
                        if (curType == null) break;
                        info = AccessTools.Method(curType, v.Name, v.GetParameters().Select(x => x.ParameterType).ToArray(), v.IsGenericMethod?v.GetGenericArguments():null);
                        if (info != null) break;
                    }

                    if (!methodsToPatch.TryGetValue(info,out var a))
                    {
                        a = new List<MethodBase>();
                        methodsToPatch[info] = a;
                    }
                    a.Add(v);
                    
                }
            }

        }
    }
}
