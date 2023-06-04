using System;
using System.Collections.Generic;

namespace SRML.Core.API.BuiltIn
{
    public interface ICategorizableEnum
    {
        List<Enum> Categorized { get; }

        void Categorize(Enum toCategorize);
        void Decategorize(Enum toDecategorize);
        bool IsCategorized(Enum categorized);
    }
}
