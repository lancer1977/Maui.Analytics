namespace Maui.Analytics.AppCenter
{
    public static class RegistrationExtensions
    {
        /// <summary>
        /// Adds AppCenter to the logging provider
        /// </summary>
        /// <param name="builder">The logging builder</param>
        /// <param name="appCenterSecret">Your appcenter secret key for any/all platforms you use.  If you don't set this value, it is assumed you will initialize AppCenter externally</param>
        /// <param name="minimumLogLevel">The minimum loglevel you wish to use - defaults to warning</param>
        /// <param name="enableVerboseInternalLogging">Enable internal verbose logging with AppCenter</param>
        /// <param name="additionalAppCenterPackages">Additional appcenter types to initialize</param>
        public static ILoggingBuilder AddAppCenter(this ILoggingBuilder builder, string? appCenterSecret = null, LogLevel minimumLogLevel = LogLevel.Warning, bool enableVerboseInternalLogging = false, params Type[] additionalAppCenterPackages
        )
        {
            builder.AddProvider(new AppCenterLoggerProvider(minimumLogLevel));
            if (string.IsNullOrWhiteSpace(appCenterSecret)) return builder;
            var list = new List<Type> { typeof(Crashes), typeof(Microsoft.AppCenter.Analytics.Analytics) };
            if (additionalAppCenterPackages.Length > 0)
                list.AddRange(additionalAppCenterPackages);

            if (Microsoft.AppCenter.AppCenter.Configured) return builder;
            if (enableVerboseInternalLogging)
                Microsoft.AppCenter.AppCenter.LogLevel = Microsoft.AppCenter.LogLevel.Verbose;

            Microsoft.AppCenter.AppCenter.Start(appCenterSecret, list.ToArray());
            return builder;
        }
    }
}