using System;

namespace Spinvoice.App.Utils
{
    public static class ActionExtensions
    {
        public static void Raise(this Action action)
        {
            action?.Invoke();
        }
    }
}