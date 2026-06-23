using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class CreateCharacterMovementTestScene
{
    private const string ScenePath = "Assets/Scenes/CharacterMovementTest.unity";

    [MenuItem("Tools/Movimiento de Personaje/Crear escena de prueba")]
    public static void CreateScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.localScale = new Vector3(5f, 1f, 5f);

        var lightObject = new GameObject("Directional Light");
        var light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.transform.position = new Vector3(0f, 1f, 0f);
        Object.DestroyImmediate(player.GetComponent<CapsuleCollider>());
        player.AddComponent<CharacterController>();
        player.AddComponent<SimpleCharacterMovement>();

        var cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.AddComponent<Camera>();
        cameraObject.AddComponent<AudioListener>();
        cameraObject.transform.position = new Vector3(0f, 8f, -10f);
        cameraObject.transform.rotation = Quaternion.Euler(35f, 0f, 0f);

        EditorSceneManager.SaveScene(scene, ScenePath);
        Debug.Log($"Escena de prueba creada en {ScenePath}. Presiona Play y usa WASD/flechas, Shift para correr y Espacio para saltar.");
    }
}
