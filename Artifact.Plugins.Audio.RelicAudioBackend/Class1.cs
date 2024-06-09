using Artifact.Plugin.Audio;
using RelicAudio;
using System.IO;
using System.Runtime.InteropServices;

namespace Artifact.Plugins.Audio.RelicAudioBackend
{
    public class RelicAudioBackend : ArtifactDisposable, IAudioBackend
    {
        Dictionary<string, RelicClip> clipCache = new Dictionary<string, RelicClip>();

        RelicContext context;

        public RelicAudioBackend()
        {
            string path = string.Empty;

            if (RuntimeInformation.ProcessArchitecture == Architecture.X86)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    path = "./runtimes/win-x86/native/";

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    path = "./runtimes/linux-x86/native/";
            }
            else if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    path = "./runtimes/win-x64/native/";

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    path = "./runtimes/linux-x64/native/";
            } else
            {
                throw new PlatformNotSupportedException("Architecture " + RuntimeInformation.ProcessArchitecture + " is not supported by RelicAudio. RelicAudio only supports x86 and x64");
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Environment.SetEnvironmentVariable("Path", Environment.GetEnvironmentVariable("Path") + ";" + Path.GetFullPath(path));
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ":" + Path.GetFullPath(path));
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ":" + Path.GetFullPath(path));

            context = Relic.CreateContext();
        }

        public override void Dispose()
        {
            Relic.DestroyContext(context);
        }

        public void PlayWav(string path)
        {
            if (clipCache.ContainsKey(path))
            {
                Relic.PlayClip(clipCache[path], false);
            } else
            {
                clipCache.Add(path, Relic.CreateClip(path));
                Relic.PlayClip(clipCache[path], false);
            }
        }

        public void PlayWavAndWait(string path)
        {
            if (clipCache.ContainsKey(path))
            {
                Relic.PlayClip(clipCache[path]);
            }
            else
            {
                clipCache.Add(path, Relic.CreateClip(path));
                Relic.PlayClip(clipCache[path], false);
            }
        }
    }
}
