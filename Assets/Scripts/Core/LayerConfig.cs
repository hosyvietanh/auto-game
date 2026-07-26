using UnityEngine;

namespace BattleCity
{
    /// <summary>
    /// Physics layer indices used by the game. The names are written into
    /// ProjectSettings/TagManager.asset by the LayerSetup editor script; runtime code
    /// only ever uses these hard-coded ints. Collision rules live in Setup() —
    /// change them here, never in the editor UI.
    /// </summary>
    public static class LayerConfig
    {
        public const int PlayerTank = 8;
        public const int EnemyTank = 9;
        public const int PlayerBullet = 10;
        public const int EnemyBullet = 11;
        public const int BrickWall = 12;
        public const int SteelWall = 13;
        public const int Base = 14;

        public static void Setup()
        {
            Physics2D.gravity = Vector2.zero;

            // Bullets never hit their owner's side.
            Physics2D.IgnoreLayerCollision(PlayerBullet, PlayerTank, true);
            Physics2D.IgnoreLayerCollision(EnemyBullet, EnemyTank, true);

            // Bullets pass through each other (MVP simplification of bullet-cancel).
            Physics2D.IgnoreLayerCollision(PlayerBullet, PlayerBullet, true);
            Physics2D.IgnoreLayerCollision(EnemyBullet, EnemyBullet, true);
            Physics2D.IgnoreLayerCollision(PlayerBullet, EnemyBullet, true);

            // The player cannot destroy their own eagle.
            Physics2D.IgnoreLayerCollision(PlayerBullet, Base, true);
        }
    }
}
