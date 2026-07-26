using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using BattleCity;

namespace BattleCity.PlayModeTests
{
    /// <summary>
    /// PROOF that Input System key simulation DOES work headless — contrary to the old
    /// CLAUDE.md note. The trick is <see cref="InputTestFixture"/>: its [SetUp] swaps the
    /// native OS input runtime for a mocked InputTestRuntime (no hardware, no display) and
    /// ties it into the player loop so [UnityTest] frames process queued input.
    ///
    /// Unlike the other PlayMode tests, these keep PlayerController ENABLED and drive it
    /// through synthesized keyboard events — so they verify the actual bindings
    /// (WASD/arrows → move, Space → fire), which direct TankMotor tests cannot.
    /// </summary>
    public class InputBindingTests : InputTestFixture
    {
        Keyboard keyboard;

        public override void Setup()
        {
            base.Setup();          // installs the mocked InputTestRuntime
            LayerConfig.Setup();
            keyboard = InputSystem.AddDevice<Keyboard>();
        }

        [UnityTest]
        public IEnumerator PressingD_DrivesTankRight()
        {
            var tank = TankFactory.CreatePlayer(Vector2.zero);   // PlayerController stays enabled
            yield return null;                                   // let Awake/OnEnable resolve bindings

            Press(keyboard.dKey);
            yield return new WaitForSeconds(0.5f);
            Release(keyboard.dKey);

            var motor = tank.GetComponent<TankMotor>();
            Assert.That(motor.Facing, Is.EqualTo(Vector2.right), "D should face the tank right");
            Assert.That(tank.transform.position.x, Is.GreaterThan(0.5f), "D should drive the tank right");

            Object.Destroy(tank);
            yield return null;
        }

        [UnityTest]
        public IEnumerator PressingSpace_FiresABullet()
        {
            var tank = TankFactory.CreatePlayer(Vector2.zero);
            yield return null;

            Assert.That(Object.FindObjectsByType<Projectile>(FindObjectsSortMode.None).Length,
                Is.EqualTo(0), "no bullet before firing");

            // Hold space DOWN across a frame — PlayerController.Update samples IsPressed(),
            // so a same-frame press+release would be missed.
            Press(keyboard.spaceKey);
            yield return null;
            yield return null;
            Release(keyboard.spaceKey);

            Assert.That(Object.FindObjectsByType<Projectile>(FindObjectsSortMode.None).Length,
                Is.GreaterThanOrEqualTo(1), "Space should spawn a player bullet");

            Object.Destroy(tank);
            yield return null;
        }
    }
}
