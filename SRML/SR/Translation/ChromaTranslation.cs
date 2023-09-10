using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRML.SR.Translation
{
    public class ChromaTranslation : ITranslatable<RanchDirector.Palette>
    {
        public const string NamePrefix = "m.palette.name.";

        public RanchDirector.Palette Key { get; protected set; }

        public string StringKey => Key.ToString().ToLower();

        public string NameKey => NamePrefix + StringKey;

        public ChromaTranslation SetNameTranslation(string name)
        {
            TranslationPatcher.AddPediaTranslation(NameKey, name);
            return this;
        }

        public ChromaTranslation(RanchDirector.Palette palette)
        {
            this.Key = palette;
        }
    }

    public class ChromaTypeTranslation : ITranslatable<RanchDirector.PaletteType>
    {
        public const string NamePrefix = "b.";

        public RanchDirector.PaletteType Key { get; protected set; }

        public string StringKey => Key.ToString().ToLower();

        public string NameKey => NamePrefix + StringKey;

        public ChromaTypeTranslation SetNameTranslation(string name)
        {
            TranslationPatcher.AddUITranslation(NameKey, name);
            return this;
        }

        public ChromaTypeTranslation(RanchDirector.PaletteType palette)
        {
            this.Key = palette;
        }
    }

    public static class ChromaTranslationExtensions
    {
        public static ChromaTranslation GetTranslation(this RanchDirector.Palette palette)
        {
            return new ChromaTranslation(palette);
        }

        public static ChromaTypeTranslation GetTranslation(this RanchDirector.PaletteType paletteType)
        {
            return new ChromaTypeTranslation(paletteType);
        }
    }
}
