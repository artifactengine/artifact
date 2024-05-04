using Artifact.Plugins.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugins.Rendering
{
    public static class ImmediateRenderer
    {
        private static Dictionary<string, IVisual> textureLookup = new Dictionary<string, IVisual>();


        public static void Draw(string texture, Vector2 position, Vector2 size, ColorRGB color)
        {
            if (!textureLookup.ContainsKey(texture))
            {
                Vertex[] vertices = {
                    new Vertex(new Vector4(0.0f, 1.0f, 0.0f, 1.0f), new Vector2(0.0f, 0.0f)),
                    new Vertex(new Vector4(1.0f, 0.0f, 0.0f, 1.0f), new Vector2(1.0f, 1.0f)),
                    new Vertex(new Vector4(0.0f, 0.0f, 0.0f, 1.0f), new Vector2(0.0f, 1.0f)),
                    new Vertex(new Vector4(1.0f, 1.0f, 0.0f, 1.0f), new Vector2(1.0f, 0.0f)),
                };

                ushort[] indices = [
                    0, 1, 2,
                    0, 3, 1
                ];

                textureLookup.Add(texture, Application.current.GetPlugin<RenderingPlugin>().CreateVisual(new Mesh(vertices, indices, texture)));
            }

            IVisual visual = textureLookup[texture];

            Vector2 resolution = new Vector2(Application.current.GetPlugin<WindowingPlugin>().Width, Application.current.GetPlugin<WindowingPlugin>().Height);
            Vector3 resV3 = new Vector3(resolution, 1.0f);

            float scaleFactor = 1.0f / resolution.X;

            Vector3 vertexPos = new Vector3(position, -5);

            visual.Position = vertexPos;
            Console.WriteLine(visual.Position);
            visual.Scale = new Vector3((size.X * resolution.X / resolution.Y) / resolution.X, size.Y / resolution.Y, 1);
            //visual.Scale = new Vector3(0.2f, 0.2f, 0.2f);
            visual.Tint = color;

            visual.Draw();
        }
    }
}
