using System.Threading.Tasks.Schedulers;

namespace Spinvoice.Services
{
    public interface ITaskSchedulerProvider
    {
        OrderedTaskScheduler PdfParseTaskScheduler { get; }
    }
}