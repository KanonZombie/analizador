using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Analizador.Console;

public static class FileParser
{
    // Compiled regexes for performance
    private static readonly Regex DiffRegex = new Regex(@"\s*([A-F0-9]{8}):\s*([A-F0-9]{2}\s+[A-F0-9]{2})\s*", RegexOptions.Compiled);
    private static readonly Regex ListRegex = new Regex(@"\s*([A-F0-9]{8})\s+[0-9A-F ]+?\t(.+)", RegexOptions.Compiled);
    /// <summary>
    /// Parsea el archivo diff y devuelve un enumerable de pares clave-valor (int, string).
    /// </summary>
    /// <param name="diffFile">Ruta al archivo diff.</param>
    /// <returns>IEnumerable de dirección (int) y bytes (string).</returns>
    public static IEnumerable<KeyValuePair<int, string>> StreamDiffFile(string diffFile)
    {
        if (string.IsNullOrWhiteSpace(diffFile) || !File.Exists(diffFile))
            throw new FileNotFoundException($"Archivo diff no encontrado: {diffFile}");
        using var reader = new StreamReader(diffFile);
        string line;
        int lineNumber = 0;
        while ((line = reader.ReadLine()) != null)
        {
            lineNumber++;
            var match = DiffRegex.Match(line);
            if (match.Success && !string.IsNullOrEmpty(match.Groups[1].Value))
            {
                yield return new KeyValuePair<int, string>(Program.HexaStringAInt(match.Groups[1].Value), match.Groups[2].Value);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[StreamDiffFile] Línea mal formateada en {diffFile} (línea {lineNumber}): '{line}'");
            }
        }
    }

    /// <summary>
    /// <summary>
    /// Parsea el archivo lst y devuelve un enumerable de objetos Linea.
    /// </summary>
    /// <param name="listFile">Ruta al archivo lst.</param>
    /// <returns>IEnumerable de objetos Linea parseados.</returns>
    public static IEnumerable<Linea> StreamListFile(string listFile)
    {
        if (string.IsNullOrWhiteSpace(listFile) || !File.Exists(listFile))
            throw new FileNotFoundException($"Archivo lst no encontrado: {listFile}");
        using var reader = new StreamReader(listFile);
        string line;
        int lineNumber = 0;
        while ((line = reader.ReadLine()) != null)
        {
            lineNumber++;
            var match = ListRegex.Match(line);
            if (match.Success && !string.IsNullOrEmpty(match.Groups[1].Value))
            {
                yield return new Linea
                {
                    Numero = Program.HexaStringAInt(match.Groups[1].Value),
                    Contenido = match.Groups[2].Value
                };
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[StreamListFile] Línea mal formateada en {listFile} (línea {lineNumber}): '{line}'");
            }
        }
    }
}
