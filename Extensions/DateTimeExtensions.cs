using System;

namespace Plugins.Shared.UnityMonstackCore.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ToDateTime(this ulong unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}