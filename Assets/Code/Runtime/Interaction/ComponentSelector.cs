using UnityEngine;

public class ComponentSelector : MonoBehaviour
{
    public static ComponentSelector Instance { get; private set; }
    [SerializeField] private ComponentInfoUI infoUI;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (infoUI == null)
            infoUI = FindFirstObjectByType<ComponentInfoUI>();
    }

    public void OnComponentSelected(ComponentData data, Transform origin)
    {
        infoUI?.Mostrar(data, origin);
    }

    public void CerrarPanel()
    {
        infoUI?.Cerrar();
    }
}
