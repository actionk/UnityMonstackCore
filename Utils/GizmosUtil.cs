using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Plugins.Shared.UnityMonstackCore.Utils
{
    public class GizmosUtil : MonoBehaviour
    {
        private class ActionWithdurationInSeconds
        {
            public Action action;
            public float startTime;
            public float durationInSecondsInSeconds;
        }

        private static readonly List<ActionWithdurationInSeconds> ACTIONS = new List<ActionWithdurationInSeconds>();

        void OnDrawGizmos()
        {
            if (ACTIONS == null || ACTIONS.Count == 0)
                return;

            for (var i = 0; i < ACTIONS.Count; i++)
            {
                var action = ACTIONS[i];
                action.action.Invoke();

                if (Time.time >= action.startTime + action.durationInSecondsInSeconds)
                    ACTIONS.RemoveAt(i--);
            }
        }

        private void OnDestroy()
        {
            ACTIONS.Clear();
        }

        public static void AddCustomAction(Action action, float durationInSecondsInSeconds = 0)
        {
#if UNITY_EDITOR
            ACTIONS.Add(new ActionWithdurationInSeconds
            {
                action = action,
                startTime = Time.time,
                durationInSecondsInSeconds = durationInSecondsInSeconds
            });
#endif
        }

        public static void DrawSphere(Vector3 center, float radius, Color color, float durationInSecondsInSeconds = 0)
        {
#if UNITY_EDITOR
            AddCustomAction(() =>
            {
                Gizmos.color = color;
                Gizmos.DrawSphere(center, radius);
            }, durationInSecondsInSeconds);
#endif
        }

        public static void DrawWireSphere(Vector3 center, float radius, Color color, Matrix4x4 matrix, float durationInSeconds = 0)
        {
#if UNITY_EDITOR

            AddCustomAction(() =>
            {
                Gizmos.matrix = matrix;
                Gizmos.color = color;
                Gizmos.DrawWireSphere(center, radius);
            }, durationInSeconds);
#endif
        }

        public static void DrawDisk(Vector3 center, float radius, Color color, float durationInSeconds = 0)
        {
#if UNITY_EDITOR
            AddCustomAction(() =>
            {
                Matrix4x4 oldMatrix = Gizmos.matrix;
                Gizmos.color = color;
                Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.identity, new Vector3(1f, 0.01f, 1f));
                Gizmos.DrawSphere(Vector3.zero, radius);
                Gizmos.matrix = oldMatrix;
            }, durationInSeconds);
#endif
        }

        public static void DrawRectangle(int2x2 bounds, Color color, float height = 1f, float durationInSeconds = 0)
        {
#if UNITY_EDITOR
            AddCustomAction(() =>
            {
                Gizmos.color = color;
                var a = new Vector3(bounds.c0.x, height, bounds.c0.y);
                var b = new Vector3(bounds.c1.x, height, bounds.c0.y);
                var c = new Vector3(bounds.c1.x, height, bounds.c1.y);
                var d = new Vector3(bounds.c0.x, height, bounds.c1.y);

                Gizmos.DrawLine(a, b);
                Gizmos.DrawLine(b, c);
                Gizmos.DrawLine(c, d);
                Gizmos.DrawLine(d, a);
            }, durationInSeconds);
#endif
        }

        public static void DrawBounds(Bounds bounds, Color color, float durationInSeconds = 0)
        {
#if UNITY_EDITOR
            AddCustomAction(() =>
            {
                Gizmos.color = color;
                Gizmos.DrawCube(bounds.center, bounds.size);
            }, durationInSeconds);
#endif
        }

        public static void DrawLine(Vector3 from, Vector3 to, Color color, float durationInSeconds = 0)
        {
#if UNITY_EDITOR
            AddCustomAction(() =>
            {
                Gizmos.color = color;
                Gizmos.DrawLine(from, to);
            }, durationInSeconds);
#endif
        }

        public static void DrawLines(Vector3[] points, Color color, float durationInSeconds = 0)
        {
#if UNITY_EDITOR
            AddCustomAction(() =>
            {
                Gizmos.color = color;
                for (var i = 1; i < points.Length; i++)
                    Gizmos.DrawLine(points[i - 1], points[i]);
            }, durationInSeconds);
#endif
        }
    }
}