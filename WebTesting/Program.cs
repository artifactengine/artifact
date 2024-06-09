using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using Artifact.Plugins.Rendering;
using Artifact.Plugins.Rendering.WebGLBackend;
using Artifact.WASM.Common;

public partial class Entrypoint
{
    public static WebGLRenderingBackend backend;

    [UnmanagedCallersOnly]
    public static int Frame(double time, nint userData)
    {
        Console.WriteLine("Loop");
        ColorRGB color = new ColorRGB(255, 255, 0, 255);

        backend.Clear(color, 1.0f);


        return 1;
    }

    [JSExport]
    public static void Main()
    {
        backend = new WebGLRenderingBackend();

        backend.CreateContext(null, 1280, 720);

        unsafe
        {
            Emscripten.RequestAnimationFrameLoop((delegate* unmanaged<double, nint, int>)&Frame, nint.Zero);
        }
    }
}