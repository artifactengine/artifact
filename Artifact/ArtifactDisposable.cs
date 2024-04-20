using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifact
{
    public abstract class ArtifactDisposable
    {
        public ArtifactDisposable()
        {
            Application.Disposables.Add(this);
        }

        public abstract void Dispose();
    }
}
