using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CustomCode
{
    public class CompletableSequenceData : MonoBehaviour
    {
        public string sequenceId;
        public AssemblyGroup assemblyGroup;
        public List<CompletableItemData> completableItems;

        public CompletableItemData FindById(string id)
        {
            CompletableItemData result = completableItems.FirstOrDefault(ci => ci.GetComponent<UniqueID>().Id == id);
            return result;
        }

        private void Start()
        {
            CompletableItemData[] completableItemFullList = assemblyGroup.GetComponentsInChildren<CompletableItemData>();
            foreach (var item in completableItemFullList)
            {
                item.GetComponent<FeedBack>()?.Hide();
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Color savedColor = Gizmos.color;
            Gizmos.color = Color.white;

            GUIStyle labelStyle = new GUIStyle();
            labelStyle.normal.textColor = Color.black;
            labelStyle.fontSize = 12;
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.alignment = TextAnchor.MiddleCenter;

            if (completableItems.Count > 1)
            {
                for (int i = 0; i < completableItems.Count; i++)
                {
                    var current = completableItems[i];
                    Handles.Label(current.transform.position + Vector3.up * 0.1f, current.gameObject.name, labelStyle);
                    if (i < completableItems.Count - 1)
                    {
                        var next = completableItems[i + 1];
                        //Handles.DrawLine(current.transform.position, next.transform.position);
                        Gizmos.DrawLine(current.transform.position, next.transform.position);
                    }
                }
            }
            Gizmos.color = savedColor;
        }
#endif
    }
}
