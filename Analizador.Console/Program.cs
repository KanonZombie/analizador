using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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

        var dictionary = ParseDiffFile(diffFile);
        var list = ParseListFile(listFile);
        AnalizadorProcessor.ProcessAnalysis(dictionary, list, outputFile);
    }

    public static Dictionary<int, string> ParseDiffFile(string diffFile)
    /// <summary>
    /// Parsea el archivo diff y devuelve un diccionario con clave int y valor string.
    /// </summary>
    {
        var lines = File.ReadAllLines(diffFile);
        return lines
            .Select(x => Regex.Match(x, "([A-F0-9]{8}): ([A-F0-9]{2} [A-F0-9]{2})"))
            .Where(x => x.Success && !string.IsNullOrEmpty(x.Groups[1].Value))
            .ToDictionary(
                t => HexaStringAInt(t.Groups[1].Value),
                t => t.Groups[2].Value
            );
    }

    public static List<Linea> ParseListFile(string listFile)
    /// <summary>
    /// Parsea el archivo list y devuelve una lista de objetos Linea.
    /// </summary>
    {
        var lines = File.ReadAllLines(listFile);
        return lines
            .Select(x => Regex.Match(x, "([A-F0-9]{8}) [0-9A-F ]+?\t(.+)"))
            .Where(x => x.Success && !string.IsNullOrEmpty(x.Groups[1].Value))
            .Select(t => new Linea
            {
                Numero = HexaStringAInt(t.Groups[1].Value),
                Contenido = t.Groups[2].Value
            })
            .ToList();
    }

    // ...el resto del código permanece igual...

    public static string IntAHexa(int numero) => numero.ToString("X").PadLeft(8, '0');

    public static int HexaStringAInt(string hexa) => int.Parse(hexa, NumberStyles.HexNumber);
}
