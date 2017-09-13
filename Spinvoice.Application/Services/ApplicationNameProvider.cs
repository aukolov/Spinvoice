using Spinvoice.Common.Domain;

namespace Spinvoice.Application.Services
{
    public class ApplicationNameProvider : IApplicationNameProvider
    {
        public string Name => "Spinvoice";
    }
}