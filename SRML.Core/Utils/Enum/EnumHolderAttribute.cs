using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.Utils.Enum
{
    public class EnumHolderAttribute : Attribute
    {
        public bool shouldCategorize = true;

        public EnumHolderAttribute()
        {
        }

        public EnumHolderAttribute(bool shouldCategorize)
        {
            this.shouldCategorize = shouldCategorize;
        }
    }
}   
