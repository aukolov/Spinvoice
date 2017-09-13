using Spinvoice.Common.Domain;

namespace Spinvoice.Server
{
    public class ApplicationNameProvider : IApplicationNameProvider
    {
        public string Name => "SpinvoiceServer";
    }
}