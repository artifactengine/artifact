using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugins.Input.PollBackend
{
    public class PollingInputBackend : IInputBackend
    {
        [DllImport("User32.dll")]
        public static extern int GetAsyncKeyState(Key key);

        public bool IsKeyDown(Key key)
        {
            return GetAsyncKeyState(key) > 0;
        }

        public bool IsKeyUp(Key key)
        {
            return !(GetAsyncKeyState(key) > 0);
        }

        private Dictionary<Key, bool> previousKeyStates = new Dictionary<Key, bool>();

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
