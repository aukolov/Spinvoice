using System.Threading.Tasks.Schedulers;

namespace Spinvoice.Application.Services
{
    public interface ITaskSchedulerProvider
    {
        OrderedTaskScheduler PdfParseTaskScheduler { get; }
    }
}