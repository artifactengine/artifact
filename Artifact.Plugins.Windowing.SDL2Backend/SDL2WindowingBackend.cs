using Artifact.Plugins.Input;
using NLog;
using SDL_Sharp;
using System.Runtime.InteropServices;
using Veldrid.Sdl2;

namespace Artifact.Plugins.Windowing.SDL2Backend
{
    public class SDL2WindowingBackend : IWindowingBackend
    {
        private Application _app;

        private Sdl2Window window;
        private bool shouldClose = false;

        public Logger logger = LogManager.GetCurrentClassLogger();

        

        public string Title { get; set; } = "";
        public string TitleSuffix { get; set; } = "";

        public object WindowHandle => window.Handle;

        public void CreateWindow(string title, int width, int height, Application app)
        {
            _app = app;
            Title = title;
            window = new Sdl2Window(Title + " - Artifact Engine", 500, 500, width, height, SDL_WindowFlags.Shown, true);
            
            window.Closed += Window_Closing;

            logger.Info("Created Window using SDL2 Backend");
        }

        public void PollEvents()
        {
            window.PumpEvents();
        }

        private void Window_Closing()
        {
            shouldClose = true;
            
        }

        public bool ShouldClose()
        {
            window.Title = Title + $" - Artifact Engine ({_app.FPS} FPS)" + TitleSuffix;
            return shouldClose;
        }

        public void Close()
        {
            window.Close();
            logger.Info("SDL2 window closed");
        }

        public bool IsKeyDown(Key key)
        {
            throw new NotImplementedException();
        }
    }
}
