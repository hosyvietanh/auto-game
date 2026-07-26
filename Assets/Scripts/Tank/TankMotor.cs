using UnityEngine;

namespace BattleCity
{
    /// <summary>
    /// Shared 4-direction tank movement. Input (player or AI) calls SetDirection each
    /// frame; movement happens through Rigidbody2D velocity so walls block naturally.
    /// PlayMode tests drive this directly — never simulate input in tests.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class TankMotor : MonoBehaviour
    {
        public float Speed = 3f;

        /// <summary>Last non-zero direction; used as the fire direction. Starts facing up.</summary>
        public Vector2 Facing { get; private set; } = Vector2.up;

        Vector2 moveInput;
        Rigidbody2D rb;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void SetDirection(Vector2 dir)
        {
            if (dir.sqrMagnitude < 0.01f)
            {
                moveInput = Vector2.zero;
                return;
            }

            // Snap to the dominant cardinal axis (classic Battle City has no diagonals).
            Vector2 snapped = Mathf.Abs(dir.x) >= Mathf.Abs(dir.y)
                ? new Vector2(Mathf.Sign(dir.x), 0f)
                : new Vector2(0f, Mathf.Sign(dir.y));

            moveInput = snapped;
            Facing = snapped;
            rb.rotation = Vector2.SignedAngle(Vector2.up, snapped);
        }

        void FixedUpdate()
        {
            rb.linearVelocity = moveInput * Speed;
        }
    }
}
