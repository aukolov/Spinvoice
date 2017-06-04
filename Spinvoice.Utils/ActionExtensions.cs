using System;

namespace Spinvoice.Utils
{
    public static class ActionExtensions
    {
        public static void Raise(this Action action)
        {
            action?.Invoke();
        }


        public static void Raise<T>(this Action<T> action, T param)
        {
            action?.Invoke(param);
        }
    }
}