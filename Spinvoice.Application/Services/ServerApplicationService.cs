using System.Diagnostics;

namespace Spinvoice.Application.Services
{
    public class ServerApplicationService : IServerApplicationService
    {
        private Process _process;

        public void Start()
        {
            _process = new Process
            {
                StartInfo = new ProcessStartInfo("Spinvoice.Server.exe")
                {
                    CreateNoWindow  = true
                }
            };
            _process.Start();
        }

        public void Dispose()
        {
            _process.Kill();
        }
    }
}