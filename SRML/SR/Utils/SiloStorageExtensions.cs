using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class SiloStorageExtensions
{
    public static bool MaybeAddIdentifiable(this SiloStorage storage, Identifiable id)
    {
        bool result = storage.GetRelevantAmmo().MaybeAddToSlot(id.id, id);
        storage.OnAdded();
        return result;
    }

    public static bool MaybeAddIdentifiable(this SiloStorage storage, Identifiable id, int slotIdx, int count = 1, bool overflow = false)
    {
        bool result = storage.GetRelevantAmmo().MaybeAddToSpecificSlot(id.id, id, slotIdx, count, overflow);
        storage.OnAdded();
        return result;
    }
}

