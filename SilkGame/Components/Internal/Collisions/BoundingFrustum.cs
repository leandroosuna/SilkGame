using Silk.NET.Assimp;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkGame.Components.Internal.Collisions
{
    public struct BoundingFrustum
    {
        private Matrix4X4<float> _matrix;
        private Plane<float>[] _planes;

        public Plane<float>[] Planes => _planes;

        public BoundingFrustum(Matrix4X4<float> matrix)
        {
            _matrix = matrix;
            _planes = new Plane<float>[6];
            ExtractPlanes(matrix, _planes);
        }

        public void Update(Matrix4X4<float> matrix)
        {
            _matrix = matrix;
            ExtractPlanes(matrix, _planes);
        }

        public bool Intersects(Box3D<float> box)
        {
            foreach (var plane in _planes)
            {
                if (BoxOutsidePlane(box, plane))
                    return false;
            }
            return true;
        }

        public bool Intersects(BoundingSphere sphere)
        {
            foreach (var plane in _planes)
            {
                var distance = PlaneHelper.DistanceToPoint(plane, sphere.Center);
                if (distance < -sphere.Radius)
                    return false;
            }
            return true;
        }

        public bool Intersects(Ray ray, out float distance)
        {
            distance = 0;
            bool hit = false;
            foreach (var plane in _planes)
            {
                if (RayIntersectsPlane(ray, plane, out var d))
                {
                    if (!hit || d < distance)
                    {
                        distance = d;
                        hit = true;
                    }
                }
            }
            return hit;
        }

        private static void ExtractPlanes(Matrix4X4<float> m, Plane<float>[] planes)
        {
            // Left
            planes[0] = PlaneHelper.Normalize(new Plane<float>(
                m.M14 + m.M11, m.M24 + m.M21, m.M34 + m.M31, m.M44 + m.M41));

            // Right
            planes[1] = PlaneHelper.Normalize(new Plane<float>(
                m.M14 - m.M11, m.M24 - m.M21, m.M34 - m.M31, m.M44 - m.M41));

            // Bottom
            planes[2] = PlaneHelper.Normalize(new Plane<float>(
                m.M14 + m.M12, m.M24 + m.M22, m.M34 + m.M32, m.M44 + m.M42));

            // Top
            planes[3] = PlaneHelper.Normalize(new Plane<float>(
                m.M14 - m.M12, m.M24 - m.M22, m.M34 - m.M32, m.M44 - m.M42));

            // Near
            planes[4] = PlaneHelper.Normalize(new Plane<float>(
                m.M13, m.M23, m.M33, m.M43));

            // Far
            planes[5] = PlaneHelper.Normalize(new Plane<float>(
                m.M14 - m.M13, m.M24 - m.M23, m.M34 - m.M33, m.M44 - m.M43));
        }

        private static bool BoxOutsidePlane(Box3D<float> box, Plane<float> plane)
        {
            var positiveVertex = new Vector3D<float>(
                plane.Normal.X >= 0 ? box.Max.X : box.Min.X,
                plane.Normal.Y >= 0 ? box.Max.Y : box.Min.Y,
                plane.Normal.Z >= 0 ? box.Max.Z : box.Min.Z
            );

            return PlaneHelper.DistanceToPoint(plane, positiveVertex) < 0;
        }

        private static bool RayIntersectsPlane(Ray ray, Plane<float> plane, out float distance)
        {
            var denom = Vector3D.Dot(plane.Normal, ray.Direction);
            if (denom == 0)
            {
                distance = 0;
                return false;
            }

            distance = -(Vector3D.Dot(plane.Normal, ray.Origin) + plane.Distance) / denom;
            return distance >= 0;
        }
    }
}
