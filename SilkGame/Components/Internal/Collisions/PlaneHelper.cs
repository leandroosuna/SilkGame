using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkGame.Components.Internal.Collisions
{
    public static class PlaneHelper
    {
        public static Plane<float> Normalize(Plane<float> plane)
        {
            var len = plane.Normal.Length;
            if (len == 0) return plane;

            var invLen = 1f / len;
            return new Plane<float>(
                plane.Normal * invLen,
                plane.Distance * invLen
            );
        }

        public static float DistanceToPoint(Plane<float> plane, Vector3D<float> point)
        {
            return Vector3D.Dot(plane.Normal, point) + plane.Distance;
        }
    }
}
