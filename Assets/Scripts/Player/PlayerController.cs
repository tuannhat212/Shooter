using UnityEngine;

namespace ArenaSurvivor.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;

        [Header("Components")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;

        private Vector2 movementInput;
        private Camera mainCamera;
        private static readonly int SpeedHash = Animator.StringToHash("Speed");

        private void Awake()
        {
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (animator == null) animator = GetComponent<Animator>();
            mainCamera = Camera.main;
        }

        private void Update()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            // Chuẩn hóa vector
            movementInput = new Vector2(horizontal, vertical).normalized;
            if (mainCamera == null) return;

            Vector3 mouseScreenPosition = Input.mousePosition;
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
            Vector2 aimDirection = (Vector2)(mouseWorldPosition - transform.position);

            if (aimDirection.sqrMagnitude < 0.001f) return;

            float targetAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);

            HandleSpriteFlipping(targetAngle);
            UpdateAnimation();
        }

        private void FixedUpdate()
        {
            MovePlayer();
        }
        

        private void MovePlayer()
        {
            rb.linearVelocity = movementInput * moveSpeed;
        }
        

        private void HandleSpriteFlipping(float angle)
        {
            if (spriteRenderer == null) return;
            bool isLookingLeft = angle > 90f || angle < -90f;
            spriteRenderer.flipY = isLookingLeft;
        }

        private void UpdateAnimation()
        {
            if (animator == null) return;
            animator.SetFloat(SpeedHash, movementInput.sqrMagnitude);
        }
    }
}
