using TriInspector;
using UnityEngine;

namespace Scripts.UI.Spinner.Data
{
    public enum SpinnerType { Normal, Safe, Super }

    [System.Serializable]
    [CreateAssetMenu(fileName = "SpinnerData", menuName = "ScriptableObjects/UI/Spinner/SpinnerData", order = 0)]
    public class SOSpinner : ScriptableObject
    {
        [SerializeField] private string spinnerName;
        [SerializeField] private SpinnerType spinnerType;
        [SerializeField] private string spinnerTopText;
        [SerializeField] private string spinnerBottomText;
        [SerializeField] private Sprite spinnerSprite;
        [SerializeField] private Sprite needleSprite;

        [HideIf(nameof(randomizeItems))]
        [SerializeField] private SlotItemConfig[] slotItems;

        [ShowIf(nameof(randomizeItems))]
        [SerializeField] private SlotItemPoolConfig[] poolSlotItems;

        [SerializeField] private bool randomizeItems;
        [SerializeField, ReadOnly] private string id;

        public string SpinnerName { get => spinnerName; }
        public SpinnerType SpinnerType { get => spinnerType; }
        public string SpinnerTopText { get => spinnerTopText; }
        public string SpinnerBottomText { get => spinnerBottomText; }
        public bool RandomizeItems { get => randomizeItems; }
        public SlotItemConfig[] SlotItems { get => slotItems; }
        public SlotItemPoolConfig[] PoolSlotItems { get => poolSlotItems; }
        public Sprite SpinnerSprite { get => spinnerSprite; }
        public Sprite NeedleSprite { get => needleSprite; }
        public string Id { get => id; }

        private void Reset()
        {
            if (string.IsNullOrEmpty(id))
                id = System.Guid.NewGuid().ToString();
        }
    }

    [System.Serializable]
    public class SlotItemConfig
    {
        [SerializeField] private SOSlotItemData itemData;
        [SerializeField] private int count;

        public SlotItemConfig(SOSlotItemData itemData, int count)
        {
            this.itemData = itemData;
            this.count = count;
        }

        public SOSlotItemData ItemData { get => itemData; }
        public int Count { get => count; }
    }

    [System.Serializable]
    public class SlotItemPoolConfig
    {
        [SerializeField] private SOSlotItemData itemData;
        [SerializeField, Range(0, 1)] private float spawnChance = 1f;
        [SerializeField, Range(0, 10)] private int maxCount = 2;
        [SerializeField, MinMaxSlider(0, 10000)] private Vector2Int countRange;

        public SOSlotItemData ItemData { get => itemData; }
        public float SpawnChance { get => spawnChance; }
        public int MaxCount { get => maxCount; }
        public Vector2Int CountRange { get => countRange; }
    }
}