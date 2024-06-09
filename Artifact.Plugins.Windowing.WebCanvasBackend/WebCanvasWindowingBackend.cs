using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices.JavaScript;
using Artifact.WASM.Common;
using Artifact.Plugins.Input;

namespace Artifact.Plugins.Windowing.WebCanvasBackend
{
    public class WebCanvasWindowingBackend : IWindowingBackend
    {
        public object WindowHandle => canvas;

        public string Title { get; set; }
        public string TitleSuffix { get; set; }

        private JSObject canvas;

        private Application app;

        public void Close()
        {
            
        }

        public void CreateWindow(string title, int width, int height, Application app)
        {
            Title = title;

            this.app = app;
        }

        public void PollEvents()
        {
            WASMCommon.current.SetFPSMarker(app.FPS.ToString("F3"));
        }

        public bool ShouldClose()
        {
            return false;
        }

        public bool IsKeyDown(Key key)
        {
            throw new NotImplementedException();
        }
    }
}
