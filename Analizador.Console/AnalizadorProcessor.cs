using System;
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
        // Diccionario auxiliar para búsquedas rápidas por Numero
        var lineasPorNumero = list.GroupBy(x => x.Numero)
                                  .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.Numero).First());

        var source3 = new Dictionary<string, string>();
        var agregados = new HashSet<string>();
        foreach (var item in dictionary)
        {
            string clave;
            string valor;
            if (lineasPorNumero.TryGetValue(item.Key, out var lineaIgual))
            {
                clave = Program.IntAHexa(item.Key);
                valor = lineaIgual.Contenido;
            }
            else
            {
                var menores = list.Where(x => x.Numero < item.Key)
                    .OrderByDescending(x => x.Numero)
                    .ThenByDescending(x => x.Contenido);
                var linea = menores.FirstOrDefault();
                if (linea == null)
                    continue;
                clave = Program.IntAHexa(linea.Numero);
                valor = linea.Contenido;
            }
            if (!agregados.Contains(clave))
            {
                source3.Add(clave, valor);
                agregados.Add(clave);
            }
            // Escribir en bloques de 300 para eficiencia
            if (source3.Count == 300)
            {
                File.AppendAllText(outputFile,
                    string.Join(Environment.NewLine,
                        source3.Select(x => x.Key + "\t" + x.Value)));
                source3.Clear();
            }
        }
        // Escribir el resto si queda algo pendiente
        if (source3.Any())
        {
            File.AppendAllText(outputFile,
                string.Join(Environment.NewLine, source3.Select(x => x.Key + "\t" + x.Value)));
        }
    }
}
