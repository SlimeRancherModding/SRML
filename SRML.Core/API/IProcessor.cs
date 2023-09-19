using SRML.Core.ModLoader;

namespace SRML.Core.API
{
    public interface IProcessor
    {
        void Register(object registerInto);
    }

    public abstract class Processor<T> : IProcessor
    {
        public readonly string ModId;
        protected T registerInto;

        public sealed override int GetHashCode() => base.GetHashCode();

        public sealed override bool Equals(object obj)
        {
            if (obj.GetType().IsInstanceOfType(typeof(Processor<>)))
                return false;

            if (obj.GetType() != this.GetType())
                return false;

            return IsIdenticalTo((Processor<T>)obj);
        }

        public virtual void Initialize()
        {
        }

        public abstract bool IsIdenticalTo(Processor<T> other);
        public abstract void Register();

        public void Register(T registerInto)
        {
            this.registerInto = registerInto;
            Register();
        }
        public void Register(object registerInto)
        {
            this.registerInto = (T)registerInto;
            Register();
        }

        public Processor() => ModId = CoreLoader.Instance.GetExecutingModContext().ModInfo.Id;
    }
}
