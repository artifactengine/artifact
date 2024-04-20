using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugins.Rendering
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public Vector4 Position;
        public Vector4 Color;
    
        public Vertex(Vector4 position, Vector4 color)
        {
            Position = position;
            Color = color;
        }

        public const uint SizeInBytes = 32;
    }
}
