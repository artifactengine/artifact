using NLog;
using SharpGL;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugins.Rendering.OpenGLBackend
{

    public class OpenGLRenderingBackend : ArtifactDisposable, IRenderingBackend
    {
        public Type VisualImplementation => typeof(OpenGLVisual);

        internal static GL gl;
        private Graphics gfx;

        public string FancyName => "OpenGL 3.3";

        private Logger logger = LogManager.GetCurrentClassLogger();


        public OpenGLRenderingBackend() : base() { }

        public void CreateContext(object windowHandle, int width, int height)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                CrossGL.Loader.LoadOpenGL(0, (nint)windowHandle);
            else
                CrossGL.Loader.LoadOpenGL((((nint, nint))windowHandle).Item1, (((nint, nint))windowHandle).Item2);

            gl = GL.GetApi(CrossGL.Loader.GetProcAddress);

            gl.Enable(EnableCap.Multisample);
        }

        public void SwapBuffers()
        {
            CrossGL.Loader.SwapBuffers();
        }

        public override void Dispose()
        {
            
        }

        public void Clear(ColorRGB color, float alpha)
        {
            gl.ClearColor(color.UnitR, color.UnitG, color.UnitB, color.UnitA);
            gl.Clear(ClearBufferMask.ColorBufferBit);
        }

        public void Predraw()
        {

        }

        public void DrawPoint(Vector3 position, ColorRGB color, float size = 10f)
        {

        }
    }
}