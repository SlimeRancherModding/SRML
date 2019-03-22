using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SRML.Editor;

namespace SRML.Editor
{
    internal class ResolvedInstance
    {
        public object Instance { get; private set; }
        public static ResolvedInstance Resolve(InstanceInfo info)
        {
            var instance = new ResolvedInstance();
            switch (info.idType)
            {
                case InstanceInfo.IDType.IDENTIFIABLE:
                    instance.Instance = GameContext.Instance.LookupDirector.GetPrefab((Identifiable.Id) info.ID);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return instance;
        }
        
    }
}
