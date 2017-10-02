using System.ComponentModel;
using System.Windows.Input;

namespace Spinvoice.Application.ViewModels.Exchange
{
    public interface IExchangeRatesViewModel : INotifyPropertyChanged
    {
        LoadExchangeRatesViewModel LoadExchangeRatesViewModel { get; }
        ICommand CloseCommand { get; }
        CheckExchangeRatesViewModel CheckExchangeRatesViewModel { get; }
    }
}