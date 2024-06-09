using PInvoke;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static PInvoke.User32;

namespace CrossGL
{
    struct PixelFormatDescriptor
    {
        public short size;
        public short version;
        public ushort flags;
        public byte pixelType;
        public byte colorBits;
        public byte redBits;
        public byte redShift;
        public byte greenBits;
        public byte greenShift;
        public byte blueBits;
        public byte blueShift;
        public byte alphaBits;
        public byte alphaShift;
        public byte accumBits;
        public byte accumRedBits;
        public byte accumGreenBits;
        public byte accumBlueBits;
        public byte accumAlphaBits;
        public byte depthBits;
        public byte stencilBits;
        public byte auxBuffers;
        public byte layerType;
        public byte reserved;
        public ushort layerMask;
        public ushort visibleMask;
        public ushort damageMask;
    }

    internal unsafe class OpenGLGdi
    {
        [DllImport("gdi32.dll")]
        internal static extern int ChoosePixelFormat(nint hdc, PixelFormatDescriptor* pfd);

        [DllImport("gdi32.dll")]
        internal static extern int DescribePixelFormat(nint hdc, int pixelFormat, uint x, PixelFormatDescriptor* pfd);

        [DllImport("gdi32.dll")]
        internal static extern int SetPixelFormat(nint hdc, int pixelFormat, PixelFormatDescriptor* pfd);

        [DllImport("opengl32.dll")]
        internal static extern int wglCreateContext(nint hdc);

        [DllImport("opengl32.dll")]
        internal static extern int wglMakeCurrent(nint hdc, nint hrc);

        [DllImport("opengl32.dll")]
        internal static extern void* wglGetProcAddress(string name);

        [DllImport("opengl32.dll")]
        internal static extern void* wglDeleteContext(nint rc);

        internal delegate bool PFNWGLCHOOSEPIXELFORMATARBPROC(nint hdc, int* piAttribIList, nint pfAttribFList, uint maxFormats, int* piFormats, uint* numFormats);
        internal delegate nint PFNWGLCREATECONTEXTATTRIBSARBPROC(nint hdc, nint hglrc, int* attribList);
        internal delegate bool PFNWGLSWAPINTERVALEXTPROC(int interval);
    }

    internal unsafe class GLXHelper
    {
        [DllImport("libglx-helper")]
        public static extern void create_context(nint display, nint window);

        [DllImport("libglx-helper", CallingConvention = CallingConvention.Cdecl)]
        public static extern nint get_proc_address(string name);

        [DllImport("libglx-helper", CallingConvention = CallingConvention.Cdecl)]
        public static extern void swap_buffers();
    }

    public class Loader
    {
        private delegate IntPtr wglCreateContextAttribsARB(IntPtr hDC, IntPtr hShareContext, int[] attribList);

        private unsafe static nint DummyWndProc(nint hWnd, WindowMessage message, void* wParam, void* lParam)
        {
            return DefWindowProc(hWnd, message, new nint(wParam), new nint(lParam));
        }

        private static void LoadOpenGLWin32(nint hwnd)
        {
            OpenGLGdi.PFNWGLCHOOSEPIXELFORMATARBPROC choosePixelFormatARB;
            OpenGLGdi.PFNWGLCREATECONTEXTATTRIBSARBPROC createContextAttribsARB;
            OpenGLGdi.PFNWGLSWAPINTERVALEXTPROC swapIntervalExt;
            unsafe
            {


                if (hwnd == nint.Zero)
                {
                    throw new Exception("Failed to create fake window to create Windows OpenGL context");
                }


                SafeDCHandle fakeDC = GetDC(hwnd);

                PixelFormatDescriptor pfd = new PixelFormatDescriptor()
                {
                    size = (short)sizeof(PixelFormatDescriptor),
                    version = 1,
                    //      Draw to window Suport OpenGL Double Buffer
                    flags = 0x00000004 | 0x00000020 | 0x00000001,
                    pixelType = 0, // : PFD_TYPE_RGBA,
                    colorBits = 32,
                    alphaBits = 8,
                    depthBits = 24,
                };

                int pixelFormat = OpenGLGdi.ChoosePixelFormat(fakeDC.DangerousGetHandle(), &pfd);

                if (pixelFormat < 0)
                {
                    throw new Exception("Failed to choose pixel format");
                }

                if (OpenGLGdi.SetPixelFormat(fakeDC.DangerousGetHandle(), pixelFormat, &pfd) < 0)
                {
                    throw new Exception("Failed to set pixel format");
                }

                nint fakeRC = OpenGLGdi.wglCreateContext(fakeDC.DangerousGetHandle());

                if (fakeRC < 0)
                {
                    throw new Exception("Failed to create rendering context");
                }


                if (OpenGLGdi.wglMakeCurrent(fakeDC.DangerousGetHandle(), fakeRC) < 0)
                {
                    throw new Exception("Failed to make context current");
                }

                void* choosePixelFormat = OpenGLGdi.wglGetProcAddress("wglChoosePixelFormatARB");
                void* createContextAttribs = OpenGLGdi.wglGetProcAddress("wglCreateContextAttribsARB");
                void* swapInterval = OpenGLGdi.wglGetProcAddress("wglCreateContextAttribsARB");

                choosePixelFormatARB = Marshal.GetDelegateForFunctionPointer<OpenGLGdi.PFNWGLCHOOSEPIXELFORMATARBPROC>(new nint(choosePixelFormat));
                createContextAttribsARB = Marshal.GetDelegateForFunctionPointer<OpenGLGdi.PFNWGLCREATECONTEXTATTRIBSARBPROC>(new nint(createContextAttribs));
                swapIntervalExt = Marshal.GetDelegateForFunctionPointer<OpenGLGdi.PFNWGLSWAPINTERVALEXTPROC>(new nint(swapInterval));

                OpenGLGdi.wglMakeCurrent(nint.Zero, nint.Zero);
                OpenGLGdi.wglDeleteContext(fakeRC);
                ReleaseDC(hwnd, fakeDC.DangerousGetHandle());

            }
            unsafe
            {
                nint dc = GetDC(hwnd).DangerousGetHandle();

                if (dc < 0)
                {
                    //throw new Exception("Failed to get DC");
                }

                int[] pixelAttribs = [
                     0x2001, 1,
                     0x2010, 1,
                     0x2011, 1,
                     0x2007, 0x2029,
                     0x2013, 0x202B,
                     0x2003, 0x2027,
                     0x2014, 24,
                     0x201B, 8,
                     0x2022, 24,
                     0x2042, 8,
                     0
                ];

                uint numPixelFormats;
                int pixelFormat = 0;
                int* pixelAttribsPtr;
                fixed (int* pixelAttribsptr = pixelAttribs)
                {
                    pixelAttribsPtr = pixelAttribsptr;
                }
                if (!choosePixelFormatARB.Invoke(dc, pixelAttribsPtr, 0, 1, pixelAttribsPtr, &numPixelFormats))
                {
                    throw new Exception("Failed to choose pixel format");
                }

                PixelFormatDescriptor pfd = new PixelFormatDescriptor();
                OpenGLGdi.DescribePixelFormat(dc, pixelFormat, (uint)sizeof(PixelFormatDescriptor), &pfd);

                if (OpenGLGdi.SetPixelFormat(dc, pixelFormat, &pfd) < 0)
                {
                    throw new Exception("Failed to set pixel format");
                }

                int[] contextAttribs = [
                    0x2091, 3,
                    0x2092, 3,
                    0x9126, 0x00000001,
                    0x2094, 0x00000001,
                    0
                ];

                fixed (int* contextAttribsPtr = contextAttribs)
                {
                    nint rc = createContextAttribsARB.Invoke(dc, 0, contextAttribsPtr);

                    if (rc < 0)
                    {
                        throw new Exception("Failed to create render context");
                    }

                    if (OpenGLGdi.wglMakeCurrent(dc, rc) < 0)
                    {
                        throw new Exception("Failed to make context current");
                    }
                }

                hdc = dc;

            }
        }

        private unsafe static void LoadOpenGLX11(nint display, nint window)
        {
            Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ":.");

            GLXHelper.create_context(display, window);
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("gdi32.dll", EntryPoint = "SwapBuffers")]
        private static extern bool _SwapBuffers(nint hdc);


        public static IntPtr opengl32 = 0;

        private static nint hdc;

        public static void SwapBuffers()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                _SwapBuffers(hdc);
            else
                GLXHelper.swap_buffers();
        }


        public unsafe static nint GetProcAddress(string name)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                nint ptr = new nint(OpenGLGdi.wglGetProcAddress(name));

                if (ptr == 0)
                {
                    if (opengl32 == 0)
                    {
                        opengl32 = LoadLibrary("opengl32.dll");
                    }
                    return GetProcAddress(opengl32, name);
                }
                else
                {
                    return ptr;
                }
            }
            else
            {
                nint ptr = GLXHelper.get_proc_address(name);
                return ptr;
            }
        }

        /// <summary>
        /// Loads OpenGL from the specified display and window.
        /// </summary>
        /// <param name="display">The handle to the display - unused on Windows, so just use nint.Zero</param>
        /// <param name="window"><The window handle/param>
        public static void LoadOpenGL(nint display, nint window)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Create context on Windows
                LoadOpenGLWin32(window);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Create context on Linux
                LoadOpenGLX11(display, window);
            }
        }
    }
}
