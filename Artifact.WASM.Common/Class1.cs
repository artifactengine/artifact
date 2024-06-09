using Silk.NET.OpenGLES;
using System.Runtime.InteropServices.JavaScript;

namespace Artifact.WASM.Common
{
    public class WASMCommon
    {
        public static WASMCommon current;

        public WASMCommon()
        {
            current = this;
        }

        public GL gl;

        public byte[] assets;
        public Action<string> SetFPSMarker;
    }
}
