using System;

namespace SRML.SR
{
    [Obsolete]
    public static class SpawnResourceRegistry
    {
        /// <summary>
        /// Creates a <see cref="SpawnResource.Id"/>.
        /// </summary>
        /// <param name="value">What value is assigned to the <see cref="SpawnResource.Id"/>.</param>
        /// <param name="name">The name of the <see cref="SpawnResource.Id"/>.</param>
        /// <returns>The created <see cref="SpawnResource.Id"/>.</returns>
        /// <exception cref="Exception">Throws if ran outside of PreLoad</exception>
        public static SpawnResource.Id CreateSpawnResourceId(object value, string name) =>
            API.Identifiable.SpawnResourceRegistry.Instance.RegisterAndParse(name, value);
    }
}
