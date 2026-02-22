using UnityEngine;

namespace Controllers {
    public class PlayerController : MonoBehaviour {
        public float moveSpeed = 5f;
        public Rigidbody2D rb;
        public Animator animator;

        Vector2 movement;
        Vector2 lastDirection = Vector2.down;

        void Update() {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            if (movement != Vector2.zero) {
                lastDirection = movement.normalized;
            }

            // manda la direccion al animator para las animaciones
            if (animator != null) {
                animator.SetFloat("MoveX", movement.x);
                animator.SetFloat("MoveY", movement.y);
                animator.SetFloat("LastMoveX", lastDirection.x);
                animator.SetFloat("LastMoveY", lastDirection.y);
                animator.SetFloat("Speed", movement.sqrMagnitude);
            }
        }

        void FixedUpdate() {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }
}
