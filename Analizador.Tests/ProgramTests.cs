using System.Linq;
using Analizador.Console;
using Xunit;

public class ProgramTests
{
    // --- Edge case: duplicate lines ---
    [Fact]
    public void ParseDiffFile_DuplicateLines_LastWins()
{
        var lines = new[] { "0000000A: 01 02", "0000000A: 03 04" };
        var tempFile = System.IO.Path.GetTempFileName();
        System.IO.File.WriteAllLines(tempFile, lines);
        try
        {
            var dict = FileParser.StreamDiffFile(tempFile)
                .GroupBy(kv => kv.Key)
                .ToDictionary(g => g.Key, g => g.Last().Value);
            Assert.Single(dict);
            Assert.Equal("03 04", dict[10]);
        }
        finally
        {
            System.IO.File.Delete(tempFile);
        }
    }

    // --- Edge case: extra whitespace ---
    [Fact]
    public void ParseDiffFile_ExtraWhitespace_ParsesCorrectly()
    {
        var lines = new[] { "   0000000A:   01 02   ", "0000000B:  03 04" };
        var tempFile = System.IO.Path.GetTempFileName();
        System.IO.File.WriteAllLines(tempFile, lines);
        try
        {
            var dict = FileParser.StreamDiffFile(tempFile).ToDictionary(kv => kv.Key, kv => kv.Value);
            Assert.Equal("01 02", dict[10]);
            Assert.Equal("03 04", dict[11]);
        }
        finally
        {
            System.IO.File.Delete(tempFile);
        }
    }

    // --- Edge case: invalid hex ---
    [Fact]
    public void HexaStringAInt_InvalidHex_ReturnsMinusOne()
    {
        Assert.Equal(-1, Program.HexaStringAInt("ZZZZ"));
    }

    // --- Negative test: missing file ---
    [Fact]
    public void StreamDiffFile_MissingFile_ThrowsFileNotFound()
    {
        var missingFile = "no_such_file.diff";
        var ex = Assert.Throws<System.IO.FileNotFoundException>(() => FileParser.StreamDiffFile(missingFile).ToList());
        Assert.Contains(missingFile, ex.Message);
    }

    // --- Performance test: large file ---
    [Fact]
    public void ParseDiffFile_LargeFile_ParsesAll()
    {
        var lines = Enumerable.Range(0, 10000).Select(i => $"{i:X8}: 01 02").ToArray();
        var tempFile = System.IO.Path.GetTempFileName();
        System.IO.File.WriteAllLines(tempFile, lines);
        try
        {
            var dict = FileParser.StreamDiffFile(tempFile).ToDictionary(kv => kv.Key, kv => kv.Value);
            Assert.Equal(10000, dict.Count);
            Assert.Equal("01 02", dict[9999]);
        }
        finally
        {
            System.IO.File.Delete(tempFile);
        }
    }
    [Fact]
    public void ParseDiffFile_EmptyFile_ReturnsEmptyDictionary()
    {
        var tempFile = System.IO.Path.GetTempFileName();
        try
        {
            var dict = FileParser.StreamDiffFile(tempFile).ToDictionary(kv => kv.Key, kv => kv.Value);
            Assert.Empty(dict);
        }
        finally
        {
            System.IO.File.Delete(tempFile);
        }
    }

    [Fact]
    public void ParseDiffFile_MalformedLines_IgnoresMalformed()
    {
        var lines = new[] { "malformed line", "0000000A: 01 02", "bad: data" };
        var tempFile = System.IO.Path.GetTempFileName();
        System.IO.File.WriteAllLines(tempFile, lines);
        try
        {
            var dict = FileParser.StreamDiffFile(tempFile).ToDictionary(kv => kv.Key, kv => kv.Value);
            Assert.Single(dict);
            Assert.Equal("01 02", dict[10]);
        }
        finally
        {
            System.IO.File.Delete(tempFile);
        }
    }

    [Fact]
    public void IntAHexa_ConvertsCorrectly()
    {
        Assert.Equal("0000000A", Program.IntAHexa(10));
        Assert.Equal("000000FF", Program.IntAHexa(255));
    }

    [Fact]
    public void HexaStringAInt_ConvertsCorrectly()
    {
        Assert.Equal(10, Program.HexaStringAInt("A"));
        Assert.Equal(255, Program.HexaStringAInt("FF"));
    }

    [Fact]
    public void ParseDiffFile_ParsesCorrectly()
    {
        var lines = new[] { "0000000A: 01 02", "0000000B: 03 04" };
        var tempFile = System.IO.Path.GetTempFileName();
        System.IO.File.WriteAllLines(tempFile, lines);
        var dict = FileParser.StreamDiffFile(tempFile).ToDictionary(kv => kv.Key, kv => kv.Value);
        Assert.Equal("01 02", dict[10]);
        Assert.Equal("03 04", dict[11]);
        System.IO.File.Delete(tempFile);
    }

    [Fact]
    public void ParseListFile_ParsesCorrectly()
    {
        var lines = new[] { "0000000A 01 02\tTextoA", "0000000B 03 04\tTextoB" };
        var tempFile = System.IO.Path.GetTempFileName();
        System.IO.File.WriteAllLines(tempFile, lines);
        var list = FileParser.StreamListFile(tempFile).ToList();
        Assert.Equal(10, list[0].Numero);
        Assert.Equal("TextoA", list[0].Contenido);
        Assert.Equal(11, list[1].Numero);
        Assert.Equal("TextoB", list[1].Contenido);
        System.IO.File.Delete(tempFile);
    }
}
