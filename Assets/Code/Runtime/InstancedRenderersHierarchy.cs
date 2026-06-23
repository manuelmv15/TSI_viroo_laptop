
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace CustomCode
{
    [ExecuteInEditMode]
    public class InstancedRenderersHierarchy : MonoBehaviour
    {
        public Material material;
        public GameObject source;
        public bool editorOnly = false;

        private List<Mesh> meshes = new List<Mesh>();
        private List<Matrix4x4> matrices = new List<Matrix4x4>();
        
        private void OnValidate()
        {
            CacheMeshesMatrices();
        }

        private void Start()
        {
            CacheMeshesMatrices();
        }

        private void Update()
        {

#if UNITY_EDITOR
            if (!Selection.Contains(this.gameObject) && editorOnly)
                return;
#endif
            if (editorOnly && !Application.isEditor)
            {
                this.enabled = false;
                return;
            }

            RenderParams rp = new RenderParams(material);
            rp.receiveShadows = false;
            rp.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            CacheMeshesMatrices();

            for (int i = 0; i < meshes.Count; i++)
            {
                Mesh mesh = meshes[i];
                Matrix4x4[] currentMatrices = { matrices[i] };

                for (int j = 0; j < mesh.subMeshCount; j++)
                {
                    Graphics.RenderMeshInstanced(rp, mesh, j, currentMatrices);
                }
            }
        }

        private void CacheMeshesMatrices()
        {
            matrices.Clear();
            meshes.Clear();

            // OnValidate is executed when something changes in the inspector
            // writing properties manually causes it to trigger when not expected
            if (source == null)
            {
                return;
            }

            Transform sourceT = source.transform;
            Transform targetT = transform;

            Matrix4x4 sourceMat = Matrix4x4.TRS(sourceT.position, sourceT.rotation, sourceT.lossyScale);
            Matrix4x4 targetMat = Matrix4x4.TRS(targetT.position, targetT.rotation, targetT.lossyScale);

            MeshFilter[] mfs = source.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter mf in mfs)
            {
                Mesh mesh = mf.sharedMesh;

                Matrix4x4 mat = Matrix4x4.TRS(mf.transform.position, mf.transform.rotation, mf.transform.lossyScale);
                Matrix4x4 newMat = sourceMat.inverse * mat;
                newMat = targetMat * newMat;

                meshes.Add(mesh);
                matrices.Add(newMat);
            }
        }
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (source != null)
            {
                Transform sourceT = source.transform;
                Transform targetT = transform;

                Handles.DrawLine(sourceT.position, targetT.position, 3);
            }
        }
#endif
    }
}
