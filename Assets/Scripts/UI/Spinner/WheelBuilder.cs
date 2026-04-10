using Scripts.UI.Spinner.Data;
using UnityEngine;

namespace Scripts.UI.Spinner
{
    public class WheelBuilder : MonoBehaviour
    {
        [SerializeField] private Transform slotsContainer;

        private SlotView[] slots;

        public int SlotCount => slots != null ? slots.Length : 0;

        private void Awake()
        {
            slots = slotsContainer.GetComponentsInChildren<SlotView>();
        }

        public void Build(SlotItemConfig[] items)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (i < items.Length)
                {
                    slots[i].gameObject.SetActive(true);
                    slots[i].Set(items[i]);
                }
                else
                {
                    slots[i].gameObject.SetActive(false);
                }
            }
        }

        public void Clear()
        {
            foreach (var slot in slots)
            {
                slot.Clear();
            }
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            slotsContainer ??= transform;
        }
#endif
    }
}
