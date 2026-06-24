using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [SerializeField] private GameObject panelQuiz;
    [SerializeField] private TextMeshProUGUI txtPregunta;
    [SerializeField] private List<Button> botonesOpciones;
    [SerializeField] private List<TextMeshProUGUI> txtOpciones;
    [SerializeField] private TextMeshProUGUI txtResultado;
    [SerializeField] private TextMeshProUGUI txtPuntaje;
    [SerializeField] private Button btnSiguiente;

    private List<QuizQuestion> preguntas;
    private int preguntaActual;
    private int puntaje;
    private bool respondida;

    private void Awake()
    {
        preguntas = InicializarPreguntas();
        AutoWire();
        if (btnSiguiente != null) btnSiguiente.onClick.AddListener(SiguientePregunta);
        if (panelQuiz != null) panelQuiz.SetActive(false);
    }

    private void AutoWire()
    {
        if (panelQuiz != null) return;
        panelQuiz = transform.Find("PanelQuiz")?.gameObject;
        if (panelQuiz == null) return;

        txtPregunta  = panelQuiz.transform.Find("txtPregunta")?.GetComponent<TextMeshProUGUI>();
        txtResultado = panelQuiz.transform.Find("txtResultado")?.GetComponent<TextMeshProUGUI>();
        txtPuntaje   = panelQuiz.transform.Find("txtPuntaje")?.GetComponent<TextMeshProUGUI>();
        btnSiguiente = panelQuiz.transform.Find("btnSiguiente")?.GetComponent<Button>();

        botonesOpciones = new List<Button>();
        txtOpciones     = new List<TextMeshProUGUI>();
        for (int i = 0; i < 4; i++)
        {
            var btnGO = panelQuiz.transform.Find($"btnOpcion{i}");
            if (btnGO == null) continue;
            botonesOpciones.Add(btnGO.GetComponent<Button>());
            txtOpciones.Add(btnGO.GetComponentInChildren<TextMeshProUGUI>());
        }
    }

    public void IniciarQuiz()
    {
        preguntaActual = 0;
        puntaje = 0;
        if (panelQuiz != null) panelQuiz.SetActive(true);
        MostrarPregunta();
    }

    private void MostrarPregunta()
    {
        if (preguntaActual >= preguntas.Count) { MostrarResultadoFinal(); return; }

        var q = preguntas[preguntaActual];
        if (txtPregunta != null) txtPregunta.text = q.pregunta;
        if (txtResultado != null) txtResultado.text = "";
        respondida = false;

        for (int i = 0; i < botonesOpciones.Count; i++)
        {
            bool visible = i < q.opciones.Count;
            botonesOpciones[i].gameObject.SetActive(visible);
            if (!visible) continue;
            if (i < txtOpciones.Count && txtOpciones[i] != null)
                txtOpciones[i].text = q.opciones[i];
            int idx = i;
            botonesOpciones[i].onClick.RemoveAllListeners();
            botonesOpciones[i].onClick.AddListener(() => Responder(idx));
            botonesOpciones[i].interactable = true;
        }

        if (btnSiguiente != null) btnSiguiente.gameObject.SetActive(false);
    }

    private void Responder(int idx)
    {
        if (respondida) return;
        respondida = true;

        var q = preguntas[preguntaActual];
        bool correcto = idx == q.indiceRespuestaCorrecta;

        if (correcto) { puntaje++; if (txtResultado != null) txtResultado.text = "¡Correcto!"; AudioManager.Instance?.PlaySFX(AudioManager.Instance.sfxQuizCorrecto); }
        else { if (txtResultado != null) txtResultado.text = $"Incorrecto. Respuesta: {q.opciones[q.indiceRespuestaCorrecta]}"; AudioManager.Instance?.PlaySFX(AudioManager.Instance.sfxQuizIncorrecto); }

        foreach (var btn in botonesOpciones) btn.interactable = false;
        if (btnSiguiente != null) btnSiguiente.gameObject.SetActive(true);
    }

    private void SiguientePregunta() { preguntaActual++; MostrarPregunta(); }

    private void MostrarResultadoFinal()
    {
        if (txtPregunta != null) txtPregunta.text = "Quiz completado";
        foreach (var btn in botonesOpciones) btn.gameObject.SetActive(false);
        if (txtResultado != null) txtResultado.text = "";
        if (txtPuntaje != null) txtPuntaje.text = $"Puntaje: {puntaje}/{preguntas.Count}";
        if (btnSiguiente != null) btnSiguiente.gameObject.SetActive(false);
    }

    private List<QuizQuestion> InicializarPreguntas() => new List<QuizQuestion>
    {
        new QuizQuestion { pregunta = "¿Qué componente almacena datos de forma permanente y de alta velocidad?", opciones = new List<string> { "Memoria RAM", "SSD", "Tarjeta madre", "Sistema de enfriamiento" }, indiceRespuestaCorrecta = 1 },
        new QuizQuestion { pregunta = "¿Qué componente suministra energía portátil a la laptop?", opciones = new List<string> { "Pantalla", "Teclado", "Batería", "Touchpad" }, indiceRespuestaCorrecta = 2 },
        new QuizQuestion { pregunta = "¿Qué componente conecta e integra todos los demás componentes?", opciones = new List<string> { "SSD", "Tarjeta madre", "Memoria RAM", "Sistema de enfriamiento" }, indiceRespuestaCorrecta = 1 },
        new QuizQuestion { pregunta = "¿Qué tipo de memoria es el almacenamiento temporal en uso?", opciones = new List<string> { "SSD", "Batería", "Memoria RAM", "Pantalla" }, indiceRespuestaCorrecta = 2 },
        new QuizQuestion { pregunta = "¿Qué componente disipa el calor de la CPU y la GPU?", opciones = new List<string> { "Teclado", "Touchpad", "Tarjeta madre", "Sistema de enfriamiento" }, indiceRespuestaCorrecta = 3 }
    };
}
