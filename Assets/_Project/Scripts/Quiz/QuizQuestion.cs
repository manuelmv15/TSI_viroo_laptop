using System;
using System.Collections.Generic;

[Serializable]
public class QuizQuestion
{
    public string pregunta;
    public List<string> opciones;
    public int indiceRespuestaCorrecta;
}
