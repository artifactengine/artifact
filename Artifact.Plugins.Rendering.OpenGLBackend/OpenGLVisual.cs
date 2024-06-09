using Artifact.Plugins.Windowing;
using NLog;
using SharpGL;
using Silk.NET.OpenGL;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugins.Rendering.OpenGLBackend
{
    public class OpenGLVisual : ArtifactDisposable, IVisual
    {
        private GL gl;

        private uint vbo, vao, ebo;
        private uint shaderProgram, vertexShader, fragmentShader;
        private uint texture;

        private Mesh _mesh;

        private static Dictionary<string, uint> vertexShaderCache = new Dictionary<string, uint>();
        private static Dictionary<string, uint> fragmentShaderCache = new Dictionary<string, uint>();
        private static Dictionary<Vertex[], uint> vertexArrayCache = new Dictionary<Vertex[], uint>();
        private static Dictionary<ushort[], uint> elementBufferCache = new Dictionary<ushort[], uint>();
        private static Dictionary<string, uint> textureCache = new Dictionary<string, uint>();

        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; } = new Vector3(1, 1, 1);
        public Vector3 Rotation { get; set; }

        public ColorRGB Tint { get; set; } = new ColorRGB(255, 255, 255, 255);


        public unsafe OpenGLVisual(Mesh mesh) : base()
        {
            gl = OpenGLRenderingBackend.gl;

            string fullVertexPath = "Assets/Shaders/opengl/" + mesh.VertexShaderPath + ".vert";
            string fullFragmentPath = "Assets/Shaders/opengl/" + mesh.FragmentShaderPath + ".frag";

            if (vertexShaderCache.ContainsKey(fullVertexPath))
            {
                vertexShader = vertexShaderCache[fullVertexPath];
            }
            else
            {
                string vertexShaderSource = File.ReadAllText(fullVertexPath);

                vertexShader = gl.CreateShader(GLEnum.VertexShader);

                gl.ShaderSource(vertexShader, vertexShaderSource);
                gl.CompileShader(vertexShader);

                vertexShaderCache.Add(fullVertexPath, vertexShader);
            }

            if (fragmentShaderCache.ContainsKey(fullFragmentPath))
            {
                fragmentShader = fragmentShaderCache[fullFragmentPath];
            }
            else
            {
                string fragmentShaderSource = File.ReadAllText(fullFragmentPath);

                fragmentShader = gl.CreateShader(GLEnum.FragmentShader);

                gl.ShaderSource(fragmentShader, fragmentShaderSource);
                gl.CompileShader(fragmentShader);

                fragmentShaderCache.Add(fullFragmentPath, fragmentShader);
            }



            shaderProgram = gl.CreateProgram();

            gl.AttachShader(shaderProgram, vertexShader);
            gl.AttachShader(shaderProgram, fragmentShader);

            gl.LinkProgram(shaderProgram);





            if (vertexArrayCache.ContainsKey(mesh.Vertices))
            {
                Console.WriteLine("Use cached VAO");
                vao = vertexArrayCache[mesh.Vertices];
            }
            else
            {
                Console.WriteLine("Create new VAO");
                uint[] vbos = new uint[1];

                gl.GenBuffers(1, vbos);

                vbo = vbos[0];

                uint[] vaos = new uint[1];

                gl.GenVertexArrays(1, vaos);

                vao = vaos[0];
                gl.BindVertexArray(vao);


                gl.BindBuffer(GLEnum.ArrayBuffer, vbo);
                fixed (Vertex* vertices = mesh.Vertices)
                    gl.BufferData(GLEnum.ArrayBuffer, (nuint)(mesh.Vertices.Length * sizeof(Vertex)), vertices, GLEnum.StaticDraw);

                uint positionLocation = (uint)gl.GetAttribLocation(shaderProgram, "aPos");
                uint colorLocation = (uint)gl.GetAttribLocation(shaderProgram, "aTexCoord");

                Console.WriteLine(positionLocation);

                gl.VertexAttribPointer(positionLocation, 4, GLEnum.Float, false, 6 * sizeof(float), 0);
                gl.VertexAttribPointer(colorLocation, 2, GLEnum.Float, false, 6 * sizeof(float), 4 * sizeof(float));
                gl.EnableVertexAttribArray(positionLocation);
                gl.EnableVertexAttribArray(colorLocation);

                gl.BindBuffer(GLEnum.ArrayBuffer, 0);

                vertexArrayCache.Add(mesh.Vertices, vao);
            }


            if (elementBufferCache.ContainsKey(mesh.Indices))
            {
                Console.WriteLine("Use cached ebo");
                ebo = elementBufferCache[mesh.Indices];
            }
            else
            {
                Console.WriteLine("Create new ebo");
                uint[] ebos = new uint[1];
                gl.GenBuffers(1, ebos);

                ebo = ebos[0];

                gl.BindBuffer(GLEnum.ElementArrayBuffer, ebo);
                fixed (ushort* indices = mesh.Indices)
                    gl.BufferData(GLEnum.ElementArrayBuffer, (nuint)(sizeof(ushort) * mesh.Indices.Length), indices, GLEnum.StaticDraw);
                gl.BindBuffer(GLEnum.ElementArrayBuffer, 0);

                elementBufferCache.Add(mesh.Indices, ebo);

            }

            _mesh = mesh;

            if (textureCache.ContainsKey(mesh.TexturePath))
            {
                Console.WriteLine("Use cached texture");
                texture = textureCache[mesh.TexturePath];
            }
            else
            {
                uint[] textures = new uint[1];
                gl.GenTextures(1, textures);

                texture = textures[0];

                gl.BindTexture(GLEnum.Texture2D, texture);
                //gl.ActiveTexture(OpenGL.GL_TEXTURE0);

                gl.UseProgram(shaderProgram);

                gl.Uniform1(gl.GetUniformLocation(shaderProgram, "tex0"), 0);

                gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)TextureWrapMode.Repeat);
                gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)TextureWrapMode.Repeat);
                gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMinFilter, Application.current.GetPlugin<RenderingPlugin>().SamplerMode == SamplerMode.Smooth ? (int)TextureMinFilter.Linear : (int)TextureMinFilter.Nearest);
                gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMagFilter, Application.current.GetPlugin<RenderingPlugin>().SamplerMode == SamplerMode.Smooth ? (int)TextureMagFilter.Linear : (int)TextureMagFilter.Nearest);


                StbImage.stbi_set_flip_vertically_on_load(0);

                FileStream stream = File.OpenRead(mesh.TexturePath);

                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

                fixed (byte* ptr = image.Data)
                    gl.TexImage2D(GLEnum.Texture2D, 0, InternalFormat.Rgba, (uint)image.Width, (uint)image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);

                textureCache.Add(mesh.TexturePath, texture);

                Console.WriteLine("Load new texture");
            }
        }

        public unsafe void Draw()
        {
            Matrix4x4 model = Matrix4x4.Identity;

            Matrix4x4 translation = Matrix4x4.CreateTranslation(Position);
            Matrix4x4 scale = Matrix4x4.CreateScale(Scale);
            Matrix4x4 rotation = Matrix4x4.CreateFromYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z);

            model = scale * rotation * translation;

            gl.UseProgram(shaderProgram);

            gl.UniformMatrix4(gl.GetUniformLocation(shaderProgram, "model"), 1, false, model.AsFloatArray());
            gl.UniformMatrix4(gl.GetUniformLocation(shaderProgram, "proj"), 1, false, Camera.ProjectionMatrix.AsFloatArray());
            gl.UniformMatrix4(gl.GetUniformLocation(shaderProgram, "view"), 1, false, Camera.ViewMatrix.AsFloatArray());
            gl.Uniform4(gl.GetUniformLocation(shaderProgram, "tint"), Tint.UnitR, Tint.UnitG, Tint.UnitB, Tint.UnitA);

            gl.BindTexture(GLEnum.Texture2D, texture);
            gl.ActiveTexture(GLEnum.Texture0);

            //Console.WriteLine(string.Join(", ", Camera.ViewMatrix.AsFloatArray()));

            //Console.WriteLine("[" + string.Join(", ", translation.AsFloatArray()) + "]");

            gl.BindVertexArray(vao);
            gl.BindBuffer(GLEnum.ElementArrayBuffer, ebo);

            gl.DrawElements(GLEnum.Triangles, (uint)_mesh.Indices.Length, GLEnum.UnsignedShort, (void*)0);
        }

        public override void Dispose()
        {

        }
    }
}