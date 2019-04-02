using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.SaveSystem
{
    internal class ModdedIDRegistry
    {
        public delegate SRMod GetModForIDDelegate(object id);

        public delegate Type GetRegistryTypeDelegate();

        public delegate bool IsModdedIDDelegate(object id);

        public delegate IList GetIDsForModDelegate(SRMod mod);

        private GetModForIDDelegate modForId;
        private GetRegistryTypeDelegate typeForRegistry;
        private IsModdedIDDelegate isIdModded;
        private GetIDsForModDelegate getids;

        public Type RegistryType => typeForRegistry();

        public bool IsModdedID(object val) => isIdModded(val);

        public SRMod GetModForID(object val) => modForId(val);

        public IList GetIDsForMod(SRMod mod) => getids(mod);

        public ModdedIDRegistry(GetModForIDDelegate modforid, GetRegistryTypeDelegate regtype,
            IsModdedIDDelegate ismodded,GetIDsForModDelegate modids)
        {
            this.modForId = modforid;
            this.typeForRegistry = regtype;
            this.isIdModded = ismodded;
            this.getids = modids;
        }
    }
}
