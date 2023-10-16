using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.Translation
{
    public abstract class PediaTranslation<T> : ITranslatable<T>
    {

        public virtual string DefaultPrefix => "m.";

        public virtual string DefaultNamePrefix => DefaultPrefix + PediaType + ".name.";
        public virtual string DefaultDescriptionPrefix => DefaultPrefix + PediaType + ".desc.";

        public virtual string NamePrefix => DefaultNamePrefix;
        public virtual string DescriptionPrefix => DefaultDescriptionPrefix;

        public virtual T Key { get; protected set; }

        public virtual string StringKey => Key.ToString().ToLower();

        public virtual string NameKey => NamePrefix + StringKey;

        public virtual string DescriptionKey => DescriptionPrefix + StringKey;

        public abstract string PediaType { get; }

        public virtual PediaTranslation<T> SetDescriptionTranslation(string desc)
        {
            TranslationPatcher.AddPediaTranslation(DescriptionKey, desc);
            return this;
        }
        public virtual PediaTranslation<T> SetNameTranslation(string name)
        {
            TranslationPatcher.AddPediaTranslation(NameKey, name);
            return this;
        }
    }
}
