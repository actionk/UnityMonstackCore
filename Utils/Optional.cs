using System;
using System.Collections;
using System.Collections.Generic;

namespace Plugins.UnityMonstackCore.Utils
{
    public class Optional<T> : IEnumerable<T>
    {
        private readonly T[] data;

        private Optional(T[] data)
        {
            this.data = data;
        }

        public bool IsEmpty => data.Length == 0;
        public bool IsPresent => data.Length > 0;
        public T Value => GetValue();

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>) data).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static Optional<T> OfValue(T element)
        {
            return new Optional<T>(new[] {element});
        }

        public static Optional<T> Empty()
        {
            return new Optional<T>(new T[0]);
        }

        public T GetValue()
        {
            return data[0];
        }

        public TValue IfPresent<TValue>(Func<T, TValue> action)
        {
            return action.Invoke(Value);
        }
    }
}