using Scripts.UI.Spinner.Data;

namespace Scripts.UI.Spinner.VO
{
    [System.Serializable]
    public struct RewardEntry
    {
        public SOSlotItemData ItemData;
        public int Count;

        public RewardEntry(SOSlotItemData itemData, int count)
        {
            ItemData = itemData;
            Count = count;
        }
    }
}