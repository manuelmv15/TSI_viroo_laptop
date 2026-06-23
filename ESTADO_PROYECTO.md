# Estado del Proyecto — Simulador de Ensamblaje de Laptop en Viroo/Unity

**Fecha de última actualización:** 2026-06-23 (sesión 3 — XRGrabInteractable, Rigidbody, Build Settings, HUD autowire)  
**Escena principal:** `Assets/_Project/Scenes/Laptop.unity`  
**Demo/plantilla original:** `Assets/Scenes/Demo.unity` (intacta, no modificar)

---

## ✅ Completado

### Estructura base
- [x] Proyecto Unity con plantilla oficial Viroo conectada y funcional
- [x] Escena `Laptop.unity` creada desde la plantilla Viroo (sin el tutorial de la plantilla)
- [x] `FadeUntilReadyLogic` deshabilitado — pantalla ya NO queda negra al entrar
- [x] Escena agregada a Build Settings
- [x] Estructura de carpetas `Assets/_Project/` completa:
  - `Scenes/`, `Models/`, `Prefabs/Components/`, `Prefabs/UI/`
  - `Scripts/Interaction/`, `Scripts/Assembly/`, `Scripts/Quiz/`, `Scripts/Data/`, `Scripts/UI/`
  - `Audio/Narration/`, `Audio/Ambient/`, `Materials/`, `Data/`

### Ambiente 3D
- [x] Mesa de trabajo (placeholder) posicionada frente al spawn del jugador
- [x] Luz puntual cálida (3000K, range 3m) sobre la mesa
- [x] Iluminación existente de la plantilla conservada (Directional Light + Reflection Probe)

### Datos de componentes (Fase 3)
- [x] `ComponentData.cs` — ScriptableObject con campos: nombre, función, características, compatibilidad, audio, ícono, ordenEnsamblaje
- [x] 8 instancias `ComponentData` assets en `Assets/_Project/Data/` con contenido educativo real:
  - `CD_Tarjeta_Madre.asset`, `CD_Memoria_RAM.asset`, `CD_SSD.asset`
  - `CD_Sistema_de_Enfriamiento.asset`, `CD_Batería.asset`, `CD_Teclado.asset`
  - `CD_Touchpad.asset`, `CD_Pantalla.asset`

### Modelos / Placeholders (Fase 4)
- [x] 8 GameObjects placeholder (Cube) sobre la mesa, uno por componente
- [x] Colores únicos y distinguibles por pieza
- [x] Escalas proporcionales a la pieza real (placa madre grande, RAM barra, SSD pequeño, etc.)
- [x] Etiquetas 3D (TextMeshPro) flotantes sobre cada pieza con el nombre del componente
- [x] Prefabs guardados en `Assets/_Project/Prefabs/Components/`

### Sistema de interacción (Fase 5)
- [x] `InteractableComponent.cs` — extiende `XRGrabInteractable` (física VR real), hover highlight + evento `OnSelected`. `movementType = Kinematic`.
- [x] `ComponentInfoUI.cs` — World Space Canvas con Nombre, Función, Características, Compatibilidad + botón Cerrar
- [x] `ComponentSelector.cs` — singleton que conecta selección → panel UI
- [x] Auto-wiring en `Awake()` — no depende de referencias en Inspector

### Sistema de ensamblaje (Fase 6)
- [x] `AssemblySlot.cs` — punto de anclaje, detecta instalación vía `OnTriggerEnter`, fuerza release del grab antes de instalar, animación SmoothStep 0.35s
- [x] `AssemblyManager.cs` — controla secuencia, auto-wiring del HUD (`txtProgreso`/`txtInstruccion`), dispara `OnAssemblyComplete`
- [x] `LaptopChassis` placeholder con 8 slots posicionados
- [x] HUD de progreso (`AssemblyHUD`) — World Space Canvas con "X/8 piezas instaladas" + instrucción de siguiente pieza

### Audio (Fase 7)
- [x] `AudioManager.cs` — singleton con 3 canales: narración, ambiente (loop), SFX
- [x] Auto-detección de `AudioSource` en Awake
- [x] Llamadas integradas: `PlayNarration()` al seleccionar componente, `PlaySFX()` al instalar

### Quiz (Fase 8)
- [x] `QuizQuestion.cs` — clase serializable (pregunta, opciones, índice correcto)
- [x] `QuizManager.cs` — 5 preguntas hardcoded sobre componentes, UI de quiz, puntaje final
- [x] Se activa automáticamente al completar las 8 instalaciones (`OnAssemblyComplete`)
- [x] Auto-wiring en Awake (busca `PanelQuiz` y sus hijos por nombre)

### Comparativa PC vs Laptop (Fase 9 — valor agregado)
- [x] `ComparativaPanel.cs` — panel World Space Canvas con 5 aspectos comparativos
- [x] Panel posicionado en la escena cerca de la mesa de trabajo

### Configuración de escena
- [x] `AudioManager` con 3 `AudioSource` en la escena
- [x] `AssemblyManager` conectado a `QuizManager` (inicia quiz al completar ensamblaje)
- [x] Todos los scripts se auto-configuran en runtime (no requieren wiring manual en Inspector)

---

## ❌ Pendiente

### Crítico (afecta la nota)
- [ ] **Audios de narración** — grabar o generar (TTS) 8 clips de 15-30s, uno por componente, y asignar en cada `CD_*.asset` en el campo `clipAudioNarracion`. Sin esto el panel de info abre pero no suena.
- [ ] **Prueba en Viroo headset** — hacer build y verificar que la escena carga en el dispositivo VR. Sin esta prueba no se puede garantizar el criterio de funcionamiento en Viroo (20% de la rúbrica).
- [x] **Build exportado** — `Laptop.unity` agregada a Build Settings (index 1, habilitada). Orden: `main.unity` → `Laptop.unity` → `Demo.unity`.

### Importante (afecta calidad)
- [ ] **Modelos 3D reales** — los placeholders actuales son cubos de colores. Reemplazar con modelos low-poly de componentes reales (Asset Store, Sketchfab CC0, o Blender). Asignar en los prefabs existentes.
- [ ] **Audio ambiental** — importar un clip de ambiente de laboratorio/taller y asignarlo en el `AudioManager` (campo `clipAmbiente`). Se reproduce en loop automáticamente.
- [ ] **SFX de instalación correcta/incorrecta** — asignar clips en `AudioManager` (`sfxInstalacionCorrecta`, `sfxInstalacionIncorrecta`, `sfxQuizCorrecto`, `sfxQuizIncorrecto`).
- [ ] **Navegación / puntos de teletransporte** — verificar en headset que los componentes son alcanzables desde el spawn. No hay `TeleportationArea` en la escena — depende del locomotion de Viroo.
- [x] **Highlight material** — `HighlightOutline.mat` (URP/Lit, emisivo cyan-azul, semi-transparente) creado y asignado en los 8 `InteractableComponent` de la escena.

### Nice-to-have (valor agregado adicional)
- [x] **Animaciones de instalación** — `AssemblySlot.cs` tiene coroutine SmoothStep (0.35s) que lerp-ea la pieza al slot.
- [ ] **Ícono UI** por componente — asignar sprites en `iconoUI` de cada `ComponentData` para mostrarlos en el panel de info.
- [x] **Botón para abrir Comparativa** — `BotonComparativa` (esfera verde, `XRSimpleInteractable`) posicionado en escena; `ComparativaPanel.AutoWireBoton3D()` conecta el evento al iniciar.

---

## Arquitectura rápida

```
Root/
└── LaptopAssembly/
    ├── Environment/
    │   ├── Workbench (mesa placeholder)
    │   ├── WorkbenchLight (point light cálida)
    │   └── LaptopChassis (chasis con 8 AssemblySlots)
    ├── Components/  (8 piezas: Tarjeta_Madre, RAM, SSD, ...)
    ├── AudioManager (3 AudioSource)
    ├── AssemblyManager
    ├── AssemblyHUD (World Space Canvas — progreso)
    ├── ComponentInfoUI (World Space Canvas — info de pieza)
    ├── ComponentSelector (singleton)
    ├── ComparativaPC (World Space Canvas — PC vs Laptop)
    └── QuizManager (World Space Canvas — quiz final)
```

## Rúbrica vs Estado

| Criterio | % | Estado |
|---|---|---|
| Diseño y organización de la escena | 15 | ✅ Estructura limpia, jerarquía organizada |
| Calidad de modelos 3D | 15 | ⚠️ Placeholders — requiere modelos reales |
| Interactividad y programación | 20 | ✅ XR interactable, ensamblaje, quiz, UI |
| Contenido educativo | 15 | ✅ 8 ComponentData con datos reales + quiz |
| Funcionamiento en Viroo | 20 | ⚠️ No probado en headset aún |
| Documentación técnica | 5 | ⚠️ Pendiente redactar (5-7 páginas) |
| Presentación final | 10 | ⚠️ Pendiente grabar video (3-5 min) |

---

## Scripts del proyecto

| Archivo | Propósito |
|---|---|
| `Scripts/Data/ComponentData.cs` | ScriptableObject — datos de cada componente |
| `Scripts/Interaction/InteractableComponent.cs` | XRSimpleInteractable + hover + OnSelected |
| `Scripts/Interaction/ComponentInfoUI.cs` | Panel World Space con info del componente |
| `Scripts/Interaction/ComponentSelector.cs` | Singleton — conecta selección → UI |
| `Scripts/Assembly/AssemblySlot.cs` | Punto de anclaje por pieza |
| `Scripts/Assembly/AssemblyManager.cs` | Secuencia + progreso + OnAssemblyComplete |
| `Scripts/AudioManager.cs` | Singleton — narración, ambiente, SFX |
| `Scripts/Quiz/QuizQuestion.cs` | Clase serializable de pregunta |
| `Scripts/Quiz/QuizManager.cs` | 5 preguntas + UI + puntaje |
| `Scripts/UI/ComparativaPanel.cs` | Panel PC vs Laptop |
| `Scripts/Data/CreateComponentDataEditor.cs` | Editor: crea los 8 assets ComponentData |
| `Scripts/Data/SceneSetupEditor.cs` | Editor: setup completo inicial de escena |
| `Scripts/Data/FinishSetupEditor.cs` | Editor: etiquetas 3D + HUD + fix escalas |
| `Scripts/Data/WireReferencesEditor.cs` | Editor: conecta referencias (backup) |
