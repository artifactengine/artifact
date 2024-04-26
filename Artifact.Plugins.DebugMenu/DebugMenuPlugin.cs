using Artifact.Plugins.Input;
using Artifact.Plugins.Rendering;
using Artifact.Plugins.Rendering.Font;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugins.DebugMenu
{
    public class DebugMenuPlugin : PluginBase, IRenderingGameLoopExtention
    {
        private bool show = false;

        private InputPlugin input;

        private FontRenderer fontRenderer;

        public DebugMenuPlugin(Application app) : base(app)
        {
            input = app.GetPlugin<InputPlugin>();
        }

        public override void OnLoad()
        {
            base.OnLoad();
            fontRenderer = new FontRenderer("C:\\Windows\\Fonts\\CascadiaCode.ttf", 32);
        }

        public void OnDraw()
        {
            if (input.WasKeyPressedThisFrame(Key.F3))
            {
                show = !show;
            }

            if (show)
            {
                fontRenderer.DrawStringShadowed("Artifact Engine v1.0", new ColorRGB(255, 255, 255, 255), new ColorRGB(42, 42, 42, 255), new Vector3(-0.88f, 0.47f, -4));
                fontRenderer.DrawStringShadowed("FPS: " + Application.FPS, new ColorRGB(255, 255, 255, 255), new ColorRGB(42, 42, 42, 255), new Vector3(-0.88f, 0.435f, -4));
                fontRenderer.DrawStringShadowed("Rendering Backend: " + Application.GetPlugin<RenderingPlugin>().Backend.FancyName, new ColorRGB(255, 255, 255, 255), new ColorRGB(42, 42, 42, 255), new Vector3(-0.88f, 0.4f, -4));
                //fontRenderer.DrawString("FPS: " + Application.FPS, new ColorRGB(255, 255, 255, 255), new Vector3(-0.87f, 0.42f, -4));
                //fontRenderer.DrawString("Renderer: " + Application.GetPlugin<RenderingPlugin>().Backend.FancyName, new ColorRGB(255, 255, 255, 255), new Vector3(-0.87f, 0.42f, -4));
            }
        }
    }
}
