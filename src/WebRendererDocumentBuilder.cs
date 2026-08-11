namespace BASpark
{
    internal static class WebRendererDocumentBuilder
    {
        internal const string ScriptPlaceholder = "<!-- BASPARK_RENDERER_SCRIPTS -->";

        internal static string Build(
            string template,
            string vendorScript,
            string adapterScript)
        {
            if (string.IsNullOrWhiteSpace(template))
            {
                throw new ArgumentException("Renderer template cannot be empty.", nameof(template));
            }
            if (string.IsNullOrWhiteSpace(vendorScript))
            {
                throw new ArgumentException("Renderer vendor script cannot be empty.", nameof(vendorScript));
            }
            if (string.IsNullOrWhiteSpace(adapterScript))
            {
                throw new ArgumentException("Renderer adapter script cannot be empty.", nameof(adapterScript));
            }

            int placeholderIndex = template.IndexOf(ScriptPlaceholder, StringComparison.Ordinal);
            if (placeholderIndex < 0)
            {
                throw new InvalidOperationException(
                    $"Renderer placeholder '{ScriptPlaceholder}' was not found.");
            }

            int nextPlaceholderIndex = template.IndexOf(
                ScriptPlaceholder,
                placeholderIndex + ScriptPlaceholder.Length,
                StringComparison.Ordinal);
            if (nextPlaceholderIndex >= 0)
            {
                // Multiple copies would initialize two renderers and duplicate every host input.
                throw new InvalidOperationException(
                    $"Renderer placeholder '{ScriptPlaceholder}' must occur exactly once.");
            }

            string inlineScripts =
                $"<script>\n{EscapeInlineScript(vendorScript)}\n</script>\n" +
                $"<script>\n{EscapeInlineScript(adapterScript)}\n</script>";

            return template[..placeholderIndex] +
                inlineScripts +
                template[(placeholderIndex + ScriptPlaceholder.Length)..];
        }

        internal static string EscapeInlineScript(string script)
        {
            // HTML parsers terminate inline scripts before JavaScript parsing, even inside JS strings.
            return script.Replace(
                "</script",
                "<\\/script",
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
