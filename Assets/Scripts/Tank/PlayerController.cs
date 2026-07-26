using UnityEngine;
using UnityEngine.InputSystem;

namespace BattleCity
{
    /// <summary>
    /// Player input → TankMotor + firing. Input actions are created entirely in code
    /// (WASD/arrows to move, Space/Enter to fire) — no .inputactions asset dependency.
    /// Classic rule: only one player bullet alive at a time.
    /// </summary>
    [RequireComponent(typeof(TankMotor))]
    public class PlayerController : MonoBehaviour
    {
        public float FireCooldown = 0.4f;

        TankMotor motor;
        InputAction moveAction;
        InputAction fireAction;
        float cooldownTimer;
        GameObject activeBullet;

        void Awake()
        {
            motor = GetComponent<TankMotor>();

            moveAction = new InputAction("Move", InputActionType.Value);
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");

            fireAction = new InputAction("Fire", InputActionType.Button, "<Keyboard>/space");
            fireAction.AddBinding("<Keyboard>/enter");
        }

        void OnEnable()
        {
            moveAction.Enable();
            fireAction.Enable();
        }

        void OnDisable()
        {
            moveAction.Disable();
            fireAction.Disable();
        }

        void OnDestroy()
        {
            moveAction.Dispose();
            fireAction.Dispose();
        }

        void Update()
        {
            motor.SetDirection(moveAction.ReadValue<Vector2>());

            cooldownTimer -= Time.deltaTime;
            if (fireAction.IsPressed() && cooldownTimer <= 0f && activeBullet == null)
            {
                activeBullet = ProjectileFactory.Create(transform.position, motor.Facing, fromPlayer: true);
                cooldownTimer = FireCooldown;
            }
        }
    }
}
