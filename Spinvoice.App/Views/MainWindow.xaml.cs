using Spinvoice.App.ViewModels;

namespace Spinvoice.App.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new AppViewModel();
        }
    }
}
