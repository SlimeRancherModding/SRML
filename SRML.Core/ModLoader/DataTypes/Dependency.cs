using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SRML.Core.ModLoader.DataTypes
{
    public class Dependency
    {
        public readonly string[] loadBeforeIds;
        public readonly string[] loadAfterIds;
        public readonly string id;

        public Dependency(string id, string[] loadBeforeIds, string[] loadAfterIds)
        {
            this.id = id;
            this.loadBeforeIds = loadBeforeIds;
            this.loadAfterIds = loadAfterIds;
        }

        public static string[] CalculateLoadOrder(Dependency[] dependencies)
        {
            if (dependencies.Any(x => x.loadAfterIds.Any(y => x.loadBeforeIds.Contains(y))))
                throw new InvalidOperationException("Dependency cannot be sorted before and after another dependency.");

            Dictionary<string, string[]> loadBefores = dependencies.ToDictionary(x => x.id, 
                y => y.loadBeforeIds.Where(x => Main.loader.mods.Any(z => z.ModInfo.Id == x)).ToArray());
            Dictionary<string, string[]> loadAfters = dependencies.ToDictionary(x => x.id, 
                y => y.loadAfterIds.Where(x => Main.loader.mods.Any(z => z.ModInfo.Id == x)).ToArray());

            if (loadBefores.Any(x => x.Value.Any(y => loadBefores[y].Contains(x.Key))) ||
                loadAfters.Any(x => x.Value.Any(y => loadAfters[y].Contains(x.Key))))
                throw new InvalidOperationException("Two dependencies cannot load before or after one another.");

            List<string> loadOrder = dependencies.Select(x => x.id).ToList();
            foreach (var before in loadBefores)
            {
                if (before.Value.Length == 0)
                    continue;

                int index = loadOrder.IndexOf(before.Key);
                int toAdd = before.Value.Select(x => loadOrder.IndexOf(x)).OrderBy(x => x).First();

                if (index < toAdd)
                    continue;

                loadOrder.RemoveAt(index);
                loadOrder.Insert(toAdd, before.Key);
            }
            foreach (var after in loadAfters)
            {
                if (after.Value.Length == 0)
                    continue;

                int index = loadOrder.IndexOf(after.Key);
                int toAdd = after.Value.Select(x => loadOrder.IndexOf(x)).OrderBy(x => x).Last();

                if (index > toAdd)
                    continue;

                loadOrder.RemoveAt(index);
                loadOrder.Insert(toAdd, after.Key);
            }

            return loadOrder.ToArray();
        }
    }
}
