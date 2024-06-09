using Artifact.WASM.Common;
using Silk.NET.OpenGLES;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugins.Rendering.WebGLBackend
{
    public class WebGLRenderingBackend : IRenderingBackend
    {
        public Type VisualImplementation => throw new NotImplementedException();

        public string FancyName => "WebGL";

        public static GL gl;

        public void Clear(ColorRGB color, float alpha)
        {
            
        }

        public unsafe void CreateContext(object windowHandle, int width, int height)
        {
            EmscriptenWebGLContextAttributes attr = new EmscriptenWebGLContextAttributes();

            EmscriptenWebGL2.InitContextAttributes(&attr);

            attr.alpha = true;
            attr.depth = true;
            attr.stencil = false;
            attr.majorVersion = 2;
            attr.minorVersion = 0;

            UIntPtr ctx = EmscriptenWebGL2.CreateContext("canvas", &attr);
            EmscriptenWebGL2.MakeContextCurrent(ctx);

            gl = GL.GetApi(EmscriptenWebGL2.GetProcAddress);

            Console.WriteLine("GL_VERSION=" + Marshal.PtrToStringUTF8((IntPtr)gl.GetString(GLEnum.Version)));
        }

        public void Predraw()
        {
            
        }

        public void SwapBuffers()
        {
            
        }
    }
}
