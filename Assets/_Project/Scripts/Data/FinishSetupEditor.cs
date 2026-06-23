#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class FinishSetupEditor
{
    [MenuItem("Tools/LaptopAssembly/4 - Etiquetas y HUD")]
    public static void Run()
    {
        AddLabels();
        CreateAssemblyHUD();
        FixComponentScales();
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        Debug.Log("Etiquetas, HUD y escalas listas.");
    }

    static void AddLabels()
    {
        var compContainer = GameObject.Find("Components");
        if (compContainer == null) { Debug.LogError("Components no encontrado"); return; }

        foreach (Transform child in compContainer.transform)
        {
            if (child.Find("Label") != null) continue;

            var ic = child.GetComponent<InteractableComponent>();
            string label = ic?.data?.nombre ?? child.name.Replace("_", " ");

            var labelGO = new GameObject("Label");
            labelGO.transform.SetParent(child);
            labelGO.transform.localPosition = new Vector3(0, 0.06f, 0);
            labelGO.transform.localRotation = Quaternion.Euler(90, 0, 0);
            labelGO.transform.localScale    = Vector3.one * 0.08f;

            var tmp = labelGO.AddComponent<TextMeshPro>();
            tmp.text = label;
            tmp.fontSize = 1.5f;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            tmp.fontStyle = FontStyles.Bold;
            tmp.outlineWidth = 0.2f;
            tmp.outlineColor = new Color32(0, 0, 0, 255);
        }
        Debug.Log("Etiquetas 3D: OK");
    }

    static void CreateAssemblyHUD()
    {
        if (GameObject.Find("AssemblyHUD") != null) return;

        var laptopAssembly = GameObject.Find("LaptopAssembly");
        if (laptopAssembly == null) return;

        var hud = new GameObject("AssemblyHUD");
        hud.transform.parent   = laptopAssembly.transform;
        hud.transform.position = new Vector3(-16f, 1.35f, 13.2f);
        hud.transform.rotation = Quaternion.Euler(0, 180, 0);

        var canvas = hud.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        hud.AddComponent<CanvasScaler>();
        hud.AddComponent<GraphicRaycaster>();
        hud.transform.localScale = Vector3.one * 0.004f;

        var bg = new GameObject("Background");
        bg.transform.SetParent(hud.transform, false);
        var img = bg.AddComponent<Image>();
        img.color = new Color(0.05f, 0.05f, 0.12f, 0.88f);
        var bgRect = bg.GetComponent<RectTransform>();
        bgRect.sizeDelta = new Vector2(320, 80);

        var txtProgreso = MakeTMP(bg.transform, "txtProgreso",
            new Vector2(0, 22), new Vector2(300, 32), 18, FontStyles.Bold);
        txtProgreso.text = "0/8 piezas instaladas";

        var txtInstruccion = MakeTMP(bg.transform, "txtInstruccion",
            new Vector2(0, -18), new Vector2(300, 28), 13, FontStyles.Normal);
        txtInstruccion.text = "Selecciona un componente para comenzar";

        // Asignar al AssemblyManager
        var am = GameObject.Find("AssemblyManager")?.GetComponent<AssemblyManager>();
        if (am != null)
        {
            var so = new SerializedObject(am);
            so.FindProperty("txtProgreso").objectReferenceValue   = txtProgreso;
            so.FindProperty("txtInstruccion").objectReferenceValue = txtInstruccion;
            so.ApplyModifiedProperties();
            Debug.Log("AssemblyHUD conectado a AssemblyManager: OK");
        }
    }

    static void FixComponentScales()
    {
        var fixes = new System.Collections.Generic.Dictionary<string, Vector3>
        {
            { "Tarjeta_Madre",          new Vector3(0.28f, 0.02f, 0.22f) },
            { "Memoria_RAM",            new Vector3(0.13f, 0.02f, 0.03f) },
            { "SSD",                    new Vector3(0.08f, 0.01f, 0.03f) },
            { "Sistema_de_Enfriamiento",new Vector3(0.12f, 0.04f, 0.10f) },
            { "Batería",                new Vector3(0.25f, 0.03f, 0.12f) },
            { "Teclado",                new Vector3(0.28f, 0.01f, 0.10f) },
            { "Touchpad",               new Vector3(0.10f, 0.01f, 0.07f) },
            { "Pantalla",               new Vector3(0.35f, 0.22f, 0.02f) },
        };

        var compContainer = GameObject.Find("Components");
        if (compContainer == null) return;
        foreach (Transform child in compContainer.transform)
            if (fixes.TryGetValue(child.name, out var sc))
                child.localScale = sc;

        Debug.Log("Escalas de componentes corregidas.");
    }

    static TextMeshProUGUI MakeTMP(Transform parent, string name,
        Vector2 pos, Vector2 size, float fs, FontStyles style)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var txt = go.AddComponent<TextMeshProUGUI>();
        txt.fontSize  = fs;
        txt.fontStyle = style;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color     = Color.white;
        var r = go.GetComponent<RectTransform>();
        r.anchoredPosition = pos;
        r.sizeDelta        = size;
        return txt;
    }
}
#endif
