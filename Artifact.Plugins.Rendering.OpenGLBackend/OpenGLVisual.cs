using Artifact.Plugins.Windowing;
using NLog;
using SharpGL;
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
        private OpenGL gl;

        private uint vbo, vao, ebo;
        private uint shaderProgram, vertexShader, fragmentShader;
        private uint texture;

        private Mesh _mesh;

        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; } = new Vector3(1, 1, 1);
        public Vector3 Rotation { get; set; }

        public static IntPtr ConvertStructArrayToIntPtr<T>(T[] structArray) where T : struct
        {
            // Allocate unmanaged memory
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)) * structArray.Length);

            // Copy each struct to the unmanaged memory
            for (int i = 0; i < structArray.Length; i++)
            {
                IntPtr currentPtr = new IntPtr(ptr.ToInt64() + (i * Marshal.SizeOf(typeof(T))));
                Marshal.StructureToPtr(structArray[i], currentPtr, false);
            }

            // Return the IntPtr pointing to the start of the unmanaged memory
            return ptr;
        }

        public unsafe OpenGLVisual(Mesh mesh) : base()
        {
            gl = OpenGLRenderingBackend.gl;

            string vertexShaderSource = File.ReadAllText("Assets/Shaders/opengl/default.vert");

            string fragmentShaderSource = File.ReadAllText("Assets/Shaders/opengl/default.frag");

            vertexShader = gl.CreateShader(OpenGL.GL_VERTEX_SHADER);

            gl.ShaderSource(vertexShader, vertexShaderSource);
            gl.CompileShader(vertexShader);

            fragmentShader = gl.CreateShader(OpenGL.GL_FRAGMENT_SHADER);

            gl.ShaderSource(fragmentShader, fragmentShaderSource);
            gl.CompileShader(fragmentShader);

            shaderProgram = gl.CreateProgram();

            gl.AttachShader(shaderProgram, vertexShader);
            gl.AttachShader(shaderProgram, fragmentShader);

            gl.LinkProgram(shaderProgram);


            gl.DeleteShader(vertexShader);
            gl.DeleteShader(fragmentShader);

            uint[] vbos = new uint[1];

            gl.GenBuffers(1, vbos);

            vbo = vbos[0];

            uint[] vaos = new uint[1];

            gl.GenVertexArrays(1, vaos);

            vao = vaos[0];
            gl.BindVertexArray(vao);


            gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, vbo);
            gl.BufferData(OpenGL.GL_ARRAY_BUFFER, mesh.Vertices.Length * sizeof(Vertex), ConvertStructArrayToIntPtr(mesh.Vertices), OpenGL.GL_STATIC_DRAW);

            uint positionLocation = (uint)gl.GetAttribLocation(shaderProgram, "aPos");
            uint colorLocation = (uint)gl.GetAttribLocation(shaderProgram, "aTexCoord");

            Console.WriteLine(positionLocation);

            gl.VertexAttribPointer(positionLocation, 4, OpenGL.GL_FLOAT, false, 6 * sizeof(float), 0);
            gl.VertexAttribPointer(colorLocation, 2, OpenGL.GL_FLOAT, false, 6 * sizeof(float), 4 * sizeof(float));
            gl.EnableVertexAttribArray(positionLocation);
            gl.EnableVertexAttribArray(colorLocation);

            gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, 0);


            uint[] ebos = new uint[1];
            gl.GenBuffers(1, ebos);

            ebo = ebos[0];

            gl.BindBuffer(OpenGL.GL_ELEMENT_ARRAY_BUFFER, ebo);
            gl.BufferData(OpenGL.GL_ELEMENT_ARRAY_BUFFER, mesh.Indices, OpenGL.GL_STATIC_DRAW);
            gl.BindBuffer(OpenGL.GL_ELEMENT_ARRAY_BUFFER, 0);

            _mesh = mesh;

            uint[] textures = new uint[1];
            gl.GenTextures(1, textures);

            texture = textures[0];

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, texture);
            gl.ActiveTexture(OpenGL.GL_TEXTURE0);

            gl.UseProgram(shaderProgram);

            gl.Uniform1(gl.GetUniformLocation(shaderProgram, "tex0"), 0);

            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR_MIPMAP_LINEAR);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);


            StbImage.stbi_set_flip_vertically_on_load(0);

            FileStream stream = File.OpenRead(mesh.TexturePath);

            ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

            gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGBA, image.Width, image.Height, 0, OpenGL.GL_RGBA, OpenGL.GL_UNSIGNED_BYTE, image.Data);
            gl.GenerateMipmapEXT(OpenGL.GL_TEXTURE_2D);

            
        }

        public void Draw()
        {
            Matrix4x4 model = Matrix4x4.Identity;

            Matrix4x4 translation = Matrix4x4.CreateTranslation(Position);
            Matrix4x4 scale = Matrix4x4.CreateScale(Scale);
            Matrix4x4 rotation = Matrix4x4.CreateFromYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z);

            model = translation * scale * rotation;

            gl.UseProgram(shaderProgram);

            gl.UniformMatrix4(gl.GetUniformLocation(shaderProgram, "model"), 1, false, model.AsFloatArray());
            gl.UniformMatrix4(gl.GetUniformLocation(shaderProgram, "proj"), 1, false, Camera.ProjectionMatrix.AsFloatArray());
            gl.UniformMatrix4(gl.GetUniformLocation(shaderProgram, "view"), 1, false, Camera.ViewMatrix.AsFloatArray());

            //Console.WriteLine(string.Join(", ", Camera.ViewMatrix.AsFloatArray()));

            //Console.WriteLine("[" + string.Join(", ", translation.AsFloatArray()) + "]");

            gl.BindVertexArray(vao);
            gl.BindBuffer(OpenGL.GL_ELEMENT_ARRAY_BUFFER, ebo);
            
            gl.DrawElements(OpenGL.GL_TRIANGLES, _mesh.Indices.Length, OpenGL.GL_UNSIGNED_SHORT, 0);
        }

        public override void Dispose()
        {
            //gl.DeleteBuffers()
            //gl.DeleteVertexArrays(1, [vao]);

            //gl.DeleteProgram(shaderProgram);
        }
    }
}
