namespace Spinvoice.ViewModels
{
    public interface IFileSystemViewModel
    {
        string Name { get; }
        string Path { get; }
        bool IsSelected { get; set; }
    }
}