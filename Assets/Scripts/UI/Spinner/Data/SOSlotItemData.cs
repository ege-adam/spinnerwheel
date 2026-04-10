using TriInspector;
using UnityEngine;

namespace Scripts.UI.Spinner.Data
{
    public enum SlotItemType
    {
        Weapon = 100,
        Coin = 200,
        Gem = 300,
        Chest = 400,
        Bomb = 500
    }

    [System.Serializable]
    [CreateAssetMenu(fileName = "SOSlotItemData", menuName = "ScriptableObjects/UI/Spinner/Slots/SlotItemData", order = 0)]
    public class SOSlotItemData : ScriptableObject
    {
        [SerializeField] private Sprite icon;
        [SerializeField] private string itemName;
        [SerializeField] private SlotItemType itemType;
        [SerializeField, ReadOnly] private string id;

        public Sprite Icon { get => icon; }
        public string ItemName { get => itemName; }
        public SlotItemType ItemType { get => itemType; }
        public string Id { get => id; }

        private void Reset()
        {
            if (string.IsNullOrEmpty(id))
                id = System.Guid.NewGuid().ToString();
        }
    }
}