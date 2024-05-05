using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Box2D.NetStandard;
using Box2D.NetStandard.Dynamics.World;
using System.Numerics;

namespace Artifact.Physics.Box2D
{
    public class PhysicsPlugin : PluginBase
    {
        internal static World world;

        public PhysicsPlugin(Application app) : base(app)
        {
            world = new World(new Vector2(0, -9.807f));
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);
            world.Step(1.0f / 60.0f, 8, 2);
        }
    }
}
