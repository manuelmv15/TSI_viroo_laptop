// Editor helper — solo se ejecuta desde el menu Tools > LaptopAssembly > Create Component Data
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class CreateComponentDataEditor
{
    [MenuItem("Tools/LaptopAssembly/Crear ComponentData assets")]
    public static void CreateAll()
    {
        var defs = new[]
        {
            new { nombre = "Pantalla", funcion = "Muestra la salida visual del sistema al usuario. Es la interfaz principal de visualización de la laptop.", caracteristicas = "Resolución Full HD o superior, tipo de panel LCD/IPS/OLED, tasa de refresco 60-144 Hz, retroiluminación LED", compatibilidad = "Depende del conector de video (LVDS/eDP) y el controlador de pantalla integrado en la tarjeta madre", orden = 8 },
            new { nombre = "Batería", funcion = "Suministra energía eléctrica portable a todos los componentes de la laptop cuando no está conectada a la red.", caracteristicas = "Capacidad en Wh o mAh, voltaje nominal 10.8V-15.4V, tecnología Li-Ion o Li-Polymer, ciclos de carga 300-1000", compatibilidad = "Debe coincidir en voltaje, conector y dimensiones físicas con el modelo específico de laptop", orden = 5 },
            new { nombre = "Tarjeta Madre", funcion = "Conecta e integra todos los componentes del sistema, distribuyendo datos y energía entre ellos.", caracteristicas = "Socket de CPU, slots de RAM DDR4/DDR5, conectores M.2 y SATA, chipset integrado, puertos de E/S", compatibilidad = "Define qué generación de CPU, tipo y velocidad de RAM, y estándares de almacenamiento soporta", orden = 1 },
            new { nombre = "Memoria RAM", funcion = "Almacena temporalmente los datos e instrucciones que el procesador está usando activamente en tiempo real.", caracteristicas = "Capacidad 8-64 GB, tipo DDR4 o DDR5, frecuencia 2400-6400 MHz, formato SO-DIMM para laptops", compatibilidad = "Debe coincidir en tipo (DDR4/DDR5), velocidad y número de pines soportados por la tarjeta madre", orden = 2 },
            new { nombre = "SSD", funcion = "Almacena permanentemente el sistema operativo, aplicaciones y archivos del usuario con alta velocidad de lectura/escritura.", caracteristicas = "Capacidad 256 GB-4 TB, interfaz NVMe (PCIe) o SATA, velocidades de lectura hasta 7000 MB/s, sin partes móviles", compatibilidad = "Depende de la interfaz disponible (M.2 NVMe, M.2 SATA o 2.5\" SATA) en la tarjeta madre", orden = 3 },
            new { nombre = "Sistema de Enfriamiento", funcion = "Disipa el calor generado por el CPU y GPU para mantener temperaturas de operación seguras y estables.", caracteristicas = "Ventilador(es) de cobre o aluminio, heatpipes de transferencia térmica, pasta térmica, control PWM de velocidad", compatibilidad = "Debe ajustarse al chasis del modelo específico y al TDP del procesador instalado", orden = 4 },
            new { nombre = "Teclado", funcion = "Permite al usuario introducir texto, comandos y caracteres al sistema de forma directa.", caracteristicas = "Distribución regional (QWERTY/AZERTY), retroiluminación LED opcional, recorrido de tecla 1.2-1.5 mm, conector de membrana", compatibilidad = "Específico por modelo y fabricante; se conecta vía cable flex propio del chasis de la laptop", orden = 6 },
            new { nombre = "Touchpad", funcion = "Dispositivo de entrada táctil que controla el cursor del sistema y detecta gestos multitáctiles del usuario.", caracteristicas = "Superficie capacitiva, soporte de gestos multipunto, botones integrados, área de detección precisa", compatibilidad = "Se conecta a la tarjeta madre mediante cable flex específico del modelo; interfaz I2C o PS/2", orden = 7 },
        };

        string path = "Assets/_Project/Data";
        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder("Assets/_Project", "Data");

        foreach (var d in defs)
        {
            var asset = ScriptableObject.CreateInstance<ComponentData>();
            asset.nombre = d.nombre;
            asset.funcion = d.funcion;
            asset.caracteristicas = d.caracteristicas;
            asset.compatibilidad = d.compatibilidad;
            asset.ordenEnsamblaje = d.orden;
            string safeName = d.nombre.Replace(" ", "_");
            AssetDatabase.CreateAsset(asset, $"{path}/CD_{safeName}.asset");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("ComponentData assets creados en Assets/_Project/Data/");
    }
}
#endif
