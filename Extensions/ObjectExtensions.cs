﻿using System;

namespace Plugins.UnityMonstackCore.Extensions
{
    public static class ObjectExtensions
    {
        public static void IfNotNull<T>(this T o, Action<T> evaluator) where T : class
        {
            if (o != null) evaluator(o);
        }
    }
}