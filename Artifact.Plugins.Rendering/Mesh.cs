using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugins.Rendering
{
    public class Mesh
    {
        public Vertex[] Vertices { get; set; }
        public ushort[] Indices { get; set; }

        public Mesh(Vertex[] vertices, ushort[] indices)
        {
            Vertices = vertices;
            Indices = indices;
        }
    }
}
