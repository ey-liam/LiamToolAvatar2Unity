# LiamToolAvatar2Unity V6.1

Herramienta web profesional para crear, configurar rig, animar avatares 2D en alta resolucion y exportarlos directamente a Unity y al juego.

- Google Sites: https://sites.google.com/view/liamtoolavatar2unity/home
- GitHub Pages: https://ey-liam.github.io/LiamToolAvatar2Unity/

## Novedades en V6.1

- Sincronizacion de velocidad de video independiente de FPS: El exportador de video WebM ahora utiliza un bucle temporalizado en tiempo real mediante performance.now(), sincronizando de forma exacta la duracion del clip con la duracion de la animacion. La velocidad de reproduccion es 100% identica y natural sin importar si se exporta a 15, 24, 30 o 60 FPS, eliminando por completo la ralentizacion a altas tasas de fotogramas.
- Bitrate optimizado y video ligero: Se recalibro la tasa de bits (bitrate) para generar archivos de video sustancialmente mas ligeros y agiles de reproducir, manteniendo contornos nitidos y colores limpios sin inflar innecesariamente el peso del archivo.

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
    - Archivo descriptor Spine/Unity JSON V6.1.
    - Script MonoBehaviour C# para Unity.
    - Exportador de animaciones en APNG de 32 bits y formato GIF.
- Sistema de Notificaciones Flotantes (V6 Toasts):
  - Mensajes de estado visuales y reactivos para confirmar guardados, acciones de rigging y avisos del sistema sin bloquear la interaccion.
- Interfaz Responsiva Adaptativa:
  - Soporte mejorado para pantallas compactas y dispositivos moviles con paneles laterales desplegables y sombreado dinamico.
- Motor de Rigging y Mallas Optimizado:
  - Normalizacion rigurosa de pesos con maximo de 4 influencias por vertice y suma exacta de 1.0 para compatibilidad absoluta con Unity.
  - Mascara estricta de huesos permitidos por pieza que evita dispersion no deseada en el pincel de pesos.
  - Radio de celda adaptativo mas ajustado para un delineado limpio de siluetas.
- Cero emojis y documentacion limpia en todo el codigo fuente.

## Novedades en V5.1

- Normalizacion estandar de pesos: Limitacion automatica a un maximo de 4 influencias oseas por vertice con suma normalizada exacta (1.0), garantizando compatibilidad nativa total con el sistema de skinning de Unity.
- Mallas poligonales mas fieles: Reduccion del radio de celda adaptativo en la generacion de mallas para contornear los bordes y siluetas de cada sprite sin distorsiones marginales.
- Mascara estricta de huesos permitidos: Las funciones de pintura de pesos y relajacion respetan de forma estricta los huesos asignados a cada pieza, evitando fugas de influencia entre capas y extremidades.
- Seleccion fluida en el visor: Correccion en la deteccion de clics sobre el lienzo cuando existen capas invisibles u ocultas, permitiendo seleccionar cualquier pieza visible sin bloqueos.
- Optimizacion y accesibilidad: Preconexiones de fuentes web, metadatos de entorno y roles de accesibilidad integrados en los lienzos interactivos.
- Postura inicial predeterminada: Inicio automatico en postura Casual con parametros de amortiguacion y rigidez equilibrados.

## Novedades en V5

- Nueva interfaz moderna y estilizada: Barra superior redisenada con logotipo vectorial original, distribucion equilibrada de herramientas y jerarquia visual mejorada.
- Apartado de Configuracion del Estudio: Panel dedicado para personalizar el entorno de trabajo:
  - Fondos de lienzo: Estudio con rejilla, patron checkerboard para transparencia, negro profundo OLED (#000000), pantalla verde chroma (#00FF00) y azul chroma (#0000FF).
  - Parametros de rigging: Ajuste de grosor de huesos (1.0x a 2.4x), nivel de opacidad, radio de nodos articulares y conmutador para mostrar u ocultar etiquetas de huesos sobre el visor.
  - Valores predeterminados de exportacion: Configuracion de nombre inicial de avatar y ruta base del sistema.
  - Persistencia local: Todas las preferencias se guardan automaticamente en el navegador.
- Template de inicio optimizado: Maniqui generico limpio estilo Just Dance listo para posar y animar.
- Exportacion directa a CustomAvatars (Atlas PNG, archivo JSON, script C# y 7 texturas estaticas del juego).
- Exportador de animaciones en APNG 32-bit y formato GIF.

## Caracteristicas Principales

- Resolucion nativa para preservacion de detalle en todas las capas.
- Empaquetador de Atlas sin espacios vacios: Detecta pixeles visibles de cada parte y genera un unico archivo PNG optimizado.
- Generacion de esqueleto JSON y script C# para Unity: Coordenadas de atlas, puntos de anclaje y jerarquia listos para renderizado.
- Rigging 2D y animacion: Sistema de huesos, pesos, envolventes, fisicas y presets de movimiento.
- Formato de proyecto .L2U: Guardado y apertura de proyectos para continuar el trabajo posteriormente.

## Estructura de Archivos Exportados

Archivos generados:
- <AvatarName>_Atlas.png: Atlas con todas las partes visibles recortadas y empaquetadas.
- <AvatarName>.json: Definicion del esqueleto, puntos de anclaje, mallas y animaciones.
- <NombreClase>.cs: Script Runtime para Unity.
- <AvatarName>_<Variante>_<Tipo>.png: Texturas estaticas para juego (Banner, Cursor, DancerCard, Full, Hud, Portrait, Square).

## Licencia y Creditos

- Dar los creditos correspondientes al autor en los proyectos donde se utilice la herramienta.
- Queda prohibida la copia no autorizada del codigo fuente.
