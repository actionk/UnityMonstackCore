#if UNITY_EDITOR

using System;
using UnityEditor;

namespace Plugins.Shared.UnityMonstackCore.Utils
{
    public static class EditorPrefsUtils
    {
        public static TEnum ReadEnum<TEnum>(string setting, TEnum defaultValue) where TEnum : struct, Enum
        {
            if (Enum.TryParse(EditorPrefs.GetString(setting, ""), out TEnum valueFromPrefs))
                return valueFromPrefs;

            return defaultValue;
        }
    }
}

#endif