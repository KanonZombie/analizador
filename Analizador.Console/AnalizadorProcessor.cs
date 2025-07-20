using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Analizador.Console;

public static class AnalizadorProcessor
{
    /// <summary>
    /// Procesa el análisis y escribe los resultados en el archivo de salida.
    /// </summary>
    public static void ProcessAnalysis(Dictionary<int, string> dictionary, List<Linea> list, string outputFile)
    {
        // Diccionario auxiliar para búsquedas rápidas por dirección
        var instruccionesPorDireccion = list.GroupBy(x => x.Numero)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.Numero).First());

        var outputBuffer = new List<string>(300);
        var direccionesAgregadas = new HashSet<string>();

        using (var writer = new StreamWriter(outputFile, append: true))
        {
            foreach (var diferencia in dictionary)
            {
                string direccionHex;
                string instruccion;
                if (instruccionesPorDireccion.TryGetValue(diferencia.Key, out var lineaIgual))
                {
                    direccionHex = Program.IntAHexa(diferencia.Key);
                    instruccion = lineaIgual.Contenido;
                }
                else
                {
                    var menores = list.Where(x => x.Numero < diferencia.Key)
                        .OrderByDescending(x => x.Numero)
                        .ThenByDescending(x => x.Contenido);
                    var linea = menores.FirstOrDefault();
                    if (linea == null)
                        continue;
                    direccionHex = Program.IntAHexa(linea.Numero);
                    instruccion = linea.Contenido;
                }
                if (!direccionesAgregadas.Contains(direccionHex))
                {
                    outputBuffer.Add(direccionHex + "\t" + instruccion);
                    direccionesAgregadas.Add(direccionHex);
                }
                // Escribir en bloques de 300 para eficiencia
                if (outputBuffer.Count == 300)
                {
                    for (int i = 0; i < outputBuffer.Count; i++)
                    {
                        if (i == outputBuffer.Count - 1 && !dictionary.Keys.Last().Equals(diferencia.Key) && !outputBuffer.Any())
                            writer.Write(outputBuffer[i]);
                        else
                            writer.WriteLine(outputBuffer[i]);
                    }
                    outputBuffer.Clear();
                }
            }
            // Escribir el resto si queda algo pendiente
            if (outputBuffer.Any())
            {
                for (int i = 0; i < outputBuffer.Count; i++)
                {
                    if (i == outputBuffer.Count - 1)
                        writer.Write(outputBuffer[i]);
                    else
                        writer.WriteLine(outputBuffer[i]);
                }
            }
        }
    }
}
