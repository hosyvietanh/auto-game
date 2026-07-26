using UnityEngine;

namespace BattleCity
{
    /// <summary>Builds complete tank GameObjects from code (no prefabs).</summary>
    public static class TankFactory
    {
        const int SortTanks = 20;
        const float TankVisualSize = 0.85f;
        const float TankColliderSize = 0.8f;

        public static GameObject CreatePlayer(Vector2 pos)
        {
            var data = TankData.For(TankType.Player);
            var go = CreateTank("PlayerTank", pos, data,
                ArtRegistry.Load(ArtRegistry.Names.PlayerTank, new Color(0.2f, 0.8f, 0.2f)),
                LayerConfig.PlayerTank);
            var controller = go.AddComponent<PlayerController>();
            controller.FireCooldown = data.FireInterval;
            return go;
        }

        public static GameObject CreateEnemy(Vector2 pos, TankType type)
        {
            var data = TankData.For(type);
            string spriteName;
            Color fallback;
            switch (type)
            {
                case TankType.FastEnemy:
                    spriteName = ArtRegistry.Names.EnemyFast;
                    fallback = new Color(0.9f, 0.3f, 0.2f);
                    break;
                case TankType.ArmoredEnemy:
                    spriteName = ArtRegistry.Names.EnemyArmored;
                    fallback = new Color(0.4f, 0.4f, 0.45f);
                    break;
                default:
                    spriteName = ArtRegistry.Names.EnemyBasic;
                    fallback = new Color(0.85f, 0.75f, 0.4f);
                    break;
            }

            var go = CreateTank($"Enemy_{type}", pos, data,
                ArtRegistry.Load(spriteName, fallback), LayerConfig.EnemyTank);
            var ai = go.AddComponent<EnemyController>();
            ai.FireInterval = data.FireInterval;
            return go;
        }

        static GameObject CreateTank(string name, Vector2 pos, TankData data, Sprite sprite, int layer)
        {
            var go = new GameObject(name);
            go.transform.position = pos;
            go.layer = layer;

            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = SortTanks;
            ArtRegistry.SetWorldSize(renderer, TankVisualSize);

            var rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            // Physics must never spin the tank; TankMotor sets rb.rotation directly,
            // which still works with frozen rotation.
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;

            var collider = go.AddComponent<BoxCollider2D>();
            float scale = go.transform.localScale.x;
            collider.size = Vector2.one * (TankColliderSize / (scale > 0f ? scale : 1f));

            var motor = go.AddComponent<TankMotor>();
            motor.Speed = data.Speed;

            var destructible = go.AddComponent<Destructible>();
            destructible.Health = data.Health;

            return go;
        }
    }
}
