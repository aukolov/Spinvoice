namespace Spinvoice.Common.Presentation
{
    public interface IWindowManager
    {
        void ShowWindow<T>(T viewModel);
        void Close(object viewModel);
        void CloseDialog(object viewModel, bool? dialogResult);
        bool? ShowDialog<T>(T viewModel);
    }
}