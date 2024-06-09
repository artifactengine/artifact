using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Artifact.Plugins.Windowing;
using FreeTypeSharp;

namespace Artifact.Plugins.Rendering.Font
{
    public class FontRenderer
    {
        private Dictionary<char, IVisual> characterVisuals = new Dictionary<char, IVisual>();
        private Dictionary<char, float> characterSizes = new Dictionary<char, float>();
        private Dictionary<char, float> characterHeightOffsets = new Dictionary<char, float>();
        private RenderingPlugin renderer;

        private uint size;

        public FontRenderer(string fontPath, uint size)
        {
            this.size = size;
            Console.WriteLine("Creating bitmaps???");
            renderer = Application.current.GetPlugin<RenderingPlugin>();
            // Create font bitmaps
            unsafe
            {
                var lib = new FreeTypeLibrary();

                FT_FaceRec_* face;

                FT.FT_New_Face(lib.Native, (byte*)Marshal.StringToHGlobalAnsi(fontPath), 0, &face);

                FT.FT_Set_Pixel_Sizes(face, 0, size);

                if (!Directory.Exists("Assets/FontGlyphs"))
                {
                    Directory.CreateDirectory("Assets/FontGlyphs");
                }

                if (!Directory.Exists("Assets/FontGlyphs/" + *face->family_name))
                {
                    Directory.CreateDirectory("Assets/FontGlyphs/" + *face->family_name);
                    for (char c = (char)0; c < 127; c++)
                    {
                        var glyphIndex = FT.FT_Get_Char_Index(face, c);
                        var error = FT.FT_Load_Glyph(face, glyphIndex, FT_LOAD.FT_LOAD_RENDER);
                        
                        if (error != FT_Error.FT_Err_Ok || (int)face->glyph->bitmap.width == 0)
                        {
                            Console.WriteLine(error);
                            continue;
                        }
                        using Bitmap bitmap = new Bitmap((int)face->glyph->bitmap.width, (int)face->glyph->bitmap.rows);

                        byte* glyphData = face->glyph->bitmap.buffer;

                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            for (int y = 0; y < bitmap.Height; y++)
                            {
                                //Console.WriteLine(glyphData[x * bitmap.Width + y]);
                                bitmap.SetPixel(x, y, glyphData[x + ((bitmap.Height - 1 - y) * bitmap.Width)] == 0 ? Color.FromArgb(0, Color.Black) : Color.White);
                            }
                        }

                        bitmap.Save("Assets/FontGlyphs/" + *face->family_name + "/" + (int)c + ".png", ImageFormat.Png);
                    }
                }

                for (char c = (char)0; c < 127; c++)
                {
                    if (!File.Exists("Assets/FontGlyphs/" + *face->family_name + "/" + (int)c + ".png"))
                    {
                        Console.WriteLine("Missing glyph " + (int)c);
                        continue;
                    }

                    Console.WriteLine("Found glyph " + (int)c);

                    using Image glyph = Image.FromFile("Assets/FontGlyphs/" + *face->family_name + "/" + (int)c + ".png");

                    float width = glyph.Width / 1280f;
                    float height = glyph.Height / 720f;

                    Console.WriteLine(width);
                    Console.WriteLine(height);

                    Vertex[] vertices = {
                        new Vertex(new Vector4(0, height, 0.5f, 1.0f), new Vector2(0.0f, 1.0f)),
                        new Vertex(new Vector4(width, 0, 0.5f, 1.0f), new Vector2(1.0f, 0.0f)),
                        new Vertex(new Vector4(0, 0, 0.5f, 1.0f), new Vector2(0.0f, 0.0f)),
                        new Vertex(new Vector4(width, height, 0.5f, 1.0f), new Vector2(1.0f, 1.0f)),
                    };

                    ushort[] indices = [
                        0, 1, 2,
                    0, 3, 1
                    ];

                    Mesh mesh = new Mesh(vertices, indices, "Assets/FontGlyphs/" + *face->family_name + "/" + (int)c + ".png");

                    characterVisuals.Add(c, renderer.CreateVisual(mesh));
                    characterSizes.Add(c, width);
                    characterHeightOffsets.Add(c, new char[] { 'g', 'j', 'q', 'y', 'p' }.Contains(c) ? -0.01f : 0f);
                }
            }
        }

        public void DrawCharacter(char c, ColorRGB color, Vector3 position)
        {
            characterVisuals[c].Position = position + new Vector3(0, characterHeightOffsets[c], 0);
            characterVisuals[c].Tint = color;
            characterVisuals[c].Draw();
        }

        public void DrawString(string s, ColorRGB color, Vector3 position, float characterOffset = 0.005f)
        {
            Vector3 currentPos = position;
            foreach (char c in s)
            {
                if (c != ' ')
                {
                    DrawCharacter(c, color, currentPos);

                    currentPos += new Vector3(characterSizes[c] + characterOffset, 0, 0);
                } else
                {
                    currentPos += new Vector3(0.02f, 0, 0);
                }
            }
        }

        public void DrawStringShadowed(string s, ColorRGB color, ColorRGB shadowColor, Vector3 position, float characterOffset = 0.005f, float shadowOffsetX = -0.0008f, float shadowOffsetY = -0.0008f)
        {
            DrawString(s, shadowColor, position + new Vector3(shadowOffsetX, shadowOffsetY, 0), characterOffset);
            DrawString(s, color, position, characterOffset);
        }

        public void DrawStringCentered(string s, ColorRGB color, Vector3 position, float charcterOffset = 0.005f)
        {
            float length = 0f;
            foreach (char c in s)
            {
                if (c != ' ')
                {
                    length += characterSizes[c] + charcterOffset;
                } else 
                {
                    length += 0.02f;
                }
            }
            Console.WriteLine(length);
            //DrawString(s, color, position, length);
            DrawString(s, color, position - new Vector3(length / 2, 0, 0));
        }

        public void DrawStringShadowedCentered(string s, ColorRGB color, ColorRGB shadowColor, Vector3 position, float characterOffset = 0.005f, float shadowOffsetX = -0.0004f, float shadowOffsetY = -0.0004f)
        {
            DrawStringCentered(s, shadowColor, position + new Vector3(shadowOffsetX, shadowOffsetY, 0), characterOffset);
            DrawStringCentered(s, color, position, characterOffset);
        }
    }
}
