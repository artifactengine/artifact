using Artifact.Plugins.Rendering;
using Artifact.Plugins.Rendering.VeldridBackend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Artifact.Plugins.ImGui
{
    public class ImGuiPlugin : PluginBase
    {
        private RenderingPlugin renderer;

        private ImGuiRenderer imguiVk;

        public ImGuiPlugin(Application app) : base(app)
        {
            renderer = app.GetPlugin<RenderingPlugin>();
        }

        public override void OnLoad()
        {
            base.OnLoad();

            if (renderer.Backend.FancyName == "Vulkan")
            {
                imguiVk = new ImGuiRenderer(VeldridRenderingBackend.device, new OutputDescription(), 1280, 720);



            }
        }

        public void Draw()
        {
            imguiVk.Update(1 / 60f, new InputSnapshot());
            imguiVk.Render(VeldridRenderingBackend.device, VeldridRenderingBackend.commandList);

        }
    }
}