using System.Collections.Generic;
using System.IO;
using System.Linq;
using Analizador.Console;
using Xunit;

public class AnalizadorProcessorTests
{
    [Fact]
    public void ProcessAnalysis_WritesExactMatch()
    {
        var dict = new Dictionary<int, string> { { 10, "01 02" } };
        var list = new List<Linea> { new Linea { Numero = 10, Contenido = "TextoA" } };
        var tempFile = Path.GetTempFileName();
        AnalizadorProcessor.ProcessAnalysis(dict, list, tempFile);
        var output = File.ReadAllText(tempFile);
        Assert.Contains("0000000A\tTextoA", output);
        File.Delete(tempFile);
    }

    [Fact]
    public void ProcessAnalysis_WritesClosestLowerMatch()
    {
        var dict = new Dictionary<int, string> { { 15, "01 02" } };
        var list = new List<Linea> {
            new Linea { Numero = 10, Contenido = "TextoA" },
            new Linea { Numero = 12, Contenido = "TextoB" }
        };
        var tempFile = Path.GetTempFileName();
        AnalizadorProcessor.ProcessAnalysis(dict, list, tempFile);
        var output = File.ReadAllText(tempFile);
        Assert.Contains("0000000C\tTextoB", output); // 12 en hex
        File.Delete(tempFile);
    }

    [Fact]
    public void ProcessAnalysis_EmptyDictionaryProducesNoOutput()
    {
        var dict = new Dictionary<int, string>();
        var list = new List<Linea> { new Linea { Numero = 10, Contenido = "TextoA" } };
        var tempFile = Path.GetTempFileName();
        AnalizadorProcessor.ProcessAnalysis(dict, list, tempFile);
        var output = File.ReadAllText(tempFile);
        Assert.True(string.IsNullOrWhiteSpace(output));
        File.Delete(tempFile);
    }

    [Fact]
    public void ProcessAnalysis_EmptyListProducesNoOutput()
    {
        var dict = new Dictionary<int, string> { { 10, "01 02" } };
        var list = new List<Linea>();
        var tempFile = Path.GetTempFileName();
        AnalizadorProcessor.ProcessAnalysis(dict, list, tempFile);
        var output = File.ReadAllText(tempFile);
        Assert.True(string.IsNullOrWhiteSpace(output));
        File.Delete(tempFile);
    }

    [Fact]
    public void ProcessAnalysis_BorderCase_MultipleMatchesAndOverflow()
    {
        var dict = Enumerable.Range(1, 305).ToDictionary(i => i, i => "X");
        var list = Enumerable.Range(1, 305).Select(i => new Linea { Numero = i, Contenido = $"C{i}" }).ToList();
        var tempFile = Path.GetTempFileName();
        AnalizadorProcessor.ProcessAnalysis(dict, list, tempFile);
        var output = File.ReadAllText(tempFile);
        // Debe haber 304 líneas únicas (sin duplicados)
        var uniqueLines = output.Split('\n').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().Count();
        Assert.Equal(304, uniqueLines);
        File.Delete(tempFile);
    }
}
