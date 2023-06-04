using System;
using System.Collections.Generic;
using System.Linq;
using SRML.SR.Utils;
using SRML.SR.SaveSystem;
using UnityEngine;

namespace SRML.SR
{
    public static class IdentifiableRegistry
    {
        /// <summary>
        /// Creates an <see cref="Identifiable.Id"/>.
        /// </summary>
        /// <param name="value">What value is assigned to the <see cref="Identifiable.Id"/>.</param>
        /// <param name="name">The name of the <see cref="Identifiable.Id"/>.</param>
        /// <returns>The created <see cref="Identifiable.Id"/>.</returns>
        /// <exception cref="Exception">Throws if ran outside of PreLoad</exception>
        public static Identifiable.Id CreateIdentifiableId(object value, string name) => CreateIdentifiableId(value, name, true);

        /// <summary>
        /// Creates an <see cref="Identifiable.Id"/>.
        /// </summary>
        /// <param name="value">What value is assigned to the <see cref="Identifiable.Id"/>.</param>
        /// <param name="name">The name of the <see cref="Identifiable.Id"/>.</param>
        /// <param name="shouldCategorize">If the <see cref="Identifiable.Id"/> should automatically be categorized into <see cref="Identifiable"/>'s classes.</param>
        /// <returns>The created <see cref="Identifiable.Id"/>.</returns>
        /// <exception cref="Exception">Throws if ran outside of PreLoad</exception>
        public static Identifiable.Id CreateIdentifiableId(object value, string name, bool shouldCategorize = true)
        {
            Identifiable.Id id = API.Identifiable.IdentifiableRegistry.Instance.Register(value, name);
            if (!shouldCategorize)
                API.Identifiable.IdentifiableRegistry.Instance.Decategorize(id);
            return id;
        }

        /// <summary>
        /// Manually set the <see cref="IdentifiableCategorization.Rule"/> of the <see cref="Identifiable.Id"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rule"></param>
        public static void Categorize(this Identifiable.Id id, IdentifiableCategorization.Rule rule) =>
            API.Identifiable.IdentifiableRegistry.Instance.Categorize(id, rule);

        /// <summary>
        /// Remove all instances of an <see cref="Identifiable.Id"/> from every class in <see cref="Identifiable"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rule"></param>
        public static void Uncategorize(this Identifiable.Id id) => API.Identifiable.IdentifiableRegistry.Instance.Decategorize(id);

        /// <summary>
        /// Check if an <see cref="Identifiable.Id"/> was registered by a mod
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if the <see cref="Identifiable.Id"/> was registered by a mod, otherwise false.</returns>
        public static bool IsModdedIdentifiable(this Identifiable.Id id) => API.Identifiable.IdentifiableRegistry.Instance.IsRegistered(id);

        /// <summary>
        /// Gets every <see cref="Identifiable.Id"/> registered by a mod.
        /// </summary>
        /// <param name="id">The ID of the mod to check.</param>
        /// <returns>All <see cref="Identifiable.Id"/>s reigstered by a mod.</returns>
        public static HashSet<Identifiable.Id> GetIdentifiablesForMod(string id) =>
            API.Identifiable.IdentifiableRegistry.Instance.RegisteredForMod(id).ToHashSet();

        /// <summary>
        /// Registers a rule for an id prefix to link to
        /// </summary>
        /// <param name="prefix">The prefix for the id to check for.</param>
        /// <param name="rule">The rule that the specified prefix links to.</param>
        public static void RegisterPrefixRule(string prefix, IdentifiableCategorization.Rule rule) =>
            API.Identifiable.IdentifiableRegistry.Instance.RegisterPrefixRule(prefix, rule);

        /// <summary>
        /// Registers a rule for an id suffix to link to
        /// </summary>
        /// <param name="suffix">The suffix for the id to check for.</param>
        /// <param name="rule">The rule that the specified prefix links to.</param>
        public static void RegisterSuffixRule(string suffix, IdentifiableCategorization.Rule rule) =>
            API.Identifiable.IdentifiableRegistry.Instance.RegisterSuffixRule(suffix, rule);

        /// <summary>
        /// Puts an <see cref="Identifiable.Id"/> into one of the vanilla categories based on its name (see <see cref="LookupDirector"/>)
        /// </summary>
        /// <param name="id"></param>
        public static void CategorizeId(Identifiable.Id id) => API.Identifiable.IdentifiableRegistry.Instance.Categorize(id);

        /// <summary>
        /// Put an <see cref="Identifiable.Id"/> into one of the vanilla categories
        /// </summary>
        /// <param name="id"></param>
        /// <param name="category"></param>
        public static void CategorizeId(Identifiable.Id id, IdentifiableCategorization.Rule category) =>
            API.Identifiable.IdentifiableRegistry.Instance.Categorize(id, category);
    }
}
