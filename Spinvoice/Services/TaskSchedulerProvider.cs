using System.Threading.Tasks.Schedulers;

namespace Spinvoice.Services
{
    public class TaskSchedulerProvider : ITaskSchedulerProvider
    {
        public TaskSchedulerProvider()
        {
            PdfParseTaskScheduler = new OrderedTaskScheduler();
        }

        public OrderedTaskScheduler PdfParseTaskScheduler { get; }
    }
}