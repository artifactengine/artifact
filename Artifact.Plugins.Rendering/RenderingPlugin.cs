using Artifact.Plugins.Windowing;
using NLog.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugins.Rendering
{
    public enum SamplerMode
    {
        Smooth,
        PixelArt
    }

    public class RenderingPlugin : PluginBase
    {
        public IRenderingBackend Backend { get; set; }

        public SamplerMode SamplerMode { get; set; } = SamplerMode.PixelArt;

        public RenderingPlugin(Application app, Type backend) : base(app)
        {
            BundleID = "com.artifact.plugins.rendering";

            if (!backend.GetInterfaces().Contains(typeof(IRenderingBackend)))
            {
                throw new RenderingException($"Invalid Rendering backend {backend.Name}");
            }

            Backend = (Activator.CreateInstance(backend) as IRenderingBackend)!;
        
            
        }

        public IVisual CreateVisual(Mesh mesh)
        {
            try
            {
                return (Activator.CreateInstance(Backend.VisualImplementation, mesh) as IVisual)!;
            } catch (TargetInvocationException e)
            {
                throw e.InnerException!;
            }
            
        }

        public override void OnLoad()
        {
            base.OnLoad();

            WindowingPlugin windowing = Application.GetPlugin<WindowingPlugin>();

            Backend.CreateContext(windowing.Backend.WindowHandle, windowing.Width, windowing.Height);

            windowing.Backend.TitleSuffix = " (" + Backend.FancyName + ")";

            new Debug();
        }

        public override void OnExit()
        {
            base.OnExit();
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
            Camera.UpdateMatrices();
            Backend.Predraw();
            try
            {
                Application.Invoke("rendering:OnDraw");

                foreach (PluginBase plugin in Application.plugins)
                {
                    plugin.Invoke("rendering:OnDraw");
                }
            } catch (Exception exec)
            {
                if (!exec.Message.ToLower().Contains("surface has been lost"))
                {
                    throw exec;
                }
            }
            SwapBuffers();
            
            base.OnUpdate(dt);
        }
    }
}
