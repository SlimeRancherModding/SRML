using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.Translation
{
    public abstract class UpgradeTranslation<T> : PediaTranslation<T>
    {
        public abstract string UpgradeType { get; }

        public override string NamePrefix => DefaultNamePrefix + UpgradeType + ".";

        public override string DescriptionPrefix => DefaultDescriptionPrefix + UpgradeType +".";

        public override string PediaType => "upgrade";

    }
}
