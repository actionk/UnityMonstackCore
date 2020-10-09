using System;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using UnityEngine;

namespace Plugins.UnityMonstackCore.Loggers
{
    public class UnityLogger
    {
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
                var time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                prefix.Append($"{(needsComma ? ", " : "")}{time}");
                needsComma = true;
            }

            prefix.Append("] ");
            return prefix.ToString();
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

        public static void Debug(string message)
        {
            if (LOGGING_LEVEL < LoggingLevel.DEBUG) return;
            Info(message);
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