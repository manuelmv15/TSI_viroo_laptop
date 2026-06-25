using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public static class TempSceneDiagnostics
{
    [MenuItem("Tools/Run Scene Diagnostics")]
    public static void Run()
    {
        var lines = new List<string>();
        var allGOs = Resources.FindObjectsOfTypeAll<GameObject>();

        // Missing scripts
        var missing = new List<string>();
        foreach (var go in allGOs)
        {
            foreach (var c in go.GetComponents<Component>())
            {
                if (c == null)
                {
                    missing.Add(go.name + " | scene:" + go.scene.name);
                    break;
                }
            }
        }
        lines.Add("MISSING_SCRIPTS:" + missing.Count);
        lines.AddRange(missing);

        // Corrupt transforms
        var corrupt = new List<string>();
        foreach (var go in allGOs)
        {
            if (string.IsNullOrEmpty(go.scene.name)) continue;
            var p = go.transform.position;
            var s = go.transform.localScale;
            bool nan = float.IsNaN(p.x) || float.IsNaN(p.y) || float.IsNaN(p.z);
            bool inf = float.IsInfinity(p.x) || float.IsInfinity(p.y) || float.IsInfinity(p.z);
            bool far = p.magnitude > 50000f;
            bool bscl = float.IsNaN(s.x) || float.IsNaN(s.y) || float.IsNaN(s.z) || float.IsInfinity(s.x) || float.IsInfinity(s.y) || float.IsInfinity(s.z);
            if (nan || inf || far || bscl)
            {
                string reason = nan ? "NaN" : inf ? "Inf" : far ? "Far(" + p.magnitude.ToString("F0") + ")" : "BadScale";
                corrupt.Add(go.name + " | " + reason + " | pos:" + p + " | scl:" + s + " | scene:" + go.scene.name);
            }
        }
        lines.Add("CORRUPT_TRANSFORMS:" + corrupt.Count);
        lines.AddRange(corrupt);

        string path = Application.dataPath + "/../diagnostics_result.txt";
        File.WriteAllLines(path, lines);
        Debug.Log("Diagnostics written to: " + path);
    }
}
