namespace BASpark
{
    internal sealed class WebViewUnresponsiveTracker
    {
        private const int RecoveryThreshold = 3;
        private static readonly long ContinuityWindowTicks =
            TimeSpan.FromSeconds(15).Ticks;

        private object? _webViewIdentity;
        private long _lastReportTicks;

        internal int ConsecutiveReports { get; private set; }

        internal bool Register(object webViewIdentity, long reportTicks)
        {
            long elapsedTicks = reportTicks - _lastReportTicks;
            bool continuesPreviousSequence =
                ReferenceEquals(_webViewIdentity, webViewIdentity) &&
                elapsedTicks >= 0 &&
                elapsedTicks <= ContinuityWindowTicks;

            ConsecutiveReports = continuesPreviousSequence
                ? ConsecutiveReports + 1
                : 1;
            _webViewIdentity = webViewIdentity;
            _lastReportTicks = reportTicks;

            return ConsecutiveReports >= RecoveryThreshold;
        }

        internal void Reset()
        {
            _webViewIdentity = null;
            _lastReportTicks = 0;
            ConsecutiveReports = 0;
        }
    }
}
