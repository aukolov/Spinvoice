using System.ComponentModel;
using System.Windows.Input;

namespace Spinvoice.ViewModels.Exchange
{
    public interface IExchangeRatesViewModel : INotifyPropertyChanged
    {
        LoadExchangeRatesViewModel LoadExchangeRatesViewModel { get; }
        ICommand CloseCommand { get; }
        CheckExchangeRatesViewModel CheckExchangeRatesViewModel { get; }
    }
}