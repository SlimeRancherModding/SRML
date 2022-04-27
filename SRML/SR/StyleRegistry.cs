using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR
{
    public static class StyleRegistry
    {
        internal static Dictionary<SecretStyle, SRMod> secretStyles = new Dictionary<SecretStyle, SRMod>(SecretStyle.EqualityComparer.Default);

        static StyleRegistry()
        {
            SRCallbacks.OnGameContextReady += () =>
            {
                GameContext.Instance.DLCDirector.onPackageInstalled += (s =>
                {
                    if (s == DLCPackage.Id.SECRET_STYLE)
                    {
                        foreach (SecretStyle ss in secretStyles.Keys)
                            ss.SlimeDefinition.RegisterDynamicAppearance(ss.ExoticAppearance);
                    }
                });
            };
        }

        /// <returns><see langword="true"/> if secret styles is installed, otherwise <see langword="false"/>.</returns>
        public static bool IsSecretStylesInstalled()
        {
            if (SRModLoader.CurrentLoadingStep == SRModLoader.LoadingStep.PRELOAD)
                throw new Exception("Cannot check if secret styles is installed during the PreLoad step");
            return GameContext.Instance.DLCDirector.IsPackageInstalledAndEnabled(DLCPackage.Id.SECRET_STYLE);
        }

        /// <summary>
        /// Register a secret style in a <see cref="SlimeDefinition"/>
        /// </summary>
        /// <param name="definition">Slime definition to register the secret style for</param>
        /// <param name="exoticAppearance">Secret style to register</param>
        public static void RegisterSecretStyle(SlimeDefinition definition, SlimeAppearance exoticAppearance)
        {
            SecretStyle ss = new SecretStyle(definition, exoticAppearance);
            SRMod mod = SRMod.GetCurrentMod();
            if (secretStyles.ContainsKey(ss))
                throw new Exception($"{definition.IdentifiableId} already has a secret style registered by {secretStyles[ss].ModInfo.Id}");
            secretStyles[ss] = mod;
        }

        internal struct SecretStyle : IEquatable<SecretStyle>
        {
            public class EqualityComparer : IEqualityComparer<SecretStyle>
            {
                public static EqualityComparer Default = new EqualityComparer();

                public bool Equals(SecretStyle x, SecretStyle y) => x == y;

                public int GetHashCode(SecretStyle obj) => obj.GetHashCode();
            }

            public SlimeDefinition SlimeDefinition;

            public SlimeAppearance ExoticAppearance;

            public static bool operator ==(SecretStyle a, SecretStyle b) => a.SlimeDefinition == b.SlimeDefinition;
            public static bool operator !=(SecretStyle a, SecretStyle b) => a.SlimeDefinition != b.SlimeDefinition;

            public override bool Equals(object obj)
            {
                if (obj is SecretStyle ss)
                    return this.Equals(ss);
                return base.Equals(obj);
            }

            public override int GetHashCode() => SlimeDefinition.GetHashCode();

            public override string ToString()
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append('[');
                if (SlimeDefinition != null)
                    stringBuilder.Append(SlimeDefinition.ToString());

                stringBuilder.Append(", ");
                if (ExoticAppearance != null)
                    stringBuilder.Append(ExoticAppearance.ToString());

                stringBuilder.Append(']');
                return stringBuilder.ToString();
            }

            public bool Equals(SecretStyle ss) => this.SlimeDefinition == ss.SlimeDefinition;

            public SecretStyle(SlimeDefinition definition, SlimeAppearance exoticAppearance)
            {
                SlimeDefinition = definition;
                ExoticAppearance = exoticAppearance;
            }
        }
    }
}