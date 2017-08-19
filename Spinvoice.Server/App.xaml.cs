using System;
using System.ServiceModel;
using System.Windows;
using Spinvoice.Server.Services;

namespace Spinvoice.Server
{
    public partial class App
    {
        private readonly ServiceHost _serviceHost;

        public App()
        {
            Exit += OnExit;


            try
            {
                _serviceHost = new ServiceHost(typeof(FileParseService));
                _serviceHost.AddServiceEndpoint(
                    typeof(IFileParseService), 
                    new NetNamedPipeBinding(),
                    "net.pipe://localhost/Spinvoice.Server.Services/FileParseService");
                _serviceHost.Open();
            }
            catch (Exception e)
            {
                _serviceHost?.Abort();
                MessageBox.Show($"Unable to start service: {e.Message}");
            }
        }

        private void OnExit(object sender, ExitEventArgs exitEventArgs)
        {
            _serviceHost?.Close();
            ((IDisposable)_serviceHost)?.Dispose();
        }
    }
}
