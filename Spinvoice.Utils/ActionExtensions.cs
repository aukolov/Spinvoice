using System;

namespace Spinvoice.Utils
{
    public static class ActionExtensions
    {
        public static void Raise(this Action action)
        {
            action?.Invoke();
        }
    }
}