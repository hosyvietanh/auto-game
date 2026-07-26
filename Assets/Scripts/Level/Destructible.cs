using System;
using UnityEngine;

namespace BattleCity
{
    /// <summary>Anything bullets can destroy: brick walls, tanks, the eagle.</summary>
    public class Destructible : MonoBehaviour
    {
        public int Health = 1;

        /// <summary>Raised right before the GameObject is destroyed.</summary>
        public event Action<Destructible> Destroyed;

        public void TakeDamage(int amount)
        {
            if (Health <= 0)
                return;

            Health -= amount;
            if (Health <= 0)
            {
                Destroyed?.Invoke(this);
                Destroy(gameObject);
            }
        }
    }
}
