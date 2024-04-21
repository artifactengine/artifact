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
        public Vector2 TexCoord;
    
        public Vertex(Vector4 position, Vector2 texCoord)
        {
            Position = position;
            TexCoord = texCoord;
        }

        public const uint SizeInBytes = 24;
    }
}
