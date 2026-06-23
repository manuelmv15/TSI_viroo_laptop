#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Virtualware.Util
{
    public class FindMissingScripts : EditorWindow
    {
        static int go_count = 0, components_count = 0, missing_count = 0;
        static List<GameObject> gameObjectsWithMissingComponents;

        [MenuItem("Window/FindMissingScripts")]
        public static void ShowWindow()
        {
            GetWindow(typeof(FindMissingScripts));
        }

        public void OnGUI()
        {
            if (GUILayout.Button("Find Missing Scripts in active Scene"))
            {
                Find(false);
            }

            if (GUILayout.Button("Remove Missing Scripts in active Scene"))
            {
                Find(true);
            }
        }
        private static void Find(bool remove)
        {
            GameObject[] go = SceneManager.GetActiveScene().GetRootGameObjects();
            gameObjectsWithMissingComponents = new List<GameObject>();
            go_count = 0;
            components_count = 0;
            missing_count = 0;
            foreach (GameObject g in go)
            {
                FindInGO(g);
            }
            Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));

            if (remove)
            {
                foreach (var gameObject in gameObjectsWithMissingComponents)
                {
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);
                }

                Debug.Log("Removed Missing Scripts");
            }
        }

        private static void FindInGO(GameObject g)
        {
            go_count++;
            Component[] components = g.GetComponents<Component>();

            for (int i = 0; i < components.Length; i++)
            {
                components_count++;
                if (components[i] == null)
                {
                    missing_count++;
                    string s = g.name;
                    Transform t = g.transform;
                    while (t.parent != null)
                    {
                        s = t.parent.name + "/" + s;
                        t = t.parent;
                    }
                    Debug.Log(s + " has an empty script attached in position: " + i, g);

                    if (!gameObjectsWithMissingComponents.Contains(g))
                    {
                        gameObjectsWithMissingComponents.Add(g);
                    }
                }
            }

            foreach (Transform childT in g.transform)
            {
                FindInGO(childT.gameObject);
            }
        }
    }
}
#endif