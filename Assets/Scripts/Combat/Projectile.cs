using UnityEngine;

namespace BattleCity
{
    /// <summary>
    /// Bullet behavior: flies straight (velocity set by the factory), damages the first
    /// Destructible it hits, dies on any collision. Lifetime capped by the factory.
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        public int Damage = 1;

        void OnCollisionEnter2D(Collision2D collision)
        {
            var destructible = collision.collider.GetComponent<Destructible>();
            if (destructible != null)
                destructible.TakeDamage(Damage);

            Destroy(gameObject);
        }
    }
}
