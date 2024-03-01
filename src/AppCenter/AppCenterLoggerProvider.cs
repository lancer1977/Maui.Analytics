namespace Maui.Analytics.AppCenter;
    public class AppCenterLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly LogLevel _logLevel;

        private IExternalScopeProvider _scopeProvider;

        public AppCenterLoggerProvider(LogLevel logLevel) => _logLevel = logLevel;

        public ILogger CreateLogger(string categoryName) => new AppCenterLogger(categoryName, _logLevel, _scopeProvider);
        public void Dispose() { }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
            => _scopeProvider = scopeProvider;
    }