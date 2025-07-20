using System.Globalization;
using System.IO;
using System.Linq;

namespace Analizador.Console;

public class Program
{
    private static void Main(string[] args)
    {
        if (args.Length < 3)
        {
            System.Console.WriteLine("Uso: Analizador.Console <diffFile> <listFile> <outputFile>");
            return;
        }

        string diffFile = args[0];
        string listFile = args[1];
        string outputFile = args[2];

        if (!File.Exists(diffFile))
        {
            System.Console.WriteLine($"No se encuentra el archivo diff: {diffFile}");
            return;
        }
        if (!File.Exists(listFile))
        {
            System.Console.WriteLine($"No se encuentra el archivo list: {listFile}");
            return;
        }

        // Borrar el archivo de salida si existe
        if (File.Exists(outputFile))
        {
            File.Delete(outputFile);
        }

        var dictionary = FileParser.StreamDiffFile(diffFile).ToDictionary(kv => kv.Key, kv => kv.Value);
        var list = FileParser.StreamListFile(listFile).ToList();
        AnalizadorProcessor.ProcessAnalysis(dictionary, list, outputFile);
    }

    // ...el resto del código permanece igual...

    // ...el resto del código permanece igual...

    public static string IntAHexa(int numero) => numero.ToString("X").PadLeft(8, '0');

    public static int HexaStringAInt(string hexa)
    {
        if (int.TryParse(hexa, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int result))
            return result;
        System.Console.WriteLine($"Error: No se pudo convertir la dirección hexadecimal '{hexa}'.");
        return -1;
    }
}
