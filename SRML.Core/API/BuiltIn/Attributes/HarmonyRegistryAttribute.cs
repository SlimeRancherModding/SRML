using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SRML.Core.API.BuiltIn.Attributes
{
    public class HarmonyRegistryAttribute : RegistryAttribute
    {
        public Type processorType;

        public Type registrationType;
        public string registrationMethod;
        public Type[] registrationParameters;
        public Type[] registrationGenerics;

        public bool prefix;

        internal static Dictionary<MethodBase, List<(IRegistryNew, Type)>> registerOnBase = new Dictionary<MethodBase, List<(IRegistryNew, Type)>>();

        internal static void Patch(object __instance, MethodBase __originalMethod)
        {
            foreach (var registryRegistrations in registerOnBase[__originalMethod])
                registryRegistrations.Item1.RegisterAllOfType(registryRegistrations.Item2, __instance);
        }

        public override void Register(IRegistryNew toRegister)
        {
            MethodBase method = AccessTools.Method(registrationType, registrationMethod, registrationParameters, registrationGenerics);
            HarmonyMethod harmony = new HarmonyMethod(typeof(HarmonyRegistryAttribute), "Patch");

            Main.HarmonyInstance.Patch(method, prefix ? harmony : null, prefix ? null : harmony);

            if (!registerOnBase.ContainsKey(method))
                registerOnBase[method] = new List<(IRegistryNew, Type)>();

            registerOnBase[method].Add((toRegister, processorType));
        }

        public HarmonyRegistryAttribute(Type processorType, Type registrationType, string registrationMethod, bool prefix = true, Type[] registrationParameters = null,
            Type[] registrationGenerics = null)
        {
            this.processorType = processorType;
            this.registrationType = registrationType;
            this.registrationMethod = registrationMethod;
            this.prefix = prefix;
            this.registrationParameters = registrationParameters;
            this.registrationGenerics = registrationGenerics;
        }
    }
}
