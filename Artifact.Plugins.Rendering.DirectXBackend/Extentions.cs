using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugins.Rendering.DirectXBackend
{
    public static class Extentions
    {
        public static Matrix ToDXMatrix(this Matrix4x4 matrix)
        {
            Matrix mat = new Matrix(matrix.AsFloatArray());

            return mat;
        }

        public static SharpDX.Vector3 ToDXVec(this System.Numerics.Vector3 vector)
        {
            return new SharpDX.Vector3(vector.X, vector.Y, vector.Z);
        }
    }
}
