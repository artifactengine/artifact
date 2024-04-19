using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugins.Input
{
    public class InputPlugin : PluginBase
    {
        private IInputBackend backend;

        public InputPlugin(Application app, Type backendType) : base(app)
        {
            BundleID = "com.artifact.plugins.input";

            if (!backendType.GetInterfaces().Contains(typeof(IInputBackend)))
            {
                throw new Exception("Invalid input backend " + backendType.Name);
            }

            backend = (IInputBackend)Activator.CreateInstance(backendType);
        }

        public bool IsKeyDown(Key key)
        {
            return backend.IsKeyDown(key);
        }

        public bool IsKeyUp(Key key)
        {
            return backend.IsKeyUp(key);
        }

        public bool WasKeyPressedThisFrame(Key key)
        {
            return backend.WasKeyPressedThisFrame(key);
        }
    }
}
