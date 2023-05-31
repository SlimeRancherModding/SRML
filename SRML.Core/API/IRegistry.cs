using SRML.Core.ModLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SRML.Core.API
{
    public interface IRegistry
    {
        void Initialize();
        void Register(object toRegister);
    }

    public abstract class Registry<T> : ClassSingleton<Registry<T>>, IRegistry
    {
        protected Dictionary<IMod, List<T>> registeredForMod = new Dictionary<IMod, List<T>>();

        public abstract void Initialize();

        public void Register(object toRegister) => Register((T)toRegister);
        public abstract void Register(T toRegister);
    }
}
