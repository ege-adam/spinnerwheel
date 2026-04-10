using Scripts.UI.Spinner.Data;
using Solo.MOST_IN_ONE;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI.Spinner
{
    public class LoseScreenController : MonoBehaviour
    {
        [SerializeField] private SpinnerManager spinnerManager;
        [SerializeField] private CurrencyManager currencyManager;

        [SerializeField] private GameObject panel;
        [SerializeField] private Transform animRoot;
        [SerializeField] private Button giveUpButton;
        [SerializeField] private Button reviveCoinsButton;
        [SerializeField] private TMP_Text reviveCostText;

        private SOGameConfig GameConfig => spinnerManager.GameConfig;

        private void Start()
        {
            panel.SetActive(false);
            UpdateReviveCostText();
        }

        private void OnEnable()
        {
            spinnerManager.OnBombHit += Show;
            giveUpButton.onClick.AddListener(OnGiveUp);
            reviveCoinsButton.onClick.AddListener(OnReviveWithCoins);
        }

        private void OnDisable()
        {
            spinnerManager.OnBombHit -= Show;
            giveUpButton.onClick.RemoveListener(OnGiveUp);
            reviveCoinsButton.onClick.RemoveListener(OnReviveWithCoins);
        }

        private void Show()
        {
            UpdateReviveCostText();
            reviveCoinsButton.interactable = currencyManager.CanAfford(GameConfig.ReviveCost);
            PanelAnimator.Show(panel, animRoot);
        }

        private void Hide(System.Action onComplete = null)
        {
            PanelAnimator.Hide(panel, animRoot, onComplete);
        }

        private void OnGiveUp()
        {
            MOST_HapticFeedback.Generate(MOST_HapticFeedback.HapticTypes.Selection);
            Hide(() => spinnerManager.RequestRestart());
        }

        private void OnReviveWithCoins()
        {
            if (!currencyManager.TrySpend(GameConfig.ReviveCost)) return;

            MOST_HapticFeedback.Generate(MOST_HapticFeedback.HapticTypes.Success);
            Hide(() => spinnerManager.RequestRevive());
        }

        private void UpdateReviveCostText()
        {
            if (reviveCostText != null)
                reviveCostText.text = GameConfig.ReviveCost.ToString();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            spinnerManager ??= GetComponentInParent<SpinnerManager>();
            currencyManager ??= FindObjectOfType<CurrencyManager>();
            animRoot ??= panel != null ? panel.transform : null;

            if (panel != null)
            {
                giveUpButton ??= panel.transform.Find("ui_lose_button_revive")?.GetComponent<Button>();
                reviveCoinsButton ??= panel.transform.Find("ui_lose_button_revive")?.GetComponent<Button>();
                reviveCostText ??= panel.transform.Find("ui_lose_button_revive_text_value")?.GetComponent<TMP_Text>();
            }
        }
#endif
    }
}
