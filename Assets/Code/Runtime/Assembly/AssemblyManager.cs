using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class AssemblyManager : MonoBehaviour
{
    public static AssemblyManager Instance { get; private set; }

    [SerializeField] private List<AssemblySlot> slots = new List<AssemblySlot>();
    [SerializeField] private List<InteractableComponent> piezas = new List<InteractableComponent>();
    [SerializeField] private TextMeshProUGUI txtProgreso;
    [SerializeField] private TextMeshProUGUI txtInstruccion;
    [SerializeField] private QuizManager quizManager;

    public UnityEvent OnAssemblyComplete = new UnityEvent();

    private int instalados = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        AutoWire();
    }

    private void AutoWire()
    {
        if (slots.Count == 0)
        {
            var chassis = GameObject.Find("LaptopChassis");
            if (chassis != null)
                slots.AddRange(chassis.GetComponentsInChildren<AssemblySlot>());
        }
        if (piezas.Count == 0)
        {
            var compContainer = GameObject.Find("Components");
            if (compContainer != null)
                piezas.AddRange(compContainer.GetComponentsInChildren<InteractableComponent>());
        }
        if (quizManager == null)
            quizManager = FindFirstObjectByType<QuizManager>();

        if (txtProgreso == null || txtInstruccion == null)
        {
            var hud = GameObject.Find("AssemblyHUD");
            if (hud != null)
            {
                var bg = hud.transform.Find("Background");
                if (bg != null)
                {
                    if (txtProgreso == null)
                        txtProgreso = bg.Find("txtProgreso")?.GetComponent<TextMeshProUGUI>();
                    if (txtInstruccion == null)
                        txtInstruccion = bg.Find("txtInstruccion")?.GetComponent<TextMeshProUGUI>();
                }
            }
        }
    }

    private void Start()
    {
        foreach (var slot in slots)
            slot.OnComponentInstalled.AddListener(OnPiezaInstalada);

        if (quizManager != null)
            OnAssemblyComplete.AddListener(quizManager.IniciarQuiz);

        ActualizarUI();
        ResaltarSiguiente();
    }

    private void OnPiezaInstalada(ComponentData data)
    {
        instalados++;
        ActualizarUI();
        ResaltarSiguiente();

        if (instalados >= slots.Count)
        {
            if (txtInstruccion != null) txtInstruccion.text = "¡Ensamblaje completo!";
            OnAssemblyComplete?.Invoke();
        }
    }

    private void ResaltarSiguiente()
    {
        var siguienteSlot = slots
            .Where(s => !s.Instalado)
            .OrderBy(s => s.componenteEsperado != null ? s.componenteEsperado.ordenEnsamblaje : 99)
            .FirstOrDefault();

        if (siguienteSlot != null && siguienteSlot.componenteEsperado != null && txtInstruccion != null)
            txtInstruccion.text = $"Instala: {siguienteSlot.componenteEsperado.nombre}";
    }

    private void ActualizarUI()
    {
        if (txtProgreso != null)
            txtProgreso.text = $"{instalados}/{slots.Count} piezas instaladas";
    }
}
