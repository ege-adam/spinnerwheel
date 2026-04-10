using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI.Spinner
{
    public class InventorySlotView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text countText;

        private string itemId;
        public string ItemId => itemId;

        public void Set(Sprite sprite, string id, string itemName, int count)
        {
            itemId = id;
            icon.sprite = sprite;
            if (nameText != null)
                nameText.text = itemName;
            countText.text = $"x{count}";
        }

        public void UpdateCount(int count)
        {
            countText.text = $"x{count}";
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            icon ??= GetComponentInChildren<Image>();
            var texts = GetComponentsInChildren<TMP_Text>();
            if (texts.Length >= 2)
            {
                nameText ??= texts[0];
                countText ??= texts[1];
            }
            else if (texts.Length == 1)
            {
                countText ??= texts[0];
            }
        }
#endif
    }
}
