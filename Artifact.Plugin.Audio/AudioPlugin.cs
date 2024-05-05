using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugin.Audio
{
    public class AudioPlugin : PluginBase
    {
        public IAudioBackend Backend { get; set; }

        public AudioPlugin(Application app, Type backend) : base(app)
        {
            Backend = Activator.CreateInstance(backend) as IAudioBackend;
        }

        public void PlayWav(string path)
        {
            Backend.PlayWav(path);
        }

        public void PlayWavAndWait(string path)
        {
            Backend.PlayWavAndWait(path);
        }
    }
}
