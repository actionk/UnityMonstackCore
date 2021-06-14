using System;
using UnityEngine;

namespace Plugins.Shared.UnityMonstackCore.Utils
{
    public class AimingUtilities
    {

        public static bool InterceptionDirection(Vector2 a, Vector2 b, Vector2 vA, float sB, out Vector2 resultDirection)
        {
            var aToB = b - a;
            var dC = aToB.magnitude;
            var alpha = Vector2.Angle(aToB, vA) * Mathf.Deg2Rad;
            var sA = vA.magnitude;
            var r = sA / sB;
            if (SolveQuadratic(1 - r * r, 2 * r * dC * Mathf.Cos(alpha), -(dC * dC), out var root1, out var root2) == 0)
            {
                resultDirection = Vector2.zero;
                return false;
            }

            var dA = Math.Max(root1, root2);
            var t = dA / sB;
            var c = a + vA * t;
            resultDirection = (c - b).normalized;
            return true;
        }
        
        public static int SolveQuadratic(float a, float b, float c, out float root1, out float root2)
        {
            var discriminant = b * b - 4 * a * c;
            if (discriminant < 0)
            {
                root1 = Mathf.Infinity;
                root2 = -root1;
                return 0;
            }

            root1 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
            root2 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);
            return discriminant > 0 ? 2 : 1;
        }
    }
}