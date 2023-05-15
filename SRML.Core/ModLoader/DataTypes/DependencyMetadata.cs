using Semver;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SRML.Core.ModLoader.DataTypes
{
    public class DependencyMetadata
    {
        public readonly string[] loadBeforeIds;
        public readonly string[] loadAfterIds;
        public readonly Dictionary<string, SemVersion> dependencies;
        public readonly string id;

        public DependencyMetadata(string id, Dictionary<string, SemVersion> dependencies)
        {
            this.id = id;
            this.dependencies = dependencies;
            loadBeforeIds = new string[0];
            loadAfterIds = new string[0];
        }

        public DependencyMetadata(string id, string[] loadBeforeIds, string[] loadAfterIds, Dictionary<string, SemVersion> dependencies)
        {
            this.id = id;
            this.loadBeforeIds = loadBeforeIds;
            this.loadAfterIds = loadAfterIds;
            this.dependencies = dependencies;
        }

        public bool Met() => dependencies.All(x => Main.loader.mods.Any(y => y.ModInfo.Id == x.Key &&
            y.ModInfo.Version.ComparePrecedenceTo(x.Value) != -1));

        public static string[] CalculateLoadOrder(DependencyMetadata[] dependencies)
        {
            if (dependencies.Any(x => x.loadAfterIds.Any(y => x.loadBeforeIds.Contains(y))))
                throw new InvalidOperationException("Dependency cannot be sorted before and after another dependency.");

            Dictionary<string, List<string>> loadAfters = dependencies.ToDictionary(x => x.id, 
                y => y.loadAfterIds.Where(x => Main.loader.mods.Any(z => z.ModInfo.Id == x)).Concat(y.dependencies.Keys).ToList());
            foreach (DependencyMetadata dependency in dependencies)
            {
                foreach (string s in dependency.loadBeforeIds)
                    loadAfters[s].Add(dependency.id);
            }

            if (loadAfters.Any(x => x.Value.Any(y => loadAfters[y].Contains(x.Key))))
                throw new InvalidOperationException("Two dependencies cannot load before or after one another.");

            List<string> loadOrder = dependencies.Select(x => x.id).ToList();
            foreach (var after in loadAfters)
            {
                if (after.Value.Count == 0)
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
