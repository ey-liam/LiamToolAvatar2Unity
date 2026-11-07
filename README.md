# LiamToolAvatar2Unity V7.1 - Multi-Engine Studio Suite

Herramienta profesional para crear, posar, animar avatares 2D en alta resolucion y exportarlos directamente a Unity y modding de Just Dance con efectos de shader en tiempo real, gizmo de rotacion en pantalla, mallas relajadas y herramientas avanzadas de pesos.

- Google Sites: https://sites.google.com/view/liamtoolavatar2unity/home
- GitHub Pages: https://ey-liam.github.io/LiamToolAvatar2Unity/
- Repositorio GitHub: https://github.com/ey-liam/LiamToolAvatar2Unity

## Novedades en V7.1

- Efectos de Shader en Tiempo Real en la Pagina Web:
  - Borde Coach Neon (Coach Outline): Renderizado de resplandor bloom y contorno brillante estilo Just Dance alrededor de la silueta deformada del personaje en el visor web.
  - Selector de Color de Borde: Paleta rapida con Cyan Neon (#00f0ff), Oro Just Dance (#ffd700), Magenta Neon (#ec4899) y Verde Esmeralda (#10b981).
  - Efecto Gold Move VFX en el Visor: Halo de 18 rayos dorados pulsantes y destellos de energia sincronizados con la pose de baile.
  - Escenario Neon (Pista de Baile Just Dance): Fondo interactivo con piso reflectante, lineas en perspectiva y reflector dinamico tras el avatar.
- Controles Interactivos en Pantalla Pulidos:
  - Gizmo de Rotacion Directo en Lienzo: Anillo circular interactivo que aparece al seleccionar un hueso, con manija de arrastre tactil, arco diferencial y lectura instantanea de grados (+00.0 grados).
  - Barra Flotante de Acceso Rapido (HUD): Control rapido en la parte inferior del visor con conmutadores de color de borde, activador rapido de Gold Move y centrado de camara.
  - Pincel de Pesos de Doble Anillo: Anillo interior solido de fuerza 100% y anillo exterior punteado de radio de decaimiento, con indicador flotante bajo el cursor que muestra el hueso activo, modo y porcentaje.
- Herramientas de Malla Mejoradas:
  - Presets de Densidad con Un Clic: Rapida (14), Media (22), Alta (30) y Ultra-HD (38 subdivisiones).
  - Relajacion y Suavizado Laplaciano de Malla: Algoritmo de suavizado laplaciano para nivelar triangulos internos sin distorsionar la silueta exterior, eliminando arrugas y pellizcos.
- Herramientas de Pesos Avanzadas:
  - Auto-Pesos Armonicos: Calculo con decaimiento exponencial inteligente y envolventes rigidas.
  - Suavizado de Hueso en 3 Pasadas: Suaviza transiciones entre vertices vecinos en codos, rodillas y hombros.
  - Normalizacion Estricta a 100%: Garantiza que la suma de influencias sea exactamente 1.0 con tope de 4 huesos.
  - Invertir y Limpiar Pesos: Operaciones instantaneas para aislar o revertir influencias por capa.
- Cero emojis en toda la plataforma y documentacion.

## Novedades en V7

- Correccion integral de importacion de imagen unica cuadrada (1:1):
  - Solucionado el problema donde las imagenes cuadradas o de proporcion no estandar eran forzadas y estiradas a 1536x2752.
  - Implementado ajuste proporcional estricto con centrado automatico y preservacion de aspect ratio nativo sin distorsion.
  - Eliminacion de anclaje forzado central (strength: 0, parentBone: none) para avatares de una sola pieza, garantizando deformacion natural por pesos sin pellizcos.
- Auto-adaptacion y calibracion del esqueleto a la silueta:
  - Nuevo modulo adaptSkeletonToBoundingBox que escanea el cuadro delimitador no transparente real del personaje.
  - Reescritura de autoDetectPoseFromAllParts eliminando rangos porcentuales verticales fijos; ahora la deteccion de torso, cabeza, cuello y extremidades se calcula de forma proporcional y relativa a la altura y anchura efectivas de la figura.
  - Nuevo comando en la Paleta de Comandos (Ctrl+K): "Auto-adaptar esqueleto a silueta" (Alt+A).
- Mallas poligonales isotropicas:
  - Generacion de mallas basada en tamano de celda uniforme (targetCellSize), produciendo quads y triangulos uniformes y regulares independientemente de si la imagen es 1:1, 16:9 o 9:16.
- Ecosistema ShaderLab para Unity:
  - Shaders/LiamAvatar2D_CustomCard.shader: Shader para deformacion 2D, soporte de aura dorada pulsante Gold Move, delineado de coach (Coach Outline) con compatibilidad Bloom, gradientes HSV y recorte alfa.
  - Shaders/LiamAvatar_StageNeon.shader: Efectos de escenario interactivo neon estilo Just Dance con lineas de barrido y aberracion cromatica.
  - Opcion de exportacion ShaderLab integrada directamente en el Export Hub modal.
- Runtime y Editor Unity en C#:
  - Unity/Runtime/LiamAvatarController.cs: Controlador runtime de avatar con soporte para 7 clips de animacion (Idle, Walk, Jump, Wave, GoldMove, Groove, Cheer), sway procedural, respiracion organica y dinamica de muelles.
  - Unity/Editor/LiamAvatarImporterWindow.cs: Ventana de importacion drag-and-drop en Unity (Tools > LiamTool > Avatar & Card Importer) para generar prefabs y materiales listos con un solo clic.
- Herramientas CLI en Python:
  - Tools/avatar_processor.py: CLI con comandos fit (ajuste sin deformacion), segment (segmentacion anatomica automatica en 6 capas) y validate (verificacion de integridad Spine JSON).
  - Tools/ffmpeg_video_converter.py: Transcodificacion automatizada con FFmpeg para WebM VP8 sin audio, MP4 H.264 para Unity VideoPlayer y GIF optimizado de 2 pasadas mediante palettegen.
- Herramienta de validacion por lotes en Java:
  - Tools/AvatarBatchCLI.java: Herramienta de consola sin dependencias externas para validar catalogos masivos de DancerCards en segundos, compatible con Java 8+.
- Nuevos clips de animacion integrados:
  - Gold Move: Pose iconica y energica con freeze dramatico estilo Just Dance.
  - Groove: Balanceo ritmico de caderas y torso para canciones funky.
  - Cheer: Movimiento festivo con brazos elevados y rebote elastico.
- Regla de diseno estricta: Cero emojis en toda la documentacion, interfaz y codigo.

## Novedades en V6.2

- Codec VP8 prioritario y transmision sin audio: Salida WebM optimizada utilizando el codec VP8 como opcion primordial para maxima fluidez y compatibilidad nativa en navegadores web y reproductores de video, empaquetado como stream de video puro libre de pistas de audio redundantes.
- Bitrate reforzado de alta fidelidad: Calibracion de tasa de bits incrementada (entre 1.8 Mbps y 8.0 Mbps segun la resolucion seleccionada, con ~2.5 Mbps para 480p y ~5.5 Mbps para 720p), proporcionando siluetas perfectamente nitidas y colores limpios sin inflar excesivamente el tamano del archivo.

## Novedades en V6.1

- Sincronizacion de velocidad de video independiente de FPS: El exportador de video WebM ahora utiliza un bucle temporalizado en tiempo real mediante performance.now(), sincronizando de forma exacta la duracion del clip con la duracion de la animacion. La velocidad de reproduccion es 100% identica y natural sin importar si se exporta a 15, 24, 30 o 60 FPS, eliminando por completo la ralentizacion a altas tasas de fotogramas.
- Bitrate optimizado y video ligero: Recalibracion de la tasa de bits para generar archivos de video mas agiles de reproducir y almacenar.

## Novedades en V6

- Modo Pareja y Coach Acompanante (Couple Mode / Duo): Soporte completo para disenar y componer dos personajes o coaches en la misma escena:
  - Espaciado horizontal regulable entre personajes (coupleGap).
  - Desfases y transformaciones individuales por personaje (characterOffsets para Primary y Companion).
  - Fusion e importacion automatica de proyectos .L2U para emparejar avatares independientes en un solo lienzo.
  - Maniqui vectorial procedural para el acompanante con esqueleto sincronizado.
- Paleta de Comandos Rapida (Command Palette):
  - Acceso instantaneo a funciones clave mediante atajo de teclado (Ctrl+K) o boton de barra superior.
  - Busqueda rapida de acciones: Guardar proyecto, deshacer, rehacer, enfocar composicion (F), alternar modo pareja (P), recalcular pesos y abrir configuracion.
- Centro de Exportacion Unificado (V6 Export Hub):
  - Ventana modal centralizada para exportar por lotes o de forma selectiva:
    - Texturas estaticas para DancerCardCatalog (Banner, Cursor, DancerCard, Full, Hud, Portrait, Square).
    - Atlas empaquetado PNG de alta resolucion.
    - Archivo descriptor Spine/Unity JSON.
    - Script MonoBehaviour C# para Unity.
    - Shaders ShaderLab para efectos y delineados.
    - Exportador de animaciones en APNG de 32 bits y formato GIF.
- Sistema de Notificaciones Flotantes (V6 Toasts):
  - Mensajes de estado visuales y reactivos para confirmar guardados, acciones de rigging y avisos del sistema sin bloquear la interaccion.

## Estructura del Repositorio

- index.html: Aplicacion web principal y estudio interactivo completo.
- Shaders/:
  - LiamAvatar2D_CustomCard.shader: Shader 2D de deformacion, skinning, Gold Move y Coach Outline.
  - LiamAvatar_StageNeon.shader: Shader de fondo y escenario interactivo con scanlines y gradientes neon.
- Unity/:
  - Runtime/LiamAvatarController.cs: Script de control procedural y ejecucion de animaciones en Unity.
  - Editor/LiamAvatarImporterWindow.cs: Ventana de importacion y generacion automatica de prefabs.
- Tools/:
  - avatar_processor.py: Preprocesamiento de imagenes, auto-ajuste y segmentacion anatomica.
  - ffmpeg_video_converter.py: Pipeline de transcodificacion para WebM VP8, MP4 y GIF optimizado.
  - AvatarBatchCLI.java: Validador de catalogos de avatares por lotes.

## Guia de Uso de las Herramientas

### Preprocesamiento con Python
```bash
# Ajustar una textura cuadrada sin distorsion a 1536x2752
python Tools/avatar_processor.py fit --input mi_personaje.png --output personaje_ajustado.png

# Segmentar automaticamente una imagen en 6 capas anatomicas
python Tools/avatar_processor.py segment --input mi_personaje.png --outdir capas_personaje/

# Validar integridad de un archivo Spine JSON exportado
python Tools/avatar_processor.py validate --json Avatar.json --atlas Avatar_Atlas.png
```

### Transcodificacion con FFmpeg
```bash
# Convertir video a WebM VP8 transparente optimizado para web
python Tools/ffmpeg_video_converter.py webm --input animacion.mp4 --output animacion.webm --bitrate 3500k

# Convertir a MP4 H.264 para Unity VideoPlayer
python Tools/ffmpeg_video_converter.py mp4 --input animacion.webm --output animacion.mp4

# Generar GIF con paleta de 2 pasadas de alta fidelidad
python Tools/ffmpeg_video_converter.py gif --input animacion.mp4 --output animacion.gif --fps 30
```

### Validacion por lotes con Java
```bash
# Compilar CLI (compatible con Java 8 o superior)
javac --release 8 Tools/AvatarBatchCLI.java

# Validar una carpeta de catalogos de avatares
java -cp Tools AvatarBatchCLI ruta/a/mis_avatares
```

## Caracteristicas Principales del Estudio Web

- Resolucion nativa de lienzo para maxima fidelidad en cada sprite.
- Empaquetador de Atlas sin espacios vacios: Detecta pixeles visibles de cada parte y genera un unico archivo PNG optimizado.
- Generacion de esqueleto JSON y scripts C# para Unity: Jerarquias, puntos de anclaje, mallas y deformacion por skinning listas para el juego.
- Rigging 2D y animacion: Sistema de huesos, pesos suaves, amortiguacion, fisicas de brisa y presets de movimiento Just Dance.
- Formato de proyecto .L2U: Guardado y apertura rapida de proyectos en formato JSON local.

## Licencia y Creditos

- Dar los creditos correspondientes al autor en los proyectos donde se utilice la herramienta.
- Queda prohibida la copia no autorizada del codigo fuente.
