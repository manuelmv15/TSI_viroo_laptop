using System.Collections.Generic;
using UnityEngine;

namespace CustomCode
{
    public class FeedbackGameObject
    {
        public static GameObject Create(GameObject source, Material overrideMaterial = null)
        {
            GameObject result = new GameObject(source.name);
            CopyHierarchy(source.transform, result.transform, overrideMaterial);

            result.transform.SetParent(source.transform.parent);
            result.transform.localPosition = source.transform.localPosition;
            result.transform.localRotation = source.transform.localRotation;
            result.transform.localScale = source.transform.localScale;

            return result;
        }

        private static void CopyHierarchy(Transform original, Transform newParent, Material overrideMaterial)
        {
            foreach (Transform child in original)
            {
                GameObject newChild = new GameObject(child.name);

                newChild.transform.SetParent(newParent);
                newChild.transform.localPosition = child.localPosition;
                newChild.transform.localRotation = child.localRotation;
                newChild.transform.localScale = child.localScale;

                Renderer originalRenderer = child.GetComponent<Renderer>();
                MeshFilter originalMeshFilter = child.GetComponent<MeshFilter>();
                if (originalRenderer != null)
                {
                    MeshFilter newMeshFilter = newChild.AddComponent<MeshFilter>();
                    newMeshFilter.sharedMesh = originalMeshFilter.sharedMesh;

                    Renderer newRenderer = newChild.AddComponent(originalRenderer.GetType()) as Renderer;

                    List<Material> materials = new List<Material>();
                    originalRenderer.GetSharedMaterials(materials);

                    if (overrideMaterial != null)
                    {
                        for (int i = 0; i < materials.Count; i++)
                        {
                            materials[i] = overrideMaterial;
                        }
                    }

                    newRenderer.SetSharedMaterials(materials);

                    //newRenderer.sharedMaterial = overrideMaterial == null ?
                    //    originalRenderer.sharedMaterial :
                    //    overrideMaterial;
                }

                CopyHierarchy(child, newChild.transform, overrideMaterial);
            }
        }
    }
}
