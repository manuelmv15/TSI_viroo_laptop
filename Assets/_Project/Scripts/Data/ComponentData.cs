using UnityEngine;

[CreateAssetMenu(fileName = "ComponentData", menuName = "LaptopAssembly/Component Data")]
public class ComponentData : ScriptableObject
{
    public string nombre;
    [TextArea(2, 4)] public string funcion;
    [TextArea(2, 4)] public string caracteristicas;
    [TextArea(1, 3)] public string compatibilidad;
    public AudioClip clipAudioNarracion;
    public Sprite iconoUI;
    public int ordenEnsamblaje;
}
