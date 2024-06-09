using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using X11;

namespace Artifact.Plugins.Windowing
{
    public class WindowingPlugin : PluginBase
    {
        public string Title { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public IWindowingBackend Backend { get; set; }

        public const int WM_SETICON = 0x0080;
        public const int ICON_SMALL = 0;
        public const int IMAGE_ICON = 1;
        public const uint LR_LOADFROMFILE = 0x00000010;

        public WindowingPlugin(Application app, string title, int width, int height, Type backend) : base(app)
        {


            if (!backend.IsAssignableTo(typeof(IWindowingBackend)))
            {
                throw new WindowingPluginException("Invalid Windowing backend " + backend.Name);
            }

            Backend = Activator.CreateInstance(backend) as IWindowingBackend;

            Title = title;
            Width = width;
            Height = height;

            BundleID = "com.artifact.plugins.windowing";
        }

        public override void OnLoad()
        {
            Backend.CreateWindow(Title, Width, Height, Application);
            
        }

        public override void OnUpdate(float dt)
        {
            if (Backend.ShouldClose())
            {
                Application.IsOpen = false;
                
            }

            Backend.PollEvents();
        }
    }
}
