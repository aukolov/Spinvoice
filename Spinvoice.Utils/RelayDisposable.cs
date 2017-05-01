using System;

namespace Spinvoice.Utils
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