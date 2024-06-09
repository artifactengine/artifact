using Artifact.Plugins.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugins.Windowing
{
    public interface IWindowingBackend
    {
        public object WindowHandle { get; }
        public string Title { get; set; }
        public string TitleSuffix { get; set; }

        public void CreateWindow(string title, int width, int height, Application app);

        public bool ShouldClose();

        public void PollEvents();

        public void Close();

        public bool IsKeyDown(Key key);
    }
}
