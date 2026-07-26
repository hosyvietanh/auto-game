using NUnit.Framework;
using BattleCity;

namespace BattleCity.EditModeTests
{
    /// <summary>
    /// Smoke tests for the procedural pixel-art generator: every known sprite name yields
    /// a sprite, unknown names fall through (null), and results are cached.
    /// </summary>
    public class NesArtTests
    {
        [TestCase(ArtRegistry.Names.PlayerTank)]
        [TestCase(ArtRegistry.Names.EnemyBasic)]
        [TestCase(ArtRegistry.Names.EnemyFast)]
        [TestCase(ArtRegistry.Names.EnemyArmored)]
        [TestCase(ArtRegistry.Names.Bullet)]
        [TestCase(ArtRegistry.Names.Brick)]
        [TestCase(ArtRegistry.Names.Steel)]
        [TestCase(ArtRegistry.Names.Eagle)]
        [TestCase(ArtRegistry.Names.Bush)]
        public void Get_ReturnsSprite_ForKnownNames(string name)
        {
            Assert.That(NesArt.Get(name), Is.Not.Null, name);
        }

        [Test]
        public void Get_ReturnsNull_ForUnknownName()
        {
            Assert.That(NesArt.Get("does_not_exist"), Is.Null);
        }

        [Test]
        public void Get_CachesSameInstance()
        {
            var a = NesArt.Get(ArtRegistry.Names.Brick);
            var b = NesArt.Get(ArtRegistry.Names.Brick);
            Assert.That(a, Is.SameAs(b));
        }
    }
}
