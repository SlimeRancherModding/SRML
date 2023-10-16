using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.Editor.Runtime
{
    public class InstanceInfo : IRuntimeInstanceInfo, IInstanceInfo
    {
        public RuntimeInstanceProviderDelegate OnResolve { get; private set; }

        public IDType idType => throw new NotImplementedException();

        public int ID => throw new NotImplementedException();

        public InstanceInfo(RuntimeInstanceProviderDelegate onResolve)
        {
            OnResolve = onResolve;
        }
    }

    public delegate object RuntimeInstanceProviderDelegate();
    public interface IRuntimeInstanceInfo
    {
        RuntimeInstanceProviderDelegate OnResolve { get; }
    }
}
