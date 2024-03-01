namespace Maui.Analytics.AppCenter;
public class AppCenterLogger : ILogger
{
    readonly string _categoryName;
    readonly IExternalScopeProvider? _scopeProvider;
    readonly LogLevel _configLogLevel;


    public AppCenterLogger(string categoryName, LogLevel logLevel, IExternalScopeProvider? scopeProvider)
    {
        _categoryName = categoryName;
        _configLogLevel = logLevel;
        _scopeProvider = scopeProvider;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => _scopeProvider?.Push(state);

    public bool IsEnabled(LogLevel logLevel) => logLevel >= _configLogLevel;
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        var message = formatter(state, exception);
        var scopeVars = new Dictionary<string, string>
            {
                { "Message", message }
            };

        _scopeProvider?.ForEachScope((value, loggingProps) =>
        {
            if (value is string && !scopeVars.ContainsKey("Scope"))
            {
                scopeVars.Add("Scope", value.ToString()!);
            }
            else if (value is IEnumerable<KeyValuePair<string, object>> props)
            {
                foreach (var pair in props)
                {
                    if (!scopeVars.ContainsKey(pair.Key))
                        scopeVars.Add(pair.Key, pair.Value.ToString()!);
                }
            }
            else if (value is IEnumerable<KeyValuePair<string, string>> props2)
            {
                foreach (var pair in props2)
                {
                    if (!scopeVars.ContainsKey(pair.Key))
                        scopeVars.Add(pair.Key, pair.Value.ToString()!);
                }
            }
        }, state);

        if (logLevel <= LogLevel.Information || exception == null)
        {
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent(
                _categoryName,
                scopeVars
            );
        }
        else
        {
            Crashes.TrackError(
                exception,
                scopeVars
            );
        }
    }
}