import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.List;

/**
 * LiamTool Avatar Batch CLI (Java)
 * Validador de catalogos de avatares, texturas estaticas e integridad de DancerCardCatalog.
 */
public class AvatarBatchCLI {

    private static final String[] EXPECTED_TEXTURE_SUFFIXES = {
        "Banner", "Cursor", "DancerCard", "Full", "Hud", "Portrait", "Square"
    };

    public static void main(String[] args) {
        if (args.length == 0 || args[0].equals("--help") || args[0].equals("-h")) {
            printUsage();
            return;
        }

        String command = args[0];
        if (command.equalsIgnoreCase("validate")) {
            if (args.length < 2) {
                System.err.println("[ERROR] Especifica el directorio del avatar a validar.");
                System.exit(1);
            }
            validateAvatarFolder(new File(args[1]));
        } else if (command.equalsIgnoreCase("scan")) {
            if (args.length < 2) {
                System.err.println("[ERROR] Especifica el directorio de DancerCardCatalog a escanear.");
                System.exit(1);
            }
            scanCatalogRoot(new File(args[1]));
        } else {
            System.err.println("[ERROR] Comando desconocido: " + command);
            printUsage();
            System.exit(1);
        }
    }

    private static void printUsage() {
        System.out.println("LiamTool Avatar Batch CLI (Java 8+)");
        System.out.println("Uso:");
        System.out.println("  java AvatarBatchCLI validate <ruta_carpeta_avatar>");
        System.out.println("  java AvatarBatchCLI scan <ruta_catalogo_raiz>");
    }

    private static void validateAvatarFolder(File dir) {
        if (!dir.exists() || !dir.isDirectory()) {
            System.err.println("[ERROR] La ruta especificada no existe o no es una carpeta valida: " + dir.getAbsolutePath());
            System.exit(1);
        }

        String avatarName = dir.getName();
        System.out.println("[VALIDANDO] Avatar: " + avatarName + " en " + dir.getAbsolutePath());

        File atlasFile = new File(dir, avatarName + "_Atlas.png");
        File jsonFile = new File(dir, avatarName + ".json");

        boolean hasAtlas = atlasFile.exists();
        boolean hasJson = jsonFile.exists();

        System.out.println("  - Atlas PNG: " + (hasAtlas ? "OK (" + atlasFile.length() + " bytes)" : "FALTA"));
        System.out.println("  - Spine JSON: " + (hasJson ? "OK (" + jsonFile.length() + " bytes)" : "FALTA"));

        int foundTextures = 0;
        File staticDir = new File(dir, "StaticTextures");
        File searchDir = staticDir.exists() ? staticDir : dir;

        for (String suffix : EXPECTED_TEXTURE_SUFFIXES) {
            boolean found = false;
            File[] files = searchDir.listFiles();
            if (files != null) {
                for (File f : files) {
                    if (f.getName().toLowerCase().endsWith(".png") && f.getName().toLowerCase().contains(suffix.toLowerCase())) {
                        found = true;
                        break;
                    }
                }
            }
            if (found) {
                foundTextures++;
            } else {
                System.out.println("  - Advertencia: Textura '" + suffix + "' no encontrada en " + searchDir.getName());
            }
        }

        System.out.println("  - Texturas estaticas verificadas: " + foundTextures + " de " + EXPECTED_TEXTURE_SUFFIXES.length);

        if (hasAtlas && hasJson) {
            System.out.println("[RESULTADO] Avatar '" + avatarName + "' cumple con los requisitos minimos del juego.");
        } else {
            System.out.println("[RESULTADO] Avatar '" + avatarName + "' tiene archivos indispensables pendientes.");
        }
    }

    private static void scanCatalogRoot(File rootDir) {
        if (!rootDir.exists() || !rootDir.isDirectory()) {
            System.err.println("[ERROR] Directorio raiz no valido: " + rootDir.getAbsolutePath());
            System.exit(1);
        }

        System.out.println("[ESCANEANDO] Catalogo raiz: " + rootDir.getAbsolutePath());
        File[] subdirs = rootDir.listFiles(File::isDirectory);
        if (subdirs == null || subdirs.length == 0) {
            System.out.println("No se encontraron subcarpetas de avatares en " + rootDir.getAbsolutePath());
            return;
        }

        int validCount = 0;
        for (File sub : subdirs) {
            String name = sub.getName();
            File atlas = new File(sub, name + "_Atlas.png");
            File json = new File(sub, name + ".json");
            if (atlas.exists() && json.exists()) {
                validCount++;
                System.out.println("  [OK] " + name + " (Completo)");
            } else {
                System.out.println("  [INCOMPLETO] " + name);
            }
        }

        System.out.println("[RESUMEN] Total avatares: " + subdirs.length + " | Completos: " + validCount);
    }
}
