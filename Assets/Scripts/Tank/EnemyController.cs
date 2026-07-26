using UnityEngine;

namespace BattleCity
{
    /// <summary>
    /// Simple classic-style enemy AI: drive in a cardinal direction, pick a new random
    /// direction every few seconds or on collision, shoot on a timer.
    /// </summary>
    [RequireComponent(typeof(TankMotor))]
    public class EnemyController : MonoBehaviour
    {
        public float FireInterval = 2f;

        static readonly Vector2[] Directions =
        {
            Vector2.up, Vector2.down, Vector2.left, Vector2.right,
        };

        TankMotor motor;
        Vector2 currentDirection;
        float directionTimer;
        float fireTimer;

        void Awake()
        {
            motor = GetComponent<TankMotor>();
        }

        void Start()
        {
            // Bias the first move downward, toward the player's side of the map.
            currentDirection = Vector2.down;
            directionTimer = Random.Range(1.5f, 3f);
            fireTimer = FireInterval * Random.Range(0.5f, 1.5f);
        }

        void Update()
        {
            directionTimer -= Time.deltaTime;
            if (directionTimer <= 0f)
                PickNewDirection();

            motor.SetDirection(currentDirection);

            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0f)
            {
                ProjectileFactory.Create(transform.position, motor.Facing, fromPlayer: false);
                fireTimer = FireInterval * Random.Range(0.7f, 1.3f);
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            PickNewDirection();
        }

        void PickNewDirection()
        {
            currentDirection = Directions[Random.Range(0, Directions.Length)];
            directionTimer = Random.Range(1.5f, 4f);
        }
    }
}
