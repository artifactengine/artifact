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
            if (key == Key.Any)
            {
                return WasKeyPressedThisFrame(Key.Q) || WasKeyPressedThisFrame(Key.W) || WasKeyPressedThisFrame(Key.E) || WasKeyPressedThisFrame(Key.R) || WasKeyPressedThisFrame(Key.T) || WasKeyPressedThisFrame(Key.Y) || WasKeyPressedThisFrame(Key.U) || WasKeyPressedThisFrame(Key.I) || WasKeyPressedThisFrame(Key.O) || WasKeyPressedThisFrame(Key.P) ||
                       WasKeyPressedThisFrame(Key.A) || WasKeyPressedThisFrame(Key.S) || WasKeyPressedThisFrame(Key.D) || WasKeyPressedThisFrame(Key.F) || WasKeyPressedThisFrame(Key.G) || WasKeyPressedThisFrame(Key.H) || WasKeyPressedThisFrame(Key.J) || WasKeyPressedThisFrame(Key.K) || WasKeyPressedThisFrame(Key.L) ||
                       WasKeyPressedThisFrame(Key.Z) || WasKeyPressedThisFrame(Key.X) || WasKeyPressedThisFrame(Key.C) || WasKeyPressedThisFrame(Key.V) || WasKeyPressedThisFrame(Key.B) || WasKeyPressedThisFrame(Key.N) || WasKeyPressedThisFrame(Key.M);
            } else
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
}
