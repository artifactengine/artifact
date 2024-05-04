using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artifact;

namespace Artifact.Plugins.ECS
{
    public abstract class Component : GameLoop
    {
        public Entity Entity { get; set; }
        public Transform Transform => Entity.Transform;
    }
}
