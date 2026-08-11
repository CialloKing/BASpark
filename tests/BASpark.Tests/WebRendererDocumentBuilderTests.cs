namespace BASpark.Tests;

public class WebRendererDocumentBuilderTests
{
    [Fact]
    public void Build_InsertsVendorBeforeAdapterAndRemovesPlaceholder()
    {
        string template =
            $"<html><body>{WebRendererDocumentBuilder.ScriptPlaceholder}</body></html>";

        string result = WebRendererDocumentBuilder.Build(
            template,
            "window.vendorLoaded = true;",
            "window.adapterLoaded = true;");

        int vendorIndex = result.IndexOf("window.vendorLoaded", StringComparison.Ordinal);
        int adapterIndex = result.IndexOf("window.adapterLoaded", StringComparison.Ordinal);

        Assert.True(vendorIndex >= 0);
        Assert.True(adapterIndex > vendorIndex);
        Assert.DoesNotContain(
            WebRendererDocumentBuilder.ScriptPlaceholder,
            result,
            StringComparison.Ordinal);
    }

    [Fact]
    public void Build_RejectsMissingPlaceholder()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
        {
            WebRendererDocumentBuilder.Build(
                "<html></html>",
                "window.vendorLoaded = true;",
                "window.adapterLoaded = true;");
        });

        Assert.Contains("was not found", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Build_RejectsDuplicatePlaceholder()
    {
        string template =
            WebRendererDocumentBuilder.ScriptPlaceholder +
            WebRendererDocumentBuilder.ScriptPlaceholder;

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
        {
            WebRendererDocumentBuilder.Build(
                template,
                "window.vendorLoaded = true;",
                "window.adapterLoaded = true;");
        });

        Assert.Contains("exactly once", exception.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("", "window.adapterLoaded = true;")]
    [InlineData(" ", "window.adapterLoaded = true;")]
    [InlineData("window.vendorLoaded = true;", "")]
    [InlineData("window.vendorLoaded = true;", "\t")]
    public void Build_RejectsEmptyScripts(string vendorScript, string adapterScript)
    {
        Assert.Throws<ArgumentException>(() =>
        {
            WebRendererDocumentBuilder.Build(
                WebRendererDocumentBuilder.ScriptPlaceholder,
                vendorScript,
                adapterScript);
        });
    }

    [Fact]
    public void Build_EscapesScriptEndTagsCaseInsensitively()
    {
        string result = WebRendererDocumentBuilder.Build(
            WebRendererDocumentBuilder.ScriptPlaceholder,
            "const vendor = '</ScRiPt>';",
            "const adapter = '</script>'; ");

        Assert.DoesNotContain("'</ScRiPt>'", result, StringComparison.Ordinal);
        Assert.DoesNotContain("'</script>'", result, StringComparison.Ordinal);
        Assert.Equal(
            2,
            result.Split("<\\/script>", StringSplitOptions.None).Length - 1);
    }
}
