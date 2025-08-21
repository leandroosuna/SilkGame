using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkGame.Components.Internal.Collisions
{
    public struct BoundingSphere
    {
        public Vector3D<float> Center;
        public float Radius;
        public BoundingSphere(Vector3D<float> center, float radius)
        {
            Center = center;
            Radius = radius;
        }
    }
}
