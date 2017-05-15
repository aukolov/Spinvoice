namespace Spinvoice.Domain.UI
{
    public interface IWindowManager
    {
        void ShowWindow<T>(T viewModel);
        void Close(object viewModel);
    }
}