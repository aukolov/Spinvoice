using System;

namespace Spinvoice.Domain
{
    public class RelayDisposable : IDisposable
    {
        private readonly Action _action;

        public RelayDisposable(Action action)
        {
            _action = action;
        }

        public void Dispose()
        {
            _action?.Invoke();
        }
    }
}