using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.Translation
{
    public class PersonalUpgradeTranslation : UpgradeTranslation<PlayerState.Upgrade>
    {
        public override string UpgradeType => "personal";
        public PersonalUpgradeTranslation(PlayerState.Upgrade upgrade)
        {
            this.Key = upgrade;
        }


    }
    public static class PersonalUpgradeExtension
    {
        public static PersonalUpgradeTranslation GetTranslation(this PlayerState.Upgrade upgrade)
        {
            return new PersonalUpgradeTranslation(upgrade);
        }
    }
}
