using NLog;
using NLog.Config;
using NLog.Targets;

namespace Spinvoice.Services
{
    public class LogConfigurator
    {
        public void Configure()
        {
            var configuration = new LoggingConfiguration();
            var fileTarget = new FileTarget("File Target")
            {
                ArchiveEvery = FileArchivePeriod.Day,
                FileName = "${specialfolder:folder=MyDocuments}/Spinvoice/logs/log.txt",
            };
            configuration.AddTarget(fileTarget);

            configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, fileTarget));

            LogManager.Configuration = configuration;
            LogManager.EnableLogging();
            LogManager.ReconfigExistingLoggers();
        }
    }
}
