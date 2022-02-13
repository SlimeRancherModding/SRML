﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRML.SR
{
    public static class SnareRegistry
    {
        internal static HashSet<Identifiable.Id> snareables = new HashSet<Identifiable.Id>(Identifiable.idComparer);

        public static void RegisterSnareable(Identifiable.Id id)
        {
            if (!snareables.Contains(id))
                snareables.Add(id);
        }
    }
}
