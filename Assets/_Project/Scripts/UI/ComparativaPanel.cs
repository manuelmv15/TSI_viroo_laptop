using UnityEngine;
using UnityEngine.UI;

public class ComparativaPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button btnAbrir;
    [SerializeField] private Button btnCerrar;

    private void Awake()
    {
        if (panel == null) panel = transform.Find("Panel")?.gameObject;
        if (btnAbrir != null) btnAbrir.onClick.AddListener(AbrirPanel);
        if (btnCerrar != null) btnCerrar.onClick.AddListener(CerrarPanel);
        if (panel != null) panel.SetActive(false);
        AutoWireBoton3D();
    }

    public void AbrirPanel()
    {
        if (panel != null) panel.SetActive(true);
    }

    public void CerrarPanel()
    {
        if (panel != null) panel.SetActive(false);
    }

    private void AutoWireBoton3D()
    {
        var boton = GameObject.Find("BotonComparativa");
        if (boton == null) return;
        var interactable = boton.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        if (interactable == null) return;
        interactable.selectEntered.AddListener(_ => AbrirPanel());
    }
}
