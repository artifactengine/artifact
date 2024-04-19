using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugins.Rendering
{
    public interface IRenderingBackend
    {
        public Type VisualImplementation { get; }

        public string FancyName { get; }

        public void CreateContext(IntPtr windowHandle, int width, int height);

        public void DisposeContext();

        public void SwapBuffers();

        public void Clear(ColorRGB color, float alpha);
    }
}
