using System;
using System.Collections.Generic;
using Scripts.UI.Spinner.Data;
using Scripts.UI.Spinner.VO;

namespace Scripts.UI.Spinner
{
    public interface IRewardInventory
    {
        IReadOnlyList<RewardEntry> CollectedRewards { get; }
        IReadOnlyList<RewardEntry> StashedRewards { get; }

        event Action OnChanged;

        void Add(SOSlotItemData itemData, int count);
        void Remove(SOSlotItemData itemData, int count);
        void RemoveFromStash(SOSlotItemData itemData, int count);
        void StashAndClear();
        void RestoreFromStash();
        void Clear();
        void ClearAll();
    }
}
