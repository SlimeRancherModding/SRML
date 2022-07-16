using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Reflection;

namespace SRML.SR.UI
{
    internal static class EnhancedCorporatePartnerHandler
    {
        public static GameObject rewardEntry;
        public static Transform content;

        public static GameObject CreateRewardEntry(CorporatePartnerUI ui, CorporatePartnerRegistry.RewardEntry entry)
        {
            GameObject reward = Object.Instantiate(rewardEntry, content, false);
            reward.GetComponentInChildren<TMP_Text>().text = ui.uiBundle.Get(entry.nameKey);
            reward.GetChild(0).GetComponent<Image>().sprite = entry.icon;
            return reward;
        }

        public static GameObject CreateRewardEntry(CorporatePartnerUI ui, int rank, int index)
        {
            GameObject reward = Object.Instantiate(rewardEntry, content, false);
            Console.Console.Instance.Log((reward.GetComponentInChildren<TMP_Text>() == null).ToString());
            reward.GetComponentInChildren<TMP_Text>().text = ui.uiBundle.Get(string.Format("m.partner_rank.{0}.reward.{1}", rank, index + 1));
            reward.GetChild(0).GetComponent<Image>().sprite = ui.ranks[rank - 1].rewardIcons[index];
            return reward;
        }
    }
}
