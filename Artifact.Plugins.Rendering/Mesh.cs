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

        public string TexturePath { get; set; }

        public string VertexShaderPath { get; set; }
        public string FragmentShaderPath { get; set; }

        public Mesh(Vertex[] vertices, ushort[] indices, string texturePath = "NONE", string vertexShaderPath = "default", string fragmentShaderPath = "default")
        {
            Vertices = vertices;
            Indices = indices;

            TexturePath = texturePath;

            VertexShaderPath = vertexShaderPath;
            FragmentShaderPath = fragmentShaderPath;
        }
    }
}
