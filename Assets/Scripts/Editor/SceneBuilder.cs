using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace BattleCity.EditorTools
{
    /// <summary>
    /// Generates the single, minimal Game.unity scene: an orthographic camera and a
    /// Bootstrap GameObject carrying GameBootstrap. Everything else is spawned at runtime.
    /// Run once via: Unity -batchmode -quit -executeMethod BattleCity.EditorTools.SceneBuilder.CreateGameScene
    /// The camera transform/size set here are placeholders; GameBootstrap reconfigures the
    /// camera at runtime to frame the level.
    /// </summary>
    public static class SceneBuilder
    {
        const string ScenePath = "Assets/Scenes/Game.unity";

        [MenuItem("Tools/BattleCity/Create Game Scene")]
        public static void CreateGameScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            var cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 8f;
            cam.transform.position = new Vector3(0f, 0f, -10f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;
            camGo.AddComponent<UniversalAdditionalCameraData>();
            camGo.AddComponent<AudioListener>();

            var bootstrap = new GameObject("Bootstrap");
            bootstrap.AddComponent<GameBootstrap>();

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            Debug.Log($"BattleCity: Game scene created at {ScenePath}");
        }
    }
}
