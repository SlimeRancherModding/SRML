using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SRML.Editor;
using SRML.Editor.Runtime;

namespace SRML.Editor
{
    internal class ResolvedInstance
    {
        public object Instance { get; private set; }
        public static ResolvedInstance Resolve(IInstanceInfo info)
        {
            var instance = new ResolvedInstance();
            if (info is IRuntimeInstanceInfo rinfo)
            {
                instance.Instance = rinfo.OnResolve();
                return instance;
            }
            switch (info.idType)
            {
                case IDType.IDENTIFIABLE:
                    instance.Instance = GameContext.Instance.LookupDirector.GetPrefab((Identifiable.Id) info.ID);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return instance;
        }
        
    }
}
