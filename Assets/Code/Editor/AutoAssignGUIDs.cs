using CustomCode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class AutoAssignGUIDs
{
    static AutoAssignGUIDs()
    {
        EditorApplication.hierarchyChanged += ConfigureNewObjects;
    }

    private static void ConfigureNewObjects()
    {
        foreach (AssemblyPart assemblyPart in Object.FindObjectsByType<AssemblyPart>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            UniqueID uniqueID = assemblyPart.GetComponent<UniqueID>();

            if (uniqueID == null)
            {
                uniqueID = assemblyPart.AddComponent<UniqueID>();
            }
            assemblyPart.id = uniqueID.Id;
        }

        foreach (CompletableItemData completableItem in Object.FindObjectsByType<CompletableItemData>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            UniqueID uniqueID = completableItem.GetComponent<UniqueID>();

            if (uniqueID == null)
            {
                uniqueID = completableItem.AddComponent<UniqueID>();
            }
        }
    }
}