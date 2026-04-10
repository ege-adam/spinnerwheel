using Scripts.UI.Spinner.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI.Spinner
{
    public class SlotView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text countText;

        public void Set(SlotItemConfig config)
        {
            icon.sprite = config.ItemData.Icon;
            countText.text = config.Count > 1 ? $"x{config.Count}" : "";
        }

        public void Clear()
        {
            icon.sprite = null;
            countText.text = "";
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            icon ??= GetComponentInChildren<Image>();
            countText ??= GetComponentInChildren<TMP_Text>();
        }
#endif
    }
}