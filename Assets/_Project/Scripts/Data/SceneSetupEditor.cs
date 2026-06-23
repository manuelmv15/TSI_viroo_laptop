#if UNITY_EDITOR
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;

public static class SceneSetupEditor
{
    static readonly Color[] coloresComponentes = {
        new Color(0.2f,0.5f,0.9f),  // Pantalla - azul
        new Color(0.9f,0.7f,0.1f),  // Batería - amarillo
        new Color(0.2f,0.7f,0.3f),  // Tarjeta Madre - verde
        new Color(0.9f,0.2f,0.2f),  // RAM - rojo
        new Color(0.6f,0.3f,0.9f),  // SSD - violeta
        new Color(0.1f,0.8f,0.8f),  // Enfriamiento - cyan
        new Color(0.9f,0.5f,0.1f),  // Teclado - naranja
        new Color(0.7f,0.9f,0.2f),  // Touchpad - verde lima
    };

    static readonly Vector3[] escalasComponentes = {
        new Vector3(0.35f,0.22f,0.02f), // Pantalla (plana)
        new Vector3(0.25f,0.04f,0.12f), // Batería (rectángulo plano)
        new Vector3(0.28f,0.02f,0.22f), // Tarjeta Madre (placa grande)
        new Vector3(0.12f,0.02f,0.03f), // RAM (barra larga delgada)
        new Vector3(0.08f,0.01f,0.03f), // SSD (rectángulo pequeño)
        new Vector3(0.12f,0.04f,0.10f), // Enfriamiento (bloque cuadrado)
        new Vector3(0.28f,0.01f,0.10f), // Teclado (placa ancha)
        new Vector3(0.10f,0.01f,0.07f), // Touchpad (rectángulo medio)
    };

    [MenuItem("Tools/LaptopAssembly/1 - Crear ComponentData assets")]
    public static void CrearComponentDataAssets()
    {
        CreateComponentDataEditor.CreateAll();
    }

    [MenuItem("Tools/LaptopAssembly/2 - Setup completo de escena")]
    public static void SetupScene()
    {
        // Asegura carpeta UI scripts
        if (!AssetDatabase.IsValidFolder("Assets/_Project/Scripts/UI"))
            AssetDatabase.CreateFolder("Assets/_Project/Scripts", "UI");

        // Localiza el contenedor de la escena
        var laptopAssembly = GameObject.Find("Root/LaptopAssembly")
                          ?? GameObject.Find("LaptopAssembly");
        if (laptopAssembly == null)
        {
            Debug.LogError("No se encontró LaptopAssembly en la escena. Asegúrate de tener Laptop.unity abierta.");
            return;
        }

        var envContainer = laptopAssembly.transform.Find("Environment")?.gameObject
                        ?? new GameObject("Environment") { transform = { parent = laptopAssembly.transform } };
        var compContainer = laptopAssembly.transform.Find("Components")?.gameObject
                         ?? new GameObject("Components") { transform = { parent = laptopAssembly.transform } };

        // Carga los ComponentData
        var dataAssets = LoadComponentDataAssets();
        if (dataAssets == null || dataAssets.Length == 0)
        {
            Debug.LogError("No se encontraron ComponentData assets. Ejecuta primero 'Crear ComponentData assets'.");
            return;
        }

        // Crea la laptop chassis (base donde se ensambla)
        var chassis = CrearChassis(envContainer.transform);

        // Crea los 8 placeholders de componentes sobre la mesa
        var workbench = GameObject.Find("Workbench");
        var workbenchPos = workbench != null ? workbench.transform.position : new Vector3(-16, 0.9f, 12);
        CrearPlaceholdersComponentes(compContainer.transform, dataAssets, workbenchPos, chassis);

        // Crea AudioManager
        CrearAudioManager(laptopAssembly.transform);

        // Crea AssemblyManager
        CrearAssemblyManager(laptopAssembly.transform);

        // Crea ComponentInfoUI (World Space Canvas)
        CrearComponentInfoUI(laptopAssembly.transform);

        // Crea ComponentSelector
        CrearComponentSelector(laptopAssembly.transform);

        // Crea Panel Comparativa
        CrearPanelComparativa(laptopAssembly.transform);

        // Crea QuizManager
        CrearQuizManager(laptopAssembly.transform);

        EditorUtility.SetDirty(laptopAssembly);
        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        Debug.Log("Setup completo de escena finalizado.");
    }

    static ComponentData[] LoadComponentDataAssets()
    {
        var guids = AssetDatabase.FindAssets("t:ComponentData", new[] { "Assets/_Project/Data" });
        var assets = new ComponentData[guids.Length];
        for (int i = 0; i < guids.Length; i++)
            assets[i] = AssetDatabase.LoadAssetAtPath<ComponentData>(AssetDatabase.GUIDToAssetPath(guids[i]));
        System.Array.Sort(assets, (a, b) => a.ordenEnsamblaje.CompareTo(b.ordenEnsamblaje));
        return assets;
    }

    static GameObject CrearChassis(Transform parent)
    {
        var existing = parent.Find("LaptopChassis");
        if (existing != null) return existing.gameObject;

        var chassis = GameObject.CreatePrimitive(PrimitiveType.Cube);
        chassis.name = "LaptopChassis";
        chassis.transform.parent = parent;
        chassis.transform.localPosition = new Vector3(0, 0.91f, -0.1f);
        chassis.transform.localScale = new Vector3(0.32f, 0.22f, 0.02f);

        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = new Color(0.15f, 0.15f, 0.15f);
        AssetDatabase.CreateAsset(mat, "Assets/_Project/Materials/M_Chassis.mat");
        chassis.GetComponent<Renderer>().sharedMaterial = mat;

        // Slot triggers para las 8 piezas (se configurarán al crear los componentes)
        return chassis;
    }

    static void CrearPlaceholdersComponentes(Transform parent, ComponentData[] dataAssets, Vector3 mesaPos, GameObject chassis)
    {
        int cols = 4;
        float spacingX = 0.22f;
        float spacingZ = 0.20f;

        for (int i = 0; i < dataAssets.Length; i++)
        {
            var data = dataAssets[i];
            string safeName = data.nombre.Replace(" ", "_");

            // ¿ya existe?
            if (parent.Find(safeName) != null) continue;

            int row = i / cols;
            int col = i % cols;
            float x = mesaPos.x - (cols / 2f - 0.5f) * spacingX + col * spacingX;
            float z = mesaPos.z - 0.15f + row * spacingZ;
            float y = mesaPos.y;

            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = safeName;
            go.transform.parent = parent;
            go.transform.position = new Vector3(x, y, z);
            int colorIdx = Mathf.Clamp(i, 0, coloresComponentes.Length - 1);
            go.transform.localScale = escalasComponentes[colorIdx];

            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = coloresComponentes[colorIdx];
            string matPath = $"Assets/_Project/Materials/M_{safeName}.mat";
            if (!File.Exists(Path.Combine(Application.dataPath, matPath.Replace("Assets/", ""))))
                AssetDatabase.CreateAsset(mat, matPath);
            go.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>(matPath);

            // Collider ya lo tiene PrimitiveType.Cube
            go.GetComponent<BoxCollider>().isTrigger = false;

            // InteractableComponent ya extiende XRSimpleInteractable, no duplicar
            // Agrega InteractableComponent
            var ic = go.AddComponent<InteractableComponent>();
            ic.data = data;

            // Rigidbody para XR Grab
            var rb = go.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;

            // Slot en el chassis
            CrearSlotEnChassis(chassis, data, i);

            // Guarda prefab
            string prefabPath = $"Assets/_Project/Prefabs/Components/{safeName}.prefab";
            Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(Application.dataPath, prefabPath.Replace("Assets/", ""))));
            PrefabUtility.SaveAsPrefabAssetAndConnect(go, prefabPath, InteractionMode.AutomatedAction);
        }
    }

    static void CrearSlotEnChassis(GameObject chassis, ComponentData data, int index)
    {
        string safeName = data.nombre.Replace(" ", "_");
        var slotGO = new GameObject($"Slot_{safeName}");
        slotGO.transform.parent = chassis.transform;

        int cols = 4;
        float spacingX = 0.045f;
        float spacingY = 0.040f;
        int row = index / cols;
        int col = index % cols;
        slotGO.transform.localPosition = new Vector3(
            -0.07f + col * spacingX,
             0.01f,
            -0.04f + row * spacingY
        );

        var col3d = slotGO.AddComponent<BoxCollider>();
        col3d.isTrigger = true;
        col3d.size = new Vector3(0.04f, 0.06f, 0.04f);

        var slot = slotGO.AddComponent<AssemblySlot>();
        slot.componenteEsperado = data;
    }

    static void CrearAudioManager(Transform parent)
    {
        if (parent.Find("AudioManager") != null) return;
        var go = new GameObject("AudioManager");
        go.transform.parent = parent;
        var am = go.AddComponent<AudioManager>();
        go.AddComponent<AudioSource>(); // narracion
        go.AddComponent<AudioSource>(); // ambiente
        go.AddComponent<AudioSource>(); // sfx
    }

    static void CrearAssemblyManager(Transform parent)
    {
        if (parent.Find("AssemblyManager") != null) return;
        var go = new GameObject("AssemblyManager");
        go.transform.parent = parent;
        go.AddComponent<AssemblyManager>();
    }

    static void CrearComponentInfoUI(Transform parent)
    {
        if (parent.Find("ComponentInfoUI") != null) return;

        var canvasGO = new GameObject("ComponentInfoUI");
        canvasGO.transform.parent = parent;

        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        canvasGO.transform.localScale = Vector3.one * 0.003f;

        var panel = new GameObject("Panel");
        panel.transform.parent = canvasGO.transform;
        panel.transform.localPosition = Vector3.zero;
        var img = panel.AddComponent<UnityEngine.UI.Image>();
        img.color = new Color(0.05f, 0.05f, 0.1f, 0.95f);
        var rect = panel.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(400, 300);

        AddTMPText(panel.transform, "txtNombre", new Vector2(0, 120), new Vector2(380, 40), 24, FontStyles.Bold);
        AddTMPText(panel.transform, "txtFuncion", new Vector2(0, 60), new Vector2(380, 60), 14);
        AddTMPText(panel.transform, "txtCaracteristicas", new Vector2(0, -20), new Vector2(380, 60), 12);
        AddTMPText(panel.transform, "txtCompatibilidad", new Vector2(0, -90), new Vector2(380, 40), 12);

        var btnGO = new GameObject("btnCerrar");
        btnGO.transform.parent = panel.transform;
        var btnImg = btnGO.AddComponent<UnityEngine.UI.Image>();
        btnImg.color = new Color(0.8f, 0.2f, 0.2f);
        var btnRect = btnGO.GetComponent<RectTransform>();
        btnRect.anchoredPosition = new Vector2(170, -130);
        btnRect.sizeDelta = new Vector2(60, 30);
        var btn = btnGO.AddComponent<UnityEngine.UI.Button>();

        var txtBtn = AddTMPText(btnGO.transform, "txtCerrar", Vector2.zero, new Vector2(60, 30), 14);
        txtBtn.GetComponent<TMPro.TextMeshProUGUI>().text = "Cerrar";

        var ui = canvasGO.AddComponent<ComponentInfoUI>();
        panel.SetActive(false);
    }

    static void CrearComponentSelector(Transform parent)
    {
        if (parent.Find("ComponentSelector") != null) return;
        var go = new GameObject("ComponentSelector");
        go.transform.parent = parent;
        go.AddComponent<ComponentSelector>();
    }

    static void CrearPanelComparativa(Transform parent)
    {
        if (parent.Find("ComparativaPC") != null) return;

        var go = new GameObject("ComparativaPC");
        go.transform.parent = parent;
        go.transform.position = new Vector3(-14f, 1.4f, 11f);
        go.transform.rotation = Quaternion.Euler(0, 180, 0);

        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        go.AddComponent<UnityEngine.UI.CanvasScaler>();
        go.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        go.transform.localScale = Vector3.one * 0.003f;

        var panel = new GameObject("Panel");
        panel.transform.parent = go.transform;

        var bg = panel.AddComponent<UnityEngine.UI.Image>();
        bg.color = new Color(0.05f, 0.08f, 0.15f, 0.97f);
        panel.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 400);

        var title = AddTMPText(panel.transform, "txtTitulo", new Vector2(0, 175), new Vector2(480, 40), 22, FontStyles.Bold);
        title.GetComponent<TMPro.TextMeshProUGUI>().text = "Comparativa: PC de Escritorio vs Laptop";

        string comparativa =
            "<b>Portabilidad</b>\nPC: Fija, requiere alimentación constante.\nLaptop: Portátil, batería integrada.\n\n" +
            "<b>Refrigeración</b>\nPC: Sistema de enfriamiento grande, mayor eficiencia.\nLaptop: Compacto, mayor temperatura operacional.\n\n" +
            "<b>Expansibilidad</b>\nPC: Alta. Fácil agregar RAM, GPU, almacenamiento.\nLaptop: Limitada. RAM y almacenamiento fijos en muchos modelos.\n\n" +
            "<b>Consumo energético</b>\nPC: Mayor consumo (200-500W típico).\nLaptop: Eficiente (15-65W típico).\n\n" +
            "<b>Costo/Rendimiento</b>\nPC: Mejor rendimiento por precio.\nLaptop: Prima por portabilidad (+20-40% vs PC equivalente).";

        var txt = AddTMPText(panel.transform, "txtComparativa", new Vector2(0, -20), new Vector2(470, 300), 11);
        txt.GetComponent<TMPro.TextMeshProUGUI>().text = comparativa;

        panel.SetActive(false);
        go.AddComponent<ComparativaPanel>();
    }

    static void CrearQuizManager(Transform parent)
    {
        if (parent.Find("QuizManager") != null) return;

        var go = new GameObject("QuizManager");
        go.transform.parent = parent;
        go.transform.position = new Vector3(-16f, 1.5f, 11f);

        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        go.AddComponent<UnityEngine.UI.CanvasScaler>();
        go.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        go.transform.localScale = Vector3.one * 0.003f;

        var panel = new GameObject("PanelQuiz");
        panel.transform.parent = go.transform;
        var bg = panel.AddComponent<UnityEngine.UI.Image>();
        bg.color = new Color(0.05f, 0.08f, 0.15f, 0.97f);
        panel.GetComponent<RectTransform>().sizeDelta = new Vector2(480, 400);

        AddTMPText(panel.transform, "txtPregunta", new Vector2(0, 150), new Vector2(460, 60), 16, FontStyles.Bold);

        var btnNames = new[] { "btnOpcion0", "btnOpcion1", "btnOpcion2", "btnOpcion3" };
        float startY = 60f;
        for (int i = 0; i < 4; i++)
        {
            var btnGO = new GameObject(btnNames[i]);
            btnGO.transform.parent = panel.transform;
            var btnImg = btnGO.AddComponent<UnityEngine.UI.Image>();
            btnImg.color = new Color(0.1f, 0.2f, 0.5f);
            var btnRect = btnGO.GetComponent<RectTransform>();
            btnRect.anchoredPosition = new Vector2(0, startY - i * 50f);
            btnRect.sizeDelta = new Vector2(440, 40);
            btnGO.AddComponent<UnityEngine.UI.Button>();
            AddTMPText(btnGO.transform, $"txtOpcion{i}", Vector2.zero, new Vector2(440, 40), 13);
        }

        AddTMPText(panel.transform, "txtResultado", new Vector2(0, -160), new Vector2(460, 40), 14);
        AddTMPText(panel.transform, "txtPuntaje", new Vector2(0, -190), new Vector2(460, 30), 13);

        var btnSig = new GameObject("btnSiguiente");
        btnSig.transform.parent = panel.transform;
        var bImg = btnSig.AddComponent<UnityEngine.UI.Image>();
        bImg.color = new Color(0.1f, 0.6f, 0.2f);
        btnSig.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -220);
        btnSig.GetComponent<RectTransform>().sizeDelta = new Vector2(160, 36);
        btnSig.AddComponent<UnityEngine.UI.Button>();
        AddTMPText(btnSig.transform, "txtSig", Vector2.zero, new Vector2(160, 36), 14).GetComponent<TMPro.TextMeshProUGUI>().text = "Siguiente";

        panel.SetActive(false);
        go.AddComponent<QuizManager>();
    }

    static GameObject AddTMPText(Transform parent, string name, Vector2 anchoredPos, Vector2 size, float fontSize, FontStyles style = FontStyles.Normal)
    {
        var go = new GameObject(name);
        go.transform.parent = parent;
        var txt = go.AddComponent<TMPro.TextMeshProUGUI>();
        txt.fontSize = fontSize;
        txt.alignment = TMPro.TextAlignmentOptions.Center;
        txt.fontStyle = style;
        txt.color = Color.white;
        var rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = size;
        return go;
    }
}
#endif
