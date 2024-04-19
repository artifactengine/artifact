using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugins.Rendering
{
    public abstract class Visual
    {
        private Mesh _mesh;

        public Visual(Mesh mesh)
        {
            _mesh = mesh;
        }

        public abstract void Draw();
        public abstract void Dispose();
    }
}
