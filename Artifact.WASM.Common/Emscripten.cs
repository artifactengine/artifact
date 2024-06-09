using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.WASM.Common
{
    public class Emscripten
    {
        [DllImport("emscripten", EntryPoint = "emscripten_request_animation_frame_loop")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
        public static extern unsafe void RequestAnimationFrameLoop(void* f, nint userDataPtr);

        [DllImport("emscripten", EntryPoint = "emscripten_set_canvas_element_size")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
        public static extern void SetCanvasElementSize(string id, int width, int height);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EmscriptenWebGLContextAttributes
    {
        public bool alpha;
        public bool depth;
        public bool stencil;
        public bool antialias;
        public bool premultipliedAlpha;
        public bool preserveDrawingBuffer;
        public int powerPreference;
        public bool failIfMajorPerformanceCaveat;

        public int majorVersion;
        public int minorVersion;

        public bool enableExtensionsByDefault;
        public bool explicitSwapControl;
        public int proxyContextToMainThread;
        public bool renderViaOffscreenBackBuffer;
    }

    public class EmscriptenWebGL2
    {
        [DllImport("emscripten", EntryPoint = "emscripten_webgl2_get_proc_address")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
        public static extern IntPtr GetProcAddress(string name);

        [DllImport("emscripten", EntryPoint = "emscripten_webgl_make_context_current")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
        public static extern IntPtr MakeContextCurrent(UIntPtr ctx);

        [DllImport("emscripten", EntryPoint = "emscripten_webgl_create_context")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
        public static extern unsafe UIntPtr CreateContext(string canvasId, EmscriptenWebGLContextAttributes* attrs);
        

        [DllImport("emscripten", EntryPoint = "emscripten_webgl2_get_proc_address")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
        public static extern unsafe void InitContextAttributes(EmscriptenWebGLContextAttributes* attribs);
    
        
    }
}
