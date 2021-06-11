using System;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Plugins.Shared.Netcode.Core.Time;
using Plugins.Shared.UnityMonstackCore.Utils;
using UnityEngine;

namespace Plugins.UnityMonstackCore.Loggers
{
    public class UnityLogger
    {
        public const string SETTING_GLOBAL_LOGGING_LEVEL = "Settings/Global/Logging/Level";
        public const string SETTING_GLOBAL_LOGGING_PREFIX = "Settings/Global/Logging/Prefix";

        public enum LoggingLevel
        {
            ERROR,
            WARNING,
            INFO,
            DEBUG
        }

        [Flags]
        public enum Prefixes
        {
            None = 0,
            Time = 1,
            TimeMillis = 2,
        }

        private static long MILLIS_FROM_START;
        private static readonly Regex PATTERN_REGEX = new Regex("\\{\\}");
        private static Prefixes PREFIX = Prefixes.None;
        private static LoggingLevel LOGGING_LEVEL = LoggingLevel.DEBUG;

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Reset()
        {
            PREFIX = 0;
        }
#endif

        public static void Initialize()
        {
#if UNITY_EDITOR
            var loggingLevel = EditorPrefsUtils.ReadEnum(SETTING_GLOBAL_LOGGING_LEVEL, LoggingLevel.INFO);
            SetLevel(loggingLevel);

            var loggingPrefix = EditorPrefsUtils.ReadEnum(SETTING_GLOBAL_LOGGING_PREFIX, Prefixes.None);
            SetPrefix(loggingPrefix);

            MILLIS_FROM_START = DateTimeOffset.Now.ToUnixTimeMilliseconds();
#endif
        }

        public static void SetLevel(LoggingLevel loggingLevel)
        {
            LOGGING_LEVEL = loggingLevel;
        }

        public static string GetPrefix()
        {
            if (PREFIX == Prefixes.None)
                return "";

            var prefix = new StringBuilder();
            prefix.Append("[");

            var needsComma = false;

            if ((PREFIX & Prefixes.Time) != Prefixes.None)
            {
                var time = DateTimeOffset.Now.LocalDateTime;
                prefix.Append($"{(needsComma ? ", " : "")}{time}");
                needsComma = true;
            }


            if ((PREFIX & Prefixes.TimeMillis) != Prefixes.None)
            {
                var time = DateTimeOffset.Now.ToUnixTimeMilliseconds() - MILLIS_FROM_START;
                prefix.Append($"{(needsComma ? ", " : "")}{time}");
                needsComma = true;
            }

            prefix.Append("] ");
            return prefix.ToString();
        }

        public static void Log(LoggingLevel level, string s)
        {
            switch (level)
            {
                case LoggingLevel.INFO:
                    Log(s);
                    return;

                case LoggingLevel.DEBUG:
                    Debug(s);
                    return;

                case LoggingLevel.ERROR:
                    Error(s);
                    return;
                case LoggingLevel.WARNING:
                    Warning(s);
                    return;
            }
        }

        public static void Log(string s)
        {
            if (LOGGING_LEVEL < LoggingLevel.INFO) return;
            Info(s);
        }

        public static void Log(string message, [NotNull] params object[] arguments)
        {
            if (LOGGING_LEVEL < LoggingLevel.INFO) return;
            Info(message, arguments);
        }

        public static void Log(string prefix, string message)
        {
            if (LOGGING_LEVEL < LoggingLevel.INFO) return;
            Info(prefix, message);
        }

        public static void Log(string prefix, string message, [NotNull] params object[] arguments)
        {
            if (LOGGING_LEVEL < LoggingLevel.INFO) return;
            Info(prefix, message, arguments);
        }

        private static string GetGameTimeMessage()
        {
            return $"GAME_TIME:{NetworkGameTime.ElapsedMilliseconds} ";
        }

        public static void Debug(string message, bool showGameTime = false)
        {
            if (LOGGING_LEVEL < LoggingLevel.DEBUG) return;
            Info((showGameTime ? GetGameTimeMessage() : "") + message);
        }

        public static void Debug(string message, [NotNull] params object[] arguments)
        {
            if (LOGGING_LEVEL < LoggingLevel.DEBUG) return;
            Info(message, arguments);
        }

        public static void Debug(string prefix, string message)
        {
            if (LOGGING_LEVEL < LoggingLevel.DEBUG) return;
            Info(prefix, message);
        }

        public static void Debug(string prefix, string message, [NotNull] params object[] arguments)
        {
            if (LOGGING_LEVEL < LoggingLevel.DEBUG) return;
            Info(prefix, message, arguments);
        }

        public static void Info(string message)
        {
            if (LOGGING_LEVEL < LoggingLevel.INFO) return;
            var logMessage = GetPrefix() + message;
            UnityEngine.Debug.Log(logMessage);
        }

        public static void Info(string message, [NotNull] params object[] arguments)
        {
            if (LOGGING_LEVEL < LoggingLevel.INFO) return;
            var logMessage = GetPrefix() + Format(message, arguments);
            UnityEngine.Debug.Log(logMessage);
        }

        public static void Info(string prefix, string message)
        {
            if (LOGGING_LEVEL < LoggingLevel.INFO) return;
            var logMessage = GetPrefix() + "[" + prefix + "] " + message;
            UnityEngine.Debug.Log(logMessage);
        }

        public static void Info(string prefix, string message, [NotNull] params object[] arguments)
        {
            if (LOGGING_LEVEL < LoggingLevel.INFO) return;
            var logMessage = GetPrefix() + "[" + prefix + "] " + Format(message, arguments);
            UnityEngine.Debug.Log(logMessage);
        }

        public static void Warning(string message)
        {
            if (LOGGING_LEVEL < LoggingLevel.WARNING) return;
            var logMessage = GetPrefix() + message;
            UnityEngine.Debug.LogWarning(logMessage);
        }

        public static void Warning(string message, [NotNull] params object[] arguments)
        {
            if (LOGGING_LEVEL < LoggingLevel.WARNING) return;
            var logMessage = GetPrefix() + Format(message, arguments);
            UnityEngine.Debug.LogWarning(logMessage);
        }

        public static void Warning(string message, Exception ex)
        {
            if (LOGGING_LEVEL < LoggingLevel.WARNING) return;
            var logMessage = GetPrefix() + message + ", Stacktrace: " + ex;
            UnityEngine.Debug.LogWarning(logMessage);
        }

        public static void Warning(string prefix, string message, [NotNull] params object[] arguments)
        {
            if (LOGGING_LEVEL < LoggingLevel.INFO) return;
            var logMessage = GetPrefix() + "[" + prefix + "] " + Format(message, arguments);
            UnityEngine.Debug.LogWarning(logMessage);
        }

        public static void Error(string message)
        {
            if (LOGGING_LEVEL < LoggingLevel.ERROR) return;
            var logMessage = GetPrefix() + message;
            UnityEngine.Debug.LogError(logMessage);
        }

        public static void Error(string message, [NotNull] params object[] arguments)
        {
            if (LOGGING_LEVEL < LoggingLevel.ERROR) return;
            var logMessage = GetPrefix() + Format(message, arguments);
            UnityEngine.Debug.LogError(logMessage);
        }

        public static void Error(string prefix, string message, [NotNull] params object[] arguments)
        {
            if (LOGGING_LEVEL < LoggingLevel.INFO) return;
            var logMessage = GetPrefix() + "[" + prefix + "] " + Format(message, arguments);
            UnityEngine.Debug.LogError(logMessage);
        }

        public static string Format(string message, params object[] arguments)
        {
            var occurence = 0;
            message = PATTERN_REGEX.Replace(message, match =>
            {
                if (occurence > arguments.Length) return match.Value;

                return arguments[occurence++].ToString();
            });

            var logMessage = message;
            foreach (var argument in arguments)
                if (argument is Exception exception)
                    logMessage += "\nOrigin exception:" + exception;

            return logMessage;
        }

        public static void SetPrefix(Prefixes prefix)
        {
            PREFIX = prefix;
        }
    }
}