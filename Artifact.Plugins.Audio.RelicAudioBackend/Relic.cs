using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RelicAudio
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RelicContext
    {
        internal IntPtr device;
        internal IntPtr context;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RelicClip
    {
        internal uint buffer;
        internal uint source;
    }

    public class Relic
    {
        [DllImport("RelicAudio-Shared", EntryPoint = "relicdyn_context_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern RelicContext CreateContext();

        [DllImport("RelicAudio-Shared", EntryPoint = "relicdyn_context_destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DestroyContext(RelicContext context);

        [DllImport("RelicAudio-Shared", EntryPoint = "relicdyn_clip_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern RelicClip CreateClip(string filename);

        [DllImport("RelicAudio-Shared", EntryPoint = "relicdyn_clip_play", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PlayClip(RelicClip clip, bool wait = true);

        [DllImport("RelicAudio-Shared", EntryPoint = "relicdyn_clip_is_playing", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool ClipIsPlaying(RelicClip clip);

        [DllImport("RelicAudio-Shared", EntryPoint = "relicdyn_clip_get_location", CallingConvention = CallingConvention.Cdecl)]
        public static extern float ClipGetLocation(RelicClip clip);
    }
}
