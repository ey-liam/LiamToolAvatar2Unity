#!/usr/bin/env python3
"""
LiamTool FFmpeg Video Converter (Python CLI)
Transcodificacion automatizada a WebM (VP8/VP9), MP4, APNG y GIF optimizados.
"""

import os
import sys
import shutil
import argparse
import subprocess
from pathlib import Path

def check_ffmpeg_available():
    """Verifica si FFmpeg esta instalado en el sistema."""
    return shutil.which("ffmpeg") is not None

def convert_to_webm_vp8(input_path, output_path, bitrate="2500k", fps=30):
    """Transcodifica a WebM VP8 sin audio optimizado para reproduccion web fluida."""
    cmd = [
        "ffmpeg", "-y",
        "-i", str(input_path),
        "-c:v", "libvpx",
        "-b:v", bitrate,
        "-r", str(fps),
        "-an",
        "-auto-alt-ref", "0",
        "-pix_fmt", "yuva420p",
        str(output_path)
    ]
    return subprocess.run(cmd, capture_output=True, text=True)

def convert_to_mp4_h264(input_path, output_path, bg_color="black", fps=30):
    """Convierte a MP4 H.264 compatible universalmente."""
    cmd = [
        "ffmpeg", "-y",
        "-i", str(input_path),
        "-vf", f"fps={fps},format=yuv420p",
        "-c:v", "libx264",
        "-preset", "medium",
        "-crf", "18",
        "-an",
        str(output_path)
    ]
    return subprocess.run(cmd, capture_output=True, text=True)

def convert_to_optimized_gif(input_path, output_path, fps=24, width=480):
    """Genera un GIF optimizado en dos pasadas usando palettegen y paletteuse."""
    palette_filter = f"fps={fps},scale={width}:-1:flags=lanczos,split[s0][s1];[s0]palettegen=reserve_transparent=1[p];[s1][p]paletteuse=dither=bayer:bayer_scale=3"
    cmd = [
        "ffmpeg", "-y",
        "-i", str(input_path),
        "-vf", palette_filter,
        str(output_path)
    ]
    return subprocess.run(cmd, capture_output=True, text=True)

def main():
    parser = argparse.ArgumentParser(description="LiamTool FFmpeg Converter CLI")
    parser.add_argument("input", help="Archivo de video o frames de entrada")
    parser.add_argument("output", help="Archivo de salida")
    parser.add_argument("--format", choices=["webm", "mp4", "gif"], default="webm", help="Formato de salida")
    parser.add_argument("--fps", type=int, default=30, help="Fotogramas por segundo")
    parser.add_argument("--bitrate", default="2500k", help="Tasa de bits (bitrate)")

    args = parser.parse_args()

    if not check_ffmpeg_available():
        print("[AVISO] FFmpeg no se encontro en el PATH del sistema. Asegurate de tener FFmpeg instalado para ejecutar esta herramienta.")
        print("[INFO] Comando equivalente que puedes ejecutar manualmente:")
        if args.format == "webm":
            print(f"ffmpeg -i {args.input} -c:v libvpx -b:v {args.bitrate} -r {args.fps} -an -pix_fmt yuva420p {args.output}")
        elif args.format == "mp4":
            print(f"ffmpeg -i {args.input} -vf fps={args.fps},format=yuv420p -c:v libx264 -crf 18 -an {args.output}")
        elif args.format == "gif":
            print(f"ffmpeg -i {args.input} -vf fps={args.fps},split[s0][s1];[s0]palettegen[p];[s1][p]paletteuse {args.output}")
        return

    print(f"[PROCESANDO] Convirtiendo {args.input} a {args.output} (Formato: {args.format.upper()})...")

    if args.format == "webm":
        res = convert_to_webm_vp8(args.input, args.output, args.bitrate, args.fps)
    elif args.format == "mp4":
        res = convert_to_mp4_h264(args.input, args.output, fps=args.fps)
    elif args.format == "gif":
        res = convert_to_optimized_gif(args.input, args.output, fps=args.fps)

    if res.returncode == 0:
        print(f"[OK] Archivo generado exitosamente en {args.output}")
    else:
        print(f"[ERROR] FFmpeg retorno codigo {res.returncode}:\n{res.stderr}")
        sys.exit(1)

if __name__ == "__main__":
    main()
