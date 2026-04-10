using System.Collections.Generic;
using Scripts.UI.Spinner.Data;
using UnityEngine;

namespace Scripts.UI.Spinner
{
    public class DataLoader : MonoBehaviour
    {
        [SerializeField] private SOLevels levels;

        public int LevelCount => levels.Levels.Length;

        public SlotItemConfig[] LoadSlotsForZone(int zone, int slotCount)
        {
            var spinner = levels.Levels[zone].Spinner;

            if (spinner.RandomizeItems)
                return ResolvePoolSlices(spinner.PoolSlotItems, slotCount);

            return spinner.SlotItems;
        }

        public SOSpinner GetSpinnerForZone(int zone)
        {
            return levels.Levels[zone].Spinner;
        }

        private SlotItemConfig[] ResolvePoolSlices(SlotItemPoolConfig[] pool, int slotCount)
        {
            var result = new List<SlotItemConfig>(slotCount);
            var remaining = new List<SlotItemPoolConfig>(pool);

            while (result.Count < slotCount && remaining.Count > 0)
            {
                var picked = PickWeighted(remaining);
                int count = Random.Range(picked.CountRange.x, picked.CountRange.y + 1);
                result.Add(new SlotItemConfig(picked.ItemData, count));

                int existing = 0;
                foreach (var r in result)
                {
                    if (r.ItemData == picked.ItemData)
                        existing++;
                }

                if (existing >= picked.MaxCount)
                    remaining.Remove(picked);
            }

            return result.ToArray();
        }

        private SlotItemPoolConfig PickWeighted(List<SlotItemPoolConfig> pool)
        {
            float totalWeight = 0f;
            foreach (var entry in pool)
                totalWeight += entry.SpawnChance;

            float roll = Random.Range(0f, totalWeight);
            float cumulative = 0f;

            foreach (var entry in pool)
            {
                cumulative += entry.SpawnChance;
                if (roll <= cumulative)
                    return entry;
            }

            return pool[pool.Count - 1];
        }
    }
}