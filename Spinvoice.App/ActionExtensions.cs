using System;

namespace Spinvoice.App
{
    public static class ActionExtensions
    {
        public static void Raise(this Action action)
        {
            action?.Invoke();
        }
    }
}