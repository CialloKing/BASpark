namespace BASpark.Tests;

public class WebViewUnresponsiveTrackerTests
{
    [Fact]
    public void Register_RecoversAfterThreeContinuousReports()
    {
        var tracker = new WebViewUnresponsiveTracker();
        var webView = new object();
        long startTicks = TimeSpan.FromSeconds(10).Ticks;

        Assert.False(tracker.Register(webView, startTicks));
        Assert.False(
            tracker.Register(webView, startTicks + TimeSpan.FromSeconds(4).Ticks));
        Assert.True(
            tracker.Register(webView, startTicks + TimeSpan.FromSeconds(8).Ticks));
        Assert.Equal(3, tracker.ConsecutiveReports);
    }

    [Fact]
    public void Register_ResetsSequenceAfterContinuityWindow()
    {
        var tracker = new WebViewUnresponsiveTracker();
        var webView = new object();

        Assert.False(tracker.Register(webView, TimeSpan.FromSeconds(1).Ticks));
        Assert.False(tracker.Register(webView, TimeSpan.FromSeconds(17).Ticks));
        Assert.Equal(1, tracker.ConsecutiveReports);
    }

    [Fact]
    public void Register_ResetsSequenceForAnotherWebView()
    {
        var tracker = new WebViewUnresponsiveTracker();
        long startTicks = TimeSpan.FromSeconds(1).Ticks;

        Assert.False(tracker.Register(new object(), startTicks));
        Assert.False(
            tracker.Register(new object(), startTicks + TimeSpan.FromSeconds(1).Ticks));
        Assert.Equal(1, tracker.ConsecutiveReports);
    }

    [Fact]
    public void Reset_ClearsSequence()
    {
        var tracker = new WebViewUnresponsiveTracker();
        var webView = new object();

        tracker.Register(webView, TimeSpan.FromSeconds(1).Ticks);
        tracker.Register(webView, TimeSpan.FromSeconds(2).Ticks);
        tracker.Reset();

        Assert.False(tracker.Register(webView, TimeSpan.FromSeconds(3).Ticks));
        Assert.Equal(1, tracker.ConsecutiveReports);
    }
}
