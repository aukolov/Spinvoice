﻿using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Spinvoice.Application.Services
{
    public class ServerManager : IServerManager
    {
        private Process _process;

        public void Start()
        {
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
                KillProcess(_process);
            }
        }

        private static void KillProcess(Process process)
        {
            process.Kill();
            process.WaitForExit();
        }
    }
}