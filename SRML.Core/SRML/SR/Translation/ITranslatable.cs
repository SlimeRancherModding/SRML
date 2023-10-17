using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.Translation
{
    interface ITranslatable<T>
    {
        T Key { get; }
        MessageDirector.Lang Language { get; }
        string StringKey { get; }
    }
}
