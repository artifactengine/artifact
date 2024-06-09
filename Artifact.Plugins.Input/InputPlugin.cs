using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artifact.Plugins.Windowing;

namespace Artifact.Plugins.Input
{
    public class InputPlugin : PluginBase
    {
        private WindowingPlugin windowing;

        private Dictionary<Key, bool> previousKeyStates = new Dictionary<Key, bool>();

        public InputPlugin(Application app) : base(app)
        {
            BundleID = "com.artifact.plugins.input";

            windowing = app.GetPlugin<WindowingPlugin>();
        }

        public bool IsKeyDown(Key key)
        {
            return windowing.Backend.IsKeyDown(key);
        }

        public bool IsKeyUp(Key key)
        {
            return !windowing.Backend.IsKeyDown(key);
        }

        public bool WasKeyPressedThisFrame(Key key)
        {
            bool isKeyDown = IsKeyUp(key);
            bool wasKeyDown = previousKeyStates.ContainsKey(key) && previousKeyStates[key];

            // Update the previous state for the next frame
            previousKeyStates[key] = isKeyDown;

            // Return true if the key was down in the previous frame and is up in the current frame
            return wasKeyDown && !isKeyDown;
        }
    }
}
