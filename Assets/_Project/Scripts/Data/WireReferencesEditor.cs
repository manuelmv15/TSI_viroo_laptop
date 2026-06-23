#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class WireReferencesEditor
{
    [MenuItem("Tools/LaptopAssembly/3 - Conectar referencias de Inspector")]
    public static void WireAll()
    {
        WireComponentInfoUI();
        WireComponentSelector();
        WireAudioManager();
        WireQuizManager();
        WireAssemblyManager();
        WireAssemblySlots();

        EditorUtility.SetDirty(GameObject.Find("Root"));
        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        Debug.Log("Todas las referencias conectadas y escena guardada.");
    }

    static T Find<T>(string path) where T : Component
    {
        var go = GameObject.Find(path);
        return go != null ? go.GetComponent<T>() : null;
    }

    static void WireComponentInfoUI()
    {
        var uiGO = GameObject.Find("ComponentInfoUI");
        if (uiGO == null) { Debug.LogError("ComponentInfoUI no encontrado"); return; }
        var ui = uiGO.GetComponent<ComponentInfoUI>();
        var panel = uiGO.transform.Find("Panel")?.gameObject;
        if (panel == null) { Debug.LogError("Panel no encontrado en ComponentInfoUI"); return; }

        var so = new SerializedObject(ui);
        so.FindProperty("panel").objectReferenceValue             = panel;
        so.FindProperty("txtNombre").objectReferenceValue         = panel.transform.Find("txtNombre")?.GetComponent<TextMeshProUGUI>();
        so.FindProperty("txtFuncion").objectReferenceValue        = panel.transform.Find("txtFuncion")?.GetComponent<TextMeshProUGUI>();
        so.FindProperty("txtCaracteristicas").objectReferenceValue = panel.transform.Find("txtCaracteristicas")?.GetComponent<TextMeshProUGUI>();
        so.FindProperty("txtCompatibilidad").objectReferenceValue  = panel.transform.Find("txtCompatibilidad")?.GetComponent<TextMeshProUGUI>();
        so.FindProperty("btnCerrar").objectReferenceValue         = panel.transform.Find("btnCerrar")?.GetComponent<Button>();
        so.ApplyModifiedProperties();
        Debug.Log("ComponentInfoUI: OK");
    }

    static void WireComponentSelector()
    {
        var selGO = GameObject.Find("ComponentSelector");
        if (selGO == null) { Debug.LogError("ComponentSelector no encontrado"); return; }
        var sel = selGO.GetComponent<ComponentSelector>();
        var infoUI = GameObject.Find("ComponentInfoUI")?.GetComponent<ComponentInfoUI>();

        var so = new SerializedObject(sel);
        so.FindProperty("infoUI").objectReferenceValue = infoUI;
        so.ApplyModifiedProperties();
        Debug.Log("ComponentSelector: OK");
    }

    static void WireAudioManager()
    {
        var amGO = GameObject.Find("AudioManager");
        if (amGO == null) { Debug.LogError("AudioManager no encontrado"); return; }
        var am   = amGO.GetComponent<AudioManager>();
        var srcs = amGO.GetComponents<AudioSource>();
        if (srcs.Length < 3) { Debug.LogError("AudioManager: faltan AudioSources"); return; }

        var so = new SerializedObject(am);
        so.FindProperty("sourceNarracion").objectReferenceValue = srcs[0];
        so.FindProperty("sourceAmbiente").objectReferenceValue  = srcs[1];
        so.FindProperty("sourceSFX").objectReferenceValue       = srcs[2];
        so.ApplyModifiedProperties();
        Debug.Log("AudioManager: OK");
    }

    static void WireQuizManager()
    {
        var qmGO = GameObject.Find("QuizManager");
        if (qmGO == null) { Debug.LogError("QuizManager no encontrado"); return; }
        var qm    = qmGO.GetComponent<QuizManager>();
        var panel = qmGO.transform.Find("PanelQuiz")?.gameObject;
        if (panel == null) { Debug.LogError("PanelQuiz no encontrado"); return; }

        var so = new SerializedObject(qm);
        so.FindProperty("panelQuiz").objectReferenceValue   = panel;
        so.FindProperty("txtPregunta").objectReferenceValue = panel.transform.Find("txtPregunta")?.GetComponent<TextMeshProUGUI>();
        so.FindProperty("txtResultado").objectReferenceValue = panel.transform.Find("txtResultado")?.GetComponent<TextMeshProUGUI>();
        so.FindProperty("txtPuntaje").objectReferenceValue  = panel.transform.Find("txtPuntaje")?.GetComponent<TextMeshProUGUI>();
        so.FindProperty("btnSiguiente").objectReferenceValue = panel.transform.Find("btnSiguiente")?.GetComponent<Button>();

        var btnList  = so.FindProperty("botonesOpciones");
        var txtList  = so.FindProperty("txtOpciones");
        btnList.arraySize = 4;
        txtList.arraySize = 4;
        for (int i = 0; i < 4; i++)
        {
            var btnGO = panel.transform.Find($"btnOpcion{i}");
            btnList.GetArrayElementAtIndex(i).objectReferenceValue = btnGO?.GetComponent<Button>();
            txtList.GetArrayElementAtIndex(i).objectReferenceValue = btnGO?.GetComponentInChildren<TextMeshProUGUI>();
        }
        so.ApplyModifiedProperties();
        Debug.Log("QuizManager: OK");
    }

    static void WireAssemblyManager()
    {
        var amGO = GameObject.Find("AssemblyManager");
        if (amGO == null) { Debug.LogError("AssemblyManager no encontrado"); return; }
        var am = amGO.GetComponent<AssemblyManager>();

        // Conectar OnAssemblyComplete → QuizManager.IniciarQuiz
        var qm = GameObject.Find("QuizManager")?.GetComponent<QuizManager>();
        if (qm != null)
        {
            var so = new SerializedObject(am);
            so.FindProperty("quizManager").objectReferenceValue = qm;
            so.ApplyModifiedProperties();
            Debug.Log("AssemblyManager → QuizManager: OK");
        }

        // Conectar slots list
        var chassis = GameObject.Find("LaptopChassis");
        if (chassis != null)
        {
            var slots = chassis.GetComponentsInChildren<AssemblySlot>();
            var piezas = GameObject.Find("Components")?.GetComponentsInChildren<InteractableComponent>();

            var so = new SerializedObject(am);
            var slotsProp  = so.FindProperty("slots");
            var piezasProp = so.FindProperty("piezas");
            slotsProp.arraySize  = slots.Length;
            piezasProp.arraySize = piezas != null ? piezas.Length : 0;
            for (int i = 0; i < slots.Length; i++)
                slotsProp.GetArrayElementAtIndex(i).objectReferenceValue = slots[i];
            if (piezas != null)
                for (int i = 0; i < piezas.Length; i++)
                    piezasProp.GetArrayElementAtIndex(i).objectReferenceValue = piezas[i];
            so.ApplyModifiedProperties();
            Debug.Log($"AssemblyManager: {slots.Length} slots, {piezas?.Length ?? 0} piezas conectadas");
        }
        else Debug.LogWarning("LaptopChassis no encontrado — slots no conectados");
    }

    static void WireAssemblySlots()
    {
        // Los slots ya tienen componenteEsperado asignado desde SceneSetupEditor
        // Solo verificamos que estén bien
        var slots = Object.FindObjectsByType<AssemblySlot>(FindObjectsSortMode.None);
        int ok = 0;
        foreach (var s in slots)
            if (s.componenteEsperado != null) ok++;
        Debug.Log($"AssemblySlots verificados: {ok}/{slots.Length} tienen ComponentData asignado");
    }
}
#endif
