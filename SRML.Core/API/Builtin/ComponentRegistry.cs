using HarmonyLib;
using SRML.Core.ModLoader;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SRML.Core.API.Builtin
{
    public abstract class ComponentRegistry<T, C> : Registry<T> where C : Component
    {
        internal delegate void Execute(C component);
        internal static Execute PrefixRegisterDelegate;
        internal static Execute PostfixRegisterDelegate;

        protected List<T> registered = new List<T>();

        public abstract MethodInfo ComponentInitializeMethod { get; set; }
        public abstract bool Prefix { get; set; }

        public abstract void InitializeComponent(C component);

        public abstract void RegisterAllIntoComponent(C component);

        public sealed override void Initialize()
        {
            if (Prefix)
                PrefixRegisterDelegate += RegisterAllIntoComponent;
            else
                PostfixRegisterDelegate += RegisterAllIntoComponent;

            Main.HarmonyInstance.Patch(ComponentInitializeMethod, 
                Prefix ? new HarmonyMethod(PrefixRegisterDelegate.Method) : null, 
                Prefix ? null : new HarmonyMethod(PostfixRegisterDelegate.Method), null);
        }

        public sealed override void Register(T toRegister)
        {
            registered.Add(toRegister);

            IMod mod = CoreLoader.Main.GetExecutingMod();
            if (mod != null)
            {
                if (!registeredForMod.ContainsKey(mod))
                    registeredForMod.Add(mod, new List<T>());

                registeredForMod[mod].Add(toRegister);
            }
        }
    }
}
