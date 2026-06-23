using CustomCode;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName ="Assembly Part Editor Config")]
public class AssemblyPartEditorConfig : ScriptableObject
{
    public ScriptGraphAsset assemblyTargetCompletableSM;
    public Color partsSectionBackground = Color.gray;
    public Color partTargetsSectionBackground = Color.gray;
    public Color partListItemColor = Color.gray;
    public Color partTargetListItemColor = Color.gray;

    public Material partFeedbackMaterial = null;
    public Material startFeedbackMaterial = null;
    public Material targetFeedbackMaterial = null;
    public Material targetFeedbackHoverMaterial = null;

    public string lastAssemblyGroupGlobalId;
}
