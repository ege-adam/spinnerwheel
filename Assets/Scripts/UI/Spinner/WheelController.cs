using System;
using DG.Tweening;
using Solo.MOST_IN_ONE;
using UnityEngine;

namespace Scripts.UI.Spinner
{
    public class WheelController : MonoBehaviour
    {
        [SerializeField] private Transform wheelTransform;
        [SerializeField] private float friction = 0.97f;
        [SerializeField] private float snapThreshold = 30f;
        [SerializeField] private float snapDuration = 0.5f;

        [Header("Audio")]
        [SerializeField] private AudioSource clickAudio;
        [SerializeField] private AudioClip clickClip;
        [SerializeField] private float minPitch = 0.6f;
        [SerializeField] private float maxPitch = 1.4f;
        [SerializeField] private float maxSpeedReference = 500f;

        [Header("Needle")]
        [SerializeField] private Transform needleTransform;
        [SerializeField] private float needleTiltAngle = 15f;
        [SerializeField] private float needleTiltDuration = 0.15f;

        public event Action<int> OnSpinComplete;

        private float currentSpeed;
        private bool isSpinning;
        private bool isSnapping;
        private int sliceCount;
        private int lastSliceIndex = -1;

        public bool IsActive => isSpinning || isSnapping;

        public void Setup(int sliceCount)
        {
            this.sliceCount = sliceCount;
            Reset();
        }

        public void ApplyFlick(float flickSpeed)
        {
            if (IsActive) return;
            if (sliceCount <= 0) return;

            currentSpeed = flickSpeed + (flickSpeed * UnityEngine.Random.Range(0.3f, 2f)); // add some random variation to prevent cheating by flicking at the exact same speed repeatedly
            currentSpeed += Mathf.Sign(flickSpeed) * snapThreshold * UnityEngine.Random.Range(10f, 50f); // ensure it always exceeds snap threshold for a satisfying spin

            isSpinning = true;
            lastSliceIndex = GetCurrentSliceIndex();
        }

        public void Reset()
        {
            wheelTransform.DOKill();
            currentSpeed = 0f;
            isSpinning = false;
            isSnapping = false;
            lastSliceIndex = -1;
        }

        private void Update()
        {
            if (!isSpinning) return;

            currentSpeed *= friction;
            wheelTransform.Rotate(0, 0, currentSpeed * Time.deltaTime);

            HandleClickSFXAndNeedle();

            if (Mathf.Abs(currentSpeed) < snapThreshold && !isSnapping)
            {
                isSnapping = true;
                isSpinning = false;
                SnapToSlice();
            }
        }

        private void SnapToSlice()
        {
            int rawIndex = GetCurrentSliceIndex();
            float sliceAngle = 360f / sliceCount;
            float targetAngle = rawIndex * sliceAngle;

            wheelTransform
                .DORotate(new Vector3(0, 0, targetAngle), snapDuration)
                .SetEase(Ease.OutBounce)
                .OnComplete(() =>
                {
                    isSnapping = false;
                    int rewardIndex = (sliceCount - rawIndex) % sliceCount;
                    MOST_HapticFeedback.Generate(MOST_HapticFeedback.HapticTypes.MediumImpact);
                    OnSpinComplete?.Invoke(rewardIndex);
                });
        }

        private void HandleClickSFXAndNeedle()
        {
            if (clickAudio == null || clickClip == null) return;

            int currentIndex = GetCurrentSliceIndex();

            if (currentIndex != lastSliceIndex)
            {
                lastSliceIndex = currentIndex;

                float speedNormalized = Mathf.Clamp01(Mathf.Abs(currentSpeed) / maxSpeedReference);
                clickAudio.pitch = Mathf.Lerp(minPitch, maxPitch, speedNormalized);
                clickAudio.PlayOneShot(clickClip);
                MOST_HapticFeedback.GenerateWithCooldown(MOST_HapticFeedback.HapticTypes.SoftImpact, 0.05f);

                if (needleTransform != null)
                {
                    needleTransform.DOKill();
                    float tilt = needleTiltAngle * Mathf.Sign(currentSpeed) * speedNormalized;
                    needleTransform.localRotation = Quaternion.Euler(0, 0, tilt);
                    needleTransform
                        .DOLocalRotate(Vector3.zero, needleTiltDuration)
                        .SetEase(Ease.OutElastic);
                }
            }
        }

        private int GetCurrentSliceIndex()
        {
            float angle = wheelTransform.eulerAngles.z % 360f;
            float sliceAngle = 360f / sliceCount;
            return Mathf.FloorToInt(angle / sliceAngle);
        }

        private void OnDisable()
        {
            wheelTransform.DOKill();
        }
    }
}
