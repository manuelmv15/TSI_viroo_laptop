using UnityEngine;
using UnityEditor;
using System.Linq;

public class AssignLaptopMeshes
{
    [MenuItem("Tools/Assign Laptop Meshes")]
    public static void Assign()
    {
        var map = new System.Collections.Generic.Dictionary<string, string>
        {
            { "Tarjeta_Madre",           "Assets/3DModels/Laptop/computer+motherboard+3d+model/tripo_convert_9e1e57b5-63ff-4763-93e6-a8686d8397de.fbx" },
            { "Memoria_RAM",             "Assets/3DModels/Laptop/laptop+ram+module+3d+model/tripo_convert_60d8b86f-9ab0-4435-81a7-1d6c6ae94c42.fbx" },
            { "SSD",                     "Assets/3DModels/Laptop/m.2+nvme+ssd+3d+model/tripo_convert_a44cc9da-f44f-4e06-a1bb-0e9aeaefb0b7.fbx" },
            { "Sistema_de_Enfriamiento", "Assets/3DModels/Laptop/cpu+cooler+fan+3d+model/tripo_convert_7f81e035-0328-4d58-875d-565502c61532.fbx" },
            { "Teclado",                 "Assets/3DModels/Laptop/keyboard+3d+model/tripo_convert_3eb73958-ae83-4ce9-aeb3-3d242d185e5d.fbx" },
        };

        foreach (var kvp in map)
        {
            var go = GameObject.Find("Root/LaptopAssembly/Components/" + kvp.Key);
            if (go == null) { Debug.LogWarning("[AssignMeshes] Not found: " + kvp.Key); continue; }

            var mf = go.GetComponent<MeshFilter>();
            var mr = go.GetComponent<MeshRenderer>();
            if (mf == null) { Debug.LogWarning("[AssignMeshes] No MeshFilter: " + kvp.Key); continue; }

            var assets = AssetDatabase.LoadAllAssetsAtPath(kvp.Value);
            var mesh = assets.OfType<Mesh>().FirstOrDefault();
            var mats = assets.OfType<Material>().ToArray();

            if (mesh == null) { Debug.LogWarning("[AssignMeshes] No mesh in FBX: " + kvp.Key); continue; }

            Undo.RecordObject(mf, "Assign FBX Mesh");
            mf.sharedMesh = mesh;

            if (mr != null && mats.Length > 0)
            {
                Undo.RecordObject(mr, "Assign FBX Materials");
                mr.sharedMaterials = mats;
            }

            EditorUtility.SetDirty(go);
            Debug.Log("[AssignMeshes] OK: " + kvp.Key + " -> " + mesh.name);
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        Debug.Log("[AssignMeshes] Done.");
    }
}
