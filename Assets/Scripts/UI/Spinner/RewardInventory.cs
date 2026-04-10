using System;
using System.Collections.Generic;
using Scripts.UI.Spinner.Data;
using Scripts.UI.Spinner.VO;

namespace Scripts.UI.Spinner
{
    public class RewardInventory : IRewardInventory
    {
        private readonly List<RewardEntry> collected = new();
        private readonly List<RewardEntry> stashed = new();

        public IReadOnlyList<RewardEntry> CollectedRewards => collected;
        public IReadOnlyList<RewardEntry> StashedRewards => stashed;

        public event Action OnChanged;

        public void Add(SOSlotItemData itemData, int count)
        {
            AddToList(collected, itemData, count);
            OnChanged?.Invoke();
        }

        public void Remove(SOSlotItemData itemData, int count)
        {
            RemoveFromList(collected, itemData, count);
            OnChanged?.Invoke();
        }

        public void RemoveFromStash(SOSlotItemData itemData, int count)
        {
            RemoveFromList(stashed, itemData, count);
        }

        public void StashAndClear()
        {
            stashed.Clear();
            stashed.AddRange(collected);
            collected.Clear();
        }

        public void RestoreFromStash()
        {
            collected.Clear();
            collected.AddRange(stashed);
            stashed.Clear();
            OnChanged?.Invoke();
        }

        public void Clear()
        {
            collected.Clear();
            OnChanged?.Invoke();
        }

        public void ClearAll()
        {
            collected.Clear();
            stashed.Clear();
            OnChanged?.Invoke();
        }

        private static void AddToList(List<RewardEntry> list, SOSlotItemData itemData, int count)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ItemData.Id == itemData.Id)
                {
                    list[i] = new RewardEntry(itemData, list[i].Count + count);
                    return;
                }
            }
            list.Add(new RewardEntry(itemData, count));
        }

        private static void RemoveFromList(List<RewardEntry> list, SOSlotItemData itemData, int count)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ItemData.Id == itemData.Id)
                {
                    int remaining = list[i].Count - count;
                    if (remaining <= 0)
                        list.RemoveAt(i);
                    else
                        list[i] = new RewardEntry(itemData, remaining);
                    return;
                }
            }
        }
    }
}
