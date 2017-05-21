namespace Spinvoice.Domain.UI
{
    public interface IWindowManager
    {
        void ShowWindow<T>(T viewModel);
        void Close(object viewModel);
        void Close(object viewModel, bool? dialogResult);
        bool? ShowDialog<T>(T viewModel);
    }
}