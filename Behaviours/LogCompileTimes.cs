using System;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Plugins.UnityMonstackCore.Behaviours
{
    // Logs the time taken to perform script compilations and domain reloads.
    public sealed class LogCompileTimes : EditorSingleton<LogCompileTimes>
    {
        [InitializeOnLoadMethod]
        static void OnLoad() => Initialize();

        void OnEnable()
        {
            // Register for script compilation events.
            CompilationPipeline.compilationStarted += OnCompilationStarted;
            CompilationPipeline.compilationFinished += OnCompilationFinished;

            // Register for domain reload events.
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        void OnDisable()
        {
            // Unregister for script compilation events.
            CompilationPipeline.compilationStarted -= OnCompilationStarted;
            CompilationPipeline.compilationFinished -= OnCompilationFinished;

            // Unregister for domain reload events.
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
        }

        void OnCompilationStarted(object value) =>
            _compilationStart = DateTime.Now.Ticks;

        void OnCompilationFinished(object value) =>
            _compilationTime = DateTime.Now.Ticks - _compilationStart;

        void OnBeforeAssemblyReload() =>
            _reloadStart = DateTime.Now.Ticks;

        void OnAfterAssemblyReload()
        {
            // Return if the assembly was reloaded before timers were started.
            if (_compilationTime == 0 || _reloadStart == 0) return;

            var compilation = new TimeSpan(_compilationTime);
            var reload = new TimeSpan(DateTime.Now.Ticks - _reloadStart);
            Debug.Log($"Script compilation: {compilation.TotalSeconds:F3}s, " +
                      $"Domain reload: {reload.TotalSeconds:F3}s, " +
                      $"Total: {(compilation + reload).TotalSeconds:F3}s ");
            _compilationTime = 0;
        }

        // The time (in ticks) when the script compilation started.
        long _compilationStart;

        // The total time (in ticks) taken for script compilation.
        long _compilationTime;

        // The time (in ticks) when the domain reload started.
        long _reloadStart;
    }
}