# Plan de Ejecución — Simulador de Ensamblaje de Laptop en Unity/Viroo
### Para usar con Claude Code conectado vía MCP a Unity

Este plan está diseñado para que se lo entregues a Claude Code (con el MCP de Unity activo) en fases. Cada fase trae un objetivo, los archivos/GameObjects que debe crear, y un **prompt sugerido** que puedes copiar/pegar directamente.

---

## 0. Requisitos del proyecto (resumen de la guía)

**Componentes obligatorios de la laptop** (8, todos deben incluirse):
1. Pantalla
2. Batería
3. Tarjeta madre
4. Memoria RAM
5. SSD
6. Sistema de enfriamiento
7. Teclado
8. Touchpad

**Al seleccionar un componente debe mostrarse:**
- Nombre
- Función
- Características principales
- Compatibilidad

**Valor agregado (puntos extra, no obligatorios pero recomendados):**
- Ensamblaje guiado
- Animaciones de instalación
- Evaluaciones interactivas
- Comparativa PC vs Laptop

**Requisitos generales obligatorios:**
- Plantilla oficial de Viroo para Unity
- Escena 3D funcional con iluminación adecuada
- Modelos 3D optimizados
- Navegación implementada
- Elementos interactivos
- Contenido educativo
- Pruebas de funcionamiento
- **Audios explicativos/inmersivos** (la guía los marca como indispensables)

**Criterio eliminatorio:** si no corre dentro de Viroo (solo en Unity Editor), la nota es **0.0** sin importar lo demás. Por eso el testing dentro de Viroo se hace desde el día 1, no al final.

**Rúbrica:**
| Criterio | % |
|---|---|
| Diseño y organización de la escena | 15 |
| Calidad de modelos 3D | 15 |
| Interactividad y programación | 20 |
| Contenido educativo | 15 |
| Funcionamiento en Viroo | 20 |
| Documentación técnica | 5 |
| Presentación final | 10 |

---

## 1. Setup inicial

**Objetivo:** tener el proyecto Unity con la plantilla de Viroo corriendo y el MCP conectado, antes de tocar contenido.

Pasos:
1. Crear/clonar el proyecto Unity a partir de la plantilla oficial de Viroo.
2. Verificar versión de Unity compatible con la plantilla.
3. Confirmar que el MCP de Unity responde desde Claude Code (probar un comando simple: listar GameObjects de la escena).
4. Hacer un build de prueba mínimo (escena vacía con el rig de Viroo) y comprobar que carga correctamente dentro de Viroo en el visor/headset. **No avanzar hasta que esto funcione.**

**Prompt sugerido para Claude Code:**
> "Conéctate al proyecto Unity vía MCP, lista la jerarquía de la escena actual de la plantilla de Viroo, y confírmame qué rig de cámara/locomoción usa la plantilla. No modifiques nada todavía, solo dame el reporte."

---

## 2. Estructura de carpetas y escena base

```
Assets/
 ├─ _Project/
 │   ├─ Scenes/          (LaptopAssembly.unity)
 │   ├─ Models/          (laptop, componentes)
 │   ├─ Prefabs/
 │   │   ├─ Components/  (un prefab por pieza)
 │   │   └─ UI/
 │   ├─ Scripts/
 │   │   ├─ Interaction/
 │   │   ├─ Assembly/
 │   │   ├─ Quiz/
 │   │   └─ Data/
 │   ├─ Audio/
 │   │   ├─ Narration/
 │   │   └─ Ambient/
 │   ├─ Materials/
 │   └─ Data/            (ScriptableObjects de cada componente)
```

**Escena sugerida:** una mesa de taller/laboratorio tech (banco de trabajo) donde la laptop desarmada está sobre la mesa y las piezas alrededor, esperando ensamblaje.

**Prompt sugerido:**
> "Crea la estructura de carpetas Assets/_Project/... que te indico. Luego crea una nueva escena LaptopAssembly.unity basada en la plantilla de Viroo (copia el rig de jugador/locomoción de la escena demo) con un piso, una mesa de trabajo simple (puede ser un cubo escalado como placeholder) y luz ambiental + una luz direccional configurada para interior de taller (temperatura cálida, ~3000-4000K)."

---

## 3. Datos de cada componente (ScriptableObjects)

Antes de modelar/programar interacciones, define los datos para que el sistema sea genérico (un solo script de info, 8 instancias de datos).

Crear `ComponentData` (ScriptableObject) con campos:
- `nombre` (string)
- `funcion` (string, 2-3 líneas)
- `caracteristicas` (string o lista)
- `compatibilidad` (string)
- `clipAudioNarracion` (AudioClip)
- `iconoUI` (Sprite, opcional)
- `ordenEnsamblaje` (int, para el modo guiado)

Crear 8 assets de datos, uno por componente, con contenido real y correcto (no placeholder), por ejemplo:

| Componente | Función (resumen) | Compatibilidad (resumen) |
|---|---|---|
| Pantalla | Muestra la salida visual del sistema | Depende de resolución, tipo de panel (LCD/OLED) y conector con la tarjeta madre |
| Batería | Suministra energía portátil | Debe coincidir en voltaje/capacidad y conector con el modelo de laptop |
| Tarjeta madre | Conecta e integra todos los componentes | Define qué CPU, RAM y almacenamiento soporta |
| Memoria RAM | Almacenamiento temporal de datos en uso | Debe coincidir en tipo (DDR4/DDR5) y velocidad soportada por la tarjeta madre |
| SSD | Almacenamiento permanente de alta velocidad | Depende de interfaz (SATA, NVMe/M.2) soportada |
| Sistema de enfriamiento | Disipa el calor generado por CPU/GPU | Debe ajustarse al tamaño/socket del chasis y TDP del procesador |
| Teclado | Entrada de datos del usuario | Específico por modelo/idioma/distribución de teclas |
| Touchpad | Dispositivo de entrada táctil para el cursor | Conectado a la tarjeta madre vía cable flex propio del modelo |

**Prompt sugerido:**
> "Crea un ScriptableObject llamado ComponentData con los campos que te indico [pegar campos]. Luego crea 8 instancias de este asset, una por cada componente de la laptop [pegar tabla], llenando nombre/función/compatibilidad con el contenido que te paso. Déjalos en Assets/_Project/Data/."

---

## 4. Modelos 3D de los componentes

**Opciones (de más rápida a más laboriosa):**
1. Usar modelos low-poly gratuitos del Asset Store / Unity Asset Store / Sketchfab (licencia libre) — más rápido y ya optimizados.
2. Modelar formas simples (cajas, planos) con materiales y texturas que se vean reconocibles — válido si el tiempo es limitado, prioriza que se reconozca cada pieza.
3. Si tienes acceso a Blender, generarlos ahí y exportarlos a .fbx.

Para cada componente necesitas:
- Modelo 3D con escala real coherente (la laptop completa ensamblada debe verse proporcional).
- Colocación de pivote correcto (para que el "snap" al ensamblar funcione bien).
- Polycount bajo (recordatorio de optimización — VR es sensible a esto).

**Prompt sugerido (si usas low-poly comprados/gratuitos):**
> "Voy a importar estos modelos FBX en Assets/_Project/Models/ [lista de archivos]. Cuando estén importados, ayúdame a configurar el import settings (escala, generación de colliders simples tipo BoxCollider, Mesh Compression Medium) para cada uno vía el MCP."

**Prompt sugerido (si se modela con primitivas):**
> "Crea 8 GameObjects placeholder (uno por componente de la lista) usando primitivas de Unity (Cube/Cylinder) con proporciones aproximadas a la pieza real, nómbralos según la lista, agrégales un material de color distintivo, y guárdalos como prefabs en Assets/_Project/Prefabs/Components/."

---

## 5. Sistema de interacción (selección + panel de info)

**Componentes técnicos:**
- `InteractableComponent.cs` — se adjunta a cada pieza; expone evento `OnSelected`.
- `ComponentInfoUI.cs` — controla un panel UI (World Space Canvas) que muestra nombre/función/características/compatibilidad del `ComponentData` recibido, y reproduce el audio de narración.
- Sistema de input compatible con la locomoción/puntero de Viroo (raycast desde el controlador VR, no mouse — verificar cómo la plantilla de Viroo maneja el input de selección).

**Flujo:**
Usuario apunta/toca componente → `InteractableComponent` dispara evento → `ComponentInfoUI` se llena con los datos y se posiciona cerca del objeto o en un panel fijo → se reproduce audio de narración.

**Prompt sugerido:**
> "Usando el sistema de input/puntero que ya trae la plantilla de Viroo, crea un script InteractableComponent.cs que detecte cuando el usuario apunta/selecciona el objeto y dispare un evento OnSelected(ComponentData data). Luego crea ComponentInfoUI.cs que reciba ese evento, muestre un panel World Space Canvas con Nombre, Función, Características y Compatibilidad, y reproduzca el AudioClip de narración asociado. Conecta este script a los 8 prefabs de componentes, asignando el ComponentData correspondiente a cada uno."

---

## 6. Ensamblaje guiado (valor agregado de mayor impacto en rúbrica)

**Concepto:** las piezas empiezan separadas sobre la mesa; el sistema indica un orden lógico de instalación (ej.: tarjeta madre → RAM → SSD → sistema de enfriamiento → batería → teclado → touchpad → pantalla) y el usuario debe arrastrar/colocar cada pieza en su "slot" dentro del chasis.

**Componentes técnicos:**
- `AssemblySlot.cs` — punto de anclaje en el chasis para una pieza específica; detecta colisión/proximidad con el componente correcto.
- `AssemblyManager.cs` — controla la secuencia: resalta la siguiente pieza esperada, valida que sea la correcta, reproduce animación/sonido de "click" al encajar, avanza al siguiente paso.
- Animación simple de instalación (puede ser un `Animation`/`Tween` de la pieza moviéndose desde su posición libre hasta el slot, o controlada por el propio agarre del usuario si usas XR Grab).

**Prompt sugerido:**
> "Crea AssemblySlot.cs: un componente que se coloca en la posición del chasis donde debe ir cada pieza, con un campo ComponentData esperado, que detecta cuando el objeto correcto entra en su zona (OnTriggerEnter con colisión configurada) y dispara un evento OnComponentInstalled. Crea AssemblyManager.cs que mantenga el orden de instalación (usando el campo ordenEnsamblaje de ComponentData), resalte visualmente (outline o color) la siguiente pieza esperada, y muestre un mensaje de progreso (ej. '3/8 piezas instaladas'). Cuando todas estén instaladas, dispara un evento OnAssemblyComplete."

---

## 7. Audio (narración + ambiente) — marcado como indispensable

- Grabar o generar (TTS) 8 clips de narración cortos (15-30s cada uno), uno por componente, explicando función/compatibilidad en lenguaje claro.
- Agregar un loop de audio ambiental de taller/laboratorio de bajo volumen.
- Sonido de feedback (click/snap) al instalar correctamente una pieza, y sonido de error si se intenta instalar en el slot incorrecto.

**Prompt sugerido:**
> "Crea un AudioManager.cs simple (singleton) con métodos PlayNarration(AudioClip clip), PlayAmbient(AudioClip loop) y PlaySFX(AudioClip clip). Conéctalo para que ComponentInfoUI llame PlayNarration al seleccionar una pieza, y AssemblyManager llame PlaySFX en instalación correcta/incorrecta. Configura un AudioSource de ambiente en loop con volumen bajo (~0.2) en la escena."

---

## 8. Evaluaciones interactivas (valor agregado)

Quiz corto al final del ensamblaje (3-5 preguntas tipo "¿Qué componente almacena los datos permanentemente?" con opciones), usando los mismos `ComponentData` como banco de preguntas.

**Prompt sugerido:**
> "Crea QuizManager.cs y un QuizQuestion (clase serializable: pregunta, lista de opciones, índice de respuesta correcta). Genera 5 preguntas basadas en los ComponentData de la laptop. Crea un Canvas simple de quiz que se active cuando AssemblyManager dispare OnAssemblyComplete, muestre las preguntas una por una, y al final muestre el puntaje obtenido."

---

## 9. Comparativa PC vs Laptop (valor agregado opcional)

Si el tiempo lo permite: un panel o segunda mini-escena con una tabla comparativa simple (texto/UI) mostrando diferencias clave (portabilidad, refrigeración, expansión, batería, costo). No requiere modelos nuevos, puede ser solo UI.

**Prompt sugerido:**
> "Crea un panel UI 'Comparativa PC vs Laptop' accesible desde un botón en el menú principal de la escena, con una tabla de texto comparando 5 aspectos clave entre ambos tipos de equipo."

---

## 10. Navegación

Usar el sistema de locomoción que ya trae la plantilla de Viroo (teleport o smooth locomotion). Tu trabajo aquí es solo:
- Definir los `NavMesh`/puntos de teletransporte alrededor de la mesa de trabajo.
- Asegurarte que el usuario puede ver y alcanzar (con el puntero) las 8 piezas y el chasis desde las posiciones de navegación disponibles.

**Prompt sugerido:**
> "Revisa el sistema de locomoción de la plantilla de Viroo en esta escena y configura los puntos de teleport (o el área navegable) para que cubran toda la mesa de trabajo y permitan ver/alcanzar las 8 piezas y el chasis."

---

## 11. Optimización

- Combinar materiales repetidos (mismo material para piezas similares).
- Activar GPU/Static batching donde aplique.
- Revisar polycount total de la escena (objetivo razonable para VR móvil: mantenerlo bajo, miles de polígonos, no millones).
- Comprimir texturas.
- Eliminar luces innecesarias / usar lightmapping si la plantilla lo soporta.

**Prompt sugerido:**
> "Revisa la escena vía MCP y dame un reporte de: número total de triángulos, número de materiales únicos, número de luces en tiempo real, y draw calls estimados. Sugiere y aplica optimizaciones (batching, combinar materiales, comprimir texturas) sin romper la funcionalidad."

---

## 12. Integración y pruebas en Viroo (continuo, no solo al final)

Checklist a repetir cada vez que agregues una fase nueva:
- [ ] Build exportado con la plantilla de Viroo.
- [ ] Carga correctamente dentro de Viroo (no solo en Unity Editor).
- [ ] Navegación funciona dentro de Viroo.
- [ ] Se ven todos los modelos 3D.
- [ ] Las interacciones (selección, ensamblaje, quiz) responden igual que en el Editor.
- [ ] Sin errores críticos en consola/log de Viroo.

**Prompt sugerido:**
> "Antes de seguir agregando funcionalidad, vamos a hacer un build y probarlo dentro de Viroo. Recuérdame los pasos exactos para exportar esta escena con la plantilla de Viroo y qué validar al cargarla."

---

## 13. Documentación técnica (5-7 páginas) y video

**Documentación debe incluir:**
1. Objetivos
2. Recursos utilizados (assets, plugins, plantilla Viroo)
3. Descripción del escenario (mesa de ensamblaje, flujo de usuario)
4. Interacciones implementadas (selección, info, ensamblaje guiado, audio, quiz)
5. Dificultades encontradas
6. Conclusiones

**Video (3-5 min) debe mostrar:**
- Navegación dentro de la escena
- Selección de componentes y su info
- Proceso de ensamblaje guiado completo
- Funcionamiento dentro de Viroo (no solo Editor)

Puedo ayudarte a redactar la documentación técnica en Word cuando tengas el proyecto funcionando — solo dime cuando estés listo.

---

## Orden recomendado de ejecución (resumen)

1. Setup + build de prueba en Viroo ✅ antes de seguir
2. Estructura de carpetas + escena base + iluminación
3. ScriptableObjects de datos (8 componentes)
4. Modelos 3D (o placeholders) + prefabs
5. Sistema de interacción + panel de info + audio narración
6. Ensamblaje guiado + audio de feedback
7. Build y prueba en Viroo (checkpoint)
8. Evaluaciones interactivas
9. Comparativa PC vs Laptop (si hay tiempo)
10. Optimización
11. Build final y prueba en Viroo (checkpoint final)
12. Documentación + video
