using HarmonyLib;
using SRML.Core.ModLoader;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SRML.Core.API.BuiltIn
{
    public abstract class ComponentRegistry<R, T, C> : Registry<R, T> 
        where C : Component 
        where R : ComponentRegistry<R, T, C>
    {
        protected List<T> registered = new List<T>();
        private bool alreadyRegistered;

        public C RegisteredComponent { get; internal set; }
        
        public abstract MethodInfo ComponentInitializeMethod { get; }
        public abstract bool Prefix { get; }

        protected abstract void InitializeComponent(C component);

        protected abstract void RegisterIntoComponent(T toRegister, C component);

        public abstract bool IsRegistered(T registered, C component);

        private void RegisterAllIntoComponent(C component)
        {
            if (component == RegisteredComponent)
                return;

            RegisteredComponent = component;
            alreadyRegistered = true;

            foreach (T toRegister in registered)
                Register(toRegister);
        }

        public sealed override void Initialize()
        {
            if (ComponentInitializeMethod.DeclaringType != typeof(C))
                throw new InvalidOperationException("Cannot initialize component on method that does not belong to that component.");

            Main.HarmonyInstance.Patch(ComponentInitializeMethod, 
                Prefix ? new HarmonyMethod(GetType(), "AddToComponent") : null, 
                Prefix ? null : new HarmonyMethod(GetType(), "AddToComponent"), null);
        }

        public sealed override void Register(T toRegister)
        {
            if (IsRegistered(toRegister))
                return;

            registered.Add(toRegister);
            if (alreadyRegistered)
                RegisterIntoComponent(toRegister, RegisteredComponent);

            IMod mod = CoreLoader.Instance.GetExecutingModContext();
            if (mod != null)
            {
                if (!registeredForMod.ContainsKey(mod))
                    registeredForMod.Add(mod, new List<T>());

                registeredForMod[mod].Add(toRegister);
            }
        }

        public sealed override bool IsRegistered(T registered)
        {
            if (RegisteredComponent == null)
                return false;

            return IsRegistered(registered, RegisteredComponent);
        }

        internal static void AddToComponent(C __instance) => Instance.RegisterAllIntoComponent(__instance);
    }
}
