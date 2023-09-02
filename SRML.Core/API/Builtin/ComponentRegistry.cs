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
        protected bool AlreadyRegistered { get; private set; }

        public C RegisteredComponent { get; internal set; }
        
        public abstract MethodInfo ComponentInitializeMethod { get; }
        public abstract bool Prefix { get; }

        protected virtual void InitializeComponent(C component)
        {
        }

        protected virtual void PreRegister(T toRegister)
        {
        }

        protected abstract void RegisterIntoComponent(T toRegister, C component);

        public abstract bool IsRegistered(T registered, C component);

        private void RegisterAllIntoComponent(C component)
        {
            if (component == RegisteredComponent)
                return;

            RegisteredComponent = component;
            AlreadyRegistered = true;

            foreach (T toRegister in _registered)
            {
                if (!IsRegistered(toRegister, component))
                    RegisterIntoComponent(toRegister, component);
            }
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

            PreRegister(toRegister);
            if (AlreadyRegistered)
                RegisterIntoComponent(toRegister, RegisteredComponent);

            _registered.AddIfDoesNotContain(toRegister);
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

        internal static void AddToComponent(C __instance)
        {
            Instance.InitializeComponent(__instance);
            Instance.RegisterAllIntoComponent(__instance);
        }
    }

    public abstract class ComponentRegisterable
    {
        public abstract MethodInfo RegisterMethod { get; }
        public abstract bool RegisterPrefix { get; }
    }
}
