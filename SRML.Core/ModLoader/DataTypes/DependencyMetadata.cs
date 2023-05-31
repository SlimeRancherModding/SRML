using Semver;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

namespace SRML.Core.ModLoader.DataTypes
{
    public class DependencyMetadata
    {
        public readonly string[] loadBeforeIds;
        public readonly string[] loadAfterIds;
        public readonly Dictionary<string, SemVersion> dependencies;
        public readonly string id;
        public readonly SemVersion version;

        public DependencyMetadata(string id, SemVersion version, Dictionary<string, SemVersion> dependencies)
        {
            this.id = id;
            this.version = version;
            this.dependencies = dependencies;
            loadBeforeIds = new string[0];
            loadAfterIds = new string[0];
        }

        public DependencyMetadata(string id, SemVersion version, string[] loadBeforeIds, string[] loadAfterIds, Dictionary<string, SemVersion> dependencies)
        {
            this.id = id;
            this.version = version;
            this.loadBeforeIds = loadBeforeIds;
            this.loadAfterIds = loadAfterIds;
            this.dependencies = dependencies;
        }

        /// <summary>
        /// Within a list of dependencies, whether all of the dependencies within <see cref="dependencies"/> are present.
        /// </summary>
        /// <param name="others">All dependencies to check.</param>
        /// <param name="unmet">Any dependencies that are not present.</param>
        /// <returns>True if all dependencies are present, otherwise false.</returns>
        public bool Met(DependencyMetadata[] others, out Dictionary<string, SemVersion> unmet)
        {
            unmet = dependencies.Where(x => !others.Any(y => y.id == x.Key && y.version.ComparePrecedenceTo(x.Value) != -1))
                .ToDictionary(x => x.Key, y => y.Value);

            return unmet.Count == 0;
        }

        /// <summary>
        /// If, for every dependency metadata in a list, all of them are met.
        /// </summary>
        /// <param name="dependencies">The dependency metadata to check.</param>
        /// <param name="unmet">Any dependencies that are not present.</param>
        /// <returns>True if all dependencies are present, otherwise false.</returns>
        public static bool AllDependenciesMet(DependencyMetadata[] dependencies, out Dictionary<string, Dictionary<string, SemVersion>> unmet)
        {
            bool allMet = true;
            unmet = new Dictionary<string, Dictionary<string, SemVersion>>();

            foreach (DependencyMetadata dependency in dependencies)
            {
                if (!dependency.Met(dependencies, out var unmetPerMod))
                {
                    unmet.Add(dependency.id, unmetPerMod);
                    allMet = false;
                }
            }

            return allMet;
        }

        /// <summary>
        /// Calculates the load order for mods based on their dependency metadata.
        /// </summary>
        /// <param name="dependencies">The dependencies to sort.</param>
        /// <returns>A list of mod ids in load order</returns>
        public static string[] CalculateLoadOrder(DependencyMetadata[] dependencies)
        {
            if (!AllDependenciesMet(dependencies, out var unmet))
            {
                // TODO: when error throwing is readded, modify this to actually throw ALL dependency errors
                throw new Exception($"Dependencies for {unmet.First().Key} unmet; requires {string.Join(", ", unmet.First().Value.Select(x => $"{x.Key} {x.Value}"))}");
            }

            if (dependencies.Any(x => x.loadAfterIds.Any(y => x.loadBeforeIds.Contains(y))))
                throw new InvalidOperationException("Dependency cannot be sorted before and after another dependency.");

            Dictionary<string, List<string>> loadAfters = dependencies.ToDictionary(x => x.id, 
                y => y.loadAfterIds.Where(x => CoreLoader.Main.mods.Any(z => z.ModInfo.Id == x)).Concat(y.dependencies.Keys).ToList());
            foreach (DependencyMetadata dependency in dependencies)
            {
                foreach (string s in dependency.loadBeforeIds)
                {
                    if (dependencies.Any(x => x.id == s))
                        loadAfters[s].Add(dependency.id);
                }
            }

            if (loadAfters.Any(x => x.Value.Any(y => loadAfters.ContainsKey(y) && loadAfters[y].Contains(x.Key))))
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
