#region import

using System.Collections.Generic;

#endregion

namespace Plugins.UnityMonstackCore.Extensions.Collections
{
    public static class ListExtensions
    {
        public static string ToDebugString<T>(this List<T> value)
        {
            var stringValues = new string[value.Count];
            for (var i = 0; i < value.Count; i++) stringValues[i] = value[i].ToString();

            return "[" + string.Join(", ", stringValues) + "]";
        }

        public static bool IsEmpty<T>(this List<T> value)
        {
            return value.Count == 0;
        }
    }
}