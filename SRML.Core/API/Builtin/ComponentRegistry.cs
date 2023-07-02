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

    public abstract class ComponentRegistry<R, T, T2, C> : Registry<R, T, T2>
        where C : Component
        where R : ComponentRegistry<R, T, T2, C>
    {
        protected bool AlreadyRegistered { get; private set; }

        public C RegisteredComponent { get; internal set; }

        public abstract MethodInfo ComponentInitializeMethod { get; }
        public abstract bool Prefix { get; }

        protected virtual void InitializeComponent(C component)
        {
        }

        protected virtual void PreRegister(T toRegister, T2 toRegister2)
        {
        }

        protected abstract void RegisterIntoComponent(T toRegister, T2 toRegister2, C component);

        public abstract bool IsRegistered(T registered, T2 registered2, C component);

        private void RegisterAllIntoComponent(C component)
        {
            if (component == RegisteredComponent)
                return;

            RegisteredComponent = component;
            AlreadyRegistered = true;

            foreach (var toRegister in _registered)
                RegisterIntoComponent(toRegister.Item1, toRegister.Item2, component);
        }

        public sealed override void Initialize()
        {
            if (ComponentInitializeMethod.DeclaringType != typeof(C))
                throw new InvalidOperationException("Cannot initialize component on method that does not belong to that component.");

            Main.HarmonyInstance.Patch(ComponentInitializeMethod,
                Prefix ? new HarmonyMethod(GetType(), "AddToComponent") : null,
                Prefix ? null : new HarmonyMethod(GetType(), "AddToComponent"), null);
        }

        public sealed override void Register(T toRegister, T2 toRegister2)
        {
            if (IsRegistered(toRegister, toRegister2))
                return;

            PreRegister(toRegister, toRegister2);
            if (AlreadyRegistered)
                RegisterIntoComponent(toRegister, toRegister2, RegisteredComponent);

            _registered.AddIfDoesNotContain((toRegister, toRegister2));
            IMod mod = CoreLoader.Instance.GetExecutingModContext();
            if (mod != null)
            {
                if (!registeredForMod.ContainsKey(mod))
                    registeredForMod.Add(mod, new List<(T, T2)>());

                registeredForMod[mod].Add((toRegister, toRegister2));
            }
        }

        public sealed override bool IsRegistered(T registered, T2 registered2)
        {
            if (RegisteredComponent == null)
                return false;

            return IsRegistered(registered, registered2, RegisteredComponent);
        }

        internal static void AddToComponent(C __instance)
        {
            Instance.InitializeComponent(__instance);
            Instance.RegisterAllIntoComponent(__instance);
        }
    }

    public abstract class ComponentRegistry<R, T, T2, T3, C> : Registry<R, T, T2, T3>
        where C : Component
        where R : ComponentRegistry<R, T, T2, T3, C>
    {
        protected bool AlreadyRegistered { get; private set; }

        public C RegisteredComponent { get; internal set; }

        public abstract MethodInfo ComponentInitializeMethod { get; }
        public abstract bool Prefix { get; }

        protected virtual void InitializeComponent(C component)
        {
        }

        protected virtual void PreRegister(T toRegister, T2 toRegister2, T3 toRegister3)
        {
        }

        protected abstract void RegisterIntoComponent(T toRegister, T2 toRegister2, T3 toRegister3, C component);

        public abstract bool IsRegistered(T registered, T2 registered2, T3 registered3, C component);

        private void RegisterAllIntoComponent(C component)
        {
            if (component == RegisteredComponent)
                return;

            RegisteredComponent = component;
            AlreadyRegistered = true;

            foreach (var toRegister in _registered)
                RegisterIntoComponent(toRegister.Item1, toRegister.Item2, toRegister.Item3, component);
        }

        public sealed override void Initialize()
        {
            if (ComponentInitializeMethod.DeclaringType != typeof(C))
                throw new InvalidOperationException("Cannot initialize component on method that does not belong to that component.");

            Main.HarmonyInstance.Patch(ComponentInitializeMethod,
                Prefix ? new HarmonyMethod(GetType(), "AddToComponent") : null,
                Prefix ? null : new HarmonyMethod(GetType(), "AddToComponent"), null);
        }

        public sealed override void Register(T toRegister, T2 toRegister2, T3 toRegister3)
        {
            if (IsRegistered(toRegister, toRegister2, toRegister3))
                return;

            PreRegister(toRegister, toRegister2, toRegister3);
            if (AlreadyRegistered)
                RegisterIntoComponent(toRegister, toRegister2, toRegister3, RegisteredComponent);

            _registered.AddIfDoesNotContain((toRegister, toRegister2, toRegister3));
            IMod mod = CoreLoader.Instance.GetExecutingModContext();
            if (mod != null)
            {
                if (!registeredForMod.ContainsKey(mod))
                    registeredForMod.Add(mod, new List<(T, T2, T3)>());

                registeredForMod[mod].Add((toRegister, toRegister2, toRegister3));
            }
        }

        public sealed override bool IsRegistered(T registered, T2 registered2, T3 registered3)
        {
            if (RegisteredComponent == null)
                return false;

            return IsRegistered(registered, registered2, registered3, RegisteredComponent);
        }

        internal static void AddToComponent(C __instance)
        {
            Instance.InitializeComponent(__instance);
            Instance.RegisterAllIntoComponent(__instance);
        }
    }
}
