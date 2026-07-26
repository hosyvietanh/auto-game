using UnityEngine;

namespace BattleCity
{
    public static class ProjectileFactory
    {
        const int SortBullets = 30;
        const float BulletSpeed = 8f;
        const float BulletWorldSize = 0.2f;
        const float SpawnOffset = 0.65f;
        const float MaxLifetime = 5f;

        public static GameObject Create(Vector2 origin, Vector2 direction, bool fromPlayer)
        {
            var dir = direction.sqrMagnitude < 0.01f ? Vector2.up : direction.normalized;

            var go = new GameObject(fromPlayer ? "PlayerBullet" : "EnemyBullet");
            go.transform.position = origin + dir * SpawnOffset;
            go.layer = fromPlayer ? LayerConfig.PlayerBullet : LayerConfig.EnemyBullet;

            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = ArtRegistry.Load(ArtRegistry.Names.Bullet, Color.white);
            renderer.sortingOrder = SortBullets;
            ArtRegistry.SetWorldSize(renderer, BulletWorldSize);

            var rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.linearVelocity = dir * BulletSpeed;

            var collider = go.AddComponent<CircleCollider2D>();
            float scale = go.transform.localScale.x;
            collider.radius = (BulletWorldSize * 0.5f) / (scale > 0f ? scale : 1f);

            go.AddComponent<Projectile>();
            Object.Destroy(go, MaxLifetime);
            return go;
        }
    }
}
