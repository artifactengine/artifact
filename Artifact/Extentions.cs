using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Artifact
{
    public static class Extentions
    {
        public static float[] AsFloatArray(this Matrix4x4 matrix)
        {
            float[] floatArray = new float[16];
            floatArray[0] = matrix.M11;
            floatArray[1] = matrix.M12;
            floatArray[2] = matrix.M13;
            floatArray[3] = matrix.M14;
            floatArray[4] = matrix.M21;
            floatArray[5] = matrix.M22;
            floatArray[6] = matrix.M23;
            floatArray[7] = matrix.M24;
            floatArray[8] = matrix.M31;
            floatArray[9] = matrix.M32;
            floatArray[10] = matrix.M33;
            floatArray[11] = matrix.M34;
            floatArray[12] = matrix.M41;
            floatArray[13] = matrix.M42;
            floatArray[14] = matrix.M43;
            floatArray[15] = matrix.M44;

            return floatArray;
        }
    }
}
