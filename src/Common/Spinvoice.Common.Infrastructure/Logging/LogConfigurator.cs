using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using Spinvoice.Common.Domain;

namespace Spinvoice.Common.Infrastructure.Logging
{
    public class LogConfigurator : ILogConfigurator
    {
        private readonly IApplicationNameProvider _applicationNameProvider;

        public LogConfigurator(IApplicationNameProvider applicationNameProvider)
        {
            _applicationNameProvider = applicationNameProvider;
        }

        public void Configure()
        {
            var configuration = new LoggingConfiguration();
            var fileTarget = new FileTarget("File Target")
            {
                ArchiveEvery = FileArchivePeriod.Day,
                FileName = "${specialfolder:folder=MyDocuments}/" + _applicationNameProvider.Name + "/logs/log.txt",
                Layout = new SimpleLayout(
                    "${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=tostring}")
            };
            configuration.AddTarget(fileTarget);

            configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, fileTarget));

            LogManager.Configuration = configuration;
            LogManager.EnableLogging();
            LogManager.ReconfigExistingLoggers();
        }
    }
}
