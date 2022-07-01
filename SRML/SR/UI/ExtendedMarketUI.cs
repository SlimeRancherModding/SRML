using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SRML.SR.UI
{
    internal class ExtendedMarketUI : BaseUI
    {
        public GameObject inventoryGridPanel;
        public GameObject inventoryEntryPrefab;
        public MarketUI.PlortEntry[] plorts;
        public Sprite increasingPriceIcon;
        public Sprite decreasingPriceIcon;
        public Sprite staticPriceIcon;
        private MessageBundle actorBundle;

        public override void Awake()
        {
            base.Awake();
            actorBundle = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("actor");
            EconomyDirector econDirector = SceneContext.Instance.EconomyDirector;
            foreach (MarketUI.PlortEntry item in plorts)
                AddInventory(item.id, econDirector.GetCurrValue(item.id).GetValueOrDefault(), econDirector.GetChangeInValue(item.id).GetValueOrDefault());
            transform.Find("MainPanel/CloseButton").GetComponent<Button>().onClick.AddListener(base.Close);
        }

        private void AddInventory(Identifiable.Id id, int value, int change) => CreateInventoryEntry(id, value, change).transform.SetParent(inventoryGridPanel.transform, false);

        private GameObject CreateInventoryEntry(Identifiable.Id id, int value, int change)
        {
            GameObject inventoryEntry = Instantiate(inventoryEntryPrefab);
            inventoryEntry.transform.Find("Content/Name").gameObject.GetComponent<TMP_Text>().text = actorBundle.Xlate("l." + id.ToString().ToLowerInvariant());
            inventoryEntry.transform.Find("Content/Icon").gameObject.GetComponent<Image>().sprite = id.GetIcon();
            inventoryEntry.transform.Find("CountsOuterPanel/CountsPanel/Counts").gameObject.GetComponent<TMP_Text>().text = value.ToString();

            if (change > 0)
                inventoryEntry.transform.Find("CountsOuterPanel/CountsPanel/ChangeIcon").gameObject.GetComponent<Image>().sprite = increasingPriceIcon;
            else if (change < 0)
                inventoryEntry.transform.Find("CountsOuterPanel/CountsPanel/ChangeIcon").gameObject.GetComponent<Image>().sprite = decreasingPriceIcon;
            else
                inventoryEntry.transform.Find("CountsOuterPanel/CountsPanel/ChangeIcon").gameObject.GetComponent<Image>().sprite = staticPriceIcon;

            return inventoryEntry;
        }
    }
}
