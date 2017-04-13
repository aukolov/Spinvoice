using System;

namespace Spinvoice.Domain.Utils
{
    public static class ActionExtensions
    {
        public static void Raise(this Action action)
        {
            action?.Invoke();
        }
    }
}