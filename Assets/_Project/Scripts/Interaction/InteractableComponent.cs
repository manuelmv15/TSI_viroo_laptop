using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class InteractableComponent : XRGrabInteractable
{
    [SerializeField] public ComponentData data;
    public UnityEvent<ComponentData> OnSelected = new UnityEvent<ComponentData>();

    private Renderer[] renderers;
    private Material[][] originalMaterials;
    [SerializeField] private Material highlightMaterial;

    protected override void Awake()
    {
        base.Awake();
        movementType = MovementType.Kinematic;

        renderers = GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length][];
        for (int i = 0; i < renderers.Length; i++)
            originalMaterials[i] = renderers[i].sharedMaterials;
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        ApplyHighlight(true);
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        ApplyHighlight(false);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        OnSelected?.Invoke(data);
        ComponentSelector.Instance?.OnComponentSelected(data, transform);
    }

    private void ApplyHighlight(bool on)
    {
        if (highlightMaterial == null || renderers == null) return;
        for (int i = 0; i < renderers.Length; i++)
        {
            if (on)
            {
                var mats = new Material[originalMaterials[i].Length + 1];
                originalMaterials[i].CopyTo(mats, 0);
                mats[mats.Length - 1] = highlightMaterial;
                renderers[i].sharedMaterials = mats;
            }
            else
            {
                renderers[i].sharedMaterials = originalMaterials[i];
            }
        }
    }
}
