using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugins.Windowing
{
    public class WindowingPlugin : PluginBase
    {
        public string Title { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public IWindowingBackend Backend { get; set; }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr LoadImage(IntPtr hInstance, string lpszName, uint uType, int cxDesired, int cyDesired, uint fuLoad);

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

            IntPtr hIcon = LoadImage(IntPtr.Zero, "Assets/icon.ico", IMAGE_ICON, 256, 256, LR_LOADFROMFILE);
            SendMessage(Backend.WindowHandle, WM_SETICON, ICON_SMALL, hIcon);
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
