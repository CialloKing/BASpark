using Microsoft.Web.WebView2.Core;

namespace BASpark
{
    internal enum WebViewProcessRecoveryAction
    {
        None,
        RebuildRendererDocument,
        RecreateWebViewControl
    }

    internal static class WebViewProcessFailurePolicy
    {
        internal static WebViewProcessRecoveryAction GetRecoveryAction(
            CoreWebView2ProcessFailedKind failureKind)
        {
            return failureKind switch
            {
                CoreWebView2ProcessFailedKind.BrowserProcessExited =>
                    WebViewProcessRecoveryAction.RecreateWebViewControl,
                CoreWebView2ProcessFailedKind.RenderProcessExited =>
                    WebViewProcessRecoveryAction.RebuildRendererDocument,
                _ => WebViewProcessRecoveryAction.None
            };
        }
    }
}
