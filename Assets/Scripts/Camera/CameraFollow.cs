using UnityEngine;

namespace ArenaSurvivor.CameraControl
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target & Smooth Settings")]
        [SerializeField] private Transform target;
        [SerializeField] private float smoothSpeed = 5f;
        [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

        [Header("Bounds (Confiner)")]
        [SerializeField] private bool useBounds = true;
        [SerializeField] private Vector2 minBounds = new Vector2(-15f, -10f);
        [SerializeField] private Vector2 maxBounds = new Vector2(15f, 10f);

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        public void SetBounds(Vector2 min, Vector2 max)
        {
            minBounds = min;
            maxBounds = max;
            useBounds = true;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            if (useBounds)
            {
                smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minBounds.x, maxBounds.x);
                smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minBounds.y, maxBounds.y);
            }

            transform.position = smoothedPosition;
        }
    }
}
