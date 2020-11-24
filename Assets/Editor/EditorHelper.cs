using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EditorHelper
{
    private static CreateSceneParameters csp = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
    private static NewSceneSetup setup = NewSceneSetup.EmptyScene;
    private static NewSceneMode mode = NewSceneMode.Additive;
    static Scene previewScene;
    static PhysicsScene previewPhysicsScene;

    public static void ManualPhysicsStepGlobal()
    {
        Physics.autoSimulation = false;
        Physics.Simulate(Time.fixedDeltaTime);
        Physics.autoSimulation = true;
    }

    public static void ManualPhysicsStepFor(GameObject obj)
    {
        Physics.autoSimulation = false;
        if (!previewScene.IsValid())
        {
            previewScene = EditorSceneManager.NewScene(setup, mode);
        }
        if (!previewPhysicsScene.IsValid())
        {
            previewPhysicsScene = previewScene.GetPhysicsScene();
        }

        // GameObject rootObj = obj.transform.root.gameObject;
        // Scene currentScene = EditorSceneManager.GetActiveScene();
        ClearScene(previewScene);
        GameObject cloneObj = GameObject.Instantiate(obj);
        EditorSceneManager.MoveGameObjectToScene(cloneObj, previewScene);
        previewPhysicsScene.Simulate(Time.fixedDeltaTime);
        Physics.autoSimulation = true;
    }

    public static void ClearScene(Scene scene)
    {
        foreach (var obj in scene.GetRootGameObjects())
        {
            GameObject.DestroyImmediate(obj);
        }
    }
}
