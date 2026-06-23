using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource sourceNarracion;
    [SerializeField] private AudioSource sourceAmbiente;
    [SerializeField] private AudioSource sourceSFX;

    [Header("SFX")]
    public AudioClip sfxInstalacionCorrecta;
    public AudioClip sfxInstalacionIncorrecta;
    public AudioClip sfxQuizCorrecto;
    public AudioClip sfxQuizIncorrecto;

    [Header("Ambiente")]
    [SerializeField] private AudioClip clipAmbiente;
    [Range(0, 1)] [SerializeField] private float volumenAmbiente = 0.2f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        AutoWire();
    }

    private void AutoWire()
    {
        var srcs = GetComponents<AudioSource>();
        if (srcs.Length >= 1 && sourceNarracion == null) sourceNarracion = srcs[0];
        if (srcs.Length >= 2 && sourceAmbiente == null)  sourceAmbiente  = srcs[1];
        if (srcs.Length >= 3 && sourceSFX == null)       sourceSFX       = srcs[2];
    }

    private void Start()
    {
        if (clipAmbiente != null && sourceAmbiente != null)
        {
            sourceAmbiente.clip = clipAmbiente;
            sourceAmbiente.volume = volumenAmbiente;
            sourceAmbiente.loop = true;
            sourceAmbiente.Play();
        }
    }

    public void PlayNarration(AudioClip clip)
    {
        if (clip == null || sourceNarracion == null) return;
        sourceNarracion.Stop();
        sourceNarracion.clip = clip;
        sourceNarracion.Play();
    }

    public void StopNarration()
    {
        if (sourceNarracion != null) sourceNarracion.Stop();
    }

    public void PlayAmbient(AudioClip loop)
    {
        if (loop == null || sourceAmbiente == null) return;
        sourceAmbiente.clip = loop;
        sourceAmbiente.loop = true;
        sourceAmbiente.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sourceSFX == null) return;
        sourceSFX.PlayOneShot(clip);
    }
}
