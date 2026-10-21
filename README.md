# LiamToolAvatar2Unity

Herramienta web (single-file HTML) para crear **avatares animados estilo Just Dance** (Ubisoft): permite importar partes PNG de un personaje, armar un rig de huesos, pintar pesos (skinning), posar y animar, y exportar para Unity.

> Estado actual: **funcional**, pero la **exportación de archivos para Unity está pendiente de corregir** y aún no se integra la **creación de atlas**.

## Características

- Editor visual en el navegador (sin instalación, abre `index.html`).
- Importación de partes PNG (multi-parte o imagen única).
- Presets de estructura (6 capas / 15 capas) y variantes de piernas.
- Rig de huesos con jerarquía, roles (torso, cabeza, brazos, piernas, pendulo/falda).
- Auto-detección de pose por silueta y plantillas de pose.
- Skinning con pincel de pesos (sumar/restar/suavizar) y envoltura automática.
- Mallas anti-recorte (silueta o lienzo completo) con densidad y margen configurables.
- Anclajes anti-dislocación.
- Timeline con clips (idle, walk, jump, wave), keyframes, reproducción y física.
- Undo/Redo, guardar/cargar proyecto `.L2U`.
- Exportación a Unity (en progreso).

## Uso

1. Abre `index.html` en un navegador moderno (Chrome/Edge recomendado).
2. En modo **SETUP**, importa las partes PNG del avatar.
3. Ajusta huesos, pesos y anclajes.
4. Cambia a modo **ANIMATE** para posar/animar.
5. Usa **Exportar Unity** (pendiente de corrección).

## Estructura

- `index.html` — aplicación completa (HTML/CSS/JS en un solo archivo).
- `README.md` — este archivo.

## Roadmap

- [ ] Corregir exportación de archivos para Unity.
- [ ] Integrar creación de atlas de texturas.
- [ ] Documentación de formato de exportación.

## Licencia

Proyecto personal de Liam. Uso libre para aprendizaje; contactar al autor para otros usos.