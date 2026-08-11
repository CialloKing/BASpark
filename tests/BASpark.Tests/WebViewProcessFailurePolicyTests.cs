using Microsoft.Web.WebView2.Core;

namespace BASpark.Tests;

public class WebViewProcessFailurePolicyTests
{
    [Theory]
    [InlineData(
        CoreWebView2ProcessFailedKind.BrowserProcessExited,
        (int)WebViewProcessRecoveryAction.RecreateWebViewControl)]
    [InlineData(
        CoreWebView2ProcessFailedKind.RenderProcessExited,
        (int)WebViewProcessRecoveryAction.RebuildRendererDocument)]
    public void GetRecoveryAction_RecoversFailuresThatInvalidateTheMainDocument(
        CoreWebView2ProcessFailedKind failureKind,
        int expectedValue)
    {
        Assert.Equal(
            (WebViewProcessRecoveryAction)expectedValue,
            WebViewProcessFailurePolicy.GetRecoveryAction(failureKind));
    }

    [Theory]
    [InlineData(CoreWebView2ProcessFailedKind.RenderProcessUnresponsive)]
    [InlineData(CoreWebView2ProcessFailedKind.FrameRenderProcessExited)]
    [InlineData(CoreWebView2ProcessFailedKind.UtilityProcessExited)]
    [InlineData(CoreWebView2ProcessFailedKind.SandboxHelperProcessExited)]
    [InlineData(CoreWebView2ProcessFailedKind.GpuProcessExited)]
    [InlineData(CoreWebView2ProcessFailedKind.PpapiPluginProcessExited)]
    [InlineData(CoreWebView2ProcessFailedKind.PpapiBrokerProcessExited)]
    [InlineData(CoreWebView2ProcessFailedKind.UnknownProcessExited)]
    public void GetRecoveryAction_PreservesRuntimeManagedFailures(
        CoreWebView2ProcessFailedKind failureKind)
    {
        Assert.Equal(
            WebViewProcessRecoveryAction.None,
            WebViewProcessFailurePolicy.GetRecoveryAction(failureKind));
    }
}
