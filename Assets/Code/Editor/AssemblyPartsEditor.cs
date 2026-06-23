using CustomCode;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Unity.VisualScripting;

public class AssemblyPartsEditor : EditorWindow
{
    public class SubWindowStyle
    {
        public GUIStyle GetStyle()
        {
            return style;
        }

        private GUIStyle style;
        private Texture2D texture = new Texture2D(1, 1);

        public SubWindowStyle(Color color)
        {
            style = new GUIStyle();
            style.border = new RectOffset(2, 2, 2, 2);
            style.padding = new RectOffset(5, 5, 5, 5);
            style.margin = new RectOffset(5, 5, 5, 5);
            style.alignment = TextAnchor.MiddleCenter;

            texture.SetPixel(0, 0, color);
            texture.Apply();
            style.normal.background = texture;
        }
    }

    AssemblyGroup assemblyGroup = null;

    SubWindowStyle partItemStyle;
    SubWindowStyle partsSectionStyle;
    SubWindowStyle partTargetItemStyle;
    SubWindowStyle partTargetsSectionStyle;

    AssemblyPartEditorConfig config = null;

    Vector2 scrollPosition = Vector2.zero;

    List<AssemblyPart> selectedAssemblyParts = new List<AssemblyPart>();

    [MenuItem("Window/Viroo/Viroo Studio Templates/Assembly Parts Editor")]
    public static void ShowWindow()
    {
        GetWindow<AssemblyPartsEditor>("Assembly Parts Editor");
    }

    private void OnSelectionChange()
    {
        Repaint();
    }

    private void OnGUI()
    {
        config = Resources.Load<AssemblyPartEditorConfig>("Config/AssemblyPartEditorConfig");

        partItemStyle = new SubWindowStyle(config.partListItemColor);
        partsSectionStyle = new SubWindowStyle(config.partsSectionBackground);

        partTargetItemStyle = new SubWindowStyle(config.partTargetListItemColor);
        partTargetsSectionStyle = new SubWindowStyle(config.partTargetsSectionBackground);

        // Start drawing
        GUILayout.Space(20);
        GUILayout.Label("Target Assembly Group", EditorStyles.boldLabel);

        if (GlobalObjectId.TryParse(config.lastAssemblyGroupGlobalId, out GlobalObjectId globalObjectId))
        {
            assemblyGroup = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(globalObjectId) as AssemblyGroup;
        }
        else
        {
            assemblyGroup = null;
        }

        assemblyGroup = EditorGUILayout.ObjectField(assemblyGroup, typeof(AssemblyGroup), true) as AssemblyGroup;

        config.lastAssemblyGroupGlobalId = GlobalObjectId.GetGlobalObjectIdSlow(assemblyGroup).ToString();

        if (assemblyGroup != null)
        {
            EditorGUILayout.BeginHorizontal();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            EditorGUILayout.BeginVertical();

            DrawPartsGUI();
            DrawPartsTargetsGUI();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawPartsTargetsGUI()
    {
        EditorGUILayout.BeginVertical(partTargetsSectionStyle.GetStyle());

        GUILayout.Label("Assembly Targets", EditorStyles.boldLabel);

        List<AssemblyTarget> assemblyTargets = assemblyGroup.assemblyTargets;
        assemblyTargets.RemoveAll(item => item == null);

        if (Selection.activeGameObject != null)
        {
            AssemblyPart selectedAssemblyPart = Selection.activeGameObject.GetComponent<AssemblyPart>();
            if (selectedAssemblyPart != null)
            {
                assemblyTargets = assemblyTargets.Where(item => item.partType == selectedAssemblyPart.partType).ToList();
            }
        }

        foreach (AssemblyTarget assemblyTarget in assemblyTargets)
        {
            CompletableAssemblyTargetData completableAssemblyTargetData = assemblyTarget.GetComponent<CompletableAssemblyTargetData>();

            EditorGUILayout.BeginVertical(partTargetItemStyle.GetStyle()); // Item

            EditorGUILayout.BeginHorizontal(); // Buttons bar

            if (GUILayout.Button("Select Target"))
            {
                Selection.activeObject = assemblyTarget;
            }
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                DestroyImmediate(assemblyTarget.gameObject);

                // Need to ensure layout is not broken
                EditorGUILayout.EndHorizontal(); // buttons bar break
                EditorGUILayout.EndVertical(); break; // item break
            }

            EditorGUILayout.EndHorizontal(); // buttons bar

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", GUILayout.Width(40));
            completableAssemblyTargetData.gameObject.name = EditorGUILayout.TextField(completableAssemblyTargetData.gameObject.name);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Type", GUILayout.Width(40));
            assemblyTarget.partType = EditorGUILayout.TextField(assemblyTarget.partType);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Tolerance Dist / Angle", GUILayout.Width(175));
            completableAssemblyTargetData.distanceTolerance = EditorGUILayout.FloatField(completableAssemblyTargetData.distanceTolerance);
            completableAssemblyTargetData.angleTolerance = EditorGUILayout.FloatField(completableAssemblyTargetData.angleTolerance);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Disable Part when Placed", GUILayout.Width(175));
            completableAssemblyTargetData.disableInteractableWhenPlaced = EditorGUILayout.Toggle(completableAssemblyTargetData.disableInteractableWhenPlaced);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Hovering Completion / Time", GUILayout.Width(175));
            completableAssemblyTargetData.allowHoveringCompletion = EditorGUILayout.Toggle(completableAssemblyTargetData.allowHoveringCompletion);
            completableAssemblyTargetData.hoveringTimeToComplete = EditorGUILayout.FloatField(completableAssemblyTargetData.hoveringTimeToComplete);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical(); // item
        }

        selectedAssemblyParts.Clear();
        foreach (GameObject gameObject in Selection.gameObjects)
        {
            AssemblyPart part = gameObject.GetComponent<AssemblyPart>();
            if (part != null)
            {
                selectedAssemblyParts.Add(part);
            }
        }

        if (selectedAssemblyParts.Count > 0)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.BeginVertical();

            GUILayout.Label("Create Assembly Targets", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 50;

            if (GUILayout.Button("Create"))
            {
                CreateAssemblyTargetForSelectedObjects(selectedAssemblyParts);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical(); // subwindow
    }

    private void DrawPartsGUI()
    {
        EditorGUILayout.BeginVertical(partsSectionStyle.GetStyle());

        GUILayout.Label("Assembly Parts", EditorStyles.boldLabel);
        List<AssemblyPart> assemblyParts = assemblyGroup.assemblyParts;
        assemblyParts.RemoveAll(item => item == null);

        foreach (AssemblyPart assemblyPart in assemblyParts)
        {
            EditorGUILayout.BeginVertical(partItemStyle.GetStyle()); // Item

            EditorGUILayout.BeginHorizontal(); // Buttons bar

            if (GUILayout.Button($"Select Part"))
            {
                Selection.activeObject = assemblyPart;
            }

            if (GUILayout.Button("Select Start"))
            {
                Selection.activeObject = assemblyPart.startTransform;
            }

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                FeedBack feedbackRem = assemblyPart.GetComponent<FeedBack>();
                if (feedbackRem != null)
                {
                    DestroyImmediate(feedbackRem.feedbackGameObject);
                }

                UniqueID uniqueId = assemblyPart.GetComponent<UniqueID>();
                if (uniqueId != null)
                {
                    DestroyImmediate(uniqueId);
                }

                if (assemblyPart.startTransform != null)
                {
                    DestroyImmediate(assemblyPart.startTransform.gameObject);
                }
                DestroyImmediate(feedbackRem);
                assemblyParts.Remove(assemblyPart);
                DestroyImmediate(assemblyPart);

                // Need to ensure layout is not broken
                EditorGUILayout.EndHorizontal(); // Buttons bar break
                EditorGUILayout.EndVertical(); break; // item break
            }
            EditorGUILayout.EndHorizontal(); // Buttons bar

            EditorGUILayout.BeginVertical(); // fields

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", GUILayout.Width(40));
            assemblyPart.gameObject.name = EditorGUILayout.TextField(assemblyPart.gameObject.name);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Type", GUILayout.Width(40));
            assemblyPart.partType = EditorGUILayout.TextField(assemblyPart.partType);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical(); // fields

            EditorGUILayout.EndVertical(); // item
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10);
        EditorGUILayout.BeginVertical();

        if (Selection.gameObjects.Count() == 1 &&
            Selection.activeObject.GetComponent<AssemblyPart>() == null &&
            Selection.activeObject.GetComponent<AssemblyTarget>() == null &&
            Selection.activeObject.GetComponent<CompletableItemData>() == null)
        {

            if (GUILayout.Button("Create"))
            {
                CreateAssemblyPartsForSelectedObjects();
            }
        }
        else
        {
            //GUILayout.Label("Select target Gameobjects as Part", EditorStyles.boldLabel);
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical(); // subwindow
    }

    private void CreateAssemblyPartsForSelectedObjects()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        foreach (GameObject gameObject in selectedObjects)
        {
            AssemblyPart assemblyPart = gameObject.GetComponent<AssemblyPart>();
            if (assemblyPart == null)
            {
                assemblyPart = gameObject.AddComponent<AssemblyPart>();
                assemblyGroup.assemblyParts.Add(assemblyPart);
            }

            // set part properties
            assemblyPart.id = gameObject.name;
            assemblyPart.partType = gameObject.name;

            // add instanced hierardchy as visual feedback
            GameObject startFeedbackGO = new GameObject();
            startFeedbackGO.name = $"FEEDBACK {assemblyPart.name}";
            startFeedbackGO.transform.SetParent(assemblyPart.transform, false);
            InstancedRenderersHierarchy instancedPartFeedback = startFeedbackGO.AddComponent<InstancedRenderersHierarchy>();
            instancedPartFeedback.material = config.partFeedbackMaterial;
            instancedPartFeedback.source = assemblyPart.gameObject;
            instancedPartFeedback.editorOnly = false;
            // Add feedback component to allow managing state
            assemblyPart.gameObject.AddComponent<FeedBack>().feedbackGameObject = startFeedbackGO;

            // Feedback for starting position
            GameObject start = new GameObject();
            start.name = $"START {assemblyPart.name}";

            start.transform.parent = assemblyPart.transform.parent;
            start.transform.localPosition = assemblyPart.transform.localPosition;
            start.transform.localRotation = assemblyPart.transform.localRotation;
            start.transform.localScale = assemblyPart.transform.localScale;

            InstancedRenderersHierarchy startFeedbackInstanced = start.AddComponent<InstancedRenderersHierarchy>();
            startFeedbackInstanced.material = config.startFeedbackMaterial;
            startFeedbackInstanced.source = assemblyPart.gameObject;
            startFeedbackInstanced.editorOnly = true;

            assemblyPart.startTransform = start.transform;
        }
    }

    private void CreateAssemblyTargetForSelectedObjects(List<AssemblyPart> assemblyParts)
    {
        foreach (AssemblyPart assemblyPart in assemblyParts)
        {
            GameObject obj = assemblyPart.gameObject;

            GameObject assemblyTargetGO = new GameObject($"{obj.name}-target");
            assemblyTargetGO.transform.parent = obj.transform.parent;
            assemblyTargetGO.transform.localPosition = obj.transform.localPosition;
            assemblyTargetGO.transform.localRotation = obj.transform.localRotation;
            assemblyTargetGO.transform.localScale = obj.transform.localScale;

            Collider[] colliders = assemblyPart.GetComponentsInChildren<Collider>();

            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);

            if (renderers.Length > 0)
            {
                Bounds combinedBounds = renderers[0].bounds;

                for (int i = 1; i < renderers.Length; i++)
                {
                    combinedBounds.Encapsulate(renderers[i].bounds);
                }

                BoxCollider rootBoxCollider = assemblyTargetGO.AddComponent<BoxCollider>();
                rootBoxCollider.isTrigger = true;

                Vector3 localCenter = assemblyTargetGO.transform.InverseTransformPoint(combinedBounds.center);

                Vector3 localSize = combinedBounds.size;
                Vector3 lossyScale = assemblyTargetGO.transform.lossyScale;

                localSize = new Vector3(
                    lossyScale.x != 0 ? localSize.x / lossyScale.x : localSize.x,
                    lossyScale.y != 0 ? localSize.y / lossyScale.y : localSize.y,
                    lossyScale.z != 0 ? localSize.z / lossyScale.z : localSize.z
                );

                rootBoxCollider.center = localCenter;
                rootBoxCollider.size = localSize;
            }

            // Create AssemblyTarget component
            AssemblyTarget assemblyTarget = assemblyTargetGO.AddComponent<AssemblyTarget>();
            assemblyTarget.partType = assemblyPart.partType;
            assemblyTarget.feedbackMaterial = config.targetFeedbackMaterial;
            assemblyTarget.feedbackMaterialHover = config.targetFeedbackHoverMaterial;

            // Create AssemblyTarget completable
            CompletableAssemblyTargetData completableAssemblyTargetData = assemblyTargetGO.AddComponent<CompletableAssemblyTargetData>();
            completableAssemblyTargetData.distanceTolerance = 2;
            completableAssemblyTargetData.angleTolerance = 180;
            completableAssemblyTargetData.disableInteractableWhenPlaced = true;

            // Add ScriptMachine to handle Part Target completions 
            ScriptMachine scriptMachine = assemblyTargetGO.AddComponent<ScriptMachine>();
            scriptMachine.nest.SwitchToMacro(config.assemblyTargetCompletableSM);

            // Add target feedback as a child to allow disabling without affecting others
            GameObject feedbackGo = new GameObject();
            feedbackGo.name = $"{obj.name}-feedback";
            feedbackGo.transform.SetParent(assemblyTargetGO.transform, false);

            InstancedRenderersHierarchy targetFeedbackInstanced = feedbackGo.AddComponent<InstancedRenderersHierarchy>();
            targetFeedbackInstanced.material = config.targetFeedbackMaterial;
            targetFeedbackInstanced.source = assemblyPart.gameObject;
            targetFeedbackInstanced.editorOnly = false;

            FeedBack feedBack = assemblyTargetGO.AddComponent<FeedBack>();
            feedBack.feedbackGameObject = feedbackGo;

            assemblyGroup.assemblyTargets.Add(assemblyTarget);
        }
    }

}
