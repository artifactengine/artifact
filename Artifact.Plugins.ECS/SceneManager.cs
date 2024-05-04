using Artifact.Plugins.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugins.ECS
{
    public class Scene
    {
        public List<Entity> Entities { get; set; } = new List<Entity>();
        public ColorRGB Background { get; set; } = new ColorRGB(255, 255, 255, 255);

        public void OnLoad()
        {
            foreach (var entity in Entities)
            {
                entity.OnLoad();
            }
        }

        public void OnExit()
        {
            foreach (var entity in Entities)
            {
                entity.OnExit();
            }
        }

        public void OnUpdate(float dt)
        {
            foreach (var entity in Entities)
            {
                entity.OnUpdate(dt);
            }
        }

        public void Invoke(string evnt, params object[] args)
        {
            if (evnt == "rendering:OnDraw")
            {
                Application.current.GetPlugin<RenderingPlugin>().Clear(Background, 255);
            }
            foreach (var entity in Entities)
            {
                entity.Invoke(evnt, args);
            }
        }
    }

    public static class SceneManager
    {
        public static Scene CurrentScene { get; set; }
    }
}
