using System;
using Scripts.UI.Spinner.Data;
using UnityEngine;

namespace Scripts.UI.Spinner
{
    public class CurrencyManager : MonoBehaviour, ICurrencyProvider
    {
        [SerializeField] private SpinnerManager spinnerManager;

        public event Action<int> OnGemsChanged;

        private IRewardInventory Inventory => spinnerManager.Inventory;

        public int GemCount
        {
            get
            {
                var source = spinnerManager.State == SpinnerState.GameOver
                    ? Inventory.StashedRewards
                    : Inventory.CollectedRewards;

                for (int i = 0; i < source.Count; i++)
                {
                    if (source[i].ItemData.ItemType == SlotItemType.Gem)
                        return source[i].Count;
                }
                return 0;
            }
        }

        public bool CanAfford(int amount)
        {
            return GemCount >= amount;
        }

        public bool TrySpend(int amount)
        {
            if (!CanAfford(amount)) return false;

            var gemData = FindGemItemData();
            if (gemData == null) return false;

            Inventory.RemoveFromStash(gemData, amount);
            OnGemsChanged?.Invoke(GemCount);
            return true;
        }

        private SOSlotItemData FindGemItemData()
        {
            var source = spinnerManager.State == SpinnerState.GameOver
                ? Inventory.StashedRewards
                : Inventory.CollectedRewards;

            for (int i = 0; i < source.Count; i++)
            {
                if (source[i].ItemData.ItemType == SlotItemType.Gem)
                    return source[i].ItemData;
            }
            return null;
        }


#if UNITY_EDITOR
        private void OnValidate()
        {
            spinnerManager ??= GetComponentInParent<SpinnerManager>();
        }
#endif
    }
}
