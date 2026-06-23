using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComponentInfoUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI txtNombre;
    [SerializeField] private TextMeshProUGUI txtFuncion;
    [SerializeField] private TextMeshProUGUI txtCaracteristicas;
    [SerializeField] private TextMeshProUGUI txtCompatibilidad;
    [SerializeField] private Image imgIcono;
    [SerializeField] private Button btnCerrar;

    [Header("Posicionamiento")]
    [SerializeField] private Vector3 offsetFromComponent = new Vector3(0, 0.5f, 0);

    private void Awake()
    {
        AutoWire();
        if (btnCerrar != null) btnCerrar.onClick.AddListener(Cerrar);
        if (panel != null) panel.SetActive(false);
    }

    private void AutoWire()
    {
        if (panel != null) return;
        panel = transform.Find("Panel")?.gameObject;
        if (panel == null) return;
        txtNombre         = panel.transform.Find("txtNombre")?.GetComponent<TextMeshProUGUI>();
        txtFuncion        = panel.transform.Find("txtFuncion")?.GetComponent<TextMeshProUGUI>();
        txtCaracteristicas = panel.transform.Find("txtCaracteristicas")?.GetComponent<TextMeshProUGUI>();
        txtCompatibilidad  = panel.transform.Find("txtCompatibilidad")?.GetComponent<TextMeshProUGUI>();
        btnCerrar         = panel.transform.Find("btnCerrar")?.GetComponent<Button>();
    }

    public void Mostrar(ComponentData data, Transform componentTransform)
    {
        if (data == null || panel == null) return;

        if (txtNombre != null) txtNombre.text = data.nombre;
        if (txtFuncion != null) txtFuncion.text = data.funcion;
        if (txtCaracteristicas != null) txtCaracteristicas.text = data.caracteristicas;
        if (txtCompatibilidad != null) txtCompatibilidad.text = data.compatibilidad;
        if (imgIcono != null) imgIcono.sprite = data.iconoUI;

        transform.position = componentTransform.position + offsetFromComponent;
        var cam = Camera.main;
        if (cam != null)
        {
            transform.LookAt(cam.transform);
            transform.Rotate(0, 180, 0);
        }

        panel.SetActive(true);
        AudioManager.Instance?.PlayNarration(data.clipAudioNarracion);
    }

    public void Cerrar()
    {
        if (panel != null) panel.SetActive(false);
        AudioManager.Instance?.StopNarration();
    }
}
