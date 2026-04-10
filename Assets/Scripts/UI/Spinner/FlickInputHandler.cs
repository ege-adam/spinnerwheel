using System;
using UnityEngine;

namespace Scripts.UI.Spinner
{
    public class FlickInputHandler : MonoBehaviour
    {
        [SerializeField] private float minFlickSpeed = 50f;
        [SerializeField] private float maxFlickSpeed = 2000f;
        [SerializeField] private float flickMultiplier = 1f;

        public event Action<float> OnFlick;

        private bool isDragging;
        private Vector2 lastPosition;
        private float lastAngle;
        private float angularVelocity;

        [SerializeField] private Transform wheelCenter;

        private void Update()
        {
            if (GetInputDown())
            {
                isDragging = true;
                lastPosition = GetInputPosition();
                lastAngle = GetAngleFromCenter(lastPosition);
                angularVelocity = 0f;
            }

            if (isDragging && GetInputHeld())
            {
                Vector2 currentPosition = GetInputPosition();
                float currentAngle = GetAngleFromCenter(currentPosition);
                float delta = Mathf.DeltaAngle(lastAngle, currentAngle);

                angularVelocity = delta / Time.deltaTime;

                lastPosition = currentPosition;
                lastAngle = currentAngle;
            }

            if (isDragging && GetInputUp())
            {
                isDragging = false;

                float speed = Mathf.Abs(angularVelocity) * flickMultiplier;

                if (speed >= minFlickSpeed)
                {
                    float clampedSpeed = Mathf.Clamp(speed, minFlickSpeed, maxFlickSpeed);
                    float direction = Mathf.Sign(angularVelocity);
                    OnFlick?.Invoke(clampedSpeed * direction);
                }

                angularVelocity = 0f;
            }
        }

        private float GetAngleFromCenter(Vector2 screenPosition)
        {
            Vector2 center = RectTransformUtility.WorldToScreenPoint(null, wheelCenter.position);
            Vector2 direction = screenPosition - center;
            return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }

        private bool GetInputDown()
        {
            if (Input.touchCount > 0)
                return Input.GetTouch(0).phase == TouchPhase.Began;
            return Input.GetMouseButtonDown(0);
        }

        private bool GetInputHeld()
        {
            if (Input.touchCount > 0)
            {
                var phase = Input.GetTouch(0).phase;
                return phase == TouchPhase.Moved || phase == TouchPhase.Stationary;
            }
            return Input.GetMouseButton(0);
        }

        private bool GetInputUp()
        {
            if (Input.touchCount > 0)
                return Input.GetTouch(0).phase == TouchPhase.Ended;
            return Input.GetMouseButtonUp(0);
        }

        private Vector2 GetInputPosition()
        {
            if (Input.touchCount > 0)
                return Input.GetTouch(0).position;
            return Input.mousePosition;
        }
    }
}
