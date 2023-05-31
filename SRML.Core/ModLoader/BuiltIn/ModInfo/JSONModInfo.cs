﻿using Semver;
using SRML.Core.ModLoader.DataTypes;
using System;
using System.Reflection;

namespace SRML.Core.ModLoader.BuiltIn.ModInfo
{
    public abstract class JSONModInfo : IModInfo
    {
        public abstract string Id { get; }

        public abstract SemVersion Version { get; }

        public abstract DependencyMetadata Dependencies { get; }

        public void Parse(Assembly modAssembly)
        {
            string json = GetJSON(modAssembly);
            if (json == null)
                throw new Exception($"Mod loader attempting to load mod from assembly {modAssembly.GetName().Name}, but it doesn't have a mod info.");

            ParseFromJSON(json);
        }

        public abstract string GetDefaultHarmonyName();
        public abstract string GetDefaultConsoleName();
        public abstract string GetJSON(Assembly modAssembly);
        public abstract void ParseFromJSON(string json);
    }
}
