using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NLog;
using Spinvoice.Utils;

namespace Spinvoice.Application.Services
{
    public class ServerManager : IServerManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private Process _process;

        public void Start()
        {
            Process.GetProcessesByName("Spinvoice.Server").ForEach(TryKillProcess);

            var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var processStartInfo = new ProcessStartInfo("Spinvoice.Server.exe")
            {
                CreateNoWindow = true,
                WorkingDirectory = directoryName ?? ""
            };
            _process = Process.Start(processStartInfo);
            _process?.WaitForInputIdle();
        }

        public void Dispose()
        {
            if (_process != null)
            {
                TryKillProcess(_process);
            }
        }

        private static void TryKillProcess(Process process)
        {
            try
            {
                process.Kill();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                Logger.Error(e, "Unable to kill process.");
            }
        }
    }
}