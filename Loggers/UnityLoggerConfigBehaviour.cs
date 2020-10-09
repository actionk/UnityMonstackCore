using System;
using UnityEngine;

namespace Plugins.UnityMonstackCore.Loggers
{
    public class UnityLoggerConfigBehaviour : MonoBehaviour
    {
        public UnityLogger.Prefixes prefix;
        public UnityLogger.LoggingLevel level;

        private void Awake()
        {
            UnityLogger.SetLevel(level);
            UnityLogger.SetPrefix(prefix);
        }
    }
}