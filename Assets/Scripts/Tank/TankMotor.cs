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
            Vector2 velocity = moveInput * Speed;

            // Auto-align to the lane on the perpendicular axis. Tanks are narrower than a
            // 1-unit corridor, so without this a slightly-off-grid tank snags on wall
            // corners and stops. Nudging the off-axis toward the nearest grid line keeps
            // the tank centered while driving (classic Battle City behavior).
            if (moveInput.x != 0f)
                velocity.y = AlignVelocity(rb.position.y);
            else if (moveInput.y != 0f)
                velocity.x = AlignVelocity(rb.position.x);

            rb.linearVelocity = velocity;
        }

        /// <summary>Velocity that moves a coordinate toward the nearest integer grid line
        /// without overshooting in one physics step.</summary>
        float AlignVelocity(float current)
        {
            float delta = Mathf.Round(current) - current;
            if (Mathf.Abs(delta) < 0.001f)
                return 0f;
            return Mathf.Clamp(delta / Time.fixedDeltaTime, -Speed, Speed);
        }
    }
}
