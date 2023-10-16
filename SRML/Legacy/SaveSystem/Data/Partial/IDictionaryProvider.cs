using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.SaveSystem.Data.Partial
{
    interface IDictionaryProvider
    {
        IDictionary InternalDictionary { get; }
    }
}
