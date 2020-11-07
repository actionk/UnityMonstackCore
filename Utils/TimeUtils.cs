using System;

namespace Plugins.Shared.UnityMonstackCore.Utils
{
    public static class TimeUtils
    {
        public static uint Unixtime => (uint) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    }
}