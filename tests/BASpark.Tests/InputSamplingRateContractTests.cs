namespace BASpark.Tests;

public class InputSamplingRateContractTests
{
    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(40, 40)]
    [InlineData(1000, 1000)]
    [InlineData(1001, 1000)]
    public void NormalizeInputSamplingRate_UsesUpstreamApiRange(
        int value,
        int expected)
    {
        Assert.Equal(expected, ConfigManager.NormalizeInputSamplingRate(value));
    }

    [Fact]
    public void BuildInputSamplingRateScript_UsesNormalizedInteger()
    {
        Assert.Equal(
            "if(window.updateInputSamplingRate) window.updateInputSamplingRate(40);",
            MainWindow.BuildInputSamplingRateScript(40));
        Assert.Equal(
            "if(window.updateInputSamplingRate) window.updateInputSamplingRate(1000);",
            MainWindow.BuildInputSamplingRateScript(2000));
    }
}
