using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class AssemblySlot : MonoBehaviour
{
    [SerializeField] public ComponentData componenteEsperado;
    public bool Instalado { get; private set; }

    public UnityEvent<ComponentData> OnComponentInstalled = new UnityEvent<ComponentData>();

    [SerializeField] private GameObject indicadorLibre;
    [SerializeField] private GameObject indicadorInstalado;
    [SerializeField] private float animDuration = 0.35f;

    private void Start() => ActualizarIndicadores();

    private void OnTriggerEnter(Collider other)
    {
        if (Instalado) return;
        var ic = other.GetComponentInParent<InteractableComponent>();
        if (ic == null || ic.data != componenteEsperado) return;

        Instalar(ic.gameObject);
    }

public void Instalar(GameObject piezaGO)
    {
        if (Instalado) return;
        Instalado = true;

        var ic = piezaGO.GetComponent<InteractableComponent>();
        if (ic != null)
        {
            // Force-release the grab before taking control of the transform
            var manager = UnityEngine.Object.FindFirstObjectByType<UnityEngine.XR.Interaction.Toolkit.XRInteractionManager>();
            if (manager != null)
            {
                foreach (var interactor in ic.interactorsSelecting.ToArray())
                    manager.SelectExit(interactor, ic);
            }
            ic.enabled = false;
        }

        ActualizarIndicadores();
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.sfxInstalacionCorrecta);
        StartCoroutine(AnimarInstalacion(piezaGO));
        OnComponentInstalled?.Invoke(componenteEsperado);
    }

private IEnumerator AnimarInstalacion(GameObject piezaGO)
    {
        Vector3 startPos = piezaGO.transform.position;
        Quaternion startRot = piezaGO.transform.rotation;
        Vector3 targetPos = transform.position;
        Quaternion targetRot = transform.rotation;

        piezaGO.transform.SetParent(transform);

        float elapsed = 0f;
        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / animDuration);
            piezaGO.transform.position = Vector3.Lerp(startPos, targetPos, t);
            piezaGO.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        piezaGO.transform.localPosition = Vector3.zero;
        piezaGO.transform.localRotation = Quaternion.identity;
    }


    private void ActualizarIndicadores()
    {
        if (indicadorLibre != null) indicadorLibre.SetActive(!Instalado);
        if (indicadorInstalado != null) indicadorInstalado.SetActive(Instalado);
    }
}
