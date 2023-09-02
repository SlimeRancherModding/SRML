using System;
using System.Reflection;

namespace SRML.Core.API.BuiltIn
{
    public abstract class IDRegistry<R, T, TEnum, C> : IEnumRegistry // this entire class is a mess, but it should make everything so much cleaner
        where R : IDRegistry<R, T, TEnum, C>
        where TEnum : Enum
        where C : UnityEngine.Component
    {
        public abstract MethodInfo ComponentInitializeMethod { get; }
        public abstract bool Prefix { get; }

        private InternalEnumRegistry enumReg;
        private InternalComponentRegistry compReg;

        public void Initialize()
        {
            enumReg = new InternalEnumRegistry(this);
            compReg = new InternalComponentRegistry(this);
        }

        public void Register(T toRegister) => compReg.Register(toRegister);
        public void Register(string name) => enumReg.Register(name);
        public void Register(string name, object value) => enumReg.Register(name, value);

        public abstract bool IsRegistered(T registered, C component);
        protected abstract void RegisterIntoComponent(T toRegister, C component);
        protected virtual void InitializeComponent(C component)
        {
        }

        public abstract void Process(Enum toProcess);

        internal class InternalEnumRegistry : EnumRegistry<InternalEnumRegistry, TEnum>
        {
            public IDRegistry<R, T, TEnum, C> parent;

            public override void Process(TEnum toProcess) => parent.Process(toProcess);

            public InternalEnumRegistry(IDRegistry<R, T, TEnum, C> parentRegistry)
            {
                parent = parentRegistry;
            }
        }

        internal class InternalComponentRegistry : ComponentRegistry<InternalComponentRegistry, T, C>
        {
            public override MethodInfo ComponentInitializeMethod { get; }
            public override bool Prefix { get; }

            public IDRegistry<R, T, TEnum, C> parent;

            public override bool IsRegistered(T registered, C component) => parent.IsRegistered(registered, component);
            protected override void RegisterIntoComponent(T toRegister, C component) => parent.RegisterIntoComponent(toRegister, component);

            public InternalComponentRegistry(IDRegistry<R, T, TEnum, C> parentRegistry)
            {
                parent = parentRegistry;
                ComponentInitializeMethod = parentRegistry.ComponentInitializeMethod;
                Prefix = parentRegistry.Prefix;
            }
        }
    }
}
