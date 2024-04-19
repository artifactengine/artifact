using Artifact.Plugins.Windowing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugins.Rendering
{
    public class RenderingPlugin : PluginBase
    {
        private IRenderingBackend Backend { get; set; }

        public RenderingPlugin(Application app, Type backend) : base(app)
        {
            BundleID = "com.artifact.plugins.rendering";

            if (!backend.GetInterfaces().Contains(typeof(IRenderingBackend)))
            {
                throw new RenderingException($"Invalid Rendering backend {backend.Name}");
            }

            Backend = (Activator.CreateInstance(backend) as IRenderingBackend)!;
        }

        public Visual CreateVisual(Mesh mesh)
        {
            return (Activator.CreateInstance(Backend.VisualImplementation, mesh) as Visual)!;
        }

        public override void OnLoad()
        {
            base.OnLoad();

            WindowingPlugin windowing = Application.GetPlugin<WindowingPlugin>();

            Backend.CreateContext(windowing.Backend.WindowHandle, windowing.Width, windowing.Height);

            windowing.Backend.TitleSuffix = " (" + Backend.FancyName + ")";
        }

        public override void OnExit()
        {
            base.OnExit();

            Backend.DisposeContext();
        }

        public void SwapBuffers()
        {
            Backend.SwapBuffers();
        }

        public void Clear(ColorRGB color, float alpha)
        {
            Backend.Clear(color, alpha);
        }

        public override void OnUpdate(float dt)
        {
            Application.Invoke("rendering:OnDraw");
            base.OnUpdate(dt);
        }
    }
}
