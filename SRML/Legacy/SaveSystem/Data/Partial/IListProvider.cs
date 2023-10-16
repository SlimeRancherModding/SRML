using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.SaveSystem.Data.Partial
{
    internal interface IListProvider
    {
        IList InternalList { get; }
    }
}
