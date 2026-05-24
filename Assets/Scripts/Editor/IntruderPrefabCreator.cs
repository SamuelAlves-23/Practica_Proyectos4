using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using SOTG.Mechanics.Intruder;

public class IntruderPrefabCreator
{
    [MenuItem("Tools/Create Intruder Prefabs")]
    public static void CreateIntruderPrefabs()
    {
        // Load the Joe model
        GameObject joeModel = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/Characters/Joe/Ch_Joe_Geo.fbx");
        RuntimeAnimatorController npcController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Art/Characters/Joe/Animations/NPCAnimatorController.controller");

        if (joeModel == null)
        {
            Debug.LogError("Joe model not found at Assets/Art/Characters/Joe/Ch_Joe_Geo.fbx");
            return;
        }

        if (npcController == null)
        {
            Debug.LogError("NPCAnimatorController not found");
            return;
        }

        string[] intruderNames = { "Intruder_Joe", "Intruder_Sophie", "Intruder_Maria" };

        foreach (string name in intruderNames)
        {
            CreateIntruderPrefab(name, joeModel, npcController);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("3 Intruder prefabs created in Assets/Prefabs/Intruders/");
    }

    private static void CreateIntruderPrefab(string name, GameObject model, RuntimeAnimatorController controller)
    {
        // Create root object
        GameObject intruder = new GameObject(name);

        // Add NavMeshAgent
        NavMeshAgent agent = intruder.AddComponent<NavMeshAgent>();
        agent.speed = 7.5f;
        agent.radius = 0.5f;
        agent.height = 2f;

        // Add Intruder script
        intruder.AddComponent<Intruder>();

        // Add SphereCollider (trigger for player detection)
        SphereCollider triggerCollider = intruder.AddComponent<SphereCollider>();
        triggerCollider.radius = 1f;
        triggerCollider.isTrigger = true;

        // Instantiate the model as child
        GameObject modelInstance = Object.Instantiate(model, intruder.transform);
        modelInstance.name = "Model";

        // Set up Animator
        Animator animator = modelInstance.GetComponent<Animator>();
        if (animator == null)
        {
            animator = modelInstance.AddComponent<Animator>();
        }
        animator.runtimeAnimatorController = controller;

        // Ensure the model has the Player tag for collision detection
        // (The Intruder script checks for "Player" tag)

        // Create prefab
        string prefabPath = $"Assets/Prefabs/Intruders/{name}.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(intruder, prefabPath);

        Debug.Log($"Created: {prefabPath}");

        // Clean up the scene object
        Object.DestroyImmediate(intruder);
    }
}