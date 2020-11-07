#region import

using System.Collections.Generic;

#endregion

namespace Plugins.Shared.UnityMonstackCore.Extensions.Collections
{
    public static class HashSetExtensions
    {
        public static string ToDebugString<T>(this HashSet<T> value)
        {
            var stringValues = new string[value.Count];
            var i = 0;
            foreach (var item in value) stringValues[i++] = item.ToString();

            return "[" + string.Join(", ", stringValues) + "]";
        }
    }
}