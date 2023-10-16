using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.Translation
{
    interface ITranslatable<T>
    {
        T Key { get; }
        string StringKey { get; }
    }
}
