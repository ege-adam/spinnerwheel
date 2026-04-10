using System;
using System.Collections.Generic;
using Scripts.UI.Spinner.Data;
using Scripts.UI.Spinner.VO;
using Solo.MOST_IN_ONE;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI.Spinner
{
    public class SpinnerManager : MonoBehaviour
    {
        [SerializeField] private SOGameConfig gameConfig;
        [SerializeField] private DataLoader dataLoader;
        [SerializeField] private WheelController wheelController;
        [SerializeField] private WheelBuilder wheelBuilder;
        [SerializeField] private FlickInputHandler flickInput;

        [Header("Wheel Visuals")]
        [SerializeField] private Image wheelImage;
        [SerializeField] private Image needleImage;

        public event Action OnReady;
        public event Action<SlotItemConfig> OnRewardEarned;
        public event Action OnBombHit;
        public event Action<int, SOSpinner> OnZoneChanged;
        public event Action<List<RewardEntry>> OnCollect;
        public event Action OnRevive;
        public event Action<bool> OnLeaveAvailabilityChanged;

        private readonly RewardInventory inventory = new();
        private SpinnerState state;
        private int currentZone;
        private SlotItemConfig[] currentSlots;
        private SOSpinner currentSpinner;

        public bool IsReady { get; private set; }
        public SOGameConfig GameConfig => gameConfig;
        public IRewardInventory Inventory => inventory;
        public SpinnerState State => state;
        public bool CanLeave => currentSpinner != null &&
            currentSpinner.SpinnerType != SpinnerType.Normal;

        private void Start()
        {
            Invoke(nameof(LateStart), 0.1f); // slight delay to ensure all references are set
        }

        private void LateStart()
        {
            LoadZone(currentZone);
            IsReady = true;
            OnReady?.Invoke();
        }

        private void OnEnable()
        {
            flickInput.OnFlick += OnFlick;
            wheelController.OnSpinComplete += HandleSpinResult;
        }

        private void OnDisable()
        {
            flickInput.OnFlick -= OnFlick;
            wheelController.OnSpinComplete -= HandleSpinResult;
        }

        private void OnFlick(float speed)
        {
            if (state != SpinnerState.Idle) return;
            state = SpinnerState.Spinning;
            wheelController.ApplyFlick(speed);
        }

        private void LoadZone(int zone)
        {
            if (zone >= dataLoader.LevelCount)
            {
                zone = 0;
                currentZone = 0;
            }

            currentSlots = dataLoader.LoadSlotsForZone(zone, wheelBuilder.SlotCount);
            currentSpinner = dataLoader.GetSpinnerForZone(zone);

            wheelController.Setup(currentSlots.Length);
            wheelBuilder.Build(currentSlots);
            ApplyWheelSkin(currentSpinner);

            OnZoneChanged?.Invoke(zone, currentSpinner);
            OnLeaveAvailabilityChanged?.Invoke(CanLeave);
            state = SpinnerState.Idle;
        }

        private void ApplyWheelSkin(SOSpinner spinner)
        {
            if (wheelImage != null && spinner.SpinnerSprite != null)
                wheelImage.sprite = spinner.SpinnerSprite;

            if (needleImage != null && spinner.NeedleSprite != null)
                needleImage.sprite = spinner.NeedleSprite;
        }

        private void HandleSpinResult(int sliceIndex)
        {
            var result = currentSlots[sliceIndex];

            if (result.ItemData.ItemType == SlotItemType.Bomb)
            {
                inventory.StashAndClear();
                state = SpinnerState.GameOver;
                MOST_HapticFeedback.Generate(MOST_HapticFeedback.HapticTypes.Failure);
                OnBombHit?.Invoke();
                return;
            }

            inventory.Add(result.ItemData, result.Count);
            OnRewardEarned?.Invoke(result);
            currentZone++;

            state = SpinnerState.Result;
        }

        public void RequestContinue()
        {
            if (state != SpinnerState.Result) return;
            LoadZone(currentZone);
        }

        public void RequestCollect()
        {
            if (state != SpinnerState.Result && state != SpinnerState.Idle) return;
            if (!CanLeave) return;

            var rewards = new List<RewardEntry>(inventory.CollectedRewards);
            MOST_HapticFeedback.Generate(MOST_HapticFeedback.HapticTypes.Success);
            OnCollect?.Invoke(rewards);
            inventory.Clear();
            currentZone = 0;
            LoadZone(currentZone);
        }

        public void RequestRestart()
        {
            if (state != SpinnerState.GameOver) return;
            currentZone = 0;
            inventory.ClearAll();
            LoadZone(currentZone);
        }

        public void RequestRevive()
        {
            if (state != SpinnerState.GameOver) return;

            inventory.RestoreFromStash();
            OnRevive?.Invoke();
            LoadZone(currentZone);
        }
    }
}
