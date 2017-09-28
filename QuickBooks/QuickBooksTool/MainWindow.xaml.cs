using System.ComponentModel;
using Autofac;
using IContainer = Autofac.IContainer;

namespace QuickBooksTool
{
    public partial class MainWindow
    {
        private readonly IContainer _container;

        public MainWindow()
        {
            InitializeComponent();

            _container = ToolBootstrapper.Init();
            DataContext = _container.Resolve<IMainViewModel>();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            _container.Dispose();
        }
    }
}
