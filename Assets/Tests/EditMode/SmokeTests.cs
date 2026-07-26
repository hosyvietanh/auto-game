using NUnit.Framework;

namespace BattleCity.EditModeTests
{
    /// <summary>Validates the EditMode test pipeline itself (M0 smoke test).</summary>
    public class SmokeTests
    {
        [Test]
        public void EditModeTestPipelineWorks()
        {
            Assert.That(1 + 1, Is.EqualTo(2));
        }
    }
}
