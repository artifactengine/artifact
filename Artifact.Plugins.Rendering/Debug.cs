using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugins.Rendering
{
    public class Debug
    {
        public static Debug current;

        private Dictionary<char, IVisual> charVisuals = new Dictionary<char, IVisual>();
        private IVisual character;

        private RenderingPlugin renderer;

        public Debug()
        {
            current = this;
            

            renderer = Application.current.GetPlugin<RenderingPlugin>();

            char c = (char)0;

            using Image fontTexture = Image.FromFile("Assets/debugFont.bmp");

            Dictionary<char, Vector2> charLocations = new Dictionary<char, Vector2>()
            {
                { 'A', new Vector2(2, 5) },
                { 'B', new Vector2(3, 5) },
                { 'C', new Vector2(4, 5) },
                { 'D', new Vector2(5, 5) },
                { 'E', new Vector2(6, 5) },
                { 'F', new Vector2(7, 5) },
                { 'G', new Vector2(8, 5) },
                { 'H', new Vector2(9, 5) },
                { 'I', new Vector2(10, 5) },
                { 'J', new Vector2(11, 5) },
                { 'K', new Vector2(12, 5) },
                { 'L', new Vector2(13, 5) },
                { 'M', new Vector2(14, 5) },
                { 'N', new Vector2(15, 5) },
                { 'O', new Vector2(16, 5) },
                { 'P', new Vector2(1, 6) },
                { 'Q', new Vector2(2, 6) },
                { 'R', new Vector2(3, 6) },
                { 'S', new Vector2(4, 6) },
                { 'T', new Vector2(5, 6) },
                { 'U', new Vector2(6, 6) },
                { 'V', new Vector2(7, 6) },
                { 'W', new Vector2(8, 6) },
                { 'X', new Vector2(9, 6) },
                { 'Y', new Vector2(10, 6) },
                { 'Z', new Vector2(11, 6) },

                { '[', new Vector2(12, 6) },
                { '\\', new Vector2(13, 6) },
                { ']', new Vector2(14, 6) },
                { '_', new Vector2(16, 6) },

                { 'a', new Vector2(2, 7) },
                { 'b', new Vector2(3, 7) },
                { 'c', new Vector2(4, 7) },
                { 'd', new Vector2(5, 7) },
                { 'e', new Vector2(6, 7) },
                { 'f', new Vector2(7, 7) },
                { 'g', new Vector2(8, 7) },
                { 'h', new Vector2(9, 7) },
                { 'i', new Vector2(10, 7) },
                { 'j', new Vector2(11, 7) },
                { 'k', new Vector2(12, 7) },
                { 'l', new Vector2(13, 7) },
                { 'm', new Vector2(14, 7) },
                { 'n', new Vector2(15, 7) },
                { 'o', new Vector2(16, 7) },
                { 'p', new Vector2(1, 8) },
                { 'q', new Vector2(2, 8) },
                { 'r', new Vector2(3, 8) },
                { 's', new Vector2(4, 8) },
                { 't', new Vector2(5, 8) },
                { 'u', new Vector2(6, 8) },
                { 'v', new Vector2(7, 8) },
                { 'w', new Vector2(8, 8) },
                { 'x', new Vector2(9, 8) },
                { 'y', new Vector2(10, 8) },
                { 'z', new Vector2(11, 8) },

                { '0', new Vector2(1, 4) },
                { '1', new Vector2(2, 4) },
                { '2', new Vector2(3, 4) },
                { '3', new Vector2(4, 4) },
                { '4', new Vector2(5, 4) },
                { '5', new Vector2(6, 4) },
                { '6', new Vector2(7, 4) },
                { '7', new Vector2(8, 4) },
                { '8', new Vector2(9, 4) },
                { '9', new Vector2(10, 4) },

                { '!', new Vector2(2, 3) },
                { '\"', new Vector2(3, 3) },
                { '#', new Vector2(4, 3) },
                { '$', new Vector2(5, 3) },
                { '%', new Vector2(6, 3) },
                { '^', new Vector2(7, 3) },
                { '&', new Vector2(8, 3) },
                { '\'', new Vector2(9, 3) },
                { '(', new Vector2(10, 3) },
                { ')', new Vector2(11, 3) },
                { '*', new Vector2(12, 3) },
                { '+', new Vector2(13, 3) },
                { ',', new Vector2(14, 3) },
                { '-', new Vector2(14, 3) },
                { '.', new Vector2(15, 3) },
                { '/', new Vector2(16, 3) },
                { ':', new Vector2(11, 4) },
                { ';', new Vector2(12, 4) },
                { '<', new Vector2(13, 4) },
                { '=', new Vector2(14, 4) },
                { '>', new Vector2(15, 4) },
                { '?', new Vector2(16, 4) },
            };

            for (int i = 0; i < 256; i++)
            {
                float width = 0.01f;
                float height = 0.015f;

                Vertex[] vertices;

                if (!charLocations.ContainsKey((char)i))
                {
                    continue;
                }

                float atlasX = charLocations[(char)i].X;
                float atlasY = charLocations[(char)i].Y;

                vertices = [
                    new Vertex(new Vector4(-width, -height, -5, 1), new Vector2(0.0625f * (atlasX - 1), 0.0625f * atlasY)),
                    new Vertex(new Vector4(width, height, -5, 1), new Vector2(0.0625f * atlasX, 0.0625f * (atlasY - 1))),
                    new Vertex(new Vector4(width, -height, -5, 1), new Vector2(0.0625f * atlasX, 0.0625f * atlasY)),
                    new Vertex(new Vector4(-width, height, -5, 1), new Vector2(0.0625f * (atlasX - 1), 0.0625f * (atlasY - 1))),
                ];

                ushort[] indices = [
                    0, 1, 2,
                    0, 3, 1
                ];

                Mesh mesh = new Mesh(vertices, indices, "Assets/debugFont.bmp", "font", "font");

                charVisuals.Add((char)i, renderer.CreateVisual(mesh));

                c++;
            }

            

        }

        public void DrawChar(Vector3 position, char c, ColorRGB color, Vector2 scale)
        {
            charVisuals[c].Position = position;
            charVisuals[c].Tint = color;
            charVisuals[c].Scale = new Vector3(scale, 1);
            charVisuals[c].Draw();
        }

        public void DrawString(Vector3 position, string s, Vector2 size)
        {
            Vector3 currentPos = position;
            foreach (char c in s)
            {
                if (c != ' ')
                {
                    DrawChar(currentPos, c, new ColorRGB(255, 255, 255, 255), size);
                }
                currentPos += new Vector3((0.0625f / 4f) + 0.0055f, 0, 0);
            }
        }

        public void DrawString(Vector3 position, string s, ColorRGB color, Vector2 size)
        {
            Vector3 currentPos = position;
            foreach (char c in s)
            {
                if (c != ' ')
                {
                    DrawChar(currentPos, c, color, size);
                }
                currentPos += new Vector3((0.0625f / 4f) + 0.0055f, 0, 0);
            }
        }

        public void DrawStringShadowed(Vector3 position, string s, Vector2 size)
        {
            DrawString(position - new Vector3(0.004f, 0.004f, 0), s, new ColorRGB(43, 43, 43, 255), size);
            DrawString(position, s, size);
        }
    }
}
