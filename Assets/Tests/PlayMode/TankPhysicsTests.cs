using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using BattleCity;

namespace BattleCity.PlayModeTests
{
    /// <summary>
    /// Physics-level integration tests. These drive TankMotor directly — Input System
    /// key simulation does not work reliably in headless batch mode (see CLAUDE.md).
    /// PlayerController is disabled so its (empty) input doesn't zero the motor.
    /// </summary>
    public class TankPhysicsTests
    {
        [SetUp]
        public void SetUp()
        {
            LayerConfig.Setup();
        }

        [UnityTest]
        public IEnumerator Tank_MovesWhenDriven()
        {
            var tank = TankFactory.CreatePlayer(Vector2.zero);
            tank.GetComponent<PlayerController>().enabled = false;
            tank.GetComponent<TankMotor>().SetDirection(Vector2.right);

            yield return new WaitForSeconds(0.5f);

            Assert.That(tank.transform.position.x, Is.GreaterThan(0.5f), "tank should have driven right");
            Object.Destroy(tank);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Tank_IsBlockedBySteelWall()
        {
            var steel = TileFactory.CreateSteel(new Vector2(2f, 0f), null);
            var tank = TankFactory.CreatePlayer(Vector2.zero);
            tank.GetComponent<PlayerController>().enabled = false;
            tank.GetComponent<TankMotor>().SetDirection(Vector2.right);

            yield return new WaitForSeconds(1.5f);

            // Unblocked, the tank would travel >5 units. Steel front face is at x=1.5.
            Assert.That(tank.transform.position.x, Is.GreaterThan(0.5f), "tank should have moved at all");
            Assert.That(tank.transform.position.x, Is.LessThan(1.4f), "steel wall must stop the tank");
            Object.Destroy(tank);
            Object.Destroy(steel);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Bullet_DestroysBrick_ButNotSteel()
        {
            var brick = TileFactory.CreateBrick(new Vector2(2f, 0f), null);
            var steel = TileFactory.CreateSteel(new Vector2(2f, 3f), null);

            ProjectileFactory.Create(new Vector2(0f, 0f), Vector2.right, fromPlayer: true);
            ProjectileFactory.Create(new Vector2(0f, 3f), Vector2.right, fromPlayer: true);

            yield return new WaitForSeconds(1f);

            Assert.That(brick == null, Is.True, "brick should be destroyed by the bullet");
            Assert.That(steel != null, Is.True, "steel must survive bullets");
            Object.Destroy(steel);
            yield return null;
        }
        [UnityTest]
        public IEnumerator Tank_SlightlyOffGrid_DrivesThroughCorridorWithoutSnagging()
        {
            // A 1-unit corridor at y=0 bounded by steel walls above and below at x=3.
            TileFactory.CreateSteel(new Vector2(3f, 1f), null);
            TileFactory.CreateSteel(new Vector2(3f, -1f), null);

            // Start the tank noticeably off the lane center so its corner would catch
            // the walls without auto-alignment.
            var tank = TankFactory.CreatePlayer(new Vector2(0f, 0.2f));
            tank.GetComponent<PlayerController>().enabled = false;
            tank.GetComponent<TankMotor>().SetDirection(Vector2.right);

            yield return new WaitForSeconds(1.5f);

            Assert.That(tank.transform.position.x, Is.GreaterThan(4f),
                "tank should auto-align and drive past the corridor walls, not snag on a corner");
            Object.Destroy(tank);
            yield return null;
        }
    }
}
