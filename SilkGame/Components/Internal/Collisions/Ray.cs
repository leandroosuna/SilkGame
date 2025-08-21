using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkGame.Components.Internal.Collisions
{
    public struct Ray
    {
        public Vector3D<float> Origin;
        public Vector3D<float> Direction;
        public Ray(Vector3D<float> origin, Vector3D<float> direction)
        {
            Origin = origin;
            Direction = Vector3D.Normalize(direction);
        }
    }
}
