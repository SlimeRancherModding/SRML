using System;
using System.Collections.Generic;

namespace SRML.Core.API.BuiltIn
{
    public interface IAttributeCategorizeableEnum
    {
        Type AttributeType { get; }
        List<Enum> Categorized { get; }

        void Categorize(Enum toCategorize, Attribute att);
        bool IsCategorized(Enum categorized);
    }
}
