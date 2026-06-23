using UnityEngine;
using System;

namespace CustomCode
{

    [ExecuteInEditMode]
    public class UniqueID : MonoBehaviour
    {
        [SerializeField]
        private string guid;

        public string Id {  get { return guid; } }

        private void Awake()
        {
            // Generate a GUID if it doesn't already exist
            if (string.IsNullOrEmpty(guid))
            {
                guid = Guid.NewGuid().ToString();
            }
        }

        // Optional: Reset GUID manually (e.g., if duplicating objects)
        [ContextMenu("Reset GUID")]
        private void ResetGUID()
        {
            guid = Guid.NewGuid().ToString();
        }
    }
}