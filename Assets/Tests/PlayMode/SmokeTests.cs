using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace BattleCity.PlayModeTests
{
    /// <summary>Validates the PlayMode test pipeline itself (M0 smoke test).</summary>
    public class SmokeTests
    {
        [UnityTest]
        public IEnumerator PlayModeTestPipelineWorks()
        {
            var go = new GameObject("smoke");
            yield return null;
            Assert.That(go != null, Is.True);
            Object.Destroy(go);
        }
    }
}
