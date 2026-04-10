using Scripts.UI.Spinner.Data;
using Solo.MOST_IN_ONE;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI.Spinner
{
    public class WinScreenController : MonoBehaviour
    {
        [SerializeField] private SpinnerManager spinnerManager;

        [SerializeField] private GameObject panel;
        [SerializeField] private Transform animRoot;
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text countText;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button collectButton;

        private void Start()
        {
            panel.SetActive(false);
        }

        private void OnEnable()
        {
            spinnerManager.OnRewardEarned += Show;
            spinnerManager.OnLeaveAvailabilityChanged += HandleLeaveAvailability;
            continueButton.onClick.AddListener(OnContinue);
            collectButton.onClick.AddListener(OnCollect);
        }

        private void OnDisable()
        {
            spinnerManager.OnRewardEarned -= Show;
            spinnerManager.OnLeaveAvailabilityChanged -= HandleLeaveAvailability;
            continueButton.onClick.RemoveListener(OnContinue);
            collectButton.onClick.RemoveListener(OnCollect);
        }

        private void Show(SlotItemConfig config)
        {
            icon.sprite = config.ItemData.Icon;
            nameText.text = config.ItemData.ItemName;
            countText.text = $"x{config.Count}";

            collectButton.gameObject.SetActive(spinnerManager.CanLeave);
            PanelAnimator.Show(panel, animRoot);
        }

        private void HandleLeaveAvailability(bool canLeave)
        {
            collectButton.gameObject.SetActive(canLeave);
        }

        private void OnContinue()
        {
            MOST_HapticFeedback.Generate(MOST_HapticFeedback.HapticTypes.Selection);
            PanelAnimator.Hide(panel, animRoot, () => spinnerManager.RequestContinue());
        }

        private void OnCollect()
        {
            MOST_HapticFeedback.Generate(MOST_HapticFeedback.HapticTypes.Selection);
            PanelAnimator.Hide(panel, animRoot, () => spinnerManager.RequestCollect());
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            spinnerManager ??= GetComponentInParent<SpinnerManager>();
            animRoot ??= panel != null ? panel.transform : null;
        }
#endif
    }
}
