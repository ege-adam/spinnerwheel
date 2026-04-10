using Scripts.UI.Spinner.Data;
using TMPro;
using UnityEngine;

namespace Scripts.UI.Spinner
{
    public class SpinnerUIController : MonoBehaviour
    {
        [SerializeField] private SpinnerManager spinnerManager;

        [Header("Info")]
        [SerializeField] private TMP_Text topText;
        [SerializeField] private TMP_Text bottomText;
        [SerializeField] private TMP_Text zoneText;

        private void OnEnable()
        {
            spinnerManager.OnZoneChanged += HandleZoneChanged;
        }

        private void OnDisable()
        {
            spinnerManager.OnZoneChanged -= HandleZoneChanged;
        }

        private void HandleZoneChanged(int zone, SOSpinner spinner)
        {
            if (topText != null)
                topText.text = spinner.SpinnerTopText;

            if (bottomText != null)
                bottomText.text = spinner.SpinnerBottomText;

            if (zoneText != null)
                zoneText.text = $"Zone {zone + 1}";
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            spinnerManager ??= GetComponentInParent<SpinnerManager>();
        }
#endif
    }
}
