# LiamToolAvatar2Unity V4

Herramienta web en un solo archivo HTML para crear, configurar rig, animar avatares 2D en resolucion 1536x2752 y exportarlos directamente a Unity y a la carpeta CustomAvatars del juego.

- Google Sites: https://sites.google.com/view/liamtoolavatar2unity/full-page-embed
- GitHub Pages: https://ey-liam.github.io/LiamToolAvatar2Unity/

## Novedades en V4

- Exportacion directa a CustomAvatars: Guarda los archivos en C:\JDLiminal\Game\CustomAvatars\<AvatarName>\ creando automaticamente la subcarpeta con el nombre del avatar.
- Exportacion completa de paquetes: Almacena en la carpeta del avatar el Atlas PNG, el script C#, el archivo JSON y las 7 texturas estaticas del juego.
- Descarga individual conservada: Posibilidad de descargar cada recurso por separado (Atlas PNG, archivo JSON, script C#, texturas individuales o paquete ZIP).
- Exportador de Texturas Estaticas: Genera las 7 texturas requeridas (Banner, Cursor, DancerCard, Full, Hud, Portrait, Square) con ajuste interactivo de zoom, posicion X y posicion Y en tiempo real.
- Exportador de animaciones en APNG 32-bit (canal alfa real sin perdida) y formato GIF universal.
- Interfaz y codigo limpios sin emojis ni comentarios redundantes.

## Caracteristicas Principales

- Resolucion nativa 1536x2752 para preservacion de detalle en todas las capas.
- Empaquetador de Atlas sin espacios vacios: Detecta pixeles visibles de cada parte y genera un unico archivo PNG optimizado.
- Generacion de esqueleto JSON y script C# para Unity: Coordenadas de atlas, puntos de anclaje y jerarquia listos para renderizado.
- Rigging 2D y animacion: Sistema de huesos, pesos, envolventes, fisicas y presets de movimiento.
- Formato de proyecto .L2U: Guardado y apertura de proyectos para continuar el trabajo posteriormente.
- Servidor MCP integrado: Soporte para comandos y automatizacion asistida por IA.

## Estructura de Archivos Exportados

Ruta de destino:
C:\JDLiminal\Game\CustomAvatars\<AvatarName>\

Archivos generados:
- <AvatarName>_Atlas.png: Atlas con todas las partes visibles recortadas y empaquetadas.
- <AvatarName>.json: Definicion del esqueleto, puntos de anclaje, mallas y animaciones.
- <NombreClase>.cs: Script Runtime para Unity.
- <AvatarName>_<Variante>_<Tipo>.png: Texturas estaticas para el juego (Banner, Cursor, DancerCard, Full, Hud, Portrait, Square).

## Licencia y Creditos

- Dar los creditos correspondientes al autor en los proyectos donde se utilice la herramienta.
- Queda prohibida la copia no autorizada del codigo fuente.
