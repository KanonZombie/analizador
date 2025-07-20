using Xunit;
using Analizador.Console;
using System.Collections.Generic;

public class ProgramTests
{
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
        var dict = Program.ParseDiffFile(tempFile);
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
        var list = Program.ParseListFile(tempFile);
        Assert.Equal(10, list[0].Numero);
        Assert.Equal("TextoA", list[0].Contenido);
        Assert.Equal(11, list[1].Numero);
        Assert.Equal("TextoB", list[1].Contenido);
        System.IO.File.Delete(tempFile);
    }
}
