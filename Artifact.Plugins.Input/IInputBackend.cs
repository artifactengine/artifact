using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugins.Input
{
    public interface IInputBackend
    {
        public bool WasKeyPressedThisFrame(Key key);
        public bool IsKeyDown(Key key);
        public bool IsKeyUp(Key key);
    }
}
