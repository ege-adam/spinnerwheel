using System.Collections.Generic;
using Scripts.UI.Spinner.Data;
using Scripts.UI.Spinner.VO;
using UnityEngine;

namespace Scripts.UI.Spinner
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private SpinnerManager spinnerManager;
        [SerializeField] private Transform scrollContent;
        [SerializeField] private InventorySlotView slotPrefab;

        private readonly Dictionary<string, InventorySlotView> activeSlots = new();
        private readonly Stack<InventorySlotView> pool = new();

        private IRewardInventory Inventory => spinnerManager.Inventory;

        private void OnEnable()
        {
            spinnerManager.OnRewardEarned += HandleRewardEarned;
            spinnerManager.OnBombHit += HandleClear;
            spinnerManager.OnCollect += HandleCollect;
            spinnerManager.OnRevive += HandleRevive;
            Inventory.OnChanged += HandleInventoryChanged;
        }

        private void OnDisable()
        {
            spinnerManager.OnRewardEarned -= HandleRewardEarned;
            spinnerManager.OnBombHit -= HandleClear;
            spinnerManager.OnCollect -= HandleCollect;
            spinnerManager.OnRevive -= HandleRevive;
            Inventory.OnChanged -= HandleInventoryChanged;
        }

        private void HandleRewardEarned(SlotItemConfig config)
        {
            Refresh();
        }

        private void HandleCollect(List<RewardEntry> rewards)
        {
            ReturnAll();
        }

        private void HandleClear()
        {
            ReturnAll();
        }

        private void HandleRevive()
        {
            ReturnAll();
            Refresh();
        }

        private void HandleInventoryChanged()
        {
            Refresh();
        }

        public void Refresh()
        {
            var rewards = Inventory.CollectedRewards;

            var idsToRemove = new List<string>();
            foreach (var kvp in activeSlots)
            {
                bool found = false;
                for (int i = 0; i < rewards.Count; i++)
                {
                    if (rewards[i].ItemData.Id == kvp.Key)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    idsToRemove.Add(kvp.Key);
            }
            foreach (var id in idsToRemove)
            {
                ReturnToPool(activeSlots[id]);
                activeSlots.Remove(id);
            }

            foreach (var reward in rewards)
            {
                string id = reward.ItemData.Id;

                if (activeSlots.TryGetValue(id, out var existing))
                {
                    existing.UpdateCount(reward.Count);
                }
                else
                {
                    var slot = GetFromPool();
                    slot.Set(reward.ItemData.Icon, id, reward.ItemData.ItemName, reward.Count);
                    activeSlots[id] = slot;
                }
            }
        }

        private InventorySlotView GetFromPool()
        {
            if (pool.Count > 0)
            {
                var slot = pool.Pop();
                slot.gameObject.SetActive(true);
                return slot;
            }
            return Instantiate(slotPrefab, scrollContent);
        }

        private void ReturnToPool(InventorySlotView slot)
        {
            slot.gameObject.SetActive(false);
            pool.Push(slot);
        }

        private void ReturnAll()
        {
            foreach (var slot in activeSlots.Values)
            {
                ReturnToPool(slot);
            }
            activeSlots.Clear();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            spinnerManager ??= GetComponentInParent<SpinnerManager>();
        }
#endif
    }
}
