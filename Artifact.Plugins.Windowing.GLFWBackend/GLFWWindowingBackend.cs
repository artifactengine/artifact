using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Silk.NET.GLFW;
using System.Runtime.InteropServices;
using Artifact.Plugins.Input;
using System.Transactions;

namespace Artifact.Plugins.Windowing.GLFWBackend
{
    public unsafe class GLFWWindowingBackend : IWindowingBackend
    {
        private object _windowhandle = null;
        public object WindowHandle => _windowhandle;

        public string Title { get; set; } = "";
        public string TitleSuffix { get; set; } = "";

        public Logger logger = LogManager.GetCurrentClassLogger();

        private Application _app;
        private Glfw glfw;

        private WindowHandle* window;
        private GlfwNativeWindow nativeWindow;

        public void Close()
        {
            glfw.DestroyWindow(window);
            glfw.Terminate();
        }

        Dictionary<Key, bool> keys = new Dictionary<Key, bool>();

        public void CreateWindow(string title, int width, int height, Application app)
        {
            _app = app;
            Title = title;

            glfw = Glfw.GetApi();
            glfw.Init();
            glfw.WindowHint(WindowHintInt.Samples, 8);
            glfw.WindowHint(WindowHintClientApi.ClientApi, ClientApi.NoApi);
            glfw.WindowHint(WindowHintBool.Resizable, false);

            window = glfw.CreateWindow(width, height, Title + " - Artifact Engine", null, null);
            nativeWindow = new GlfwNativeWindow(glfw, window);
            logger.Info("Created window using GLFW backend");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _windowhandle = nativeWindow.Win32.Value.Hwnd;
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _windowhandle = (nativeWindow.X11.Value.Display, (nint)(nuint)nativeWindow.X11.Value.Window);
            }
        }

        public void PollEvents()
        {
            glfw.SetWindowTitle(window, Title + $" - Artifact Engine ({_app.FPS:0.0} FPS){TitleSuffix}");
            glfw.PollEvents();
        }

        public bool ShouldClose()
        {
            return glfw.WindowShouldClose(window);
        }

        public bool IsKeyDown(Key key)
        {
            if (key == Key.Any)
            {
                return IsKeyDown(Key.A) ||
                        IsKeyDown(Key.B) ||
                        IsKeyDown(Key.C) ||
                        IsKeyDown(Key.D) ||
                        IsKeyDown(Key.E) ||
                        IsKeyDown(Key.F) ||
                        IsKeyDown(Key.G) ||
                        IsKeyDown(Key.H) ||
                        IsKeyDown(Key.I) ||
                        IsKeyDown(Key.J) ||
                        IsKeyDown(Key.K) ||
                        IsKeyDown(Key.L) ||
                        IsKeyDown(Key.M) ||
                        IsKeyDown(Key.N) ||
                        IsKeyDown(Key.O) ||
                        IsKeyDown(Key.P) ||
                        IsKeyDown(Key.Q) ||
                        IsKeyDown(Key.R) ||
                        IsKeyDown(Key.S) ||
                        IsKeyDown(Key.T) ||
                        IsKeyDown(Key.U) ||
                        IsKeyDown(Key.V) ||
                        IsKeyDown(Key.W) ||
                        IsKeyDown(Key.X) ||
                        IsKeyDown(Key.Y) ||
                        IsKeyDown(Key.Z);
            }

            Keys glfwKey;

            switch (key)
            {
                case Key.A: glfwKey = Keys.A; break;
                case Key.B: glfwKey = Keys.B; break;
                case Key.C: glfwKey = Keys.C; break;
                case Key.D: glfwKey = Keys.D; break;
                case Key.E: glfwKey = Keys.E; break;
                case Key.F: glfwKey = Keys.F; break;
                case Key.G: glfwKey = Keys.G; break;
                case Key.H: glfwKey = Keys.H; break;
                case Key.I: glfwKey = Keys.I; break;
                case Key.J: glfwKey = Keys.J; break;
                case Key.K: glfwKey = Keys.K; break;
                case Key.L: glfwKey = Keys.L; break;
                case Key.M: glfwKey = Keys.M; break;
                case Key.N: glfwKey = Keys.N; break;
                case Key.O: glfwKey = Keys.O; break;
                case Key.P: glfwKey = Keys.P; break;
                case Key.Q: glfwKey = Keys.Q; break;
                case Key.R: glfwKey = Keys.R; break;
                case Key.S: glfwKey = Keys.S; break;
                case Key.T: glfwKey = Keys.T; break;
                case Key.U: glfwKey = Keys.U; break;
                case Key.V: glfwKey = Keys.V; break;
                case Key.W: glfwKey = Keys.W; break;
                case Key.X: glfwKey = Keys.X; break;
                case Key.Y: glfwKey = Keys.Y; break;
                case Key.Z: glfwKey = Keys.Z; break;
                default: throw new Exception("Key is not supported on GLFW: " + key);
            }

            return glfw.GetKey(window, glfwKey) == 1;
        }
    }
}
