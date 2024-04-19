using NLog;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugins.Rendering.OpenGLBackend
{

    public class OpenGLRenderingBackend : IRenderingBackend
    {
        public Type VisualImplementation => typeof(OpenGLVisual);

        internal static OpenGL gl;
        private Graphics gfx;

        public string FancyName => "OpenGL 4.4";

        private Logger logger = LogManager.GetCurrentClassLogger();

        public void CreateContext(nint windowHandle, int width, int height)
        {
            gl = new OpenGL();

            gl.Create(SharpGL.Version.OpenGLVersion.OpenGL3_3, RenderContextType.NativeWindow, width, height, 24, windowHandle);
            gl.MakeCurrent();
            gl.Viewport(0, 0, width, height);
            gfx = Graphics.FromHwnd(windowHandle);

            logger.Info("Created OpenGL 4.4 Context");
        }

        public void SwapBuffers()
        {
            try
            {
                nint hdc = gfx.GetHdc();
                gl.Blit(hdc);
                gfx.ReleaseHdc(hdc);
            } catch
            {

            }
            
        }

        public void DisposeContext()
        {
            gl.MakeNothingCurrent();
            gfx.Dispose();

            logger.Info("Disposed OpenGL 4.4 Context");
        }

        public void Clear(ColorRGB color, float alpha)
        {
            gl.ClearColor(color.UnitR, color.UnitG, color.UnitB, alpha);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT);
        }
    }
}
