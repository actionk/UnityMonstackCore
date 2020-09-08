#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Plugins.UnityMonstackCore.Behaviours
{
    [InitializeOnLoad]
    class CompilationTimeBehaviour
    {
        private static double m_assembleStartTime;

        [InitializeOnLoadMethod]
        public static void CreateListener()
        {
            EditorApplication.playModeStateChanged += OnModeChanged;
        }

        static CompilationTimeBehaviour()
        {
            CompilationPipeline.compilationStarted += CompilationStarted;
            CompilationPipeline.compilationFinished += CompilationFinished;
            CompilationPipeline.assemblyCompilationStarted += AssemblyCompilationStarted;
            CompilationPipeline.assemblyCompilationFinished += AssemblyCompilationFinished;
            EditorApplication.update += EditorUpdate;
        }

        private static void EditorUpdate()
        {
            if (!EditorApplication.isCompiling)
            {
                float startTime = PlayerPrefs.GetFloat("StartTime", 0);
                if (startTime > 0)
                {
                    double finishTime = EditorApplication.timeSinceStartup;
                    double compilationTime = finishTime - startTime;
                    PlayerPrefs.DeleteKey("StartTime");
                    Debug.Log("Full compilation process finished: " + compilationTime.ToString("F2"));
                }
            }
        }

        private static void OnModeChanged(PlayModeStateChange state)
        {
            float time = (float) EditorApplication.timeSinceStartup;
            float modeSwitchTimer = PlayerPrefs.GetFloat("tscm", 0);
            //Debug.Log("STATE CHANGED to = " + state + " for " + (time - modeSwitchTimer) + " seconds");
            PlayerPrefs.SetFloat("tscm", time);
        }

        private static void CompilationStarted(object obj)
        {
            Debug.Log("Compilation started...");
            PlayerPrefs.SetFloat("StartTime", (float) EditorApplication.timeSinceStartup);
        }

        private static void CompilationFinished(object obj)
        {
            float startTime = PlayerPrefs.GetFloat("StartTime", 0);
            double finishTime = EditorApplication.timeSinceStartup;
            double compilationTime = finishTime - startTime;
            Debug.Log("Compilation finished: " + compilationTime.ToString("F2"));
        }

        private static void AssemblyCompilationStarted(string message)
        {
            m_assembleStartTime = EditorApplication.timeSinceStartup;
        }

        private static void AssemblyCompilationFinished(string message, CompilerMessage[] signatures)
        {
            double finishTime = EditorApplication.timeSinceStartup;
            double compilationTime = finishTime - m_assembleStartTime;
            Debug.Log($"Assemble compilation finished: {compilationTime:F2} \n {message}");
        }
    }
}
#endif