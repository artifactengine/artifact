using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugins.Rendering
{
    public interface IVisual
    {
        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; }
        public Vector3 Rotation { get; set; }

        public void Draw();
    }
}
