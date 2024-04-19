using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Silk.NET.GLFW;

namespace Artifact.Plugins.Windowing.GLFWBackend
{
    public unsafe class GLFWWindowingBackend : IWindowingBackend
    {
        public nint WindowHandle => nativeWindow.Win32.Value.Hwnd;

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

        public void CreateWindow(string title, int width, int height, Application app)
        {
            _app = app;
            Title = title;

            glfw = Glfw.GetApi();
            glfw.Init();
            glfw.WindowHint(WindowHintClientApi.ClientApi, ClientApi.NoApi);
            glfw.WindowHint(WindowHintBool.Resizable, false);

            window = glfw.CreateWindow(width, height, Title + " - Artifact Engine", null, null);
            nativeWindow = new GlfwNativeWindow(glfw, window);
            logger.Info("Created window using GLFW backend");
        }

        public void PollEvents()
        {
            glfw.SetWindowTitle(window, Title + $" - Artifact Engine ({_app.FPS} FPS){TitleSuffix}");
            glfw.PollEvents();
        }

        public bool ShouldClose()
        {
            return glfw.WindowShouldClose(window);
        }
    }
}
