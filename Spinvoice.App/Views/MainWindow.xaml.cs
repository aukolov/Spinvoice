using Spinvoice.App.ViewModels;
using Spinvoice.Domain.Company;
using Spinvoice.Infrastructure.DataAccess;

namespace Spinvoice.App.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var companyRepository = new CompanyRepository(new CompanyDataAccess());
            DataContext = new AppViewModel(companyRepository);
        }
    }
}
