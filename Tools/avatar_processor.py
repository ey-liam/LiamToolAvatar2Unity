#!/usr/bin/env python3
"""
LiamTool Avatar Processor (Python CLI)
Herramienta de procesamiento de imagenes, segmentacion anatomica y empaquetado de atlas.
"""

import os
import sys
import json
import argparse
from pathlib import Path
from PIL import Image

def get_character_bounding_box(image, alpha_threshold=15):
    """Calcula la caja delimitadora del contenido no transparente."""
    if image.mode != "RGBA":
        image = image.convert("RGBA")
    alpha = image.split()[-1]
    bbox = alpha.getbbox()
    return bbox or (0, 0, image.width, image.height)

def fit_to_aspect_ratio(image, target_w=1536, target_h=2752, fill_color=(0, 0, 0, 0)):
    """Ajusta una imagen (cuadrada o de cualquier proporcion) al lienzo objetivo sin distorsion."""
    if image.mode != "RGBA":
        image = image.convert("RGBA")

    bbox = get_character_bounding_box(image)
    cropped = image.crop(bbox)

    char_w, char_h = cropped.size
    scale = min((target_w * 0.90) / char_w, (target_h * 0.90) / char_h)
    new_w = max(1, int(char_w * scale))
    new_h = max(1, int(char_h * scale))

    resized = cropped.resize((new_w, new_h), Image.Resampling.LANCZOS)
    canvas = Image.new("RGBA", (target_w, target_h), fill_color)

    offset_x = (target_w - new_w) // 2
    offset_y = (target_h - new_h) // 2
    canvas.paste(resized, (offset_x, offset_y), resized)

    return canvas, (offset_x, offset_y, offset_x + new_w, offset_y + new_h)

def segment_single_character(image_path, output_dir):
    """Segmenta automaticamente una imagen unica en capas anatomicas aproximadas."""
    out_path = Path(output_dir)
    out_path.mkdir(parents=True, exist_ok=True)

    img = Image.open(image_path).convert("RGBA")
    fitted, bbox = fit_to_aspect_ratio(img, 1536, 2752)
    min_x, min_y, max_x, max_y = bbox
    total_h = max_y - min_y
    total_w = max_x - min_x
    mid_x = (min_x + max_x) // 2

    slices = {
        "Cabeza": (min_x, min_y, max_x, min_y + int(total_h * 0.22)),
        "Torso": (min_x, min_y + int(total_h * 0.22), max_x, min_y + int(total_h * 0.54)),
        "Brazo_Izq": (min_x, min_y + int(total_h * 0.22), mid_x - int(total_w * 0.15), min_y + int(total_h * 0.60)),
        "Brazo_Der": (mid_x + int(total_w * 0.15), min_y + int(total_h * 0.22), max_x, min_y + int(total_h * 0.60)),
        "Pierna_Izq": (min_x, min_y + int(total_h * 0.54), mid_x, max_y),
        "Pierna_Der": (mid_x, min_y + int(total_h * 0.54), max_x, max_y),
    }

    generated_files = []
    for part_name, (x0, y0, x1, y1) in slices.items():
        part_canvas = Image.new("RGBA", (1536, 2752), (0, 0, 0, 0))
        crop_box = (max(0, x0), max(0, y0), min(1536, x1), min(2752, y1))
        cropped_region = fitted.crop(crop_box)
        part_canvas.paste(cropped_region, (crop_box[0], crop_box[1]), cropped_region)

        file_path = out_path / f"{part_name}.png"
        part_canvas.save(file_path, "PNG")
        generated_files.append(str(file_path))

    return generated_files

def validate_spine_json(json_path):
    """Valida la integridad de un archivo Spine JSON generado."""
    with open(json_path, "r", encoding="utf-8") as f:
        data = json.load(f)

    errors = []
    if not data.get("avatarName"):
        errors.append("Falta 'avatarName'")
    if not isinstance(data.get("bones"), list) or len(data["bones"]) == 0:
        errors.append("Lista de huesos vacia o no valida")
    if not data.get("skeleton"):
        errors.append("Falta definicion de 'skeleton'")

    return len(errors) == 0, errors

def main():
    parser = argparse.ArgumentParser(description="LiamTool Avatar Processor CLI")
    subparsers = parser.add_subparsers(dest="command", required=True)

    fit_p = subparsers.add_parser("fit", help="Ajusta imagen a lienzo 9:16 o cuadrado sin distorsion")
    fit_p.add_argument("input", help="Ruta de la imagen de entrada")
    fit_p.add_argument("output", help="Ruta de la imagen de salida")
    fit_p.add_argument("--width", type=int, default=1536, help="Ancho del lienzo")
    fit_p.add_argument("--height", type=int, default=2752, help="Alto del lienzo")

    seg_p = subparsers.add_parser("segment", help="Segmenta imagen unica en capas anatomicas")
    seg_p.add_argument("input", help="Ruta de la imagen de entrada")
    seg_p.add_argument("output_dir", help="Directorio para las capas generadas")

    val_p = subparsers.add_parser("validate", help="Valida descriptor Spine JSON")
    val_p.add_argument("json_file", help="Ruta del archivo JSON")

    args = parser.parse_args()

    if args.command == "fit":
        img = Image.open(args.input)
        canvas, bbox = fit_to_aspect_ratio(img, args.width, args.height)
        canvas.save(args.output, "PNG")
        print(f"[OK] Imagen ajustada exitosamente en {args.output}")

    elif args.command == "segment":
        files = segment_single_character(args.input, args.output_dir)
        print(f"[OK] Generadas {len(files)} capas en {args.output_dir}")

    elif args.command == "validate":
        valid, errors = validate_spine_json(args.json_file)
        if valid:
            print(f"[OK] El archivo {args.json_file} es valido.")
        else:
            print(f"[ERROR] Errores encontrados: {errors}")
            sys.exit(1)

if __name__ == "__main__":
    main()
