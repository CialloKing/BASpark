using System.Collections;
using System.Reflection;
using System.Resources;

namespace BASpark.Tests;

public class PackagedRendererResourcesTests
{
    private static readonly string[] ExpectedWebResources =
    {
        "web/index.html",
        "web/index.legacy.html",
        "web/fx-adapter.js",
        "web/vendor/ba-click-fx.iife.js"
    };

    private static readonly string[] ExpectedLicenseFiles =
    {
        "LICENSE",
        "THIRD_PARTY_NOTICES.md",
        "VERSION.txt"
    };

    [Fact]
    public void WebResources_AreEmbeddedAndNonEmpty()
    {
        Assembly assembly = typeof(WebRendererDocumentBuilder).Assembly;
        string generatedResourceName = Assert.Single(
            assembly.GetManifestResourceNames(),
            name => name.EndsWith(".g.resources", StringComparison.Ordinal));

        using Stream stream = Assert.IsAssignableFrom<Stream>(
            assembly.GetManifestResourceStream(generatedResourceName));
        using var reader = new ResourceReader(stream);
        var resourceNames = reader.Cast<DictionaryEntry>()
            .Select(entry => Assert.IsType<string>(entry.Key))
            .ToHashSet(StringComparer.Ordinal);

        foreach (string expectedResource in ExpectedWebResources)
        {
            Assert.Contains(expectedResource, resourceNames);
            reader.GetResourceData(
                expectedResource,
                out string resourceType,
                out byte[] resourceData);

            Assert.False(string.IsNullOrWhiteSpace(resourceType));
            Assert.NotEmpty(resourceData);
        }
    }

    [Fact]
    public void ThirdPartyLicenseFiles_AreCopiedToOutput()
    {
        string licenseDirectory = Path.Combine(
            AppContext.BaseDirectory,
            "licenses",
            "ba-click-fx");

        foreach (string expectedFile in ExpectedLicenseFiles)
        {
            var file = new FileInfo(Path.Combine(licenseDirectory, expectedFile));

            Assert.True(file.Exists, $"Missing third-party file: {file.FullName}");
            Assert.True(file.Length > 0, $"Empty third-party file: {file.FullName}");
        }
    }
}
