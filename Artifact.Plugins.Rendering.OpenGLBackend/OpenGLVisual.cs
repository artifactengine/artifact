using NLog;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugins.Rendering.OpenGLBackend
{
    public class OpenGLVisual : Visual
    {
        private OpenGL gl;

        private uint vbo, vao, ebo;
        private uint shaderProgram, vertexShader, fragmentShader;

        private Mesh _mesh;

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

        ~OpenGLVisual()
        {
            Console.WriteLine("Finalize!");
        }

        public unsafe OpenGLVisual(Mesh mesh) : base(mesh)
        {
            gl = OpenGLRenderingBackend.gl;


            Vertex[] vertices = {
                new Vertex(new Vector4(-0.5f, -0.5f, 0.0f, 0.0f), new Vector4(0.180392f, 0.180392f, 0.180392f, 1.0f)),
                new Vertex(new Vector4(0.5f, -0.5f, 0.0f, 0.0f), new Vector4(0.180392f, 0.180392f, 0.180392f, 1.0f)),
                new Vertex(new Vector4(0.0f, 0.5f, 0.0f, 0.0f), new Vector4(0.180392f, 0.180392f, 0.180392f, 1.0f)),
            };

            string vertexShaderSource = @" 
#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aColor;

out vec3 color;

void main()
{
    gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);
    color = aColor;
}
                ";

            string fragmentShaderSource = @" 
#version 330 core
out vec4 FragColor;

in vec3 color;

void main()
{
    FragColor = vec4(color, 1.0f);
}
                ";

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

            float[] verticesF = {
                0.0f, 0.5f, 0.0f, 1.0f, 1.0f, 0.0f,
                0.5f, -0.5f, 0.0f, 1.0f, 1.0f, 0.0f,
                -0.5f, -0.5f, 0.0f, 1.0f, 1.0f, 0.0f,
            };


            gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, vbo);
            gl.BufferData(OpenGL.GL_ARRAY_BUFFER, mesh.Vertices.Length * sizeof(Vertex), ConvertStructArrayToIntPtr(mesh.Vertices), OpenGL.GL_STATIC_DRAW);

            uint positionLocation = (uint)gl.GetAttribLocation(shaderProgram, "aPos");
            uint colorLocation = (uint)gl.GetAttribLocation(shaderProgram, "aColor");

            Console.WriteLine(positionLocation);

            gl.VertexAttribPointer(positionLocation, 4, OpenGL.GL_FLOAT, false, 8 * sizeof(float), 0);
            gl.VertexAttribPointer(colorLocation, 4, OpenGL.GL_FLOAT, false, 8 * sizeof(float), 4 * sizeof(float));
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
        }

        public override void Draw()
        {
            gl.UseProgram(shaderProgram);

            gl.BindVertexArray(vao);
            gl.BindBuffer(OpenGL.GL_ELEMENT_ARRAY_BUFFER, ebo);
            gl.DrawElements(OpenGL.GL_TRIANGLES, _mesh.Indices.Length, OpenGL.GL_UNSIGNED_SHORT, 0);
        }

        public override void Dispose()
        {
            gl.DeleteBuffers(1, [vbo]);
            gl.DeleteVertexArrays(1, [vao]);

            gl.DeleteProgram(shaderProgram);
        }
    }
}
