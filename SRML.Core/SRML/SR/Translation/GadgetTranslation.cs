using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.Translation
{
    public class GadgetTranslation : PediaTranslation<Gadget.Id>
    {
        public override string PediaType => "gadget";
        public GadgetTranslation(Gadget.Id id)
        {
            this.Key = id;
        }
    }

    public static class GadgetTranslationExtensions
    {
        public static GadgetTranslation GetTranslation(this Gadget.Id id)
        {
            return new GadgetTranslation(id);
        }
    }
}
