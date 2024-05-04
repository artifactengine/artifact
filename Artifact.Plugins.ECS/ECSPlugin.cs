using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugins.ECS
{
    public class ECSPlugin : PluginBase
    {
        public ECSPlugin(Application app) : base(app)
        {
        }

        public override void OnLoad()
        {
            SceneManager.CurrentScene.OnLoad();
            base.OnLoad();
        }

        public override void OnUpdate(float dt)
        {
            SceneManager.CurrentScene.OnUpdate(dt);
            base.OnUpdate(dt);
        }

        public override void OnExit()
        {
            SceneManager.CurrentScene.OnExit();
            base.OnExit();
        }

        public override void Invoke(string evnt, params object[] args)
        {
            SceneManager.CurrentScene.Invoke(evnt, args);
        }
    }
}
