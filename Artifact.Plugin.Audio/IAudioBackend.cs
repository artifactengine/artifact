using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugin.Audio
{
    public interface IAudioBackend
    {
        public void PlayWav(string path);
        public void PlayWavAndWait(string path);
    }
}
